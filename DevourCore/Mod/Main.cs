using HarmonyLib;
using Il2Cpp;
using Il2CppHorror;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(DevourCore.Main), "DevourCore", "1.1.1", "Steany & Mikasa :3")]
[assembly: MelonGame("Straight Back Games", "DEVOUR")]

namespace DevourCore
{
    public sealed class Main : MelonMod
    {
        private readonly HashSet<string> _maps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Devour", "Molly", "Inn", "Town", "Slaughterhouse", "Manor", "Carnival"
        };

        private string lastScene = "";
        private bool inValidMap = false;
        private bool inMenu = false;

        private bool hasShownWelcomePopup = false;
        private bool hasShownHelpPopup = false;

        private MelonPreferences_Category prefs;
        private MelonPreferences_Entry<bool> prefFirstRunDone;
        private MelonPreferences_Entry<bool> prefWelcomePopupShown;
        private MelonPreferences_Entry<bool> prefHelpPopupShown;

        private Optimize optimizeTab;
        private Icon hsvTab;
        private Outfit outfitTab;
        internal Speedrun speedrunTab;
        private FOV fovTab;
        private Settings settingsTab;
        private Anticheat anticheatTab;
        private Misc menuTab;
        private GUIManager guiManager;
        private Info infoManager;

        public override void OnInitializeMelon()
        {
            MelonCoroutines.Start(DelayedSplash());

            prefs = MelonPreferences.CreateCategory("DevourCore");

            optimizeTab = new Optimize();
            hsvTab = new Icon();
            outfitTab = new Outfit();
            speedrunTab = new Speedrun();
            fovTab = new FOV();
            settingsTab = new Settings();
            anticheatTab = new Anticheat();
            menuTab = new Misc();
            guiManager = new GUIManager();
            infoManager = new Info();

            optimizeTab.Initialize(prefs);
            hsvTab.Initialize(prefs);
            outfitTab.Initialize(prefs);
            speedrunTab.Initialize(prefs);
            fovTab.Initialize(prefs);
            settingsTab.Initialize(prefs);
            anticheatTab.Initialize(prefs);
            menuTab.Initialize(prefs);
            infoManager.Initialize(prefs);
            guiManager.Initialize(prefs, optimizeTab, hsvTab, outfitTab, speedrunTab, fovTab, settingsTab, anticheatTab, menuTab, infoManager);

            prefFirstRunDone = prefs.CreateEntry("FirstRunDone", false);
            prefWelcomePopupShown = prefs.CreateEntry("WelcomePopupShown", false);
            prefHelpPopupShown = prefs.CreateEntry("HelpPopupShown", false);

            hasShownWelcomePopup = prefWelcomePopupShown.Value;
            hasShownHelpPopup = prefHelpPopupShown.Value;

            if (!prefFirstRunDone.Value)
            {
                ApplyFirstRunOffDefaults(true);
                prefFirstRunDone.Value = true;
                prefs.SaveToFile(false);
            }

            var harmony = this.HarmonyInstance;

            var onEnableOriginal = AccessTools.Method(typeof(Image), "OnEnable");
            var onEnablePostfix = AccessTools.Method(typeof(Main), "ImageOnEnablePostfix");
            harmony.Patch(onEnableOriginal, null, new HarmonyMethod(onEnablePostfix));

            var setSpriteOriginal = AccessTools.PropertySetter(typeof(Image), "sprite");
            var setSpritePostfix = AccessTools.Method(typeof(Main), "ImageSetSpritePostfix");
            harmony.Patch(setSpriteOriginal, null, new HarmonyMethod(setSpritePostfix));

            var getLongPressOriginal = AccessTools.Method(typeof(DevourInput), "GetLongPress");
            var getLongPressPrefix = AccessTools.Method(typeof(Main), "DevourInputGetLongPressPrefix");
            harmony.Patch(getLongPressOriginal, new HarmonyMethod(getLongPressPrefix));

            var audioPlayOriginal = AccessTools.Method(typeof(AudioSource), "Play", Type.EmptyTypes);
            var audioPlayPrefix = AccessTools.Method(typeof(WeatherAudioPatches), "Play_Prefix");
            harmony.Patch(audioPlayOriginal, new HarmonyMethod(audioPlayPrefix), null);

            var audioPlayDelayedOriginal = AccessTools.Method(typeof(AudioSource), "PlayDelayed", new[] { typeof(float) });
            var audioPlayDelayedPrefix = AccessTools.Method(typeof(WeatherAudioPatches), "PlayDelayed_Prefix");
            harmony.Patch(audioPlayDelayedOriginal, new HarmonyMethod(audioPlayDelayedPrefix), null);

            var audioPlayOneShot1Original = AccessTools.Method(typeof(AudioSource), "PlayOneShot", new[] { typeof(AudioClip) });
            var audioPlayOneShot1Prefix = AccessTools.Method(typeof(WeatherAudioPatches), "PlayOneShot1_Prefix");
            harmony.Patch(audioPlayOneShot1Original, new HarmonyMethod(audioPlayOneShot1Prefix), null);

            var audioPlayOneShot2Original = AccessTools.Method(typeof(AudioSource), "PlayOneShot", new[] { typeof(AudioClip), typeof(float) });
            var audioPlayOneShot2Prefix = AccessTools.Method(typeof(WeatherAudioPatches), "PlayOneShot2_Prefix");
            harmony.Patch(audioPlayOneShot2Original, new HarmonyMethod(audioPlayOneShot2Prefix), null);

            var lobbyStartOriginal = AccessTools.Method(typeof(Il2CppHorror.Menu), "OnLobbyStartButtonClick");
            var lobbyStartPrefix = AccessTools.Method(typeof(Main), "LobbyStartButtonClickPrefix");
            harmony.Patch(lobbyStartOriginal, new HarmonyMethod(lobbyStartPrefix));

            SceneManager.sceneLoaded += new Action<Scene, LoadSceneMode>(OnSceneLoaded);
        }

