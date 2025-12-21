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
			public const string Tab_Menu = "Misc";
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
			public const string Toggle_CullEnabled = "Enable In-Game";
			public const string Toggle_CullInMenu = "Enable In Menu";
			public const string CullDistanceFormat = "Distance: {0} m";
			public const string Header_Weather = "Weather";
			public const string Toggle_DisableWeather = "Disable Effects";
			public const string Toggle_MuteWeather = "Mute Audio";

			public const string Header_OutfitColor = "Outfit Color";
			public const string Header_IconColor = "Icon Color";
			public const string Desc_OutfitColor = "Change the color of all player outfits";
			public const string Desc_IconColor = "Customize the color of the 666 icon";
			public const string Toggle_IconEnabled = "Enable Icon";
			public const string Toggle_OutfitEnabled = "Enable Outfit";
			public const string Swap_ToOutfit = "Outfit ↔";
			public const string Swap_ToIcon = "Icon ↔";
			public const string Header_Preview = "Preview";

			public const string Header_Speedrun = "Speedrunning Mods";
			public const string Desc_Speedrun = "Useful features for speedrun categories";
			public const string Toggle_InstantInteract = "Instant Interaction";
			public const string Toggle_AtticSpawn = "Attic Spawn";
			public const string Header_AutoStart = "Auto Start";
			public const string Desc_AutoStart = "Automatically start game when returning to lobby";
			public const string Toggle_ForceStart = "Enable";
			public const string Toggle_UseArm = "Arming Window";
			public const string ForceStartDelayFormat = "Start Delay: {0:F1}s";
			public const string ForceStartArmFormat = "Arming Window: {0:F1} min";
			public const string SpeedrunPopupBody =
				"Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be seen as cheating. Exploiting these features to gain levels or unfair advantages is considered cheating. These mods are intended purely for fun and speedrunning purposes, without causing harm or disadvantages to others. Use with caution.";
			public const string SpeedrunPopupConfirm = "Understood";

			public const string Header_FOV = "Custom FOV";
			public const string Desc_FOV = "Customize your field of view over the normal limit";
			public const string Toggle_FOVEnabled = "Enable";
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
			public const string Toggle_DisableIngameMusic = "Disable Music In-Game";
			public const string Toggle_MuteTunnel = "Mute Carnival Tunnel";
			public const string Toggle_RememberMusic = "Remember Menu Music";

			public const string Header_Settings = "Client Settings";
			public const string Desc_Settings = "Customize menu keybind and appearance";
			public const string Header_Hotkeys = "Client Keybind";
			public const string Header_ThemeColor = "Theme Colors";
			public const string Desc_ThemeColor = "Adjust the hue to change all UI colors";
			public const string Header_Miscellaneous = "Advanced";

			public const string ThemeTabsHueFormat = "Tabs: {0}°";
			public const string ThemeBackgroundHueFormat = "Background: {0}°";
			public const string Toggle_DarkMode = "Dark Mode";
			public const string Toggle_NoBackground = "No Background";
			public const string LanguageLabel = "Language: English";

			public const string InfoOverlayTitle = "Info";

			public const string Header_VisibleCategories = "Visible Categories";
			public const string Desc_VisibleCategories = "Choose which tabs are visible in the client.";
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
{B}<b><color={ACCENT}>Render Distance:</color></b> Adjusts how far map objects are rendered. Works In-Game and in the Menu.

<b><color={WARN}>Warning:</color></b> Any perks that highlight items (e.g. <b>Inspired</b>), will only appear when you are close to them.
It is recommended to assign a <b>keybind</b> to quickly toggle render distance and temporarily restore full visibility for a few seconds.

{B}<b><color={ACCENT}>Disable Weather Effects:</color></b> Removes all weather effects (rain, snow, wind) and can optionally mute related audio. Can only be enabled from the menu or lobby.

