using MelonLoader;
using UnityEngine;
using System;

namespace DevourCore
{
    public class Settings
    {
        private KeyCode _toggleGuiKey = KeyCode.RightShift;
        private bool _isCapturingMenuKey = false;
        private float themeHue = 0.75f;
        private Color themeColorPrimary = new Color(0.5f, 0.2f, 0.7f, 1f);
        private Color themeColorSecondary = new Color(1f, 0.2f, 0.8f, 1f);
        private Color themeColorDark = new Color(0.2f, 0.05f, 0.25f, 1f);
        private Color themeColorHover = new Color(0.6f, 0.3f, 0.8f, 1f);

        private MelonPreferences_Entry<KeyCode> prefMenuKey;
        private MelonPreferences_Entry<float> prefThemeHue;

        private MelonPreferences_Category prefs;

        public KeyCode ToggleGuiKey => _toggleGuiKey;
        public bool IsCapturingMenuKey => _isCapturingMenuKey;
        public float ThemeHue => themeHue;
        public Color ThemeColorPrimary => themeColorPrimary;
        public Color ThemeColorSecondary => themeColorSecondary;
        public Color ThemeColorDark => themeColorDark;
        public Color ThemeColorHover => themeColorHover;

        public void Initialize(MelonPreferences_Category prefsCategory)
        {
            prefs = prefsCategory;

            prefMenuKey = prefs.CreateEntry("MenuKey", KeyCode.RightShift);
            prefThemeHue = prefs.CreateEntry("ThemeHue", 0.75f);

            _toggleGuiKey = prefMenuKey.Value;
            themeHue = prefThemeHue.Value;
            UpdateThemeColors();
        }

        public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
        {
            prefMenuKey.Value = KeyCode.RightShift; _toggleGuiKey = KeyCode.RightShift;
            prefThemeHue.Value = 0.75f; themeHue = 0.75f;
            UpdateThemeColors();
        }

        public void OnUpdate()
        {
            if (_isCapturingMenuKey)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key) && key != KeyCode.Escape && key != KeyCode.None)
                    {
                        _toggleGuiKey = key;
                        prefMenuKey.Value = key;
                        prefs.SaveToFile(false);
                        _isCapturingMenuKey = false;
                        break;
                    }
                }
            }
        }

        public void StartCapturingKey()
        {
            _isCapturingMenuKey = true;
        }

        public void SetThemeHue(float hue, MelonPreferences_Category prefsCategory)
        {
            themeHue = hue;
            prefThemeHue.Value = hue;
            prefs.SaveToFile(false);
            UpdateThemeColors();
        }

        public void ResetThemeToDefault(MelonPreferences_Category prefsCategory)
        {
            themeHue = 0.75f;
            prefThemeHue.Value = themeHue;
            prefs.SaveToFile(false);
            UpdateThemeColors();
        }

        private void UpdateThemeColors()
        {
            float sAccent = 0.60f;
            float vAccent = 0.66f;
            float sHover = 0.58f;
            float vHover = 0.75f;
            float sDark = 0.45f;
            float vDark = 0.22f;

            themeColorPrimary = Color.HSVToRGB(themeHue, sAccent, vAccent);
            themeColorHover = Color.HSVToRGB(themeHue, sHover, vHover);
            themeColorDark = Color.HSVToRGB(themeHue, sDark, vDark);
            themeColorSecondary = Color.HSVToRGB(themeHue, sAccent * 0.95f, vAccent * 0.92f);
        }
    }
}