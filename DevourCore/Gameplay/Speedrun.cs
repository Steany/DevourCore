using HarmonyLib;
using Il2Cpp;
using Il2CppHorror;
using Il2CppOpsive.UltimateCharacterController.Character;
using Il2CppOpsive.UltimateCharacterController.Integrations.Rewired;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using GameMenu = Il2CppHorror.Menu;
using Component = UnityEngine.Component;

namespace DevourCore
{
	public class Speedrun
	{
		internal bool disableLongInteract = true;
		private bool enableAtticSpawn = true;
		private bool teleported = false;
		private readonly Vector3 targetPosition = new Vector3(19.95f, 6.36f, -21.84f);
		private GameMenu cachedMenu = null;
		private RewiredInput cachedInput = null;
		private float sceneLoadTime = -1f;
		private float nextAtticSpawnTryTime = 0f;
		private const float searchThrottle = 1f;
		private float lastSearchTime = 0f;
		internal bool isHoldingInteract = false;
		private KeyCode customInteractKey = KeyCode.E;
		private bool isCapturingInteractKey = false;

		private bool forceStartEnabled = false;
		private float forceStartDelay = 1.5f;
		private bool forceStartTriggered = false;
		private bool wasInGame = false;
		private float lobbyReturnTime = -1f;

		private bool useArmingWindow = true;

		private bool forceStartArmed = false;
		private float forceStartExpireAt = -1f;

		private const float MIN_ARM_MINUTES = 2f;
		private const float MAX_ARM_MINUTES = 10f;
		private float forceStartArmMinutes = MIN_ARM_MINUTES;

		private MelonPreferences_Entry<bool> prefDisableLongInteract;
		private MelonPreferences_Entry<bool> prefEnableAtticSpawn;
		private MelonPreferences_Entry<KeyCode> prefInteractKey;
		private MelonPreferences_Entry<bool> prefForceStartEnabled;
		private MelonPreferences_Entry<float> prefForceStartDelay;
		private MelonPreferences_Entry<float> prefForceStartArmMinutes;
		private MelonPreferences_Entry<bool> prefUseArmingWindow;

		private MelonPreferences_Category prefs;

		private bool farmhousePatchesInstalled = false;
		private HarmonyLib.Harmony sharedHarmony = null;

		private const string FARMHOUSE_SCENE = "Devour";
		private const string BACK_DOOR_NAME = "Back Door";

		private struct DoorSnap
		{
			public bool WasOpen;
			public bool WasRight;
			public string Name;
		}

		private readonly Dictionary<int, DoorSnap> doorSnap = new Dictionary<int, DoorSnap>(256);
		private float guardActiveUntil = -9999f;
		private const float DEFAULT_GUARD_SECONDS = 15f;

		private static readonly HashSet<int> Reopening = new HashSet<int>();
		private readonly HashSet<int> forceLocked = new HashSet<int>();
		private readonly HashSet<int> playerUnlocked = new HashSet<int>();

		private static Speedrun _instForPatches;

		public void InstallFarmhouseDoorPatches(HarmonyLib.Harmony harmony)
		{
			if (farmhousePatchesInstalled) return;
			farmhousePatchesInstalled = true;
			sharedHarmony = harmony;
			_instForPatches = this;

			try
			{
				var annaIntro = AccessTools.Method(typeof(AnnaIntro), "OnTriggerEntered");
				if (annaIntro != null)
					harmony.Patch(annaIntro, prefix: new HarmonyMethod(typeof(Speedrun), nameof(AnnaIntro_OnTriggerEntered_Prefix)));
			}
			catch { }

			try
			{
				var onOpenLeft = AccessTools.Method(typeof(DoorBehaviour), "OnOpenLeft");
				if (onOpenLeft != null)
					harmony.Patch(onOpenLeft, postfix: new HarmonyMethod(typeof(Speedrun), nameof(DoorBehaviour_OnOpenLeft_Postfix)));
			}
			catch { }

			try
			{
				var onOpenRight = AccessTools.Method(typeof(DoorBehaviour), "OnOpenRight");
				if (onOpenRight != null)
					harmony.Patch(onOpenRight, postfix: new HarmonyMethod(typeof(Speedrun), nameof(DoorBehaviour_OnOpenRight_Postfix)));
			}
			catch { }

			try
			{
				var isLocked = AccessTools.Method(typeof(DoorBehaviour), "IsLocked");
				if (isLocked != null)
					harmony.Patch(isLocked, prefix: new HarmonyMethod(typeof(Speedrun), nameof(DoorBehaviour_IsLocked_Prefix)));
			}
			catch { }

			try
			{
				var unlock = AccessTools.Method(typeof(DoorBehaviour), "Unlock", Type.EmptyTypes);
				if (unlock != null)
				{
					harmony.Patch(unlock,
						prefix: new HarmonyMethod(typeof(Speedrun), nameof(DoorBehaviour_Unlock_Prefix)),
						postfix: new HarmonyMethod(typeof(Speedrun), nameof(DoorBehaviour_Unlock_Postfix)));
				}
			}
			catch { }

			TryPatchDoorInteract(harmony);
		}

