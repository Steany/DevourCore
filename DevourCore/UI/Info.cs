using System.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;

namespace DevourCore
{
	public class Info
	{
		private string infoOptimize;
		private string infoHSV;
		private string infoSpeedrun;
		private string infoFOV;
		private string infoGameplay;
		private string infoHUD;
		private string infoSettings;
		private string infoAnticheat;
		private string infoMenu;

		private MelonPreferences_Category _prefs;

		private const string SECTION_OPTIMIZE = "Optimize";
		private const string SECTION_HSV = "HSV";
		private const string SECTION_SPEEDRUN = "Speedrun";
		private const string SECTION_FOV = "FOV";
		private const string SECTION_GAMEPLAY = "Gameplay";
		private const string SECTION_HUD = "HUD";
		private const string SECTION_SETTINGS = "Settings";
		private const string SECTION_ANTICHEAT = "Anticheat";
		private const string SECTION_MENU = "Menu";

		private const int INFO_ICON_SIZE = 20;
		private const float INFO_ICON_TEXT_X_OFFSET = 2f;
		private const float INFO_MARGIN_RIGHT = 14f;
		private const float INFO_MARGIN_BOTTOM = 14f;
		private const float STROKE_ALPHA = 0.6f;

		private const float TITLE_BAR_HEIGHT = 28f;
		private const float OVERLAY_EDGE_MARGIN = 4f;
		private const float CONTENT_HPAD = 10f;
		private const float CONTENT_TOP_GAP = 0f;
		private const float CONTENT_VPADDING = 16f;
		private const float BASE_BOTTOM_SAFE_EXTRA = 30f;
		private const float OVERLAY_TOP_EXTRA = 5f + 13f + 6f;

		private const string DEFAULT_ACCENT_HEX = "#64B5F6";
		private const string HEX_WARN = "#FFD54A";
		private const string HEX_DANGER = "#FF3A3A";
		private const string HEX_MUTED = "#B0BEC5";

		private const int FONT_DEFAULT = 13;
		private const int FONT_SPEEDRUN = 11;
		private const int FONT_ANTICHEAT = 11;
		private const int FONT_HUD = 11;

		private bool _infoHoverIcon = false;
		private string _infoActiveKey = null;

		private string _accentHex = DEFAULT_ACCENT_HEX;

		private Texture2D infoIconBg, infoIconBgHover;
		private Texture2D infoOverlaySolidBg;

		private GUIStyle infoIconTextStyle;
		private GUIStyle infoOverlayStyle;

		public bool IsOverlayVisible { get; private set; } = false;


		public int UiTextStyleMode { get; set; } = 0;

		public bool UiControlShadowsEnabled { get; set; } = true;

		public void Initialize(MelonPreferences_Category prefs)
		{
			_prefs = prefs;


			ApplyInfoTexts();
		}

		public void InitializeStyles(
			Color themeColorPrimary,
			Color themeColorHover,
			Color overlayBackgroundColor,
			GUIStyle headerStyle,
			GUIStyle descriptionStyle)
		{
			infoIconBg = MakeCircleTexture(INFO_ICON_SIZE, themeColorPrimary);
			infoIconBgHover = MakeCircleTexture(INFO_ICON_SIZE, themeColorHover);
			Color overlayColor = overlayBackgroundColor;
			overlayColor.a = 1f;
			infoOverlaySolidBg = MakeRoundedTexture(128, 128, overlayColor, 8);

			Object.DontDestroyOnLoad(infoIconBg);
			Object.DontDestroyOnLoad(infoIconBgHover);
			Object.DontDestroyOnLoad(infoOverlaySolidBg);

			infoIconTextStyle = new GUIStyle(GUI.skin.label)
			{
				alignment = TextAnchor.MiddleCenter,
				fontSize = 9,
				fontStyle = FontStyle.Bold,
				clipping = TextClipping.Overflow,
				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0),
				richText = true
			};
			infoIconTextStyle.normal.textColor = Color.white;

			infoOverlayStyle = new GUIStyle(GUI.skin.box)
			{
				padding = new RectOffset(12, 12, 12, 12),
				border = new RectOffset(8, 8, 8, 8),
				richText = true
			};
			SetAllStates(infoOverlayStyle, infoOverlaySolidBg, Color.white);
			_accentHex = ColorToHex(themeColorPrimary);
			ApplyInfoTexts();
		}

