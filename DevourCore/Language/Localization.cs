using MelonLoader;

namespace DevourCore
{
	public enum Language
	{
		EN = 0,
		CN = 1
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

		public static class GUI
		{
			public static string Tab_Optimize => Current == Language.EN ? EN.GUI.Tab_Optimize : CN.GUI.Tab_Optimize;
			public static string Tab_HSV => Current == Language.EN ? EN.GUI.Tab_HSV : CN.GUI.Tab_HSV;
			public static string Tab_Speedrun => Current == Language.EN ? EN.GUI.Tab_Speedrun : CN.GUI.Tab_Speedrun;
			public static string Tab_FOV => Current == Language.EN ? EN.GUI.Tab_FOV : CN.GUI.Tab_FOV;
			public static string Tab_Anticheat => Current == Language.EN ? EN.GUI.Tab_Anticheat : CN.GUI.Tab_Anticheat;
			public static string Tab_Menu => Current == Language.EN ? EN.GUI.Tab_Menu : CN.GUI.Tab_Menu;
			public static string Tab_Settings => Current == Language.EN ? EN.GUI.Tab_Settings : CN.GUI.Tab_Settings;

			public static string PressAnyKey => Current == Language.EN ? EN.GUI.PressAnyKey : CN.GUI.PressAnyKey;
			public static string ToggleKeyFormat => Current == Language.EN ? EN.GUI.ToggleKeyFormat : CN.GUI.ToggleKeyFormat;
			public static string InteractKeyFormat => Current == Language.EN ? EN.GUI.InteractKeyFormat : CN.GUI.InteractKeyFormat;
			public static string MenuKeyFormat => Current == Language.EN ? EN.GUI.MenuKeyFormat : CN.GUI.MenuKeyFormat;
			public static string CurrentClipFormat => Current == Language.EN ? EN.GUI.CurrentClipFormat : CN.GUI.CurrentClipFormat;
			public static string AreYouSure => Current == Language.EN ? EN.GUI.AreYouSure : CN.GUI.AreYouSure;
			public static string ResetClientSettings => Current == Language.EN ? EN.GUI.ResetClientSettings : CN.GUI.ResetClientSettings;

			public static string Header_Optimize => Current == Language.EN ? EN.GUI.Header_Optimize : CN.GUI.Header_Optimize;
			public static string Desc_Optimize => Current == Language.EN ? EN.GUI.Desc_Optimize : CN.GUI.Desc_Optimize;
			public static string Header_Cull => Current == Language.EN ? EN.GUI.Header_Cull : CN.GUI.Header_Cull;
			public static string Toggle_CullEnabled => Current == Language.EN ? EN.GUI.Toggle_CullEnabled : CN.GUI.Toggle_CullEnabled;
			public static string Toggle_CullInMenu => Current == Language.EN ? EN.GUI.Toggle_CullInMenu : CN.GUI.Toggle_CullInMenu;
			public static string CullDistanceFormat => Current == Language.EN ? EN.GUI.CullDistanceFormat : CN.GUI.CullDistanceFormat;
			public static string Header_Weather => Current == Language.EN ? EN.GUI.Header_Weather : CN.GUI.Header_Weather;
			public static string Toggle_DisableWeather => Current == Language.EN ? EN.GUI.Toggle_DisableWeather : CN.GUI.Toggle_DisableWeather;
			public static string Toggle_MuteWeather => Current == Language.EN ? EN.GUI.Toggle_MuteWeather : CN.GUI.Toggle_MuteWeather;

			public static string Header_OutfitColor => Current == Language.EN ? EN.GUI.Header_OutfitColor : CN.GUI.Header_OutfitColor;
			public static string Header_IconColor => Current == Language.EN ? EN.GUI.Header_IconColor : CN.GUI.Header_IconColor;
			public static string Desc_OutfitColor => Current == Language.EN ? EN.GUI.Desc_OutfitColor : CN.GUI.Desc_OutfitColor;
			public static string Desc_IconColor => Current == Language.EN ? EN.GUI.Desc_IconColor : CN.GUI.Desc_IconColor;
			public static string Toggle_IconEnabled => Current == Language.EN ? EN.GUI.Toggle_IconEnabled : CN.GUI.Toggle_IconEnabled;
			public static string Toggle_OutfitEnabled => Current == Language.EN ? EN.GUI.Toggle_OutfitEnabled : CN.GUI.Toggle_OutfitEnabled;
			public static string Swap_ToOutfit => Current == Language.EN ? EN.GUI.Swap_ToOutfit : CN.GUI.Swap_ToOutfit;
			public static string Swap_ToIcon => Current == Language.EN ? EN.GUI.Swap_ToIcon : CN.GUI.Swap_ToIcon;
			public static string Header_Preview => Current == Language.EN ? EN.GUI.Header_Preview : CN.GUI.Header_Preview;