        private IEnumerator DelayedSplash()
        {
            yield return new WaitForSecondsRealtime(0f);
            System.Console.Clear();
            Console.Show();
        }

        public void ApplyFirstRunOffDefaults(bool firstRun)
        {
            optimizeTab.ApplyFirstRunDefaults(prefs);
            hsvTab.ApplyFirstRunDefaults(prefs);
            outfitTab.ApplyFirstRunDefaults(prefs);
            speedrunTab.ApplyFirstRunDefaults(prefs);
            fovTab.ApplyFirstRunDefaults(prefs);
            settingsTab.ApplyFirstRunDefaults(prefs);
            anticheatTab.ApplyFirstRunDefaults(prefs);
            menuTab.ApplyFirstRunDefaults(prefs, firstRun);

            if (firstRun)
            {
                if (prefFirstRunDone != null) prefFirstRunDone.Value = false;
                if (prefWelcomePopupShown != null) prefWelcomePopupShown.Value = false;
                if (prefHelpPopupShown != null) prefHelpPopupShown.Value = false;

                hasShownWelcomePopup = false;
                hasShownHelpPopup = false;
            }

            prefs.SaveToFile(false);
        }

        public void ApplyFirstRunOffDefaults()
        {
            ApplyFirstRunOffDefaults(true);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string sceneName = scene.name;
            string previousScene = lastScene;
            lastScene = sceneName;

            bool wasInValidMap = inValidMap;
            inValidMap = _maps.Contains(sceneName);
            inMenu = sceneName == "Menu";

            if (!hasShownWelcomePopup && inMenu)
            {
                MelonCoroutines.Start(ShowWelcomePopup());
            }

            optimizeTab.UpdateSceneState(inValidMap, inMenu);
            optimizeTab.OnSceneLoaded(inValidMap);
            hsvTab.OnSceneLoaded();
            outfitTab.OnSceneLoaded(sceneName);
            speedrunTab.OnSceneLoaded(sceneName, wasInValidMap, inValidMap);
            fovTab.OnSceneLoaded(inValidMap);
            anticheatTab.OnSceneWasLoaded();
            menuTab.OnSceneWasLoaded(sceneName);
        }

