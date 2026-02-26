using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using BoltEntityIl2Cpp = Il2CppPhoton.Bolt.BoltEntity;

namespace DevourCore
{
	public class Misc
	{
		private const string MENU_SCENE_NAME = "Menu";
		private float _menuCustomFOV = 45f;

		private const float MAP_SWITCH_COOLDOWN = 1.0f;
		private float _lastMapSwitchTime = -99f;

		public class MapInfo
		{
			public int Id;
			public string DisplayName;
			public string LobbyObjectName;
			public float HorizontalOffset;
			public float DistanceOffset;
			public float VerticalOffset;
		}

		public static readonly MapInfo[] MapInfos = new[]
		{
			new MapInfo { Id = 1, DisplayName = "Town",           LobbyObjectName = "Town",           HorizontalOffset = 3.4f,  DistanceOffset = 1.4f,   VerticalOffset = 0.8f },
			new MapInfo { Id = 2, DisplayName = "Manor",          LobbyObjectName = "Manor",          HorizontalOffset = 1.4f,  DistanceOffset = 1.4f,   VerticalOffset = 0.7f },
			new MapInfo { Id = 3, DisplayName = "Farmhouse",      LobbyObjectName = "Farmhouse",      HorizontalOffset = 3.2f,  DistanceOffset = -13.3f, VerticalOffset = 0.6f },
			new MapInfo { Id = 4, DisplayName = "Asylum",         LobbyObjectName = "Asylum",         HorizontalOffset = 3.2f,  DistanceOffset = 0.8f,   VerticalOffset = 0.6f },
			new MapInfo { Id = 5, DisplayName = "Inn",            LobbyObjectName = "Inn",            HorizontalOffset = 2.8f,  DistanceOffset = 0.8f,   VerticalOffset = 0.9f },
			new MapInfo { Id = 6, DisplayName = "Slaughterhouse", LobbyObjectName = "Slaughterhouse", HorizontalOffset = 2.3f,  DistanceOffset = 0.9f,   VerticalOffset = 0.8f },
			new MapInfo { Id = 7, DisplayName = "Carnival",       LobbyObjectName = "Carnival",       HorizontalOffset = 2.9f,  DistanceOffset = 2.0f,   VerticalOffset = 0.5f },
		};

		private const int MIN_BG_ID = 1;
		private const int MAX_BG_ID = 7;

		private MelonPreferences_Entry<int> prefSelectedBg;
		private MelonPreferences_Entry<bool> prefSceneEnabled;

		private int _selectedBg = 2;
		private bool _sceneEnabled = false;

		private bool _wasInLobby = false;
		private bool _autoDisabledForLobby = false;
		private bool _isApplying = false;
		private object _currentApplyRoutine = null;
		private string _lastMenuUiSignature = null;

		private struct TransformBackup
		{
			public bool HasValue;
			public Vector3 Position;
			public Quaternion Rotation;
			public Vector3 Scale;
			public bool WasActive;
		}

		private readonly Dictionary<string, TransformBackup> _backups =
			new Dictionary<string, TransformBackup>();

		private readonly Dictionary<string, GameObject> _mapTemplates =
			new Dictionary<string, GameObject>();

		private GameObject _originalMenuMapRoot = null;
		private GameObject _spawnedMenuMapClone = null;

		private MelonPreferences_Category _prefsCategory;

		private float _originalMenuFov = -1f;
		private bool _menuFovApplied = false;

		private Audio _audio;

		private float _lobbyLeftTime = -1f;
		private const float LOBBY_LEAVE_DELAY = 1.0f;

		public static bool IsStartingGame = false;

		private bool _pendingGuiRetoggle = false;
		private object _guiRetoggleRoutine = null;

		private bool _lobbyButtonsHooked = false;
		private Button _lobbyStartButton = null;
		private Button _lobbyReadyButton = null;
		private bool _lastLobbyUiVisible = false;
		private Il2Cpp.NolanBehaviour _cachedLocalPlayer = null;
		private float _lastLocalPlayerCheckTime = 0f;
		private const float LOCAL_PLAYER_CHECK_INTERVAL = 0.5f;