		private bool FarmhouseDoorBugEnabled => enableAtticSpawn;

		private bool InFarmhouseScene => string.Equals(lastScene, FARMHOUSE_SCENE, StringComparison.OrdinalIgnoreCase);

		private static bool IsBackDoor(DoorBehaviour d)
		{
			try
			{
				if (d == null || d.gameObject == null) return false;
				return string.Equals(d.gameObject.name, BACK_DOOR_NAME, StringComparison.OrdinalIgnoreCase);
			}
			catch { }
			return false;
		}

		private DoorBehaviour FindBackDoor()
		{
			try
			{
				var doors = UnityEngine.Object.FindObjectsOfType<DoorBehaviour>(true);
				if (doors == null) return null;
				foreach (var d in doors)
					if (IsBackDoor(d)) return d;
			}
			catch { }
			return null;
		}

		private void ForceBackDoorOnce()
		{
			var d = FindBackDoor();
			if (d == null) return;

			int id = d.gameObject.GetInstanceID();
			forceLocked.Remove(id);
			playerUnlocked.Add(id);

			try { d.Unlock(); } catch { }
			try { d.SetState("Locked", false); } catch { }
			try { d.SetState("IsLocked", false); } catch { }
		}

		private IEnumerator ForceBackDoorForSeconds(float seconds)
		{
			float end = Time.realtimeSinceStartup + Mathf.Clamp(seconds, 1f, 20f);
			while (Time.realtimeSinceStartup < end)
			{
				ForceBackDoorOnce();
				yield return new WaitForSeconds(0.5f);
			}
		}

		private void ArmFarmhouseDoorGuard(Component source)
		{
			if (!FarmhouseDoorBugEnabled) return;
			if (!InFarmhouseScene) return;

			guardActiveUntil = Time.realtimeSinceStartup + DEFAULT_GUARD_SECONDS;
			doorSnap.Clear();
			forceLocked.Clear();
			playerUnlocked.Clear();

			var doors = UnityEngine.Object.FindObjectsOfType<DoorBehaviour>(true);
			if (doors != null)
			{
				for (int i = 0; i < doors.Length; i++)
				{
					var d = doors[i];
					if (d == null || d.gameObject == null) continue;

					bool open = false;
					bool right = false;

					try
					{
						bool oR = d.IsOpenRight();
						bool oL = d.IsOpenLeft();
						open = oR || oL;
						right = oR;
					}
					catch
					{
						try { open = d.IsOpen(); } catch { }
					}

					doorSnap[d.gameObject.GetInstanceID()] = new DoorSnap
					{
						WasOpen = open,
						WasRight = right,
						Name = d.gameObject.name ?? "<door>"
					};
				}
			}

			MelonCoroutines.Start(ForceBackDoorForSeconds(4f));
		}



		private IEnumerator ResetAnimatorSpeed(Animator anim, float oldSpeed, float delay)
		{
			yield return new WaitForSeconds(Mathf.Clamp(delay, 0.05f, 2f));
			try
			{
				if (anim != null) anim.speed = oldSpeed;
			}
			catch { }
		}

		private bool ShouldGuardNow()
		{
			if (!FarmhouseDoorBugEnabled) return false;
			if (!InFarmhouseScene) return false;
			return Time.realtimeSinceStartup <= guardActiveUntil;
		}

		private bool TryGetSnap(DoorBehaviour door, out DoorSnap s)
		{
			s = default;
			if (door == null || door.gameObject == null) return false;
			return doorSnap.TryGetValue(door.gameObject.GetInstanceID(), out s);
		}

