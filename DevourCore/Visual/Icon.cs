using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DevourCore
{
    public class Icon
    {
        private float hue = 0f;
        private float sat = 1f;
        private float val = 1f;
        private Color currentColor = Color.white;
        private readonly List<Image> rankImages = new List<Image>();
        private bool colorChanged = false;
        private Texture2D colorTexture;
        private bool hsvModEnabled = false;

        private MelonPreferences_Entry<float> prefHue, prefSat, prefVal;
        private MelonPreferences_Entry<bool> prefHsvEnabled;

        public bool HsvModEnabled => hsvModEnabled;
        public Color CurrentColor => currentColor;
        public List<Image> RankImages => rankImages;
        public float Hue => hue;
        public float Sat => sat;
        public float Val => val;
        public Texture2D ColorTexture => colorTexture;

        public void Initialize(MelonPreferences_Category prefs)
        {
            prefHue = prefs.CreateEntry("Hue", 1f);
            prefSat = prefs.CreateEntry("Saturation", 1f);
            prefVal = prefs.CreateEntry("Value", 1f);
            prefHsvEnabled = prefs.CreateEntry("HsvEnabled", false);

            hue = prefHue.Value;
            sat = prefSat.Value;
            val = prefVal.Value;
            hsvModEnabled = prefHsvEnabled.Value;

            EnsureColorTexture();
        }

        public void ApplyFirstRunDefaults(MelonPreferences_Category prefs)
        {
            SetEnabled(false, prefs);

            hue = 1f;
            sat = 1f;
            val = 1f;

            prefHue.Value = hue;
            prefSat.Value = sat;
            prefVal.Value = val;

            currentColor = Color.HSVToRGB(hue, sat, val);
            colorChanged = true;

            EnsureColorTexture();
            colorTexture.SetPixel(0, 0, currentColor);
            colorTexture.Apply();
        }

        public void OnSceneLoaded()
        {
            rankImages.Clear();
            var images = GameObject.FindObjectsOfType<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                var img = images[i];
                if (img.sprite?.name == "rank-17" && !rankImages.Contains(img))
                {
                    rankImages.Add(img);
                    img.color = hsvModEnabled ? currentColor : Color.white;
                }
            }
        }

        public void OnUpdate()
        {
            if (hsvModEnabled && colorChanged)
            {
                for (int i = 0; i < rankImages.Count; i++)
                    if (rankImages[i] != null) rankImages[i].color = currentColor;
                colorChanged = false;
            }

            if (hsvModEnabled)
            {
                for (int i = rankImages.Count - 1; i >= 0; i--)
                {
                    var img = rankImages[i];
                    if (img == null || img.sprite?.name != "rank-17") rankImages.RemoveAt(i);
                    else img.color = currentColor;
                }
            }
        }

        public void SetEnabled(bool enabled, MelonPreferences_Category prefs)
        {
            hsvModEnabled = enabled;
            prefHsvEnabled.Value = enabled;
            prefs.SaveToFile(false);
            colorChanged = true;

            if (!hsvModEnabled)
            {
                for (int i = 0; i < rankImages.Count; i++)
                    if (rankImages[i] != null) rankImages[i].color = Color.white;
            }
            else
            {
                for (int i = 0; i < rankImages.Count; i++)
                    if (rankImages[i] != null) rankImages[i].color = currentColor;
            }
        }

        public void SetHSV(float h, float s, float v, MelonPreferences_Category prefs)
        {
            hue = h;
            sat = s;
            val = v;

            currentColor = Color.HSVToRGB(hue, sat, val);
            colorChanged = true;

            prefHue.Value = hue;
            prefSat.Value = sat;
            prefVal.Value = val;
            prefs.SaveToFile(false);

            EnsureColorTexture();
            colorTexture.SetPixel(0, 0, currentColor);
            colorTexture.Apply();

            if (hsvModEnabled)
            {
                for (int i = 0; i < rankImages.Count; i++)
                    if (rankImages[i] != null) rankImages[i].color = currentColor;
            }
        }

        public void HandleImageOnEnable(Image image)
        {
            if (image.sprite?.name == "rank-17")
            {
                if (!rankImages.Contains(image))
                {
                    rankImages.Add(image);
                    image.color = hsvModEnabled ? currentColor : Color.white;
                }
            }
        }

        public void HandleImageSetSprite(Image image, Sprite value)
        {
            if (value?.name == "rank-17")
            {
                if (!rankImages.Contains(image))
                {
                    rankImages.Add(image);
                    image.color = hsvModEnabled ? currentColor : Color.white;
                }
            }
        }

        private void EnsureColorTexture()
        {
            if (colorTexture != null) return;

            currentColor = Color.HSVToRGB(hue, sat, val);
            colorTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            colorTexture.SetPixel(0, 0, currentColor);
            colorTexture.Apply();
            colorTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            UnityEngine.Object.DontDestroyOnLoad(colorTexture);
        }
    }
}