		public bool SceneEnabled => _sceneEnabled;
		public int SelectedBg => _selectedBg;
		public Audio Audio => _audio;
		public bool IsMapSwitchCooldownActive => (Time.time - _lastMapSwitchTime) < MAP_SWITCH_COOLDOWN;

		public bool RememberMusic => _audio?.RememberMusic ?? false;
		public string LastClipName => _audio?.LastClipName ?? string.Empty;
		public bool DisableIngameMusic => _audio?.DisableIngameMusic ?? false;
		public bool MuteCarnivalTunnel => _audio?.MuteCarnivalTunnel ?? false;
		public bool MuteCarnivalClockSounds => _audio?.MuteCarnivalClockSounds ?? false;


		public void Initialize(MelonPreferences_Category prefs)
		{
			_prefsCategory = prefs;

			prefSelectedBg = prefs.CreateEntry("MenuSelectedBackground", 2);
			prefSceneEnabled = prefs.CreateEntry("MenuSceneEnabled", false);

			_selectedBg = Mathf.Clamp(prefSelectedBg.Value, MIN_BG_ID, MAX_BG_ID);
			_sceneEnabled = prefSceneEnabled.Value;

			_audio = new Audio();
			_audio.Initialize(prefs);
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory, bool firstRun)
		{
			_selectedBg = 2;
			prefSelectedBg.Value = 2;

			if (firstRun)
			{
				SetSceneEnabled(false, prefsCategory);
			}

			_audio?.ApplyFirstRunDefaults(prefsCategory);

			_autoDisabledForLobby = false;
			_isApplying = false;
			_currentApplyRoutine = null;
			_backups.Clear();
			_mapTemplates.Clear();
			_originalMenuMapRoot = null;

			if (_spawnedMenuMapClone != null)
			{
				try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
				_spawnedMenuMapClone = null;
			}

			_originalMenuFov = -1f;
			_menuFovApplied = false;

			_pendingGuiRetoggle = false;

			if (_guiRetoggleRoutine != null)
			{
				try { MelonCoroutines.Stop(_guiRetoggleRoutine); } catch { }
				_guiRetoggleRoutine = null;
			}

			_lobbyLeftTime = -1f;
			_lobbyButtonsHooked = false;
			_lobbyStartButton = null;
			_lobbyReadyButton = null;
			_lastLobbyUiVisible = false;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			ApplyFirstRunDefaults(prefsCategory, true);
		}

		public void SetSceneEnabled(bool enabled, MelonPreferences_Category prefs)
		{
			_sceneEnabled = enabled;
			prefSceneEnabled.Value = enabled;
			prefs.SaveToFile(false);

			if (!enabled)
			{
				_autoDisabledForLobby = false;
				RevertToCarnival();
				RestoreOriginalMenuFov();
			}
			else
			{
				StartApplyBackground();
				if (IsInMenuSceneInternal() && !IsInLobby())
					ApplyMenuFov();
			}
		}

		public void SetSelectedBg(int id, MelonPreferences_Category prefs)
		{
			if (IsMapSwitchCooldownActive) return;

			_selectedBg = Mathf.Clamp(id, MIN_BG_ID, MAX_BG_ID);
			prefSelectedBg.Value = _selectedBg;
			if (prefs != null)
				prefs.SaveToFile(false);

			_lastMapSwitchTime = Time.time;

			StartApplyBackground();
		}

		public void SetRememberMusic(bool enabled, MelonPreferences_Category prefs)
		{
			_audio?.SetRememberMusic(enabled, prefs);
		}

		public void SetDisableIngameMusic(bool value, MelonPreferences_Category prefs)
		{
			_audio?.SetDisableIngameMusic(value, prefs);
		}

		public void SetMuteCarnivalTunnel(bool value, MelonPreferences_Category prefs)
		{
			_audio?.SetMuteCarnivalTunnel(value, prefs);
		}