		private bool IsDoorOpenNow(DoorBehaviour door)
		{
			if (door == null) return false;
			try
			{
				bool oR = door.IsOpenRight();
				bool oL = door.IsOpenLeft();
				return oR || oL;
			}
			catch
			{
				try { return door.IsOpen(); } catch { }
			}
			return false;
		}

		private void MarkForceLocked(DoorBehaviour door)
		{
			if (door == null || door.gameObject == null) return;
			if (IsBackDoor(door)) return;

			int id = door.gameObject.GetInstanceID();

			bool locked = false;
			string key = "";
			try { locked = door.IsLocked(); } catch { }
			try { key = door.KeyName(); } catch { }

			if (locked || !string.IsNullOrEmpty(key))
				forceLocked.Add(id);
		}

		private bool ShouldForceLocked(DoorBehaviour door)
		{
			if (door == null || door.gameObject == null) return false;
			if (IsBackDoor(door)) return false;
			int id = door.gameObject.GetInstanceID();
			if (!forceLocked.Contains(id)) return false;
			if (playerUnlocked.Contains(id)) return false;
			return true;
		}

		private void MarkPlayerUnlocked(DoorBehaviour door)
		{
			if (door == null || door.gameObject == null) return;
			if (IsBackDoor(door)) return;

			int id = door.gameObject.GetInstanceID();
			if (!forceLocked.Contains(id)) return;

			playerUnlocked.Add(id);
		}

		private bool IsReopeningDoor(DoorBehaviour door)
		{
			if (door == null || door.gameObject == null) return false;
			return Reopening.Contains(door.gameObject.GetInstanceID());
		}

		private void ForceReopenIfNeeded(DoorBehaviour door)
		{
			if (!ShouldGuardNow()) return;
			if (!TryGetSnap(door, out var s)) return;
			if (!s.WasOpen) return;

			int id = door.gameObject.GetInstanceID();
			if (Reopening.Contains(id)) return;
			if (IsDoorOpenNow(door)) return;

			MarkForceLocked(door);


			Reopening.Add(id);
			try
			{
				try
				{
					door.SetState("OpenRight", s.WasRight);
				}
				catch { }

				try
				{
					door.SetState("OpenLeft", !s.WasRight);
				}
				catch { }

				try
				{
					door.SetState("Open", true);
				}
				catch { }

				if (!IsDoorOpenNow(door))
				{
					try
					{
						Animator anim = null;
						float oldSpeed = 1f;

						try
						{
							anim = door.GetComponentInChildren<Animator>();
							if (anim != null)
							{
								oldSpeed = anim.speed;
								anim.speed = 8f;
								MelonCoroutines.Start(ResetAnimatorSpeed(anim, oldSpeed, 0.25f));
							}
						}
						catch { }

						door.OpenDoor(s.WasRight);
					}
					catch { }
				}
			}
			finally
			{
				Reopening.Remove(id);
			}
		}

		private bool TryConsumeInteractionToClose(DoorBehaviour door)
		{
			if (!FarmhouseDoorBugEnabled) return false;
			if (!InFarmhouseScene) return false;
			if (door == null || door.gameObject == null) return false;
			if (IsBackDoor(door)) return false;

			if (!ShouldForceLocked(door)) return false;
			if (!IsDoorOpenNow(door)) return false;

			bool closed = false;
			try
			{
				var m1 = AccessTools.Method(typeof(DoorBehaviour), "CloseDoor", new[] { typeof(bool) });
				if (m1 != null)
				{
					bool wasRight = false;
					try { wasRight = door.IsOpenRight(); } catch { }
					m1.Invoke(door, new object[] { wasRight });
					closed = true;
				}
				else
				{
					var m0 = AccessTools.Method(typeof(DoorBehaviour), "CloseDoor", Type.EmptyTypes);
					if (m0 != null)
					{
						m0.Invoke(door, null);
						closed = true;
					}
				}
			}
			catch { }

			if (!closed)
			{
				try { door.SetState("OpenRight", false); } catch { }
				try { door.SetState("OpenLeft", false); } catch { }
				try { door.SetState("Open", false); } catch { }
			}

			return true;
		}

