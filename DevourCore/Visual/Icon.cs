using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DevourCore
{
	public class Icon
	{
		
		private static readonly string[] RankSpriteNames =
		{
			"rank-10", 
            "rank-11", 
            "rank-12", 
            "rank-13", 
            "rank-14", 
            "rank-15", 
            "rank-16", 
            "rank-17"  
        };

		private static readonly int[] RankLevels =
		{
			70,   
            100,  
            200,  
            300,  
            400,  
            500,  
            600,  
            666   
        };

		
		private static readonly string[] UntintedRankSpriteNames =
		{
			"rank-1",
			"rank-2",
			"rank-3",
			"rank-4",
			"rank-5",
			"rank-6",
			"rank-7",
			"rank-8",
			"rank-9"
		};

		private const int RankCount = 8;

		private readonly List<Image> rankImages = new List<Image>();

		private float hue = 1f;
		private float sat = 1f;
		private float val = 1f;
		private Color currentColor = Color.white;

		private Texture2D colorTexture;
		private bool hsvModEnabled = false;

		private MelonPreferences_Entry<float> prefHue;
		private MelonPreferences_Entry<float> prefSat;
		private MelonPreferences_Entry<float> prefVal;
		private MelonPreferences_Entry<bool> prefHsvEnabled;
		private MelonPreferences_Entry<int> prefCurrentRankIndex;

		private int currentRankIndex = RankCount - 1;

		public bool HsvModEnabled => hsvModEnabled;
		public Color CurrentColor => currentColor;
		public List<Image> RankImages => rankImages;
		public float Hue => hue;
		public float Sat => sat;
		public float Val => val;
		public Texture2D ColorTexture => colorTexture;
		public int CurrentRankLevel => RankLevels[currentRankIndex];

		public void Initialize(MelonPreferences_Category prefs)
		{
			prefHue = prefs.CreateEntry("Icon_Hue", 0.7f);
			prefSat = prefs.CreateEntry("Icon_Sat", 1f);
			prefVal = prefs.CreateEntry("Icon_Val", 1f);

			hue = prefHue.Value;
			sat = prefSat.Value;
			val = prefVal.Value;

			currentColor = Color.HSVToRGB(hue, sat, val);

			prefHsvEnabled = prefs.CreateEntry("HsvEnabled", false);
			hsvModEnabled = prefHsvEnabled.Value;

			prefCurrentRankIndex = prefs.CreateEntry("Icon_CurrentRankIndex", 7);
			currentRankIndex = Mathf.Clamp(prefCurrentRankIndex.Value, 0, RankCount - 1);

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

			currentRankIndex = RankCount - 1;
			prefCurrentRankIndex.Value = currentRankIndex;

			prefs.SaveToFile(false);

			EnsureColorTexture();
		}

		public void OnSceneLoaded()
		{
			rankImages.Clear();

			var images = GameObject.FindObjectsOfType<Image>();
			for (int i = 0; i < images.Length; i++)
			{
				var img = images[i];
				string spriteName = img.sprite?.name;
				int idx = GetRankIndexFromSpriteName(spriteName); 
				bool untinted = IsUntintedRankSpriteName(spriteName);

				if (idx >= 0 || untinted)
				{
					if (!rankImages.Contains(img))
					{
						rankImages.Add(img);
					}

					if (hsvModEnabled && idx >= 0 && idx == currentRankIndex)
						img.color = currentColor;
					else
						img.color = Color.white;
				}
			}
		}

		public void OnUpdate()
		{
			for (int i = rankImages.Count - 1; i >= 0; i--)
			{
				var img = rankImages[i];
				if (img == null)
				{
					rankImages.RemoveAt(i);
					continue;
				}

				string spriteName = img.sprite?.name;
				int idx = GetRankIndexFromSpriteName(spriteName);
				bool untinted = IsUntintedRankSpriteName(spriteName);

				if (idx < 0 && !untinted)
				{
					rankImages.RemoveAt(i);
					continue;
				}

				if (hsvModEnabled && idx >= 0 && idx == currentRankIndex)
					img.color = currentColor;
				else
					img.color = Color.white;
			}
		}

		public void SetEnabled(bool enabled, MelonPreferences_Category prefs)
		{
			hsvModEnabled = enabled;
			prefHsvEnabled.Value = enabled;
			prefs.SaveToFile(false);

			if (!hsvModEnabled)
			{
				for (int i = 0; i < rankImages.Count; i++)
					if (rankImages[i] != null) rankImages[i].color = Color.white;
			}
			else
			{
				for (int i = 0; i < rankImages.Count; i++)
				{
					var img = rankImages[i];
					if (img == null) continue;

					string spriteName = img.sprite?.name;
					int idx = GetRankIndexFromSpriteName(spriteName);

					if (idx >= 0 && idx == currentRankIndex)
						img.color = currentColor;
					else
						img.color = Color.white;
				}
			}
		}

		public void SetHSV(float h, float s, float v, MelonPreferences_Category prefs)
		{
			hue = h;
			sat = s;
			val = v;

			currentColor = Color.HSVToRGB(hue, sat, val);

			prefHue.Value = hue;
			prefSat.Value = sat;
			prefVal.Value = val;
			prefs.SaveToFile(false);

			EnsureColorTexture();

			if (hsvModEnabled)
			{
				for (int i = 0; i < rankImages.Count; i++)
				{
					var img = rankImages[i];
					if (img == null) continue;

					string spriteName = img.sprite?.name;
					int ri = GetRankIndexFromSpriteName(spriteName);
					if (ri == currentRankIndex)
						img.color = currentColor;
				}
			}
		}

		public void CycleRank(MelonPreferences_Category prefs)
		{
			currentRankIndex = (currentRankIndex + 1) % RankCount;
			prefCurrentRankIndex.Value = currentRankIndex;
			prefs.SaveToFile(false);

			EnsureColorTexture();

			if (!hsvModEnabled)
				return;

			for (int i = 0; i < rankImages.Count; i++)
			{
				var img = rankImages[i];
				if (img == null) continue;

				string spriteName = img.sprite?.name;
				int idx = GetRankIndexFromSpriteName(spriteName);

				if (idx >= 0 && idx == currentRankIndex)
					img.color = currentColor;
				else
					img.color = Color.white;
			}
		}

		public void HandleImageOnEnable(Image image)
		{
			if (image == null) return;

			string spriteName = image.sprite?.name;
			int idx = GetRankIndexFromSpriteName(spriteName);
			bool untinted = IsUntintedRankSpriteName(spriteName);

			if (idx >= 0 || untinted)
			{
				if (!rankImages.Contains(image))
				{
					rankImages.Add(image);
				}

				if (hsvModEnabled && idx >= 0 && idx == currentRankIndex)
					image.color = currentColor;
				else
					image.color = Color.white;
			}
		}

		public void HandleImageSetSprite(Image image, Sprite value)
		{
			if (image == null || value == null) return;

			string spriteName = value.name;
			int idx = GetRankIndexFromSpriteName(spriteName);
			bool untinted = IsUntintedRankSpriteName(spriteName);

			if (idx >= 0 || untinted)
			{
				if (!rankImages.Contains(image))
				{
					rankImages.Add(image);
				}

				if (hsvModEnabled && idx >= 0 && idx == currentRankIndex)
					image.color = currentColor;
				else
					image.color = Color.white;
			}
		}

		
		private int GetRankIndexFromSpriteName(string spriteName)
		{
			if (string.IsNullOrEmpty(spriteName))
				return -1;

			for (int i = 0; i < RankCount; i++)
			{
				if (spriteName == RankSpriteNames[i])
					return i;
			}
			return -1;
		}

		private bool IsUntintedRankSpriteName(string spriteName)
		{
			if (string.IsNullOrEmpty(spriteName))
				return false;

			for (int i = 0; i < UntintedRankSpriteNames.Length; i++)
			{
				if (spriteName == UntintedRankSpriteNames[i])
					return true;
			}
			return false;
		}

		private void EnsureColorTexture()
		{
			if (colorTexture == null)
			{
				colorTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				colorTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
				UnityEngine.Object.DontDestroyOnLoad(colorTexture);
			}

			colorTexture.SetPixel(0, 0, currentColor);
			colorTexture.Apply();
		}
	}
}