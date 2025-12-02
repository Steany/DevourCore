using MelonLoader;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.Globalization.HebrewNumber;

namespace DevourCore
{
    public class GUIManager
    {
        private bool _showGui = false;
        private Rect _guiArea = new Rect(10, 10, 390, 440);
        private int _selectedTab = 6;
        private bool centerGuiOnFirstOpen = false;
        private bool hasCenteredGuiOnce = false;

        private const float FULL_WIN_WIDTH = 530f;
        private const float REDUCED_WIN_WIDTH = 460f;
        private const float SMALL_WIN_WIDTH = 390f;

        private const float BASE_WIN_HEIGHT = 465f;
        private const float HSV_WIN_HEIGHT = 480f;
        private const float SPEEDRUN_WIN_HEIGHT = 505f;
        private const float FOV_WIN_HEIGHT = 315f;
        private const float ANTICHEAT_WIN_HEIGHT = 380f;
        private const float MENU_WIN_HEIGHT = 520f;
        private const float SETTINGS_WIN_HEIGHT = 620f;
        private const float TAB_HEIGHT = 24f;
        private const float STROKE_ALPHA = 0.6f;

        private const float MAP_BUTTON_HEIGHT = 24f;
        private const float MAP_BUTTON_SPACING = 8f;

        private const float SLIDER_FIXED_WIDTH = 340f;

        private bool _cursorStateSaved = false;
        private bool _prevCursorVisible;
        private CursorLockMode _prevCursorLockState;
        private const bool CURSOR_CONFINED_WHEN_GUI = false;

        private bool isEditingOutfit = false;
        private MelonPreferences_Entry<bool> prefIsEditingOutfit;

        private bool hasShownSpeedrunPopup = false;
        private bool isSpeedrunPopupActive = false;

        private bool hasShownOutfitPopup = false;
        private bool isOutfitPopupActive = false;

        private GameObject persistentMessageModal = null;

        private GUIStyle windowStyle;
        private GUIStyle headerStyle;
        private GUIStyle tabActiveStyle;
        private GUIStyle tabInactiveStyle;
        private GUIStyle tabTitleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle toggleStyle;
        private GUIStyle verticalSliderStyle;
        private GUIStyle verticalSliderThumbStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle checkboxStyle;
        private GUIStyle checkboxDisabledStyle;
        private Texture2D blackBg;
        private Texture2D purpleBg;
        private Texture2D darkPurpleBg;
        private Texture2D purpleHoverBg;
        private Texture2D checkboxBg;
        private Texture2D checkboxCheckedBg;
        private Texture2D checkboxDisabledBg;
        private Texture2D checkboxCheckedDisabledBg;
        private Texture2D sliderBg;
        private Texture2D sliderFillBg;
        private bool stylesInitialized = false;

        private MelonPreferences_Entry<float> prefGuiX, prefGuiY;

        private Optimize optimizeTab;
        private Icon hsvTab;
        private Outfit outfitTab;
        private Speedrun speedrunTab;
        private FOV fovTab;
        private Settings settingsTab;
        private Anticheat anticheatTab;
        private Menu menuTab;
        private Info infoManager;
        private Tabs tabVisibility;
        private MelonPreferences_Category prefs;

        private bool resetConfirmPending = false;

        public bool ShowGui => _showGui;

        public void Initialize(
            MelonPreferences_Category prefsCategory,
            Optimize optTab,
            Icon hTab,
            Outfit oTab,
            Speedrun sTab,
            FOV fTab,
            Settings setTab,
            Anticheat acTab,
            Menu menTab,
            Info info)
        {
            prefs = prefsCategory;
            optimizeTab = optTab;
            hsvTab = hTab;
            outfitTab = oTab;
            speedrunTab = sTab;
            fovTab = fTab;
            settingsTab = setTab;
            anticheatTab = acTab;
            menuTab = menTab;
            infoManager = info;

            tabVisibility = new Tabs();
            tabVisibility.Initialize(prefs);

            prefGuiX = prefs.CreateEntry("GuiX", 1376f);
            prefGuiY = prefs.CreateEntry("GuiY", 87f);
            prefIsEditingOutfit = prefs.CreateEntry("IsEditingOutfit", false);

            _guiArea.x = prefGuiX.Value;
            _guiArea.y = prefGuiY.Value;
            _guiArea.width = CalculateWindowWidth();
            isEditingOutfit = prefIsEditingOutfit.Value;
        }

        public void SetCenterOnFirstOpen(bool center)
        {
            centerGuiOnFirstOpen = center;
        }

        public void ToggleGui()
        {
            bool wasShowing = _showGui;
            _showGui = !_showGui;

            SetGuiCursor(_showGui);

            if (_showGui && !hasCenteredGuiOnce && centerGuiOnFirstOpen)
            {
                CenterGuiWindow();
                hasCenteredGuiOnce = true;
                centerGuiOnFirstOpen = false;
            }

            if (wasShowing && !_showGui)
            {
                hasShownSpeedrunPopup = false;
                hasShownOutfitPopup = false;
                resetConfirmPending = false;
            }
        }

        private int GetVisibleTabCount()
        {
            int count = 1;
            if (tabVisibility.ShowOptimize) count++;
            if (tabVisibility.ShowHSV) count++;
            if (tabVisibility.ShowSpeedrun) count++;
            if (tabVisibility.ShowFOV) count++;
            if (tabVisibility.ShowAnticheat) count++;
            if (tabVisibility.ShowMenu) count++;
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

            x = Mathf.Clamp(x, 10f, Mathf.Max(10f, Screen.width - width - 10f));
            y = Mathf.Clamp(y, 10f, Mathf.Max(10f, Screen.height - height - 10f));

            _guiArea.x = x;
            _guiArea.y = y;
            _guiArea.width = width;

            prefGuiX.Value = _guiArea.x;
            prefGuiY.Value = _guiArea.y;
            prefs.SaveToFile(false);
        }

