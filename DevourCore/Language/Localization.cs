using MelonLoader;

namespace DevourCore
{
	public enum Language
	{
		EN = 0,
		CN = 1,
		PT = 2
	}

	public static class Loc
	{
		private static Language _current = Language.EN;
		private static MelonPreferences_Entry<int> _prefLang;

		public static Language Current
		{
			get => _current;
			set
			{
				_current = value;
				if (_prefLang != null)
				{
					_prefLang.Value = (int)_current;
					_prefLang.Category.SaveToFile(false);
				}
			}
		}

		public static void Initialize(MelonPreferences_Category prefsCategory)
		{
			_prefLang = prefsCategory.CreateEntry("Language", (int)Language.EN);
			_current = (Language)_prefLang.Value;
		}

		private static string Pick(string en, string cn, string pt)
		{
			switch (Current)
			{
				case Language.CN: return cn;
				case Language.PT: return pt;
				default: return en;
			}
		}

		public static class GUI
		{
			public static string Tab_Optimize => Pick(EN.GUI.Tab_Optimize, CN.GUI.Tab_Optimize, PT.GUI.Tab_Optimize);
			public static string Tab_HSV => Pick(EN.GUI.Tab_HSV, CN.GUI.Tab_HSV, PT.GUI.Tab_HSV);
			public static string Tab_Speedrun => Pick(EN.GUI.Tab_Speedrun, CN.GUI.Tab_Speedrun, PT.GUI.Tab_Speedrun);
			public static string Tab_FOV => Pick(EN.GUI.Tab_FOV, CN.GUI.Tab_FOV, PT.GUI.Tab_FOV);
			public static string Tab_Anticheat => Pick(EN.GUI.Tab_Anticheat, CN.GUI.Tab_Anticheat, PT.GUI.Tab_Anticheat);
			public static string Tab_Menu => Pick(EN.GUI.Tab_Menu, CN.GUI.Tab_Menu, PT.GUI.Tab_Menu);
			public static string Tab_Settings => Pick(EN.GUI.Tab_Settings, CN.GUI.Tab_Settings, PT.GUI.Tab_Settings);

			public static string PressAnyKey => Pick(EN.GUI.PressAnyKey, CN.GUI.PressAnyKey, PT.GUI.PressAnyKey);
			public static string ToggleKeyFormat => Pick(EN.GUI.ToggleKeyFormat, CN.GUI.ToggleKeyFormat, PT.GUI.ToggleKeyFormat);
			public static string InteractKeyFormat => Pick(EN.GUI.InteractKeyFormat, CN.GUI.InteractKeyFormat, PT.GUI.InteractKeyFormat);
			public static string MenuKeyFormat => Pick(EN.GUI.MenuKeyFormat, CN.GUI.MenuKeyFormat, PT.GUI.MenuKeyFormat);
			public static string CurrentClipFormat => Pick(EN.GUI.CurrentClipFormat, CN.GUI.CurrentClipFormat, PT.GUI.CurrentClipFormat);
			public static string AreYouSure => Pick(EN.GUI.AreYouSure, CN.GUI.AreYouSure, PT.GUI.AreYouSure);
			public static string ResetClientSettings => Pick(EN.GUI.ResetClientSettings, CN.GUI.ResetClientSettings, PT.GUI.ResetClientSettings);

			public static string Header_Optimize => Pick(EN.GUI.Header_Optimize, CN.GUI.Header_Optimize, PT.GUI.Header_Optimize);
			public static string Desc_Optimize => Pick(EN.GUI.Desc_Optimize, CN.GUI.Desc_Optimize, PT.GUI.Desc_Optimize);
			public static string Header_Cull => Pick(EN.GUI.Header_Cull, CN.GUI.Header_Cull, PT.GUI.Header_Cull);
			public static string Toggle_CullEnabled => Pick(EN.GUI.Toggle_CullEnabled, CN.GUI.Toggle_CullEnabled, PT.GUI.Toggle_CullEnabled);
			public static string Toggle_CullInMenu => Pick(EN.GUI.Toggle_CullInMenu, CN.GUI.Toggle_CullInMenu, PT.GUI.Toggle_CullInMenu);
			public static string CullDistanceFormat => Pick(EN.GUI.CullDistanceFormat, CN.GUI.CullDistanceFormat, PT.GUI.CullDistanceFormat);
			public static string Header_Weather => Pick(EN.GUI.Header_Weather, CN.GUI.Header_Weather, PT.GUI.Header_Weather);
			public static string Toggle_DisableWeather => Pick(EN.GUI.Toggle_DisableWeather, CN.GUI.Toggle_DisableWeather, PT.GUI.Toggle_DisableWeather);
			public static string Toggle_MuteWeather => Pick(EN.GUI.Toggle_MuteWeather, CN.GUI.Toggle_MuteWeather, PT.GUI.Toggle_MuteWeather);

