using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace DevourCore
{
	public class GUIManager
	{





		private bool _showGui = false;
		private Rect _guiArea = new Rect(10, 10, 390, 440);
		private int _selectedTab = 8;
		private bool centerGuiOnFirstOpen = false;
		private bool hasCenteredGuiOnce = false;

		private const float FULL_WIN_WIDTH = 530f;
		private const float REDUCED_WIN_WIDTH = 460f;
		private const float SMALL_WIN_WIDTH = 390f;

		private const float BASE_WIN_HEIGHT = 404f;
		private const float HSV_WIN_HEIGHT = 455f;
		private const float SPEEDRUN_WIN_HEIGHT = 465f;
		private const float GAMEPLAY_WIN_HEIGHT = 585f;
		private const float MENU_WIN_HEIGHT = 360f;
		private const float HUD_WIN_HEIGHT = 440f;
		private const float SETTINGS_WIN_HEIGHT = 583f;

		private const float TAB_HEIGHT = 24f;
		private const float STROKE_ALPHA = 0.6f;
		private const float MAP_BUTTON_HEIGHT = 24f;
		private const float MAP_BUTTON_SPACING = 8f;
		private const float SLIDER_FIXED_WIDTH = 340f;
		private const float TAB_VIS_BUTTON_WIDTH = 105f;


		private const string HEX_GREEN = "66FF66";
		private const string HEX_RED = "FF6666";

		private bool isEditingOutfit = false;
		private MelonPreferences_Entry<bool> prefIsEditingOutfit;

		private bool hasShownSpeedrunPopup = false;
		private bool isSpeedrunPopupActive = false;
		private GameObject persistentMessageModal = null;

		private Theme theme;


		private KeyCode _toggleGuiKey = KeyCode.RightShift;
		private bool _isCapturingMenuKey = false;
		private MelonPreferences_Entry<KeyCode> prefMenuKey;



		private bool _isCapturingCpsKey1 = false;
		private bool _isCapturingCpsKey2 = false;
		private MelonPreferences_Entry<float> prefGuiX, prefGuiY;
		private MelonPreferences_Entry<bool> prefInitialDefaultsApplied;
		private MelonPreferences_Entry<int> prefLastSelectedTab;
		private MelonPreferences_Entry<bool> prefUiClickSounds;
		private MelonPreferences_Entry<bool> prefUiControlShadows;


		private int lastSavedSelectedTab = 8;
		private bool isEditingTheme = false;

		private Optimize optimizeTab;
		private Icon hsvTab;
		private Outfit outfitTab;
		private Speedrun speedrunTab;
		private FOV fovTab;
		private Misc menuTab;
		private Anticheat anticheatTab;
		private Info infoManager;
		private MelonPreferences_Category prefs;
		private RunLookBackFeature runLookBack;
		private HudManager hudManager;
		private StunStatusIndicator stunStatus;

		private MelonPreferences_Entry<bool> prefShowOptimizeTab;
		private MelonPreferences_Entry<bool> prefShowHSVTab;
		private MelonPreferences_Entry<bool> prefShowSpeedrunTab;
		private MelonPreferences_Entry<bool> prefShowFOVTab;
		private MelonPreferences_Entry<bool> prefShowHudTab;
		private MelonPreferences_Entry<bool> prefShowMenuTab;

		private bool showOptimizeTab = true;
		private bool showHSVTab = true;
		private bool showSpeedrunTab = true;
		private bool showFOVTab = true;
		private bool showHudTab = true;
		private bool showMenuTab = true;

		private bool resetConfirmPending = false;
		private bool resetHudPosConfirmPending = false;

		private bool _cursorStateSaved = false;
		private bool _prevCursorVisible;
		private CursorLockMode _prevCursorLockState;
		private bool _hideTabContentCached = false;
		private bool pendingInitialDefaults = false;


		private bool _uiClickSoundsEnabled = true;
		private bool _uiControlShadowsEnabled = true;

		private AudioClip _uiClickClip;
		private AudioSource _uiClickSource;
		private UnityEngine.Audio.AudioMixerGroup _uiClickMixerGroup;

		private bool _inMenu;
		private bool _inValidMap;

		private UnityEngine.GUI.WindowFunction _windowFunc;
		private bool _delegateLookupAttempted = false;


		private enum HudSubMenu
		{
			NONE, FPS, CPS, COORDS, ENRAGE, SPEED, TIMER
		}

		private HudSubMenu currentHudSubMenu = HudSubMenu.NONE;



		private GUIStyle _tempStyle;

		private GUIStyle _tempStyle2;
		private static readonly KeyCode[] _allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
		private Dictionary<int, Misc.MapInfo> _mapInfoById;


		private Color _outlineColor;
		private Color _headerShadowColor;


		public bool ShowGui => _showGui;
		public KeyCode ToggleGuiKey => _toggleGuiKey;

		public void Initialize(
			MelonPreferences_Category prefsCategory,
			Optimize optTab,
			Icon hTab,
			Outfit oTab,
			Speedrun sTab,
			FOV fTab,
			Anticheat acTab,
			Misc menTab,
			Info info,
			RunLookBackFeature lookBackFeature,
			StunStatusIndicator stunStatusIndicator,
			HudManager hud)
		{
			prefs = prefsCategory;
			if (prefs == null) prefs = MelonPreferences.CreateCategory("DevourCore");

			Loc.Initialize(prefsCategory);

			optimizeTab = optTab;
			hsvTab = hTab;
			outfitTab = oTab;
			speedrunTab = sTab;
			fovTab = fTab;
			anticheatTab = acTab;
			menuTab = menTab;
			infoManager = info;
			runLookBack = lookBackFeature;
			stunStatus = stunStatusIndicator;
			hudManager = hud;

			InitializeTabVisibilityPrefs();

			prefMenuKey = prefs.CreateEntry("MenuToggleKey", KeyCode.RightShift);
			_toggleGuiKey = prefMenuKey.Value;

			prefGuiX = prefs.CreateEntry("GuiX", 1384f);
			prefGuiY = prefs.CreateEntry("GuiY", 157f);
			prefIsEditingOutfit = prefs.CreateEntry("IsEditingOutfit", false);

			prefLastSelectedTab = prefs.CreateEntry("LastSelectedTab", 8);
			_selectedTab = Mathf.Clamp(prefLastSelectedTab.Value, 0, 8);
			if (_selectedTab == 4) _selectedTab = 7;
			if (_selectedTab == 6) _selectedTab = 3;
			lastSavedSelectedTab = _selectedTab;

			prefInitialDefaultsApplied = prefs.CreateEntry("InitialGuiDefaultsApplied", true);


			prefUiClickSounds = prefs.CreateEntry("UiClickSounds", true);
			_uiClickSoundsEnabled = prefUiClickSounds.Value;

			prefUiControlShadows = prefs.CreateEntry("UiControlShadows", true);
			_uiControlShadowsEnabled = prefUiControlShadows.Value;
			if (infoManager != null) infoManager.UiControlShadowsEnabled = _uiControlShadowsEnabled;

			_guiArea.x = prefGuiX.Value;
			_guiArea.y = prefGuiY.Value;
			_guiArea.width = CalculateWindowWidth();
			isEditingOutfit = prefIsEditingOutfit.Value;
			pendingInitialDefaults = !prefInitialDefaultsApplied.Value;

			theme = new Theme();
			theme.Initialize(prefsCategory, infoManager);


			_tempStyle = new GUIStyle();


			_outlineColor = new Color(0f, 0f, 0f, STROKE_ALPHA);
			_headerShadowColor = new Color(0f, 0f, 0f, 0.8f);


			_mapInfoById = new Dictionary<int, Misc.MapInfo>();
			if (Misc.MapInfos != null)
			{
				foreach (var m in Misc.MapInfos)
					if (m != null) _mapInfoById[m.Id] = m;
			}
		}





		private UnityEngine.GUI.WindowFunction CreateWindowFunction()
		{

			Action<int> action = new Action<int>(OnWindowCallback);


			var result = TryConvertDelegate<UnityEngine.GUI.WindowFunction>(action, "Il2CppInterop.Runtime.DelegateSupport, Il2CppInterop.Runtime");
			if (result != null) return result;


			result = TryConvertDelegate<UnityEngine.GUI.WindowFunction>(action, "UnhollowerRuntimeLib.DelegateSupport, UnhollowerRuntimeLib");
			if (result != null) return result;



			return null;
		}

		private static T TryConvertDelegate<T>(Delegate d, string typeName) where T : class
		{
			try
			{
				var t = Type.GetType(typeName);
				if (t == null) return null;

				var mi = t.GetMethod("ConvertDelegate", BindingFlags.Public | BindingFlags.Static);
				if (mi == null) return null;

				var g = mi.MakeGenericMethod(typeof(T));

				return g.Invoke(null, new object[] { d }) as T;
			}
			catch
			{
				return null;
			}
		}

		private void OnWindowCallback(int id)
		{
			DrawMainWindow(id, _inMenu, _inValidMap);
		}

		private void InitializeTabVisibilityPrefs()
		{
			prefShowOptimizeTab = prefs.CreateEntry("ShowOptimizeTab", true);
			prefShowHSVTab = prefs.CreateEntry("ShowHSVTab", true);
			prefShowSpeedrunTab = prefs.CreateEntry("ShowSpeedrunTab", true);
			prefShowFOVTab = prefs.CreateEntry("ShowFOVTab", true);
			prefShowHudTab = prefs.CreateEntry("ShowHudTab", true);
			prefShowMenuTab = prefs.CreateEntry("ShowMenuTab", true);

			showOptimizeTab = prefShowOptimizeTab.Value;
			showHSVTab = prefShowHSVTab.Value;
			showSpeedrunTab = prefShowSpeedrunTab.Value;
			showFOVTab = prefShowFOVTab.Value;
			showHudTab = prefShowHudTab.Value;
			showMenuTab = prefShowMenuTab.Value;
		}

		public void SetCenterOnFirstOpen(bool center)
		{
			centerGuiOnFirstOpen = center;
		}

		private void ApplyCenterOnFirstOpenIfNeeded()
		{
			if (_showGui && !hasCenteredGuiOnce && centerGuiOnFirstOpen)
			{
				CenterGuiWindow();
				hasCenteredGuiOnce = true;
				centerGuiOnFirstOpen = false;
			}
		}

		private void OnGuiClosed()
		{
			hasShownSpeedrunPopup = false;
			resetConfirmPending = false;
			theme?.SaveIfPending(true);
			if (anticheatTab != null && anticheatTab.IsEditingPosition)
				anticheatTab.SetEditPositionMode(false, prefs);
		}

		private void SetGuiCursor(bool enable)
		{
			if (enable)
			{
				if (!_cursorStateSaved)
				{
					_prevCursorVisible = Cursor.visible;
					_prevCursorLockState = Cursor.lockState;
					_cursorStateSaved = true;
				}
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				if (_cursorStateSaved)
				{
					Cursor.lockState = _prevCursorLockState;
					Cursor.visible = _prevCursorVisible;
					_cursorStateSaved = false;
				}
			}
		}

		private void EnforceGuiCursor()
		{
			if (!_showGui) return;
			if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
			if (!Cursor.visible) Cursor.visible = true;
		}

		private void SetGuiVisible(bool visible)
		{
			bool wasShowing = _showGui;
			_showGui = visible;
			SetGuiCursor(_showGui);
			ApplyCenterOnFirstOpenIfNeeded();
			if (wasShowing && !_showGui) OnGuiClosed();
		}

		public void ToggleGui()
		{
			SetGuiVisible(!_showGui);
		}

		public void Update()
		{
			theme?.Tick();


			if (_isCapturingMenuKey)
			{
				for (int i = 0; i < _allKeyCodes.Length; i++)
				{
					KeyCode key = _allKeyCodes[i];
					if (!Input.GetKeyDown(key) || key == KeyCode.None) continue;


					if (key == KeyCode.Escape)
					{
						_isCapturingMenuKey = false;
						break;
					}

					_toggleGuiKey = key;
					if (prefMenuKey != null) prefMenuKey.Value = key;
					if (prefs != null) prefs.SaveToFile(false);
					_isCapturingMenuKey = false;
					break;
				}
			}


			if (_isCapturingCpsKey1 || _isCapturingCpsKey2)
			{
				for (int k = 0; k < _allKeyCodes.Length; k++)
				{
					KeyCode kc = _allKeyCodes[k];
					if (!Input.GetKeyDown(kc) || kc == KeyCode.None) continue;


					if (kc == KeyCode.Escape)
					{
						_isCapturingCpsKey1 = false;
						_isCapturingCpsKey2 = false;
						break;
					}

					var hm = hudManager;
					if (hm != null)
					{
						if (_isCapturingCpsKey1) hm.CpsKey1 = kc;
						if (_isCapturingCpsKey2) hm.CpsKey2 = kc;
					}

					_isCapturingCpsKey1 = false;
					_isCapturingCpsKey2 = false;
					break;
				}
			}

			if (pendingInitialDefaults)
			{
				ApplyInitialGuiDefaults();
				if (AreInitialDefaultsSatisfied())
				{
					pendingInitialDefaults = false;
					if (prefInitialDefaultsApplied != null) prefInitialDefaultsApplied.Value = true;
					if (prefs != null) prefs.SaveToFile(false);
				}
			}
		}

		private void StartCapturingMenuKey()
		{
			_isCapturingMenuKey = true;
		}

		private void ApplyInitialGuiDefaults()
		{
			const float INITIAL_HSV = 1f;
			const int INITIAL_RANK = 17;
			if (optimizeTab != null)
			{
				optimizeTab.SetWeatherDisabled(false, prefs, false, false);
				optimizeTab.SetMuteWeatherAudio(false, prefs, false, false);
			}
			if (hsvTab != null)
			{
				hsvTab.SetHSV(INITIAL_HSV, INITIAL_HSV, INITIAL_HSV, prefs);
				SetIconRankToTarget(INITIAL_RANK);
			}
			if (outfitTab != null) outfitTab.SetHSV(INITIAL_HSV, INITIAL_HSV, INITIAL_HSV, prefs);
		}

		private bool AreInitialDefaultsSatisfied()
		{
			const float TARGET = 1f;
			bool iconOk = true;
			if (hsvTab != null) iconOk = Mathf.Approximately(hsvTab.Hue, TARGET) && Mathf.Approximately(hsvTab.Sat, TARGET) && Mathf.Approximately(hsvTab.Val, TARGET);
			bool outfitOk = true;
			if (outfitTab != null) outfitOk = Mathf.Approximately(outfitTab.OutfitHue, TARGET) && Mathf.Approximately(outfitTab.OutfitSat, TARGET) && Mathf.Approximately(outfitTab.OutfitVal, TARGET);
			bool optOk = true;
			if (optimizeTab != null) optOk = !optimizeTab.DisableWeather && !optimizeTab.MuteWeatherAudio;
			return iconOk && outfitOk && optOk;
		}

		private void SetIconRankToTarget(int targetLevel)
		{
			if (hsvTab == null) return;
			int safety = 0;
			while (hsvTab.CurrentRankLevel != targetLevel && safety < 64)
			{
				hsvTab.CycleRank(prefs);
				safety++;
			}
		}

		private int GetVisibleTabCount()
		{
			int count = 1;
			if (showOptimizeTab) count++;
			if (showHSVTab) count++;
			if (showSpeedrunTab) count++;
			if (showFOVTab) count++;
			if (showHudTab) count++;
			if (showMenuTab) count++;
			return count;
		}

		private float CalculateWindowWidth()
		{
			int visibleTabs = GetVisibleTabCount();
			if (visibleTabs >= 7) return FULL_WIN_WIDTH;
			if (visibleTabs >= 6) return REDUCED_WIN_WIDTH;
			return SMALL_WIN_WIDTH;
		}

		public void CenterGuiWindow()
		{
			float width = CalculateWindowWidth();
			float height = _guiArea.height;
			float x = Mathf.Round((Screen.width - width) * 0.5f);
			float y = Mathf.Round((Screen.height - height) * 0.5f - 40f);
			_guiArea.x = Mathf.Clamp(x, 10f, Mathf.Max(10f, Screen.width - width - 10f));
			_guiArea.y = Mathf.Clamp(y, 10f, Mathf.Max(10f, Screen.height - height - 10f));
			_guiArea.width = width;
			prefGuiX.Value = _guiArea.x;
			prefGuiY.Value = _guiArea.y;
			prefs.SaveToFile(false);
		}

		public void OnGUI(bool inMenu, bool inValidMap)
		{
			if (!_showGui) return;
			_inMenu = inMenu;
			_inValidMap = inValidMap;
			EnforceGuiCursor();
			if (theme == null) return;
			theme.EnsureStyles();

			float targetH = BASE_WIN_HEIGHT;
			if (_selectedTab == 1) targetH = HSV_WIN_HEIGHT;
			else if (_selectedTab == 2) targetH = SPEEDRUN_WIN_HEIGHT;
			else if (_selectedTab == 3) targetH = GAMEPLAY_WIN_HEIGHT;
			else if (_selectedTab == 5) targetH = MENU_WIN_HEIGHT;
			else if (_selectedTab == 7) targetH = HUD_WIN_HEIGHT;
			else if (_selectedTab == 8) targetH = SETTINGS_WIN_HEIGHT;

			float targetW = CalculateWindowWidth();
			float previousHeight = _guiArea.height;
			float previousWidth = _guiArea.width;

			_guiArea.height = targetH;
			_guiArea.width = targetW;

			if (!Mathf.Approximately(previousHeight, targetH))
				_guiArea.y = Mathf.Clamp(_guiArea.y, 10f, Mathf.Max(10f, Screen.height - _guiArea.height - 10f));
			if (!Mathf.Approximately(previousWidth, targetW))
				_guiArea.x = Mathf.Clamp(_guiArea.x, 10f, Mathf.Max(10f, Screen.width - _guiArea.width - 10f));

			if (_windowFunc == null && !_delegateLookupAttempted)
			{
				_delegateLookupAttempted = true;
				_windowFunc = CreateWindowFunction();
			}

			if (_windowFunc != null)
				_guiArea = UnityEngine.GUI.Window(12345, _guiArea, _windowFunc, "", theme.WindowStyle);

			_guiArea.x = Mathf.Clamp(_guiArea.x, 0f, Mathf.Max(0f, Screen.width - _guiArea.width));
			_guiArea.y = Mathf.Clamp(_guiArea.y, 0f, Mathf.Max(0f, Screen.height - _guiArea.height));
		}

		private void DrawMainWindow(int windowID, bool inMenu, bool inValidMap)
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			if (showOptimizeTab && DrawTabButton(Loc.GUI.Tab_Optimize, 0)) { _selectedTab = 0; resetConfirmPending = false; }
			if (showHSVTab && DrawTabButton(Loc.GUI.Tab_HSV, 1)) { _selectedTab = 1; resetConfirmPending = false; }
			if (showSpeedrunTab && DrawTabButton(Loc.GUI.Tab_Speedrun, 2)) { _selectedTab = 2; resetConfirmPending = false; }
			if (showFOVTab && DrawTabButton(Loc.GUI.Tab_Gameplay, 3)) { _selectedTab = 3; resetConfirmPending = false; }
			if (showMenuTab && DrawTabButton(Loc.GUI.Tab_Menu, 5)) { _selectedTab = 5; resetConfirmPending = false; }
			if (showHudTab && DrawTabButton(Loc.GUI.Tab_HUD, 7)) { _selectedTab = 7; resetConfirmPending = false; }
			if (DrawTabButton(Loc.GUI.Tab_Settings, 8)) { _selectedTab = 8; resetConfirmPending = false; }
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			if (Event.current.type == EventType.Layout)
				_hideTabContentCached = infoManager != null && infoManager.IsOverlayVisible;

			if (!_hideTabContentCached)
			{
				switch (_selectedTab)
				{
					case 0: DrawOptimizationTab(inMenu, inValidMap); break;
					case 1: DrawHSVTab(); break;
					case 2: DrawSpeedrunTab(inMenu); break;
					case 3: DrawGameplayTab(inMenu, inValidMap); break;
					case 5: DrawMenuTab(); break;
					case 7: DrawHudTab(); break;
					case 8: DrawSettingsTab(inMenu, inValidMap); break;
				}
			}

			infoManager.UiTextStyleMode = theme.UiTextStyleMode;
			infoManager.DrawInfoBox(_guiArea, TAB_HEIGHT, GetInfoKeyForSelectedTab(), theme.HeaderStyle, theme.DescriptionStyle);

			bool posChanged = false;
			if (!Mathf.Approximately(prefGuiX.Value, _guiArea.x)) { prefGuiX.Value = _guiArea.x; posChanged = true; }
			if (!Mathf.Approximately(prefGuiY.Value, _guiArea.y)) { prefGuiY.Value = _guiArea.y; posChanged = true; }

			SaveLastSelectedTabIfNeeded();

			if (posChanged && Input.GetMouseButtonUp(0))
				prefs.SaveToFile(false);

			UnityEngine.GUI.DragWindow();
		}

		private void SaveLastSelectedTabIfNeeded()
		{
			if (prefLastSelectedTab == null) return;
			if (_selectedTab != lastSavedSelectedTab)
			{
				if (anticheatTab != null && anticheatTab.IsEditingPosition && lastSavedSelectedTab == 7)
				{
					anticheatTab.SetEditPositionMode(false, prefs);
				}
				lastSavedSelectedTab = _selectedTab;
				prefLastSelectedTab.Value = _selectedTab;
				prefs.SaveToFile(false);
			}
		}

		private string GetInfoKeyForSelectedTab()
		{
			switch (_selectedTab)
			{
				case 0: return "Optimize";
				case 1: return "HSV";
				case 2: return "Speedrun";
				case 3: return "Gameplay";
				case 4: return "FOV";
				case 5: return "Menu";
				case 7: return "HUD";
				case 8: return "Settings";
			}
			return "Optimize";
		}

		private bool DrawTabButton(string title, int index)
		{
			bool active = (_selectedTab == index);
			bool clicked = ClickButton(GUIContent.none, active ? theme.TabActiveStyle : theme.TabInactiveStyle, GUILayout.Height(TAB_HEIGHT));
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, title, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			return clicked;
		}

		private void DrawOptimizationTab(bool inMenu, bool inValidMap)
		{
			DrawHeader(Loc.GUI.Header_Cull);
			GUILayout.Space(8);
			bool newCullEnabled = DrawCustomToggle(optimizeTab.CullEnabled, Loc.GUI.Toggle_CullEnabled);
			if (newCullEnabled != optimizeTab.CullEnabled) optimizeTab.SetCullEnabled(newCullEnabled, prefs);
			GUILayout.Space(5);
			bool newApplyInMenu = DrawCustomToggle(optimizeTab.ApplyInMenu, Loc.GUI.Toggle_CullInMenu);
			if (newApplyInMenu != optimizeTab.ApplyInMenu) optimizeTab.SetApplyInMenu(newApplyInMenu, prefs);
			GUILayout.Space(10);
			DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.CullDistanceFormat, Mathf.RoundToInt(optimizeTab.CullRadius)));
			float newCull = DrawCustomSlider(optimizeTab.CullRadius, 6f, 30f);
			if (!Mathf.Approximately(newCull, optimizeTab.CullRadius)) optimizeTab.SetCullRadius(newCull, prefs);
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string cullKeyText = optimizeTab.IsCapturingCullKey ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.ToggleKeyFormat, optimizeTab.CullToggleKey);
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200))) optimizeTab.StartCapturingKey();
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, cullKeyText, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(20);
			DrawHeader(Loc.GUI.Header_Weather);
			GUILayout.Space(8);
			UnityEngine.GUI.enabled = inMenu;
			bool newWeather = DrawCustomToggle(optimizeTab.DisableWeather, Loc.GUI.Toggle_DisableWeather);
			if (newWeather != optimizeTab.DisableWeather && inMenu) optimizeTab.SetWeatherDisabled(newWeather, prefs, inMenu, inValidMap);
			UnityEngine.GUI.enabled = true;
		}

		private void DrawHSVTab()
		{
			DrawHeader(isEditingOutfit ? Loc.GUI.Header_OutfitColor : Loc.GUI.Header_IconColor);
			GUILayout.Space(8);
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.BeginVertical();
			bool newIconEnabled = DrawCustomToggle(hsvTab.HsvModEnabled, Loc.GUI.Toggle_IconEnabled);
			if (newIconEnabled != hsvTab.HsvModEnabled) hsvTab.SetEnabled(newIconEnabled, prefs);
			GUILayout.Space(5);
			bool newOutfitEnabled = DrawCustomToggle(outfitTab.OutfitModEnabled, Loc.GUI.Toggle_OutfitEnabled);
			if (newOutfitEnabled != outfitTab.OutfitModEnabled) outfitTab.SetEnabled(newOutfitEnabled, prefs);
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(30);
			float currentHue = isEditingOutfit ? outfitTab.OutfitHue : hsvTab.Hue;
			float currentSat = isEditingOutfit ? outfitTab.OutfitSat : hsvTab.Sat;
			float currentVal = isEditingOutfit ? outfitTab.OutfitVal : hsvTab.Val;
			Texture2D previewTex = isEditingOutfit ? outfitTab.OutfitColorTexture : hsvTab.ColorTexture;
			GUILayout.BeginHorizontal();
			float windowWidth = _guiArea.width;
			float leftSpacing = windowWidth >= REDUCED_WIN_WIDTH ? 60f : 35f;
			float betweenSliders = windowWidth >= REDUCED_WIN_WIDTH ? 54f : 40f;
			GUILayout.Space(leftSpacing);
			float newHue = DrawCustomVerticalSlider(currentHue, 0f, 1f, Loc.GUI.Slider_H);
			GUILayout.Space(betweenSliders);
			float newSat = DrawCustomVerticalSlider(currentSat, 0f, 1f, Loc.GUI.Slider_S);
			GUILayout.Space(betweenSliders);
			float newVal = DrawCustomVerticalSlider(currentVal, 0f, 1f, Loc.GUI.Slider_V);
			GUILayout.Space(30);
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			const float miniBtnW = 65f;
			const float miniBtnH = 22f;
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(GUILayout.Width(miniBtnW));
			Rect topRect = GUILayoutUtility.GetRect(miniBtnW, miniBtnH, GUILayout.Width(miniBtnW), GUILayout.Height(miniBtnH));
			if (!isEditingOutfit)
			{
				int level = hsvTab.CurrentRankLevel;
				if (ClickButtonRect(topRect, GUIContent.none, theme.ButtonStyle))
					hsvTab.CycleRank(prefs);
				if (Event.current.type == EventType.Repaint)
				{
					Rect rankInset = new Rect(topRect.x + 1f, topRect.y + 1f, topRect.width - 2f, topRect.height - 2f);
					string labelText = level.ToString();
					DrawButtonTextWithColoredSymbol(rankInset, "✓", labelText, theme.TabTitleStyle, HEX_GREEN);
				}
			}
			GUILayout.Space(8f);
			string swapText = isEditingOutfit ? Loc.GUI.Swap_ToIcon : Loc.GUI.Swap_ToOutfit;
			Rect swapRect = GUILayoutUtility.GetRect(miniBtnW, miniBtnH, GUILayout.Width(miniBtnW), GUILayout.Height(miniBtnH));
			if (ClickButtonRect(swapRect, GUIContent.none, theme.ButtonStyle))
			{
				isEditingOutfit = !isEditingOutfit;
				prefIsEditingOutfit.Value = isEditingOutfit;
				prefs.SaveToFile(false);
			}
			Rect swapInset = new Rect(swapRect.x + 1f, swapRect.y + 1f, swapRect.width - 2f, swapRect.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(swapInset, swapText, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Space(3);
			DrawHeader(Loc.GUI.Header_Preview);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (previewTex != null)
			{
				Rect previewRect = GUILayoutUtility.GetRect(60, 60, GUILayout.Width(60), GUILayout.Height(60));
				if (Event.current.type == EventType.Repaint)
				{
					UnityEngine.GUI.DrawTexture(new Rect(previewRect.x - 2, previewRect.y - 2, previewRect.width + 4, previewRect.height + 4), theme.AccentFrameTexture);
					UnityEngine.GUI.DrawTexture(previewRect, previewTex, ScaleMode.StretchToFill);
				}
			}
			else
				GUILayout.Box("", GUILayout.Width(60), GUILayout.Height(60));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			if (isEditingOutfit)
			{
				if (newHue != currentHue || newSat != currentSat || newVal != currentVal)
					outfitTab.SetHSV(newHue, newSat, newVal, prefs);
			}
			else
			{
				if (newHue != currentHue || newSat != currentSat || newVal != currentVal)
					hsvTab.SetHSV(newHue, newSat, newVal, prefs);
			}
			GUILayout.Space(15);
		}

		private void DrawSpeedrunTab(bool inMenu)
		{
			DrawHeader(Loc.GUI.Header_Speedrun);
			GUILayout.Space(8);
			bool newLong = DrawCustomToggle(speedrunTab.DisableLongInteract, Loc.GUI.Toggle_InstantInteract);
			if (newLong != speedrunTab.DisableLongInteract)
			{
				if (newLong && !speedrunTab.DisableLongInteract) TriggerSpeedrunPopup();
				speedrunTab.SetDisableLongInteract(newLong, prefs);
			}
			GUILayout.Space(5);
			bool newAttic = DrawCustomToggle(speedrunTab.EnableAtticSpawn, Loc.GUI.Toggle_AtticSpawn);
			if (newAttic != speedrunTab.EnableAtticSpawn)
			{
				if (newAttic && !speedrunTab.EnableAtticSpawn) TriggerSpeedrunPopup();
				speedrunTab.SetEnableAtticSpawn(newAttic, prefs);
			}
			GUILayout.Space(15);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string keyText = speedrunTab.IsCapturingInteractKey ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.InteractKeyFormat, speedrunTab.CustomInteractKey);
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200))) speedrunTab.StartCapturingInteractKey();
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, keyText, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(16);
			DrawHeader(Loc.GUI.Header_AutoStart);
			GUILayout.Space(6);
			bool forceStartOn = speedrunTab.ForceStartEnabled;
			bool canToggleHere = inMenu || (!inMenu && forceStartOn);
			UnityEngine.GUI.enabled = canToggleHere;
			bool newForceStart = DrawCustomToggle(forceStartOn, Loc.GUI.Toggle_ForceStart);
			UnityEngine.GUI.enabled = true;
			if (newForceStart != forceStartOn)
			{
				if (inMenu || !newForceStart)
				{
					if (newForceStart && !forceStartOn) TriggerSpeedrunPopup();
					speedrunTab.SetForceStartEnabled(newForceStart, prefs, inMenu || !newForceStart);
				}
			}
			GUILayout.Space(5);
			UnityEngine.GUI.enabled = inMenu;
			bool newUseArm = DrawCustomToggle(speedrunTab.UseArmingWindow, Loc.GUI.Toggle_UseArm);
			if (newUseArm != speedrunTab.UseArmingWindow && inMenu) speedrunTab.SetUseArmingWindow(newUseArm, prefs);
			UnityEngine.GUI.enabled = true;
			GUILayout.Space(10);
			DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.ForceStartDelayFormat, speedrunTab.ForceStartDelay));
			float newDelay = DrawCustomSlider(speedrunTab.ForceStartDelay, 0.7f, 1.5f);
			if (!Mathf.Approximately(newDelay, speedrunTab.ForceStartDelay)) speedrunTab.SetForceStartDelay(newDelay);
			GUILayout.Space(6);
			DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.ForceStartArmFormat, speedrunTab.ForceStartArmMinutes));
			float newArm = DrawCustomSlider(speedrunTab.ForceStartArmMinutes, 2f, 10f);
			newArm = Mathf.Round(newArm * 10f) / 10f;
			if (!Mathf.Approximately(newArm, speedrunTab.ForceStartArmMinutes)) speedrunTab.SetForceStartArmMinutes(newArm);
		}

		private void TriggerSpeedrunPopup()
		{
			if (!hasShownSpeedrunPopup && !isSpeedrunPopupActive)
				MelonCoroutines.Start(ShowSpeedrunWarningPopup());
		}

		private IEnumerator ShowSpeedrunWarningPopup()
		{
			hasShownSpeedrunPopup = true;
			isSpeedrunPopupActive = true;
			yield return new WaitForSeconds(0.5f);
			GameObject messageModal = null;
			if (persistentMessageModal != null) messageModal = persistentMessageModal;
			else
			{
				messageModal = GameObject.Find("Message Modal");
				if (messageModal != null)
				{
					UnityEngine.Object.DontDestroyOnLoad(messageModal);
					persistentMessageModal = messageModal;
				}
			}
			if (messageModal != null)
			{
				Text[] allTexts = messageModal.GetComponentsInChildren<Text>(true);
				if (allTexts.Length >= 2)
				{
					allTexts[0].text = Loc.GUI.SpeedrunPopupBody;
					allTexts[1].text = Loc.GUI.SpeedrunPopupConfirm;
					messageModal.SetActive(true);
					CanvasGroup canvasGroup = messageModal.GetComponent<CanvasGroup>();
					if (canvasGroup != null)
					{
						canvasGroup.alpha = 0.95f;
						canvasGroup.interactable = true;
						canvasGroup.blocksRaycasts = true;
					}
				}
			}
			isSpeedrunPopupActive = false;
		}

		private void DrawGameplayTab(bool inMenu, bool inValidMap)
		{
			DrawHeader(Loc.GUI.Header_FOV);
			GUILayout.Space(8);
			bool newFovEnabled = DrawCustomToggle(fovTab.FovModEnabled, Loc.GUI.Toggle_FOVEnabled);
			if (newFovEnabled != fovTab.FovModEnabled) fovTab.SetFovEnabled(newFovEnabled, prefs);
			GUILayout.Space(10);
			DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.FOVValueFormat, Mathf.RoundToInt(fovTab.TargetFOV)));
			float newFov = DrawCustomSlider(fovTab.TargetFOV, 50f, 110f);
			if (!Mathf.Approximately(newFov, fovTab.TargetFOV)) fovTab.SetTargetFOV(newFov, prefs, inValidMap);
			GUILayout.Space(25);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string fovKeyText = fovTab.IsCapturingFovKey ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.ToggleKeyFormat, fovTab.FovToggleKey);
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200))) fovTab.StartCapturingKey();
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, fovKeyText, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(18);
			DrawHeader(Loc.GUI.Header_LookBack);
			GUILayout.Space(8);
			if (runLookBack != null)
			{
				bool lbEnabled = runLookBack.Enabled;
				bool newLbEnabled = DrawCustomToggle(lbEnabled, Loc.GUI.Toggle_EnableLookBack);
				if (newLbEnabled != lbEnabled) runLookBack.SetEnabled(newLbEnabled);
				GUILayout.Space(5);
				bool toggleMode = runLookBack.ToggleMode;
				bool newToggleMode = DrawCustomToggle(toggleMode, Loc.GUI.Toggle_ToggleMode);
				if (newToggleMode != toggleMode) runLookBack.SetToggleMode(newToggleMode);
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				string lookBackKeyText = runLookBack.IsCapturingKey ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.LookBackKeyFormat, runLookBack.LookBackKey);
				if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200))) runLookBack.StartCapturingKey();
				Rect rLb = GUILayoutUtility.GetLastRect();
				Rect rLbInset = new Rect(rLb.x + 1f, rLb.y + 1f, rLb.width - 2f, rLb.height - 2f);
				if (Event.current.type == EventType.Repaint)
					DrawOutlinedCenteredText(rLbInset, lookBackKeyText, theme.TabTitleStyle, Color.white, _outlineColor, 1);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			else
				GUILayout.Label(Loc.GUI.LookBackNotAvailable, theme.LabelStyle);
			GUILayout.Space(18);
			DrawHeader(Loc.GUI.Header_Audio);
			GUILayout.Space(8);
			UnityEngine.GUI.enabled = inMenu;
			bool newMuteWeather = DrawCustomToggle(optimizeTab.MuteWeatherAudio, Loc.GUI.Toggle_MuteWeather);
			if (newMuteWeather != optimizeTab.MuteWeatherAudio && inMenu) optimizeTab.SetMuteWeatherAudio(newMuteWeather, prefs, inMenu, inValidMap);
			UnityEngine.GUI.enabled = true;
			GUILayout.Space(5);
			bool inMenuScene = menuTab.IsMenuSceneActive();
			UnityEngine.GUI.enabled = inMenuScene;
			bool newDisableIngameMusic = DrawCustomToggle(menuTab.DisableIngameMusic, Loc.GUI.Toggle_DisableIngameMusic);
			if (newDisableIngameMusic != menuTab.DisableIngameMusic && inMenuScene) menuTab.SetDisableIngameMusic(newDisableIngameMusic, prefs);
			GUILayout.Space(5);
			bool newMuteTunnel = DrawCustomToggle(menuTab.MuteCarnivalTunnel, Loc.GUI.Toggle_MuteTunnel);
			if (newMuteTunnel != menuTab.MuteCarnivalTunnel && inMenuScene) menuTab.SetMuteCarnivalTunnel(newMuteTunnel, prefs);
			GUILayout.Space(5);
			bool newMuteClock = DrawCustomToggle(menuTab.MuteCarnivalClockSounds, Loc.GUI.Toggle_MuteCarnivalClockSounds);
			if (newMuteClock != menuTab.MuteCarnivalClockSounds && inMenuScene) menuTab.SetMuteCarnivalClockSounds(newMuteClock, prefs);
			GUILayout.Space(5);
		}

		private void DrawMenuTab()
		{
			DrawHeader(Loc.GUI.Header_MenuBackground);
			GUILayout.Space(8);
			bool inLobby = menuTab.IsInLobby();
			bool cooldown = menuTab.IsMapSwitchCooldownActive;
			bool canChangeMaps = menuTab.SceneEnabled && !inLobby && !cooldown;
			UnityEngine.GUI.enabled = !inLobby;
			bool newSceneEnabled = DrawCustomToggle(menuTab.SceneEnabled, Loc.GUI.Toggle_CustomBackground);
			if (newSceneEnabled != menuTab.SceneEnabled && !inLobby) menuTab.SetSceneEnabled(newSceneEnabled, prefs);
			UnityEngine.GUI.enabled = true;
			GUILayout.Space(18);
			if (inLobby) GUILayout.Space(8);
			UnityEngine.GUI.enabled = canChangeMaps;
			int[] order = { 3, 4, 5, 1, 6, 2, 7 };
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			for (int i = 0; i < 4; i++)
			{
				var map = GetMapInfoById(order[i]);
				if (map != null)
				{
					bool isSelected = (menuTab.SelectedBg == map.Id);
					string label = GetMapLabelById(map.Id);
					float buttonWidth = GetMapButtonWidth(map.Id);
					if (DrawMapButton(label, isSelected, buttonWidth)) menuTab.SetSelectedBg(map.Id, prefs);
				}
				if (i < 3) GUILayout.Space(MAP_BUTTON_SPACING);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			for (int i = 4; i < 7; i++)
			{
				var map = GetMapInfoById(order[i]);
				if (map != null)
				{
					bool isSelected = (menuTab.SelectedBg == map.Id);
					string label = GetMapLabelById(map.Id);
					float buttonWidth = GetMapButtonWidth(map.Id);
					if (DrawMapButton(label, isSelected, buttonWidth)) menuTab.SetSelectedBg(map.Id, prefs);
				}
				if (i < 6) GUILayout.Space(MAP_BUTTON_SPACING);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			UnityEngine.GUI.enabled = true;
			GUILayout.Space(18);
			DrawHeader(Loc.GUI.Header_Audio);
			GUILayout.Space(8);
			bool newMusicEnabled = DrawCustomToggle(menuTab.RememberMusic, Loc.GUI.Toggle_RememberMusic);
			if (newMusicEnabled != menuTab.RememberMusic) menuTab.SetRememberMusic(newMusicEnabled, prefs);
		}

		private Misc.MapInfo GetMapInfoById(int id)
		{
			if (_mapInfoById != null && _mapInfoById.TryGetValue(id, out var m))
				return m;
			return null;
		}

		private string GetMapLabelById(int id)
		{
			switch (id)
			{
				case 1: return Loc.MenuText.Town;
				case 2: return Loc.MenuText.Manor;
				case 3: return Loc.MenuText.Farmhouse;
				case 4: return Loc.MenuText.Asylum;
				case 5: return Loc.MenuText.Inn;
				case 6: return Loc.MenuText.Slaughterhouse;
				case 7: return Loc.MenuText.Carnival;
				default: return Loc.GUI.UnknownSymbol;
			}
		}

		private float GetMapButtonWidth(int mapId)
		{
			switch (mapId)
			{
				case 5: return 65f;
				case 1: return 70f;
				case 2: return 75f;
				case 4: return 82f;
				case 7: return 90f;
				case 3: return 95f;
				case 6: return 115f;
				default: return 88f;
			}
		}

		private bool DrawMapButton(string label, bool isSelected, float buttonWidth)
		{
			bool clicked = ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(MAP_BUTTON_HEIGHT));
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
			{

				string hexColor = isSelected ? HEX_GREEN : HEX_RED;
				DrawButtonTextWithColoredSymbol(rInset, isSelected ? "✓" : "", label, theme.TabTitleStyle, hexColor);
			}
			return clicked && UnityEngine.GUI.enabled;
		}

		private void DrawHudTab()
		{
			if (currentHudSubMenu != HudSubMenu.NONE)
			{
				DrawHeader(Loc.GUI.Header_HUD);
				GUILayout.Space(8);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(100)))
					currentHudSubMenu = HudSubMenu.NONE;
				Rect rBack = GUILayoutUtility.GetLastRect();
				Rect rBackInset = new Rect(rBack.x + 1f, rBack.y + 1f, rBack.width - 2f, rBack.height - 2f);
				if (Event.current.type == EventType.Repaint)
					DrawOutlinedCenteredText(rBackInset, Loc.GUI.Button_Back, theme.TabTitleStyle, Color.white, _outlineColor, 1);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(12);


				switch (currentHudSubMenu)
				{
					case HudSubMenu.FPS:
						if (hudManager != null)
						{
							bool newShowFps = DrawCustomToggle(hudManager.ShowFps, Loc.GUI.Toggle_EnableFPS);
							if (newShowFps != hudManager.ShowFps) hudManager.ShowFps = newShowFps;

							bool newHidePrefix = DrawCustomToggle(hudManager.HideFpsPrefix, Loc.GUI.Toggle_HidePrefix);
							if (newHidePrefix != hudManager.HideFpsPrefix) hudManager.HideFpsPrefix = newHidePrefix;

							bool newPrefixFormat = DrawCustomToggle(hudManager.FpsUsePrefixFormat, Loc.GUI.Toggle_InvertPrefix);
							if (newPrefixFormat != hudManager.FpsUsePrefixFormat) hudManager.FpsUsePrefixFormat = newPrefixFormat;



							bool newUppercase = DrawCustomToggle(hudManager.FpsUppercaseLabel, Loc.GUI.Toggle_UppercasePrefix);
							if (newUppercase != hudManager.FpsUppercaseLabel) hudManager.FpsUppercaseLabel = newUppercase;
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HueFormat, Mathf.RoundToInt(hudManager.FpsHue * 360f)));
							float newHue = DrawCustomSlider(hudManager.FpsHue, 0f, 1f);
							if (!Mathf.Approximately(newHue, hudManager.FpsHue)) hudManager.FpsHue = newHue;
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HudOpacityFormat, Mathf.RoundToInt(hudManager.FpsOpacity * 100f)));
							float newFpsOpacity = DrawCustomSlider(hudManager.FpsOpacity, 0f, 1f);
							if (!Mathf.Approximately(newFpsOpacity, hudManager.FpsOpacity))
							{
								hudManager.FpsOpacity = newFpsOpacity;
								if (prefs != null) prefs.SaveToFile(false);
							}
							GUILayout.Space(6);
							bool newFpsChroma = DrawCustomToggle(hudManager.FpsChroma, Loc.GUI.Toggle_HudChroma);
							if (newFpsChroma != hudManager.FpsChroma)
							{
								hudManager.FpsChroma = newFpsChroma;
								if (prefs != null) prefs.SaveToFile(false);
							}
						}
						break;

					case HudSubMenu.CPS:
						if (hudManager != null)
						{
							bool newShowCps = DrawCustomToggle(hudManager.ShowCps, Loc.GUI.Toggle_EnableCPS);
							if (newShowCps != hudManager.ShowCps) hudManager.ShowCps = newShowCps;

							bool newHidePrefix = DrawCustomToggle(hudManager.HideCpsPrefix, Loc.GUI.Toggle_HidePrefix);
							if (newHidePrefix != hudManager.HideCpsPrefix) hudManager.HideCpsPrefix = newHidePrefix;

							bool newPrefixFormat = DrawCustomToggle(hudManager.CpsUsePrefixFormat, Loc.GUI.Toggle_InvertPrefix);
							if (newPrefixFormat != hudManager.CpsUsePrefixFormat) hudManager.CpsUsePrefixFormat = newPrefixFormat;


							bool newUppercase = DrawCustomToggle(hudManager.CpsUppercaseLabel, Loc.GUI.Toggle_UppercasePrefix);
							if (newUppercase != hudManager.CpsUppercaseLabel) hudManager.CpsUppercaseLabel = newUppercase;

							bool newDual = DrawCustomToggle(hudManager.CpsSplit, Loc.GUI.Toggle_DualCPS);
							if (newDual != hudManager.CpsSplit) hudManager.CpsSplit = newDual;

							if (hudManager.CpsSplit)
							{
								GUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();
								if (DrawOutlinedActionButton(_isCapturingCpsKey1 ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.BindAFormat, hudManager.CpsKey1), 150f, 24f))
								{
									_isCapturingCpsKey1 = true;
									_isCapturingCpsKey2 = false;
								}
								GUILayout.Space(10);
								if (DrawOutlinedActionButton(_isCapturingCpsKey2 ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.BindBFormat, hudManager.CpsKey2), 150f, 24f))
								{
									_isCapturingCpsKey2 = true;
									_isCapturingCpsKey1 = false;
								}
								GUILayout.FlexibleSpace();
								GUILayout.EndHorizontal();
							}
							GUILayout.Space(16);
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HueFormat, Mathf.RoundToInt(hudManager.CpsHue * 360f)));
							float newHue = DrawCustomSlider(hudManager.CpsHue, 0f, 1f);
							if (!Mathf.Approximately(newHue, hudManager.CpsHue)) hudManager.CpsHue = newHue;
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HudOpacityFormat, Mathf.RoundToInt(hudManager.CpsOpacity * 100f)));
							float newCpsOpacity = DrawCustomSlider(hudManager.CpsOpacity, 0f, 1f);
							if (!Mathf.Approximately(newCpsOpacity, hudManager.CpsOpacity))
							{
								hudManager.CpsOpacity = newCpsOpacity;
								if (prefs != null) prefs.SaveToFile(false);
							}
							GUILayout.Space(6);
							bool newCpsChroma = DrawCustomToggle(hudManager.CpsChroma, Loc.GUI.Toggle_HudChroma);
							if (newCpsChroma != hudManager.CpsChroma)
							{
								hudManager.CpsChroma = newCpsChroma;
								if (prefs != null) prefs.SaveToFile(false);
							}
						}
						break;

					case HudSubMenu.COORDS:
						if (hudManager != null)
						{
							bool newShowCoords = DrawCustomToggle(hudManager.ShowCoords, Loc.GUI.Toggle_ShowCoordinates);
							if (newShowCoords != hudManager.ShowCoords) hudManager.ShowCoords = newShowCoords;

							bool newXYZ = DrawCustomToggle(hudManager.CoordsXYZPrefix, Loc.GUI.Toggle_ShowPrefix);
							if (newXYZ != hudManager.CoordsXYZPrefix) hudManager.CoordsXYZPrefix = newXYZ;

							bool newHide = DrawCustomToggle(hudManager.CoordsHidePrefix, Loc.GUI.Toggle_HidePrefix);
							if (newHide != hudManager.CoordsHidePrefix) hudManager.CoordsHidePrefix = newHide;

							bool newUpper = DrawCustomToggle(hudManager.CoordsUppercaseLabel, Loc.GUI.Toggle_UppercasePrefix);
							if (newUpper != hudManager.CoordsUppercaseLabel) hudManager.CoordsUppercaseLabel = newUpper;

							bool newVertical = DrawCustomToggle(hudManager.CoordsVertical, Loc.GUI.Toggle_VerticalPosition);
							if (newVertical != hudManager.CoordsVertical) hudManager.CoordsVertical = newVertical;

							GUILayout.Space(16);
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HueFormat, Mathf.RoundToInt(hudManager.CoordsHue * 360f)));
							float newHue = DrawCustomSlider(hudManager.CoordsHue, 0f, 1f);
							if (!Mathf.Approximately(newHue, hudManager.CoordsHue)) hudManager.CoordsHue = newHue;
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HudOpacityFormat, Mathf.RoundToInt(hudManager.CoordsOpacity * 100f)));
							float newCoordsOpacity = DrawCustomSlider(hudManager.CoordsOpacity, 0f, 1f);
							if (!Mathf.Approximately(newCoordsOpacity, hudManager.CoordsOpacity))
							{
								hudManager.CoordsOpacity = newCoordsOpacity;
								if (prefs != null) prefs.SaveToFile(false);
							}
							GUILayout.Space(6);
							bool newCoordsChroma = DrawCustomToggle(hudManager.CoordsChroma, Loc.GUI.Toggle_HudChroma);
							if (newCoordsChroma != hudManager.CoordsChroma)
							{
								hudManager.CoordsChroma = newCoordsChroma;
								if (prefs != null) prefs.SaveToFile(false);
							}
						}
						break;

					case HudSubMenu.ENRAGE:
						if (hudManager != null)
						{
							bool newShowStun = DrawCustomToggle(hudManager.ShowStunStatus, Loc.GUI.Toggle_ShowEnrageStatus);
							if (newShowStun != hudManager.ShowStunStatus) hudManager.ShowStunStatus = newShowStun;


							bool newHideBg = DrawCustomToggle(hudManager.HideStunBackground, Loc.GUI.Toggle_NoBackground);
							if (newHideBg != hudManager.HideStunBackground) hudManager.HideStunBackground = newHideBg;
						}
						break;

					case HudSubMenu.SPEED:
						if (anticheatTab != null)
						{
							bool newEnabled = DrawCustomToggle(anticheatTab.Enabled, Loc.GUI.Toggle_EnableSpeedDetector);
							if (newEnabled != anticheatTab.Enabled) anticheatTab.SetEnabled(newEnabled, prefs);

							GUILayout.Space(10);

							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.WarningDurationFormat, anticheatTab.AlertDuration));
							float newDur = DrawCustomSlider(anticheatTab.AlertDuration, 3f, 15f);
							if (!Mathf.Approximately(newDur, anticheatTab.AlertDuration))
								anticheatTab.SetAlertDuration(newDur, prefs);

							GUILayout.Space(10);

							int alertCount = anticheatTab.GetAlertCount();
							int trackedCount = anticheatTab.GetTrackedPlayerCount();
							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							GUILayout.Label(string.Format(Loc.GUI.AlertsTrackedFormat, alertCount, trackedCount), theme.LabelStyle);
							GUILayout.FlexibleSpace();
							GUILayout.EndHorizontal();

							GUILayout.Space(8);

							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							string clearLabel = alertCount > 0 ? string.Format(Loc.GUI.ClearAlertsWithCountFormat, alertCount) : Loc.GUI.Button_ClearAlertsSimple;
							if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
							{
								anticheatTab.ClearAlerts();
							}
							Rect rClear = GUILayoutUtility.GetLastRect();
							Rect rClearInset = new Rect(rClear.x + 1f, rClear.y + 1f, rClear.width - 2f, rClear.height - 2f);
							if (Event.current.type == EventType.Repaint)
								DrawOutlinedCenteredText(rClearInset, clearLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
							GUILayout.FlexibleSpace();
							GUILayout.EndHorizontal();

							GUILayout.Space(15);
						}
						break;

					case HudSubMenu.TIMER:
						if (hudManager != null)
						{
							bool newShow = DrawCustomToggle(hudManager.ShowTimer, Loc.GUI.Toggle_ShowGameTime);
							if (newShow != hudManager.ShowTimer) hudManager.ShowTimer = newShow;

							bool newLeading = DrawCustomToggle(hudManager.TimerShowLeadingMinutes, Loc.GUI.Toggle_ShowLeadingZeros);
							if (newLeading != hudManager.TimerShowLeadingMinutes) hudManager.TimerShowLeadingMinutes = newLeading;

							bool newDecimals = DrawCustomToggle(hudManager.TimerShowDecimals, Loc.GUI.Toggle_ShowDecimals);
							if (newDecimals != hudManager.TimerShowDecimals) hudManager.TimerShowDecimals = newDecimals;

							if (hudManager.TimerShowDecimals)
							{
								GUILayout.Space(10);

								GUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();

								string dpLabel = string.Format(Loc.GUI.DecimalsAmountFormat, hudManager.TimerDecimalPlaces);
								if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
								{
									int next = hudManager.TimerDecimalPlaces + 1;
									if (next > 3) next = 0;
									hudManager.TimerDecimalPlaces = next;
								}
								Rect rDp = GUILayoutUtility.GetLastRect();
								Rect rDpInset = new Rect(rDp.x + 1f, rDp.y + 1f, rDp.width - 2f, rDp.height - 2f);
								if (Event.current.type == EventType.Repaint)
									DrawOutlinedCenteredText(rDpInset, dpLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);

								GUILayout.FlexibleSpace();
								GUILayout.EndHorizontal();
							}

							GUILayout.Space(10);

							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HueFormat, Mathf.RoundToInt(hudManager.TimerHue * 360f)));
							float newTimerHue = DrawCustomSlider(hudManager.TimerHue, 0f, 1f);
							if (!Mathf.Approximately(newTimerHue, hudManager.TimerHue)) hudManager.TimerHue = newTimerHue;
							DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.HudOpacityFormat, Mathf.RoundToInt(hudManager.TimerOpacity * 100f)));
							float newTimerOpacity = DrawCustomSlider(hudManager.TimerOpacity, 0f, 1f);
							if (!Mathf.Approximately(newTimerOpacity, hudManager.TimerOpacity))
							{
								hudManager.TimerOpacity = newTimerOpacity;
								if (prefs != null) prefs.SaveToFile(false);
							}
							GUILayout.Space(6);
							bool newTimerChroma = DrawCustomToggle(hudManager.TimerChroma, Loc.GUI.Toggle_HudChroma);
							if (newTimerChroma != hudManager.TimerChroma)
							{
								hudManager.TimerChroma = newTimerChroma;
								if (prefs != null) prefs.SaveToFile(false);
							}
						}
						break;
				}

				return;
			}


			DrawHeader(Loc.GUI.Header_HUD);
			GUILayout.Space(8);


			const float btnW = TAB_VIS_BUTTON_WIDTH;
			const float btnH = 24f;
			const float gap = 10f;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (DrawOutlinedButton(Loc.GUI.HudMenu_FpsCounter, theme.TabInactiveStyle, btnW, btnH)) currentHudSubMenu = HudSubMenu.FPS;
			GUILayout.Space(gap);
			if (DrawOutlinedButton(Loc.GUI.HudMenu_CpsCounter, theme.TabInactiveStyle, btnW, btnH)) currentHudSubMenu = HudSubMenu.CPS;
			GUILayout.Space(gap);
			if (DrawOutlinedButton(Loc.GUI.HudMenu_Coordinates, theme.TabInactiveStyle, btnW, btnH)) currentHudSubMenu = HudSubMenu.COORDS;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (DrawOutlinedButton(Loc.GUI.HudMenu_EnrageStatus, theme.TabInactiveStyle, btnW, btnH)) currentHudSubMenu = HudSubMenu.ENRAGE;
			GUILayout.Space(gap);
			if (DrawOutlinedButton(Loc.GUI.HudMenu_SpeedDetector, theme.TabInactiveStyle, btnW, btnH)) currentHudSubMenu = HudSubMenu.SPEED;
			GUILayout.Space(gap);
			if (DrawOutlinedButton(Loc.GUI.HudMenu_GameTime, theme.TabInactiveStyle, btnW, btnH)) currentHudSubMenu = HudSubMenu.TIMER;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(14);
			if (hudManager != null)
			{
				DrawCenteredLabelAboveSlider(Loc.GUI.HudSizeLabel);
				float newScale = DrawCustomSlider(hudManager.HudScale, 1f, 1.4f);
				if (!Mathf.Approximately(newScale, hudManager.HudScale))
					hudManager.HudScale = newScale;
			}
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string hudStyleName = (hudManager != null && hudManager.HudTextStyleMode == 1) ? Loc.GUI.TextStyle_Shadow : Loc.GUI.TextStyle_Outline;
			string hudStyleLabel = string.Format(Loc.GUI.Button_HudTextStyleFormat, hudStyleName);
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
			{
				if (hudManager != null)
				{
					hudManager.HudTextStyleMode = hudManager.HudTextStyleMode == 1 ? 0 : 1;
					if (prefs != null) prefs.SaveToFile(false);
				}
			}
			Rect rHudStyle = GUILayoutUtility.GetLastRect();
			Rect rHudStyleInset = new Rect(rHudStyle.x + 1f, rHudStyle.y + 1f, rHudStyle.width - 2f, rHudStyle.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rHudStyleInset, hudStyleLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();



			GUILayout.Space(6);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string posButtonLabel = anticheatTab.IsEditingPosition ? Loc.GUI.Button_SavePositions : Loc.GUI.Button_EditPositions;
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
			{
				anticheatTab.ToggleEditPositionMode(prefs);

				if (!anticheatTab.IsEditingPosition)
				{
					resetHudPosConfirmPending = false;
				}
			}
			Rect r2 = GUILayoutUtility.GetLastRect();
			Rect r2Inset = new Rect(r2.x + 1f, r2.y + 1f, r2.width - 2f, r2.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(r2Inset, posButtonLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (anticheatTab.IsEditingPosition)
			{
				GUILayout.Space(6);

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				string resetHudLabel = resetHudPosConfirmPending ? Loc.GUI.AreYouSure : Loc.GUI.Button_ResetPositions;
				if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
				{
					if (!resetHudPosConfirmPending) resetHudPosConfirmPending = true;
					else
					{
						resetHudPosConfirmPending = false;

						try { hudManager?.ResetAllHudPositionsToDefaults(); } catch { }
						try { anticheatTab?.ResetAlertPosition(prefs); } catch { }
						try { stunStatus?.ResetPositionToDefaults(prefs); } catch { }
					}
				}
				Rect rResetHud = GUILayoutUtility.GetLastRect();
				Rect rResetHudInset = new Rect(rResetHud.x + 1f, rResetHud.y + 1f, rResetHud.width - 2f, rResetHud.height - 2f);
				if (Event.current.type == EventType.Repaint)
					DrawOutlinedCenteredText(rResetHudInset, resetHudLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}

		}


		private void DrawSettingsTab(bool inMenu, bool inValidMap)
		{
			if (isEditingTheme)
			{
				DrawHeader(Loc.GUI.Header_Appearance);
				GUILayout.Space(8);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(100)))
					isEditingTheme = false;
				Rect rBack = GUILayoutUtility.GetLastRect();
				Rect rBackInset = new Rect(rBack.x + 1f, rBack.y + 1f, rBack.width - 2f, rBack.height - 2f);
				if (Event.current.type == EventType.Repaint)
					DrawOutlinedCenteredText(rBackInset, Loc.GUI.Button_Back, theme.TabTitleStyle, Color.white, _outlineColor, 1);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(20);
				DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.ThemeTabsHueFormat, Mathf.RoundToInt(theme.ThemeHue * 360)));
				float newThemeHue = DrawCustomSlider(theme.ThemeHue, 0f, 1f);
				if (!Mathf.Approximately(newThemeHue, theme.ThemeHue)) theme.ThemeHue = newThemeHue;

				GUILayout.Space(10);
				DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.ThemeBackgroundHueFormat, Mathf.RoundToInt(theme.BackgroundHue * 360)));
				float newBgHue = DrawCustomSlider(theme.BackgroundHue, 0f, 1f);
				if (!Mathf.Approximately(newBgHue, theme.BackgroundHue)) theme.BackgroundHue = newBgHue;

				GUILayout.Space(10);
				DrawCenteredLabelAboveSlider(string.Format(Loc.GUI.BackgroundOpacityFormat, Mathf.RoundToInt(theme.BackgroundOpacity * 100f)));
				float newBgOpacity = DrawCustomSlider(theme.BackgroundOpacity, 0f, 1f);
				if (!Mathf.Approximately(newBgOpacity, theme.BackgroundOpacity)) theme.BackgroundOpacity = newBgOpacity;

				GUILayout.Space(14);
				bool newDefaultBg = DrawCustomToggle(theme.UseDefaultBackground, Loc.GUI.Toggle_DefaultBackground);
				if (newDefaultBg != theme.UseDefaultBackground) theme.UseDefaultBackground = newDefaultBg;

				GUILayout.Space(6);
				bool newDarkMode = DrawCustomToggle(theme.DarkMode, Loc.GUI.Toggle_DarkMode);
				if (newDarkMode != theme.DarkMode) theme.DarkMode = newDarkMode;

				GUILayout.Space(6);
				bool newChroma = DrawCustomToggle(theme.ChromaEnabled, Loc.GUI.Toggle_ChromaMode);
				if (newChroma != theme.ChromaEnabled) theme.ChromaEnabled = newChroma;

				GUILayout.Space(6);
				bool newUiClicks = DrawCustomToggle(_uiClickSoundsEnabled, Loc.GUI.ClickSoundsLabel);
				if (newUiClicks != _uiClickSoundsEnabled)
				{
					_uiClickSoundsEnabled = newUiClicks;
					if (prefUiClickSounds != null) prefUiClickSounds.Value = newUiClicks;
					if (prefs != null) prefs.SaveToFile(false);
					PlayUiClick(true);
				}

				GUILayout.Space(6);
				bool newButtonShadows = DrawCustomToggle(_uiControlShadowsEnabled, Loc.GUI.ButtonShadowLabel);
				if (newButtonShadows != _uiControlShadowsEnabled)
				{
					_uiControlShadowsEnabled = newButtonShadows;
					if (prefUiControlShadows != null) prefUiControlShadows.Value = newButtonShadows;
					if (prefs != null) prefs.SaveToFile(false);
					if (infoManager != null) infoManager.UiControlShadowsEnabled = _uiControlShadowsEnabled;
					PlayUiClick(true);
				}

				GUILayout.Space(12);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				string uiStyleName = (theme.UiTextStyleMode == 1) ? Loc.GUI.TextStyle_Shadow : Loc.GUI.TextStyle_Outline;
				string uiStyleLabel = string.Format(Loc.GUI.Button_UiTextStyleFormat, uiStyleName);
				if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
				{
					theme.UiTextStyleMode = theme.UiTextStyleMode == 1 ? 0 : 1;
					if (prefs != null) prefs.SaveToFile(false);
				}
				Rect rUiStyle = GUILayoutUtility.GetLastRect();
				Rect rUiStyleInset = new Rect(rUiStyle.x + 1f, rUiStyle.y + 1f, rUiStyle.width - 2f, rUiStyle.height - 2f);
				if (Event.current.type == EventType.Repaint)
					DrawOutlinedCenteredText(rUiStyleInset, uiStyleLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				return;
			}
			DrawHeader(Loc.GUI.Header_Hotkeys);
			GUILayout.Space(8);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string menuKeyText = _isCapturingMenuKey ? Loc.GUI.PressAnyKey : string.Format(Loc.GUI.MenuKeyFormat, _toggleGuiKey);
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200))) StartCapturingMenuKey();
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, menuKeyText, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(18);
			DrawHeader(Loc.GUI.Header_ThemeColor);
			GUILayout.Space(8);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200))) isEditingTheme = true;
			Rect rTheme = GUILayoutUtility.GetLastRect();
			Rect rThemeInset = new Rect(rTheme.x + 1f, rTheme.y + 1f, rTheme.width - 2f, rTheme.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rThemeInset, Loc.GUI.CustomizeAppearanceTitle, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(2);
			DrawTabVisibilityUI();
			GUILayout.Space(18);
			DrawHeader(Loc.GUI.Header_Miscellaneous);
			GUILayout.Space(8);

			GUILayout.Space(8);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string langLabel = Loc.GUI.LanguageLabel;
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
			{
				Loc.Current = (Language)(((int)Loc.Current + 1) % 3);
				theme.InvalidateStyles();
			}
			Rect rLang = GUILayoutUtility.GetLastRect();
			Rect rLangInset = new Rect(rLang.x + 1f, rLang.y + 1f, rLang.width - 2f, rLang.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rLangInset, langLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string resetLabel = resetConfirmPending ? Loc.GUI.AreYouSure : Loc.GUI.ResetClientSettings;
			bool canReset = menuTab != null && menuTab.IsMenuSceneActive();
			UnityEngine.GUI.enabled = canReset;
			if (ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(200)))
			{
				if (!resetConfirmPending) resetConfirmPending = true;
				else
				{
					resetConfirmPending = false;
					var mod = Melon<Main>.Instance;
					if (mod != null) mod.ApplyFirstRunOffDefaults(false);
					ResetGuiThemeToDefaults(inMenu, inValidMap);
				}
			}
			UnityEngine.GUI.enabled = true;
			Rect r2 = GUILayoutUtility.GetLastRect();
			Rect r2Inset = new Rect(r2.x + 1f, r2.y + 1f, r2.width - 2f, r2.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(r2Inset, resetLabel, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawTabVisibilityUI()
		{
			GUILayout.Space(20);
			DrawHeader(Loc.GUI.Header_VisibleCategories);
			GUILayout.Space(15);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			DrawTabVisibilityToggle(Loc.GUI.Tab_Optimize, ref showOptimizeTab, prefShowOptimizeTab, 0);
			GUILayout.Space(10);
			DrawTabVisibilityToggle(Loc.GUI.Tab_HSV, ref showHSVTab, prefShowHSVTab, 1);
			GUILayout.Space(10);
			DrawTabVisibilityToggle(Loc.GUI.Tab_Speedrun, ref showSpeedrunTab, prefShowSpeedrunTab, 2);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			DrawTabVisibilityToggle(Loc.GUI.Tab_Gameplay, ref showFOVTab, prefShowFOVTab, 3);
			GUILayout.Space(10);
			DrawTabVisibilityToggle(Loc.GUI.Tab_HUD, ref showHudTab, prefShowHudTab, 7);
			GUILayout.Space(10);
			DrawTabVisibilityToggle(Loc.GUI.Tab_Menu, ref showMenuTab, prefShowMenuTab, 5);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawTabVisibilityToggle(string label, ref bool flag, MelonPreferences_Entry<bool> pref, int tabIndex)
		{
			if (DrawVisibilityToggleButton(label, ref flag))
			{
				flag = !flag;
				if (pref != null) pref.Value = flag;
				if (!flag && _selectedTab == tabIndex) _selectedTab = 8;
				if (prefs != null) prefs.SaveToFile(false);
			}
		}

		private bool DrawVisibilityToggleButton(string label, ref bool isOn)
		{

			string hexColor = isOn ? HEX_GREEN : HEX_RED;
			bool clicked = ClickButton(GUIContent.none, theme.TabInactiveStyle, GUILayout.Width(TAB_VIS_BUTTON_WIDTH), GUILayout.Height(24));
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawButtonTextWithColoredSymbol(rInset, isOn ? "✓" : "✗", label, theme.TabTitleStyle, hexColor);
			return clicked;
		}


		private void DrawButtonTextWithColoredSymbol(Rect rect, string symbol, string label, GUIStyle style, string hexColor)
		{

			if (Event.current.type != EventType.Repaint) return;

			if (_tempStyle == null)
			{
				_tempStyle = new GUIStyle(style);
				_tempStyle.normal.textColor = Color.white;
				_tempStyle.fontSize = style.fontSize;
				_tempStyle.fontStyle = style.fontStyle;
				_tempStyle.richText = true;
				_tempStyle.alignment = TextAnchor.MiddleCenter;
			}
			else
			{
				if (_tempStyle.fontSize != style.fontSize) _tempStyle.fontSize = style.fontSize;
				if (_tempStyle.fontStyle != style.fontStyle) _tempStyle.fontStyle = style.fontStyle;
				if (!_tempStyle.richText) _tempStyle.richText = true;
				if (_tempStyle.alignment != TextAnchor.MiddleCenter) _tempStyle.alignment = TextAnchor.MiddleCenter;
			}

			string coloredText;
			if (!string.IsNullOrEmpty(symbol))
				coloredText = $"<color=#{hexColor}>{symbol}</color> {label}";
			else
				coloredText = label;

			Color prev = UnityEngine.GUI.color;

			if (theme != null && theme.UiTextStyleMode == 1)
			{
				UnityEngine.GUI.color = _outlineColor;
				UnityEngine.GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), string.IsNullOrEmpty(symbol) ? label : $"{symbol} {label}", _tempStyle);
				UnityEngine.GUI.color = prev;
				_tempStyle.normal.textColor = Color.white;
				UnityEngine.GUI.Label(rect, coloredText, _tempStyle);
				return;
			}

			UnityEngine.GUI.color = _outlineColor;
			string fullText = string.IsNullOrEmpty(symbol) ? label : $"{symbol} {label}";

			UnityEngine.GUI.Label(new Rect(rect.x - 1, rect.y - 1, rect.width, rect.height), fullText, _tempStyle);
			UnityEngine.GUI.Label(new Rect(rect.x + 1, rect.y - 1, rect.width, rect.height), fullText, _tempStyle);
			UnityEngine.GUI.Label(new Rect(rect.x - 1, rect.y + 1, rect.width, rect.height), fullText, _tempStyle);
			UnityEngine.GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), fullText, _tempStyle);

			UnityEngine.GUI.color = prev;

			_tempStyle.normal.textColor = Color.white;
			UnityEngine.GUI.Label(rect, coloredText, _tempStyle);
		}

		private void ResetGuiThemeToDefaults(bool inMenu, bool inValidMap)
		{


			bool keepUseDefaultBg = theme != null && theme.UseDefaultBackground;
			bool keepHideBackground = theme != null && theme.HideBackground;

			const float DEFAULT_THEME_HUE = 270f / 360f;
			const float DEFAULT_BG_HUE = 0.5f;
			const float DEFAULT_BG_OPACITY = 0.95f;
			const float DEFAULT_HSV = 1f;
			const int DEFAULT_RANK = 17;

			theme.ThemeHue = DEFAULT_THEME_HUE;
			theme.DarkMode = false;
			theme.ResetBackground(DEFAULT_BG_HUE, DEFAULT_BG_OPACITY, keepUseDefaultBg);

			theme.HideBackground = keepHideBackground;

			_toggleGuiKey = KeyCode.RightShift;
			prefMenuKey.Value = KeyCode.RightShift;

			if (hsvTab != null)
			{
				hsvTab.SetHSV(DEFAULT_HSV, DEFAULT_HSV, DEFAULT_HSV, prefs);
				SetIconRankToTarget(DEFAULT_RANK);
			}
			if (outfitTab != null) outfitTab.SetHSV(DEFAULT_HSV, DEFAULT_HSV, DEFAULT_HSV, prefs);

			if (optimizeTab != null)
			{
				optimizeTab.SetWeatherDisabled(false, prefs, inMenu, inValidMap);
				optimizeTab.SetMuteWeatherAudio(false, prefs, inMenu, inValidMap);
			}

			theme?.InvalidateStyles();
			if (prefs != null) prefs.SaveToFile(false);
		}





		private void EnsureUiClickAudio()
		{
			if (_uiClickSource == null)
			{
				var go = new GameObject("DevourCore_UI_ClickAudio");
				UnityEngine.Object.DontDestroyOnLoad(go);
				_uiClickSource = go.AddComponent<AudioSource>();
				_uiClickSource.playOnAwake = false;
				_uiClickSource.loop = false;
				_uiClickSource.spatialBlend = 0f;
			}

			if (_uiClickClip == null)
			{
				try
				{
					_uiClickClip = Resources.FindObjectsOfTypeAll<AudioClip>()
						.FirstOrDefault(c => c != null && string.Equals(c.name, "click", StringComparison.OrdinalIgnoreCase));
				}
				catch { }
			}



			if (_uiClickMixerGroup == null)
			{
				try
				{
					var src = Resources.FindObjectsOfTypeAll<AudioSource>()
						.FirstOrDefault(s => s != null && s.clip != null &&
							string.Equals(s.clip.name, "click", StringComparison.OrdinalIgnoreCase));

					if (src != null)
						_uiClickMixerGroup = src.outputAudioMixerGroup;
				}
				catch { }
			}

			if (_uiClickSource != null && _uiClickMixerGroup != null)
				_uiClickSource.outputAudioMixerGroup = _uiClickMixerGroup;
		}

		private void PlayUiClick(bool force = false)
		{
			if (!force && !_uiClickSoundsEnabled) return;

			EnsureUiClickAudio();
			if (_uiClickSource == null || _uiClickClip == null) return;


			float vol = 1f;
			if (_uiClickMixerGroup == null)
			{
				try
				{

					if (PlayerPrefs.HasKey("SFXVolume")) vol = Mathf.Clamp01(PlayerPrefs.GetFloat("SFXVolume", 1f));
					else if (PlayerPrefs.HasKey("SfxVolume")) vol = Mathf.Clamp01(PlayerPrefs.GetFloat("SfxVolume", 1f));
					else if (PlayerPrefs.HasKey("sfxVolume")) vol = Mathf.Clamp01(PlayerPrefs.GetFloat("sfxVolume", 1f));
				}
				catch { }
			}

			_uiClickSource.PlayOneShot(_uiClickClip, vol);
		}

		private bool ClickButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect r = GUILayoutUtility.GetRect(content, style, options);


			if (_uiControlShadowsEnabled && Event.current.type == EventType.Repaint && style != null)
			{
				GUIStyle shadowStyle = new GUIStyle(style);
				shadowStyle.normal.textColor = Color.clear;

				Color prev = GUI.color;
				GUI.color = new Color(0f, 0f, 0f, 0.28f);

				Rect shadowRect = r;
				shadowRect.x += 1f;
				shadowRect.y += 3f;
				shadowStyle.Draw(shadowRect, GUIContent.none, false, false, false, false);

				GUI.color = prev;
			}

			bool clicked = GUI.Button(r, content, style);
			if (clicked) PlayUiClick();
			return clicked;
		}


		private bool ClickButtonRect(Rect r, GUIContent content, GUIStyle style)
		{
			if (_uiControlShadowsEnabled && Event.current.type == EventType.Repaint && style != null)
			{
				GUIStyle shadowStyle = new GUIStyle(style);
				shadowStyle.normal.textColor = Color.clear;

				Color prev = GUI.color;
				GUI.color = new Color(0f, 0f, 0f, 0.28f);

				Rect shadowRect = r;
				shadowRect.x += 1f;
				shadowRect.y += 3f;
				shadowStyle.Draw(shadowRect, GUIContent.none, false, false, false, false);

				GUI.color = prev;
			}

			bool clicked = GUI.Button(r, content, style);
			if (clicked) PlayUiClick();
			return clicked;
		}

		private bool ClickButton(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect r = GUILayoutUtility.GetRect(new GUIContent(text), style, options);

			if (_uiControlShadowsEnabled && Event.current.type == EventType.Repaint && style != null)
			{
				GUIStyle shadowStyle = new GUIStyle(style);
				shadowStyle.normal.textColor = Color.clear;

				Color prev = GUI.color;
				GUI.color = new Color(0f, 0f, 0f, 0.28f);

				Rect shadowRect = r;
				shadowRect.x += 1f;
				shadowRect.y += 3f;
				shadowStyle.Draw(shadowRect, GUIContent.none, false, false, false, false);

				GUI.color = prev;
			}

			bool clicked = GUI.Button(r, text, style);
			if (clicked) PlayUiClick();
			return clicked;
		}


		private void DrawStyleShadow(Rect r, GUIStyle style, float dx = 2f, float dy = 3f, float alpha = 0.18f)
		{
			if (Event.current.type != EventType.Repaint) return;
			if (!_uiControlShadowsEnabled) return;
			if (style == null) return;

			GUIStyle shadowStyle = new GUIStyle(style);
			shadowStyle.normal.textColor = Color.clear;

			Color prev = GUI.color;
			GUI.color = new Color(0f, 0f, 0f, alpha);

			Rect shadowRect = r;
			shadowRect.x += dx;
			shadowRect.y += dy;
			shadowStyle.Draw(shadowRect, GUIContent.none, false, false, false, false);

			GUI.color = prev;
		}

		private float ShadowedHorizontalSlider(float value, float leftValue, float rightValue, GUIStyle sliderStyle, GUIStyle thumbStyle, params GUILayoutOption[] options)
		{
			Rect r = GUILayoutUtility.GetRect(GUIContent.none, sliderStyle, options);


			DrawStyleShadow(r, sliderStyle);

			DrawStyleShadow(r, sliderStyle, 2f, 2f);
			float v = GUI.HorizontalSlider(r, value, leftValue, rightValue, sliderStyle, thumbStyle);
			return v;
		}



		private bool DrawOutlinedButton(string label, GUIStyle style, float width, float height)
		{
			bool clicked = ClickButton(GUIContent.none, style, GUILayout.Width(width), GUILayout.Height(height));
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, label, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			return clicked;
		}

		private bool DrawOutlinedActionButton(string label, float width, float height)
		{
			bool clicked = ClickButton(GUIContent.none, theme.ButtonStyle, GUILayout.Width(width), GUILayout.Height(height));
			Rect r = GUILayoutUtility.GetLastRect();
			Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rInset, label, theme.TabTitleStyle, Color.white, _outlineColor, 1);
			return clicked;
		}


		private void DrawHeader(string text)
		{
			GUIContent content = new GUIContent(text);
			Vector2 size = theme.HeaderStyle.CalcSize(content);
			Rect rect = GUILayoutUtility.GetRect(size.x + 40f, size.y + 8f, GUILayout.ExpandWidth(true));
			Rect textRect = new Rect(rect.x + (rect.width - size.x) / 2f, rect.y, size.x, rect.height);

			if (Event.current.type != EventType.Repaint) return;

			Color prev = UnityEngine.GUI.color;
			UnityEngine.GUI.color = _headerShadowColor;
			UnityEngine.GUI.Label(new Rect(textRect.x + 1f, textRect.y + 1f, textRect.width, textRect.height), content, theme.HeaderStyle);
			UnityEngine.GUI.color = prev;
			UnityEngine.GUI.Label(textRect, content, theme.HeaderStyle);
		}

		private void CopyStyleBasics(GUIStyle src, GUIStyle dst)
		{
			if (src == null || dst == null) return;

			dst.font = src.font;
			dst.fontSize = src.fontSize;
			dst.fontStyle = src.fontStyle;
			dst.alignment = src.alignment;
			dst.richText = src.richText;
			dst.wordWrap = src.wordWrap;
			dst.clipping = src.clipping;

			dst.padding = src.padding;
			dst.margin = src.margin;
			dst.border = src.border;
			dst.overflow = src.overflow;

			dst.contentOffset = src.contentOffset;
			dst.imagePosition = src.imagePosition;

			dst.fixedWidth = src.fixedWidth;
			dst.fixedHeight = src.fixedHeight;
			dst.stretchWidth = src.stretchWidth;
			dst.stretchHeight = src.stretchHeight;
		}

		private void DrawShadowedCenteredTextAlways(Rect rect, string text, GUIStyle style, Color textColor, Color shadowColor)
		{
			if (Event.current.type != EventType.Repaint) return;
			if (style == null) style = GUI.skin.label;

			if (_tempStyle == null) _tempStyle = new GUIStyle(style);
			else CopyStyleBasics(style, _tempStyle);

			if (_tempStyle2 == null) _tempStyle2 = new GUIStyle(style);
			else CopyStyleBasics(style, _tempStyle2);

			_tempStyle.normal.textColor = shadowColor;
			Rect shadow = rect;
			shadow.x += 1f;
			shadow.y += 1f;
			_tempStyle.Draw(shadow, text, false, false, false, false);

			_tempStyle2.normal.textColor = textColor;
			_tempStyle2.Draw(rect, text, false, false, false, false);
		}

		void DrawOutlinedCenteredText(Rect rect, string text, GUIStyle style, Color textColor, Color outlineColor, int thickness = 1)
		{
			if (Event.current.type != EventType.Repaint) return;

			if (style == null) style = GUI.skin.label;

			if (_tempStyle == null) _tempStyle = new GUIStyle(style);
			else CopyStyleBasics(style, _tempStyle);

			if (_tempStyle2 == null) _tempStyle2 = new GUIStyle(style);
			else CopyStyleBasics(style, _tempStyle2);

			if (theme != null && theme.UiTextStyleMode == 1)
			{
				_tempStyle.normal.textColor = _headerShadowColor;
				Rect shadow = rect;
				shadow.x += 1f;
				shadow.y += 1f;
				_tempStyle.Draw(shadow, text, false, false, false, false);

				_tempStyle2.normal.textColor = textColor;
				_tempStyle2.Draw(rect, text, false, false, false, false);
				return;
			}

			_tempStyle.normal.textColor = outlineColor;
			_tempStyle2.normal.textColor = textColor;

			for (int x = -thickness; x <= thickness; x++)
			{
				for (int y = -thickness; y <= thickness; y++)
				{
					if (x == 0 && y == 0) continue;
					Rect o = new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
					_tempStyle.Draw(o, text, false, false, false, false);
				}
			}

			_tempStyle2.Draw(rect, text, false, false, false, false);
		}

		private bool DrawCustomToggle(bool value, string label)
		{
			GUIStyle cbStyle = UnityEngine.GUI.enabled ? theme.CheckboxStyle : theme.CheckboxDisabledStyle;


			float cbSize = (cbStyle.fixedHeight > 0f) ? cbStyle.fixedHeight :
				((cbStyle.fixedWidth > 0f) ? cbStyle.fixedWidth : 16f);

			float textH = (theme.ToggleStyle != null && theme.ToggleStyle.lineHeight > 0f) ? (theme.ToggleStyle.lineHeight + 6f) : 18f;
			float rowH = Mathf.Max(cbSize, textH);

			Rect rowRect = GUILayoutUtility.GetRect(0f, rowH, GUILayout.ExpandWidth(true));


			Rect cbRect = new Rect(rowRect.x, rowRect.y + (rowH - cbSize) * 0.5f, cbSize, cbSize);


			const float LABEL_PAD_X = 6f;
			Rect labelRect = new Rect(cbRect.xMax + LABEL_PAD_X, rowRect.y, Mathf.Max(0f, rowRect.width - cbSize - LABEL_PAD_X), rowH);


			DrawStyleShadow(cbRect, cbStyle, 2f, 2f);
			bool newValue = GUI.Toggle(cbRect, value, "", cbStyle);
			if (newValue != value) PlayUiClick();


			float labelClickW = labelRect.width;
			if (theme.ToggleStyle != null)
			{
				labelClickW = Mathf.Min(labelRect.width, theme.ToggleStyle.CalcSize(new GUIContent(label)).x + 8f);
			}
			Rect labelClickRect = new Rect(labelRect.x, labelRect.y, Mathf.Max(0f, labelClickW), labelRect.height);

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && labelClickRect.Contains(Event.current.mousePosition))
			{
				newValue = !newValue;
				PlayUiClick();
				Event.current.Use();
			}


			if (Event.current.type == EventType.Repaint)
			{
				Rect inset = new Rect(labelRect.x + 1f, labelRect.y + 1f, Mathf.Max(0f, labelRect.width - 2f), Mathf.Max(0f, labelRect.height - 2f));
				DrawStyleShadow(inset, theme.ToggleStyle, 1f, 3f, 0.5f);
				DrawShadowedCenteredTextAlways(inset, label, theme.ToggleStyle, Color.white, _headerShadowColor);
			}

			return newValue;
		}

		private void DrawCenteredLabelAboveSlider(string text)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUIContent content = new GUIContent(text);
			GUIStyle labelStyle = theme.SliderLabelStyle != null ? theme.SliderLabelStyle : GUI.skin.label;
			Vector2 size = labelStyle.CalcSize(content);
			Rect rect = GUILayoutUtility.GetRect(size.x + 20f, size.y + 4f, GUILayout.ExpandWidth(false));

			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(rect, text, labelStyle, Color.white, _outlineColor, 1);

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(4);
		}
		private float DrawCustomSlider(float value, float min, float max)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			Rect sliderRect = GUILayoutUtility.GetRect(SLIDER_FIXED_WIDTH, 16f, GUILayout.Width(SLIDER_FIXED_WIDTH), GUILayout.Height(16f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (Event.current.type == EventType.Repaint)
			{

				Rect shadowRect = sliderRect;
				shadowRect.x += 1f;
				shadowRect.y += 3f;
				if (_uiControlShadowsEnabled)
					UnityEngine.GUI.DrawTexture(shadowRect, theme.SliderBackground, ScaleMode.StretchToFill, true, 0f, new Color(0f, 0f, 0f, 0.20f), 0, 5);

				DrawRoundedTexture(sliderRect, theme.SliderBackground, 5, 0);

				float normalizedValue = (value - min) / (max - min);
				float margin = 2f;
				float availableWidth = sliderRect.width - margin * 2;
				float fillWidth = Mathf.Max(0, availableWidth * normalizedValue);

				if (fillWidth > 0)
				{
					Rect fillRect = new Rect(sliderRect.x + margin, sliderRect.y + margin, fillWidth, sliderRect.height - margin * 2);
					DrawRoundedTexture(fillRect, theme.SliderFillBackground, 4, 0);
				}
			}

			return UnityEngine.GUI.HorizontalSlider(sliderRect, value, min, max, GUIStyle.none, GUIStyle.none);
		}

		private float DrawCustomVerticalSlider(float value, float min, float max, string label)
		{
			GUILayout.BeginVertical(GUILayout.Width(16));
			Rect sliderRect = GUILayoutUtility.GetRect(16, 180, GUILayout.Width(16), GUILayout.Height(180));

			if (Event.current.type == EventType.Repaint)
			{

				Rect shadowRect = sliderRect;
				shadowRect.x += 1f;
				shadowRect.y += 3f;
				if (_uiControlShadowsEnabled)
					UnityEngine.GUI.DrawTexture(shadowRect, theme.SliderBackground, ScaleMode.StretchToFill, true, 0f, new Color(0f, 0f, 0f, 0.20f), 0, 5);

				DrawRoundedTexture(sliderRect, theme.SliderBackground, 5, 0);

				float normalizedValue = (value - min) / (max - min);
				float margin = 2f;
				float availableHeight = sliderRect.height - margin * 2;
				float fillHeight = Mathf.Max(0, availableHeight * normalizedValue);
				float fillY = sliderRect.y + margin + (availableHeight - fillHeight);

				if (fillHeight > 0)
				{
					Rect fillRect = new Rect(sliderRect.x + margin, fillY, sliderRect.width - margin * 2, fillHeight);
					DrawRoundedTexture(fillRect, theme.SliderFillBackground, 4, 0);
				}
			}

			float newValue = UnityEngine.GUI.VerticalSlider(sliderRect, value, max, min, GUIStyle.none, GUIStyle.none);

			GUILayout.Space(8);

			GUIContent content = new GUIContent(label);
			Vector2 size = theme.SliderLabelStyle.CalcSize(content);
			Rect labelRect = GUILayoutUtility.GetRect(16f, size.y + 4f, GUILayout.Width(16f), GUILayout.Height(size.y + 4f));

			if (Event.current.type == EventType.Repaint)
				DrawOutlinedCenteredText(labelRect, label, theme.SliderLabelStyle, Color.white, _outlineColor, 1);

			GUILayout.EndVertical();
			return newValue;
		}

		private float DrawHueSlider(float value, float width)
		{
			float height = 10f;
			Rect sliderRect = GUILayoutUtility.GetRect(width, height, GUILayout.Width(width), GUILayout.Height(height));
			sliderRect.y += 8f;

			if (Event.current.type == EventType.Repaint)
			{
				Rect shadowRect = sliderRect;
				shadowRect.x += 1f;
				shadowRect.y += 3f;
				if (_uiControlShadowsEnabled)
					UnityEngine.GUI.DrawTexture(shadowRect, theme.SliderBackground, ScaleMode.StretchToFill, true, 0f, new Color(0f, 0f, 0f, 0.20f), 0, 5);

				DrawRoundedTexture(sliderRect, theme.SliderBackground, 5, 0);

				float normalizedValue = Mathf.Clamp01(value);
				float margin = 2f;
				float availableWidth = sliderRect.width - margin * 2;
				float fillWidth = Mathf.Max(0, availableWidth * normalizedValue);

				if (fillWidth > 0)
				{
					Rect fillRect = new Rect(sliderRect.x + margin, sliderRect.y + margin, fillWidth, sliderRect.height - margin * 2);
					DrawRoundedTexture(fillRect, theme.SliderFillBackground, 4, 0);
				}
			}

			return UnityEngine.GUI.HorizontalSlider(sliderRect, value, 0f, 1f, GUIStyle.none, GUIStyle.none);
		}

		private void DrawRoundedTexture(Rect rect, Texture2D texture, int cornerRadius, int borderWidth = 0)
		{
			if (Event.current.type != EventType.Repaint) return;
			if (texture == null) return;
			UnityEngine.GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true, 0, Color.white, borderWidth, cornerRadius);
		}


		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory, bool firstRun)
		{
			if (prefsCategory == null) return;


			if (firstRun)
			{
				if (prefGuiX != null) prefGuiX.Value = 1384f;
				if (prefGuiY != null) prefGuiY.Value = 157f;
			}


			if (prefShowOptimizeTab != null) prefShowOptimizeTab.Value = true;
			if (prefShowHSVTab != null) prefShowHSVTab.Value = true;
			if (prefShowSpeedrunTab != null) prefShowSpeedrunTab.Value = true;
			if (prefShowFOVTab != null) prefShowFOVTab.Value = true;
			if (prefShowHudTab != null) prefShowHudTab.Value = true;
			if (prefShowMenuTab != null) prefShowMenuTab.Value = true;

			if (prefMenuKey != null) prefMenuKey.Value = KeyCode.RightShift;
			if (prefLastSelectedTab != null) prefLastSelectedTab.Value = 8;
			if (prefInitialDefaultsApplied != null) prefInitialDefaultsApplied.Value = true;
			if (prefUiClickSounds != null) prefUiClickSounds.Value = true;
			if (prefUiControlShadows != null) prefUiControlShadows.Value = true;



			showOptimizeTab = prefShowOptimizeTab != null ? prefShowOptimizeTab.Value : showOptimizeTab;
			showHSVTab = prefShowHSVTab != null ? prefShowHSVTab.Value : showHSVTab;
			showSpeedrunTab = prefShowSpeedrunTab != null ? prefShowSpeedrunTab.Value : showSpeedrunTab;
			showFOVTab = prefShowFOVTab != null ? prefShowFOVTab.Value : showFOVTab;
			showHudTab = prefShowHudTab != null ? prefShowHudTab.Value : showHudTab;
			showMenuTab = prefShowMenuTab != null ? prefShowMenuTab.Value : showMenuTab;

			_toggleGuiKey = prefMenuKey != null ? prefMenuKey.Value : _toggleGuiKey;
			_selectedTab = prefLastSelectedTab != null ? prefLastSelectedTab.Value : _selectedTab;
			_uiClickSoundsEnabled = prefUiClickSounds != null && prefUiClickSounds.Value;
			_uiControlShadowsEnabled = prefUiControlShadows != null && prefUiControlShadows.Value;

			prefsCategory.SaveToFile(false);
		}

	}
}