		public void SetMuteCarnivalClockSounds(bool value, MelonPreferences_Category prefs)
		{
			_audio?.SetMuteCarnivalClockSounds(value, prefs);
		}





		public MapInfo GetSelectedMapInfo()
		{
			foreach (var map in MapInfos)
				if (map.Id == _selectedBg)
					return map;
			return MapInfos[0];
		}

		public void OnSceneWasLoaded(string sceneName)
		{
			if (_spawnedMenuMapClone != null)
			{
				try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
				_spawnedMenuMapClone = null;
			}

			if (_guiRetoggleRoutine != null)
			{
				try { MelonCoroutines.Stop(_guiRetoggleRoutine); } catch { }
				_guiRetoggleRoutine = null;
			}
			_pendingGuiRetoggle = false;

			_lobbyButtonsHooked = false;
			_lobbyStartButton = null;
			_lobbyReadyButton = null;
			_lastLobbyUiVisible = false;
			_cachedLocalPlayer = null;
			_lastLocalPlayerCheckTime = 0f;

			if (sceneName != MENU_SCENE_NAME)
			{
				IsStartingGame = false;
			}

			_lobbyLeftTime = -1f;

			if (sceneName == MENU_SCENE_NAME)
			{
				bool inLobbyNow = FindLocalPlayer() != null;
				_wasInLobby = inLobbyNow;
				_lastMenuUiSignature = GetMenuUiSignature();

				if (inLobbyNow)
				{
					AutoDisableForLobby();
					RestoreOriginalMenuFov();
				}
				else
				{
					if (_sceneEnabled)
					{
						if (!_autoDisabledForLobby)
						{
							StartApplyBackground();
							ApplyMenuFov();
						}
						else
						{
							_lobbyLeftTime = Time.time;
						}
					}
					else
					{
						RestoreOriginalMenuFov();
					}
				}
			}
			else
			{
				_menuFovApplied = false;
			}

			_audio?.OnSceneWasLoaded(sceneName);
		}

		public void OnUpdate()
		{
			var scene = SceneManager.GetActiveScene();
			if (!scene.IsValid())
				return;

			if (Input.GetKeyDown(KeyCode.F8))
			{
				DumpAllButtons();
			}

			_audio?.OnUpdate();

			if (scene.name != MENU_SCENE_NAME)
				return;

			bool inLobby = FindLocalPlayer() != null;

			if (inLobby && !_lobbyButtonsHooked)
			{
				HookLobbyButtons();
				_lobbyButtonsHooked = true;
			}

			bool lobbyUiVisibleNow = IsLobbyUIVisible();
			if (_lastLobbyUiVisible && !lobbyUiVisibleNow)
			{
				CheckAndStartGuiRetoggle(_prefsCategory);
			}
			_lastLobbyUiVisible = lobbyUiVisibleNow;

			if (!_wasInLobby && inLobby)
			{
				AutoDisableForLobby();
				RestoreOriginalMenuFov();
				_lobbyLeftTime = -1f;
			}

			if (_wasInLobby && !inLobby)
			{
				if (!IsLobbyUIVisible())
					_lobbyLeftTime = Time.time;
				else
					_lobbyLeftTime = -1f;
			}

			_wasInLobby = inLobby;

			if (_lobbyLeftTime > 0f && !inLobby && !IsLobbyUIVisible())
			{
				if (Time.time - _lobbyLeftTime >= LOBBY_LEAVE_DELAY)
				{
					if (_sceneEnabled && IsInMenuSceneInternal() && !IsInLobby())
					{
						StartApplyBackground();
						ApplyMenuFov();
					}

					_autoDisabledForLobby = false;
					_lobbyLeftTime = -1f;
				}
			}

			if (_sceneEnabled && !inLobby && !IsLobbyUIVisible() && _originalMenuMapRoot != null)
			{
				try
				{
					if (_originalMenuMapRoot.activeInHierarchy)
						_originalMenuMapRoot.SetActive(false);
				}
				catch { }
			}

			if (_sceneEnabled && !inLobby && !IsLobbyUIVisible() && !_menuFovApplied)
			{
				ApplyMenuFov();
			}
		}