			public static string Header_OutfitColor => Pick(EN.GUI.Header_OutfitColor, CN.GUI.Header_OutfitColor, PT.GUI.Header_OutfitColor);
			public static string Header_IconColor => Pick(EN.GUI.Header_IconColor, CN.GUI.Header_IconColor, PT.GUI.Header_IconColor);
			public static string Desc_OutfitColor => Pick(EN.GUI.Desc_OutfitColor, CN.GUI.Desc_OutfitColor, PT.GUI.Desc_OutfitColor);
			public static string Desc_IconColor => Pick(EN.GUI.Desc_IconColor, CN.GUI.Desc_IconColor, PT.GUI.Desc_IconColor);
			public static string Toggle_IconEnabled => Pick(EN.GUI.Toggle_IconEnabled, CN.GUI.Toggle_IconEnabled, PT.GUI.Toggle_IconEnabled);
			public static string Toggle_OutfitEnabled => Pick(EN.GUI.Toggle_OutfitEnabled, CN.GUI.Toggle_OutfitEnabled, PT.GUI.Toggle_OutfitEnabled);
			public static string Swap_ToOutfit => Pick(EN.GUI.Swap_ToOutfit, CN.GUI.Swap_ToOutfit, PT.GUI.Swap_ToOutfit);
			public static string Swap_ToIcon => Pick(EN.GUI.Swap_ToIcon, CN.GUI.Swap_ToIcon, PT.GUI.Swap_ToIcon);
			public static string Header_Preview => Pick(EN.GUI.Header_Preview, CN.GUI.Header_Preview, PT.GUI.Header_Preview);

			public static string Header_Speedrun => Pick(EN.GUI.Header_Speedrun, CN.GUI.Header_Speedrun, PT.GUI.Header_Speedrun);
			public static string Desc_Speedrun => Pick(EN.GUI.Desc_Speedrun, CN.GUI.Desc_Speedrun, PT.GUI.Desc_Speedrun);
			public static string Toggle_InstantInteract => Pick(EN.GUI.Toggle_InstantInteract, CN.GUI.Toggle_InstantInteract, PT.GUI.Toggle_InstantInteract);
			public static string Toggle_AtticSpawn => Pick(EN.GUI.Toggle_AtticSpawn, CN.GUI.Toggle_AtticSpawn, PT.GUI.Toggle_AtticSpawn);
			public static string Header_AutoStart => Pick(EN.GUI.Header_AutoStart, CN.GUI.Header_AutoStart, PT.GUI.Header_AutoStart);
			public static string Desc_AutoStart => Pick(EN.GUI.Desc_AutoStart, CN.GUI.Desc_AutoStart, PT.GUI.Desc_AutoStart);
			public static string Toggle_ForceStart => Pick(EN.GUI.Toggle_ForceStart, CN.GUI.Toggle_ForceStart, PT.GUI.Toggle_ForceStart);
			public static string Toggle_UseArm => Pick(EN.GUI.Toggle_UseArm, CN.GUI.Toggle_UseArm, PT.GUI.Toggle_UseArm);
			public static string ForceStartDelayFormat => Pick(EN.GUI.ForceStartDelayFormat, CN.GUI.ForceStartDelayFormat, PT.GUI.ForceStartDelayFormat);
			public static string ForceStartArmFormat => Pick(EN.GUI.ForceStartArmFormat, CN.GUI.ForceStartArmFormat, PT.GUI.ForceStartArmFormat);
			public static string SpeedrunPopupBody => Pick(EN.GUI.SpeedrunPopupBody, CN.GUI.SpeedrunPopupBody, PT.GUI.SpeedrunPopupBody);
			public static string SpeedrunPopupConfirm => Pick(EN.GUI.SpeedrunPopupConfirm, CN.GUI.SpeedrunPopupConfirm, PT.GUI.SpeedrunPopupConfirm);

			public static string Header_FOV => Pick(EN.GUI.Header_FOV, CN.GUI.Header_FOV, PT.GUI.Header_FOV);
			public static string Desc_FOV => Pick(EN.GUI.Desc_FOV, CN.GUI.Desc_FOV, PT.GUI.Desc_FOV);
			public static string Toggle_FOVEnabled => Pick(EN.GUI.Toggle_FOVEnabled, CN.GUI.Toggle_FOVEnabled, PT.GUI.Toggle_FOVEnabled);
			public static string FOVValueFormat => Pick(EN.GUI.FOVValueFormat, CN.GUI.FOVValueFormat, PT.GUI.FOVValueFormat);