			public static string Header_Speedrun => Current == Language.EN ? EN.GUI.Header_Speedrun : CN.GUI.Header_Speedrun;
			public static string Desc_Speedrun => Current == Language.EN ? EN.GUI.Desc_Speedrun : CN.GUI.Desc_Speedrun;
			public static string Toggle_InstantInteract => Current == Language.EN ? EN.GUI.Toggle_InstantInteract : CN.GUI.Toggle_InstantInteract;
			public static string Toggle_AtticSpawn => Current == Language.EN ? EN.GUI.Toggle_AtticSpawn : CN.GUI.Toggle_AtticSpawn;
			public static string Header_AutoStart => Current == Language.EN ? EN.GUI.Header_AutoStart : CN.GUI.Header_AutoStart;
			public static string Desc_AutoStart => Current == Language.EN ? EN.GUI.Desc_AutoStart : CN.GUI.Desc_AutoStart;
			public static string Toggle_ForceStart => Current == Language.EN ? EN.GUI.Toggle_ForceStart : CN.GUI.Toggle_ForceStart;
			public static string Toggle_UseArm => Current == Language.EN ? EN.GUI.Toggle_UseArm : CN.GUI.Toggle_UseArm;
			public static string ForceStartDelayFormat => Current == Language.EN ? EN.GUI.ForceStartDelayFormat : CN.GUI.ForceStartDelayFormat;
			public static string ForceStartArmFormat => Current == Language.EN ? EN.GUI.ForceStartArmFormat : CN.GUI.ForceStartArmFormat;
			public static string SpeedrunPopupBody => Current == Language.EN ? EN.GUI.SpeedrunPopupBody : CN.GUI.SpeedrunPopupBody;
			public static string SpeedrunPopupConfirm => Current == Language.EN ? EN.GUI.SpeedrunPopupConfirm : CN.GUI.SpeedrunPopupConfirm;

			public static string Header_FOV => Current == Language.EN ? EN.GUI.Header_FOV : CN.GUI.Header_FOV;
			public static string Desc_FOV => Current == Language.EN ? EN.GUI.Desc_FOV : CN.GUI.Desc_FOV;
			public static string Toggle_FOVEnabled => Current == Language.EN ? EN.GUI.Toggle_FOVEnabled : CN.GUI.Toggle_FOVEnabled;
			public static string FOVValueFormat => Current == Language.EN ? EN.GUI.FOVValueFormat : CN.GUI.FOVValueFormat;

			public static string Header_Anticheat => Current == Language.EN ? EN.GUI.Header_Anticheat : CN.GUI.Header_Anticheat;
			public static string Desc_Anticheat => Current == Language.EN ? EN.GUI.Desc_Anticheat : CN.GUI.Desc_Anticheat;
			public static string Toggle_AnticheatEnabled => Current == Language.EN ? EN.GUI.Toggle_AnticheatEnabled : CN.GUI.Toggle_AnticheatEnabled;
			public static string AlertDurationFormat => Current == Language.EN ? EN.GUI.AlertDurationFormat : CN.GUI.AlertDurationFormat;
			public static string Button_ClearAlerts => Current == Language.EN ? EN.GUI.Button_ClearAlerts : CN.GUI.Button_ClearAlerts;
			public static string AnticheatStatusFormat => Current == Language.EN ? EN.GUI.AnticheatStatusFormat : CN.GUI.AnticheatStatusFormat;
			public static string Button_SaveAlertPosition => Current == Language.EN ? EN.GUI.Button_SaveAlertPosition : CN.GUI.Button_SaveAlertPosition;
			public static string Button_EditAlertPosition => Current == Language.EN ? EN.GUI.Button_EditAlertPosition : CN.GUI.Button_EditAlertPosition;
			public static string Button_ResetAlertPosition => Current == Language.EN ? EN.GUI.Button_ResetAlertPosition : CN.GUI.Button_ResetAlertPosition;

			public static string Header_Menu => Current == Language.EN ? EN.GUI.Header_Menu : CN.GUI.Header_Menu;
			public static string Desc_Menu => Current == Language.EN ? EN.GUI.Desc_Menu : CN.GUI.Desc_Menu;
			public static string Header_MenuBackground => Current == Language.EN ? EN.GUI.Header_MenuBackground : CN.GUI.Header_MenuBackground;
			public static string Toggle_CustomBackground => Current == Language.EN ? EN.GUI.Toggle_CustomBackground : CN.GUI.Toggle_CustomBackground;
			public static string Header_MusicSettings => Current == Language.EN ? EN.GUI.Header_MusicSettings : CN.GUI.Header_MusicSettings;
			public static string Toggle_DisableIngameMusic => Current == Language.EN ? EN.GUI.Toggle_DisableIngameMusic : CN.GUI.Toggle_DisableIngameMusic;
			public static string Toggle_MuteTunnel => Current == Language.EN ? EN.GUI.Toggle_MuteTunnel : CN.GUI.Toggle_MuteTunnel;
			public static string Toggle_RememberMusic => Current == Language.EN ? EN.GUI.Toggle_RememberMusic : CN.GUI.Toggle_RememberMusic;

