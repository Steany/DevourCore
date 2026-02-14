using UnityEngine;

namespace DevourCore
{
	public static class EN
	{
		public static class GUI
		{
			public const string Tab_Optimize = "Optimize";
			public const string Tab_HSV = "HSV";
			public const string Tab_Speedrun = "Speedrun";
			public const string Tab_FOV = "FOV";
			public const string Tab_Anticheat = "Anticheat";
			public const string Tab_Menu = "Menu";
			public const string Tab_Settings = "Settings";

			public const string PressAnyKey = "Press any key...";
			public const string ToggleKeyFormat = "Toggle Key: {0}";
			public const string InteractKeyFormat = "Interact Key: {0}";
			public const string MenuKeyFormat = "Open Client: {0}";
			public const string CurrentClipFormat = "Current: {0}";
			public const string AreYouSure = "Are you sure?";
			public const string ResetClientSettings = "Reset Client Settings";

			public const string Header_Optimize = "Performance Optimizations";
			public const string Desc_Optimize = "Customize render distance and disable weather effects";
			public const string Header_Cull = "Render Distance";
			public const string Toggle_CullEnabled = "Render Distance";
			public const string Toggle_CullInMenu = "Enable in Menu";
			public const string CullDistanceFormat = "Distance: {0} m";
			public const string Header_Weather = "Weather";
			public const string Toggle_DisableWeather = "Disable Weather Effects";
			public const string Toggle_MuteWeather = "Mute Weather Sounds";

			public const string Header_OutfitColor = "Outfit Color";
			public const string Header_IconColor = "Icon Color";
			public const string Desc_OutfitColor = "Change the color of all player outfits";
			public const string Desc_IconColor = "Customize the color of the 666 icon";
			public const string Toggle_IconEnabled = "Icon HSV";
			public const string Toggle_OutfitEnabled = "Outfit HSV";
			public const string Swap_ToOutfit = "Outfit ↔";
			public const string Swap_ToIcon = "Icon ↔";
			public const string Header_Preview = "Preview";

			public const string Header_Speedrun = "Speedrunning Mods";
			public const string Desc_Speedrun = "Useful features for speedrun categories";
			public const string Toggle_InstantInteract = "Instant Interaction";
			public const string Toggle_AtticSpawn = "Attic Spawn";
			public const string Header_AutoStart = "Auto Start";
			public const string Desc_AutoStart = "Automatically start game when returning to lobby";
			public const string Toggle_ForceStart = "Enable Auto Start";
			public const string Toggle_UseArm = "Auto Start Timeout";
			public const string ForceStartDelayFormat = "Start Delay: {0:F1}s";
			public const string ForceStartArmFormat = "Timeout Trigger: {0:F1} min";
			public const string SpeedrunPopupBody =
				"Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be seen as cheating. Exploiting these features to gain levels or unfair advantages is considered cheating. These mods are intended purely for fun and speedrunning purposes, without causing harm or disadvantages to others. Use with caution.";
			public const string SpeedrunPopupConfirm = "Understood";

			public const string Header_FOV = "Custom FOV";
			public const string Desc_FOV = "Customize your field of view over the normal limit";
			public const string Toggle_FOVEnabled = "FOV Limit Bypass";
			public const string FOVValueFormat = "FOV: {0}°";

			public const string Header_Anticheat = "Speed Anticheat";
			public const string Desc_Anticheat = "Monitor players for suspicious movement speed";
			public const string Toggle_AnticheatEnabled = "Enable Detection";
			public const string AlertDurationFormat = "Alert Duration: {0:F1}s";
			public const string Button_ClearAlerts = "Clear Alerts";
			public const string AnticheatStatusFormat = "Tracking {0} players | {1} active alerts";
			public const string Button_SaveAlertPosition = "Save Alert Position";
			public const string Button_EditAlertPosition = "Edit Alert Position";
			public const string Button_ResetAlertPosition = "Reset Position";

			public const string Header_Menu = "Menu Customization";
			public const string Desc_Menu = "Customize menu background and music settings";
			public const string Header_MenuBackground = "Menu Customization";
			public const string Toggle_CustomBackground = "Custom Background";
			public const string Header_MusicSettings = "Audio Settings";
			public const string Toggle_DisableIngameMusic = "Disable Music in-Game";
			public const string Toggle_MuteTunnel = "Mute Carnival Tunnel";
			public const string Toggle_RememberMusic = "Remember Menu Music";

			public const string Header_Settings = "Client Settings";
			public const string Desc_Settings = "Customize menu keybind and appearance";
			public const string Header_Hotkeys = "Client Keybind";
			public const string Header_ThemeColor = "Appearence";
			public const string Desc_ThemeColor = "Adjust the hue to change all UI colors";
			public const string Header_Miscellaneous = "Advanced";