			public static string Header_Anticheat => Pick(EN.GUI.Header_Anticheat, CN.GUI.Header_Anticheat, PT.GUI.Header_Anticheat);
			public static string Desc_Anticheat => Pick(EN.GUI.Desc_Anticheat, CN.GUI.Desc_Anticheat, PT.GUI.Desc_Anticheat);
			public static string Toggle_AnticheatEnabled => Pick(EN.GUI.Toggle_AnticheatEnabled, CN.GUI.Toggle_AnticheatEnabled, PT.GUI.Toggle_AnticheatEnabled);
			public static string AlertDurationFormat => Pick(EN.GUI.AlertDurationFormat, CN.GUI.AlertDurationFormat, PT.GUI.AlertDurationFormat);
			public static string Button_ClearAlerts => Pick(EN.GUI.Button_ClearAlerts, CN.GUI.Button_ClearAlerts, PT.GUI.Button_ClearAlerts);
			public static string AnticheatStatusFormat => Pick(EN.GUI.AnticheatStatusFormat, CN.GUI.AnticheatStatusFormat, PT.GUI.AnticheatStatusFormat);
			public static string Button_SaveAlertPosition => Pick(EN.GUI.Button_SaveAlertPosition, CN.GUI.Button_SaveAlertPosition, PT.GUI.Button_SaveAlertPosition);
			public static string Button_EditAlertPosition => Pick(EN.GUI.Button_EditAlertPosition, CN.GUI.Button_EditAlertPosition, PT.GUI.Button_EditAlertPosition);
			public static string Button_ResetAlertPosition => Pick(EN.GUI.Button_ResetAlertPosition, CN.GUI.Button_ResetAlertPosition, PT.GUI.Button_ResetAlertPosition);

			public static string Header_Menu => Pick(EN.GUI.Header_Menu, CN.GUI.Header_Menu, PT.GUI.Header_Menu);
			public static string Desc_Menu => Pick(EN.GUI.Desc_Menu, CN.GUI.Desc_Menu, PT.GUI.Desc_Menu);
			public static string Header_MenuBackground => Pick(EN.GUI.Header_MenuBackground, CN.GUI.Header_MenuBackground, PT.GUI.Header_MenuBackground);
			public static string Toggle_CustomBackground => Pick(EN.GUI.Toggle_CustomBackground, CN.GUI.Toggle_CustomBackground, PT.GUI.Toggle_CustomBackground);
			public static string Header_MusicSettings => Pick(EN.GUI.Header_MusicSettings, CN.GUI.Header_MusicSettings, PT.GUI.Header_MusicSettings);
			public static string Toggle_DisableIngameMusic => Pick(EN.GUI.Toggle_DisableIngameMusic, CN.GUI.Toggle_DisableIngameMusic, PT.GUI.Toggle_DisableIngameMusic);
			public static string Toggle_MuteTunnel => Pick(EN.GUI.Toggle_MuteTunnel, CN.GUI.Toggle_MuteTunnel, PT.GUI.Toggle_MuteTunnel);
			public static string Toggle_RememberMusic => Pick(EN.GUI.Toggle_RememberMusic, CN.GUI.Toggle_RememberMusic, PT.GUI.Toggle_RememberMusic);

			public static string Header_Settings => Pick(EN.GUI.Header_Settings, CN.GUI.Header_Settings, PT.GUI.Header_Settings);
			public static string Desc_Settings => Pick(EN.GUI.Desc_Settings, CN.GUI.Desc_Settings, PT.GUI.Desc_Settings);
			public static string Header_Hotkeys => Pick(EN.GUI.Header_Hotkeys, CN.GUI.Header_Hotkeys, PT.GUI.Header_Hotkeys);
			public static string Header_ThemeColor => Pick(EN.GUI.Header_ThemeColor, CN.GUI.Header_ThemeColor, PT.GUI.Header_ThemeColor);
			public static string Desc_ThemeColor => Pick(EN.GUI.Desc_ThemeColor, CN.GUI.Desc_ThemeColor, PT.GUI.Desc_ThemeColor);
			public static string Header_Miscellaneous => Pick(EN.GUI.Header_Miscellaneous, CN.GUI.Header_Miscellaneous, PT.GUI.Header_Miscellaneous);