        public void OnGUI(bool inMenu, bool inValidMap)
        {
            if (!_showGui) return;

            EnforceGuiCursor();

            if (!stylesInitialized || windowStyle == null)
                InitializeStyles();

            float targetH = BASE_WIN_HEIGHT;
            if (_selectedTab == 1) targetH = HSV_WIN_HEIGHT;
            else if (_selectedTab == 2) targetH = SPEEDRUN_WIN_HEIGHT;
            else if (_selectedTab == 3) targetH = FOV_WIN_HEIGHT;
            else if (_selectedTab == 4) targetH = ANTICHEAT_WIN_HEIGHT;
            else if (_selectedTab == 5) targetH = MENU_WIN_HEIGHT;
            else if (_selectedTab == 6) targetH = SETTINGS_WIN_HEIGHT;

            float targetW = CalculateWindowWidth();

            float previousHeight = _guiArea.height;
            float previousWidth = _guiArea.width;

            _guiArea.height = targetH;
            _guiArea.width = targetW;

            if (!Mathf.Approximately(previousHeight, targetH))
                _guiArea.y = Mathf.Clamp(_guiArea.y, 10f, Mathf.Max(10f, Screen.height - _guiArea.height - 10f));

            if (!Mathf.Approximately(previousWidth, targetW))
                _guiArea.x = Mathf.Clamp(_guiArea.x, 10f, Mathf.Max(10f, Screen.width - _guiArea.width - 10f));

            _guiArea = GUI.Window(
                12345,
                _guiArea,
                new Action<int>((id) => DrawMainWindow(id, inMenu, inValidMap)),
                "",
                windowStyle);

            _guiArea.x = Mathf.Clamp(_guiArea.x, 0f, Mathf.Max(0f, Screen.width - _guiArea.width));
            _guiArea.y = Mathf.Clamp(_guiArea.y, 0f, Mathf.Max(0f, Screen.height - _guiArea.height));
        }

        private void DrawMainWindow(int windowID, bool inMenu, bool inValidMap)
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (tabVisibility.ShowOptimize && DrawTabButton("Optimize", 0)) { _selectedTab = 0; resetConfirmPending = false; }
            if (tabVisibility.ShowHSV && DrawTabButton("HSV", 1)) { _selectedTab = 1; resetConfirmPending = false; }
            if (tabVisibility.ShowSpeedrun && DrawTabButton("Speedrun", 2)) { _selectedTab = 2; resetConfirmPending = false; }
            if (tabVisibility.ShowFOV && DrawTabButton("FOV", 3)) { _selectedTab = 3; resetConfirmPending = false; }
            if (tabVisibility.ShowAnticheat && DrawTabButton("Anticheat", 4)) { _selectedTab = 4; resetConfirmPending = false; }
            if (tabVisibility.ShowMenu && DrawTabButton("Menu", 5)) { _selectedTab = 5; resetConfirmPending = false; }
            if (DrawTabButton("Settings", 6)) { _selectedTab = 6; resetConfirmPending = false; }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            switch (_selectedTab)
            {
                case 0: DrawOptimizationTab(inMenu, inValidMap); break;
                case 1: DrawHSVTab(); break;
                case 2: DrawSpeedrunTab(inMenu); break;
                case 3: DrawFOVTab(inValidMap); break;
                case 4: DrawAnticheatTab(); break;
                case 5: DrawMenuTab(); break;
                case 6: DrawSettingsTab(); break;
            }

            infoManager.DrawInfoBox(
                _guiArea,
                TAB_HEIGHT,
                GetInfoKeyForSelectedTab(),
                headerStyle,
                descriptionStyle);

            prefGuiX.Value = _guiArea.x;
            prefGuiY.Value = _guiArea.y;
            prefs.SaveToFile(false);

            GUI.DragWindow();
        }

        private string GetInfoKeyForSelectedTab()
        {
            switch (_selectedTab)
            {
                case 0: return "Optimize";
                case 1: return "HSV";
                case 2: return "Speedrun";
                case 3: return "FOV";
                case 4: return "Anticheat";
                case 5: return "Menu";
                case 6: return "Settings";
            }
            return "Optimize";
        }

        private bool DrawTabButton(string title, int index)
        {
            bool active = (_selectedTab == index);
            bool clicked = GUILayout.Button(GUIContent.none,
                active ? tabActiveStyle : tabInactiveStyle, GUILayout.Height(TAB_HEIGHT));
            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f, r.width - 2f, r.height - 2f);
            DrawOutlinedCenteredText(rInset, title, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            return clicked;
        }