			public static string Header_Settings => Current == Language.EN ? EN.GUI.Header_Settings : CN.GUI.Header_Settings;
			public static string Desc_Settings => Current == Language.EN ? EN.GUI.Desc_Settings : CN.GUI.Desc_Settings;
			public static string Header_Hotkeys => Current == Language.EN ? EN.GUI.Header_Hotkeys : CN.GUI.Header_Hotkeys;
			public static string Header_ThemeColor => Current == Language.EN ? EN.GUI.Header_ThemeColor : CN.GUI.Header_ThemeColor;
			public static string Desc_ThemeColor => Current == Language.EN ? EN.GUI.Desc_ThemeColor : CN.GUI.Desc_ThemeColor;
			public static string Header_Miscellaneous => Current == Language.EN ? EN.GUI.Header_Miscellaneous : CN.GUI.Header_Miscellaneous;

			public static string ThemeTabsHueFormat => Current == Language.EN ? EN.GUI.ThemeTabsHueFormat : CN.GUI.ThemeTabsHueFormat;
			public static string ThemeBackgroundHueFormat => Current == Language.EN ? EN.GUI.ThemeBackgroundHueFormat : CN.GUI.ThemeBackgroundHueFormat;
			public static string Toggle_DarkMode => Current == Language.EN ? EN.GUI.Toggle_DarkMode : CN.GUI.Toggle_DarkMode;
			public static string Toggle_NoBackground => Current == Language.EN ? EN.GUI.Toggle_NoBackground : CN.GUI.Toggle_NoBackground;
			public static string LanguageLabel => Current == Language.EN ? EN.GUI.LanguageLabel : CN.GUI.LanguageLabel;

			public static string InfoOverlayTitle => Current == Language.EN ? EN.GUI.InfoOverlayTitle : CN.GUI.InfoOverlayTitle;

			public static string Header_VisibleCategories => Current == Language.EN ? EN.GUI.Header_VisibleCategories : CN.GUI.Header_VisibleCategories;
			public static string Desc_VisibleCategories => Current == Language.EN ? EN.GUI.Desc_VisibleCategories : CN.GUI.Desc_VisibleCategories;
		}

		public static class MenuText
		{
			public static string Town => Current == Language.EN ? EN.MenuText.Town : CN.MenuText.Town;
			public static string Manor => Current == Language.EN ? EN.MenuText.Manor : CN.MenuText.Manor;
			public static string Farmhouse => Current == Language.EN ? EN.MenuText.Farmhouse : CN.MenuText.Farmhouse;
			public static string Asylum => Current == Language.EN ? EN.MenuText.Asylum : CN.MenuText.Asylum;
			public static string Inn => Current == Language.EN ? EN.MenuText.Inn : CN.MenuText.Inn;
			public static string Slaughterhouse => Current == Language.EN ? EN.MenuText.Slaughterhouse : CN.MenuText.Slaughterhouse;
			public static string Carnival => Current == Language.EN ? EN.MenuText.Carnival : CN.MenuText.Carnival;
		}

		public static class Anti
		{
			public static string UnknownName => Current == Language.EN ? EN.Anti.UnknownName : CN.Anti.UnknownName;
			public static string SuspiciousSpeedFormat => Current == Language.EN ? EN.Anti.SuspiciousSpeedFormat : CN.Anti.SuspiciousSpeedFormat;
			public static string AlertsTitle => Current == Language.EN ? EN.Anti.AlertsTitle : CN.Anti.AlertsTitle;
		}

		public static class Tabs
		{
			public static string Optimize => Current == Language.EN ? EN.Tabs.Optimize : CN.Tabs.Optimize;
			public static string HSV => Current == Language.EN ? EN.Tabs.HSV : CN.Tabs.HSV;
			public static string Speedrun => Current == Language.EN ? EN.Tabs.Speedrun : CN.Tabs.Speedrun;
			public static string FOV => Current == Language.EN ? EN.Tabs.FOV : CN.Tabs.FOV;
			public static string Anticheat => Current == Language.EN ? EN.Tabs.Anticheat : CN.Tabs.Anticheat;
			public static string Menu => Current == Language.EN ? EN.Tabs.Menu : CN.Tabs.Menu;
		}

		public static class Info
		{
			public static string Optimize => Current == Language.EN ? EN.InfoText.Optimize : CN.InfoText.Optimize;
			public static string HSV => Current == Language.EN ? EN.InfoText.HSV : CN.InfoText.HSV;
			public static string Speedrun => Current == Language.EN ? EN.InfoText.Speedrun : CN.InfoText.Speedrun;
			public static string FOV => Current == Language.EN ? EN.InfoText.FOV : CN.InfoText.FOV;
			public static string Settings => Current == Language.EN ? EN.InfoText.Settings : CN.InfoText.Settings;
			public static string Anticheat => Current == Language.EN ? EN.InfoText.Anticheat : CN.InfoText.Anticheat;
			public static string Menu => Current == Language.EN ? EN.InfoText.Menu : CN.InfoText.Menu;
		}
	}
}