		private void TryPatchDoorInteract(HarmonyLib.Harmony h)
		{
			try
			{
				var t = typeof(DoorBehaviour);
				var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(m => m.Name == "Interact" || m.Name == "OnInteract" || m.Name == "Use")
					.OrderBy(m => m.GetParameters().Length)
					.ToArray();

				MethodInfo target = methods.FirstOrDefault(m => m.GetParameters().Length <= 2);
				if (target == null) return;

				var prefix = new HarmonyMethod(typeof(Speedrun), nameof(DoorInteract_Prefix));
				h.Patch(target, prefix: prefix);
			}
			catch { }
		}

		private static void AnnaIntro_OnTriggerEntered_Prefix(AnnaIntro __instance, int part)
		{
			try { _instForPatches?.ArmFarmhouseDoorGuard(__instance); } catch { }
		}

		private static void DoorBehaviour_OnOpenLeft_Postfix(DoorBehaviour __instance)
		{
			try { _instForPatches?.ForceReopenIfNeeded(__instance); } catch { }
		}

		private static void DoorBehaviour_OnOpenRight_Postfix(DoorBehaviour __instance)
		{
			try { _instForPatches?.ForceReopenIfNeeded(__instance); } catch { }
		}

		private static bool DoorBehaviour_IsLocked_Prefix(DoorBehaviour __instance, ref bool __result)
		{
			try
			{
				var inst = _instForPatches;
				if (inst == null) return true;
				if (!inst.FarmhouseDoorBugEnabled || !inst.InFarmhouseScene) return true;

				if (IsBackDoor(__instance))
				{
					__result = false;
					return false;
				}

				if (inst.ShouldForceLocked(__instance))
				{
					__result = true;
					return false;
				}
			}
			catch { }
			return true;
		}

		private static bool DoorBehaviour_Unlock_Prefix(DoorBehaviour __instance)
		{
			try
			{
				var inst = _instForPatches;
				if (inst == null) return true;
				if (!inst.FarmhouseDoorBugEnabled || !inst.InFarmhouseScene) return true;

				if (IsBackDoor(__instance))
					return true;

				if (inst.ShouldGuardNow())
					return false;
				if (inst.IsReopeningDoor(__instance))
					return false;
			}
			catch { }
			return true;
		}

		private static void DoorBehaviour_Unlock_Postfix(DoorBehaviour __instance)
		{
			try
			{
				var inst = _instForPatches;
				if (inst == null) return;
				if (!inst.FarmhouseDoorBugEnabled || !inst.InFarmhouseScene) return;
				if (inst.ShouldGuardNow()) return;
				if (inst.IsReopeningDoor(__instance)) return;
				if (IsBackDoor(__instance)) return;

				inst.MarkPlayerUnlocked(__instance);
			}
			catch { }
		}

		private static bool DoorInteract_Prefix(DoorBehaviour __instance)
		{
			try
			{
				var inst = _instForPatches;
				if (inst == null) return true;
				if (!inst.FarmhouseDoorBugEnabled || !inst.InFarmhouseScene) return true;

				if (inst.TryConsumeInteractionToClose(__instance))
					return false;
			}
			catch { }
			return true;
		}


		private string lastScene = "";
		private bool hasShownLobbyNotice = false;
		private float lastLobbyCheckTime = 0f;
		private const float lobbyCheckInterval = 2f;

		public bool DisableLongInteract => disableLongInteract;
		public bool IsHoldingInteract => isHoldingInteract;
		public KeyCode CustomInteractKey => customInteractKey;
		public bool IsCapturingInteractKey => isCapturingInteractKey;
		public bool EnableAtticSpawn => enableAtticSpawn;
		public bool ForceStartEnabled => forceStartEnabled;
		public float ForceStartDelay => forceStartDelay;
		public float ForceStartArmMinutes => forceStartArmMinutes;
		public bool UseArmingWindow => useArmingWindow;

		private float QuickForceStartTimeoutSec => forceStartArmMinutes * 60f;