		public void CheckAndStartGuiRetoggle(MelonPreferences_Category prefs)
		{
			if (prefs != null)
				_prefsCategory = prefs;

			if (_pendingGuiRetoggle)
				return;

			if (!_sceneEnabled)
				return;

			if (!IsInMenuSceneInternal() || IsInLobby())
				return;

			if (_prefsCategory == null)
				return;

			_pendingGuiRetoggle = true;
			_guiRetoggleRoutine = MelonCoroutines.Start(GuiRetoggleCoroutine(_prefsCategory));
		}

		private IEnumerator GuiRetoggleCoroutine(MelonPreferences_Category prefs)
		{
			const float RETOGGLE_DELAY = 1f;
			float startTime = Time.time;

			while (Time.time - startTime < RETOGGLE_DELAY)
			{
				if (!IsInMenuSceneInternal() || IsInLobby())
				{
					_pendingGuiRetoggle = false;
					_guiRetoggleRoutine = null;
					yield break;
				}

				yield return null;
			}

			if (!_sceneEnabled || !IsInMenuSceneInternal() || IsInLobby())
			{
				_pendingGuiRetoggle = false;
				_guiRetoggleRoutine = null;
				yield break;
			}

			try
			{
				if (_sceneEnabled && prefs != null)
				{
					SetSceneEnabled(false, prefs);
					yield return null;
					SetSceneEnabled(true, prefs);
				}
			}
			finally
			{
				_pendingGuiRetoggle = false;
				_guiRetoggleRoutine = null;
			}
		}

		private void ApplyMenuFov()
		{
			if (!_sceneEnabled || !IsInMenuSceneInternal() || IsInLobby() || IsLobbyUIVisible())
				return;

			var cam = Camera.main;
			if (cam != null)
			{
				if (_originalMenuFov < 0)
				{
					_originalMenuFov = cam.fieldOfView;
				}

				cam.fieldOfView = _menuCustomFOV;
				_menuFovApplied = true;
			}
		}

		private void RestoreOriginalMenuFov()
		{
			if (_originalMenuFov < 0 || !IsInMenuSceneInternal())
				return;

			var cam = Camera.main;
			if (cam != null)
			{
				cam.fieldOfView = _originalMenuFov;
				_menuFovApplied = false;
			}
		}

		public bool IsInLobby()
		{
			return _wasInLobby;
		}

		public bool IsMenuSceneActive()
		{
			var scene = SceneManager.GetActiveScene();
			return scene.IsValid() && scene.name == MENU_SCENE_NAME;
		}

		private bool IsInMenuSceneInternal()
		{
			var scene = SceneManager.GetActiveScene();
			return scene.IsValid() && scene.name == MENU_SCENE_NAME;
		}

		private void AutoDisableForLobby()
		{
			if (!IsInMenuSceneInternal())
				return;

			if (_currentApplyRoutine != null)
			{
				try { MelonCoroutines.Stop(_currentApplyRoutine); } catch { }
				_currentApplyRoutine = null;
			}

			_isApplying = false;

			if (_spawnedMenuMapClone != null)
			{
				try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
				_spawnedMenuMapClone = null;
			}

			if (!_sceneEnabled)
			{
				_autoDisabledForLobby = false;
				return;
			}

			_autoDisabledForLobby = true;
		}

		private void StartApplyBackground()
		{
			if (!IsInMenuSceneInternal())
				return;

			if (!_sceneEnabled)
				return;

			if (_isApplying)
				return;

			if (_spawnedMenuMapClone != null)
			{
				try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
				_spawnedMenuMapClone = null;
			}

			_currentApplyRoutine = MelonCoroutines.Start(ApplyBackgroundCoroutine());
		}