		public void DrawInfoBox(Rect guiArea, float tabHeight, string infoKey, GUIStyle headerStyle, GUIStyle descriptionStyle)
		{
			_infoHoverIcon = false;
			IsOverlayVisible = false;

			float x = guiArea.width - INFO_ICON_SIZE - INFO_MARGIN_RIGHT;
			float y = guiArea.height - INFO_ICON_SIZE - INFO_MARGIN_BOTTOM;
			Rect iconRect = new Rect(x, y, INFO_ICON_SIZE, INFO_ICON_SIZE);

			bool hover = iconRect.Contains(Event.current.mousePosition);
			Texture2D bg = hover ? infoIconBgHover : infoIconBg;

			if (UiControlShadowsEnabled)
			{
				Rect shadowRect = iconRect;
				shadowRect.x += 2f;
				shadowRect.y += 3f;
				GUI.DrawTexture(shadowRect, bg, ScaleMode.StretchToFill, true, 0f, new Color(0f, 0f, 0f, 0.22f), 0, 0);
			}



			GUI.DrawTexture(iconRect, bg, ScaleMode.StretchToFill, true);

			Rect iconTextRect = new Rect(iconRect.x + INFO_ICON_TEXT_X_OFFSET, iconRect.y, iconRect.width, iconRect.height);
			DrawInfoIconText(iconTextRect);

			if (hover)
			{
				_infoHoverIcon = true;
				_infoActiveKey = infoKey;
			}

			bool showing = (_infoActiveKey != null && _infoHoverIcon);
			if (showing)
			{
				DrawInfoOverlay(guiArea, tabHeight, headerStyle, descriptionStyle);
			}

			IsOverlayVisible = showing;
		}

		private void DrawInfoOverlay(Rect guiArea, float tabHeight, GUIStyle headerStyle, GUIStyle descriptionStyle)
		{
			Rect overlayRect = GetOverlayRect(guiArea, tabHeight);
			Rect titleBarRect = GetTitleBarRect(overlayRect);
			DrawTitle(titleBarRect, Loc.GUI.InfoOverlayTitle, headerStyle);

			Rect contentRect = GetContentRect(overlayRect, titleBarRect);
			int fs;
			if (_infoActiveKey == SECTION_SPEEDRUN)
				fs = FONT_SPEEDRUN;
			else if (_infoActiveKey == SECTION_ANTICHEAT)
				fs = FONT_ANTICHEAT;
			else if (_infoActiveKey == SECTION_HUD)
				fs = FONT_HUD;
			else
				fs = FONT_DEFAULT;

			GUIStyle infoLabel = new GUIStyle(descriptionStyle)
			{
				alignment = TextAnchor.UpperLeft,
				wordWrap = true,
				richText = true,
				fontStyle = FontStyle.Normal,
				fontSize = fs
			};

			string txt = GetInfoText(_infoActiveKey);



			if (UiTextStyleMode == 1 && !ContainsRichText(txt))
			{
				Color prev = GUI.color;
				GUI.color = new Color(0f, 0f, 0f, 0.8f);
				GUI.Label(new Rect(contentRect.x + 1f, contentRect.y + 1f, contentRect.width, contentRect.height), txt, infoLabel);
				GUI.color = prev;
			}

			GUI.Label(contentRect, txt, infoLabel);
		}

		private void DrawShadowedCenteredText(Rect rect, string text, GUIStyle style, Color textColor, Color shadowColor, float shadowOffset = 1f)
		{
			var s = new GUIStyle(style) { alignment = TextAnchor.MiddleCenter };

			Color prev = GUI.color;

			GUI.color = shadowColor;
			GUI.Label(new Rect(rect.x + shadowOffset, rect.y + shadowOffset, rect.width, rect.height), text, s);

			GUI.color = prev;
			s.normal.textColor = textColor;
			GUI.Label(rect, text, s);
		}

