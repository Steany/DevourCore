using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

using BoltEntityIl2Cpp = Il2CppPhoton.Bolt.BoltEntity;

namespace DevourCore
{
    public class Menu
    {
        private const string MENU_SCENE_NAME = "Menu";

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
            new MapInfo { Id = 1, DisplayName = "Town",             LobbyObjectName = "Town",           HorizontalOffset = 1.2f, DistanceOffset = 3.0f, VerticalOffset = -0.2f },
            new MapInfo { Id = 2, DisplayName = "Manor",            LobbyObjectName = "Manor",          HorizontalOffset = 1.2f, DistanceOffset = 3.5f, VerticalOffset =  0.0f },
            new MapInfo { Id = 3, DisplayName = "Farmhouse",        LobbyObjectName = "Farmhouse",      HorizontalOffset = 1.2f, DistanceOffset = 3.5f, VerticalOffset =  0.0f },
            new MapInfo { Id = 4, DisplayName = "Asylum",           LobbyObjectName = "Asylum",         HorizontalOffset = 1.2f, DistanceOffset = 3.5f, VerticalOffset =  0.0f },
            new MapInfo { Id = 5, DisplayName = "Inn",              LobbyObjectName = "Inn",            HorizontalOffset = 1.2f, DistanceOffset = 3.5f, VerticalOffset =  0.3f },
            new MapInfo { Id = 6, DisplayName = "Slaughterhouse",   LobbyObjectName = "Slaughterhouse", HorizontalOffset = 0.5f, DistanceOffset = 1.5f, VerticalOffset =  0.0f },
            new MapInfo { Id = 7, DisplayName = "Carnival",         LobbyObjectName = "Carnival",       HorizontalOffset = 1.2f, DistanceOffset = 3.5f, VerticalOffset =  0.0f },
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
        private object _forceLoopRoutine = null;
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

        private MelonPreferences_Entry<string> prefLastClipName;
        private MelonPreferences_Entry<bool> prefRememberMusic;
        private MelonPreferences_Entry<bool> prefDisableIngameMusic;

        private UnityEngine.AudioSource _bgMusicSource;
        private string _currentClipName = null;
        private bool _isTracking = false;
        private object _trackingRoutine = null;

        private bool _disableIngameMusic = false;
        private readonly Dictionary<int, float> _originalVolumes = new Dictionary<int, float>();
        private readonly List<AudioSource> _bgmSources = new List<AudioSource>();
        private int _mutedCount = 0;
        private string _currentScene = "";

        private float _lastBgmScanTime = 0f;
        private const float BGM_SCAN_INTERVAL = 2.0f;
        private int _bgmScanAttempts = 0;
        private const int MAX_BGM_SCAN_ATTEMPTS = 5;

        private MelonPreferences_Category _prefsCategory;

        public bool SceneEnabled => _sceneEnabled;
        public int SelectedBg => _selectedBg;
        public bool RememberMusic => prefRememberMusic?.Value ?? false;
        public string LastClipName => prefLastClipName?.Value ?? string.Empty;
        public bool DisableIngameMusic => _disableIngameMusic;

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

