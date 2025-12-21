using MelonLoader;
using UnityEngine;
using Il2Cpp;
using Il2CppHorror;
using Il2CppOpsive.UltimateCharacterController.Character;
using Il2CppOpsive.UltimateCharacterController.Integrations.Rewired;
using System;
using GameMenu = Il2CppHorror.Menu;

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

			SetUseArmingWindow(false, prefsCategory);

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

			if (enableAtticSpawn && !teleported && lastScene == "Devour" && Time.time - sceneLoadTime < 6f)
			{
				if (cachedInput == null)
				{
					cachedInput = UnityEngine.Object.FindObjectOfType<RewiredInput>();
					if (cachedInput != null)
					{
						var locomotion = cachedInput.GetComponent<UltimateCharacterLocomotion>();
						if (locomotion != null)
						{
							locomotion.enabled = false;
							locomotion.transform.position = targetPosition;
							locomotion.enabled = true;
							teleported = true;
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