			public static string ThemeTabsHueFormat => Pick(EN.GUI.ThemeTabsHueFormat, CN.GUI.ThemeTabsHueFormat, PT.GUI.ThemeTabsHueFormat);
			public static string ThemeBackgroundHueFormat => Pick(EN.GUI.ThemeBackgroundHueFormat, CN.GUI.ThemeBackgroundHueFormat, PT.GUI.ThemeBackgroundHueFormat);
			public static string Toggle_DarkMode => Pick(EN.GUI.Toggle_DarkMode, CN.GUI.Toggle_DarkMode, PT.GUI.Toggle_DarkMode);
			public static string Toggle_NoBackground => Pick(EN.GUI.Toggle_NoBackground, CN.GUI.Toggle_NoBackground, PT.GUI.Toggle_NoBackground);
			public static string LanguageLabel => Pick(EN.GUI.LanguageLabel, CN.GUI.LanguageLabel, PT.GUI.LanguageLabel);

			public static string InfoOverlayTitle => Pick(EN.GUI.InfoOverlayTitle, CN.GUI.InfoOverlayTitle, PT.GUI.InfoOverlayTitle);

			public static string Header_VisibleCategories => Pick(EN.GUI.Header_VisibleCategories, CN.GUI.Header_VisibleCategories, PT.GUI.Header_VisibleCategories);
			public static string Desc_VisibleCategories => Pick(EN.GUI.Desc_VisibleCategories, CN.GUI.Desc_VisibleCategories, PT.GUI.Desc_VisibleCategories);

			public static string HueFormat => Pick(EN.GUI.HueFormat, CN.GUI.HueFormat, PT.GUI.HueFormat);
			public static string HudOpacityFormat => Pick(EN.GUI.HudOpacityFormat, CN.GUI.HudOpacityFormat, PT.GUI.HudOpacityFormat);
			public static string Toggle_HudChroma => Pick(EN.GUI.Toggle_HudChroma, CN.GUI.Toggle_HudChroma, PT.GUI.Toggle_HudChroma);
			public static string UnknownSymbol => Pick(EN.GUI.UnknownSymbol, CN.GUI.UnknownSymbol, PT.GUI.UnknownSymbol);

			public static string Tab_Gameplay => Pick(EN.GUI.Tab_Gameplay, CN.GUI.Tab_Gameplay, PT.GUI.Tab_Gameplay);
			public static string Tab_HUD => Pick(EN.GUI.Tab_HUD, CN.GUI.Tab_HUD, PT.GUI.Tab_HUD);

			public static string Slider_H => Pick(EN.GUI.Slider_H, CN.GUI.Slider_H, PT.GUI.Slider_H);
			public static string Slider_S => Pick(EN.GUI.Slider_S, CN.GUI.Slider_S, PT.GUI.Slider_S);
			public static string Slider_V => Pick(EN.GUI.Slider_V, CN.GUI.Slider_V, PT.GUI.Slider_V);

			public static string Header_LookBack => Pick(EN.GUI.Header_LookBack, CN.GUI.Header_LookBack, PT.GUI.Header_LookBack);
			public static string Toggle_EnableLookBack => Pick(EN.GUI.Toggle_EnableLookBack, CN.GUI.Toggle_EnableLookBack, PT.GUI.Toggle_EnableLookBack);
			public static string Toggle_LookBackToggleMode => Pick(EN.GUI.Toggle_LookBackToggleMode, CN.GUI.Toggle_LookBackToggleMode, PT.GUI.Toggle_LookBackToggleMode);
			public static string LookBackKeyFormat => Pick(EN.GUI.LookBackKeyFormat, CN.GUI.LookBackKeyFormat, PT.GUI.LookBackKeyFormat);
			public static string LookBackNotAvailable => Pick(EN.GUI.LookBackNotAvailable, CN.GUI.LookBackNotAvailable, PT.GUI.LookBackNotAvailable);

			public static string Header_Audio => Pick(EN.GUI.Header_Audio, CN.GUI.Header_Audio, PT.GUI.Header_Audio);
			public static string Toggle_MuteCarnivalClock => Pick(EN.GUI.Toggle_MuteCarnivalClock, CN.GUI.Toggle_MuteCarnivalClock, PT.GUI.Toggle_MuteCarnivalClock);