			public const string ThemeTabsHueFormat = "Tabs: {0}°";
			public const string ThemeBackgroundHueFormat = "Background: {0}°";
			public const string Toggle_DarkMode = "Dark Tabs";
			public const string Toggle_NoBackground = "No Background";
			public const string LanguageLabel = "Language: English (EN)";

			public const string InfoOverlayTitle = "Info";

			public const string Header_VisibleCategories = "Visible Categories";
			public const string Desc_VisibleCategories = "Choose which tabs are visible in the client.";

			public const string HueFormat = "Hue: {0}°";
			public const string HudOpacityFormat = "Opacity: {0}%";
			public const string Toggle_HudChroma = "Chroma";
			public const string UnknownSymbol = "?";


			public const string Tab_Gameplay = "Gameplay";
			public const string Tab_HUD = "HUD";

			public const string Slider_H = "H";
			public const string Slider_S = "S";
			public const string Slider_V = "V";

			public const string Header_LookBack = "Look-Back";
			public const string Toggle_EnableLookBack = "Enable Look-Back";
			public const string Toggle_LookBackToggleMode = "Toggle Mode";
			public const string LookBackKeyFormat = "Look-back Key: {0}";
			public const string LookBackNotAvailable = "Look-back feature not available.";

			public const string Header_Audio = "Audio";
			public const string Toggle_MuteCarnivalClock = "Mute Carnival Clock Sounds";

			public const string Header_HUD = "HUD";
			public const string Button_Back = "Back";
			public const string Hud_EnableFps = "Enable FPS";
			public const string Hud_ShowInMenu = "Show in Menu";
			public const string Hud_HidePrefix = "Hide Prefix";
			public const string Hud_InvertPrefix = "Invert Prefix";
			public const string Hud_UppercasePrefix = "Uppercase Prefix";
			public const string Hud_EnableCps = "Enable CPS";
			public const string Hud_DualCps = "Dual CPS";
			public const string Hud_BindAFormat = "Bind A: {0}";
			public const string Hud_BindBFormat = "Bind B: {0}";
			public const string Hud_ShowCoordinates = "Show Coordinates";
			public const string Hud_ShowPrefix = "Show Prefix";
			public const string Hud_VerticalPosition = "Vertical Position";
			public const string Hud_ShowEnrageStatus = "Show Enrage Status";
			public const string Hud_NoBackground = "No Background";
			public const string Hud_EnableSpeedDetector = "Enable Speed Detector";
			public const string Hud_WarningDurationFormat = "Warning Duration: {0:0.0}s";
			public const string Hud_AlertsFormat = "Alerts: {0} | Tracked: {1}";
			public const string Hud_ClearAlerts = "Clear Alerts";
			public const string Hud_ClearAlertsCountFormat = "Clear Alerts ({0})";
			public const string Hud_ShowGameTime = "Show Game Time";
			public const string Hud_ShowLeadingMinutes = "Show zeros before minutes";
			public const string Hud_ShowDecimals = "Show decimals";
			public const string Hud_DecimalsAmountFormat = "Decimals Amount: {0}";
			public const string Hud_FpsCounter = "FPS Counter";
			public const string Hud_CpsCounter = "CPS Counter";
			public const string Hud_Coordinates = "Coordinates";
			public const string Hud_EnrageStatus = "Enrage Status";
			public const string Hud_SpeedDetector = "Speed Detector";
			public const string Hud_GameTime = "Game Time";
			public const string Hud_Size = "HUD Size";
			public const string Hud_SavePositions = "Save Positions";
			public const string Hud_EditPositions = "Edit Positions";
			public const string Hud_ResetPositions = "Reset Positions";

			public const string Header_Appearence = "Appearence";
			public const string BackgroundOpacityFormat = "Background Opacity: {0}%";
			public const string Toggle_DefaultBackground = "Default Background";
			public const string Toggle_ChromaMode = "Chroma Mode";
			public const string Button_CustomizeAppearence = "Customize Appearence";
			public const string Toggle_ClickSounds = "Click Sounds";
			public const string Toggle_ButtonShadow = "Buttons Shadow";

			public const string Stun_Calm = "CALM";
			public const string Stun_Enraged = "ENRAGED";
			public const string Stun_RedEyes = "RED EYES";

			public const string Hud_FpsUpper = "FPS";
			public const string Hud_FpsLower = "fps";
			public const string AxisZLower = "z";
			public const string AxisZUpper = "Z";
			public const string AxisYLower = "y";
			public const string AxisYUpper = "Y";
			public const string AxisXLower = "x";
			public const string AxisXUpper = "X";
			public const string Hud_CpsLower = "cps";
			public const string Hud_CpsUpper = "CPS";

