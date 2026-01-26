using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourCore
{
	public class Audio
	{
		private const string MENU_SCENE_NAME = "Menu";

		private MelonPreferences_Entry<string> prefLastClipName;
		private MelonPreferences_Entry<bool> prefRememberMusic;
		private MelonPreferences_Entry<bool> prefDisableIngameMusic;
		private MelonPreferences_Entry<bool> prefMuteCarnivalTunnel;
		private MelonPreferences_Entry<bool> prefMuteCarnivalClockSounds;

		private UnityEngine.AudioSource _bgMusicSource;
		private string _currentClipName = null;
		private bool _isTracking = false;
		private object _trackingRoutine = null;

		private bool _disableIngameMusic = false;
		private bool _muteCarnivalTunnel = false;
		private bool _muteCarnivalClockSounds = false;

		private readonly Dictionary<int, float> _originalVolumes = new Dictionary<int, float>();
		private readonly List<AudioSource> _bgmSources = new List<AudioSource>();
		private int _mutedCount = 0;
		private string _currentScene = "";

		private float _lastBgmScanTime = 0f;
		private const float BGM_SCAN_INTERVAL = 2.0f;
		private int _bgmScanAttempts = 0;
		private const int MAX_BGM_SCAN_ATTEMPTS = 5;

		private MelonPreferences_Category _prefsCategory;

		
		private static readonly HashSet<string> ExplicitBgmClipNames =
			new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
			{
				"farmhouse-bgm-0-3",
				"farmhouse-bgm-4-6",
				"farmhouse-bgm-7-10",
				"farmhouse-bgm-7-10-inside",
				"farmhouse-bgm-7-10-outside",
				"farmhouse-bgm-8-10",

				"intro-bgm",
				"asylum-bgm-1-3",
				"asylum-bgm-4-7",
				"asylum-bgm-8-10",

				"Devour_The Inn_BGM_Levels1-3",
				"Devour_The Inn_Enrage BGM_Levels 1-3",
				"Devour_The Inn_Enrage BGM_Levels 4-6",
				"Devour_The Inn_BGM_Levels4-6",
				"Devour_The Inn_BGM Enrage_Levels 7-10",
				"Devour_The Inn_BGM_Levels6-10",
				"Devour_The Inn_BGM_Levels7-10",

				"Devour_The Town_BGM_Levels 0-3",
				"Devour_The Town_BGM_Levels 4-6",
				"Devour_The Town_BGM_Levels 7-10",
				"Devour_The Town_Enrage Drone_Levels 1-3",
				"Devour_The Town_Enrage Drone_Levels 4-6",
				"Devour_The Town_Enrage Drone_Levels 7-10",

				"Devour_The SlHse_BGM 0-3",
				"DV_SlHse_Enrage BGM 0-3",
				"DV_SlHse_Enrage BGM 4-6",
				"DV_SlHse_Enrage BGM 7-10",

				"Devour_The Manor_BGM_Wander_0-3",
				"Devour_The Manor_BGM_Enrage_0-3",
				"Devour_The Manor_BGM_Wander_4-6",
				"Devour_The Manor_BGM_Wander_7-10",
				"Devour_The Manor_BGM_Enrage_7-10",
				"Devour_TM_Enrage BGM_0-3",
				"Devour_TM_Enrage BGM_4-6",
				"Devour_TM_Enrage BGM_7-10",

				"Carnival_BGM_Wander0-3",
				"Carnival_BGM_Enrage_0-3",
				"Carnival_BGM_Enrage_4-6",
				"Carnival_BGM_Enrage_7-10"
			};

		public bool RememberMusic => prefRememberMusic?.Value ?? false;
		public string LastClipName => prefLastClipName?.Value ?? string.Empty;
		public bool DisableIngameMusic => _disableIngameMusic;
		public bool MuteCarnivalTunnel => _muteCarnivalTunnel;
		public bool MuteCarnivalClockSounds => _muteCarnivalClockSounds;

		public void Initialize(MelonPreferences_Category prefs)
		{
			_prefsCategory = prefs;

			prefLastClipName = prefs.CreateEntry("MenuMusicLastClipName", "Trapped");
			prefRememberMusic = prefs.CreateEntry("MenuMusicRememberEnabled", true);

			prefDisableIngameMusic = prefs.CreateEntry("DisableIngameMusic", false);
			_disableIngameMusic = prefDisableIngameMusic.Value;

			prefMuteCarnivalTunnel = prefs.CreateEntry("MuteCarnivalTunnel", true);
			_muteCarnivalTunnel = prefMuteCarnivalTunnel.Value;

			prefMuteCarnivalClockSounds = prefs.CreateEntry("MuteCarnivalClockSounds_Carnival", false);
			_muteCarnivalClockSounds = prefMuteCarnivalClockSounds.Value;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			prefRememberMusic.Value = false;
			SetDisableIngameMusic(false, prefsCategory);
			SetMuteCarnivalTunnel(false, prefsCategory);
			SetMuteCarnivalClockSounds(false, prefsCategory);
		}

		public void SetRememberMusic(bool enabled, MelonPreferences_Category prefs)
		{
			prefRememberMusic.Value = enabled;
			prefs.SaveToFile(false);
		}

		public void SetDisableIngameMusic(bool value, MelonPreferences_Category prefs)
		{
			_disableIngameMusic = value;
			prefDisableIngameMusic.Value = value;
			prefs.SaveToFile(false);

			if (!value)
			{
				if (!IsMenuScene())
					RestoreAllBGMInScene();
			}
			else
			{
				if (!IsMenuScene())
					EnsureBgmSourcesMutedLazy();
			}
		}

		public void SetMuteCarnivalTunnel(bool value, MelonPreferences_Category prefs)
		{
			_muteCarnivalTunnel = value;
			prefMuteCarnivalTunnel.Value = value;
			prefs.SaveToFile(false);

			RestoreAllBGMInScene();
			if (!IsMenuScene())
				EnsureBgmSourcesMutedLazy();
		}

		public void SetMuteCarnivalClockSounds(bool value, MelonPreferences_Category prefs)
		{
			_muteCarnivalClockSounds = value;
			prefMuteCarnivalClockSounds.Value = value;
			prefs.SaveToFile(false);

			RestoreAllBGMInScene();
			if (!IsMenuScene())
				EnsureBgmSourcesMutedLazy();
		}

		public void OnSceneWasLoaded(string sceneName)
		{
			_currentScene = sceneName;

			_originalVolumes.Clear();
			_bgmSources.Clear();
			_mutedCount = 0;
			_bgmScanAttempts = 0;
			_lastBgmScanTime = 0f;

			_isTracking = false;
			_bgMusicSource = null;

			if (_trackingRoutine != null)
			{
				try { MelonCoroutines.Stop(_trackingRoutine); } catch { }
				_trackingRoutine = null;
			}

			_trackingRoutine = MelonCoroutines.Start(InitMusicInScene());
		}

		public void OnUpdate()
		{
			if (IsMenuScene())
				return;

			if (_disableIngameMusic || _muteCarnivalTunnel || _muteCarnivalClockSounds)
				EnsureBgmSourcesMutedLazy();
		}

		private bool IsMenuScene()
		{
			return _currentScene == MENU_SCENE_NAME;
		}

		private IEnumerator InitMusicInScene()
		{
			for (int i = 0; i < 60; i++)
				yield return null;

			var bgObj = GameObject.Find("Background Music");
			if (bgObj == null)
				yield break;

			_bgMusicSource = bgObj.GetComponent<UnityEngine.AudioSource>();
			if (_bgMusicSource == null)
				yield break;

			if (RememberMusic)
			{
				string savedName = prefLastClipName.Value;
				if (!string.IsNullOrEmpty(savedName))
					TryApplySavedClip(savedName);
			}

			_isTracking = true;
			MelonCoroutines.Start(TrackMusicChanges());
		}

		private IEnumerator TrackMusicChanges()
		{
			while (_isTracking && _bgMusicSource != null)
			{
				if (RememberMusic)
				{
					var clip = _bgMusicSource.clip;
					string nameNow = (clip != null) ? clip.name : null;

					if (!string.IsNullOrEmpty(nameNow) && nameNow != _currentClipName)
					{
						_currentClipName = nameNow;
						prefLastClipName.Value = _currentClipName;
						_prefsCategory?.SaveToFile(false);
					}
				}

				yield return new WaitForSeconds(0.5f);
			}
		}

		private void TryApplySavedClip(string savedName)
		{
			if (_bgMusicSource == null)
				return;

			if (_bgMusicSource.clip != null && _bgMusicSource.clip.name == savedName)
			{
				_currentClipName = savedName;
				return;
			}

			UnityEngine.AudioClip target = null;
			var allClips = Resources.FindObjectsOfTypeAll<UnityEngine.AudioClip>();
			foreach (var clip in allClips)
			{
				if (clip != null && clip.name == savedName)
				{
					target = clip;
					break;
				}
			}

			if (target == null)
				return;

			_bgMusicSource.clip = target;
			_bgMusicSource.time = 0f;
			_bgMusicSource.Play();

			_currentClipName = savedName;
		}

		private bool IsBGMSource(AudioSource source)
		{
			if (source == null)
				return false;

			AudioClip clip = null;
			try { clip = source.clip; } catch { }

			if (clip == null)
				return false;

			string clipName = "";
			string clipNameUpper = "";
			try
			{
				clipName = clip.name;
				clipNameUpper = clip.name.ToUpperInvariant();
			}
			catch { }

			if (!_disableIngameMusic && !_muteCarnivalTunnel && !_muteCarnivalClockSounds)
				return false;

			bool isExplicitBgm = false;
			if (!string.IsNullOrEmpty(clipName))
				isExplicitBgm = ExplicitBgmClipNames.Contains(clipName);

			bool isCarnivalScene = (_currentScene == "Carnival");
			bool isTunnel = clipNameUpper.Contains("CARNIVAL_AMB_RT_SPINNINGTUNNEL");
			bool isCarnivalClock = clipNameUpper.Contains("CLOCK");

			bool wantMute =
				(_disableIngameMusic && isExplicitBgm) ||
				(_muteCarnivalTunnel && isCarnivalScene && isTunnel) ||
				(_muteCarnivalClockSounds && isCarnivalScene && isCarnivalClock);

			return wantMute;
		}

		private void EnsureBgmSourcesMutedLazy()
		{
			if (_bgmSources.Count > 0)
			{
				MuteCachedBgmSources();
				return;
			}

			if (_bgmScanAttempts >= MAX_BGM_SCAN_ATTEMPTS)
				return;

			if (Time.time - _lastBgmScanTime < BGM_SCAN_INTERVAL)
				return;

			_lastBgmScanTime = Time.time;
			_bgmScanAttempts++;

			AudioSource[] allSources = null;
			try { allSources = GameObject.FindObjectsOfType<AudioSource>(); }
			catch { return; }

			if (allSources == null || allSources.Length == 0)
				return;

			_bgmSources.Clear();
			_originalVolumes.Clear();
			_mutedCount = 0;

			foreach (var source in allSources)
			{
				if (source == null)
					continue;

				try
				{
					if (!IsBGMSource(source))
						continue;

					int id = source.GetInstanceID();

					if (!_originalVolumes.ContainsKey(id))
						_originalVolumes[id] = source.volume;

					_bgmSources.Add(source);
				}
				catch { }
			}

			if (_bgmSources.Count > 0)
				MuteCachedBgmSources();
		}

		private void MuteCachedBgmSources()
		{
			foreach (var source in _bgmSources)
			{
				if (source == null) continue;

				try
				{
					if (source.volume > 0f)
					{
						source.volume = 0f;
						_mutedCount++;
					}
				}
				catch { }
			}
		}

		public void RestoreAllBGMInScene()
		{
			if (_bgmSources.Count == 0 && _originalVolumes.Count == 0)
				return;

			foreach (var source in _bgmSources)
			{
				if (source == null)
					continue;

				try
				{
					int id = source.GetInstanceID();
					if (_originalVolumes.TryGetValue(id, out float vol))
						source.volume = vol;
				}
				catch { }
			}

			_mutedCount = 0;
			_bgmSources.Clear();
			_originalVolumes.Clear();
			_bgmScanAttempts = 0;
			_lastBgmScanTime = 0f;
		}

	}

	internal static class WeatherAudioPatches
	{
		public static bool Play_Prefix(AudioSource __instance)
		{
			var opt = Optimize.Instance;
			if (opt == null || !opt.MuteWeatherAudio) return true;

			var clip = __instance != null ? __instance.clip : null;
			return !Optimize.ShouldMuteWeatherAudio(__instance, clip);
		}

		public static bool PlayDelayed_Prefix(AudioSource __instance, float delay)
		{
			var opt = Optimize.Instance;
			if (opt == null || !opt.MuteWeatherAudio) return true;

			var clip = __instance != null ? __instance.clip : null;
			return !Optimize.ShouldMuteWeatherAudio(__instance, clip);
		}

		public static bool PlayOneShot1_Prefix(AudioSource __instance, AudioClip clip)
		{
			var opt = Optimize.Instance;
			if (opt == null || !opt.MuteWeatherAudio) return true;

			return !Optimize.ShouldMuteWeatherAudio(__instance, clip);
		}

		public static bool PlayOneShot2_Prefix(AudioSource __instance, AudioClip clip, float volumeScale)
		{
			var opt = Optimize.Instance;
			if (opt == null || !opt.MuteWeatherAudio) return true;

			return !Optimize.ShouldMuteWeatherAudio(__instance, clip);
		}
	}
}