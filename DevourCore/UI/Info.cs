using System.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;

namespace DevourCore
{
    public class Info
    {
        private MelonPreferences_Entry<string> prefInfoOptimize;
        private MelonPreferences_Entry<string> prefInfoHSV;
        private MelonPreferences_Entry<string> prefInfoSpeedrun;
        private MelonPreferences_Entry<string> prefInfoFOV;
        private MelonPreferences_Entry<string> prefInfoSettings;
        private MelonPreferences_Entry<string> prefInfoAnticheat;
        private MelonPreferences_Entry<string> prefInfoMenu;

        private MelonPreferences_Category _prefs;

        private const string SECTION_OPTIMIZE = "Optimize";
        private const string SECTION_HSV = "HSV";
        private const string SECTION_SPEEDRUN = "Speedrun";
        private const string SECTION_FOV = "FOV";
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
        private const float CONTENT_TOP_GAP = 6f;
        private const float CONTENT_VPADDING = 16f;
        private const float BASE_BOTTOM_SAFE_EXTRA = 30f;
        private const float OVERLAY_TOP_EXTRA = 5f + 13f;

        private const string DEFAULT_ACCENT_HEX = "#64B5F6";
        private const string HEX_WARN = "#FFD54A";
        private const string HEX_DANGER = "#FF3A3A";
        private const string HEX_MUTED = "#B0BEC5";

        private const int FONT_DEFAULT = 13;
        private const int FONT_SPEEDRUN = 11;

        private bool _infoHoverIcon = false;
        private string _infoActiveKey = null;

        private string _accentHex = DEFAULT_ACCENT_HEX;

        private Texture2D infoIconBg, infoIconBgHover;
        private Texture2D infoOverlaySolidBg;

        private GUIStyle infoIconTextStyle;
        private GUIStyle infoOverlayStyle;

        public void Initialize(MelonPreferences_Category prefs)
        {
            _prefs = prefs;

            prefInfoOptimize = prefs.CreateEntry("InfoOptimize", "");
            prefInfoHSV = prefs.CreateEntry("InfoHSV", "");
            prefInfoSpeedrun = prefs.CreateEntry("InfoSpeedrun", "");
            prefInfoFOV = prefs.CreateEntry("InfoFOV", "");
            prefInfoSettings = prefs.CreateEntry("InfoSettings", "");
            prefInfoAnticheat = prefs.CreateEntry("InfoAnticheat", "");
            prefInfoMenu = prefs.CreateEntry("InfoMenu", "");

            ApplyInfoTexts();
        }

        public void InitializeStyles(Color themeColorPrimary, Color themeColorHover, GUIStyle headerStyle, GUIStyle descriptionStyle)
        {
            infoIconBg = MakeCircleTexture(INFO_ICON_SIZE, themeColorPrimary);
            infoIconBgHover = MakeCircleTexture(INFO_ICON_SIZE, themeColorHover);
            infoOverlaySolidBg = MakeRoundedTexture(128, 128, new Color(0.05f, 0.05f, 0.05f, 1f), 8);

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

            float x = guiArea.width - INFO_ICON_SIZE - INFO_MARGIN_RIGHT;
            float y = guiArea.height - INFO_ICON_SIZE - INFO_MARGIN_BOTTOM;
            Rect iconRect = new Rect(x, y, INFO_ICON_SIZE, INFO_ICON_SIZE);

            bool hover = iconRect.Contains(Event.current.mousePosition);
            Texture2D bg = hover ? infoIconBgHover : infoIconBg;
            GUI.DrawTexture(iconRect, bg, ScaleMode.StretchToFill, true);

            Rect iconTextRect = new Rect(iconRect.x + INFO_ICON_TEXT_X_OFFSET, iconRect.y, iconRect.width, iconRect.height);
            DrawOutlinedCenteredText(iconTextRect, "i", infoIconTextStyle, Color.white, new Color(0f, 0f, 0f, STROKE_ALPHA), 1);

            if (hover)
            {
                _infoHoverIcon = true;
                _infoActiveKey = infoKey;
            }

            if (_infoActiveKey != null && _infoHoverIcon)
            {
                DrawInfoOverlay(guiArea, tabHeight, headerStyle, descriptionStyle);
            }
        }

