using UnityEngine;

namespace DevourCore
{
    public static class EN
    {
        public static class GUI
        {
            // Tab titles
            public const string Tab_Optimize = "Optimize";
            public const string Tab_HSV = "HSV";
            public const string Tab_Speedrun = "Speedrun";
            public const string Tab_FOV = "FOV";
            public const string Tab_Anticheat = "Anticheat";
            public const string Tab_Menu = "Menu";
            public const string Tab_Settings = "Settings";

            // Generic / shared
            public const string PressAnyKey = "Press any key...";
            public const string ToggleKeyFormat = "Toggle Key: {0}";
            public const string InteractKeyFormat = "Interact Key: {0}";
            public const string MenuKeyFormat = "Open Menu: {0}";
            public const string CurrentClipFormat = "Current: {0}";
            public const string AreYouSure = "Are you sure?";
            public const string ResetClientSettings = "Reset Client Settings";

            // Optimize tab
            public const string Header_Optimize = "Performance Optimizations";
            public const string Desc_Optimize = "Customize render distance and disable weather effects";
            public const string Header_Cull = "Render Distance";
            public const string Toggle_CullEnabled = "Enable Render Distance";
            public const string Toggle_CullInMenu = "Enable in Menu";
            public const string CullDistanceFormat = "Distance: {0} m";
            public const string Header_Weather = "Weather";
            public const string Toggle_DisableWeather = "Disable Weather Effects";

            // HSV / Outfit tab
            public const string Header_OutfitColor = "Outfit Color";
            public const string Header_IconColor = "666 Icon Color";
            public const string Desc_OutfitColor = "Change the color of all player outfits";
            public const string Desc_IconColor = "Customize the color of the 666 icon";
            public const string Toggle_IconEnabled = "Enable 666 Icon";
            public const string Toggle_OutfitEnabled = "Enable Outfit";
            public const string Swap_ToOutfit = "Outfit ↔";
            public const string Swap_ToIcon = "666 ↔";
            public const string Header_Preview = "Preview";

            // Speedrun tab
            public const string Header_Speedrun = "Speedrunning Mods";
            public const string Desc_Speedrun = "Useful features for speedrun categories";
            public const string Toggle_InstantInteract = "Instant Interaction";
            public const string Toggle_AtticSpawn = "Attic Spawn";
            public const string Header_AutoStart = "Auto Start";
            public const string Desc_AutoStart = "Automatically start game when returning to lobby";
            public const string Toggle_ForceStart = "Enable Auto Start";
            public const string Toggle_UseArm = "Enable Arming Window";
            public const string ForceStartDelayFormat = "Start Delay: {0:F1}s";
            public const string ForceStartArmFormat = "Arming Window: {0:F1} min";
            public const string SpeedrunPopupBody =
                "Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be seen as cheating. Exploiting these features to gain levels or unfair advantages is considered cheating. These mods are intended purely for fun and speedrunning purposes, without causing harm or disadvantages to others. Use with caution.";
            public const string SpeedrunPopupConfirm = "Understood";

            // FOV tab
            public const string Header_FOV = "Custom FOV";
            public const string Desc_FOV = "Customize your field of view over the normal limit";
            public const string Toggle_FOVEnabled = "Enable Custom FOV";
            public const string FOVValueFormat = "FOV: {0}°";

            // Anticheat tab
            public const string Header_Anticheat = "Speed Anticheat";
            public const string Desc_Anticheat = "Monitor players for suspicious movement speed";
            public const string Toggle_AnticheatEnabled = "Enable Speed Detection";
            public const string AlertDurationFormat = "Alert Duration: {0:F1}s";
            public const string Button_ClearAlerts = "Clear Alerts";
            public const string AnticheatStatusFormat = "Tracking {0} players | {1} active alerts";

            // Menu tab
            public const string Header_Menu = "Menu Customization";
            public const string Desc_Menu = "Customize menu background and music settings";
            public const string Header_MenuBackground = "Menu Background";
            public const string Toggle_CustomBackground = "Enable Custom Background";
            public const string InLobby_NoBgChange = "Background selection is disabled while you are in a lobby.";
            public const string Header_MusicSettings = "Music Settings";
            public const string Toggle_DisableIngameMusic = "Disable Music in-game";
            public const string Toggle_MuteTunnel = "Mute Tunnel (Carnival)";
            public const string Toggle_RememberMusic = "Remember Menu Music";

            // Settings tab
            public const string Header_Settings = "Client Settings";
            public const string Desc_Settings = "Customize menu keybind and appearance";
            public const string Header_Hotkeys = "Menu Keybind";
            public const string Header_ThemeColor = "Theme Color";
            public const string Desc_ThemeColor = "Adjust the hue to change all UI colors";
            public const string Header_Miscellaneous = "Miscellaneous";

            // Info overlay title
            public const string InfoOverlayTitle = "Info";

            // Visible categories (for Tabs)
            public const string Header_VisibleCategories = "Visible Categories";
            public const string Desc_VisibleCategories = "Choose which tabs to show in the client.";
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
{B}When using <b><color={ACCENT}>render distance</color></b> with perks that highlight items (e.g. <b><color={ACCENT}>Inspired</color></b>), highlights only appear when you are close.
It is recommended to assign a <b><color={ACCENT}>keybind</color></b> to quickly toggle render distance and temporarily restore full visibility for a few seconds.

{B}The <b><color={ACCENT}>Disable Weather Effects</color></b> option should only be used in the menu or lobby to avoid issues on maps.

<b><color={WARN}>Warning:</color></b> If <b><color={ACCENT}>Disable Weather Effects</color></b> stays enabled and you remain in cutscenes for too long (>12s), weather may fail to disable correctly.";

