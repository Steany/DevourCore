using MelonLoader;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevourCore
{
	public class Theme
	{


		private MelonPreferences_Category _prefs;
		private Info _infoManager;

		private MelonPreferences_Entry<float> _prefBgHue;
		private MelonPreferences_Entry<float> _prefBgOpacity;
		private MelonPreferences_Entry<bool> _prefUseDefaultBg;
		private MelonPreferences_Entry<bool> _prefHideBackground;
		private MelonPreferences_Entry<int> _prefUiTextStyleMode;
		private MelonPreferences_Entry<float> _prefThemeHue;
		private MelonPreferences_Entry<bool> _prefDarkMode;
		private MelonPreferences_Entry<bool> _prefChromaEnabled;

		private float _backgroundHue = 0.5f;
		private float _backgroundOpacity = 0.95f;
		private bool _useDefaultBg = false;
		private bool _hideBackground = false;

		private float _themeHue = 0.0f;
		private bool _darkMode = false;
		private int _uiTextStyleMode = 0;


		private bool _chromaEnabled = false;
		private float _chromaSpeed = 0.50f;
		private float _chromaOffset = 0f;
		private float _runtimeHue = 0f;
		private float _lastChromaUpdateTime = -999f;
		private const float CHROMA_UPDATE_INTERVAL = 0.05f;

		private bool _stylesInitialized = false;
		private bool _stylesDirty = true;
		private int _lastEnsureFrame = -1;

		private bool _savePending = false;
		private float _lastPrefChangeTime = 0f;
		private const float SAVE_DEBOUNCE_SECONDS = 0.25f;

		public float ThemeHue
		{
			get => _themeHue;
			set
			{
				value = Mathf.Clamp01(value);
				if (Mathf.Approximately(_themeHue, value)) return;

				_themeHue = value;
				if (_prefThemeHue != null) _prefThemeHue.Value = _themeHue;

				if (_chromaEnabled)
					SyncChromaOffsetToHue(_themeHue);

				UpdateThemeColors();
				InvalidateStyles();
				RequestSave();
			}
		}

		public bool DarkMode
		{
			get => _darkMode;
			set
			{
				if (_darkMode == value) return;

				_darkMode = value;
				if (_prefDarkMode != null) _prefDarkMode.Value = _darkMode;

				if (_darkMode && _chromaEnabled)
				{
					_chromaEnabled = false;
					if (_prefChromaEnabled != null) _prefChromaEnabled.Value = false;
					_runtimeHue = _themeHue;
				}

				UpdateThemeColors();
				InvalidateStyles();
				RequestSave();
			}
		}


		public int UiTextStyleMode
		{
			get => _uiTextStyleMode;
			set
			{
				if (value < 0) value = 0;
				if (value > 1) value = 1;
				if (_uiTextStyleMode == value) return;
				_uiTextStyleMode = value;
				if (_prefUiTextStyleMode != null) _prefUiTextStyleMode.Value = value;
				_stylesDirty = true;
			}
		}

		public bool ChromaEnabled
		{
			get => _chromaEnabled;
			set
			{
				if (_chromaEnabled == value) return;

				_chromaEnabled = value;
				if (_prefChromaEnabled != null) _prefChromaEnabled.Value = _chromaEnabled;

				if (_chromaEnabled && _darkMode)
				{
					_darkMode = false;
					if (_prefDarkMode != null) _prefDarkMode.Value = false;
				}

				if (_chromaEnabled)
				{
					SyncChromaOffsetToHue(_themeHue);
					_lastChromaUpdateTime = -999f;
				}
				else
				{
					_runtimeHue = _themeHue;
				}

				UpdateThemeColors();
				InvalidateStyles();
				RequestSave();
			}
		}
		public float ChromaSpeed => 0.50f;
		public Color ThemeColorPrimary { get; private set; }
		public Color ThemeColorSecondary { get; private set; }
		public Color ThemeColorDark { get; private set; }
		public Color ThemeColorHover { get; private set; }
		public Color ThemeTextColor { get; private set; }

		public float BackgroundHue
		{
			get => _backgroundHue;
			set
			{
				value = Mathf.Clamp01(value);
				if (Mathf.Approximately(_backgroundHue, value)) return;

				_backgroundHue = value;
				if (_prefBgHue != null) _prefBgHue.Value = _backgroundHue;

				InvalidateStyles();
				RequestSave();
			}
		}

		public float BackgroundOpacity
		{
			get => _backgroundOpacity;
			set
			{
				value = Mathf.Clamp01(value);
				if (Mathf.Approximately(_backgroundOpacity, value)) return;

				_backgroundOpacity = value;
				if (_prefBgOpacity != null) _prefBgOpacity.Value = _backgroundOpacity;

				InvalidateStyles();
				RequestSave();
			}
		}

		public bool UseDefaultBackground
		{
			get => _useDefaultBg;
			set
			{
				if (_useDefaultBg == value) return;

				_useDefaultBg = value;
				if (_prefUseDefaultBg != null) _prefUseDefaultBg.Value = _useDefaultBg;

				InvalidateStyles();
				RequestSave();
			}
		}

		public bool HideBackground
		{
			get => _hideBackground;
			set
			{
				if (_hideBackground == value) return;

				_hideBackground = value;
				if (_prefHideBackground != null) _prefHideBackground.Value = _hideBackground;

				InvalidateStyles();
				RequestSave();
			}
		}

		public void ResetBackground(float hue, float opacity, bool useDefault)
		{
			_backgroundHue = Mathf.Clamp01(hue);
			_backgroundOpacity = Mathf.Clamp01(opacity);
			_useDefaultBg = useDefault;
			_hideBackground = false;

			if (_prefBgHue != null) _prefBgHue.Value = _backgroundHue;
			if (_prefBgOpacity != null) _prefBgOpacity.Value = _backgroundOpacity;
			if (_prefUseDefaultBg != null) _prefUseDefaultBg.Value = _useDefaultBg;
			if (_prefHideBackground != null) _prefHideBackground.Value = false;

			InvalidateStyles();
			RequestSave();
		}

		public GUIStyle WindowStyle { get; private set; }
		public GUIStyle HeaderStyle { get; private set; }
		public GUIStyle DescriptionStyle { get; private set; }

		public GUIStyle TabActiveStyle { get; private set; }
		public GUIStyle TabInactiveStyle { get; private set; }
		public GUIStyle TabTitleStyle { get; private set; }

		public GUIStyle LabelStyle { get; private set; }
		public GUIStyle SliderLabelStyle { get; private set; }
		public GUIStyle ButtonStyle { get; private set; }
		public GUIStyle ToggleStyle { get; private set; }
		public GUIStyle CheckboxStyle { get; private set; }
		public GUIStyle CheckboxDisabledStyle { get; private set; }

		public Texture2D SliderBackground { get; private set; }
		public Texture2D SliderFillBackground { get; private set; }
		public Texture2D AccentFrameTexture { get; private set; }

		private Texture2D _blackBg;
		private Texture2D _purpleBg;
		private Texture2D _darkPurpleBg;
		private Texture2D _purpleHoverBg;
		private Texture2D _checkboxBg;
		private Texture2D _checkboxCheckedBg;
		private Texture2D _checkboxDisabledBg;
		private Texture2D _checkboxCheckedDisabledBg;
		private Texture2D _sliderBg;
		private Texture2D _sliderFillBg;

		public void Initialize(MelonPreferences_Category prefsCategory, Info infoManager)
		{
			_prefs = prefsCategory;
			_infoManager = infoManager;

			_prefBgHue = _prefs.CreateEntry("ThemeBackgroundHue", 0.5f);
			_prefBgOpacity = _prefs.CreateEntry("ThemeBackgroundOpacity", 0.95f);
			_prefUseDefaultBg = _prefs.CreateEntry("UseDefaultWindowBackground", true);
			_prefHideBackground = _prefs.CreateEntry("HideWindowBackground", false);

			_backgroundHue = Mathf.Clamp01(_prefBgHue.Value);
			_backgroundOpacity = Mathf.Clamp01(_prefBgOpacity.Value);
			_useDefaultBg = _prefUseDefaultBg.Value;
			_hideBackground = _prefHideBackground.Value;

			_prefThemeHue = _prefs.CreateEntry("ThemeHue", 0.7235294f);
			_prefUiTextStyleMode = _prefs.CreateEntry("UiTextStyleMode", 1);
			_prefDarkMode = _prefs.CreateEntry("ThemeDarkMode", false);

			_prefChromaEnabled = _prefs.CreateEntry("ThemeChromaEnabled", false);
			_themeHue = Mathf.Clamp01(_prefThemeHue.Value);
			_uiTextStyleMode = _prefUiTextStyleMode != null ? _prefUiTextStyleMode.Value : 0;
			_darkMode = _prefDarkMode.Value;

			_chromaEnabled = _prefChromaEnabled.Value;
			_chromaSpeed = 0.50f;
			_runtimeHue = _themeHue;
			if (_chromaEnabled)
				SyncChromaOffsetToHue(_themeHue);

			UpdateThemeColors();

			_stylesInitialized = false;
			_stylesDirty = true;
		}
		public void Tick()
		{
			TickChroma();
			SaveIfPending(false);
		}

		public void EnsureStyles()
		{
			if (Time.frameCount == _lastEnsureFrame) return;
			_lastEnsureFrame = Time.frameCount;

			if (!_stylesInitialized || _stylesDirty || WindowStyle == null)
				InitializeStyles();
		}

		public void InvalidateStyles()
		{
			_stylesInitialized = false;
			_stylesDirty = true;
		}

		public void SaveIfPending(bool force)
		{
			if (!_savePending || _prefs == null) return;

			if (!force)
			{
				float dt = Time.unscaledTime - _lastPrefChangeTime;
				if (dt < SAVE_DEBOUNCE_SECONDS) return;
			}

			_savePending = false;
			_prefs.SaveToFile(false);
		}

		public void Dispose()
		{
			DestroyOwnedTextures();
		}

		private void RequestSave()
		{
			_savePending = true;
			_lastPrefChangeTime = Time.unscaledTime;
		}
		private float GetActiveHue()
		{
			return _chromaEnabled ? _runtimeHue : _themeHue;
		}

		private static float Repeat01(float v)
		{
			v = v - Mathf.Floor(v);
			if (v < 0f) v += 1f;
			return v;
		}

		private void SyncChromaOffsetToHue(float desiredHue)
		{
			float t = Time.unscaledTime;
			_chromaOffset = Repeat01(desiredHue - (t * _chromaSpeed));
			_runtimeHue = desiredHue;
		}

		private void TickChroma()
		{
			if (!_chromaEnabled) return;
			if (_darkMode) return;

			float now = Time.unscaledTime;
			if (now - _lastChromaUpdateTime < CHROMA_UPDATE_INTERVAL) return;
			_lastChromaUpdateTime = now;

			float newHue = Repeat01((now * _chromaSpeed) + _chromaOffset);
			if (Mathf.Approximately(newHue, _runtimeHue)) return;

			_runtimeHue = newHue;

			UpdateThemeColors();
			InvalidateStyles();
		}

		private void UpdateThemeColors()
		{
			if (_darkMode)
			{
				ThemeColorDark = Color.black;
				ThemeColorPrimary = new Color(0.03f, 0.03f, 0.03f, 1f);
				ThemeColorSecondary = new Color(0.05f, 0.05f, 0.05f, 1f);
				ThemeColorHover = new Color(0.08f, 0.08f, 0.08f, 1f);
				ThemeTextColor = Color.white;
			}
			else
			{
				float h = GetActiveHue();
				const float sAccent = 0.60f;
				const float vAccent = 0.66f;
				const float sHover = 0.58f;
				const float vHover = 0.75f;
				const float sDark = 0.45f;
				const float vDark = 0.22f;

				ThemeColorPrimary = Color.HSVToRGB(h, sAccent, vAccent);
				ThemeColorHover = Color.HSVToRGB(h, sHover, vHover);
				ThemeColorDark = Color.HSVToRGB(h, sDark, vDark);
				ThemeColorSecondary = Color.HSVToRGB(h, sAccent * 0.95f, vAccent * 0.92f);
				ThemeTextColor = Color.white;
			}
		}

		private Color GetWindowBackgroundColor()
		{
			if (_hideBackground)
				return new Color(0f, 0f, 0f, 0f);

			if (_useDefaultBg)
			{
				Color c = new Color(0.05f, 0.05f, 0.05f, 1f);
				c.a = _backgroundOpacity;
				return c;
			}

			float t = Mathf.Clamp01(_backgroundHue);
			Color final = Color.HSVToRGB(t, 0.6f, 0.2f);
			final.a = _backgroundOpacity;
			return final;
		}

		private void InitializeStyles()
		{
			DestroyOwnedTextures();

			Color accentPrimary = ThemeColorPrimary;
			Color accentDark = ThemeColorDark;
			Color accentHover = ThemeColorHover;
			Color headerColor = _darkMode ? Color.white : ThemeColorPrimary;

			Color headerHoverColor = _darkMode
				? new Color(0.9f, 0.9f, 0.9f, 1f)
				: ThemeColorHover;

			Color winColor = GetWindowBackgroundColor();

			_blackBg = MakeRoundedTexture(128, 128, winColor, 8);
			_purpleBg = MakeRoundedTexture(128, 128, accentPrimary, 8);
			_darkPurpleBg = MakeRoundedTexture(128, 128, accentDark, 8);
			_purpleHoverBg = MakeRoundedTexture(128, 128, accentHover, 8);

			_checkboxBg = MakeRoundedTexture(64, 64, accentDark, 3);
			_checkboxCheckedBg = MakeRoundedTextureWithBorder(64, 64, accentPrimary, accentDark, 3, 2);

			Color disabledDark = new Color(accentDark.r * 0.65f, accentDark.g * 0.65f, accentDark.b * 0.65f, 1f);
			Color disabledPrimary = new Color(accentPrimary.r * 0.65f, accentPrimary.g * 0.65f, accentPrimary.b * 0.65f, 1f);

			_checkboxDisabledBg = MakeRoundedTexture(64, 64, disabledDark, 3);
			_checkboxCheckedDisabledBg = MakeRoundedTextureWithBorder(64, 64, disabledPrimary, disabledDark, 3, 2);

			_sliderBg = MakeRoundedTexture(128, 128, accentDark, 5);
			_sliderFillBg = MakeRoundedTexture(128, 128, accentPrimary, 5);

			SliderBackground = _sliderBg;
			SliderFillBackground = _sliderFillBg;
			AccentFrameTexture = _purpleBg;

			int borderSize = 8;

			WindowStyle = new GUIStyle(GUI.skin.window);
			SetAllStates(WindowStyle, _blackBg, Color.white);
			WindowStyle.fontSize = 16;
			WindowStyle.fontStyle = FontStyle.Bold;
			WindowStyle.alignment = TextAnchor.UpperCenter;
			WindowStyle.padding = new RectOffset(10, 10, 10, 10);
			WindowStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);

			HeaderStyle = new GUIStyle(GUI.skin.label);
			SetAllStates(HeaderStyle, null, headerColor);
			HeaderStyle.fontSize = 14;
			HeaderStyle.fontStyle = FontStyle.Bold;
			HeaderStyle.alignment = TextAnchor.MiddleCenter;
			HeaderStyle.clipping = TextClipping.Overflow;
			HeaderStyle.padding = new RectOffset(2, 2, 2, 2);

			DescriptionStyle = new GUIStyle(GUI.skin.label);
			SetAllStates(DescriptionStyle, null, new Color(0.85f, 0.85f, 0.85f, 1f));
			DescriptionStyle.fontSize = 11;
			DescriptionStyle.fontStyle = FontStyle.Italic;
			DescriptionStyle.alignment = TextAnchor.UpperCenter;
			DescriptionStyle.wordWrap = true;
			DescriptionStyle.richText = true;
			DescriptionStyle.clipping = TextClipping.Overflow;
			DescriptionStyle.padding = new RectOffset(3, 3, 2, 2);

			_infoManager?.InitializeStyles(accentPrimary, accentHover, winColor, HeaderStyle, DescriptionStyle);

			TabActiveStyle = new GUIStyle(GUI.skin.button);
			TabActiveStyle.normal.background = _purpleBg;
			TabActiveStyle.normal.textColor = Color.white;
			TabActiveStyle.hover.background = _purpleHoverBg;
			TabActiveStyle.hover.textColor = Color.white;
			TabActiveStyle.active.background = _purpleHoverBg;
			TabActiveStyle.active.textColor = Color.white;
			TabActiveStyle.focused.background = _purpleBg;
			TabActiveStyle.focused.textColor = Color.white;
			TabActiveStyle.onNormal.background = _purpleBg;
			TabActiveStyle.onNormal.textColor = Color.white;
			TabActiveStyle.onHover.background = _purpleHoverBg;
			TabActiveStyle.onHover.textColor = Color.white;
			TabActiveStyle.onActive.background = _purpleHoverBg;
			TabActiveStyle.onActive.textColor = Color.white;
			TabActiveStyle.onFocused.background = _purpleBg;
			TabActiveStyle.onFocused.textColor = Color.white;
			TabActiveStyle.fontSize = 13;
			TabActiveStyle.fontStyle = FontStyle.Bold;
			TabActiveStyle.alignment = TextAnchor.MiddleCenter;
			TabActiveStyle.padding = new RectOffset(12, 12, 3, 3);
			TabActiveStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);

			TabInactiveStyle = new GUIStyle(GUI.skin.button);
			TabInactiveStyle.normal.background = _darkPurpleBg;
			TabInactiveStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
			TabInactiveStyle.hover.background = _purpleBg;
			TabInactiveStyle.hover.textColor = Color.white;
			TabInactiveStyle.active.background = _purpleBg;
			TabInactiveStyle.active.textColor = Color.white;
			TabInactiveStyle.focused.background = _darkPurpleBg;
			TabInactiveStyle.focused.textColor = new Color(0.9f, 0.9f, 0.9f);
			TabInactiveStyle.onNormal.background = _darkPurpleBg;
			TabInactiveStyle.onNormal.textColor = new Color(0.9f, 0.9f, 0.9f);
			TabInactiveStyle.onHover.background = _purpleBg;
			TabInactiveStyle.onHover.textColor = Color.white;
			TabInactiveStyle.onActive.background = _purpleBg;
			TabInactiveStyle.onActive.textColor = Color.white;
			TabInactiveStyle.onFocused.background = _darkPurpleBg;
			TabInactiveStyle.onFocused.textColor = new Color(0.9f, 0.9f, 0.9f);
			TabInactiveStyle.fontSize = 13;
			TabInactiveStyle.fontStyle = FontStyle.Normal;
			TabInactiveStyle.alignment = TextAnchor.MiddleCenter;
			TabInactiveStyle.padding = new RectOffset(12, 12, 3, 3);
			TabInactiveStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);

			TabTitleStyle = new GUIStyle(GUI.skin.label);
			TabTitleStyle.alignment = TextAnchor.MiddleCenter;
			TabTitleStyle.fontSize = 13;
			TabTitleStyle.fontStyle = FontStyle.Bold;
			TabTitleStyle.normal.textColor = Color.white;
			TabTitleStyle.clipping = TextClipping.Overflow;
			TabTitleStyle.richText = true;
			TabTitleStyle.padding = new RectOffset(2, 2, 2, 2);

			LabelStyle = new GUIStyle(GUI.skin.label);
			SetAllStates(LabelStyle, null, Color.white);
			LabelStyle.fontSize = 12;
			LabelStyle.alignment = TextAnchor.MiddleLeft;
			LabelStyle.padding = new RectOffset(5, 5, 2, 2);
			LabelStyle.richText = true;
			LabelStyle.clipping = TextClipping.Overflow;

			SliderLabelStyle = new GUIStyle(GUI.skin.label);
			SetAllStates(SliderLabelStyle, null, Color.white);
			SliderLabelStyle.fontSize = 12;
			SliderLabelStyle.fontStyle = FontStyle.Bold;
			SliderLabelStyle.alignment = TextAnchor.MiddleCenter;
			SliderLabelStyle.richText = true;
			SliderLabelStyle.clipping = TextClipping.Overflow;
			SliderLabelStyle.padding = new RectOffset(2, 2, 2, 2);

			ButtonStyle = new GUIStyle(GUI.skin.button);
			ButtonStyle.normal.background = _darkPurpleBg;
			ButtonStyle.normal.textColor = Color.white;
			ButtonStyle.hover.background = _purpleBg;
			ButtonStyle.hover.textColor = Color.white;
			ButtonStyle.active.background = _purpleHoverBg;
			ButtonStyle.active.textColor = Color.white;
			ButtonStyle.focused.background = _darkPurpleBg;
			ButtonStyle.focused.textColor = Color.white;
			ButtonStyle.onNormal.background = _darkPurpleBg;
			ButtonStyle.onNormal.textColor = Color.white;
			ButtonStyle.onHover.background = _purpleBg;
			ButtonStyle.onHover.textColor = Color.white;
			ButtonStyle.onActive.background = _purpleHoverBg;
			ButtonStyle.onActive.textColor = Color.white;
			ButtonStyle.onFocused.background = _darkPurpleBg;
			ButtonStyle.onFocused.textColor = Color.white;
			ButtonStyle.fontSize = 12;
			ButtonStyle.fontStyle = FontStyle.Bold;
			ButtonStyle.alignment = TextAnchor.MiddleCenter;
			ButtonStyle.padding = new RectOffset(10, 10, 6, 6);
			ButtonStyle.border = new RectOffset(8, 8, 8, 8);

			CheckboxStyle = new GUIStyle();
			CheckboxStyle.normal.background = _checkboxBg;
			CheckboxStyle.hover.background = _checkboxBg;
			CheckboxStyle.active.background = _checkboxBg;
			CheckboxStyle.focused.background = _checkboxBg;
			CheckboxStyle.onNormal.background = _checkboxCheckedBg;
			CheckboxStyle.onHover.background = _checkboxCheckedBg;
			CheckboxStyle.onActive.background = _checkboxCheckedBg;
			CheckboxStyle.onFocused.background = _checkboxCheckedBg;
			CheckboxStyle.fixedWidth = 12;
			CheckboxStyle.fixedHeight = 12;
			CheckboxStyle.margin = new RectOffset(0, 8, 5, 5);
			CheckboxStyle.border = new RectOffset(3, 3, 3, 3);

			CheckboxDisabledStyle = new GUIStyle();
			CheckboxDisabledStyle.normal.background = _checkboxDisabledBg;
			CheckboxDisabledStyle.hover.background = _checkboxDisabledBg;
			CheckboxDisabledStyle.active.background = _checkboxDisabledBg;
			CheckboxDisabledStyle.focused.background = _checkboxDisabledBg;
			CheckboxDisabledStyle.onNormal.background = _checkboxCheckedDisabledBg;
			CheckboxDisabledStyle.onHover.background = _checkboxCheckedDisabledBg;
			CheckboxDisabledStyle.onActive.background = _checkboxCheckedDisabledBg;
			CheckboxDisabledStyle.onFocused.background = _checkboxCheckedDisabledBg;
			Color disabledText = new Color(0.5f, 0.5f, 0.5f, 1f);
			CheckboxDisabledStyle.normal.textColor = disabledText;
			CheckboxDisabledStyle.hover.textColor = disabledText;
			CheckboxDisabledStyle.active.textColor = disabledText;
			CheckboxDisabledStyle.focused.textColor = disabledText;
			CheckboxDisabledStyle.onNormal.textColor = disabledText;
			CheckboxDisabledStyle.onHover.textColor = disabledText;
			CheckboxDisabledStyle.onActive.textColor = disabledText;
			CheckboxDisabledStyle.onFocused.textColor = disabledText;
			CheckboxDisabledStyle.fixedWidth = 12;
			CheckboxDisabledStyle.fixedHeight = 12;
			CheckboxDisabledStyle.margin = new RectOffset(0, 8, 5, 5);
			CheckboxDisabledStyle.border = new RectOffset(3, 3, 3, 3);

			ToggleStyle = new GUIStyle(GUI.skin.label);
			ToggleStyle.normal.textColor = Color.white;
			ToggleStyle.hover.textColor = headerHoverColor;
			ToggleStyle.onHover.textColor = headerHoverColor;
			ToggleStyle.fontSize = 12;
			ToggleStyle.padding = new RectOffset(0, 0, 0, 0);
			ToggleStyle.alignment = TextAnchor.MiddleLeft;
			ToggleStyle.richText = true;
			ToggleStyle.clipping = TextClipping.Overflow;

			_stylesInitialized = true;
			_stylesDirty = false;
		}

		private void DestroyOwnedTextures()
		{
			DestroyTex(ref _blackBg);
			DestroyTex(ref _purpleBg);
			DestroyTex(ref _darkPurpleBg);
			DestroyTex(ref _purpleHoverBg);

			DestroyTex(ref _checkboxBg);
			DestroyTex(ref _checkboxCheckedBg);
			DestroyTex(ref _checkboxDisabledBg);
			DestroyTex(ref _checkboxCheckedDisabledBg);

			DestroyTex(ref _sliderBg);
			DestroyTex(ref _sliderFillBg);

			SliderBackground = null;
			SliderFillBackground = null;
			AccentFrameTexture = null;
		}

		private static void DestroyTex(ref Texture2D tex)
		{
			if (tex == null) return;
			Object.Destroy(tex);
			tex = null;
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

		private Texture2D MakeRoundedTexture(int width, int height, Color color, int cornerRadius)
		{
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			Color[] pixels = new Color[width * height];

			if (cornerRadius == 0)
			{
				for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
			}
			else
			{
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
							float radiusF = cornerRadius;
							float cx, cy;
							if (inTopLeft) { cx = radiusF; cy = radiusF; }
							else if (inTopRight) { cx = width - radiusF; cy = radiusF; }
							else if (inBottomLeft) { cx = radiusF; cy = height - radiusF; }
							else { cx = width - radiusF; cy = height - radiusF; }

							float dx = x + 0.5f - cx;
							float dy = y + 0.5f - cy;
							float dist = Mathf.Sqrt(dx * dx + dy * dy);

							if (dist > radiusF + 0.5f) pixels[y * width + x] = new Color(color.r, color.g, color.b, 0);
							else if (dist > radiusF - 0.5f)
							{
								float a = Mathf.Clamp01(radiusF + 0.5f - dist) * color.a;
								pixels[y * width + x] = new Color(color.r, color.g, color.b, a);
							}
							else pixels[y * width + x] = color;
						}
						else pixels[y * width + x] = color;
					}
				}
			}

			texture.SetPixels(pixels);
			texture.Apply();
			texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
			texture.filterMode = FilterMode.Bilinear;
			return texture;
		}

		private Texture2D MakeRoundedTextureWithBorder(int width, int height, Color innerColor, Color borderColor, int cornerRadius, int borderWidth)
		{
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			Color[] pixels = new Color[width * height];

			float outerRadius = cornerRadius;
			float innerCornerRadius = Mathf.Max(1, cornerRadius - borderWidth);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					bool inOuterTopLeft = x < outerRadius && y < outerRadius;
					bool inOuterTopRight = x >= width - outerRadius && y < outerRadius;
					bool inOuterBottomLeft = x < outerRadius && y >= height - outerRadius;
					bool inOuterBottomRight = x >= width - outerRadius && y >= height - outerRadius;

					if (inOuterTopLeft || inOuterTopRight || inOuterBottomLeft || inOuterBottomRight)
					{
						float ocx, ocy;
						if (inOuterTopLeft) { ocx = outerRadius; ocy = outerRadius; }
						else if (inOuterTopRight) { ocx = width - outerRadius; ocy = outerRadius; }
						else if (inOuterBottomLeft) { ocx = outerRadius; ocy = height - outerRadius; }
						else { ocx = width - outerRadius; ocy = height - outerRadius; }

						float dx = x + 0.5f - ocx;
						float dy = y + 0.5f - ocy;
						float distOuter = Mathf.Sqrt(dx * dx + dy * dy);

						if (distOuter > outerRadius + 0.5f) pixels[y * width + x] = new Color(borderColor.r, borderColor.g, borderColor.b, 0);
						else if (distOuter > outerRadius - 0.5f)
						{
							float a = Mathf.Clamp01(outerRadius + 0.5f - distOuter);
							pixels[y * width + x] = new Color(borderColor.r, borderColor.g, borderColor.b, a);
						}
						else
						{
							float distInner = Mathf.Sqrt(dx * dx + dy * dy);
							if (distInner <= innerCornerRadius - 0.5f) pixels[y * width + x] = innerColor;
							else if (distInner < innerCornerRadius + 0.5f)
							{
								float t2 = Mathf.Clamp01(distInner - (innerCornerRadius - 0.5f));
								pixels[y * width + x] = Color.Lerp(innerColor, borderColor, t2);
							}
							else pixels[y * width + x] = borderColor;
						}
					}
					else
					{
						bool inBorder = x < borderWidth || x >= width - borderWidth || y < borderWidth || y >= height - borderWidth;
						pixels[y * width + x] = inBorder ? borderColor : innerColor;
					}
				}
			}

			texture.SetPixels(pixels);
			texture.Apply();
			texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
			texture.filterMode = FilterMode.Bilinear;
			return texture;
		}


		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			if (prefsCategory == null) return;
			if (_prefBgHue != null) _prefBgHue.Value = 0.5f;
			if (_prefBgOpacity != null) _prefBgOpacity.Value = 0.95f;
			if (_prefUseDefaultBg != null) _prefUseDefaultBg.Value = true;
			if (_prefHideBackground != null) _prefHideBackground.Value = false;
			if (_prefThemeHue != null) _prefThemeHue.Value = 0.7235294f;
			if (_prefUiTextStyleMode != null) _prefUiTextStyleMode.Value = 1;
			if (_prefDarkMode != null) _prefDarkMode.Value = false;
			if (_prefChromaEnabled != null) _prefChromaEnabled.Value = false;

			BackgroundHue = _prefBgHue != null ? _prefBgHue.Value : BackgroundHue;
			BackgroundOpacity = _prefBgOpacity != null ? _prefBgOpacity.Value : BackgroundOpacity;

			UseDefaultBackground = _prefUseDefaultBg != null && _prefUseDefaultBg.Value;
			HideBackground = _prefHideBackground != null && _prefHideBackground.Value;
			ThemeHue = _prefThemeHue != null ? _prefThemeHue.Value : ThemeHue;
			UiTextStyleMode = _prefUiTextStyleMode != null ? _prefUiTextStyleMode.Value : UiTextStyleMode;
			DarkMode = _prefDarkMode != null && _prefDarkMode.Value;
			ChromaEnabled = _prefChromaEnabled != null && _prefChromaEnabled.Value;
			prefsCategory.SaveToFile(false);
		}

	}
}