            _autoDisabledForLobby = false;
            _isApplying = false;
            _currentApplyRoutine = null;
            _forceLoopRoutine = null;
            _backups.Clear();
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
            }
            else
            {
                MaybeApplyOnMenuChange("EnableToggle");
            }
        }

        public void SetSelectedBg(int id, MelonPreferences_Category prefs)
        {
            _selectedBg = Mathf.Clamp(id, MIN_BG_ID, MAX_BG_ID);
            prefSelectedBg.Value = _selectedBg;
            prefs.SaveToFile(false);

            MaybeApplyOnMenuChange("BgChanged");
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

            if (sceneName == MENU_SCENE_NAME)
            {
                bool inLobbyNow = FindLocalPlayer() != null;
                _wasInLobby = inLobbyNow;
                _lastMenuUiSignature = GetMenuUiSignature();

                if (inLobbyNow)
                    AutoDisableForLobby();
                else
                    MaybeApplyOnMenuChange("OnSceneWasLoaded");
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

            if (!scene.name.Equals(MENU_SCENE_NAME))
            {
                if (_disableIngameMusic)
                {
                    EnsureBgmSourcesMutedLazy();
                }
                return;
            }

            if (scene.name != MENU_SCENE_NAME)
                return;

            bool inLobby = FindLocalPlayer() != null;

            if (!inLobby && _sceneEnabled)
            {
                string sig = GetMenuUiSignature();
                if (!string.IsNullOrEmpty(sig) && sig != _lastMenuUiSignature)
                {
                    _lastMenuUiSignature = sig;
                    MaybeApplyOnMenuChange("Menu UI change");
                }
            }

            if (!_wasInLobby && inLobby)
                AutoDisableForLobby();

            if (_wasInLobby && !inLobby)
            {
                if (_autoDisabledForLobby)
                {
                    _sceneEnabled = true;
                    prefSceneEnabled.Value = true;
                    _autoDisabledForLobby = false;
                }

                MaybeApplyOnMenuChange("Lobby->Menu");
            }

            _wasInLobby = inLobby;
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

        private void MaybeApplyOnMenuChange(string reason)
        {
            if (!_sceneEnabled)
                return;

            if (!IsInMenuSceneInternal())
                return;

            if (FindLocalPlayer() != null)
                return;

            ForceReapplyBackground(reason);
        }

        private void AutoDisableForLobby()
        {
            if (!IsInMenuSceneInternal())
                return;

            if (_forceLoopRoutine != null)
            {
                try { MelonCoroutines.Stop(_forceLoopRoutine); } catch { }
                _forceLoopRoutine = null;
            }

            if (_currentApplyRoutine != null)
            {
                try { MelonCoroutines.Stop(_currentApplyRoutine); } catch { }
                _currentApplyRoutine = null;
            }

            _isApplying = false;

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

        private void ForceReapplyBackground(string reason)
        {
            if (!_sceneEnabled || !IsInMenuSceneInternal())
                return;

            if (_forceLoopRoutine != null)
            {
                try { MelonCoroutines.Stop(_forceLoopRoutine); } catch { }
                _forceLoopRoutine = null;
            }

            _forceLoopRoutine = MelonCoroutines.Start(ForceReapplyLoopCoroutine(60));
        }

        private IEnumerator ForceReapplyLoopCoroutine(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                if (!_sceneEnabled || !IsInMenuSceneInternal() || FindLocalPlayer() != null)
                    break;

                StartApplyBackground();

                float timeout = 1f;
                while (_isApplying && timeout > 0f)
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }

                yield return null;
                yield return null;
            }

            _forceLoopRoutine = null;
        }

        private void StartApplyBackground()
        {
            if (!IsInMenuSceneInternal())
                return;

            if (!_sceneEnabled)
                return;

            if (_isApplying)
                return;

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

                var carnivalT = menuEnvRoot.GetComponentsInChildren<Transform>(true)
                                           .FirstOrDefault(t => t != null && t.name == "Carnival");

                if (carnivalT == null)
                    yield break;

                GameObject carnivalRoot = carnivalT.gameObject;

                Transform cT = carnivalRoot.transform;
                Vector3 carnivalPos = cT.position;
                Quaternion carnivalRot = cT.rotation;
                Vector3 carnivalScale = cT.localScale;
                int carnivalLayer = carnivalRoot.layer;

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

                    var t = lobbyEnvRoot.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(x => x != null && x.name == mi.LobbyObjectName);

                    if (t != null && t.gameObject != selectedT.gameObject)
                        t.gameObject.SetActive(false);
                }

                GameObject selectedRoot = selectedT.gameObject;

                selectedRoot.SetActive(true);
                SetLayerRecursively(selectedRoot, carnivalLayer);

                Transform sT = selectedRoot.transform;
                sT.position = carnivalPos;
                sT.rotation = carnivalRot;
                sT.localScale = carnivalScale;

                var cam = Camera.main;
                if (cam != null)
                {
                    sT.position += cam.transform.right * map.HorizontalOffset;
                    sT.position += cam.transform.forward * map.DistanceOffset;
                    sT.position += Vector3.up * map.VerticalOffset;
                }

                carnivalRoot.SetActive(false);
            }
            finally
            {
                _isApplying = false;
            }
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

            if (menuEnvRoot == null)
                return;

            var carnivalT = menuEnvRoot.GetComponentsInChildren<Transform>(true)
                                       .FirstOrDefault(t => t != null && t.name == "Carnival");
            if (carnivalT == null)
                return;

            GameObject carnivalRoot = carnivalT.gameObject;
            carnivalRoot.SetActive(true);

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

        private static readonly string[] BgmExclusionClipNames =
        {
            "DEVOUR_THE INN_ZARA INTRO SEQUENCE"
        };

        private bool IsExcludedBgmClip(AudioClip clip)
        {
            if (clip == null) return false;

            string nameUpper = "";
            try
            {
                nameUpper = clip.name.ToUpper();
            }
            catch { }

            if (string.IsNullOrEmpty(nameUpper))
                return false;

            for (int i = 0; i < BgmExclusionClipNames.Length; i++)
            {
                if (nameUpper == BgmExclusionClipNames[i])
                    return true;
            }

            return false;
        }

        private bool IsBGMSource(AudioSource source)
        {
            if (source == null)
                return false;

            try
            {
                if (source.clip != null && IsExcludedBgmClip(source.clip))
                    return false;
            }
            catch { }

            string objName = "";
            string clipName = "";

            try
            {
                if (source.gameObject != null)
                    objName = source.gameObject.name.ToUpper();
            }
            catch { }

            try
            {
                if (source.clip != null)
                    clipName = source.clip.name.ToUpper();
            }
            catch { }

            return objName.Contains("BGM") || clipName.Contains("BGM");
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
    }
}