			public static string Header_HUD => Pick(EN.GUI.Header_HUD, CN.GUI.Header_HUD, PT.GUI.Header_HUD);
			public static string Button_Back => Pick(EN.GUI.Button_Back, CN.GUI.Button_Back, PT.GUI.Button_Back);
			public static string Hud_EnableFps => Pick(EN.GUI.Hud_EnableFps, CN.GUI.Hud_EnableFps, PT.GUI.Hud_EnableFps);
			public static string Hud_HidePrefix => Pick(EN.GUI.Hud_HidePrefix, CN.GUI.Hud_HidePrefix, PT.GUI.Hud_HidePrefix);
			public static string Hud_InvertPrefix => Pick(EN.GUI.Hud_InvertPrefix, CN.GUI.Hud_InvertPrefix, PT.GUI.Hud_InvertPrefix);
			public static string Hud_UppercasePrefix => Pick(EN.GUI.Hud_UppercasePrefix, CN.GUI.Hud_UppercasePrefix, PT.GUI.Hud_UppercasePrefix);
			public static string Hud_EnableCps => Pick(EN.GUI.Hud_EnableCps, CN.GUI.Hud_EnableCps, PT.GUI.Hud_EnableCps);
			public static string Hud_ShowInMenu => Pick(EN.GUI.Hud_ShowInMenu, CN.GUI.Hud_ShowInMenu, PT.GUI.Hud_ShowInMenu);
			public static string Hud_DualCps => Pick(EN.GUI.Hud_DualCps, CN.GUI.Hud_DualCps, PT.GUI.Hud_DualCps);
			public static string Hud_BindAFormat => Pick(EN.GUI.Hud_BindAFormat, CN.GUI.Hud_BindAFormat, PT.GUI.Hud_BindAFormat);
			public static string Hud_BindBFormat => Pick(EN.GUI.Hud_BindBFormat, CN.GUI.Hud_BindBFormat, PT.GUI.Hud_BindBFormat);
			public static string Hud_ShowCoordinates => Pick(EN.GUI.Hud_ShowCoordinates, CN.GUI.Hud_ShowCoordinates, PT.GUI.Hud_ShowCoordinates);
			public static string Hud_ShowPrefix => Pick(EN.GUI.Hud_ShowPrefix, CN.GUI.Hud_ShowPrefix, PT.GUI.Hud_ShowPrefix);
			public static string Hud_VerticalPosition => Pick(EN.GUI.Hud_VerticalPosition, CN.GUI.Hud_VerticalPosition, PT.GUI.Hud_VerticalPosition);
			public static string Hud_ShowEnrageStatus => Pick(EN.GUI.Hud_ShowEnrageStatus, CN.GUI.Hud_ShowEnrageStatus, PT.GUI.Hud_ShowEnrageStatus);
			public static string Hud_NoBackground => Pick(EN.GUI.Hud_NoBackground, CN.GUI.Hud_NoBackground, PT.GUI.Hud_NoBackground);
			public static string Hud_EnableSpeedDetector => Pick(EN.GUI.Hud_EnableSpeedDetector, CN.GUI.Hud_EnableSpeedDetector, PT.GUI.Hud_EnableSpeedDetector);
			public static string Hud_WarningDurationFormat => Pick(EN.GUI.Hud_WarningDurationFormat, CN.GUI.Hud_WarningDurationFormat, PT.GUI.Hud_WarningDurationFormat);
			public static string Hud_AlertsFormat => Pick(EN.GUI.Hud_AlertsFormat, CN.GUI.Hud_AlertsFormat, PT.GUI.Hud_AlertsFormat);
			public static string Hud_ClearAlerts => Pick(EN.GUI.Hud_ClearAlerts, CN.GUI.Hud_ClearAlerts, PT.GUI.Hud_ClearAlerts);
			public static string Hud_ClearAlertsCountFormat => Pick(EN.GUI.Hud_ClearAlertsCountFormat, CN.GUI.Hud_ClearAlertsCountFormat, PT.GUI.Hud_ClearAlertsCountFormat);
			public static string Hud_ShowGameTime => Pick(EN.GUI.Hud_ShowGameTime, CN.GUI.Hud_ShowGameTime, PT.GUI.Hud_ShowGameTime);
			public static string Hud_ShowLeadingMinutes => Pick(EN.GUI.Hud_ShowLeadingMinutes, CN.GUI.Hud_ShowLeadingMinutes, PT.GUI.Hud_ShowLeadingMinutes);
			public static string Hud_ShowDecimals => Pick(EN.GUI.Hud_ShowDecimals, CN.GUI.Hud_ShowDecimals, PT.GUI.Hud_ShowDecimals);
			public static string Hud_DecimalsAmountFormat => Pick(EN.GUI.Hud_DecimalsAmountFormat, CN.GUI.Hud_DecimalsAmountFormat, PT.GUI.Hud_DecimalsAmountFormat);
			public static string Hud_FpsCounter => Pick(EN.GUI.Hud_FpsCounter, CN.GUI.Hud_FpsCounter, PT.GUI.Hud_FpsCounter);
			public static string Hud_CpsCounter => Pick(EN.GUI.Hud_CpsCounter, CN.GUI.Hud_CpsCounter, PT.GUI.Hud_CpsCounter);
			public static string Hud_Coordinates => Pick(EN.GUI.Hud_Coordinates, CN.GUI.Hud_Coordinates, PT.GUI.Hud_Coordinates);
			public static string Hud_EnrageStatus => Pick(EN.GUI.Hud_EnrageStatus, CN.GUI.Hud_EnrageStatus, PT.GUI.Hud_EnrageStatus);
			public static string Hud_SpeedDetector => Pick(EN.GUI.Hud_SpeedDetector, CN.GUI.Hud_SpeedDetector, PT.GUI.Hud_SpeedDetector);
			public static string Hud_GameTime => Pick(EN.GUI.Hud_GameTime, CN.GUI.Hud_GameTime, PT.GUI.Hud_GameTime);
			public static string Hud_Size => Pick(EN.GUI.Hud_Size, CN.GUI.Hud_Size, PT.GUI.Hud_Size);
			public static string Hud_SavePositions => Pick(EN.GUI.Hud_SavePositions, CN.GUI.Hud_SavePositions, PT.GUI.Hud_SavePositions);
			public static string Hud_EditPositions => Pick(EN.GUI.Hud_EditPositions, CN.GUI.Hud_EditPositions, PT.GUI.Hud_EditPositions);
			public static string Hud_ResetPositions => Pick(EN.GUI.Hud_ResetPositions, CN.GUI.Hud_ResetPositions, PT.GUI.Hud_ResetPositions);