		private IEnumerator ApplyBackgroundCoroutine()
		{
			_isApplying = true;

			try
			{
				for (int i = 0; i < 5; i++)
					yield return null;

				if (!_sceneEnabled)
					yield break;

				var menuScene = SceneManager.GetSceneByName(MENU_SCENE_NAME);
				if (!menuScene.IsValid())
					yield break;

				var roots = menuScene.GetRootGameObjects();
				if (roots == null || roots.Length == 0)
					yield break;

				GameObject menuEnvRoot = roots.FirstOrDefault(r => r.name == "Menu Environment");
				GameObject lobbyEnvRoot = roots.FirstOrDefault(r => r.name == "Lobby Environments");

				if (menuEnvRoot == null || lobbyEnvRoot == null)
					yield break;

				var menuMapT = FindMenuMapTransform(menuEnvRoot);
				if (menuMapT == null)
					yield break;

				GameObject menuMapRoot = menuMapT.gameObject;
				_originalMenuMapRoot = menuMapRoot;

				Transform mT = menuMapRoot.transform;
				Vector3 basePos = mT.position;
				Quaternion baseRot = mT.rotation;
				Vector3 baseScale = mT.localScale;
				int baseLayer = menuMapRoot.layer;

				Bounds origBounds = new Bounds();
				bool hasOrigBounds = false;
				try
				{
					var origRenderers = menuMapRoot.GetComponentsInChildren<Renderer>(true);
					if (origRenderers != null && origRenderers.Length > 0)
					{
						origBounds = origRenderers[0].bounds;
						for (int i = 1; i < origRenderers.Length; i++)
						{
							if (origRenderers[i] != null)
								origBounds.Encapsulate(origRenderers[i].bounds);
						}
						hasOrigBounds = true;
					}
				}
				catch { }

				var map = GetSelectedMapInfo();

				if (string.IsNullOrEmpty(map.LobbyObjectName))
				{
					RevertToCarnival();
					yield break;
				}

				foreach (var mi in MapInfos)
				{
					if (string.IsNullOrEmpty(mi.LobbyObjectName))
						continue;

					var tMi = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
						.FirstOrDefault(x => x != null && x.name == mi.LobbyObjectName);

					if (tMi == null)
						continue;

					EnsureBackup(mi.LobbyObjectName, tMi);

					if (!_mapTemplates.TryGetValue(mi.LobbyObjectName, out var existingTemplate) || existingTemplate == null)
					{
						var template = UnityEngine.Object.Instantiate(tMi.gameObject);
						template.name = tMi.gameObject.name + "_DevourCoreTemplatePrefab";
						UnityEngine.Object.DontDestroyOnLoad(template);
						template.SetActive(false);
						_mapTemplates[mi.LobbyObjectName] = template;
					}
				}

				GameObject selectedRoot = null;
				if (!_mapTemplates.TryGetValue(map.LobbyObjectName, out selectedRoot) || selectedRoot == null)
				{
					var selectedT = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
						.FirstOrDefault(t => t != null && t.name == map.LobbyObjectName);

					if (selectedT == null)
						yield break;

					selectedRoot = selectedT.gameObject;
				}

				if (_spawnedMenuMapClone != null)
				{
					try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
					_spawnedMenuMapClone = null;
				}

				_spawnedMenuMapClone = UnityEngine.Object.Instantiate(selectedRoot);
				var cloneRoot = _spawnedMenuMapClone;
				cloneRoot.name = selectedRoot.name + "_DevourCoreClone";
				cloneRoot.SetActive(true);

				cloneRoot.transform.SetParent(null, true);

				Transform sT = cloneRoot.transform;

				sT.position = basePos;
				sT.rotation = baseRot;
				sT.localScale = baseScale;

				SetLayerRecursively(cloneRoot, baseLayer);

				Bounds cloneBounds = new Bounds();
				bool hasCloneBounds = false;
				try
				{
					var cloneRenderers = cloneRoot.GetComponentsInChildren<Renderer>(true);
					if (cloneRenderers != null && cloneRenderers.Length > 0)
					{
						cloneBounds = cloneRenderers[0].bounds;
						for (int i = 1; i < cloneRenderers.Length; i++)
						{
							if (cloneRenderers[i] != null)
								cloneBounds.Encapsulate(cloneRenderers[i].bounds);
						}
						hasCloneBounds = true;
					}
				}
				catch { }

				if (hasOrigBounds && hasCloneBounds)
				{
					Vector3 origCenter = origBounds.center;
					Vector3 cloneCenter = cloneBounds.center;

					Vector3 delta = origCenter - cloneCenter;
					sT.position += delta;
				}

				var cam = Camera.main;
				if (cam != null)
				{
					sT.position += cam.transform.forward * map.DistanceOffset;
					sT.position += cam.transform.right * map.HorizontalOffset;
				}

				if (Mathf.Abs(map.VerticalOffset) > 0.001f)
				{
					sT.position += Vector3.up * map.VerticalOffset;
				}

				menuMapRoot.SetActive(false);

				DisableAllAnimations(cloneRoot);
				DisableCameraAnimations();

				ApplyMenuFov();
			}
			finally
			{
				_isApplying = false;
			}
		}

