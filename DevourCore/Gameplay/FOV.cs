using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;

namespace DevourCore
{
    public class FOV
    {
        private const float MIN_FOV = 50f;
        private const float MAX_FOV = 110f;
        private const float DEFAULT_FOV = 100f;

        private float targetFOV = DEFAULT_FOV;
        private float lastCustomFOV = DEFAULT_FOV;
        private float originalGameFOV = DEFAULT_FOV;
        private float originalMenuFOV = -1f;

        private bool fovModEnabled = false;

        private Il2CppArrayBase<Camera> allCameras = null;
        private KeyCode fovToggleKey = KeyCode.F6;
        private bool isCapturingFovKey = false;
        private bool gameFOVCaptured = false;

        private MelonPreferences_Entry<float> prefLastFov;
        private MelonPreferences_Entry<bool> prefFovEnabled;
        private MelonPreferences_Entry<KeyCode> prefFovToggleKey;
        private MelonPreferences_Entry<float> prefMenuFOV;

        private MelonPreferences_Category prefs;

        private bool _inValidMap = false;

        public bool FovModEnabled => fovModEnabled;
        public float TargetFOV => targetFOV;
        public KeyCode FovToggleKey => fovToggleKey;
        public bool IsCapturingFovKey => isCapturingFovKey;

        public void Initialize(MelonPreferences_Category prefsCategory)
        {
            prefs = prefsCategory;

            prefLastFov = prefs.CreateEntry("LastFov", DEFAULT_FOV);
            prefFovEnabled = prefs.CreateEntry("FovEnabled", false);
            prefFovToggleKey = prefs.CreateEntry("FovToggleKey", KeyCode.F6);
            prefMenuFOV = prefs.CreateEntry("MenuFOV", -1f);

            lastCustomFOV = Mathf.Clamp(prefLastFov.Value, MIN_FOV, MAX_FOV);
            targetFOV = lastCustomFOV;
            fovModEnabled = prefFovEnabled.Value;
            fovToggleKey = prefFovToggleKey.Value;
            originalMenuFOV = prefMenuFOV.Value;
        }

        public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
        {
            SetFovEnabled(false, prefsCategory);

            targetFOV = DEFAULT_FOV;
            lastCustomFOV = DEFAULT_FOV;
            prefLastFov.Value = DEFAULT_FOV;

            fovToggleKey = KeyCode.RightAlt;
            prefFovToggleKey.Value = KeyCode.RightAlt;

            originalGameFOV = DEFAULT_FOV;
            originalMenuFOV = -1f;
            prefMenuFOV.Value = -1f;
            gameFOVCaptured = false;
            allCameras = null;

            _inValidMap = false;

            prefsCategory.SaveToFile(false);
        }

        public void OnSceneLoaded(bool inValidMap)
        {
            _inValidMap = inValidMap;

            allCameras = null;
            gameFOVCaptured = false;

            if (!inValidMap)
            {
                var cams = UnityEngine.Object.FindObjectsOfType<Camera>(true);
                if (cams != null)
                {
                    for (int i = 0; i < cams.Length; i++)
                    {
                        var cam = cams[i];
                        if (cam != null && cam.gameObject.activeInHierarchy)
                        {
                            if (originalMenuFOV > 0)
                            {
                                cam.fieldOfView = originalMenuFOV;
                            }
                            else
                            {
                                originalMenuFOV = cam.fieldOfView;
                                prefMenuFOV.Value = originalMenuFOV;
                                prefs.SaveToFile(false);
                            }
                        }
                    }
                }
            }
        }

        public void OnUpdate(bool inValidMap)
        {
            _inValidMap = inValidMap;

            if (isCapturingFovKey)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key) && key != KeyCode.Escape && key != KeyCode.None)
                    {
                        fovToggleKey = key;
                        prefFovToggleKey.Value = key;
                        prefs.SaveToFile(false);
                        isCapturingFovKey = false;
                        break;
                    }
                }
            }

            if (!_inValidMap)
                return;

            if (Input.GetKeyDown(fovToggleKey) && !isCapturingFovKey)
            {
                fovModEnabled = !fovModEnabled;
                prefFovEnabled.Value = fovModEnabled;
                prefs.SaveToFile(false);

                EnsureCamerasCached();
                EnsureOriginalGameFovCaptured();

                float value = fovModEnabled ? lastCustomFOV : originalGameFOV;
                ApplyFovToAllCameras(value);
            }

            if (!fovModEnabled)
                return;

            EnsureCamerasCached();
            EnsureOriginalGameFovCaptured();

            ApplyFovToAllCameras(targetFOV);
        }

        public void SetFovEnabled(bool enabled, MelonPreferences_Category prefsCategory)
        {

            fovModEnabled = enabled;
            prefFovEnabled.Value = enabled;
            prefs.SaveToFile(false);

            if (!_inValidMap)
                return;

            EnsureCamerasCached();
            EnsureOriginalGameFovCaptured();

            if (allCameras != null)
            {
                float value = fovModEnabled ? lastCustomFOV : originalGameFOV;
                ApplyFovToAllCameras(value);
            }
        }

        public void SetTargetFOV(float fov, MelonPreferences_Category prefsCategory, bool inValidMap)
        {
            targetFOV = Mathf.Clamp(fov, MIN_FOV, MAX_FOV);
            lastCustomFOV = targetFOV;

            prefLastFov.Value = targetFOV;
            prefs.SaveToFile(false);

            if (!inValidMap || !_inValidMap || !fovModEnabled)
                return;

            EnsureCamerasCached();
            ApplyFovToAllCameras(targetFOV);
        }

        public void StartCapturingKey()
        {
            isCapturingFovKey = true;
        }

        private void EnsureCamerasCached()
        {
            if (allCameras != null)
                return;

            try
            {
                allCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);
            }
            catch
            {
                allCameras = null;
            }
        }

        private void EnsureOriginalGameFovCaptured()
        {
            if (gameFOVCaptured || allCameras == null)
                return;

            for (int i = 0; i < allCameras.Length; i++)
            {
                var cam = allCameras[i];
                if (cam != null && cam.gameObject.activeInHierarchy)
                {
                    originalGameFOV = cam.fieldOfView;
                    gameFOVCaptured = true;
                    break;
                }
            }
        }

        private void ApplyFovToAllCameras(float fov)
        {
            if (allCameras == null)
                return;

            for (int i = 0; i < allCameras.Length; i++)
            {
                var cam = allCameras[i];
                if (cam != null && cam.gameObject.activeInHierarchy)
                    cam.fieldOfView = fov;
            }
        }
    }
}
