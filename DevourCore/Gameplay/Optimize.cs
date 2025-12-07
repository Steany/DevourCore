using MelonLoader;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace DevourCore
{
    public class Optimize
    {
        private const float MIN_CULL_RADIUS = 6f;
        private const float MAX_CULL_RADIUS = 30f;
        private const float DEFAULT_CULL_RADIUS = 20f;

        private float _cullRadius = DEFAULT_CULL_RADIUS;
        private bool _cullEnabled = true;
        private bool _applyInMenu = false;
        private KeyCode _cullToggleKey = KeyCode.RightControl;
        private bool _isCapturingCullKey = false;

        private struct CamState
        {
            public float Far;
            public bool LayerCullSpherical;
            public float[] LayerCullDistances;
            public bool HadDistances;
        }

        private readonly Dictionary<int, CamState> originalById = new Dictionary<int, CamState>();
        private float[] sharedCullDistances = null;
        private float sharedValue = -1f;

        private bool _disableWeather = true;
        private readonly HashSet<ParticleSystem> _handledWeather = new HashSet<ParticleSystem>();
        private readonly string[] _weatherKeywords = new[]
        {
            "rain", "snow", "dust", "splash", "debris", "sandstorm",
            "tumbleweed", "wind", "leaf", "blizzard", "blowingsnow"
        };
        private object _weatherCoroutineToken;

        private MelonPreferences_Entry<float> prefCullRadius;
        private MelonPreferences_Entry<bool> prefCullEnabled;
        private MelonPreferences_Entry<bool> prefApplyInMenu;
        private MelonPreferences_Entry<bool> prefWeatherDisabled;
        private MelonPreferences_Entry<KeyCode> prefCullToggleKey;

        private MelonPreferences_Category prefs;

        private bool inValidMap = false;
        private bool inMenu = false;

        private Il2CppArrayBase<Camera> _cachedCameras = null;

        internal static Optimize Instance;

        internal static readonly HashSet<string> WeatherMutedNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ThunderClap_A",
                "ThunderClap_B",
                "ThunderClap_C",
                "ThunderClap_D",
                "better-rain-w",
                "better-rain-wind",
                "better-rain",
                "better-rain-wind-w",
                "Manor_Outside Amb_7-10",
                "Manor_Outside Amb_4-6",
                "Manor_Outside Amb_5-7",
                "Devour_The Manor_Outside Amb_5-7",
                "Devour_The Town_Sandstorm_Levels4-6",
                "Town_Sandstorm_Levels4-6",
                "Devour_The Town_Sandstorm_Levels7-10",
                "Town_Sandstorm_Levels7-10",
                "Devour_The Manor_Thunder_3",
                "Devour_The Manor_Thunder_1",
                "Thunder Lightning Audio Source" +
                "Weather_DryThunder_1" +
                "Weather_DryThunder_2" +
                "Weather_DryThunder_3" +
                "Weather_DryThunder_4" +
                "Weather_DryThunder_5",
            };

        internal static bool ShouldMuteWeatherAudio(AudioSource src, AudioClip clip)
        {
            var inst = Instance;
            if (inst == null)
                return false;

            if (!inst._disableWeather || !inst.inValidMap)
                return false;

            string goName = null;
            string clipName = null;

            try { goName = src?.gameObject?.name; } catch { }
            try { clipName = clip?.name; } catch { }

            if (!string.IsNullOrEmpty(goName) && WeatherMutedNames.Contains(goName))
                return true;

            if (!string.IsNullOrEmpty(clipName) && WeatherMutedNames.Contains(clipName))
                return true;

            return false;
        }

        public bool CullEnabled => _cullEnabled;
        public bool IsCapturingCullKey => _isCapturingCullKey;
        public KeyCode CullToggleKey => _cullToggleKey;
        public float CullRadius => _cullRadius;
        public bool ApplyInMenu => _applyInMenu;
        public bool DisableWeather => _disableWeather;

        public void Initialize(MelonPreferences_Category prefsCategory)
        {
            prefs = prefsCategory;

            Instance = this;

            prefCullRadius = prefs.CreateEntry("CullRadius", DEFAULT_CULL_RADIUS);
            prefCullEnabled = prefs.CreateEntry("CullEnabled", false);
            prefApplyInMenu = prefs.CreateEntry("ApplyInMenu", false);
            prefWeatherDisabled = prefs.CreateEntry("WeatherDisabled", false);
            prefCullToggleKey = prefs.CreateEntry("CullToggleKey", KeyCode.RightControl);

            _cullRadius = Mathf.Clamp(prefCullRadius.Value, MIN_CULL_RADIUS, MAX_CULL_RADIUS);
            _cullEnabled = prefCullEnabled.Value;
            _applyInMenu = prefApplyInMenu.Value;
            _disableWeather = prefWeatherDisabled.Value;
            _cullToggleKey = prefCullToggleKey.Value;
        }

        public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
        {
            SetCullEnabled(false, prefsCategory);
            SetApplyInMenu(false, prefsCategory);

            SetCullRadius(DEFAULT_CULL_RADIUS, prefsCategory);

            SetWeatherDisabled(false, prefsCategory, inMenu: true, inValidMap: false);

            RestoreAllCameras();
            originalById.Clear();
            _cachedCameras = null;
            sharedCullDistances = null;
            sharedValue = -1f;
        }

        public void UpdateSceneState(bool validMap, bool menu)
        {
            inValidMap = validMap;
            inMenu = menu;
        }

        public void OnSceneLoaded(bool inValidMap)
        {
            RestoreAllCameras();
            originalById.Clear();
            _cachedCameras = null;
            sharedCullDistances = null;
            sharedValue = -1f;

            if (_disableWeather && inValidMap)
            {
                if (_weatherCoroutineToken != null)
                {
                    MelonCoroutines.Stop(_weatherCoroutineToken);
                    _weatherCoroutineToken = null;
                }
                _handledWeather.Clear();
                _weatherCoroutineToken = MelonCoroutines.Start(TryDisableWeather());
            }
        }

        public void OnUpdate()
        {
            HandleCullKeyCapture();
            HandleCullToggle();

            if (!_cullEnabled)
                return;

            ApplyCameraSettings();
        }

        private void HandleCullKeyCapture()
        {
            if (_isCapturingCullKey)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        _cullToggleKey = key;
                        prefCullToggleKey.Value = key;
                        prefs.SaveToFile(false);
                        _isCapturingCullKey = false;
                        break;
                    }
                }
            }
        }

        private void HandleCullToggle()
        {
            if (Input.GetKeyDown(_cullToggleKey) && !_isCapturingCullKey)
            {
                _cullEnabled = !_cullEnabled;
                prefCullEnabled.Value = _cullEnabled;
                prefs.SaveToFile(false);

                if (!_cullEnabled)
                {
                    RestoreAllCameras();
                    originalById.Clear();
                    _cachedCameras = null;
                    sharedCullDistances = null;
                    sharedValue = -1f;
                }
            }
        }

        private void ApplyCameraSettings()
        {
            bool inActiveScene = inValidMap || inMenu;
            if (!inActiveScene)
                return;

            bool shouldApplyCustom = _cullEnabled && (inValidMap || (inMenu && _applyInMenu));
            if (!shouldApplyCustom)
            {
                if (originalById.Count > 0)
                    RestoreAllCameras();
                return;
            }

            EnsureCamerasCached();

            if (_cachedCameras == null || _cachedCameras.Length == 0)
                return;

            float clamped = Mathf.Clamp(_cullRadius, MIN_CULL_RADIUS, MAX_CULL_RADIUS);
            if (sharedCullDistances == null || !Mathf.Approximately(sharedValue, clamped))
            {
                sharedCullDistances = new float[32];
                for (int i = 0; i < 32; i++)
                    sharedCullDistances[i] = clamped;
                sharedValue = clamped;
            }

            try
            {
                for (int i = 0; i < _cachedCameras.Length; i++)
                {
                    var cam = _cachedCameras[i];
                    if (cam == null) continue;

                    int id = cam.GetInstanceID();

                    if (!originalById.ContainsKey(id))
                    {
                        CamState st = new CamState
                        {
                            Far = cam.farClipPlane,
                            LayerCullSpherical = cam.layerCullSpherical
                        };

                        var ld = cam.layerCullDistances;
                        if (ld != null && ld.Length == 32)
                        {
                            st.HadDistances = true;
                            st.LayerCullDistances = new float[32];
                            for (int j = 0; j < 32; j++)
                                st.LayerCullDistances[j] = ld[j];
                        }
                        else
                        {
                            st.HadDistances = false;
                            st.LayerCullDistances = null;
                        }

                        originalById[id] = st;
                    }

                    cam.farClipPlane = sharedValue;
                    cam.layerCullSpherical = true;
                    cam.layerCullDistances = sharedCullDistances;
                }
            }
            catch { }
        }

        private void EnsureCamerasCached()
        {
            if (_cachedCameras != null)
                return;

            try
            {
                _cachedCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);
            }
            catch
            {
                _cachedCameras = null;
            }
        }

        public void RestoreAllCameras()
        {
            try
            {
                if (_cachedCameras == null)
                {
                    _cachedCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);
                }

                if (_cachedCameras == null || _cachedCameras.Length == 0)
                    return;

                foreach (var cam in _cachedCameras)
                {
                    if (cam == null) continue;

                    int id = cam.GetInstanceID();
                    if (!originalById.TryGetValue(id, out CamState st))
                        continue;

                    cam.farClipPlane = st.Far;
                    cam.layerCullSpherical = st.LayerCullSpherical;

                    if (st.HadDistances && st.LayerCullDistances != null)
                    {
                        var restore = new float[st.LayerCullDistances.Length];
                        for (int j = 0; j < st.LayerCullDistances.Length; j++)
                            restore[j] = st.LayerCullDistances[j];
                        cam.layerCullDistances = restore;
                    }
                }
            }
            catch { }
        }

        private IEnumerator TryDisableWeather()
        {
            for (int i = 0; i < 12; i++)
            {
                DisableNewWeather();
                yield return new WaitForSeconds(1);
            }
            _weatherCoroutineToken = null;
        }

        private void DisableNewWeather()
        {
            ParticleSystem[] particleSystems = null;
            try
            {
                particleSystems = GameObject.FindObjectsOfType<ParticleSystem>(true);
            }
            catch
            {
                return;
            }

            if (particleSystems == null || particleSystems.Length == 0)
                return;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                var ps = particleSystems[i];
                if (ps == null || _handledWeather.Contains(ps))
                    continue;

                string name = ps.gameObject?.name;
                if (string.IsNullOrEmpty(name))
                    continue;

                for (int j = 0; j < _weatherKeywords.Length; j++)
                {
                    if (name.IndexOf(_weatherKeywords[j], StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var main = ps.main;
                        main.loop = false;
                        main.playOnAwake = false;
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        ps.gameObject.SetActive(false);
                        _handledWeather.Add(ps);
                        break;
                    }
                }
            }
        }

        public void StartCapturingKey()
        {
            _isCapturingCullKey = true;
        }

        public void SetCullEnabled(bool enabled, MelonPreferences_Category prefsCategory)
        {
            _cullEnabled = enabled;
            prefCullEnabled.Value = enabled;
            prefs.SaveToFile(false);

            if (!_cullEnabled)
            {
                RestoreAllCameras();
                originalById.Clear();
                _cachedCameras = null;
                sharedCullDistances = null;
                sharedValue = -1f;
            }
        }

        public void SetApplyInMenu(bool apply, MelonPreferences_Category prefsCategory)
        {
            _applyInMenu = apply;
            prefApplyInMenu.Value = apply;
            prefs.SaveToFile(false);
        }

        public void SetCullRadius(float radius, MelonPreferences_Category prefsCategory)
        {
            _cullRadius = Mathf.Clamp(radius, MIN_CULL_RADIUS, MAX_CULL_RADIUS);
            prefCullRadius.Value = _cullRadius;
            prefs.SaveToFile(false);
        }

        public void SetWeatherDisabled(bool disabled, MelonPreferences_Category prefsCategory,
            bool inMenu, bool inValidMap)
        {
            if (!inMenu && prefsCategory != null) return;

            _disableWeather = disabled;
            prefWeatherDisabled.Value = disabled;
            prefs.SaveToFile(false);

            if (_disableWeather && inValidMap)
            {
                if (_weatherCoroutineToken != null)
                {
                    MelonCoroutines.Stop(_weatherCoroutineToken);
                    _weatherCoroutineToken = null;
                }
                _handledWeather.Clear();
                _weatherCoroutineToken = MelonCoroutines.Start(TryDisableWeather());
            }
            else if (!_disableWeather && _weatherCoroutineToken != null)
            {
                MelonCoroutines.Stop(_weatherCoroutineToken);
                _weatherCoroutineToken = null;
            }
        }
    }
}