			public static string Header_Appearence => Pick(EN.GUI.Header_Appearence, CN.GUI.Header_Appearence, PT.GUI.Header_Appearence);
			public static string BackgroundOpacityFormat => Pick(EN.GUI.BackgroundOpacityFormat, CN.GUI.BackgroundOpacityFormat, PT.GUI.BackgroundOpacityFormat);
			public static string Toggle_DefaultBackground => Pick(EN.GUI.Toggle_DefaultBackground, CN.GUI.Toggle_DefaultBackground, PT.GUI.Toggle_DefaultBackground);
			public static string Toggle_ChromaMode => Pick(EN.GUI.Toggle_ChromaMode, CN.GUI.Toggle_ChromaMode, PT.GUI.Toggle_ChromaMode);
			public static string Button_CustomizeAppearence => Pick(EN.GUI.Button_CustomizeAppearence, CN.GUI.Button_CustomizeAppearence, PT.GUI.Button_CustomizeAppearence);
			public static string Toggle_ClickSounds => Pick(EN.GUI.Toggle_ClickSounds, CN.GUI.Toggle_ClickSounds, PT.GUI.Toggle_ClickSounds);
			public static string Toggle_ButtonShadow => Pick(EN.GUI.Toggle_ButtonShadow, CN.GUI.Toggle_ButtonShadow, PT.GUI.Toggle_ButtonShadow);
			public static string Button_HudTextStyleFormat => Pick(EN.GUI.Button_HudTextStyleFormat, CN.GUI.Button_HudTextStyleFormat, PT.GUI.Button_HudTextStyleFormat);
			public static string Button_UiTextStyleFormat => Pick(EN.GUI.Button_UiTextStyleFormat, CN.GUI.Button_UiTextStyleFormat, PT.GUI.Button_UiTextStyleFormat);
			public static string TextStyle_Outline => Pick(EN.GUI.TextStyle_Outline, CN.GUI.TextStyle_Outline, PT.GUI.TextStyle_Outline);
			public static string TextStyle_Shadow => Pick(EN.GUI.TextStyle_Shadow, CN.GUI.TextStyle_Shadow, PT.GUI.TextStyle_Shadow);

			public static string Stun_Calm => Pick(EN.GUI.Stun_Calm, CN.GUI.Stun_Calm, PT.GUI.Stun_Calm);
			public static string Stun_Enraged => Pick(EN.GUI.Stun_Enraged, CN.GUI.Stun_Enraged, PT.GUI.Stun_Enraged);
			public static string Stun_RedEyes => Pick(EN.GUI.Stun_RedEyes, CN.GUI.Stun_RedEyes, PT.GUI.Stun_RedEyes);

			public static string Hud_FpsUpper => Pick(EN.GUI.Hud_FpsUpper, CN.GUI.Hud_FpsUpper, PT.GUI.Hud_FpsUpper);
			public static string Hud_FpsLower => Pick(EN.GUI.Hud_FpsLower, CN.GUI.Hud_FpsLower, PT.GUI.Hud_FpsLower);