		private void DisableAllAnimations(GameObject root)
		{
			if (root == null)
				return;

			try
			{
				var anims = root.GetComponentsInChildren<Animation>(true);
				for (int i = 0; i < anims.Length; i++)
				{
					if (anims[i] != null)
						anims[i].enabled = false;
				}
			}
			catch { }

			try
			{
				var animators = root.GetComponentsInChildren<Animator>(true);
				for (int i = 0; i < animators.Length; i++)
				{
					if (animators[i] != null)
						animators[i].enabled = false;
				}
			}
			catch { }

			try
			{
				var rbs = root.GetComponentsInChildren<Rigidbody>(true);
				for (int i = 0; i < rbs.Length; i++)
				{
					var rb = rbs[i];
					if (rb == null) continue;
					rb.isKinematic = true;
					rb.velocity = Vector3.zero;
					rb.angularVelocity = Vector3.zero;
				}
			}
			catch { }
		}

		private void DisableCameraAnimations()
		{
			var cam = Camera.main;
			if (cam == null)
				return;

			try
			{
				var anims = cam.GetComponentsInChildren<Animation>(true);
				for (int i = 0; i < anims.Length; i++)
				{
					if (anims[i] != null)
						anims[i].enabled = false;
				}
			}
			catch { }

			try
			{
				var animators = cam.GetComponentsInChildren<Animator>(true);
				for (int i = 0; i < animators.Length; i++)
				{
					if (animators[i] != null)
						animators[i].enabled = false;
				}
			}
			catch { }
		}

		private void EnsureBackup(string key, Transform t)
		{
			if (string.IsNullOrEmpty(key) || t == null)
				return;

			if (_backups.ContainsKey(key))
				return;

			var go = t.gameObject;
			var backup = new TransformBackup
			{
				HasValue = true,
				Position = t.position,
				Rotation = t.rotation,
				Scale = t.localScale,
				WasActive = go != null ? go.activeSelf : false
			};

			_backups[key] = backup;
		}

		private void RestoreLobbyMapTransforms()
		{
			if (!IsInMenuSceneInternal())
				return;

			var menuScene = SceneManager.GetSceneByName(MENU_SCENE_NAME);
			if (!menuScene.IsValid())
				return;

			var roots = menuScene.GetRootGameObjects();
			if (roots == null || roots.Length == 0)
				return;

			GameObject lobbyEnvRoot = roots.FirstOrDefault(r => r.name == "Lobby Environments");
			if (lobbyEnvRoot == null)
				return;

			foreach (var mi in MapInfos)
			{
				if (string.IsNullOrEmpty(mi.LobbyObjectName))
					continue;

				if (!_backups.TryGetValue(mi.LobbyObjectName, out var backup) || !backup.HasValue)
					continue;

				var t = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
					.FirstOrDefault(x => x != null && x.name == mi.LobbyObjectName);

				if (t != null)
				{
					t.position = backup.Position;
					t.rotation = backup.Rotation;
					t.localScale = backup.Scale;

					var go = t.gameObject;
					if (go != null)
					{
						go.SetActive(backup.WasActive);
					}
				}
			}
		}

