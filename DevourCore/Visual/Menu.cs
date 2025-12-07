using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

using BoltEntityIl2Cpp = Il2CppPhoton.Bolt.BoltEntity;

namespace DevourCore
{
    public class Menu
    {
        private const string MENU_SCENE_NAME = "Menu";
        private float _menuCustomFOV = 40f;

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
            new MapInfo { Id = 1, DisplayName = "Town",           LobbyObjectName = "Town",           HorizontalOffset = 3.4f, DistanceOffset = 1.4f,   VerticalOffset = 0.8f },
            new MapInfo { Id = 2, DisplayName = "Manor",          LobbyObjectName = "Manor",          HorizontalOffset = 1.4f, DistanceOffset = 1.4f,   VerticalOffset = 0.7f },
            new MapInfo { Id = 3, DisplayName = "Farmhouse",      LobbyObjectName = "Farmhouse",      HorizontalOffset = 3.2f, DistanceOffset = -13.3f, VerticalOffset = 0.6f },
            new MapInfo { Id = 4, DisplayName = "Asylum",         LobbyObjectName = "Asylum",         HorizontalOffset = 3.2f, DistanceOffset = 0.8f,   VerticalOffset = 0.6f },
            new MapInfo { Id = 5, DisplayName = "Inn",            LobbyObjectName = "Inn",            HorizontalOffset = 2.8f, DistanceOffset = 0.8f,   VerticalOffset = 0.9f },
            new MapInfo { Id = 6, DisplayName = "Slaughterhouse", LobbyObjectName = "Slaughterhouse", HorizontalOffset = 2.3f, DistanceOffset = 0.9f,   VerticalOffset = 0.8f },
            new MapInfo { Id = 7, DisplayName = "Carnival",       LobbyObjectName = "Carnival",       HorizontalOffset = 2.9f, DistanceOffset = 2.0f,   VerticalOffset = 0.5f },
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
        }

        private readonly Dictionary<string, TransformBackup> _backups =
            new Dictionary<string, TransformBackup>();

        private GameObject _originalMenuMapRoot = null;
        private GameObject _spawnedMenuMapClone = null;

        private MelonPreferences_Entry<string> prefLastClipName;
        private MelonPreferences_Entry<bool> prefRememberMusic;
        private MelonPreferences_Entry<bool> prefDisableIngameMusic;
        private MelonPreferences_Entry<bool> prefMuteCarnivalTunnel;

        private UnityEngine.AudioSource _bgMusicSource;
        private string _currentClipName = null;
        private bool _isTracking = false;
        private object _trackingRoutine = null;

        private bool _disableIngameMusic = false;
        private bool _muteCarnivalTunnel = false;
        private readonly Dictionary<int, float> _originalVolumes = new Dictionary<int, float>();
        private readonly List<AudioSource> _bgmSources = new List<AudioSource>();
        private int _mutedCount = 0;
        private string _currentScene = "";

        private float _lastBgmScanTime = 0f;
        private const float BGM_SCAN_INTERVAL = 2.0f;
        private int _bgmScanAttempts = 0;
        private const int MAX_BGM_SCAN_ATTEMPTS = 5;

        private const float BGM_MIN_LENGTH_SECONDS = 22f;
        private const bool REQUIRE_LOOP_FOR_BGM = true;

        private MelonPreferences_Category _prefsCategory;

        private float _originalMenuFov = -1f;
        private bool _menuFovApplied = false;

        private float _lobbyLeftTime = -1f;
        private const float LOBBY_LEAVE_DELAY = 1.0f;

        public bool SceneEnabled => _sceneEnabled;
        public int SelectedBg => _selectedBg;
        public bool RememberMusic => prefRememberMusic?.Value ?? false;
        public string LastClipName => prefLastClipName?.Value ?? string.Empty;
        public bool DisableIngameMusic => _disableIngameMusic;
        public bool MuteCarnivalTunnel => _muteCarnivalTunnel;

