using HarmonyLib;
using Il2Cpp;
using Il2CppHorror;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Component = UnityEngine.Component;
using static System.Runtime.CompilerServices.RuntimeHelpers;

[assembly: MelonInfo(typeof(DevourCore.Main), "DevourCore", "1.2.1", "Steany & Mikasa :3")]
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
		private Anticheat anticheatTab;
		private Misc menuTab;
		private GUIManager guiManager;
		private Info infoManager;
		private Theme theme;

		private StunStatusIndicator stunStatus;
		private RunLookBackFeature runLookBack;
		private HudManager hud;

		public override void OnInitializeMelon()
		{
			MelonCoroutines.Start(DelayedSplash());

			prefs = MelonPreferences.CreateCategory("DevourCore");

			optimizeTab = new Optimize();
			hsvTab = new Icon();
			outfitTab = new Outfit();
			speedrunTab = new Speedrun();
			fovTab = new FOV();
			anticheatTab = new Anticheat();
			menuTab = new Misc();
			guiManager = new GUIManager();
			infoManager = new Info();
			theme = new Theme();

			stunStatus = new StunStatusIndicator();
			runLookBack = new RunLookBackFeature();
			hud = new HudManager();

			optimizeTab.Initialize(prefs);
			hsvTab.Initialize(prefs);
			outfitTab.Initialize(prefs);
			speedrunTab.Initialize(prefs);
			fovTab.Initialize(prefs);
			anticheatTab.Initialize(prefs);
			menuTab.Initialize(prefs);
			infoManager.Initialize(prefs);
			runLookBack.Initialize(prefs);
			hud.Initialize(prefs, anticheatTab);

			stunStatus.Initialize(prefs, anticheatTab, hud);
			guiManager.Initialize(
							prefs,
							optimizeTab,
							hsvTab,
							outfitTab,
							speedrunTab,
							fovTab,
							anticheatTab,
							menuTab,
							infoManager,
							runLookBack,
							stunStatus,
							hud);

			prefFirstRunDone = prefs.CreateEntry("FirstRunDone", false);
			prefWelcomePopupShown = prefs.CreateEntry("WelcomePopupShown", false);
			prefHelpPopupShown = prefs.CreateEntry("HelpPopupShown", false);

			if (!prefFirstRunDone.Value)
			{
				ApplyFirstRunOffDefaults(true);
				prefFirstRunDone.Value = true;
				prefs.SaveToFile(false);
			}

			TxtPrefs_ApplyOrCreate(prefs);

			try { MelonLogger.Msg($"[DevourCore] DevourCore.txt path: {TxtPrefsFilePath}"); } catch { }
			if (!File.Exists(TxtPrefsFilePath))
				TxtPrefs_SaveFromCategorySafe(prefs);
			try { MelonLogger.Msg($"[DevourCore] DevourCore.txt exists: {File.Exists(TxtPrefsFilePath)}"); } catch { }

			hasShownWelcomePopup = prefWelcomePopupShown.Value;
			hasShownHelpPopup = prefHelpPopupShown.Value;

			var harmony = this.HarmonyInstance;

			speedrunTab?.InstallFarmhouseDoorPatches(harmony);

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

			var audioMenuPlayPrefix = AccessTools.Method(typeof(MenuMusicAudioPatches), "Play_Prefix");
			harmony.Patch(audioPlayOriginal, new HarmonyMethod(audioMenuPlayPrefix), null);

			var audioPlayDelayedOriginal = AccessTools.Method(typeof(AudioSource), "PlayDelayed", new[] { typeof(float) });
			var audioPlayDelayedPrefix = AccessTools.Method(typeof(WeatherAudioPatches), "PlayDelayed_Prefix");
			harmony.Patch(audioPlayDelayedOriginal, new HarmonyMethod(audioPlayDelayedPrefix), null);

			var audioMenuPlayDelayedPrefix = AccessTools.Method(typeof(MenuMusicAudioPatches), "PlayDelayed_Prefix");
			harmony.Patch(audioPlayDelayedOriginal, new HarmonyMethod(audioMenuPlayDelayedPrefix), null);

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



			try
			{
				var active = SceneManager.GetActiveScene();
				if (active.IsValid() && active.name == "Menu" && !hasShownWelcomePopup)
				{
					MelonCoroutines.Start(ShowWelcomePopup());
				}
			}
			catch { }
		}

		private IEnumerator DelayedSplash()
		{
			yield return new WaitForSecondsRealtime(0f);
			System.Console.Clear();
			Console.Show();
		}

		public void ApplyFirstRunOffDefaults(bool firstRun)
		{
			bool keepMenuSceneEnabled = false;
			int keepMenuSelectedBg = 2;
			if (!firstRun && menuTab != null)
			{
				keepMenuSceneEnabled = menuTab.SceneEnabled;
				keepMenuSelectedBg = menuTab.SelectedBg;
			}

			optimizeTab.ApplyFirstRunDefaults(prefs);
			hsvTab.ApplyFirstRunDefaults(prefs);
			outfitTab.ApplyFirstRunDefaults(prefs);
			speedrunTab.ApplyFirstRunDefaults(prefs);
			fovTab.ApplyFirstRunDefaults(prefs);
			anticheatTab.ApplyFirstRunDefaults(prefs);
			menuTab.ApplyFirstRunDefaults(prefs, firstRun);

			if (!firstRun && menuTab != null)
			{
				try { menuTab.SetSelectedBg(keepMenuSelectedBg, prefs); } catch { }
				try { menuTab.SetSceneEnabled(keepMenuSceneEnabled, prefs); } catch { }
			}


			guiManager.ApplyFirstRunDefaults(prefs, firstRun);
			theme.ApplyFirstRunDefaults(prefs);
			hud.ApplyFirstRunDefaults(prefs);
			try { hud?.ReloadFromPrefs(); } catch { }
			stunStatus.ApplyFirstRunDefaults(prefs);
			runLookBack.ApplyFirstRunDefaults(prefs);


			try { hud?.ResetAllHudPositionsToDefaults(); } catch { }
			try { anticheatTab?.ResetAlertPosition(prefs); } catch { }
			try { stunStatus?.ResetPositionToDefaults(prefs); } catch { }

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

			stunStatus.OnSceneLoaded(sceneName, inValidMap);
			runLookBack.OnSceneLoaded(sceneName, inValidMap);
			hud.NotifyTimerSceneLoaded(inValidMap);
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
				titleLabel.text = string.Format(Loc.GUI.WelcomeBodyFormat, guiManager.ToggleGuiKey.ToString());
			}

			if (buttonLabel != null)
				buttonLabel.text = Loc.GUI.WelcomeButtonNext;

			messageModal.SetActive(true);

			if (canvasGroup != null)
			{
				canvasGroup.alpha = 1f;
				canvasGroup.interactable = true;
				canvasGroup.blocksRaycasts = true;
			}

			hasShownWelcomePopup = true;
			if (prefWelcomePopupShown != null)
				prefWelcomePopupShown.Value = true;
			if (prefs != null)
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
				titleLabel.text = Loc.GUI.HelpBody;
			}

			if (buttonLabel != null)
				buttonLabel.text = Loc.GUI.HelpButtonEnjoy;

			messageModal.SetActive(true);

			if (canvasGroup != null)
			{
				canvasGroup.alpha = 1f;
				canvasGroup.interactable = true;
				canvasGroup.blocksRaycasts = true;
			}

			hasShownHelpPopup = true;
			if (prefHelpPopupShown != null)
				prefHelpPopupShown.Value = true;
			if (prefs != null)
				prefs.SaveToFile(false);
		}

		public override void OnUpdate()
		{
			guiManager.Update();


			if (Input.GetKeyDown(KeyCode.F1) &&
				!optimizeTab.IsCapturingCullKey &&
				!speedrunTab.IsCapturingInteractKey &&
				!fovTab.IsCapturingFovKey &&
				(runLookBack == null || !runLookBack.IsCapturingKey))
			{
				guiManager.CenterGuiWindow();
			}

			if (Input.GetKeyDown(guiManager.ToggleGuiKey))
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

			runLookBack.OnUpdate();
			stunStatus.OnUpdate();
			hud.OnUpdate();
		}

		public override void OnLateUpdate()
		{
			anticheatTab.OnLateUpdate();
			runLookBack.OnLateUpdate();
		}

		public override void OnGUI()
		{
			guiManager.OnGUI(inMenu, inValidMap);
			anticheatTab.OnGUI();

			bool editingHudPos = anticheatTab.IsEditingPosition;
			if (editingHudPos || hud.ShowStunStatus)
				stunStatus.OnGUI();

			hud.OnGUI();
		}

		public override void OnApplicationQuit()
		{
			TxtPrefs_SaveFromCategorySafe(prefs);
		}

		public override void OnDeinitializeMelon()
		{
			TxtPrefs_SaveFromCategorySafe(prefs);
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

		private const string TxtPrefsSectionName = "DevourCore";

		private static string TxtPrefsFilePath
		{
			get
			{
				var dir = MelonEnvironment.UserDataDirectory;
				try
				{
					if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
				}
				catch { }
				return Path.Combine(dir, "DevourCore.txt");
			}
		}

		private static void TxtPrefs_ApplyOrCreate(MelonPreferences_Category category)
		{
			if (category == null) return;

			if (!File.Exists(TxtPrefsFilePath))
			{
				TxtPrefs_SaveFromCategorySafe(category);
				return;
			}

			var map = TxtPrefs_LoadRawMap(TxtPrefsFilePath);
			if (map.Count == 0) return;

			TxtPrefs_ApplyMapToCategory(category, map);

			try { category.SaveToFile(false); } catch { }
		}

		private static void TxtPrefs_SaveFromCategorySafe(MelonPreferences_Category category)
		{
			try
			{
				TxtPrefs_SaveFromCategory(category);
			}
			catch (Exception ex)
			{
				try { MelonLogger.Error($"[DevourCore] Failed writing DevourCore.txt: {ex.GetType().Name}: {ex.Message}"); } catch { }
			}
		}

		private static void TxtPrefs_SaveFromCategory(MelonPreferences_Category category)
		{
			if (category == null) return;

			var entries = TxtPrefs_GetAllEntries(category);
			var lines = new List<string>
			{
				"# DevourCore preferences",
				$"[{TxtPrefsSectionName}]"
			};

			foreach (var e in entries.OrderBy(e => TxtPrefs_GetEntryIdentifier(e), StringComparer.OrdinalIgnoreCase))
			{
				var key = TxtPrefs_GetEntryIdentifier(e);
				if (string.IsNullOrWhiteSpace(key)) continue;

				object boxed = TxtPrefs_GetEntryBoxedValue(e);
				string valueStr = TxtPrefs_ToInvariantString(boxed);
				lines.Add($"{key} = {valueStr}");
			}

		}

		private static Dictionary<string, string> TxtPrefs_LoadRawMap(string path)
		{
			var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			string currentSection = "";

			foreach (var raw in File.ReadAllLines(path))
			{
				var line = raw.Trim();
				if (line.Length == 0) continue;
				if (line.StartsWith("#") || line.StartsWith(";")) continue;

				if (line.StartsWith("[") && line.EndsWith("]"))
				{
					currentSection = line.Substring(1, line.Length - 2).Trim();
					continue;
				}

				if (!currentSection.Equals(TxtPrefsSectionName, StringComparison.OrdinalIgnoreCase))
					continue;

				int idx = line.IndexOf('=');
				if (idx <= 0) continue;

				var key = line.Substring(0, idx).Trim();
				var value = line.Substring(idx + 1).Trim();
				if (key.Length == 0) continue;

				dict[key] = value;
			}

			return dict;
		}

		private static void TxtPrefs_ApplyMapToCategory(MelonPreferences_Category category, Dictionary<string, string> map)
		{
			var entries = TxtPrefs_GetAllEntries(category);
			foreach (var e in entries)
			{
				var key = TxtPrefs_GetEntryIdentifier(e);
				if (string.IsNullOrWhiteSpace(key)) continue;

				if (!map.TryGetValue(key, out var valueStr))
					continue;

				TxtPrefs_TrySetEntryFromString(e, valueStr);
			}
		}

		private static List<object> TxtPrefs_GetAllEntries(MelonPreferences_Category category)
		{
			var list = new List<object>();
			if (category == null) return list;

			Type t = category.GetType();
			object maybe = null;

			try
			{
				var prop = t.GetProperty("Entries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (prop != null) maybe = prop.GetValue(category);
			}
			catch { }

			if (maybe == null)
			{
				try
				{
					var field = t.GetField("Entries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
							?? t.GetField("_entries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
							?? t.GetField("entries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null) maybe = field.GetValue(category);
				}
				catch { }
			}

			if (maybe is IDictionary dict)
			{
				foreach (DictionaryEntry de in dict)
					if (de.Value != null) list.Add(de.Value);
				return list;
			}

			if (maybe is IEnumerable en)
			{
				foreach (var item in en)
					if (item != null) list.Add(item);
				return list;
			}

			try
			{
				foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					var ft = f.FieldType;
					if (!ft.IsGenericType) continue;
					if (ft.GetGenericTypeDefinition().Name.StartsWith("MelonPreferences_Entry", StringComparison.Ordinal))
					{
						var v = f.GetValue(category);
						if (v != null) list.Add(v);
					}
				}
			}
			catch { }

			return list;
		}

		private static string TxtPrefs_GetEntryIdentifier(object entry)
		{
			if (entry == null) return "";
			var t = entry.GetType();

			try
			{
				var prop = t.GetProperty("Identifier", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (prop != null)
				{
					var v = prop.GetValue(entry) as string;
					if (!string.IsNullOrEmpty(v)) return v;
				}
			}
			catch { }

			try
			{
				var prop = t.GetProperty("DisplayName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (prop != null)
				{
					var v = prop.GetValue(entry) as string;
					if (!string.IsNullOrEmpty(v)) return v;
				}
			}
			catch { }

			return "";
		}

		private static object TxtPrefs_GetEntryBoxedValue(object entry)
		{
			if (entry == null) return null;
			var t = entry.GetType();

			try
			{
				var prop = t.GetProperty("BoxedValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (prop != null) return prop.GetValue(entry);
			}
			catch { }

			try
			{
				var prop = t.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (prop != null) return prop.GetValue(entry);
			}
			catch { }

			return null;
		}

		private static void TxtPrefs_TrySetEntryFromString(object entry, string valueStr)
		{
			if (entry == null) return;
			var t = entry.GetType();

			PropertyInfo boxedProp = null;
			try { boxedProp = t.GetProperty("BoxedValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); } catch { }
			if (boxedProp != null)
			{
				var targetType = boxedProp.PropertyType;
				var parsed = TxtPrefs_TryParse(valueStr, targetType);
				if (parsed.success)
				{
					try { boxedProp.SetValue(entry, parsed.value); } catch { }
				}
				return;
			}

			PropertyInfo valueProp = null;
			try { valueProp = t.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); } catch { }
			if (valueProp == null || !valueProp.CanWrite) return;

			var target = valueProp.PropertyType;
			var parsed2 = TxtPrefs_TryParse(valueStr, target);
			if (!parsed2.success) return;

			try { valueProp.SetValue(entry, parsed2.value); } catch { }
		}

		private static (bool success, object value) TxtPrefs_TryParse(string s, Type t)
		{
			if (t == typeof(string)) return (true, s);

			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var inner = Nullable.GetUnderlyingType(t);
				if (string.Equals(s, "null", StringComparison.OrdinalIgnoreCase))
					return (true, null);
				var innerParsed = TxtPrefs_TryParse(s, inner);
				return innerParsed.success ? (true, innerParsed.value) : (false, null);
			}

			try
			{
				if (t == typeof(bool)) return (true, bool.Parse(s));
				if (t == typeof(int)) return (true, int.Parse(s, CultureInfo.InvariantCulture));
				if (t == typeof(float)) return (true, float.Parse(s, CultureInfo.InvariantCulture));
				if (t == typeof(double)) return (true, double.Parse(s, CultureInfo.InvariantCulture));
				if (t == typeof(long)) return (true, long.Parse(s, CultureInfo.InvariantCulture));
				if (t == typeof(byte)) return (true, byte.Parse(s, CultureInfo.InvariantCulture));
				if (t == typeof(KeyCode)) return (true, (KeyCode)Enum.Parse(typeof(KeyCode), s, true));

				if (t.IsEnum) return (true, Enum.Parse(t, s, true));

				if (t == typeof(Color))
				{
					var parts = s.Split(',');
					if (parts.Length >= 3)
					{
						float r = float.Parse(parts[0], CultureInfo.InvariantCulture);
						float g = float.Parse(parts[1], CultureInfo.InvariantCulture);
						float b = float.Parse(parts[2], CultureInfo.InvariantCulture);
						float a = (parts.Length >= 4) ? float.Parse(parts[3], CultureInfo.InvariantCulture) : 1f;
						return (true, new Color(r, g, b, a));
					}
				}
			}
			catch { }

			return (false, null);
		}

		private static string TxtPrefs_ToInvariantString(object v)
		{
			if (v == null) return "null";
			if (v is float f) return f.ToString(CultureInfo.InvariantCulture);
			if (v is double d) return d.ToString(CultureInfo.InvariantCulture);
			if (v is IFormattable formattable) return formattable.ToString(null, CultureInfo.InvariantCulture);
			if (v is Color c) return string.Join(",", c.r.ToString(CultureInfo.InvariantCulture), c.g.ToString(CultureInfo.InvariantCulture), c.b.ToString(CultureInfo.InvariantCulture), c.a.ToString(CultureInfo.InvariantCulture));
			return v.ToString();
		}

	}
}