			public static string WelcomeBodyFormat => Pick(EN.GUI.WelcomeBodyFormat, CN.GUI.WelcomeBodyFormat, PT.GUI.WelcomeBodyFormat);
			public static string WelcomeButtonNext => Pick(EN.GUI.WelcomeButtonNext, CN.GUI.WelcomeButtonNext, PT.GUI.WelcomeButtonNext);
			public static string HelpBody => Pick(EN.GUI.HelpBody, CN.GUI.HelpBody, PT.GUI.HelpBody);
			public static string HelpButtonEnjoy => Pick(EN.GUI.HelpButtonEnjoy, CN.GUI.HelpButtonEnjoy, PT.GUI.HelpButtonEnjoy);

			public static string Hud_CpsUpper => Pick(EN.GUI.Hud_CpsUpper, CN.GUI.Hud_CpsUpper, PT.GUI.Hud_CpsUpper);
			public static string Hud_CpsLower => Pick(EN.GUI.Hud_CpsLower, CN.GUI.Hud_CpsLower, PT.GUI.Hud_CpsLower);
			public static string AxisXUpper => Pick(EN.GUI.AxisXUpper, CN.GUI.AxisXUpper, PT.GUI.AxisXUpper);
			public static string AxisXLower => Pick(EN.GUI.AxisXLower, CN.GUI.AxisXLower, PT.GUI.AxisXLower);
			public static string AxisYUpper => Pick(EN.GUI.AxisYUpper, CN.GUI.AxisYUpper, PT.GUI.AxisYUpper);
			public static string AxisYLower => Pick(EN.GUI.AxisYLower, CN.GUI.AxisYLower, PT.GUI.AxisYLower);
			public static string AxisZUpper => Pick(EN.GUI.AxisZUpper, CN.GUI.AxisZUpper, PT.GUI.AxisZUpper);
			public static string AxisZLower => Pick(EN.GUI.AxisZLower, CN.GUI.AxisZLower, PT.GUI.AxisZLower);
			public static string AlertsTrackedFormat => Hud_AlertsFormat;
			public static string ClearAlertsWithCountFormat => Hud_ClearAlertsCountFormat;
			public static string Button_ClearAlertsSimple => Hud_ClearAlerts;

			public static string BindAFormat => Hud_BindAFormat;
			public static string BindBFormat => Hud_BindBFormat;

			public static string WarningDurationFormat => Hud_WarningDurationFormat;
			public static string DecimalsAmountFormat => Hud_DecimalsAmountFormat;

			public static string Toggle_EnableFPS => Hud_EnableFps;
			public static string Toggle_EnableCPS => Hud_EnableCps;
			public static string Toggle_ShowInMenu => Hud_ShowInMenu;
			public static string Toggle_DualCPS => Hud_DualCps;
			public static string Toggle_HidePrefix => Hud_HidePrefix;
			public static string Toggle_InvertPrefix => Hud_InvertPrefix;
			public static string Toggle_UppercasePrefix => Hud_UppercasePrefix;
			public static string Toggle_ShowCoordinates => Hud_ShowCoordinates;
			public static string Toggle_ShowPrefix => Hud_ShowPrefix;
			public static string Toggle_VerticalPosition => Hud_VerticalPosition;
			public static string Toggle_ShowEnrageStatus => Hud_ShowEnrageStatus;
			public static string Toggle_EnableSpeedDetector => Hud_EnableSpeedDetector;
			public static string Toggle_ShowGameTime => Hud_ShowGameTime;
			public static string Toggle_ShowLeadingZeros => Hud_ShowLeadingMinutes;
			public static string Toggle_ShowDecimals => Hud_ShowDecimals;

			public static string HudMenu_FpsCounter => Hud_FpsCounter;
			public static string HudMenu_CpsCounter => Hud_CpsCounter;
			public static string HudMenu_Coordinates => Hud_Coordinates;
			public static string HudMenu_EnrageStatus => Hud_EnrageStatus;
			public static string HudMenu_SpeedDetector => Hud_SpeedDetector;
			public static string HudMenu_GameTime => Hud_GameTime;
			public static string HudSizeLabel => Hud_Size;

			public static string Button_SavePositions => Hud_SavePositions;
			public static string Button_EditPositions => Hud_EditPositions;
			public static string Button_ResetPositions => Hud_ResetPositions;

			public static string Toggle_ToggleMode => Toggle_LookBackToggleMode;
			public static string Toggle_MuteCarnivalClockSounds => Toggle_MuteCarnivalClock;

			public static string Header_Appearance => Header_Appearence;
			public static string CustomizeAppearanceTitle => Button_CustomizeAppearence;
			public static string ClickSoundsLabel => Toggle_ClickSounds;
			public static string ButtonShadowLabel => Toggle_ButtonShadow;