		public void Initialize(MelonPreferences_Category prefsCategory)
		{
			prefs = prefsCategory;

			prefDisableLongInteract = prefs.CreateEntry("DisableLongInteract", false);
			prefEnableAtticSpawn = prefs.CreateEntry("EnableAtticSpawn", false);
			prefInteractKey = prefs.CreateEntry("InteractKey", KeyCode.E);
			prefForceStartEnabled = prefs.CreateEntry("ForceStartEnabled", false);
			prefForceStartDelay = prefs.CreateEntry("ForceStartDelay", 0.8f);
			prefForceStartArmMinutes = prefs.CreateEntry("ForceStartArmMinutes", 3.0f);
			prefUseArmingWindow = prefs.CreateEntry("UseArmingWindow", true);

			disableLongInteract = prefDisableLongInteract.Value;
			enableAtticSpawn = prefEnableAtticSpawn.Value;
			customInteractKey = prefInteractKey.Value;
			forceStartEnabled = prefForceStartEnabled.Value;
			forceStartDelay = Mathf.Clamp(prefForceStartDelay.Value, 0.7f, 1.5f);
			forceStartArmMinutes = Mathf.Clamp(prefForceStartArmMinutes.Value, MIN_ARM_MINUTES, MAX_ARM_MINUTES);
			useArmingWindow = prefUseArmingWindow.Value;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			SetDisableLongInteract(false, prefsCategory);
			SetEnableAtticSpawn(false, prefsCategory);

			prefInteractKey.Value = KeyCode.E;
			customInteractKey = KeyCode.E;

			SetForceStartEnabled(false, prefsCategory, inMenu: true);

			prefForceStartDelay.Value = 0.8f;
			forceStartDelay = 0.8f;

			prefForceStartArmMinutes.Value = 3.0f;
			forceStartArmMinutes = 3.0f;

			SetUseArmingWindow(true, prefsCategory);

			forceStartArmed = false;
			forceStartExpireAt = -1f;
			forceStartTriggered = false;
			lobbyReturnTime = -1f;
			hasShownLobbyNotice = false;

			prefsCategory.SaveToFile(false);
		}

		public void OnSceneLoaded(string sceneName, bool wasInValidMap, bool inValidMap)
		{
			string previousScene = lastScene;
			lastScene = sceneName;

			teleported = false;
			cachedMenu = null;
			cachedInput = null;
			lastSearchTime = 0f;
			sceneLoadTime = sceneName == "Devour" ? Time.time : -1f;

			nextAtticSpawnTryTime = 0f;
			forceStartTriggered = false;
			hasShownLobbyNotice = false;

			if (wasInValidMap && IsLobbyScene(sceneName))
			{
				wasInGame = true;

				if (forceStartEnabled && useArmingWindow && forceStartArmed)
				{
					if (forceStartExpireAt > 0f && Time.time > forceStartExpireAt)
					{
						forceStartEnabled = false;
						prefForceStartEnabled.Value = false;
						forceStartArmed = false;
						forceStartExpireAt = -1f;
						prefs.SaveToFile(false);
					}
				}

				if (forceStartEnabled && (!useArmingWindow || forceStartArmed))
				{
					lobbyReturnTime = Time.time + forceStartDelay;
				}
				else
				{
					lobbyReturnTime = -1f;
				}
			}
			else if (!IsLobbyScene(sceneName))
			{
				wasInGame = false;
				lobbyReturnTime = -1f;

				if (forceStartEnabled && useArmingWindow && !forceStartArmed)
				{
					forceStartArmed = true;
					forceStartExpireAt = Time.time + QuickForceStartTimeoutSec;
				}
			}
			else
			{
				wasInGame = false;
				lobbyReturnTime = -1f;
			}

			try
			{
				if (FarmhouseDoorBugEnabled && InFarmhouseScene)
				{
					MelonCoroutines.Start(ForceBackDoorForSeconds(6f));
				}
			}
			catch { }

		}