        private void DrawOptimizationTab(bool inMenu, bool inValidMap)
        {
            DrawHeader("Performance Optimizations");
            GUILayout.Label("Customize render distance and disable weather effects", descriptionStyle);
            GUILayout.Space(10);

            DrawHeader("Render Distance");
            GUILayout.Space(10);

            bool newCullEnabled = DrawCustomToggle(optimizeTab.CullEnabled, "Enable Render Distance");
            if (newCullEnabled != optimizeTab.CullEnabled)
                optimizeTab.SetCullEnabled(newCullEnabled, prefs);

            GUILayout.Space(5);

            bool newApplyInMenu = DrawCustomToggle(optimizeTab.ApplyInMenu, "Enable in Menu");
            if (newApplyInMenu != optimizeTab.ApplyInMenu)
                optimizeTab.SetApplyInMenu(newApplyInMenu, prefs);

            GUILayout.Space(10);

            DrawCenteredLabelAboveSlider($"Distance: {Mathf.RoundToInt(optimizeTab.CullRadius)} m");
            float newCull = DrawCustomSlider(optimizeTab.CullRadius, 6f, 30f);
            if (!Mathf.Approximately(newCull, optimizeTab.CullRadius))
                optimizeTab.SetCullRadius(newCull, prefs);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string cullKeyText =
                optimizeTab.IsCapturingCullKey ? "Press any key..." : $"Toggle Key: {optimizeTab.CullToggleKey}";
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(200)))
                optimizeTab.StartCapturingKey();
            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f,
                r.width - 2f, r.height - 2f);
            DrawOutlinedCenteredText(rInset, cullKeyText, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            DrawHeader("Weather");
            GUILayout.Space(10);

            GUI.enabled = inMenu;

            bool newWeather = DrawCustomToggle(optimizeTab.DisableWeather, "Disable Weather Effects");
            if (newWeather != optimizeTab.DisableWeather && inMenu)
                optimizeTab.SetWeatherDisabled(newWeather, prefs, inMenu, inValidMap);

            GUI.enabled = true;
        }

        private void DrawHSVTab()
        {
            DrawHeader(isEditingOutfit ? "Outfit Color" : "666 Icon Color");
            GUILayout.Label(
                isEditingOutfit
                    ? "Change the color of all player outfits"
                    : "Customize the color of the 666 icon",
                descriptionStyle);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();

            bool new666Enabled = DrawCustomToggle(hsvTab.HsvModEnabled, "Enable 666 Icon");
            if (new666Enabled != hsvTab.HsvModEnabled)
                hsvTab.SetEnabled(new666Enabled, prefs);

            GUILayout.Space(5);

            bool newOutfitEnabled = DrawCustomToggle(outfitTab.OutfitModEnabled, "Enable Outfit");
            if (newOutfitEnabled != outfitTab.OutfitModEnabled)
            {
                outfitTab.SetEnabled(newOutfitEnabled, prefs);
            }

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
            float leftSpacing;
            float betweenSliders;

            if (windowWidth >= REDUCED_WIN_WIDTH)
            {
                leftSpacing = 60f;
                betweenSliders = 54f;
            }
            else
            {
                leftSpacing = 35f;
                betweenSliders = 40f;
            }

            GUILayout.Space(leftSpacing);

            float newHue = DrawCustomVerticalSlider(currentHue, 0f, 1f, "H");
            GUILayout.Space(betweenSliders);
            float newSat = DrawCustomVerticalSlider(currentSat, 0f, 1f, "S");
            GUILayout.Space(betweenSliders);
            float newVal = DrawCustomVerticalSlider(currentVal, 0f, 1f, "V");
            GUILayout.Space(40);

            GUILayout.BeginVertical();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string swapText = isEditingOutfit ? "Outfit ↔" : "666 ↔";
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(65), GUILayout.Height(22)))
            {
                isEditingOutfit = !isEditingOutfit;
                prefIsEditingOutfit.Value = isEditingOutfit;
                prefs.SaveToFile(false);
            }
            Rect swapRect = GUILayoutUtility.GetLastRect();
            Rect swapInset = new Rect(swapRect.x + 1f, swapRect.y + 1f,
                swapRect.width - 2f, swapRect.height - 2f);
            DrawOutlinedCenteredText(swapInset, swapText, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(3);
            DrawHeader("Preview");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (previewTex != null)
            {
                Rect previewRect = GUILayoutUtility.GetRect(60, 60,
                    GUILayout.Width(60), GUILayout.Height(60));
                GUI.DrawTexture(new Rect(previewRect.x - 2, previewRect.y - 2,
                        previewRect.width + 4, previewRect.height + 4),
                    purpleBg);
                GUI.DrawTexture(previewRect, previewTex, ScaleMode.StretchToFill);
            }
            else
            {
                GUILayout.Box("", GUILayout.Width(60), GUILayout.Height(60));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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
            DrawHeader("Speedrunning Mods");
            GUILayout.Label("Useful features for speedrun categories", descriptionStyle);
            GUILayout.Space(10);

            bool newLong = DrawCustomToggle(speedrunTab.DisableLongInteract, "Instant Interaction");
            if (newLong != speedrunTab.DisableLongInteract)
            {
                if (newLong && !speedrunTab.DisableLongInteract)
                    TriggerSpeedrunPopup();

                speedrunTab.SetDisableLongInteract(newLong, prefs);
            }

            GUILayout.Space(5);

            bool newAttic = DrawCustomToggle(speedrunTab.EnableAtticSpawn, "Attic Spawn");
            if (newAttic != speedrunTab.EnableAtticSpawn)
            {
                if (newAttic && !speedrunTab.EnableAtticSpawn)
                    TriggerSpeedrunPopup();

                speedrunTab.SetEnableAtticSpawn(newAttic, prefs);
            }

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string keyText =
                speedrunTab.IsCapturingInteractKey
                    ? "Press any key..."
                    : $"Interact Key: {speedrunTab.CustomInteractKey}";
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(200)))
                speedrunTab.StartCapturingInteractKey();
            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f,
                r.width - 2f, r.height - 2f);
            DrawOutlinedCenteredText(rInset, keyText, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            DrawHeader("Auto Start");
            GUILayout.Label("Automatically start game when returning to lobby", descriptionStyle);
            GUILayout.Space(10);

            GUI.enabled = inMenu;

            bool newForceStart = DrawCustomToggle(speedrunTab.ForceStartEnabled, "Enable Auto Start");
            if (newForceStart != speedrunTab.ForceStartEnabled && inMenu)
            {
                if (newForceStart && !speedrunTab.ForceStartEnabled)
                    TriggerSpeedrunPopup();

                speedrunTab.SetForceStartEnabled(newForceStart, prefs, inMenu);
            }

            GUILayout.Space(5);

            bool newUseArm = DrawCustomToggle(speedrunTab.UseArmingWindow, "Enable Arming Window");
            if (newUseArm != speedrunTab.UseArmingWindow && inMenu)
                speedrunTab.SetUseArmingWindow(newUseArm, prefs);

            GUI.enabled = true;

            GUILayout.Space(10);

            DrawCenteredLabelAboveSlider($"Start Delay: {speedrunTab.ForceStartDelay:F1}s");
            float newDelay = DrawCustomSlider(speedrunTab.ForceStartDelay, 0.7f, 1.5f);
            if (!Mathf.Approximately(newDelay, speedrunTab.ForceStartDelay))
                speedrunTab.SetForceStartDelay(newDelay);

            GUILayout.Space(6);

            DrawCenteredLabelAboveSlider($"Arming Window: {speedrunTab.ForceStartArmMinutes:F1} min");
            float newArm = DrawCustomSlider(speedrunTab.ForceStartArmMinutes, 2f, 10f);
            newArm = Mathf.Round(newArm * 10f) / 10f;
            if (!Mathf.Approximately(newArm, speedrunTab.ForceStartArmMinutes))
                speedrunTab.SetForceStartArmMinutes(newArm);
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

            if (persistentMessageModal != null)
            {
                messageModal = persistentMessageModal;
            }
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
                    allTexts[0].text =
                        "Please use these modifications responsibly. Avoid using them in casual matches with players you do not know, as they may be seen as cheating. Exploiting these features to gain levels or unfair advantages is considered cheating. These mods are intended purely for fun and speedrunning purposes, without causing harm or disadvantages to others. Use with caution.";
                    allTexts[1].text = "Understood";

                    messageModal.SetActive(true);

                    CanvasGroup canvasGroup = messageModal.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                        canvasGroup.interactable = true;
                        canvasGroup.blocksRaycasts = true;
                    }
                }
            }

            isSpeedrunPopupActive = false;
        }

        private void DrawFOVTab(bool inValidMap)
        {
            DrawHeader("Custom FOV");
            GUILayout.Label("Customize your field of view over the normal limit", descriptionStyle);
            GUILayout.Space(10);

            bool newFovEnabled = DrawCustomToggle(fovTab.FovModEnabled, "Enable Custom FOV");
            if (newFovEnabled != fovTab.FovModEnabled)
                fovTab.SetFovEnabled(newFovEnabled, prefs);

            GUILayout.Space(10);

            DrawCenteredLabelAboveSlider($"FOV: {Mathf.RoundToInt(fovTab.TargetFOV)}°");
            float newFov = DrawCustomSlider(fovTab.TargetFOV, 50f, 110f);
            if (!Mathf.Approximately(newFov, fovTab.TargetFOV))
                fovTab.SetTargetFOV(newFov, prefs, inValidMap);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string fovKeyText =
                fovTab.IsCapturingFovKey ? "Press any key..." : $"Toggle Key: {fovTab.FovToggleKey}";
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(200)))
                fovTab.StartCapturingKey();
            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f,
                r.width - 2f, r.height - 2f);
            DrawOutlinedCenteredText(rInset, fovKeyText, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawAnticheatTab()
        {
            DrawHeader("Speed Anticheat");
            GUILayout.Label("Monitor players for suspicious movement speed", descriptionStyle);
            GUILayout.Space(10);

            bool newEnabled = DrawCustomToggle(anticheatTab.Enabled, "Enable Speed Detection");
            if (newEnabled != anticheatTab.Enabled)
                anticheatTab.SetEnabled(newEnabled, prefs);

            GUILayout.Space(10);

            DrawCenteredLabelAboveSlider($"Alert Duration: {anticheatTab.AlertDuration:F1}s");
            float newDuration = DrawCustomSlider(anticheatTab.AlertDuration, 3f, 15f);
            if (!Mathf.Approximately(newDuration, anticheatTab.AlertDuration))
                anticheatTab.SetAlertDuration(newDuration, prefs);

            GUILayout.Space(25);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(200)))
                anticheatTab.ClearAlerts();
            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f,
                r.width - 2f, r.height - 2f);
            DrawOutlinedCenteredText(rInset, "Clear Alerts", tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            int alertCount = anticheatTab.GetAlertCount();
            int trackedCount = anticheatTab.GetTrackedPlayerCount();
            GUILayout.Label($"Tracking {trackedCount} players | {alertCount} active alerts",
                descriptionStyle);
        }

        private void DrawMenuTab()
        {
            DrawHeader("Menu Customization");
            GUILayout.Label("Customize menu background and music settings", descriptionStyle);
            GUILayout.Space(10);

            DrawHeader("Menu Background");
            GUILayout.Space(10);

            bool inLobby = menuTab.IsInLobby();

            GUI.enabled = !inLobby;

            bool newSceneEnabled = DrawCustomToggle(menuTab.SceneEnabled, "Enable Custom Background");
            if (newSceneEnabled != menuTab.SceneEnabled && !inLobby)
                menuTab.SetSceneEnabled(newSceneEnabled, prefs);

            GUI.enabled = true;

            GUILayout.Space(15);

            if (inLobby)
            {
                GUILayout.Label("Background selection is disabled while you are in a lobby.",
                    descriptionStyle);
                GUILayout.Space(8);
            }

            bool canChangeMaps = menuTab.SceneEnabled && !inLobby;
            GUI.enabled = canChangeMaps;

            int[] order = { 3, 4, 5, 1, 6, 2, 7 };

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int i = 0; i < 4; i++)
            {
                var map = GetMapInfoById(order[i]);
                if (map != null)
                {
                    bool isSelected = (menuTab.SelectedBg == map.Id);
                    float buttonWidth = GetMapButtonWidth(map.DisplayName);
                    if (DrawMapButton(map.DisplayName, isSelected, buttonWidth))
                        menuTab.SetSelectedBg(map.Id, prefs);
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
                    float buttonWidth = GetMapButtonWidth(map.DisplayName);
                    if (DrawMapButton(map.DisplayName, isSelected, buttonWidth))
                        menuTab.SetSelectedBg(map.Id, prefs);
                }
                if (i < 6) GUILayout.Space(MAP_BUTTON_SPACING);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.enabled = true;

            GUILayout.Space(30);

            DrawHeader("Music Settings");
            GUILayout.Space(10);

            bool inMenuScene = menuTab.IsMenuSceneActive();
            GUI.enabled = inMenuScene;

            bool newDisableIngameMusic =
                DrawCustomToggle(menuTab.DisableIngameMusic, "Disable Music in-game");
            if (newDisableIngameMusic != menuTab.DisableIngameMusic && inMenuScene)
                menuTab.SetDisableIngameMusic(newDisableIngameMusic, prefs);

            GUI.enabled = true;

            GUILayout.Space(5);

            bool newMusicEnabled = DrawCustomToggle(menuTab.RememberMusic, "Remember Menu Music");
            if (newMusicEnabled != menuTab.RememberMusic)
                menuTab.SetRememberMusic(newMusicEnabled, prefs);

            GUILayout.Space(5);

            if (menuTab.RememberMusic && !string.IsNullOrEmpty(menuTab.LastClipName))
                GUILayout.Label($"Current: {menuTab.LastClipName}", descriptionStyle);
        }

        private Menu.MapInfo GetMapInfoById(int id)
        {
            foreach (var m in Menu.MapInfos)
                if (m.Id == id) return m;
            return null;
        }

        private float GetMapButtonWidth(string mapName)
        {
            switch (mapName)
            {
                case "Inn": return 65f;
                case "Town": return 70f;
                case "Manor": return 75f;
                case "Asylum": return 82f;
                case "Carnival": return 90f;
                case "Farmhouse": return 95f;
                case "Slaughterhouse": return 115f;
                default: return 88f;
            }
        }

        private bool DrawMapButton(string label, bool isSelected, float buttonWidth)
        {
            bool clicked = GUILayout.Button(GUIContent.none, buttonStyle,
                GUILayout.Width(buttonWidth), GUILayout.Height(MAP_BUTTON_HEIGHT));

            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f,
                r.width - 2f, r.height - 2f);

            var s = new GUIStyle(tabTitleStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                wordWrap = false,
                clipping = TextClipping.Overflow
            };

            if (label == "Slaughterhouse")
                s.fontSize = 11;

            string symbol;
            Color symbolColor;

            if (isSelected)
            {
                symbol = "✓";
                symbolColor = new Color(0.4f, 1f, 0.4f);
            }
            else
            {
                symbol = "";
                symbolColor = Color.white;
            }

            string plainText;
            string coloredText;
            if (!string.IsNullOrEmpty(symbol))
            {
                plainText = $"{symbol} {label}";
                string hex = ColorUtility.ToHtmlStringRGB(symbolColor);
                coloredText = $"<color=#{hex}>{symbol}</color> {label}";
            }
            else
            {
                plainText = label;
                coloredText = label;
            }

            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, STROKE_ALPHA);
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    GUI.Label(new Rect(rInset.x + dx, rInset.y + dy,
                            rInset.width, rInset.height),
                        plainText, s);
                }
            }
            GUI.color = prev;

            s.normal.textColor = Color.white;
            GUI.Label(rInset, coloredText, s);

            return clicked && GUI.enabled;
        }

        private void DrawSettingsTab()
        {
            DrawHeader("Client Settings");
            GUILayout.Label("Customize menu keybind and appearance", descriptionStyle);
            GUILayout.Space(15);

            DrawHeader("Menu Keybind");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string menuKeyText =
                settingsTab.IsCapturingMenuKey ? "Press any key..." : $"Open Menu: {settingsTab.ToggleGuiKey}";
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(200)))
                settingsTab.StartCapturingKey();
            Rect r = GUILayoutUtility.GetLastRect();
            Rect rInset = new Rect(r.x + 1f, r.y + 1f,
                r.width - 2f, r.height - 2f);
            DrawOutlinedCenteredText(rInset, menuKeyText, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            DrawHeader("Theme Color");
            GUILayout.Label("Adjust the hue to change all UI colors", descriptionStyle);
            GUILayout.Space(10);

            DrawCenteredLabelAboveSlider($"Hue: {Mathf.RoundToInt(settingsTab.ThemeHue * 360)}°");
            float newThemeHue = DrawCustomSlider(settingsTab.ThemeHue, 0f, 1f);
            if (!Mathf.Approximately(newThemeHue, settingsTab.ThemeHue))
            {
                settingsTab.SetThemeHue(newThemeHue, prefs);
                stylesInitialized = false;
            }

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string resetLabel = resetConfirmPending ? "Are you sure?" : "Reset Client Settings";
            if (GUILayout.Button(GUIContent.none, buttonStyle, GUILayout.Width(200)))
            {
                if (!resetConfirmPending)
                {
                    resetConfirmPending = true;
                }
                else
                {
                    resetConfirmPending = false;

                    var mod = Melon<Main>.Instance;
                    if (mod != null)
                    {
                        mod.ApplyFirstRunOffDefaults(false);
                    }
                    stylesInitialized = false;
                }
            }
            Rect r2 = GUILayoutUtility.GetLastRect();
            Rect r2Inset = new Rect(r2.x + 1f, r2.y + 1f,
                r2.width - 2f, r2.height - 2f);
            DrawOutlinedCenteredText(r2Inset, resetLabel, tabTitleStyle, Color.white,
                new Color(0f, 0f, 0f, STROKE_ALPHA), 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            tabVisibility.DrawVisibilityUI(
                ref _selectedTab,
                headerStyle,
                descriptionStyle,
                checkboxStyle,
                labelStyle,
                settingsTab.ThemeColorPrimary,
                tabActiveStyle,
                tabInactiveStyle,
                tabTitleStyle);
        }

        private void DrawHeader(string text)
        {
            GUIContent content = new GUIContent(text);
            Vector2 size = headerStyle.CalcSize(content);
            Rect rect = GUILayoutUtility.GetRect(size.x + 40f, size.y + 8f, GUILayout.ExpandWidth(true));
            Rect textRect = new Rect(rect.x + (rect.width - size.x) / 2f,
                rect.y, size.x, rect.height);

            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.8f);
            GUI.Label(new Rect(textRect.x + 1f, textRect.y + 1f,
                    textRect.width, textRect.height),
                content, headerStyle);
            GUI.color = prev;

            GUI.Label(textRect, content, headerStyle);
        }

        private void DrawOutlinedCenteredText(
            Rect rect,
            string text,
            GUIStyle style,
            Color textColor,
            Color outlineColor,
            int thickness = 1)
        {
            var s = new GUIStyle(style) { alignment = TextAnchor.MiddleCenter };

            Color prev = GUI.color;
            GUI.color = outlineColor;
            for (int dx = -thickness; dx <= thickness; dx++)
            {
                for (int dy = -thickness; dy <= thickness; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    GUI.Label(new Rect(rect.x + dx, rect.y + dy,
                            rect.width, rect.height),
                        text, s);
                }
            }
            GUI.color = prev;

            s.normal.textColor = textColor;
            GUI.Label(rect, text, s);
        }

        private bool DrawCustomToggle(bool value, string label)
        {
            GUILayout.BeginHorizontal();
            bool newValue = GUILayout.Toggle(value, "", GUI.enabled ? checkboxStyle : checkboxDisabledStyle);
            if (GUILayout.Button(label, toggleStyle))
                newValue = !value;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return newValue;
        }

        private void DrawCenteredLabelAboveSlider(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
        }

        private float DrawCustomSlider(float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Rect sliderRect = GUILayoutUtility.GetRect(
                SLIDER_FIXED_WIDTH, 16f,
                GUILayout.Width(SLIDER_FIXED_WIDTH),
                GUILayout.Height(16f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawRoundedTexture(sliderRect, sliderBg, 5, 0);

            float normalizedValue = (value - min) / (max - min);
            float margin = 2f;
            float availableWidth = sliderRect.width - margin * 2;
            float fillWidth = Mathf.Max(0, availableWidth * normalizedValue);

            if (fillWidth > 0)
            {
                Rect fillRect = new Rect(
                    sliderRect.x + margin,
                    sliderRect.y + margin,
                    fillWidth,
                    sliderRect.height - margin * 2);
                DrawRoundedTexture(fillRect, sliderFillBg, 4, 0);
            }

            return GUI.HorizontalSlider(sliderRect, value, min, max,
                GUIStyle.none, GUIStyle.none);
        }

        private float DrawCustomVerticalSlider(float value, float min, float max, string label)
        {
            GUILayout.BeginVertical(GUILayout.Width(16));

            Rect sliderRect = GUILayoutUtility.GetRect(16, 180,
                GUILayout.Width(16), GUILayout.Height(180));

            DrawRoundedTexture(sliderRect, sliderBg, 5, 0);

            float normalizedValue = (value - min) / (max - min);
            float margin = 2f;
            float availableHeight = sliderRect.height - margin * 2;
            float fillHeight = Mathf.Max(0, availableHeight * normalizedValue);
            float fillY = sliderRect.y + margin + (availableHeight - fillHeight);

            if (fillHeight > 0)
            {
                Rect fillRect = new Rect(
                    sliderRect.x + margin,
                    fillY,
                    sliderRect.width - margin * 2,
                    fillHeight);
                DrawRoundedTexture(fillRect, sliderFillBg, 4, 0);
            }

            float newValue = GUI.VerticalSlider(sliderRect, value, max, min,
                GUIStyle.none, GUIStyle.none);

            GUILayout.Space(8);

            GUIStyle centeredLabelStyle = new GUIStyle(labelStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                clipping = TextClipping.Overflow,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            GUILayout.Label(label, centeredLabelStyle);

            GUILayout.EndVertical();

            return newValue;
        }

        private void InitializeStyles()
        {
            blackBg = MakeRoundedTexture(128, 128, new Color(0.05f, 0.05f, 0.05f, 0.95f), 8);
            purpleBg = MakeRoundedTexture(128, 128, settingsTab.ThemeColorPrimary, 8);
            darkPurpleBg = MakeRoundedTexture(128, 128, settingsTab.ThemeColorDark, 8);
            purpleHoverBg = MakeRoundedTexture(128, 128, settingsTab.ThemeColorHover, 8);

            checkboxBg = MakeRoundedTexture(64, 64, settingsTab.ThemeColorDark, 3);
            checkboxCheckedBg = MakeRoundedTextureWithBorder(
                64, 64, settingsTab.ThemeColorPrimary, settingsTab.ThemeColorDark, 3, 2);

            Color disabledDark = new Color(
                settingsTab.ThemeColorDark.r * 0.65f,
                settingsTab.ThemeColorDark.g * 0.65f,
                settingsTab.ThemeColorDark.b * 0.65f,
                1f);
            Color disabledPrimary = new Color(
                settingsTab.ThemeColorPrimary.r * 0.65f,
                settingsTab.ThemeColorPrimary.g * 0.65f,
                settingsTab.ThemeColorPrimary.b * 0.65f,
                1f);

            checkboxDisabledBg = MakeRoundedTexture(64, 64, disabledDark, 3);
            checkboxCheckedDisabledBg = MakeRoundedTextureWithBorder(
                64, 64, disabledPrimary, disabledDark, 3, 2);

            sliderBg = MakeRoundedTexture(128, 128, settingsTab.ThemeColorDark, 5);
            sliderFillBg = MakeRoundedTexture(128, 128, settingsTab.ThemeColorPrimary, 5);

            UnityEngine.Object.DontDestroyOnLoad(blackBg);
            UnityEngine.Object.DontDestroyOnLoad(purpleBg);
            UnityEngine.Object.DontDestroyOnLoad(darkPurpleBg);
            UnityEngine.Object.DontDestroyOnLoad(purpleHoverBg);
            UnityEngine.Object.DontDestroyOnLoad(checkboxBg);
            UnityEngine.Object.DontDestroyOnLoad(checkboxCheckedBg);
            UnityEngine.Object.DontDestroyOnLoad(checkboxDisabledBg);
            UnityEngine.Object.DontDestroyOnLoad(checkboxCheckedDisabledBg);
            UnityEngine.Object.DontDestroyOnLoad(sliderBg);
            UnityEngine.Object.DontDestroyOnLoad(sliderFillBg);

            int borderSize = 8;

            windowStyle = new GUIStyle(GUI.skin.window);
            SetAllStates(windowStyle, blackBg, Color.white);
            windowStyle.fontSize = 16;
            windowStyle.fontStyle = FontStyle.Bold;
            windowStyle.alignment = TextAnchor.UpperCenter;
            windowStyle.padding = new RectOffset(10, 10, 10, 10);
            windowStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);

            headerStyle = new GUIStyle(GUI.skin.label);
            SetAllStates(headerStyle, null, settingsTab.ThemeColorPrimary);
            headerStyle.fontSize = 14;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.clipping = TextClipping.Overflow;
            headerStyle.padding = new RectOffset(2, 2, 2, 2);

            descriptionStyle = new GUIStyle(GUI.skin.label);
            SetAllStates(descriptionStyle, null, new Color(0.85f, 0.85f, 0.85f, 1f));
            descriptionStyle.fontSize = 11;
            descriptionStyle.fontStyle = FontStyle.Italic;
            descriptionStyle.alignment = TextAnchor.MiddleCenter;
            descriptionStyle.wordWrap = true;
            descriptionStyle.richText = true;
            descriptionStyle.clipping = TextClipping.Overflow;
            descriptionStyle.padding = new RectOffset(3, 3, 2, 2);

            infoManager.InitializeStyles(
                settingsTab.ThemeColorPrimary,
                settingsTab.ThemeColorHover,
                headerStyle,
                descriptionStyle);

            tabActiveStyle = new GUIStyle(GUI.skin.button);
            tabActiveStyle.normal.background = purpleBg;
            tabActiveStyle.normal.textColor = Color.white;
            tabActiveStyle.hover.background = purpleHoverBg;
            tabActiveStyle.hover.textColor = Color.white;
            tabActiveStyle.active.background = purpleHoverBg;
            tabActiveStyle.active.textColor = Color.white;
            tabActiveStyle.focused.background = purpleBg;
            tabActiveStyle.focused.textColor = Color.white;
            tabActiveStyle.onNormal.background = purpleBg;
            tabActiveStyle.onNormal.textColor = Color.white;
            tabActiveStyle.onHover.background = purpleHoverBg;
            tabActiveStyle.onHover.textColor = Color.white;
            tabActiveStyle.onActive.background = purpleHoverBg;
            tabActiveStyle.onActive.textColor = Color.white;
            tabActiveStyle.onFocused.background = purpleBg;
            tabActiveStyle.onFocused.textColor = Color.white;
            tabActiveStyle.fontSize = 13;
            tabActiveStyle.fontStyle = FontStyle.Bold;
            tabActiveStyle.alignment = TextAnchor.MiddleCenter;
            tabActiveStyle.padding = new RectOffset(12, 12, 3, 3);
            tabActiveStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);

            tabInactiveStyle = new GUIStyle(GUI.skin.button);
            tabInactiveStyle.normal.background = darkPurpleBg;
            tabInactiveStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            tabInactiveStyle.hover.background = purpleBg;
            tabInactiveStyle.hover.textColor = Color.white;
            tabInactiveStyle.active.background = purpleBg;
            tabInactiveStyle.active.textColor = Color.white;
            tabInactiveStyle.focused.background = darkPurpleBg;
            tabInactiveStyle.focused.textColor = new Color(0.9f, 0.9f, 0.9f);
            tabInactiveStyle.onNormal.background = darkPurpleBg;
            tabInactiveStyle.onNormal.textColor = new Color(0.9f, 0.9f, 0.9f);
            tabInactiveStyle.onHover.background = purpleBg;
            tabInactiveStyle.onHover.textColor = Color.white;
            tabInactiveStyle.onActive.background = purpleBg;
            tabInactiveStyle.onActive.textColor = Color.white;
            tabInactiveStyle.onFocused.background = darkPurpleBg;
            tabInactiveStyle.onFocused.textColor = new Color(0.9f, 0.9f, 0.9f);
            tabInactiveStyle.fontSize = 13;
            tabInactiveStyle.fontStyle = FontStyle.Normal;
            tabInactiveStyle.alignment = TextAnchor.MiddleCenter;
            tabInactiveStyle.padding = new RectOffset(12, 12, 3, 3);
            tabInactiveStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);

            tabTitleStyle = new GUIStyle(GUI.skin.label);
            tabTitleStyle.alignment = TextAnchor.MiddleCenter;
            tabTitleStyle.fontSize = 13;
            tabTitleStyle.fontStyle = FontStyle.Bold;
            tabTitleStyle.normal.textColor = Color.white;
            tabTitleStyle.clipping = TextClipping.Overflow;
            tabTitleStyle.richText = true;
            tabTitleStyle.padding = new RectOffset(2, 2, 2, 2);

            labelStyle = new GUIStyle(GUI.skin.label);
            SetAllStates(labelStyle, null, Color.white);
            labelStyle.fontSize = 12;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.padding = new RectOffset(5, 5, 2, 2);
            labelStyle.richText = true;
            labelStyle.clipping = TextClipping.Overflow;

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = darkPurpleBg;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.background = purpleBg;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.active.background = purpleHoverBg;
            buttonStyle.active.textColor = Color.white;
            buttonStyle.focused.background = darkPurpleBg;
            buttonStyle.focused.textColor = Color.white;
            buttonStyle.onNormal.background = darkPurpleBg;
            buttonStyle.onNormal.textColor = Color.white;
            buttonStyle.onHover.background = purpleBg;
            buttonStyle.onHover.textColor = Color.white;
            buttonStyle.onActive.background = purpleHoverBg;
            buttonStyle.onActive.textColor = Color.white;
            buttonStyle.onFocused.background = darkPurpleBg;
            buttonStyle.onFocused.textColor = Color.white;
            buttonStyle.fontSize = 12;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(10, 10, 6, 6);
            buttonStyle.border = new RectOffset(8, 8, 8, 8);

            checkboxStyle = new GUIStyle();
            checkboxStyle.normal.background = checkboxBg;
            checkboxStyle.hover.background = checkboxBg;
            checkboxStyle.active.background = checkboxBg;
            checkboxStyle.focused.background = checkboxBg;
            checkboxStyle.onNormal.background = checkboxCheckedBg;
            checkboxStyle.onHover.background = checkboxCheckedBg;
            checkboxStyle.onActive.background = checkboxCheckedBg;
            checkboxStyle.onFocused.background = checkboxCheckedBg;
            checkboxStyle.normal.textColor = Color.white;
            checkboxStyle.hover.textColor = Color.white;
            checkboxStyle.active.textColor = Color.white;
            checkboxStyle.focused.textColor = Color.white;
            checkboxStyle.onNormal.textColor = Color.white;
            checkboxStyle.onHover.textColor = Color.white;
            checkboxStyle.onActive.textColor = Color.white;
            checkboxStyle.onFocused.textColor = Color.white;
            checkboxStyle.fixedWidth = 12;
            checkboxStyle.fixedHeight = 12;
            checkboxStyle.margin = new RectOffset(0, 8, 5, 5);
            checkboxStyle.border = new RectOffset(3, 3, 3, 3);

            checkboxDisabledStyle = new GUIStyle();
            checkboxDisabledStyle.normal.background = checkboxDisabledBg;
            checkboxDisabledStyle.hover.background = checkboxDisabledBg;
            checkboxDisabledStyle.active.background = checkboxDisabledBg;
            checkboxDisabledStyle.focused.background = checkboxDisabledBg;
            checkboxDisabledStyle.onNormal.background = checkboxCheckedDisabledBg;
            checkboxDisabledStyle.onHover.background = checkboxCheckedDisabledBg;
            checkboxDisabledStyle.onActive.background = checkboxCheckedDisabledBg;
            checkboxDisabledStyle.onFocused.background = checkboxCheckedDisabledBg;
            checkboxDisabledStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.hover.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.active.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.focused.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.onNormal.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.onHover.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.onActive.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.onFocused.textColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            checkboxDisabledStyle.fixedWidth = 12;
            checkboxDisabledStyle.fixedHeight = 12;
            checkboxDisabledStyle.margin = new RectOffset(0, 8, 5, 5);
            checkboxDisabledStyle.border = new RectOffset(3, 3, 3, 3);

            toggleStyle = new GUIStyle(GUI.skin.label);
            toggleStyle.normal.textColor = Color.white;
            toggleStyle.hover.textColor = settingsTab.ThemeColorHover;
            toggleStyle.active.textColor = Color.white;
            toggleStyle.focused.textColor = Color.white;
            toggleStyle.onNormal.textColor = Color.white;
            toggleStyle.onHover.textColor = settingsTab.ThemeColorHover;
            toggleStyle.onActive.textColor = Color.white;
            toggleStyle.onFocused.textColor = Color.white;
            toggleStyle.fontSize = 12;
            toggleStyle.padding = new RectOffset(0, 0, 0, 0);
            toggleStyle.alignment = TextAnchor.MiddleLeft;
            toggleStyle.richText = true;
            toggleStyle.clipping = TextClipping.Overflow;

            verticalSliderStyle = new GUIStyle(GUI.skin.verticalSlider);
            SetAllStates(verticalSliderStyle, sliderBg, Color.white);
            verticalSliderStyle.fixedWidth = 16;
            verticalSliderStyle.margin = new RectOffset(5, 5, 0, 0);
            verticalSliderStyle.padding = new RectOffset(0, 0, 0, 0);
            verticalSliderStyle.border = new RectOffset(16, 16, 16, 16);

            verticalSliderThumbStyle = new GUIStyle(GUI.skin.verticalSliderThumb);
            SetAllStates(verticalSliderThumbStyle, sliderFillBg, Color.white);
            verticalSliderThumbStyle.fixedWidth = 16;
            verticalSliderThumbStyle.fixedHeight = 0;
            verticalSliderThumbStyle.stretchHeight = true;
            verticalSliderThumbStyle.border = new RectOffset(16, 16, 16, 16);
            verticalSliderThumbStyle.overflow = new RectOffset(0, 0, 0, 0);

            stylesInitialized = true;
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

                Cursor.lockState = CURSOR_CONFINED_WHEN_GUI ? CursorLockMode.Confined : CursorLockMode.None;
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
            if (_showGui)
            {
                var desired = CURSOR_CONFINED_WHEN_GUI ? CursorLockMode.Confined : CursorLockMode.None;
                if (Cursor.lockState != desired) Cursor.lockState = desired;
                if (!Cursor.visible) Cursor.visible = true;
            }
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
                            {
                                pixels[y * width + x] = color;
                            }
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

        private Texture2D MakeRoundedTextureWithBorder(
            int width,
            int height,
            Color innerColor,
            Color borderColor,
            int cornerRadius,
            int borderWidth)
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
                        float outerCornerX, outerCornerY;
                        if (inOuterTopLeft) { outerCornerX = outerRadius; outerCornerY = outerRadius; }
                        else if (inOuterTopRight) { outerCornerX = width - outerRadius; outerCornerY = outerRadius; }
                        else if (inOuterBottomLeft) { outerCornerX = outerRadius; outerCornerY = height - outerRadius; }
                        else { outerCornerX = width - outerRadius; outerCornerY = height - outerRadius; }

                        float dx = x + 0.5f - outerCornerX;
                        float dy = y + 0.5f - outerCornerY;
                        float distToOuterCorner = Mathf.Sqrt(dx * dx + dy * dy);

                        if (distToOuterCorner > outerRadius + 0.5f)
                        {
                            pixels[y * width + x] = new Color(borderColor.r, borderColor.g, borderColor.b, 0);
                        }
                        else if (distToOuterCorner > outerRadius - 0.5f)
                        {
                            float alpha = Mathf.Clamp01(outerRadius + 0.5f - distToOuterCorner);
                            pixels[y * width + x] = new Color(borderColor.r, borderColor.g, borderColor.b, alpha);
                        }
                        else
                        {
                            float innerCornerX = outerCornerX;
                            float innerCornerY = outerCornerY;

                            float innerDx = x + 0.5f - innerCornerX;
                            float innerDy = y + 0.5f - innerCornerY;
                            float distToInnerCorner = Mathf.Sqrt(innerDx * innerDx + innerDy * innerDy);

                            if (distToInnerCorner <= innerCornerRadius - 0.5f)
                            {
                                pixels[y * width + x] = innerColor;
                            }
                            else if (distToInnerCorner < innerCornerRadius + 0.5f)
                            {
                                float t = Mathf.Clamp01(distToInnerCorner - (innerCornerRadius - 0.5f));
                                pixels[y * width + x] = Color.Lerp(innerColor, borderColor, t);
                            }
                            else
                            {
                                pixels[y * width + x] = borderColor;
                            }
                        }
                    }
                    else
                    {
                        bool inBorder = x < borderWidth || x >= width - borderWidth ||
                                        y < borderWidth || y >= height - borderWidth;
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

        private void DrawRoundedTexture(Rect rect, Texture2D texture, int cornerRadius, int borderWidth = 0)
        {
            if (texture == null) return;
            GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true,
                0, Color.white, borderWidth, cornerRadius);
        }
    }
}