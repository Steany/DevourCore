using MelonLoader;
using UnityEngine;

namespace DevourCore
{
    public class Tabs
    {
        private MelonPreferences_Entry<bool> prefShowOptimize;
        private MelonPreferences_Entry<bool> prefShowHSV;
        private MelonPreferences_Entry<bool> prefShowSpeedrun;
        private MelonPreferences_Entry<bool> prefShowFOV;
        private MelonPreferences_Entry<bool> prefShowAnticheat;
        private MelonPreferences_Entry<bool> prefShowMenu;

        private MelonPreferences_Category prefs;

        public bool ShowOptimize => prefShowOptimize.Value;
        public bool ShowHSV => prefShowHSV.Value;
        public bool ShowSpeedrun => prefShowSpeedrun.Value;
        public bool ShowFOV => prefShowFOV.Value;
        public bool ShowAnticheat => prefShowAnticheat.Value;
        public bool ShowMenu => prefShowMenu.Value;

        private const float BUTTON_WIDTH = 105f;
        private const float STROKE_ALPHA = 0.6f;

        public void Initialize(MelonPreferences_Category prefsCategory)
        {
            prefs = prefsCategory;

            prefShowOptimize = prefs.CreateEntry("ShowOptimizeTab", true);
            prefShowHSV = prefs.CreateEntry("ShowHSVTab", true);
            prefShowSpeedrun = prefs.CreateEntry("ShowSpeedrunTab", true);
            prefShowFOV = prefs.CreateEntry("ShowFOVTab", true);
            prefShowAnticheat = prefs.CreateEntry("ShowAnticheatTab", true);
            prefShowMenu = prefs.CreateEntry("ShowMenuTab", true);
        }

        public void SetOptimizeVisible(bool visible) { prefShowOptimize.Value = visible; prefs.SaveToFile(false); }
        public void SetHSVVisible(bool visible) { prefShowHSV.Value = visible; prefs.SaveToFile(false); }
        public void SetSpeedrunVisible(bool visible) { prefShowSpeedrun.Value = visible; prefs.SaveToFile(false); }
        public void SetFOVVisible(bool visible) { prefShowFOV.Value = visible; prefs.SaveToFile(false); }
        public void SetAnticheatVisible(bool visible) { prefShowAnticheat.Value = visible; prefs.SaveToFile(false); }
        public void SetMenuVisible(bool visible) { prefShowMenu.Value = visible; prefs.SaveToFile(false); }

        public int GetVisibleTabCount()
        {
            int count = 1;
            if (ShowOptimize) count++;
            if (ShowHSV) count++;
            if (ShowSpeedrun) count++;
            if (ShowFOV) count++;
            if (ShowAnticheat) count++;
            if (ShowMenu) count++;
            return count;
        }

        public bool IsTabVisible(int tabIndex)
        {
            switch (tabIndex)
            {
                case 0: return ShowOptimize;
                case 1: return ShowHSV;
                case 2: return ShowSpeedrun;
                case 3: return ShowFOV;
                case 4: return ShowAnticheat;
                case 5: return ShowMenu;
                case 6: return true;
                default: return false;
            }
        }

        public void DrawVisibilityUI(
            ref int selectedTab,
            GUIStyle headerStyle,
            GUIStyle descriptionStyle,
            GUIStyle checkboxStyle,
            GUIStyle labelStyle,
            Color themeColor,
            GUIStyle tabActiveStyle,
            GUIStyle tabInactiveStyle,
            GUIStyle tabTitleStyle)
        {
            GUILayout.Space(20);

            DrawHeader(Loc.GUI.Header_VisibleCategories, headerStyle, themeColor);
            GUILayout.Label(Loc.GUI.Desc_VisibleCategories, descriptionStyle);

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (DrawToggleButton(Loc.Tabs.Optimize, ShowOptimize, tabInactiveStyle, tabTitleStyle))
            {
                SetOptimizeVisible(!ShowOptimize);
                if (!ShowOptimize && selectedTab == 0) selectedTab = 6;
            }

            GUILayout.Space(10);

            if (DrawToggleButton(Loc.Tabs.HSV, ShowHSV, tabInactiveStyle, tabTitleStyle))
            {
                SetHSVVisible(!ShowHSV);
                if (!ShowHSV && selectedTab == 1) selectedTab = 6;
            }

            GUILayout.Space(10);

            if (DrawToggleButton(Loc.Tabs.Speedrun, ShowSpeedrun, tabInactiveStyle, tabTitleStyle))
            {
                SetSpeedrunVisible(!ShowSpeedrun);
                if (!ShowSpeedrun && selectedTab == 2) selectedTab = 6;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (DrawToggleButton(Loc.Tabs.FOV, ShowFOV, tabInactiveStyle, tabTitleStyle))
            {
                SetFOVVisible(!ShowFOV);
                if (!ShowFOV && selectedTab == 3) selectedTab = 6;
            }

            GUILayout.Space(10);

            if (DrawToggleButton(Loc.Tabs.Anticheat, ShowAnticheat, tabInactiveStyle, tabTitleStyle))
            {
                SetAnticheatVisible(!ShowAnticheat);
                if (!ShowAnticheat && selectedTab == 4) selectedTab = 6;
            }

            GUILayout.Space(10);

            if (DrawToggleButton(Loc.Tabs.Menu, ShowMenu, tabInactiveStyle, tabTitleStyle))
            {
                SetMenuVisible(!ShowMenu);
                if (!ShowMenu && selectedTab == 5) selectedTab = 6;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private bool DrawToggleButton(string label, bool isOn, GUIStyle buttonStyle, GUIStyle textStyle)
        {
            string symbol = isOn ? "✓" : "✗";
            Color symbolColor = isOn ? new Color(0.4f, 1f, 0.4f) : new Color(1f, 0.4f, 0.4f);

            bool clicked = GUILayout.Button(GUIContent.none, buttonStyle,
                GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(24));

            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);

            DrawButtonTextWithColoredSymbol(rInset, symbol, label, textStyle, symbolColor);

            return clicked;
        }

        private void DrawButtonTextWithColoredSymbol(Rect rect, string symbol, string label, GUIStyle style, Color symbolColor)
        {
            var s = new GUIStyle(style)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, STROKE_ALPHA);
            string fullText = $"{symbol} {label}";
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    GUI.Label(new Rect(rect.x + dx, rect.y + dy, rect.width, rect.height), fullText, s);
                }
            }
            GUI.color = prev;

            string colorHex = ColorUtility.ToHtmlStringRGB(symbolColor);
            string coloredText = $"<color=#{colorHex}>{symbol}</color> {label}";
            s.normal.textColor = Color.white;
            GUI.Label(rect, coloredText, s);
        }

        private void DrawHeader(string text, GUIStyle headerStyle, Color themeColor)
        {
            GUIContent content = new GUIContent(text);
            Vector2 size = headerStyle.CalcSize(content);
            Rect rect = GUILayoutUtility.GetRect(size.x + 40f, size.y + 8f, GUILayout.ExpandWidth(true));
            Rect textRect = new Rect(rect.x + (rect.width - size.x) / 2f, rect.y, size.x, rect.height);

            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.8f);
            GUI.Label(new Rect(textRect.x + 1f, textRect.y + 1f, textRect.width, textRect.height), content, headerStyle);
            GUI.color = prev;

            GUI.Label(textRect, content, headerStyle);
        }
    }
}