			public static string FPS_Upper => Hud_FpsUpper;
			public static string FPS_Lower => Hud_FpsLower;
			public static string SuspiciousPlayersTitle => Pick(EN.Anti.SuspiciousPlayersTitle, CN.Anti.SuspiciousPlayersTitle, PT.Anti.SuspiciousPlayersTitle);
			public static string DragBoxHint => Pick(EN.Anti.DragBoxHint, CN.Anti.DragBoxHint, PT.Anti.DragBoxHint);
			public static string BulletPrefix => Pick(EN.Anti.BulletPrefix, CN.Anti.BulletPrefix, PT.Anti.BulletPrefix);
		}

		public static class MenuText
		{
			public static string Town => Pick(EN.MenuText.Town, CN.MenuText.Town, PT.MenuText.Town);
			public static string Manor => Pick(EN.MenuText.Manor, CN.MenuText.Manor, PT.MenuText.Manor);
			public static string Farmhouse => Pick(EN.MenuText.Farmhouse, CN.MenuText.Farmhouse, PT.MenuText.Farmhouse);
			public static string Asylum => Pick(EN.MenuText.Asylum, CN.MenuText.Asylum, PT.MenuText.Asylum);
			public static string Inn => Pick(EN.MenuText.Inn, CN.MenuText.Inn, PT.MenuText.Inn);
			public static string Slaughterhouse => Pick(EN.MenuText.Slaughterhouse, CN.MenuText.Slaughterhouse, PT.MenuText.Slaughterhouse);
			public static string Carnival => Pick(EN.MenuText.Carnival, CN.MenuText.Carnival, PT.MenuText.Carnival);
		}

		public static class Anti
		{
			public static string UnknownName => Pick(EN.Anti.UnknownName, CN.Anti.UnknownName, PT.Anti.UnknownName);
			public static string SuspiciousSpeedFormat => Pick(EN.Anti.SuspiciousSpeedFormat, CN.Anti.SuspiciousSpeedFormat, PT.Anti.SuspiciousSpeedFormat);
			public static string AlertsTitle => Pick(EN.Anti.AlertsTitle, CN.Anti.AlertsTitle, PT.Anti.AlertsTitle);

			public static string SuspiciousPlayersTitle => Pick(EN.Anti.SuspiciousPlayersTitle, CN.Anti.SuspiciousPlayersTitle, PT.Anti.SuspiciousPlayersTitle);
			public static string DragBoxHint => Pick(EN.Anti.DragBoxHint, CN.Anti.DragBoxHint, PT.Anti.DragBoxHint);
		}

		public static class Tabs
		{
			public static string Optimize => Pick(EN.Tabs.Optimize, CN.Tabs.Optimize, PT.Tabs.Optimize);
			public static string HSV => Pick(EN.Tabs.HSV, CN.Tabs.HSV, PT.Tabs.HSV);
			public static string Speedrun => Pick(EN.Tabs.Speedrun, CN.Tabs.Speedrun, PT.Tabs.Speedrun);
			public static string FOV => Pick(EN.Tabs.FOV, CN.Tabs.FOV, PT.Tabs.FOV);
			public static string Anticheat => Pick(EN.Tabs.Anticheat, CN.Tabs.Anticheat, PT.Tabs.Anticheat);
			public static string Menu => Pick(EN.Tabs.Menu, CN.Tabs.Menu, PT.Tabs.Menu);
		}

		public static class Info
		{
			public static string Optimize => Pick(EN.InfoText.Optimize, CN.InfoText.Optimize, PT.InfoText.Optimize);
			public static string HSV => Pick(EN.InfoText.HSV, CN.InfoText.HSV, PT.InfoText.HSV);
			public static string Speedrun => Pick(EN.InfoText.Speedrun, CN.InfoText.Speedrun, PT.InfoText.Speedrun);
			public static string Gameplay => Pick(EN.InfoText.Gameplay, CN.InfoText.Gameplay, PT.InfoText.Gameplay);
			public static string FOV => Pick(EN.InfoText.FOV, CN.InfoText.FOV, PT.InfoText.FOV);
			public static string Settings => Pick(EN.InfoText.Settings, CN.InfoText.Settings, PT.InfoText.Settings);
			public static string Anticheat => Pick(EN.InfoText.Anticheat, CN.InfoText.Anticheat, PT.InfoText.Anticheat);
			public static string Menu => Pick(EN.InfoText.Menu, CN.InfoText.Menu, PT.InfoText.Menu);
			public static string HUD => Pick(EN.InfoText.HUD, CN.InfoText.HUD, PT.InfoText.HUD);
		}
	}
}