        private void DrawInfoOverlay(Rect guiArea, float tabHeight, GUIStyle headerStyle, GUIStyle descriptionStyle)
        {
            Rect overlayRect = GetOverlayRect(guiArea, tabHeight);
            GUI.Box(overlayRect, GUIContent.none, infoOverlayStyle);

            Rect titleBarRect = GetTitleBarRect(overlayRect);
            DrawTitle(titleBarRect, "Info", headerStyle);

            Rect contentRect = GetContentRect(overlayRect, titleBarRect);
            GUIStyle infoLabel = new GUIStyle(descriptionStyle)
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true,
                fontStyle = FontStyle.Normal,
                fontSize = (_infoActiveKey == SECTION_SPEEDRUN) ? FONT_SPEEDRUN : FONT_DEFAULT
            };

            GUI.Label(contentRect, GetInfoText(_infoActiveKey), infoLabel);
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
                case SECTION_OPTIMIZE: return prefInfoOptimize.Value;
                case SECTION_HSV: return prefInfoHSV.Value;
                case SECTION_SPEEDRUN: return prefInfoSpeedrun.Value;
                case SECTION_FOV: return prefInfoFOV.Value;
                case SECTION_SETTINGS: return prefInfoSettings.Value;
                case SECTION_ANTICHEAT: return prefInfoAnticheat.Value;
                case SECTION_MENU: return prefInfoMenu.Value;
                default: return prefInfoOptimize.Value;
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
            string b = $"<color={_accentHex}>•</color> ";

            // Optimize
            string optimizeInfo = ToRichText(
$@"{b}When using <b><color={_accentHex}>render distance</color></b> with perks that highlight items (e.g., <b><color={_accentHex}>Inspired</color></b>), the highlights will only render when you are close.
It is recommended to assign a <b><color={_accentHex}>keybind</color></b> to quickly toggle render distance, temporarily restoring full visibility for a few seconds when needed.

{b}The <b><color={_accentHex}>Disable Weather Effects</color></b> option can only be used in the menu or lobby to avoid breaking on maps.

<b><color={HEX_WARN}>Warning:</color></b> If <b><color={_accentHex}>Disable Weather Effects</color></b> mod is on and you stay in a cutscene for too long (>12s) the weather will not disable."
            );

            // HSV & Outfit
            string hsvInfo = ToRichText(
$@"<b>You can swap between Outfit and 666 Icon using the button above the preview.</b>

{b}<b><color={_accentHex}>666 Icon:</color></b> Customize the color for all level 666 icons including your own.

<b><color={HEX_WARN}>Warning:</color></b> This only works if your level is <b><color={_accentHex}>666</color></b>. Otherwise, nothing will change.

{b}<b><color={_accentHex}>Outfit:</color></b> Customize the <b><color={_accentHex}>HSV</color></b> of all player outfits at once.

<b><color={HEX_WARN}>Warning:</color></b> The <b><color={_accentHex}>Outfit</color></b> mod may cause noticeable lag or FPS drops depending on your PC. If it becomes too laggy feel free to disable the mod. To drastically improve performance, Outfit scanning might take a bit longer in some maps.

{b}These are visual hue overlays rather than hard recolors, so certain colors may look slightly different than expected.

{b}<b>All changes are purely visual and are not visible to other players.</b>"
            );