			public const string WelcomeBodyFormat = "Welcome to DevourCore!\nPress '{0}' to open the Client.";
			public const string WelcomeButtonNext = "Next";
			public const string HelpBody = "For mod explanations on each category hover the info icon in the bottom right of the GUI. You can also drag the GUI to position it anywhere you like.";
			public const string HelpButtonEnjoy = "Enjoy <3";

			public const string Button_HudTextStyleFormat = "HUD Text Style: {0}";
			public const string Button_UiTextStyleFormat = "UI Text Style: {0}";
			public const string TextStyle_Outline = "Outline";
			public const string TextStyle_Shadow = "Shadow";
		}

		public static class MenuText
		{
			public const string Town = "Town";
			public const string Manor = "Manor";
			public const string Farmhouse = "Farmhouse";
			public const string Asylum = "Asylum";
			public const string Inn = "Inn";
			public const string Slaughterhouse = "Slaughterhouse";
			public const string Carnival = "Carnival";
		}

		public static class Anti
		{
			public const string UnknownName = "Unknown";
			public const string SuspiciousSpeedFormat = "{0} - suspicious speed! avg={1:F2} m/s";
			public const string AlertsTitle = "⚠ SpeedWatch - Suspicious Players";

			public const string SuspiciousPlayersTitle = "Suspicious Players";
			public const string DragBoxHint = "Drag this box to set alert position";
			public const string BulletPrefix = "• ";
		}

		public static class Tabs
		{
			public const string Optimize = "Optimize";
			public const string HSV = "HSV";
			public const string Speedrun = "Speedrun";
			public const string FOV = "FOV";
			public const string Anticheat = "Anticheat";
			public const string Menu = "Menu";
		}

		public static class InfoText
		{
			public const string Optimize = @"
{B}<b><color={ACCENT}>Render Distance:</color></b> Adjusts how far map objects are rendered. Works in-Game and in the Menu.

<b><color={WARN}>Warning:</color></b> Any perks that highlight items (e.g. <b>Inspired</b>), will only appear when you are close to them.
It is recommended to assign a <b>keybind</b> to quickly toggle render distance and temporarily restore full visibility for a few seconds.

{B}<b><color={ACCENT}>Disable Weather Effects:</color></b> Removes all weather effects (rain, snow, wind). Can only be enabled from the menu or lobby.

<b><color={WARN}>Warning:</color></b> If you remain in cutscenes for too long, the weather will not disable.";

			public const string HSV = @"
{B}<b>You can swap between <b><color={ACCENT}>Outfit:</color></b> and <b><color={ACCENT}>Icon:</color></b> HSV using the button above the preview.</b>

{B}<b><color={ACCENT}>Icon:</color></b> Customizes your icon color and select (based on your level) between 70 and 666. Use the button above the Swap button to cycle through icons.

{B}<b><color={ACCENT}>Outfit:</color></b> Adjusts the <b>HSV</b> (hue, saturation, value) of all player outfits at once.

<b><color={WARN}>Warning:</color></b> To drastically improve performance, Outfit scanning may take a bit longer on some maps.

{B}These are visual hue overlays rather than hard recolors, so certain colors may look slightly different than expected.

{B}<b>All changes are purely visual and not visible to other players.</b>";

			public const string Speedrun = @"
<b><color={DANGER}>Disclaimer</color></b>: Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be considered cheating. Power-leveling or gaining unfair advantages with these features is cheating. These mods are intended for fun and legitimate speedrunning only, not to harm or oppress other players.

{B}<b><color={ACCENT}>Instant Interaction:</color></b> Removes all long interactions (revives, rituals, cages, etc).

{B}Make sure your configured <b>interact key</b> matches the in-game interact key exactly, or the feature may not work.

{B}<b><color={ACCENT}>Attic Spawn:</color></b> Re‑creates the old Farmhouse bug where using Anna would spawn you in the attic, additionally allows it to work with <b>any character</b>. This also restores the old door behavior, meaning doors you open before triggering Anna will remain open instead of closing automatically.

{B}<b><color={ACCENT}>Auto Start:</color></b> Automatically starts matches from the lobby, with a <b><color={ACCENT}>Start Delay</color></b> to wait before starting and an <b><color={ACCENT}>Auto Start Timeout</color></b> that disables Auto Start after the slider time has passed, preventing the game from auto-starting the next time you enter a lobby.

<b><color={WARN}>Warning:</color></b> If the lobby takes too long to load (performance dependent), <b><color={ACCENT}>Auto Start</color></b> may trigger before the lobby is fully loaded, causing the mod to break. If this happens, simply rejoin the lobby or reload singleplayer if playing solo. The mod works in singleplayer and as host. If you are not the host, it will break and require the same fix above.";

			public const string FOV = @"
{B}When enabled, toggles the custom FOV override without touching your normal in‑game FOV setting.

{B}Allows values below <b><color={ACCENT}>60</color></b> and above <b><color={ACCENT}>95</color></b> without changing the in‑game FOV slider.

{B}When enabled, jumpscares and cutscenes also use your custom FOV.

{B}Using UV does not cause additional camera side‑effects.";