<b><color={WARN}>Warning:</color></b> If you remain in cutscenes for too long, the weather will not disable.";

			public const string HSV = @"
<b>You can swap between {B}<b><color={ACCENT}>Outfit:</color></b> and {B}<b><color={ACCENT}>Icon:</color></b> HSV using the button above the preview.</b>

{B}<b><color={ACCENT}>Icon:</color></b> Customizes your icon color and select (based on your level) between 70 and 666. Use the button above the Swap button to cycle through icons.

{B}<b><color={ACCENT}>Outfit:</color></b> Adjusts the <b>HSV</b> (hue, saturation, value) of all player outfits at once.

<b><color={WARN}>Warning:</color></b> To drastically improve performance, Outfit scanning may take a bit longer on some maps.

{B}These are visual hue overlays rather than hard recolors, so certain colors may look slightly different than expected.

{B}<b>All changes are purely visual and not visible to other players.</b>";

			public const string Speedrun = @"
<b><color={DANGER}>Disclaimer</color></b>: Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be considered cheating. Power‑leveling or gaining unfair advantages with these features is cheating. These mods are intended for fun and legitimate speedrunning only, not to harm or oppress other players.

{B}<b><color={ACCENT}>Instant Interaction:</color></b> Removes all long interactions (revives, rituals, cages, etc).

{B}Make sure your configured <b>interact key</b> matches the in‑game interact key exactly, or the feature may not work.

{B}<b><color={ACCENT}>Attic Spawn:</color></b> Re‑creates the old Farmhouse bug where using Anna would spawn you in the attic. Only affects Farmhouse and is intended for speedruns.

{B}<b><color={ACCENT}>Auto Start:</color></b> Automatically starts matches from the menu/lobby, with a <b><color={ACCENT}>Start Delay</color></b> to wait before starting and an <b><color={ACCENT}>Arming Window</color></b> that keeps Auto Start active after a run ends.

<b><color={WARN}>Warning:</color></b> If the lobby takes too long to load (performance dependent), <b><color={ACCENT}>Auto Start</color></b> may trigger before the lobby is fully loaded, causing the mod to break. If this happens, simply rejoin the lobby or reload singleplayer if playing solo. The mod works in singleplayer and as host. If you are not the host, it will break and require the same fix above.";

			public const string FOV = @"
{B}When enabled, toggles the custom FOV override without touching your normal in‑game FOV setting.

{B}Allows values below <b><color={ACCENT}>60</color></b> and above <b><color={ACCENT}>95</color></b> without changing the in‑game FOV slider.

{B}When enabled, jumpscares and cutscenes also use your custom FOV.

{B}Using UV does not cause additional camera side‑effects.";

			public const string Settings = @"
{B}You can set a <b>custom hotkey</b> to open the client.

<b><color={WARN}>Note:</color></b> If you accidentally bind it to <b>Left Mouse Button</b>, you can change it again by using <b>Right Mouse</b> Button.

{B}<b><color={ACCENT}>Theme Colors:</color></b> Customize the Client accent color across <b>Tabs</b> and <b>Background</b>, including <b><color={ACCENT}>Dark Mode</color></b> which can be enabled for a darker theme, aswell as <b><color={ACCENT}>No Background</color></b> option whcih will completely remove the UI background.

{B}Under <b><color={ACCENT}>Visible Categories</color></b> you can enable or disable entire tabs so only the ones you care about remain visible.

{B}<b><color={ACCENT}>Language</color></b> swaps the language between English and Chinese.

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

{B}<b><color={ACCENT}>Remember Menu Music:</color></b> After restarting the game, the last chosen <b>menu music</b> will be used again.

{B}<b><color={ACCENT}>Disable Music in‑game:</color></b> Disables maps background music but keeps menu music.

{B}<b><color={ACCENT}>Mute Carnival Tunnel:</color></b> Mutes the blue spinning tunnel music in <b>The Carnival.</b>";
		}
	}
}