            public const string HSV = @"
<b>You can swap between Outfit and 666 Icon using the button above the preview.</b>

{B}<b><color={ACCENT}>666 Icon:</color></b> Customizes the color of all level 666 icons, including your own.

<b><color={WARN}>Warning:</color></b> This only works if your level is <b><color={ACCENT}>666</color></b>. Otherwise, nothing will change.

{B}<b><color={ACCENT}>Outfit:</color></b> Adjusts the <b><color={ACCENT}>HSV</color></b> (hue, saturation, value) of all player outfits at once.

<b><color={WARN}>Warning:</color></b> The <b><color={ACCENT}>Outfit</color></b> mod may cause noticeable lag or FPS drops depending on your PC. If it becomes too laggy, disable the mod. To drastically improve performance, Outfit scanning may take a bit longer on some maps.

{B}These are visual hue overlays rather than hard recolors, so certain colors may look slightly different than expected.

{B}<b>All changes are purely visual and not visible to other players.</b>";

            public const string Speedrun = @"
<b><color={DANGER}>Disclaimer</color></b>: Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be considered cheating. Power‑leveling or gaining unfair advantages with these features is cheating. These mods are intended for fun and legitimate speedrunning only, not to harm or oppress other players.

{B}<b><color={ACCENT}>Instant Interaction:</color></b> Removes all long interactions (revives, rituals, cages, etc.).

{B}Make sure your configured <b><color={ACCENT}>interact key</color></b> matches the in‑game interact key exactly, or the feature may not work.

{B}<b><color={ACCENT}>Attic Spawn:</color></b> Re‑creates the old Farmhouse bug where using Anna would spawn you in the attic. Only affects Farmhouse and is intended for speedruns.

{B}<b><color={ACCENT}>Auto Start:</color></b> Automatically starts the match when returning to the lobby. Can only be enabled from the menu/lobby to avoid breaking the mod.

 • <b><color={ACCENT}>Start Delay</color></b>: How long to wait in the lobby before auto starting (tune based on your PC load time).
 
• <b><color={ACCENT}>Arming Window</color></b>: How long Auto Start stays armed after a run ends.

<b><color={WARN}>Warning:</color></b> If the lobby takes too long to load (performance dependent), <b><color={ACCENT}>Auto Start</color></b> may trigger before the lobby is fully loaded, causing the mod to break. If this happens, simply rejoin the lobby or reload singleplayer if playing solo. The mod works in singleplayer and as host. If you are not the host, it will break and require the same fix above.";

            public const string FOV = @"
{B}Allows values below <b><color={ACCENT}>60</color></b> and above <b><color={ACCENT}>95</color></b> without changing the in‑game FOV slider.

{B}When enabled, jumpscares and cutscenes also use your custom FOV.

{B}Using UV does not cause additional camera side‑effects.";

            public const string Settings = @"
{B}You can set a custom <b><color={ACCENT}>hotkey</color></b> to open the client. This key is saved automatically.

<b><color={WARN}>Note:</color></b> If you accidentally bind it to Left Mouse Button, you can change it again by using Right Mouse Button.

{B}The <b><color={ACCENT}>Theme Hue</color></b> slider changes the accent color of the entire client. Use it to match your preferred color scheme.

{B}Under <b><color={ACCENT}>Visible Categories</color></b> you can enable or disable entire tabs so only the ones you care about remain visible.

{B}<b><color={ACCENT}>Language</color></b> swaps the language between English and Chinese.

{B}<b><color={ACCENT}>Reset Client Settings</color></b> restores all settings to their default values.

{B}If the GUI ever moves off-screen, press <b><color={ACCENT}>F1</color></b> to re-center it.";

            public const string Anticheat = @"
{B}<b><color={ACCENT}>Speed Anticheat</color></b> continuously monitors all active players and estimates their <b>average movement speed</b>.

{B}If a player's vanilla movement speed remains clearly above the threshold for an extended period, the module flags them as suspicious, shows an on‑screen alert with their name and average speed, and highlights them locally for the duration of the alert.

<b><color={WARN}>Note:</color></b> This module is <b>informational only</b>. It <b>never kicks or bans</b> players; use it as a tool to help judge suspicious behaviour, not as proof. Alerts and tracking reset on scene changes or when you use <b>Clear Alerts</b>.

{B}<b><color={ACCENT}>Clear Alerts</color></b> removes all current warnings and resets per‑player alert flags so the same player can be flagged again if they remain above the threshold.";

            public const string Menu = @"
{B}<b><color={ACCENT}>Menu Background:</color></b> Replaces the default menu background with any of the lobby environments.

<b><color={WARN}>Note:</color></b> While you are in a lobby, background selection buttons remain visible but cannot be changed until you return to the main menu.

{B}<b><color={ACCENT}>Disable Music in‑game:</color></b> Disables map background music but keeps menu music.

{B}<b><color={ACCENT}>Mute Tunnel (Carnival):</color></b> Mutes only the blue spinning tunnel music in Carnival.

{B}<b><color={ACCENT}>Remember Last Music:</color></b> After restarting the game, the last chosen <b><color={ACCENT}>menu music</color></b> will be used again.";
        }
    }
}