		private Il2Cpp.NolanBehaviour FindLocalPlayer()
		{
			float now = Time.time;
			if (now - _lastLocalPlayerCheckTime < LOCAL_PLAYER_CHECK_INTERVAL && _cachedLocalPlayer != null)
			{
				try
				{
					if (_cachedLocalPlayer != null && _cachedLocalPlayer.gameObject != null)
					{
						var entity = _cachedLocalPlayer.gameObject.GetComponent<BoltEntityIl2Cpp>();
						if (entity != null && entity.IsOwner)
							return _cachedLocalPlayer;
					}
				}
				catch { }
				_cachedLocalPlayer = null;
			}

			_lastLocalPlayerCheckTime = now;

			Il2Cpp.NolanBehaviour[] players = null;
			try
			{
				players = Il2Cpp.NolanBehaviour.FindObjectsOfType<Il2Cpp.NolanBehaviour>();
			}
			catch
			{
				return null;
			}

			if (players == null || players.Length == 0)
			{
				_cachedLocalPlayer = null;
				return null;
			}

			for (int i = 0; i < players.Length; i++)
			{
				var nb = players[i];
				if (nb == null || nb.gameObject == null)
					continue;

				try
				{
					var entity = nb.gameObject.GetComponent<BoltEntityIl2Cpp>();
					if (entity != null && entity.IsOwner)
					{
						_cachedLocalPlayer = nb;
						return nb;
					}
				}
				catch { }
			}

			_cachedLocalPlayer = null;
			return null;
		}

		private void RevertToCarnival()
		{
			if (!IsInMenuSceneInternal())
				return;

			var menuScene = SceneManager.GetSceneByName(MENU_SCENE_NAME);
			if (!menuScene.IsValid())
				return;

			var roots = menuScene.GetRootGameObjects();
			if (roots == null || roots.Length == 0)
				return;

			GameObject menuEnvRoot = roots.FirstOrDefault(r => r.name == "Menu Environment");

			if (_spawnedMenuMapClone != null)
			{
				try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
				_spawnedMenuMapClone = null;
			}

			GameObject menuMapRoot = _originalMenuMapRoot;

			if (menuMapRoot == null && menuEnvRoot != null)
			{
				var t = FindMenuMapTransform(menuEnvRoot);
				if (t != null)
					menuMapRoot = t.gameObject;
			}

			if (menuMapRoot != null)
				menuMapRoot.SetActive(true);
		}

		private static void SetLayerRecursively(GameObject go, int layer)
		{
			if (go == null)
				return;

			var transforms = go.GetComponentsInChildren<Transform>(true);
			foreach (var t in transforms)
			{
				if (t == null || t.gameObject == null)
					continue;

				t.gameObject.layer = layer;
			}
		}

		private Transform FindMenuMapTransform(GameObject menuEnvRoot)
		{
			if (menuEnvRoot == null)
				return null;

			var transforms = menuEnvRoot.GetComponentsInChildren<Transform>(true);
			if (transforms == null || transforms.Length == 0)
				return null;

			var t = transforms.FirstOrDefault(tr => tr != null && tr.name == "Carnival");
			if (t != null)
				return t;

			foreach (var mi in MapInfos)
			{
				if (string.IsNullOrEmpty(mi.LobbyObjectName))
					continue;

				var candidate = transforms.FirstOrDefault(tr => tr != null && tr.name == mi.LobbyObjectName);
				if (candidate != null)
					return candidate;
			}

			Transform best = null;
			int bestScore = -1;

			for (int i = 0; i < transforms.Length; i++)
			{
				var tr = transforms[i];
				if (tr == null || tr.gameObject == null)
					continue;

				if (tr.GetComponent<Canvas>() != null || tr.GetComponent<RectTransform>() != null)
					continue;

				Renderer[] renderers = null;
				try
				{
					renderers = tr.GetComponentsInChildren<Renderer>(true);
				}
				catch { }

				int count = renderers != null ? renderers.Length : 0;
				if (count > 0 && count > bestScore)
				{
					bestScore = count;
					best = tr;
				}
			}

			return best;
		}