			public const string Settings = @"
{B}You can set a <b>custom hotkey</b> to open the client.

<b><color={WARN}>Note:</color></b> If you accidentally bind it to <b>Left Mouse Button</b>, you can change it again by using <b>Right Mouse</b> Button.

<b><color={ACCENT}>Appearance:</color></b> Customize your client’s look with tab, checkbox, and slider colors, custom background color and opacity, <b><color={ACCENT}>Default Background</color></b>, <b><color={ACCENT}>Dark Tabs</color></b>, <b><color={ACCENT}>Chroma Mode</color></b>, game's <b><color={ACCENT}>Click Sounds</color></b>, <b><color={ACCENT}>Buttons Shadow</color></b>, and a choice between text <b><color={ACCENT}>Outline</color></b> or <b><color={ACCENT}>Shadow</color></b> styles.

{B}Under <b><color={ACCENT}>Visible Categories</color></b> you can enable or disable entire tabs so only the ones you care about remain visible.

{B}<b><color={ACCENT}>Language</color></b> cycles the language between English, Chinese, and Portuguese.

{B}<b><color={ACCENT}>Reset Client Settings</color></b> restores all settings to their default values.

{B}If the GUI ever moves off-screen, press <b>F1</b> to re-center it.";

			public const string Anticheat = @"
{B}<b><color={ACCENT}>Speed Anticheat:</color></b> Monitors players <b>average movement speed</b> and flags those exceeding the vanilla threshold, displaying an on-screen alert with their name and speed.

{B}<b><color={ACCENT}>Clear Alerts</color></b> removes all current warnings and resets per‑player alert flags.

{B}<b><color={ACCENT}>Alert Duration:</color></b> Sets how long each warning stays on screen and how long a suspicious player remains highlighted.

{B}<b><color={ACCENT}>Edit Alert Position:</color></b> Lets drag the alert box anywhere on your screen and then save its new position.

{B}<b><color={ACCENT}>Reset Position:</color></b> Restores the alert box back to its default location.

<b><color={WARN}>Note:</color></b> This mod is <b>informational only</b>. It <b>never kicks</b> players; use it as a tool to help judge suspicious behaviour. Alerts and tracking reset on scene changes or when you use <b>Clear Alerts</b>.";

			public const string Menu = @"
{B}<b><color={ACCENT}>Menu Background:</color></b> Replaces the default menu background with any of the lobby environments.

<b><color={WARN}>Note:</color></b> While you are in a lobby, background selection cannot be changed until you return to the <b>main menu.</b>

{B}<b><color={ACCENT}>Remember Menu Music:</color></b> After restarting the game, the last chosen <b>menu music</b> will be used again.";

			public const string Gameplay = @"
{B}<b><color={ACCENT}>FOV Limit Bypass:</color></b> Lets you go below the normal minimum and above the normal maximum without touching the in-game FOV slider.

{B}When enabled, your chosen FOV also applies during cutscenes and jumpscares.

{B}<b><color={ACCENT}>Look-Back:</color></b> Adds a quick way to check behind you while running.

{B}You can use it as <b>Hold</b> (only active while holding the key) or <b>Toggle Mode</b> (press once to lock it, press again to return).

{B}<b><color={ACCENT}>Audio</color></b> offers some quality-of-life audio toggles.

{B}<b>Mute Weather Sounds</b> silences rain/wind ambience.

{B}<b>Disable Music in-Game</b> turns off map music while keeping menu music.

{B}<b>Mute Carnival Tunnel</b> removes the blue tunnel loop in Carnival.

{B}<b>Mute Carnival Clock Sounds</b> disables the clock ambience in Carnival.";

			public const string HUD = @"
{B}<b><color={ACCENT}>HUD:</color></b> Customizable on-screen overlays you can position freely.

{B}<b><color={ACCENT}>FPS Counter:</color></b> Displays your frame rate.

{B}<b><color={ACCENT}>CPS Counter:</color></b> Displays your clicks per second that you can bind to two keys of your choice (commonly <b>action</b> and <b>drop</b>).

{B}<b><color={ACCENT}>Coordinates:</color></b> Shows your position in-game.

{B}<b><color={ACCENT}>Enrage Status:</color></b> Shows Anna’s current state (enraged, red eyes, calm).

{B}<b><color={ACCENT}>Speed Detector:</color></b> Monitors average player movement speed in real time and sends a warning when values look suspicious.

{B}<b><color={ACCENT}>Game Time:</color></b> Shows how long the match has been going for.

{B}<b><color={ACCENT}>HUD Size:</color></b> Global scale for all HUD elements.";

		}
	}
}