        public void Initialize(MelonPreferences_Category prefs)
        {
            _prefsCategory = prefs;

            prefSelectedBg = prefs.CreateEntry("MenuSelectedBackground", 2);
            prefSceneEnabled = prefs.CreateEntry("MenuSceneEnabled", false);

            _selectedBg = Mathf.Clamp(prefSelectedBg.Value, MIN_BG_ID, MAX_BG_ID);
            _sceneEnabled = prefSceneEnabled.Value;

            prefLastClipName = prefs.CreateEntry("MenuMusicLastClipName", string.Empty);
            prefRememberMusic = prefs.CreateEntry("MenuMusicRememberEnabled", false);

            prefDisableIngameMusic = prefs.CreateEntry("DisableIngameMusic", false);
            _disableIngameMusic = prefDisableIngameMusic.Value;

            prefMuteCarnivalTunnel = prefs.CreateEntry("MuteCarnivalTunnel", false);
            _muteCarnivalTunnel = prefMuteCarnivalTunnel.Value;
        }

        public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory, bool firstRun)
        {
            _selectedBg = 2;
            prefSelectedBg.Value = 2;

            if (firstRun)
            {
                SetSceneEnabled(false, prefsCategory);
            }

            prefRememberMusic.Value = false;

            SetDisableIngameMusic(false, prefsCategory);
            SetMuteCarnivalTunnel(false, prefsCategory);

            _autoDisabledForLobby = false;
            _isApplying = false;
            _currentApplyRoutine = null;
            _backups.Clear();
            _originalMenuMapRoot = null;

            if (_spawnedMenuMapClone != null)
            {
                try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
                _spawnedMenuMapClone = null;
            }

            _originalMenuFov = -1f;
            _menuFovApplied = false;
            _lobbyLeftTime = -1f;
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
                RestoreLobbyMapTransforms();
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
            _selectedBg = Mathf.Clamp(id, MIN_BG_ID, MAX_BG_ID);
            prefSelectedBg.Value = _selectedBg;
            if (prefs != null)
                prefs.SaveToFile(false);

            StartApplyBackground();
        }

        public void SetRememberMusic(bool enabled, MelonPreferences_Category prefs)
        {
            prefRememberMusic.Value = enabled;
            prefs.SaveToFile(false);
        }

        public void SetDisableIngameMusic(bool value, MelonPreferences_Category prefs)
        {
            _disableIngameMusic = value;
            prefDisableIngameMusic.Value = value;
            prefs.SaveToFile(false);

            if (!value)
            {
                if (!IsMenuScene())
                    RestoreAllBGMInScene();
            }
            else
            {
                if (!IsMenuScene())
                    EnsureBgmSourcesMutedLazy();
            }
        }

        public void SetMuteCarnivalTunnel(bool value, MelonPreferences_Category prefs)
        {
            _muteCarnivalTunnel = value;
            prefMuteCarnivalTunnel.Value = value;
            prefs.SaveToFile(false);

            RestoreAllBGMInScene();
            if (!IsMenuScene())
                EnsureBgmSourcesMutedLazy();
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
            _currentScene = sceneName;

            _originalVolumes.Clear();
            _bgmSources.Clear();
            _mutedCount = 0;
            _bgmScanAttempts = 0;
            _lastBgmScanTime = 0f;
            _lobbyLeftTime = -1f;

            if (_spawnedMenuMapClone != null)
            {
                try { UnityEngine.Object.Destroy(_spawnedMenuMapClone); } catch { }
                _spawnedMenuMapClone = null;
            }

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
                else if (_sceneEnabled)
                {
                    StartApplyBackground();
                    ApplyMenuFov();
                }
                else
                {
                    RestoreOriginalMenuFov();
                }
            }
            else
            {
                _menuFovApplied = false;
            }

            _isTracking = false;
            _bgMusicSource = null;

            if (_trackingRoutine != null)
            {
                try { MelonCoroutines.Stop(_trackingRoutine); } catch { }
                _trackingRoutine = null;
            }

            _trackingRoutine = MelonCoroutines.Start(InitMusicInScene());
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

            if (!scene.name.Equals(MENU_SCENE_NAME))
            {
                if (_disableIngameMusic || _muteCarnivalTunnel)
                    EnsureBgmSourcesMutedLazy();

                return;
            }

            if (scene.name != MENU_SCENE_NAME)
                return;

            bool inLobby = FindLocalPlayer() != null;

            if (!_wasInLobby && inLobby)
            {
                AutoDisableForLobby();
                RestoreOriginalMenuFov();
                _lobbyLeftTime = -1f;
            }

            if (_wasInLobby && !inLobby)
            {
                _lobbyLeftTime = Time.time;
            }

            _wasInLobby = inLobby;

            if (_lobbyLeftTime > 0f)
            {
                if (Time.time - _lobbyLeftTime >= LOBBY_LEAVE_DELAY)
                {
                    if (IsInMenuSceneInternal() && !IsInLobby())
                    {
                        if (_autoDisabledForLobby)
                        {
                            _sceneEnabled = true;
                            prefSceneEnabled.Value = true;
                            _autoDisabledForLobby = false;
                        }

                        if (_sceneEnabled)
                        {
                            StartApplyBackground();
                            ApplyMenuFov();
                        }
                    }

                    _lobbyLeftTime = -1f;
                }
            }

            if (_sceneEnabled && !inLobby && _originalMenuMapRoot != null)
            {
                try
                {
                    if (_originalMenuMapRoot.activeInHierarchy)
                        _originalMenuMapRoot.SetActive(false);
                }
                catch { }
            }

            if (_sceneEnabled && !inLobby && !_menuFovApplied)
            {
                ApplyMenuFov();
            }
        }

        private void ApplyMenuFov()
        {
            if (!_sceneEnabled || !IsInMenuSceneInternal() || IsInLobby())
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

        private bool IsMenuScene()
        {
            return _currentScene == MENU_SCENE_NAME;
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
                RestoreLobbyMapTransforms();
                return;
            }

            _autoDisabledForLobby = true;
            _sceneEnabled = false;
            prefSceneEnabled.Value = false;

            RestoreLobbyMapTransforms();
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

                var selectedT = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t != null && t.name == map.LobbyObjectName);

                if (selectedT == null)
                    yield break;

                EnsureBackup(map.LobbyObjectName, selectedT);

                foreach (var mi in MapInfos)
                {
                    if (string.IsNullOrEmpty(mi.LobbyObjectName))
                        continue;

                    var tMi = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(x => x != null && x.name == mi.LobbyObjectName);

                    if (tMi != null)
                        tMi.gameObject.SetActive(false);
                }

                GameObject selectedRoot = selectedT.gameObject;

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

            var backup = new TransformBackup
            {
                HasValue = true,
                Position = t.position,
                Rotation = t.rotation,
                Scale = t.localScale
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
                }
            }
        }

        private Il2Cpp.NolanBehaviour FindLocalPlayer()
        {
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
                return null;

            for (int i = 0; i < players.Length; i++)
            {
                var nb = players[i];
                if (nb == null || nb.gameObject == null)
                    continue;

                try
                {
                    var entity = nb.gameObject.GetComponent<BoltEntityIl2Cpp>();
                    if (entity != null && entity.IsOwner)
                        return nb;
                }
                catch { }
            }

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
            GameObject lobbyEnvRoot = roots.FirstOrDefault(r => r.name == "Lobby Environments");

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

            if (lobbyEnvRoot != null)
            {
                foreach (var mi in MapInfos)
                {
                    if (string.IsNullOrEmpty(mi.LobbyObjectName))
                        continue;

                    var t = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(x => x != null && x.name == mi.LobbyObjectName);

                    if (t != null)
                        t.gameObject.SetActive(false);
                }
            }
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

        private IEnumerator InitMusicInScene()
        {
            for (int i = 0; i < 60; i++)
                yield return null;

            var bgObj = GameObject.Find("Background Music");
            if (bgObj == null)
                yield break;

            _bgMusicSource = bgObj.GetComponent<UnityEngine.AudioSource>();
            if (_bgMusicSource == null)
                yield break;

            if (RememberMusic)
            {
                string savedName = prefLastClipName.Value;
                if (!string.IsNullOrEmpty(savedName))
                    TryApplySavedClip(savedName);
            }

            _isTracking = true;
            MelonCoroutines.Start(TrackMusicChanges());
        }

        private IEnumerator TrackMusicChanges()
        {
            while (_isTracking && _bgMusicSource != null)
            {
                if (RememberMusic)
                {
                    var clip = _bgMusicSource.clip;
                    string nameNow = (clip != null) ? clip.name : null;

                    if (!string.IsNullOrEmpty(nameNow) && nameNow != _currentClipName)
                    {
                        _currentClipName = nameNow;
                        prefLastClipName.Value = _currentClipName;
                        _prefsCategory?.SaveToFile(false);
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        private void TryApplySavedClip(string savedName)
        {
            if (_bgMusicSource == null)
                return;

            if (_bgMusicSource.clip != null && _bgMusicSource.clip.name == savedName)
            {
                _currentClipName = savedName;
                return;
            }

            UnityEngine.AudioClip target = null;
            var allClips = Resources.FindObjectsOfTypeAll<UnityEngine.AudioClip>();
            foreach (var clip in allClips)
            {
                if (clip != null && clip.name == savedName)
                {
                    target = clip;
                    break;
                }
            }

            if (target == null)
                return;

            _bgMusicSource.clip = target;
            _bgMusicSource.time = 0f;
            _bgMusicSource.Play();

            _currentClipName = savedName;
        }

        private bool IsBGMSource(AudioSource source)
        {
            if (source == null)
                return false;

            AudioClip clip = null;
            try
            {
                clip = source.clip;
            }
            catch { }

            if (clip == null)
                return false;

            string clipNameUpper = "";
            try
            {
                clipNameUpper = clip.name.ToUpperInvariant();
            }
            catch { }

            bool isCarnivalScene = (_currentScene == "Carnival");
            bool isTunnel = clipNameUpper.Contains("CARNIVAL_AMB_RT_SPINNINGTUNNEL");

            bool isExcludedFromBgmMute =
                clipNameUpper.Contains("CARNIVAL_AMB_CAROUSEL") ||

                clipNameUpper == "BETTER-RAIN-W" ||
                clipNameUpper == "BETTER-RAIN-WIND" ||
                clipNameUpper == "BETTER-RAIN" ||
                clipNameUpper == "BETTER-RAIN-WIND-W" ||
                clipNameUpper == "MANOR_OUTSIDE AMB_7-10" ||
                clipNameUpper == "MANOR_OUTSIDE AMB_4-6" ||
                clipNameUpper == "MANOR_OUTSIDE AMB_5-7" ||
                clipNameUpper == "MANOR_OUTSIDE AMB_5-6" ||
                clipNameUpper == "MANOR_OUTSIDE AMB_4-7" ||
                clipNameUpper == "DEVOUR_THE TOWN_SANDSTORM_LEVELS4-6" ||
                clipNameUpper == "TOWN_SANDSTORM_LEVELS4-6" ||
                clipNameUpper == "DEVOUR_THE TOWN_SANDSTORM_LEVELS7-10" ||
                clipNameUpper == "TOWN_SANDSTORM_LEVELS7-10" ||
                clipNameUpper == "DEVOUR_THE MANOR_THUNDER_3" ||
                clipNameUpper == "DEVOUR_THE MANOR_THUNDER_1" ||
                clipNameUpper == "THUNDER LIGHTNING AUDIO SOURCE";

            if (isExcludedFromBgmMute)
                return false;

            float length = 0f;
            try
            {
                length = clip.length;
            }
            catch { }

            bool isLongMusic = length >= BGM_MIN_LENGTH_SECONDS;

            bool isLooping = false;
            if (REQUIRE_LOOP_FOR_BGM)
            {
                try
                {
                    isLooping = source.loop;
                }
                catch { }
            }

            bool isRegularBgm = isLongMusic && isLooping;

            if (!_disableIngameMusic && !_muteCarnivalTunnel)
                return false;

            if (!_disableIngameMusic && _muteCarnivalTunnel)
            {
                return isCarnivalScene && isTunnel;
            }

            if (_disableIngameMusic && !_muteCarnivalTunnel)
            {
                if (isCarnivalScene && isTunnel)
                    return false;

                return isRegularBgm;
            }

            if (_disableIngameMusic && _muteCarnivalTunnel)
            {
                if (isRegularBgm)
                    return true;

                if (isCarnivalScene && isTunnel)
                    return true;

                return false;
            }

            return false;
        }

        private void EnsureBgmSourcesMutedLazy()
        {
            if (_bgmSources.Count > 0)
            {
                MuteCachedBgmSources();
                return;
            }

            if (_bgmScanAttempts >= MAX_BGM_SCAN_ATTEMPTS)
                return;

            if (Time.time - _lastBgmScanTime < BGM_SCAN_INTERVAL)
                return;

            _lastBgmScanTime = Time.time;
            _bgmScanAttempts++;

            AudioSource[] allSources = null;
            try
            {
                allSources = GameObject.FindObjectsOfType<AudioSource>();
            }
            catch
            {
                return;
            }

            if (allSources == null || allSources.Length == 0)
                return;

            _bgmSources.Clear();
            _originalVolumes.Clear();
            _mutedCount = 0;

            foreach (var source in allSources)
            {
                if (source == null)
                    continue;

                try
                {
                    if (!IsBGMSource(source))
                        continue;

                    int id = source.GetInstanceID();

                    if (!_originalVolumes.ContainsKey(id))
                        _originalVolumes[id] = source.volume;

                    _bgmSources.Add(source);
                }
                catch { }
            }

            if (_bgmSources.Count > 0)
                MuteCachedBgmSources();
        }

        private void MuteCachedBgmSources()
        {
            foreach (var source in _bgmSources)
            {
                if (source == null) continue;

                try
                {
                    if (source.volume > 0f)
                    {
                        source.volume = 0f;
                        _mutedCount++;
                    }
                }
                catch { }
            }
        }

        private void RestoreAllBGMInScene()
        {
            if (_bgmSources.Count == 0 && _originalVolumes.Count == 0)
                return;

            foreach (var source in _bgmSources)
            {
                if (source == null)
                    continue;

                try
                {
                    int id = source.GetInstanceID();
                    if (_originalVolumes.TryGetValue(id, out float vol))
                        source.volume = vol;
                }
                catch { }
            }

            _mutedCount = 0;
            _bgmSources.Clear();
            _originalVolumes.Clear();
            _bgmScanAttempts = 0;
            _lastBgmScanTime = 0f;
        }

        private void DumpAllButtons()
        {
            try
            {
                var buttons = GameObject.FindObjectsOfType<Button>(true);
                var sceneName = SceneManager.GetActiveScene().name;
                MelonLogger.Msg($"[Menu][Buttons] Dumping {buttons.Length} Buttons in scene '{sceneName}':");

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

                    MelonLogger.Msg($"[Menu][Button] name='{name}', activeSelf={activeSelf}, activeHierarchy={activeHierarchy}, text='{text}', path='{path}'");
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[Menu][Buttons] Dump failed: {ex}");
            }
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