            // Speedrun
            string speedrunInfo = ToRichText(
$@"<b><color={HEX_DANGER}>Disclaimer</color></b>: Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be considered cheating. Exploiting these features for fast leveling or to gain unfair advantages is cheating. These mods are intended purely for fun and proper speedrunning, without harming or disadvantaging others.

{b}<b><color={_accentHex}>Instant Interaction:</color></b> Removes all long interactions (revives, rituals, cage, etc).

{b}Make sure the <b><color={_accentHex}>interact key</color></b> set matches your in-game interaction key to ensure proper functionality.

{b}<b><color={_accentHex}>Attic Spawn:</color></b> Brings old Farmhouse bug where using Anna would spawn you in the attic. This has been reintroduced solely for speedrunning and only affects Farmhouse.

{b}<b><color={_accentHex}>Auto Start:</color></b> Automatically starts the match when returning to the lobby. Can only be enabled from the menu/lobby to avoid breaking the mod from breaking.

 • <b><color={_accentHex}>Start Delay</color></b> how long to wait in the lobby before auto starting (tune based on your PC load time).
 
• <b><color={_accentHex}>Arming Window</color></b> how long Auto Start stays active after a run ends.

<b><color={HEX_WARN}>Warning:</color></b> If the lobby takes too long to load (performance dependent), the <b><color={_accentHex}>Auto Start</color></b> may trigger before the lobby is fully loaded, causing the mod to break. If this happens, simply rejoin the lobby or reload singleplayer if playing solo. This mod works in singleplayer and as host. If you are not the host, the mod will break and require the same fix above."
            );

            // FOV
            string fovInfo = ToRichText(
$@"{b}Allows values below <b><color={_accentHex}>60</color></b> and above <b><color={_accentHex}>95</color></b> without touching your in-game FOV settings.

{b}Jumpscares and cutscenes are affected if the mod is enabled.

{b}Using UV light does not interfere with the camera behaviour."
            );

            // Settings
            string settingsInfo = ToRichText(
$@"{b}You can assign a custom <b><color={_accentHex}>keybind</color></b> to open the client. This keybind is saved automatically.

<b><color={HEX_WARN}>Note:</color></b> If you accidentally set the keybind to Left Mouse Button you can change it again by using Right Mouse Button instead.

{b}You can <b><color={_accentHex}>Reset Client Settings</color></b> to its default state by pressing the button.

{b}The <b><color={_accentHex}>Theme Hue</color></b> slider changes the accent color of the entire client. Use it to match your preferred color scheme.

{b}Under <b><color={_accentHex}>Visible Categories</color></b> you can enable or disable entire tabs to only keep visible what you care about.

{b}If the GUI ever disappears off-screen, press <b><color={_accentHex}>F1</color></b> to re-center it."
            );

            // Anticheat
            string anticheatInfo = ToRichText(
$@"{b}<b><color={_accentHex}>Speed Anticheat</color></b> monitors all active players and estimates their <b>average movement speed</b> over time.

{b}If a player's vanilla movement speed consistently exceeds roughly, the Anticheat flags them as suspicious and shows an on-screen alert with their name with estimated average speed, and highlights them on the local overlay for the duration of the alert.

<b><color={HEX_WARN}>Note:</color></b> This mod is <b>informational only</b>. It <b>never kicks or bans</b> players, use it as a tool to help judge suspicious behaviour, not as proof. Alerts and tracking reset on scene change, or when using <b>Clear Alerts</b>.

{b}<b><color={_accentHex}>Clear Alerts</color></b> removes all current warnings and resets per-player alert flags so the same player can be flagged again if they stay above the threshold."
            );

            // Menu
            string menuInfo = ToRichText(
$@"{b}<b><color={_accentHex}>Menu Background:</color></b> Replaces the default menu background with any of the lobby ones.

<b><color={HEX_WARN}>Note:</color></b> While you are actually in a lobby, background selection buttons are shown but cannot be changed until you return to the main menu.

{b}<b><color={_accentHex}>Remember Last Music:</color></b> Saves the selected <b><color={_accentHex}>Menu Music:</color></b> even after restarting your game.

{b}<b><color={_accentHex}>Disable Music in-game:</color></b> Disables the Background Music of every map, but remains enabled in the Menu."


         
);

            prefInfoOptimize.Value = optimizeInfo;
            prefInfoHSV.Value = hsvInfo;
            prefInfoSpeedrun.Value = speedrunInfo;
            prefInfoFOV.Value = fovInfo;
            prefInfoSettings.Value = settingsInfo;
            prefInfoAnticheat.Value = anticheatInfo;
            prefInfoMenu.Value = menuInfo;

            _prefs?.SaveToFile(false);
        }

        private string ToRichText(string markdown)
        {
            try { return Regex.Replace(markdown, @"\*\*(.+?)\*\*", "<b>$1</b>"); }
            catch { return markdown; }
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