		private void DrawInfoIconText(Rect rect)
		{

			if (UiTextStyleMode == 1)
			{
				DrawShadowedCenteredText(rect, "i", infoIconTextStyle, Color.white, new Color(0f, 0f, 0f, 0.8f), 1f);
				return;
			}

			DrawOutlinedCenteredText(rect, "i", infoIconTextStyle, Color.white, new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
		}

		private void DrawOutlinedCenteredText(Rect rect, string text, GUIStyle style, Color textColor, Color outlineColor, int thickness = 1)
		{
			var s = new GUIStyle(style) { alignment = TextAnchor.MiddleCenter };

			Color prev = GUI.color;
			GUI.color = outlineColor;
			for (int dx = -thickness; dx <= thickness; dx++)
			{
				for (int dy = -thickness; dy <= thickness; dy++)
				{
					if (dx == 0 && dy == 0) continue;
					GUI.Label(new Rect(rect.x + dx, rect.y + dy, rect.width, rect.height), text, s);
				}
			}
			GUI.color = prev;

			s.normal.textColor = textColor;
			GUI.Label(rect, text, s);
		}

		public string GetInfoText(string key)
		{
			switch (key)
			{
				case SECTION_OPTIMIZE: return infoOptimize;
				case SECTION_HSV: return infoHSV;
				case SECTION_SPEEDRUN: return infoSpeedrun;
				case SECTION_GAMEPLAY: return infoGameplay;
				case SECTION_FOV: return infoFOV;
				case SECTION_HUD: return infoHUD;
				case SECTION_SETTINGS: return infoSettings;
				case SECTION_ANTICHEAT: return infoAnticheat;
				case SECTION_MENU: return infoMenu;
				default: return infoOptimize;
			}
		}

		private Rect GetOverlayRect(Rect guiArea, float tabHeight)
		{
			float overlayTop = OVERLAY_TOP_EXTRA + tabHeight;
			float bottomSafe = GetBottomSafePadding(_infoActiveKey);
			return new Rect(
				OVERLAY_EDGE_MARGIN,
				overlayTop,
				guiArea.width - OVERLAY_EDGE_MARGIN * 2f,
				guiArea.height - overlayTop - bottomSafe
			);
		}

		private Rect GetTitleBarRect(Rect overlayRect) =>
			new Rect(overlayRect.x, overlayRect.y, overlayRect.width, TITLE_BAR_HEIGHT);

		private Rect GetContentRect(Rect overlayRect, Rect titleBarRect) =>
			new Rect(
				overlayRect.x + CONTENT_HPAD,
				titleBarRect.yMax + CONTENT_TOP_GAP,
				overlayRect.width - CONTENT_HPAD * 2f,
				overlayRect.height - (TITLE_BAR_HEIGHT + CONTENT_VPADDING)
			);

		private float GetBottomSafePadding(string key)
		{
			float baseSafe = INFO_MARGIN_BOTTOM + INFO_ICON_SIZE + BASE_BOTTOM_SAFE_EXTRA;
			if (key == SECTION_FOV) return Mathf.Max(0f, baseSafe - 20f);
			if (key == SECTION_SPEEDRUN) return Mathf.Max(0f, baseSafe - 25f);
			if (key == SECTION_SETTINGS) return Mathf.Max(0f, baseSafe - 25f);
			if (key == SECTION_HSV) return Mathf.Max(0f, baseSafe - 25f);
			if (key == SECTION_MENU) return Mathf.Max(0f, baseSafe - 25f);
			if (key == SECTION_ANTICHEAT) return Mathf.Max(0f, baseSafe - 25f);
			return baseSafe;
		}

		private void DrawTitle(Rect rect, string title, GUIStyle headerStyle)
		{
			GUIStyle titleStyle = new GUIStyle(headerStyle) { alignment = TextAnchor.MiddleCenter };

			Color prev = GUI.color;
			GUI.color = new Color(0f, 0f, 0f, 0.8f);
			GUI.Label(new Rect(rect.x + 1f, rect.y + 1f, rect.width, rect.height), title, titleStyle);
			GUI.color = prev;

			GUI.Label(rect, title, titleStyle);
		}

		private void ApplyInfoTexts()
		{
			string accent = _accentHex;
			string bullet = $"<color={accent}>•</color> ";

			string optimizeInfo = ToRichText(
				Loc.Info.Optimize
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			string hsvInfo = ToRichText(
				Loc.Info.HSV
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			string speedrunInfo = ToRichText(
				Loc.Info.Speedrun
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
					.Replace("{DANGER}", HEX_DANGER)
			);

			string fovInfo = ToRichText(
				Loc.Info.FOV
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
			);

			string gameplayInfo = ToRichText(
				Loc.Info.Gameplay
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			string settingsInfo = ToRichText(
				Loc.Info.Settings
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			string anticheatInfo = ToRichText(
				Loc.Info.Anticheat
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			string menuInfo = ToRichText(
				Loc.Info.Menu
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			string hudInfo = ToRichText(
				Loc.Info.HUD
					.Replace("{B}", bullet)
					.Replace("{ACCENT}", accent)
					.Replace("{WARN}", HEX_WARN)
			);

			infoOptimize = optimizeInfo;
			infoHSV = hsvInfo;
			infoSpeedrun = speedrunInfo;
			infoGameplay = gameplayInfo;
			infoFOV = fovInfo;
			infoHUD = hudInfo;
			infoSettings = settingsInfo;
			infoAnticheat = anticheatInfo;
			infoMenu = menuInfo;

			_prefs?.SaveToFile(false);
		}

		private string ToRichText(string markdown)
		{
			if (string.IsNullOrEmpty(markdown))
				return markdown;

			try
			{
				string cleaned = markdown.TrimStart('\r', '\n');
				return Regex.Replace(cleaned, @"\*\*(.+?)\*\*", "<b>$1</b>");
			}
			catch
			{
				return markdown.TrimStart('\r', '\n');
			}
		}

		private static bool ContainsRichText(string s)
		{
			if (string.IsNullOrEmpty(s)) return false;

			return s.IndexOf('<') >= 0 && s.IndexOf('>') >= 0;
		}

		private void SetAllStates(GUIStyle style, Texture2D background, Color textColor)
		{
			if (background != null)
			{
				style.normal.background = background;
				style.hover.background = background;
				style.active.background = background;
				style.focused.background = background;
				style.onNormal.background = background;
				style.onHover.background = background;
				style.onActive.background = background;
				style.onFocused.background = background;
			}

			style.normal.textColor = textColor;
			style.hover.textColor = textColor;
			style.active.textColor = textColor;
			style.focused.textColor = textColor;
			style.onNormal.textColor = textColor;
			style.onHover.textColor = textColor;
			style.onActive.textColor = textColor;
			style.onFocused.textColor = textColor;
		}

		private static string ColorToHex(Color c, bool includeAlpha = false)
		{
			int r = Mathf.RoundToInt(Mathf.Clamp01(c.r) * 255f);
			int g = Mathf.RoundToInt(Mathf.Clamp01(c.g) * 255f);
			int b = Mathf.RoundToInt(Mathf.Clamp01(c.b) * 255f);
			if (!includeAlpha) return $"#{r:X2}{g:X2}{b:X2}";
			int a = Mathf.RoundToInt(Mathf.Clamp01(c.a) * 255f);
			return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
		}

		private Texture2D MakeRoundedTexture(int width, int height, Color color, int cornerRadius)
		{
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			Color[] pixels = new Color[width * height];

			if (cornerRadius == 0)
			{
				for (int i = 0; i < pixels.Length; i++)
					pixels[i] = color;
			}
			else
			{
				float radius = cornerRadius;
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						bool inTopLeft = x < cornerRadius && y < cornerRadius;
						bool inTopRight = x >= width - cornerRadius && y < cornerRadius;
						bool inBottomLeft = x < cornerRadius && y >= height - cornerRadius;
						bool inBottomRight = x >= width - cornerRadius && y >= height - cornerRadius;

						if (inTopLeft || inTopRight || inBottomLeft || inBottomRight)
						{
							float radiusF = radius;
							float cornerCenterX, cornerCenterY;
							if (inTopLeft) { cornerCenterX = radiusF; cornerCenterY = radiusF; }
							else if (inTopRight) { cornerCenterX = width - radiusF; cornerCenterY = radiusF; }
							else if (inBottomLeft) { cornerCenterX = radiusF; cornerCenterY = height - radiusF; }
							else { cornerCenterX = width - radiusF; cornerCenterY = height - radiusF; }

							float dx = x + 0.5f - cornerCenterX;
							float dy = y + 0.5f - cornerCenterY;
							float distanceToCorner = Mathf.Sqrt(dx * dx + dy * dy);

							if (distanceToCorner > radiusF + 0.5f)
								pixels[y * width + x] = new Color(color.r, color.g, color.b, 0);
							else if (distanceToCorner > radiusF - 0.5f)
							{
								float alpha = Mathf.Clamp01(radiusF + 0.5f - distanceToCorner) * color.a;
								pixels[y * width + x] = new Color(color.r, color.g, color.b, alpha);
							}
							else
								pixels[y * width + x] = color;
						}
						else
						{
							pixels[y * width + x] = color;
						}
					}
				}
			}

			texture.SetPixels(pixels);
			texture.Apply();
			texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
			texture.filterMode = FilterMode.Bilinear;
			return texture;
		}

		private Texture2D MakeCircleTexture(int diameter, Color color)
		{
			Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
			Color[] pixels = new Color[diameter * diameter];
			float r = diameter / 2f;
			float cx = r - 0.5f;
			float cy = r - 0.5f;

			for (int y = 0; y < diameter; y++)
			{
				for (int x = 0; x < diameter; x++)
				{
					float dx = x - cx;
					float dy = y - cy;
					float dist = Mathf.Sqrt(dx * dx + dy * dy);

					if (dist <= r - 0.5f)
						pixels[y * diameter + x] = color;
					else if (dist < r + 0.5f)
					{
						float a = Mathf.Clamp01(r + 0.5f - dist) * color.a;
						pixels[y * diameter + x] = new Color(color.r, color.g, color.b, a);
					}
					else
						pixels[y * diameter + x] = new Color(0, 0, 0, 0);
				}
			}

			tex.SetPixels(pixels);
			tex.Apply();
			tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
			tex.filterMode = FilterMode.Bilinear;
			return tex;
		}
	}
}