		private string GetMenuUiSignature()
		{
			var menuScene = SceneManager.GetSceneByName(MENU_SCENE_NAME);
			if (!menuScene.IsValid())
				return null;

			var roots = menuScene.GetRootGameObjects();
			if (roots == null || roots.Length == 0)
				return null;

			var sb = new System.Text.StringBuilder();

			foreach (var root in roots)
			{
				if (root == null)
					continue;

				var canvas = root.GetComponentInChildren<Canvas>(true);
				if (canvas == null)
					continue;

				int activeCount = 0;
				var transforms = root.GetComponentsInChildren<Transform>(true);
				for (int i = 0; i < transforms.Length; i++)
				{
					var t = transforms[i];
					if (t != null && t.gameObject != null && t.gameObject.activeInHierarchy)
						activeCount++;
				}

				sb.Append(root.name)
				  .Append(':')
				  .Append(root.activeSelf ? '1' : '0')
				  .Append(':')
				  .Append(activeCount)
				  .Append(';');
			}

			return sb.Length > 0 ? sb.ToString() : null;
		}

		private void DumpAllButtons()
		{
			try
			{
				var buttons = GameObject.FindObjectsOfType<Button>(true);
				var sceneName = SceneManager.GetActiveScene().name;

				foreach (var btn in buttons)
				{
					if (btn == null || btn.gameObject == null)
						continue;

					string name = btn.gameObject.name;
					bool activeSelf = false;
					bool activeHierarchy = false;
					try
					{
						activeSelf = btn.gameObject.activeSelf;
						activeHierarchy = btn.gameObject.activeInHierarchy;
					}
					catch { }

					string text = "";
					try
					{
						var uiText = btn.GetComponentInChildren<Text>(true);
						if (uiText != null && !string.IsNullOrEmpty(uiText.text))
							text = uiText.text;
					}
					catch { }

					string path = GetTransformPath(btn.transform);
				}
			}
			catch
			{
			}
		}

		private void HookLobbyButtons()
		{
			try
			{
				var buttons = GameObject.FindObjectsOfType<Button>(true);
				foreach (var btn in buttons)
				{
					if (btn == null || btn.gameObject == null)
						continue;

					string label = "";
					try
					{
						var uiText = btn.GetComponentInChildren<Text>(true);
						if (uiText != null && !string.IsNullOrEmpty(uiText.text))
							label = uiText.text;
					}
					catch { }

					string trimmed = label == null ? "" : label.Trim();
					if (trimmed.Length == 0)
						continue;

					bool isStartGame = string.Equals(trimmed, "START GAME", System.StringComparison.OrdinalIgnoreCase);
					bool isReady = string.Equals(trimmed, "READY", System.StringComparison.OrdinalIgnoreCase);

					if (!isStartGame && !isReady)
						continue;

					string path = GetTransformPath(btn.transform);

					if (isStartGame)
					{
						_lobbyStartButton = btn;
						btn.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
						{
						}));
					}

					if (isReady)
					{
						_lobbyReadyButton = btn;
						btn.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
						{
						}));
					}
				}
			}
			catch
			{
			}
		}

		private bool IsLobbyUIVisible()
		{
			try
			{
				if (_lobbyStartButton != null && _lobbyStartButton.gameObject != null && _lobbyStartButton.gameObject.activeInHierarchy)
					return true;

				if (_lobbyReadyButton != null && _lobbyReadyButton.gameObject != null && _lobbyReadyButton.gameObject.activeInHierarchy)
					return true;
			}
			catch { }

			return false;
		}

		private static string GetTransformPath(Transform t)
		{
			if (t == null)
				return "<null>";

			var list = new List<string>();
			var current = t;
			while (current != null)
			{
				list.Add(current.name);
				current = current.parent;
			}
			list.Reverse();
			return string.Join("/", list);
		}
	}
}