        private IEnumerator ShowWelcomePopup()
        {
            yield return new WaitForSeconds(0.5f);

            GameObject messageModal = GameObject.Find("Message Modal");
            if (messageModal == null)
                yield break;

            CanvasGroup canvasGroup = messageModal.GetComponent<CanvasGroup>();
            Text[] allTexts = messageModal.GetComponentsInChildren<Text>(true);

            if (allTexts == null || allTexts.Length < 2)
                yield break;

            Text titleLabel = allTexts[0];
            Text buttonLabel = allTexts[1];

            if (titleLabel != null)
            {
                titleLabel.text =
                    "Welcome to DevourCore!\nPress '" +
                    settingsTab.ToggleGuiKey.ToString() +
                    "' to open the Client.";
            }

            if (buttonLabel != null)
                buttonLabel.text = "Next";

            messageModal.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            hasShownWelcomePopup = true;
            prefWelcomePopupShown.Value = true;
            prefs.SaveToFile(false);
        }

        private IEnumerator ShowHelpPopup()
        {
            yield return new WaitForSeconds(0.1f);

            GameObject messageModal = GameObject.Find("Message Modal");
            if (messageModal == null)
                yield break;

            CanvasGroup canvasGroup = messageModal.GetComponent<CanvasGroup>();
            Text[] allTexts = messageModal.GetComponentsInChildren<Text>(true);

            if (allTexts == null || allTexts.Length < 2)
                yield break;

            Text titleLabel = allTexts[0];
            Text buttonLabel = allTexts[1];

            if (titleLabel != null)
            {
                titleLabel.text =
                    "For mod explanations on each category hover the info icon in the bottom right of the GUI. You can also drag the GUI to position it anywhere you like.";
            }

            if (buttonLabel != null)
                buttonLabel.text = "Enjoy <3";

            messageModal.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            hasShownHelpPopup = true;
            prefHelpPopupShown.Value = true;
            prefs.SaveToFile(false);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1) &&
                !settingsTab.IsCapturingMenuKey &&
                !optimizeTab.IsCapturingCullKey &&
                !speedrunTab.IsCapturingInteractKey &&
                !fovTab.IsCapturingFovKey)
            {
                guiManager.CenterGuiWindow();
            }

            settingsTab.OnUpdate();

            if (Input.GetKeyDown(settingsTab.ToggleGuiKey) && !settingsTab.IsCapturingMenuKey)
            {
                if (!hasShownHelpPopup)
                {
                    MelonCoroutines.Start(ShowHelpPopup());
                }

                guiManager.ToggleGui();
            }

            optimizeTab.OnUpdate();
            hsvTab.OnUpdate();
            outfitTab.OnUpdate();
            speedrunTab.OnUpdate(inValidMap);
            fovTab.OnUpdate(inValidMap);
            menuTab.OnUpdate();
        }

        public override void OnLateUpdate()
        {
            anticheatTab.OnLateUpdate();
        }

        public override void OnGUI()
        {
            guiManager.OnGUI(inMenu, inValidMap);
            anticheatTab.OnGUI();
        }

        public static void ImageOnEnablePostfix(Image __instance)
        {
            var mod = Melon<Main>.Instance;
            mod.hsvTab.HandleImageOnEnable(__instance);
        }

        public static void ImageSetSpritePostfix(Image __instance, Sprite value)
        {
            var mod = Melon<Main>.Instance;
            mod.hsvTab.HandleImageSetSprite(__instance, value);
        }

        public static bool DevourInputGetLongPressPrefix(ref float duration)
        {
            var inst = Melon<Main>.Instance;
            if (inst?.speedrunTab != null)
            {
                return inst.speedrunTab.HandleGetLongPressPrefix(ref duration);
            }
            return true;
        }

        public static void LobbyStartButtonClickPrefix()
        {
            Misc.IsStartingGame = true;
        }
    }
}
