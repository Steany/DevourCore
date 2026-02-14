using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevourCore
{
	public class HudManager
	{
		private MelonPreferences_Category prefs;

		private MelonPreferences_Entry<bool> prefShowFps;
		private MelonPreferences_Entry<bool> prefShowCps;
		private MelonPreferences_Entry<bool> prefFpsShowInMenu;
		private MelonPreferences_Entry<bool> prefCpsShowInMenu;
		private MelonPreferences_Entry<bool> prefShowCoords;
		private MelonPreferences_Entry<bool> prefShowEnrage;

		private MelonPreferences_Entry<bool> prefShowTimer;
		private MelonPreferences_Entry<float> prefTimerHue;
		private MelonPreferences_Entry<bool> prefTimerShowDecimals;
		private MelonPreferences_Entry<int> prefTimerDecimalPlaces;
		private MelonPreferences_Entry<bool> prefTimerShowLeadingMinutes;

		private MelonPreferences_Entry<int> prefHudTextStyleMode;
		private MelonPreferences_Entry<float> prefHudScale;

		private MelonPreferences_Entry<bool> prefHideFpsPrefix;
		private MelonPreferences_Entry<bool> prefFpsUsePrefixFormat;
		private MelonPreferences_Entry<bool> prefFpsUppercaseLabel;
		private MelonPreferences_Entry<bool> prefHideCpsPrefix;
		private MelonPreferences_Entry<bool> prefCpsUsePrefixFormat;
		private MelonPreferences_Entry<bool> prefCpsUppercaseLabel;
		private MelonPreferences_Entry<bool> prefCoordsVertical;
		private MelonPreferences_Entry<bool> prefCoordsXYZPrefix;
		private MelonPreferences_Entry<bool> prefCoordsRemovePrefix;
		private MelonPreferences_Entry<bool> prefCoordsUppercaseLabel;
		private MelonPreferences_Entry<bool> prefHideStunBackground;

		private MelonPreferences_Entry<float> prefFpsHue;
		private MelonPreferences_Entry<float> prefCpsHue;
		private MelonPreferences_Entry<bool> prefCpsSplit;
		private MelonPreferences_Entry<KeyCode> prefCpsKey1;
		private MelonPreferences_Entry<KeyCode> prefCpsKey2;
		private MelonPreferences_Entry<float> prefCoordsHue;
		private MelonPreferences_Entry<float> prefTimerOpacity;
		private MelonPreferences_Entry<bool> prefTimerChroma;
		private MelonPreferences_Entry<float> prefFpsOpacity;
		private MelonPreferences_Entry<bool> prefFpsChroma;
		private MelonPreferences_Entry<float> prefCpsOpacity;
		private MelonPreferences_Entry<bool> prefCpsChroma;
		private MelonPreferences_Entry<float> prefCoordsOpacity;
		private MelonPreferences_Entry<bool> prefCoordsChroma;

		private MelonPreferences_Entry<float> prefFpsPosX;
		private MelonPreferences_Entry<float> prefFpsPosY;
		private MelonPreferences_Entry<float> prefCpsPosX;
		private MelonPreferences_Entry<float> prefCpsPosY;
		private MelonPreferences_Entry<float> prefCoordsPosX;
		private MelonPreferences_Entry<float> prefCoordsPosY;
		private MelonPreferences_Entry<float> prefTimerPosX;
		private MelonPreferences_Entry<float> prefTimerPosY;

		private bool showFps;
		private bool showCps;
		private bool showFpsInMenu;
		private bool showCpsInMenu;
		private bool showCoords;
		private bool showEnrage;
		private bool showTimer;

		private bool hideFpsPrefix;
		private bool fpsUsePrefixFormat;
		private bool fpsUppercaseLabel;
		private bool hideCpsPrefix;
		private bool cpsUsePrefixFormat;
		private bool cpsUppercaseLabel;
		private bool coordsVertical;
		private bool coordsUppercaseLabel;
		private bool hideStunBackground;

		private float fpsHue;
		private float cpsHue;
		private float coordsHue;
		private float timerOpacity = 1f;
		private bool timerChroma = false;
		private float fpsOpacity = 1f;
		private bool fpsChroma = false;
		private float cpsOpacity = 1f;
		private bool cpsChroma = false;
		private float coordsOpacity = 1f;
		private bool coordsChroma = false;

		private float hudScale = 1f;
		private float lastAppliedScale = -1f;
		private int hudTextStyleMode = 0;

		public static float GlobalHudScale { get; private set; } = 1f;

		public float TimerOpacity { get => timerOpacity; set { timerOpacity = Mathf.Clamp01(value); if (prefTimerOpacity != null) prefTimerOpacity.Value = timerOpacity; } }
		public bool TimerChroma { get => timerChroma; set { timerChroma = value; if (prefTimerChroma != null) prefTimerChroma.Value = timerChroma; } }
		public float FpsOpacity { get => fpsOpacity; set { fpsOpacity = Mathf.Clamp01(value); if (prefFpsOpacity != null) prefFpsOpacity.Value = fpsOpacity; } }
		public bool FpsChroma { get => fpsChroma; set { fpsChroma = value; if (prefFpsChroma != null) prefFpsChroma.Value = fpsChroma; } }
		public float CpsOpacity { get => cpsOpacity; set { cpsOpacity = Mathf.Clamp01(value); if (prefCpsOpacity != null) prefCpsOpacity.Value = cpsOpacity; } }
		public bool CpsChroma { get => cpsChroma; set { cpsChroma = value; if (prefCpsChroma != null) prefCpsChroma.Value = cpsChroma; } }
		public float CoordsOpacity { get => coordsOpacity; set { coordsOpacity = Mathf.Clamp01(value); if (prefCoordsOpacity != null) prefCoordsOpacity.Value = coordsOpacity; } }
		public bool CoordsChroma { get => coordsChroma; set { coordsChroma = value; if (prefCoordsChroma != null) prefCoordsChroma.Value = coordsChroma; } }


		private float timerHue;
		private bool timerShowDecimals;
		private int timerDecimalPlaces;
		private bool timerShowLeadingMinutes;
		private float timerPosX;
		private float timerPosY;
		private bool timerRunning;
		private float timerStartTime;

		private float fpsPosX;
		private float fpsPosY;
		private float cpsPosX;
		private float cpsPosY;
		private float coordsPosX;
		private float coordsPosY;

		private bool wasEditing = false;
		private bool wasEditingTimer = false;
		private float previewTimerStartTime = 0f;

		private bool dragFps = false;
		private bool dragCps = false;
		private bool dragCoords = false;
		private bool dragTimer = false;
		private Vector2 dragOffsetFps;
		private Vector2 dragOffsetCps;
		private Vector2 dragOffsetCoords;
		private Vector2 dragOffsetTimer;



		private bool _fpsStickRight;
		private bool _fpsStickBottom;
		private bool _cpsStickRight;
		private bool _cpsStickBottom;
		private bool _coordsStickRight;
		private bool _coordsStickBottom;
		private bool _timerStickRight;
		private bool _timerStickBottom;

		private float smoothedDelta = 0f;
		private float currentFps = 0f;

		private readonly List<float> clickTimes = new List<float>();
		private float currentCps = 0f;

		private bool cpsSplit = false;
		private KeyCode cpsKey1 = KeyCode.None;
		private KeyCode cpsKey2 = KeyCode.None;
		private readonly List<float> clickTimesA = new List<float>();
		private readonly List<float> clickTimesB = new List<float>();
		private float currentCpsA = 0f;
		private float currentCpsB = 0f;

		private GUIStyle hudStyle;
		private GUIStyle timerStyle;
		private bool stylesInitialized = false;
		private static GUIStyle _tempStyle;

		private static GUIStyle _tempStyle2;
		private Color _hudOutlineColor;

		private Anticheat anticheat;

		public bool ShowFps
		{
			get => showFps;
			set { if (showFps == value) return; showFps = value; prefShowFps.Value = value; prefs?.SaveToFile(false); }
		}

		public bool ShowCps
		{
			get => showCps;
			set { if (showCps == value) return; showCps = value; prefShowCps.Value = value; prefs?.SaveToFile(false); }
		}


		public bool FpsShowInMenu
		{
			get => showFpsInMenu;
			set { if (showFpsInMenu == value) return; showFpsInMenu = value; if (prefFpsShowInMenu != null) prefFpsShowInMenu.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CpsShowInMenu
		{
			get => showCpsInMenu;
			set { if (showCpsInMenu == value) return; showCpsInMenu = value; if (prefCpsShowInMenu != null) prefCpsShowInMenu.Value = value; prefs?.SaveToFile(false); }
		}

		public bool ShowCoords
		{
			get => showCoords;
			set { if (showCoords == value) return; showCoords = value; prefShowCoords.Value = value; prefs?.SaveToFile(false); }
		}

		public bool ShowEnrage
		{
			get => showEnrage;
			set { if (showEnrage == value) return; showEnrage = value; prefShowEnrage.Value = value; prefs?.SaveToFile(false); }
		}


		public bool ShowTimer
		{
			get => showTimer;
			set { if (showTimer == value) return; showTimer = value; prefShowTimer.Value = value; prefs?.SaveToFile(false); }
		}

		public float TimerHue
		{
			get => timerHue;
			set { if (Mathf.Approximately(timerHue, value)) return; timerHue = value; prefTimerHue.Value = value; prefs?.SaveToFile(false); }
		}

		public bool TimerShowLeadingMinutes
		{
			get => timerShowLeadingMinutes;
			set { if (timerShowLeadingMinutes == value) return; timerShowLeadingMinutes = value; prefTimerShowLeadingMinutes.Value = value; prefs?.SaveToFile(false); }
		}

		public bool TimerShowDecimals
		{
			get => timerShowDecimals;
			set { if (timerShowDecimals == value) return; timerShowDecimals = value; prefTimerShowDecimals.Value = value; prefs?.SaveToFile(false); }
		}

		public int TimerDecimalPlaces
		{
			get => timerDecimalPlaces;
			set { int v = Mathf.Clamp(value, 1, 3); if (timerDecimalPlaces == v) return; timerDecimalPlaces = v; prefTimerDecimalPlaces.Value = v; prefs?.SaveToFile(false); }
		}


		public bool ShowStunStatus { get => ShowEnrage; set => ShowEnrage = value; }

		public bool HideFpsPrefix
		{
			get => hideFpsPrefix;
			set { if (hideFpsPrefix == value) return; hideFpsPrefix = value; prefHideFpsPrefix.Value = value; prefs?.SaveToFile(false); }
		}

		public bool FpsUsePrefixFormat
		{
			get => fpsUsePrefixFormat;
			set { if (fpsUsePrefixFormat == value) return; fpsUsePrefixFormat = value; prefFpsUsePrefixFormat.Value = value; prefs?.SaveToFile(false); }
		}

		public bool FpsUppercaseLabel
		{
			get => fpsUppercaseLabel;
			set { if (fpsUppercaseLabel == value) return; fpsUppercaseLabel = value; prefFpsUppercaseLabel.Value = value; prefs?.SaveToFile(false); }
		}

		public bool HideCpsPrefix
		{
			get => hideCpsPrefix;
			set { if (hideCpsPrefix == value) return; hideCpsPrefix = value; prefHideCpsPrefix.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CpsUsePrefixFormat
		{
			get => cpsUsePrefixFormat;
			set { if (cpsUsePrefixFormat == value) return; cpsUsePrefixFormat = value; prefCpsUsePrefixFormat.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CpsUppercaseLabel
		{
			get => cpsUppercaseLabel;
			set { if (cpsUppercaseLabel == value) return; cpsUppercaseLabel = value; prefCpsUppercaseLabel.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CpsSplit
		{
			get => cpsSplit;
			set { if (cpsSplit == value) return; cpsSplit = value; prefCpsSplit.Value = value; prefs?.SaveToFile(false); }
		}

		public KeyCode CpsKey1
		{
			get => cpsKey1;
			set { if (cpsKey1 == value) return; cpsKey1 = value; prefCpsKey1.Value = value; prefs?.SaveToFile(false); }
		}

		public KeyCode CpsKey2
		{
			get => cpsKey2;
			set { if (cpsKey2 == value) return; cpsKey2 = value; prefCpsKey2.Value = value; prefs?.SaveToFile(false); }
		}


		public bool CoordsVertical
		{
			get => coordsVertical;
			set { if (coordsVertical == value) return; coordsVertical = value; prefCoordsVertical.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CoordsXYZPrefix
		{
			get => prefCoordsXYZPrefix != null ? prefCoordsXYZPrefix.Value : true;
			set { if (prefCoordsXYZPrefix == null) return; if (prefCoordsXYZPrefix.Value == value) return; prefCoordsXYZPrefix.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CoordsHidePrefix
		{
			get => prefCoordsRemovePrefix != null ? prefCoordsRemovePrefix.Value : false;
			set { if (prefCoordsRemovePrefix == null) return; if (prefCoordsRemovePrefix.Value == value) return; prefCoordsRemovePrefix.Value = value; prefs?.SaveToFile(false); }
		}

		public bool CoordsUppercaseLabel
		{
			get => coordsUppercaseLabel;
			set { if (coordsUppercaseLabel == value) return; coordsUppercaseLabel = value; if (prefCoordsUppercaseLabel != null) prefCoordsUppercaseLabel.Value = value; prefs?.SaveToFile(false); }
		}

		public bool HideStunBackground
		{
			get => hideStunBackground;
			set { if (hideStunBackground == value) return; hideStunBackground = value; prefHideStunBackground.Value = value; prefs?.SaveToFile(false); }
		}

		public float FpsHue
		{
			get => fpsHue;
			set { if (Mathf.Approximately(fpsHue, value)) return; fpsHue = Mathf.Clamp01(value); prefFpsHue.Value = fpsHue; prefs?.SaveToFile(false); }
		}

		public float CpsHue
		{
			get => cpsHue;
			set { if (Mathf.Approximately(cpsHue, value)) return; cpsHue = Mathf.Clamp01(value); prefCpsHue.Value = cpsHue; prefs?.SaveToFile(false); }
		}

		public float CoordsHue
		{
			get => coordsHue;
			set { if (Mathf.Approximately(coordsHue, value)) return; coordsHue = Mathf.Clamp01(value); prefCoordsHue.Value = coordsHue; prefs?.SaveToFile(false); }
		}


		public int HudTextStyleMode
		{
			get => hudTextStyleMode;
			set
			{
				if (value < 0) value = 0;
				if (value > 1) value = 1;
				if (hudTextStyleMode == value) return;
				hudTextStyleMode = value;
				if (prefHudTextStyleMode != null) prefHudTextStyleMode.Value = value;
			}
		}

		public float HudScale
		{
			get => hudScale;
			set
			{
				float v = Mathf.Clamp(value, 1f, 2f);
				if (Mathf.Approximately(hudScale, v)) return;
				hudScale = v;
				GlobalHudScale = v;
				lastAppliedScale = -1f;
				if (prefHudScale != null) prefHudScale.Value = v;
				prefs?.SaveToFile(false);
			}
		}

		public void Initialize(MelonPreferences_Category prefsCategory, Anticheat anticheatRef)
		{

			if (prefsCategory == null)
			{
				MelonLogger.Warning("HudManager.Initialize: prefsCategory was null, creating DevourCore category as fallback.");
				prefsCategory = MelonPreferences.CreateCategory("DevourCore");
			}

			prefs = prefsCategory;
			anticheat = anticheatRef;
			if (prefs == null)
			{
				MelonLogger.Error("HudManager.Initialize: prefs is still null after fallback. HUD will be disabled.");
				return;
			}

			prefShowFps = prefs.CreateEntry("HudShowFps", true);
			prefShowCps = prefs.CreateEntry("HudShowCps", true);
			prefFpsShowInMenu = prefs.CreateEntry("HudFpsShowInMenu", true);
			prefCpsShowInMenu = prefs.CreateEntry("HudCpsShowInMenu", true);
			prefShowCoords = prefs.CreateEntry("HudShowCoords", false);
			prefShowEnrage = prefs.CreateEntry("HudShowEnrage", true);

			prefHideFpsPrefix = prefs.CreateEntry("HudHideFpsPrefix", false);

			prefFpsUsePrefixFormat = prefs.CreateEntry("HudFpsUsePrefixFormat", true);
			prefFpsUppercaseLabel = prefs.CreateEntry("HudFpsUppercaseLabel", true);
			prefHideCpsPrefix = prefs.CreateEntry("HudHideCpsPrefix", false);
			prefCpsUsePrefixFormat = prefs.CreateEntry("HudCpsUsePrefixFormat", true);
			prefCpsUppercaseLabel = prefs.CreateEntry("HudCpsUppercaseLabel", true);
			prefCoordsVertical = prefs.CreateEntry("HudCoordsVertical", false);
			prefCoordsXYZPrefix = prefs.CreateEntry("HudCoordsXYZPrefix", true);
			prefCoordsRemovePrefix = prefs.CreateEntry("HudCoordsRemovePrefix", false);
			prefCoordsUppercaseLabel = prefs.CreateEntry("HudCoordsUppercaseLabel", true);
			prefHideStunBackground = prefs.CreateEntry("HudHideStunBackground", false);

			prefFpsHue = prefs.CreateEntry("HudFpsHue", 0f);
			prefCpsHue = prefs.CreateEntry("HudCpsHue", 0f);
			prefCpsSplit = prefs.CreateEntry("HudCpsSplit", true);
			prefCpsKey1 = prefs.CreateEntry("HudCpsKey1", KeyCode.Mouse0);
			prefCpsKey2 = prefs.CreateEntry("HudCpsKey2", KeyCode.Mouse1);
			prefCoordsHue = prefs.CreateEntry("HudCoordsHue", 0f);


			prefFpsOpacity = prefs.CreateEntry("HudFpsOpacity", 1f);
			prefFpsChroma = prefs.CreateEntry("HudFpsChroma", false);
			prefCpsOpacity = prefs.CreateEntry("HudCpsOpacity", 1f);
			prefCpsChroma = prefs.CreateEntry("HudCpsChroma", false);
			prefCoordsOpacity = prefs.CreateEntry("HudCoordsOpacity", 1f);
			prefCoordsChroma = prefs.CreateEntry("HudCoordsChroma", false);
			prefTimerOpacity = prefs.CreateEntry("HudTimerOpacity", 1f);
			prefTimerChroma = prefs.CreateEntry("HudTimerChroma", false);

			prefHudScale = prefs.CreateEntry("HudScale", 1.1188235f);
			prefHudTextStyleMode = prefs.CreateEntry("HudTextStyleMode", 1);

			prefShowTimer = prefs.CreateEntry("HudShowTimer", true);
			prefTimerHue = prefs.CreateEntry("HudTimerHue", 0f);
			prefTimerShowDecimals = prefs.CreateEntry("HudTimerShowDecimals", false);
			prefTimerDecimalPlaces = prefs.CreateEntry("HudTimerDecimalPlaces", 1);
			prefTimerShowLeadingMinutes = prefs.CreateEntry("HudTimerShowLeadingMinutes", false);

			showFps = prefShowFps.Value;
			showCps = prefShowCps.Value;
			showFpsInMenu = prefFpsShowInMenu != null ? prefFpsShowInMenu.Value : true;
			showCpsInMenu = prefCpsShowInMenu != null ? prefCpsShowInMenu.Value : true;
			showCoords = prefShowCoords.Value;
			showEnrage = prefShowEnrage.Value;
			showTimer = prefShowTimer.Value;

			hideFpsPrefix = prefHideFpsPrefix.Value;
			fpsUsePrefixFormat = prefFpsUsePrefixFormat.Value;
			fpsUppercaseLabel = prefFpsUppercaseLabel.Value;
			hideCpsPrefix = prefHideCpsPrefix.Value;
			cpsUsePrefixFormat = prefCpsUsePrefixFormat.Value;
			cpsUppercaseLabel = prefCpsUppercaseLabel.Value;
			cpsSplit = prefCpsSplit.Value;
			cpsKey1 = prefCpsKey1.Value;
			cpsKey2 = prefCpsKey2.Value;
			coordsVertical = prefCoordsVertical.Value;
			coordsUppercaseLabel = prefCoordsUppercaseLabel.Value;
			hideStunBackground = prefHideStunBackground.Value;

			fpsHue = Mathf.Clamp01(prefFpsHue.Value);
			cpsHue = Mathf.Clamp01(prefCpsHue.Value);
			coordsHue = Mathf.Clamp01(prefCoordsHue.Value);
			timerHue = Mathf.Clamp01(prefTimerHue.Value);

			fpsOpacity = prefFpsOpacity != null ? Mathf.Clamp01(prefFpsOpacity.Value) : 1f;
			fpsChroma = prefFpsChroma != null && prefFpsChroma.Value;
			cpsOpacity = prefCpsOpacity != null ? Mathf.Clamp01(prefCpsOpacity.Value) : 1f;
			cpsChroma = prefCpsChroma != null && prefCpsChroma.Value;
			coordsOpacity = prefCoordsOpacity != null ? Mathf.Clamp01(prefCoordsOpacity.Value) : 1f;
			coordsChroma = prefCoordsChroma != null && prefCoordsChroma.Value;
			timerOpacity = prefTimerOpacity != null ? Mathf.Clamp01(prefTimerOpacity.Value) : 1f;
			timerChroma = prefTimerChroma != null && prefTimerChroma.Value;

			hudScale = Mathf.Clamp(prefHudScale.Value, 1f, 1.4f);
			hudTextStyleMode = prefHudTextStyleMode != null ? prefHudTextStyleMode.Value : 0;
			GlobalHudScale = hudScale;

			prefFpsPosX = prefs.CreateEntry("HudFpsPosX", 0f);
			prefFpsPosY = prefs.CreateEntry("HudFpsPosY", 0f);

			prefCpsPosX = prefs.CreateEntry("HudCpsPosX", 0f);
			prefCpsPosY = prefs.CreateEntry("HudCpsPosY", 15f);

			prefCoordsPosX = prefs.CreateEntry("HudCoordsPosX", 559f);
			prefCoordsPosY = prefs.CreateEntry("HudCoordsPosY", 0f);

			prefTimerPosX = prefs.CreateEntry("HudTimerPosX", 1908f);
			prefTimerPosY = prefs.CreateEntry("HudTimerPosY", 0f);

			fpsPosX = prefFpsPosX.Value;
			fpsPosY = prefFpsPosY.Value;
			cpsPosX = prefCpsPosX.Value;
			cpsPosY = prefCpsPosY.Value;
			coordsPosX = prefCoordsPosX.Value;
			coordsPosY = prefCoordsPosY.Value;

			timerPosX = prefTimerPosX.Value;
			timerPosY = prefTimerPosY.Value;

			_tempStyle = new GUIStyle();
			_hudOutlineColor = new Color(0f, 0f, 0f, 0.85f);

			ReloadFromPrefs();
		}


		public void ReloadFromPrefs()
		{
			if (prefs == null) return;

			if (prefShowFps != null) showFps = prefShowFps.Value;
			if (prefShowCps != null) showCps = prefShowCps.Value;
			if (prefFpsShowInMenu != null) showFpsInMenu = prefFpsShowInMenu.Value;
			if (prefCpsShowInMenu != null) showCpsInMenu = prefCpsShowInMenu.Value;
			if (prefFpsShowInMenu != null) showFpsInMenu = prefFpsShowInMenu.Value;
			if (prefCpsShowInMenu != null) showCpsInMenu = prefCpsShowInMenu.Value;
			if (prefShowCoords != null) showCoords = prefShowCoords.Value;
			if (prefShowEnrage != null) showEnrage = prefShowEnrage.Value;
			if (prefShowTimer != null) showTimer = prefShowTimer.Value;

			if (prefHideFpsPrefix != null) hideFpsPrefix = prefHideFpsPrefix.Value;
			if (prefFpsUsePrefixFormat != null) fpsUsePrefixFormat = prefFpsUsePrefixFormat.Value;
			if (prefFpsUppercaseLabel != null) fpsUppercaseLabel = prefFpsUppercaseLabel.Value;
			if (prefHideCpsPrefix != null) hideCpsPrefix = prefHideCpsPrefix.Value;
			if (prefCpsUsePrefixFormat != null) cpsUsePrefixFormat = prefCpsUsePrefixFormat.Value;
			if (prefCpsUppercaseLabel != null) cpsUppercaseLabel = prefCpsUppercaseLabel.Value;
			if (prefCoordsVertical != null) coordsVertical = prefCoordsVertical.Value;
			if (prefCoordsUppercaseLabel != null) coordsUppercaseLabel = prefCoordsUppercaseLabel.Value;
			if (prefHideStunBackground != null) hideStunBackground = prefHideStunBackground.Value;

			if (prefFpsHue != null) fpsHue = Mathf.Clamp01(prefFpsHue.Value);
			if (prefCpsHue != null) cpsHue = Mathf.Clamp01(prefCpsHue.Value);
			if (prefCoordsHue != null) coordsHue = Mathf.Clamp01(prefCoordsHue.Value);
			timerHue = Mathf.Clamp01(prefTimerHue.Value);

			if (prefCpsSplit != null) cpsSplit = prefCpsSplit.Value;
			if (prefCpsKey1 != null) cpsKey1 = prefCpsKey1.Value;
			if (prefCpsKey2 != null) cpsKey2 = prefCpsKey2.Value;

			if (prefFpsOpacity != null) fpsOpacity = Mathf.Clamp01(prefFpsOpacity.Value);
			if (prefFpsChroma != null) fpsChroma = prefFpsChroma.Value;
			if (prefCpsOpacity != null) cpsOpacity = Mathf.Clamp01(prefCpsOpacity.Value);
			if (prefCpsChroma != null) cpsChroma = prefCpsChroma.Value;
			if (prefCoordsOpacity != null) coordsOpacity = Mathf.Clamp01(prefCoordsOpacity.Value);
			if (prefCoordsChroma != null) coordsChroma = prefCoordsChroma.Value;
			if (prefTimerOpacity != null) timerOpacity = Mathf.Clamp01(prefTimerOpacity.Value);
			if (prefTimerChroma != null) timerChroma = prefTimerChroma.Value;

			if (prefHudScale != null) hudScale = Mathf.Clamp(prefHudScale.Value, 1f, 1.4f);
			if (prefHudTextStyleMode != null) hudTextStyleMode = prefHudTextStyleMode.Value;
			GlobalHudScale = hudScale;

			if (prefTimerHue != null) timerHue = Mathf.Clamp01(prefTimerHue.Value);
			if (prefTimerShowDecimals != null) timerShowDecimals = prefTimerShowDecimals.Value;
			if (prefTimerDecimalPlaces != null) timerDecimalPlaces = prefTimerDecimalPlaces.Value;
			if (prefTimerShowLeadingMinutes != null) timerShowLeadingMinutes = prefTimerShowLeadingMinutes.Value;

			if (prefFpsPosX != null) fpsPosX = prefFpsPosX.Value;
			if (prefFpsPosY != null) fpsPosY = prefFpsPosY.Value;
			if (prefCpsPosX != null) cpsPosX = prefCpsPosX.Value;
			if (prefCpsPosY != null) cpsPosY = prefCpsPosY.Value;
			if (prefCoordsPosX != null) coordsPosX = prefCoordsPosX.Value;
			if (prefCoordsPosY != null) coordsPosY = prefCoordsPosY.Value;
			if (prefTimerPosX != null) timerPosX = prefTimerPosX.Value;
			if (prefTimerPosY != null) timerPosY = prefTimerPosY.Value;

			stylesInitialized = false;
			lastAppliedScale = -1f;
		}


		private void InitStyles()
		{
			int fs = Mathf.RoundToInt(13f * hudScale);

			if (!stylesInitialized)
			{
				hudStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = fs,
					fontStyle = FontStyle.Bold,
					alignment = TextAnchor.UpperLeft,
					richText = true,
					clipping = TextClipping.Overflow,
					padding = new RectOffset(2, 2, 0, 0)
				};
				hudStyle.normal.textColor = Color.white;

				timerStyle = new GUIStyle(hudStyle)
				{
					alignment = TextAnchor.UpperCenter
				};

				stylesInitialized = true;
			}

			if (!Mathf.Approximately(lastAppliedScale, hudScale))
			{
				if (hudStyle != null) hudStyle.fontSize = fs;
				if (timerStyle != null) timerStyle.fontSize = fs;
				lastAppliedScale = hudScale;
			}
		}



		private void CalcHudBox(string text, float s, float lineHeight, GUIStyle style, out float width, out float height)
		{
			if (style == null) style = GUI.skin.label;


			int lines = 1;
			if (!string.IsNullOrEmpty(text))
				lines = text.IndexOf('\n') >= 0 ? text.Split('\n').Length : 1;



			float extraGap = 3f * s;
			height = lineHeight * lines + extraGap * Mathf.Max(0, lines - 1);


			float maxW = 0f;
			if (lines > 1)
			{
				string[] parts = text.Split('\n');
				for (int i = 0; i < parts.Length; i++)
				{
					Vector2 sz = style.CalcSize(new GUIContent(parts[i]));
					if (sz.x > maxW) maxW = sz.x;
				}
			}
			else
			{
				Vector2 sz = style.CalcSize(new GUIContent(text ?? ""));
				maxW = sz.x;
			}



			width = Mathf.Clamp(maxW, 4f * s, Screen.width);
		}

		private float GetChromaHue(float baseHue)
		{
			float h = baseHue;
			if (h < 0.0001f) h = 0f;
			h += Time.time * 0.12f;
			h = h - Mathf.Floor(h);
			return h;
		}

		private Color ApplyOpacity(Color c, float opacity)
		{
			c.a = Mathf.Clamp01(opacity);
			return c;
		}

		private Color HueToColor(float h)
		{
			if (h <= 0.0001f) return Color.white;

			Color c = Color.HSVToRGB(h, 0.90f, 1f);
			return Color.Lerp(Color.white, c, 0.80f);
		}


		private Color HueToColorFps(float h)
		{
			if (h <= 0.0001f) return Color.white;

			float strength = 0.85f;
			Color c = Color.HSVToRGB(h, 0.90f, 1f);
			return Color.Lerp(Color.white, c, strength);
		}

		public void NotifyTimerSceneLoaded(bool inValidMap)
		{
			timerRunning = inValidMap;
			if (inValidMap) timerStartTime = Time.time;
		}

		public void OnUpdate()
		{
			float dt = Time.unscaledDeltaTime;
			if (dt > 0f)
			{
				if (smoothedDelta <= 0f) smoothedDelta = dt;
				else smoothedDelta += (dt - smoothedDelta) * 0.1f;
				currentFps = 1f / smoothedDelta;
			}

			if (!cpsSplit)
			{

				if (cpsKey1 != KeyCode.None)
				{
					if (Input.GetKeyDown(cpsKey1)) clickTimes.Add(Time.time);
				}
				else
				{
					if (Input.GetMouseButtonDown(0)) clickTimes.Add(Time.time);
				}
				float cutoff = Time.time - 1f;
				int i = 0;
				while (i < clickTimes.Count)
				{
					if (clickTimes[i] < cutoff) clickTimes.RemoveAt(i);
					else i++;
				}
				currentCps = clickTimes.Count;
			}
			else
			{

				if (cpsKey1 != KeyCode.None && Input.GetKeyDown(cpsKey1)) clickTimesA.Add(Time.time);
				if (cpsKey2 != KeyCode.None && Input.GetKeyDown(cpsKey2)) clickTimesB.Add(Time.time);

				float cutoff = Time.time - 1f;

				int i = 0;
				while (i < clickTimesA.Count)
				{
					if (clickTimesA[i] < cutoff) clickTimesA.RemoveAt(i);
					else i++;
				}

				i = 0;
				while (i < clickTimesB.Count)
				{
					if (clickTimesB[i] < cutoff) clickTimesB.RemoveAt(i);
					else i++;
				}

				currentCpsA = clickTimesA.Count;
				currentCpsB = clickTimesB.Count;
			}

			bool editing = anticheat != null && anticheat.IsEditingPosition;
			if (wasEditing && !editing && prefs != null)
				prefs.SaveToFile(false);
			wasEditing = editing;
		}

		public void OnGUI()
		{
			bool editing = anticheat != null && anticheat.IsEditingPosition;
			if (!editing && !showFps && !showCps && !showCoords) return;

			InitStyles();

			float s = hudScale;
			float baseWidth = 260f * s;


			float lineHeight = 18f * s;
			try
			{
				GUIStyle st = hudStyle ?? GUI.skin.label;
				lineHeight = st.CalcSize(new GUIContent("Ag")).y * s;
			}
			catch { /* ignore */ }

			bool inMenu = SceneManager.GetActiveScene().name == "Menu";

			bool drawFps = editing || (showFps && (!inMenu || showFpsInMenu));
			bool drawCps = editing || (showCps && (!inMenu || showCpsInMenu));
			bool drawCoords = editing || showCoords;
			bool drawTimer = editing || (showTimer && timerRunning);

			if (drawFps)
			{
				string label = fpsUppercaseLabel ? Loc.GUI.FPS_Upper : Loc.GUI.FPS_Lower;
				string text;
				if (hideFpsPrefix) text = $"{currentFps:0}";
				else if (fpsUsePrefixFormat) text = $"{label}: {currentFps:0}";
				else text = $"{currentFps:0} {label}";
				float fpsW, fpsH;
				CalcHudBox(text, s, lineHeight, hudStyle, out fpsW, out fpsH);
				AutoClamp(ref fpsPosX, ref fpsPosY, fpsW, fpsH, ref _fpsStickRight, ref _fpsStickBottom, dragFps, prefFpsPosX, prefFpsPosY);
				Rect r = new Rect(fpsPosX, fpsPosY, fpsW, fpsH);
				HandleDrag(ref fpsPosX, ref fpsPosY, ref dragFps, ref dragOffsetFps, r, fpsW, fpsH, editing, prefFpsPosX, prefFpsPosY);
				float hue = fpsChroma ? GetChromaHue(fpsHue) : fpsHue;
				Color c = ApplyOpacity(HueToColorFps(hue), fpsOpacity);
				DrawHudText(new Rect(fpsPosX, fpsPosY, fpsW, fpsH), text, c);
			}

			if (drawCps)
			{
				string label = cpsUppercaseLabel ? Loc.GUI.Hud_CpsUpper : Loc.GUI.Hud_CpsLower;
				string text;

				if (!cpsSplit)
				{
					if (hideCpsPrefix) text = $"{currentCps:0}";
					else if (cpsUsePrefixFormat) text = $"{label}: {currentCps:0}";
					else text = $"{currentCps:0} {label}";
				}
				else
				{
					string val = $"{currentCpsA:0} / {currentCpsB:0}";
					if (hideCpsPrefix) text = val;
					else if (cpsUsePrefixFormat) text = $"{label}: {val}";
					else text = $"{val} {label}";
				}

				float cpsW, cpsH;
				CalcHudBox(text, s, lineHeight, hudStyle, out cpsW, out cpsH);
				AutoClamp(ref cpsPosX, ref cpsPosY, cpsW, cpsH, ref _cpsStickRight, ref _cpsStickBottom, dragCps, prefCpsPosX, prefCpsPosY);
				Rect r = new Rect(cpsPosX, cpsPosY, cpsW, cpsH);
				HandleDrag(ref cpsPosX, ref cpsPosY, ref dragCps, ref dragOffsetCps, r, cpsW, cpsH, editing, prefCpsPosX, prefCpsPosY);
				float hue = cpsChroma ? GetChromaHue(cpsHue) : cpsHue;
				Color c = ApplyOpacity(HueToColorFps(hue), cpsOpacity);
				DrawHudText(new Rect(cpsPosX, cpsPosY, cpsW, cpsH), text, c);
			}

			if (drawCoords)
			{
				Vector3 pos = Vector3.zero;
				var cam = Camera.main;
				if (cam != null) pos = cam.transform.position;

				bool showPrefix = prefCoordsXYZPrefix != null ? prefCoordsXYZPrefix.Value : true;
				bool hidePrefix = prefCoordsRemovePrefix != null ? prefCoordsRemovePrefix.Value : false;
				bool upper = coordsUppercaseLabel;

				if (hidePrefix) showPrefix = false;

				string lx = upper ? Loc.GUI.AxisXUpper : Loc.GUI.AxisXLower;
				string ly = upper ? Loc.GUI.AxisYUpper : Loc.GUI.AxisYLower;
				string lz = upper ? Loc.GUI.AxisZUpper : Loc.GUI.AxisZLower;

				string text;
				float width, height;

				if (!coordsVertical)
				{
					text = showPrefix
						? $"{lx}: {pos.x:0.0}, {ly}: {pos.y:0.0}, {lz}: {pos.z:0.0}"
						: $"{pos.x:0.0}, {pos.y:0.0}, {pos.z:0.0}";

					width = (showPrefix ? 420f : 340f) * s;
					height = lineHeight;
				}
				else
				{
					text = showPrefix
						? $"{lx}: {pos.x:0.0}\n{ly}: {pos.y:0.0}\n{lz}: {pos.z:0.0}"
						: $"{pos.x:0.0}\n{pos.y:0.0}\n{pos.z:0.0}";

					width = (showPrefix ? 160f : 120f) * s;
					height = lineHeight * 3f + 6f * s;
				}

				CalcHudBox(text, s, lineHeight, hudStyle, out width, out height);
				AutoClamp(ref coordsPosX, ref coordsPosY, width, height, ref _coordsStickRight, ref _coordsStickBottom, dragCoords, prefCoordsPosX, prefCoordsPosY);

				Rect r = new Rect(coordsPosX, coordsPosY, width, height);
				HandleDrag(ref coordsPosX, ref coordsPosY, ref dragCoords, ref dragOffsetCoords, r, width, height, editing, prefCoordsPosX, prefCoordsPosY);
				float hue = coordsChroma ? GetChromaHue(coordsHue) : coordsHue;
				Color c = ApplyOpacity(HueToColor(hue), coordsOpacity);
				DrawHudText(new Rect(coordsPosX, coordsPosY, width, height), text, c);
			}



			if (drawTimer)
			{

				if (editing && !wasEditingTimer)
					previewTimerStartTime = Time.time;


				if (!editing)
					previewTimerStartTime = 0f;

				wasEditingTimer = editing;

				float elapsed;
				if (editing)
					elapsed = Time.time - previewTimerStartTime;
				else
					elapsed = timerRunning ? (Time.time - timerStartTime) : 0f;


				string t = FormatTimer(elapsed, timerShowDecimals, timerDecimalPlaces, timerShowLeadingMinutes);


				float timerWidth = baseWidth;
				try
				{
					GUIStyle st = timerStyle ?? hudStyle ?? GUI.skin.label;
					Vector2 sz = st.CalcSize(new GUIContent(t));

					timerWidth = Mathf.Clamp(sz.x, 1f, Screen.width);
				}
				catch {}



				if (!editing && !dragTimer && timerPosX >= 0f)
				{
					float gapRight = Screen.width - (timerPosX + timerWidth);
					if (Mathf.Abs(gapRight) <= 12f)
					{
						_timerStickRight = true;
						timerPosX = Screen.width - timerWidth;
						if (prefTimerPosX != null) prefTimerPosX.Value = timerPosX;
					}
				}

				AutoClamp(ref timerPosX, ref timerPosY, timerWidth, lineHeight, ref _timerStickRight, ref _timerStickBottom, dragTimer, prefTimerPosX, prefTimerPosY);

				float x = timerPosX;
				if (x < 0f) x = (Screen.width - timerWidth) * 0.5f;

				Rect r = new Rect(x, timerPosY, timerWidth, lineHeight);
				if (editing && timerPosX < 0f) { timerPosX = x; if (prefTimerPosX != null) prefTimerPosX.Value = timerPosX; }
				HandleDrag(ref timerPosX, ref timerPosY, ref dragTimer, ref dragOffsetTimer, r, timerWidth, lineHeight, editing, prefTimerPosX, prefTimerPosY);

				x = timerPosX;
				if (x < 0f) x = (Screen.width - timerWidth) * 0.5f;
				r = new Rect(x, timerPosY, timerWidth, lineHeight);

				float hue = timerChroma ? GetChromaHue(timerHue) : timerHue;
				Color textColor = ApplyOpacity(HueToColor(hue), timerOpacity);
				Color outline = _hudOutlineColor;
				outline.a *= textColor.a;
				DrawOutlinedText(r, t, timerStyle ?? hudStyle, textColor, outline, 1);
			}

		}



		private void AutoClamp(ref float posX, ref float posY, float width, float height,
			ref bool stickRight, ref bool stickBottom, bool isDragging,
			MelonPreferences_Entry<float> prefPosX, MelonPreferences_Entry<float> prefPosY)
		{



			const float edgeEpsilon = 1.5f;

			if (posX >= 0f)
			{
				float maxX = Mathf.Max(0f, Screen.width - width);


				if (!isDragging && stickRight)
					posX = maxX;
				else
					posX = Mathf.Clamp(posX, 0f, maxX);


				if (isDragging)
					stickRight = (maxX - posX) <= edgeEpsilon;
				else if ((maxX - posX) <= edgeEpsilon)
					stickRight = true;

				if (prefPosX != null) prefPosX.Value = posX;
			}

			if (posY >= 0f)
			{
				float maxY = Mathf.Max(0f, Screen.height - height);

				if (!isDragging && stickBottom)
					posY = maxY;
				else
					posY = Mathf.Clamp(posY, 0f, maxY);

				if (isDragging)
					stickBottom = (maxY - posY) <= edgeEpsilon;
				else if ((maxY - posY) <= edgeEpsilon)
					stickBottom = true;

				if (prefPosY != null) prefPosY.Value = posY;
			}
		}
		private void HandleDrag(ref float posX, ref float posY, ref bool isDragging, ref Vector2 dragOffset, Rect rect, float width, float height, bool editing, MelonPreferences_Entry<float> prefPosX, MelonPreferences_Entry<float> prefPosY)
		{
			if (!editing) { isDragging = false; return; }
			Event e = Event.current;
			int controlId = GUIUtility.GetControlID(FocusType.Passive);

			switch (e.GetTypeForControl(controlId))
			{
				case EventType.MouseDown:
					if (e.button == 0 && rect.Contains(e.mousePosition))
					{
						GUIUtility.hotControl = controlId;
						isDragging = true;
						dragOffset = e.mousePosition - new Vector2(posX, posY);
						e.Use();
					}
					break;
				case EventType.MouseDrag:
					if (isDragging && GUIUtility.hotControl == controlId && e.button == 0)
					{
						Vector2 newPos = e.mousePosition - dragOffset;
						posX = Mathf.Clamp(newPos.x, 0f, Mathf.Max(0f, Screen.width - width));
						posY = Mathf.Clamp(newPos.y, 0f, Mathf.Max(0f, Screen.height - height));
						if (prefPosX != null) prefPosX.Value = posX;
						if (prefPosY != null) prefPosY.Value = posY;
						e.Use();
					}
					break;
				case EventType.MouseUp:
					if (isDragging && GUIUtility.hotControl == controlId && e.button == 0)
					{
						isDragging = false;
						if (GUIUtility.hotControl == controlId) GUIUtility.hotControl = 0;
						e.Use();
					}
					break;
			}
		}

		private void DrawHudText(Rect rect, string text, Color color)
		{
			Color outline = _hudOutlineColor;
			outline.a *= color.a;
			DrawOutlinedText(rect, text, hudStyle, color, outline, 1);
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
			dst.imagePosition = src.imagePosition;
			dst.fixedWidth = src.fixedWidth;
			dst.fixedHeight = src.fixedHeight;
			dst.stretchWidth = src.stretchWidth;
			dst.stretchHeight = src.stretchHeight;
		}

		private void DrawOutlinedText(Rect rect, string text, GUIStyle style, Color textColor, Color outlineColor, int thickness)
		{
			if (Event.current.type != EventType.Repaint) return;

			if (style == null) style = GUI.skin.label;

			if (_tempStyle == null) _tempStyle = new GUIStyle(style);
			else CopyStyleBasics(style, _tempStyle);

			if (_tempStyle2 == null) _tempStyle2 = new GUIStyle(style);
			else CopyStyleBasics(style, _tempStyle2);

			if (hudTextStyleMode == 1)
			{
				_tempStyle.normal.textColor = outlineColor;
				Rect shadow = rect;
				shadow.x += thickness;
				shadow.y += thickness;
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

		private static string FormatTimer(float seconds, bool showDecimals, int decimalPlaces, bool showLeadingMinutes)
		{
			if (seconds < 0f) seconds = 0f;

			int mins = (int)(seconds / 60f);
			float rem = seconds - mins * 60f;

			int secsInt = (int)rem;
			float frac = rem - secsInt;

			int dp = showDecimals ? Mathf.Clamp(decimalPlaces, 1, 3) : 0;

			string fracStr = "";
			if (dp > 0)
			{
				int pow = (int)Mathf.Pow(10, dp);
				int dec = Mathf.FloorToInt(frac * pow + 1e-4f);
				fracStr = "." + dec.ToString().PadLeft(dp, '0');
			}

			if (!showLeadingMinutes && mins <= 0)
				return secsInt.ToString() + fracStr;

			string minStr = showLeadingMinutes ? mins.ToString("00") : mins.ToString();
			return $"{minStr}:{secsInt:00}{fracStr}";
		}




		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			if (prefsCategory == null) return;


			if (prefShowFps != null) prefShowFps.Value = true;
			if (prefShowCps != null) prefShowCps.Value = true;
			if (prefShowCoords != null) prefShowCoords.Value = false;
			if (prefShowEnrage != null) prefShowEnrage.Value = true;
			if (prefShowTimer != null) prefShowTimer.Value = true;


			if (prefHideFpsPrefix != null) prefHideFpsPrefix.Value = false;
			if (prefFpsUsePrefixFormat != null) prefFpsUsePrefixFormat.Value = true;
			if (prefFpsUppercaseLabel != null) prefFpsUppercaseLabel.Value = true;
			if (prefHideCpsPrefix != null) prefHideCpsPrefix.Value = false;
			if (prefCpsUsePrefixFormat != null) prefCpsUsePrefixFormat.Value = true;
			if (prefCpsUppercaseLabel != null) prefCpsUppercaseLabel.Value = true;
			if (prefCoordsVertical != null) prefCoordsVertical.Value = false;
			if (prefCoordsXYZPrefix != null) prefCoordsXYZPrefix.Value = true;
			if (prefCoordsRemovePrefix != null) prefCoordsRemovePrefix.Value = false;
			if (prefCoordsUppercaseLabel != null) prefCoordsUppercaseLabel.Value = true;
			if (prefHideStunBackground != null) prefHideStunBackground.Value = false;


			if (prefFpsHue != null) prefFpsHue.Value = 0f;
			if (prefCpsHue != null) prefCpsHue.Value = 0f;
			if (prefCoordsHue != null) prefCoordsHue.Value = 0f;
			if (prefCpsSplit != null) prefCpsSplit.Value = true;
			if (prefCpsKey1 != null) prefCpsKey1.Value = KeyCode.Mouse0;
			if (prefCpsKey2 != null) prefCpsKey2.Value = KeyCode.Mouse1;
			if (prefFpsOpacity != null) prefFpsOpacity.Value = 1f;
			if (prefFpsChroma != null) prefFpsChroma.Value = false;
			if (prefCpsOpacity != null) prefCpsOpacity.Value = 1f;
			if (prefCpsChroma != null) prefCpsChroma.Value = false;
			if (prefCoordsOpacity != null) prefCoordsOpacity.Value = 1f;
			if (prefCoordsChroma != null) prefCoordsChroma.Value = false;
			if (prefTimerOpacity != null) prefTimerOpacity.Value = 1f;
			if (prefTimerChroma != null) prefTimerChroma.Value = false;

			if (prefHudScale != null) prefHudScale.Value = 1.1188235f;
			if (prefHudTextStyleMode != null) prefHudTextStyleMode.Value = 1;


			if (prefTimerHue != null) prefTimerHue.Value = 0f;
			if (prefTimerShowDecimals != null) prefTimerShowDecimals.Value = false;
			if (prefTimerDecimalPlaces != null) prefTimerDecimalPlaces.Value = 1;
			if (prefTimerShowLeadingMinutes != null) prefTimerShowLeadingMinutes.Value = false;


			if (prefFpsPosX != null) prefFpsPosX.Value = 0f;
			if (prefFpsPosY != null) prefFpsPosY.Value = 0f;
			if (prefCpsPosX != null) prefCpsPosX.Value = 0f;
			if (prefCpsPosY != null) prefCpsPosY.Value = 15f;
			if (prefCoordsPosX != null) prefCoordsPosX.Value = 559f;
			if (prefCoordsPosY != null) prefCoordsPosY.Value = 0f;
			if (prefTimerPosX != null) prefTimerPosX.Value = 1887f;
			if (prefTimerPosY != null) prefTimerPosY.Value = 0f;

			prefsCategory.SaveToFile(false);
		}






		public void ResetAllHudPositionsToDefaults()
		{

			const float DEFAULT_FPS_X = 0f;
			const float DEFAULT_FPS_Y = 0f;
			const float DEFAULT_CPS_X = 0f;
			const float DEFAULT_CPS_Y = 15f;
			const float DEFAULT_COORDS_X = 559f;
			const float DEFAULT_COORDS_Y = 0f;
			const float DEFAULT_TIMER_X = 1908f;
			const float DEFAULT_TIMER_Y = 0f;

			fpsPosX = DEFAULT_FPS_X;
			fpsPosY = DEFAULT_FPS_Y;
			cpsPosX = DEFAULT_CPS_X;
			cpsPosY = DEFAULT_CPS_Y;
			coordsPosX = DEFAULT_COORDS_X;
			coordsPosY = DEFAULT_COORDS_Y;
			timerPosX = DEFAULT_TIMER_X;
			timerPosY = DEFAULT_TIMER_Y;

			if (prefFpsPosX != null) prefFpsPosX.Value = fpsPosX;
			if (prefFpsPosY != null) prefFpsPosY.Value = fpsPosY;
			if (prefCpsPosX != null) prefCpsPosX.Value = cpsPosX;
			if (prefCpsPosY != null) prefCpsPosY.Value = cpsPosY;
			if (prefCoordsPosX != null) prefCoordsPosX.Value = coordsPosX;
			if (prefCoordsPosY != null) prefCoordsPosY.Value = coordsPosY;
			if (prefTimerPosX != null) prefTimerPosX.Value = timerPosX;
			if (prefTimerPosY != null) prefTimerPosY.Value = timerPosY;

			prefs?.SaveToFile(false);
		}
	}
}