		public void OnUpdate(bool inValidMap)
		{
			bool wasHoldingInteract = isHoldingInteract;
			isHoldingInteract = Input.GetKey(customInteractKey);

			if (isCapturingInteractKey)
			{
				foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
				{
					if (Input.GetKeyDown(key))
					{
						customInteractKey = key;
						prefInteractKey.Value = key;
						prefs.SaveToFile(false);
						isCapturingInteractKey = false;
						break;
					}
				}
			}

			if (cachedMenu == null && Time.time - lastSearchTime > searchThrottle)
			{
				cachedMenu = UnityEngine.Object.FindObjectOfType<GameMenu>();
				lastSearchTime = Time.time;
			}

			if (enableAtticSpawn && !teleported && lastScene == "Devour" && sceneLoadTime > 0f && Time.time - sceneLoadTime < 10f)
			{

				if (Time.time >= nextAtticSpawnTryTime)
				{
					nextAtticSpawnTryTime = Time.time + 0.15f;

					if (cachedInput == null)
						cachedInput = UnityEngine.Object.FindObjectOfType<RewiredInput>();

					if (cachedInput != null)
					{
						var locomotion = cachedInput.GetComponent<UltimateCharacterLocomotion>();
						if (locomotion != null)
						{
							try
							{
								locomotion.enabled = false;
								locomotion.transform.position = targetPosition;
								locomotion.enabled = true;
							}
							catch { }


							try
							{
								if (Vector3.Distance(locomotion.transform.position, targetPosition) <= 0.25f)
									teleported = true;
							}
							catch { }
						}
					}
				}
			}
			if (forceStartEnabled
							&& (!useArmingWindow || forceStartArmed)
							&& !forceStartTriggered
							&& wasInGame
							&& lobbyReturnTime > 0f
							&& Time.time >= lobbyReturnTime
							&& (!useArmingWindow || forceStartExpireAt <= 0f || Time.time <= forceStartExpireAt))
			{
				TriggerForceStart();
			}

			if (!hasShownLobbyNotice && Time.time - lastLobbyCheckTime > lobbyCheckInterval)
			{
				lastLobbyCheckTime = Time.time;

				if (inValidMap && (disableLongInteract || enableAtticSpawn))
				{
					hasShownLobbyNotice = true;
				}
			}
		}

		public void SetDisableLongInteract(bool disabled, MelonPreferences_Category prefsCategory)
		{
			disableLongInteract = disabled;
			prefDisableLongInteract.Value = disabled;
			prefs.SaveToFile(false);
			hasShownLobbyNotice = false;
		}

		public void SetEnableAtticSpawn(bool enabled, MelonPreferences_Category prefsCategory)
		{
			enableAtticSpawn = enabled;
			prefEnableAtticSpawn.Value = enabled;
			prefs.SaveToFile(false);
			hasShownLobbyNotice = false;
		}

		public void StartCapturingInteractKey()
		{
			isCapturingInteractKey = true;
		}

		public void SetForceStartEnabled(bool enabled, MelonPreferences_Category prefsCategory, bool inMenu)
		{
			if (!inMenu && prefsCategory != null) return;

			forceStartEnabled = enabled;
			prefForceStartEnabled.Value = enabled;

			forceStartArmed = false;
			forceStartExpireAt = -1f;
			forceStartTriggered = false;
			lobbyReturnTime = -1f;

			prefs.SaveToFile(false);
		}

		public void SetForceStartDelay(float delay)
		{
			forceStartDelay = delay;
			prefForceStartDelay.Value = delay;
			prefs.SaveToFile(false);
		}

		public void SetForceStartArmMinutes(float minutes)
		{
			forceStartArmMinutes = Mathf.Clamp(minutes, MIN_ARM_MINUTES, MAX_ARM_MINUTES);
			prefForceStartArmMinutes.Value = forceStartArmMinutes;
			prefs.SaveToFile(false);
		}

		public void SetUseArmingWindow(bool use, MelonPreferences_Category prefsCategory)
		{
			useArmingWindow = use;
			prefUseArmingWindow.Value = use;
			prefs.SaveToFile(false);

			forceStartArmed = false;
			forceStartExpireAt = -1f;
		}

		private void TriggerForceStart()
		{
			if (cachedMenu == null)
				cachedMenu = UnityEngine.Object.FindObjectOfType<GameMenu>();

			if (cachedMenu != null)
			{
				try
				{
					cachedMenu.OnLobbyStartButtonClick();
					forceStartTriggered = true;
					wasInGame = false;
					lobbyReturnTime = -1f;

					forceStartArmed = false;
					forceStartExpireAt = -1f;
				}
				catch { }
			}
		}

		private bool IsLobbyScene(string sceneName)
		{
			return sceneName.Contains("Lobby") ||
				   sceneName.Contains("Menu") ||
				   sceneName == "MainMenu";
		}

		public bool HandleGetLongPressPrefix(ref float duration)
		{
			if (disableLongInteract && isHoldingInteract && duration > 0.03f)
				duration = 0.03f;
			return true;
		}
	}
}