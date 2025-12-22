using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace DevourCore
{
	public class Optimize
	{
		private const float MIN_CULL_RADIUS = 6f;
		private const float MAX_CULL_RADIUS = 30f;
		private const float DEFAULT_CULL_RADIUS = 20f;

		private float _cullRadius = DEFAULT_CULL_RADIUS;
		private bool _cullEnabled = true;
		private bool _applyInMenu = false;
		private KeyCode _cullToggleKey = KeyCode.RightControl;
		private bool _isCapturingCullKey = false;

		private struct CamState
		{
			public float Far;
			public bool LayerCullSpherical;
			public float[] LayerCullDistances;
			public bool HadDistances;
		}

		private readonly Dictionary<int, CamState> originalById = new Dictionary<int, CamState>();
		private float[] sharedCullDistances = null;
		private float sharedValue = -1f;

		private bool _disableWeather = true;
		private readonly HashSet<ParticleSystem> _handledWeather = new HashSet<ParticleSystem>();
		private readonly string[] _weatherKeywords = new[]
		{
			"rain", "snow", "dust", "splash", "debris", "sandstorm",
			"tumbleweed", "wind", "leaf", "blizzard", "blowingsnow"
		};
		private object _weatherCoroutineToken;
		private object _weatherAudioWatchToken;

		private bool _muteWeatherAudio = true;

		private MelonPreferences_Entry<float> prefCullRadius;
		private MelonPreferences_Entry<bool> prefCullEnabled;
		private MelonPreferences_Entry<bool> prefApplyInMenu;
		private MelonPreferences_Entry<bool> prefWeatherDisabled;
		private MelonPreferences_Entry<KeyCode> prefCullToggleKey;
		private MelonPreferences_Entry<bool> prefMuteWeatherAudio;

		private MelonPreferences_Category prefs;

		private bool inValidMap = false;
		private bool inMenu = false;

		private Il2CppArrayBase<Camera> _cachedCameras = null;

		private bool _cullNeedsReapply = false;

		private bool _hasMollySceneCache = false;
		private bool _isMollySceneCached = false;

		private bool _wasCullActive = false;

		private bool _cullOverrideActive = false;
		private bool _savedCullEnabled;
		private bool _savedApplyInMenu;

		private static readonly KeyCode[] AllKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

		internal static Optimize Instance;

		internal static readonly HashSet<string> WeatherMutedNames =
			new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"ThunderClap_A_Audio",
				"ThunderClap_B_Audio",
				"ThunderClap_C_Audio",
				"ThunderClap_D_Audio",

				"Weather_DryThunder_1",
				"Weather_DryThunder_2",
				"Weather_DryThunder_3",
				"Weather_DryThunder_4",
				"Weather_DryThunder_5",

				"No Rain Audio Source",
				"Heavy Rain Audio Source",
				"Thunder Lightning Audio Source",

				"Start Location Tunnel",
				"Sewer Back Entrance",
				"Sewer Tent Entrance",
				"Funhouse Tent Front",
				"Funhouse Tent Back",

				"House Shake (Tent)",

				"Devour_The Town_3D Environment Clip_Rattlesnake_4",
				"Devour_The Town_3D Environment Clip_Rattlesnake_3",
				"Devour_The Town_3D Environment Clip_Rattlesnake_2",
				"Devour_The Town_3D Environment Clip_Rattlesnake_1",

				"farmhouse-shake-thump-6",
				"farmhouse-shake-thump-5",
				"farmhouse-shake-thump-4",
				"farmhouse-shake-thump-3",
				"farmhouse-shake-thump-2",
				"farmhouse-shake-thump-1",

				"Devour_The Manor_Outside Amb_0-3",
				"Devour_The Manor_Outside Amb_7-10",
				"Devour_The Manor_Outside Amb_3-5",

				"Devour_Storm_Lv1_Closed Window_2m Loop",
				"Devour_Storm_Lv2_Closed Window_2m Loop",
				"Devour_Storm_Lv3_Closed Window_2m Loop",
				"Devour_Storm_Lv1_Open Window_2m Loop",
				"Devour_Storm_Lv2_Open Window_2m Loop",
				"Devour_Storm_Lv3_Open Window_2m Loop",

				"Weather",
				"Devour_Storm_Window Smash",
			};

		internal static readonly string[] WeatherMuteKeywords = new[]
		{
			"storm",
			"_storm",
			"storm_",
			"storm_lv",
			"devour_storm",
			"window",
			"closed window",
			"open window",
			"thunder",
			"thunderclap",
			"better-rain",
			"outside amb",
			"sandstorm",
			"blizzard",
			"tentshake",
			"tent_shake",
			"carnival_amb",
			"manor_outside",
			"town_sandstorm",
			"rattlesnake",
			"farmhouse-shake",
			"farmhouse_shake",
			"_lv1_",
			"_lv2_",
			"_lv3_",
			"_lv4_",
			"_lv5_",
			"_lv6_",
			"_lv7_",
			"_lv8_",
			"_lv9_",
			"_lv10_",
			"2m loop",
			"storm_lv1",
			"storm_lv2",
			"storm_lv3",
		};

		internal static readonly string[] MollySceneKeywords = new[]
		{
			"wind",
			"_wind",
			"wind_",
			"wind tunnel",
			"outside",
			"outside amb",
			"rain",
			"_rain",
			"rain_",
			"heavy rain",
			"light rain",
		};

		internal static readonly string[] MollySceneNames = new[]
		{
			"molly",
			"farmhouse",
			"farm",
		};

		internal static readonly HashSet<string> WeatherMuteScenes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
		};

		internal static bool ShouldMuteWeatherAudio(AudioSource src, AudioClip clip)
		{
			if (src == null || clip == null)
				return false;

			if (Instance == null || !Instance._muteWeatherAudio)
				return false;

			string goName = null;
			string clipName = null;

			try { goName = src.gameObject != null ? src.gameObject.name : null; } catch { }
			try { clipName = clip.name; } catch { }

			if (string.IsNullOrEmpty(goName) && string.IsNullOrEmpty(clipName))
				return false;

			bool isMolly = IsMollyScene();

			if (!string.IsNullOrEmpty(goName) && WeatherMutedNames.Contains(goName))
				return true;

			if (!string.IsNullOrEmpty(clipName) && WeatherMutedNames.Contains(clipName))
				return true;

			if (MatchesAnyKeyword(goName) || MatchesAnyKeyword(clipName))
				return true;

			if (isMolly && (MatchesAnyMollyKeyword(goName) || MatchesAnyMollyKeyword(clipName)))
				return true;

			return false;
		}

		private static bool IsMollyScene()
		{
			if (Instance != null && Instance._hasMollySceneCache)
				return Instance._isMollySceneCached;

			return InternalIsMollyScene();
		}

		private static bool InternalIsMollyScene()
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var s = SceneManager.GetSceneAt(i);
				var name = s.name;
				if (string.IsNullOrEmpty(name))
					continue;

				for (int j = 0; j < MollySceneNames.Length; j++)
				{
					if (name.IndexOf(MollySceneNames[j], StringComparison.OrdinalIgnoreCase) >= 0)
						return true;
				}
			}

			return false;
		}

		private static bool MatchesAnyKeyword(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;

			for (int i = 0; i < WeatherMuteKeywords.Length; i++)
			{
				if (name.IndexOf(WeatherMuteKeywords[i], StringComparison.OrdinalIgnoreCase) >= 0)
					return true;
			}

			return false;
		}

		private static bool MatchesAnyMollyKeyword(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;

			for (int i = 0; i < MollySceneKeywords.Length; i++)
			{
				if (name.IndexOf(MollySceneKeywords[i], StringComparison.OrdinalIgnoreCase) >= 0)
					return true;
			}

			return false;
		}

		public bool CullEnabled => _cullEnabled;
		public bool IsCapturingCullKey => _isCapturingCullKey;
		public KeyCode CullToggleKey => _cullToggleKey;
		public float CullRadius => _cullRadius;
		public bool ApplyInMenu => _applyInMenu;
		public bool DisableWeather => _disableWeather;
		public bool MuteWeatherAudio => _muteWeatherAudio;

		public void Initialize(MelonPreferences_Category prefsCategory)
		{
			prefs = prefsCategory;

			Instance = this;

			prefCullRadius = prefs.CreateEntry("CullRadius", DEFAULT_CULL_RADIUS);
			prefCullEnabled = prefs.CreateEntry("CullEnabled", false);
			prefApplyInMenu = prefs.CreateEntry("ApplyInMenu", false);
			prefWeatherDisabled = prefs.CreateEntry("WeatherDisabled", true);
			prefCullToggleKey = prefs.CreateEntry("CullToggleKey", KeyCode.RightControl);
			prefMuteWeatherAudio = prefs.CreateEntry("MuteWeatherAudio", true);

			_cullRadius = Mathf.Clamp(prefCullRadius.Value, MIN_CULL_RADIUS, MAX_CULL_RADIUS);
			_cullEnabled = prefCullEnabled.Value;
			_applyInMenu = prefApplyInMenu.Value;
			_disableWeather = prefWeatherDisabled.Value;
			_cullToggleKey = prefCullToggleKey.Value;
			_muteWeatherAudio = prefMuteWeatherAudio.Value;

			_cullNeedsReapply = _cullEnabled || _applyInMenu;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			SetCullEnabled(false, prefsCategory);
			SetApplyInMenu(false, prefsCategory);

			SetCullRadius(DEFAULT_CULL_RADIUS, prefsCategory);

			SetWeatherDisabled(true, prefsCategory, inMenu: true, inValidMap: false);
			SetMuteWeatherAudio(true, prefsCategory, inMenu: true, inValidMap: false);

			RestoreAllCameras();
			originalById.Clear();
			_cachedCameras = null;
			sharedCullDistances = null;
			sharedValue = -1f;
			_cullNeedsReapply = false;
			_wasCullActive = false;
		}

		public void UpdateSceneState(bool validMap, bool menu)
		{
			inValidMap = validMap;
			inMenu = menu;
			if (_cullEnabled || _applyInMenu)
				_cullNeedsReapply = true;
		}

		public void OnSceneLoaded(bool inValidMap)
		{
			this.inValidMap = inValidMap;

			_isMollySceneCached = InternalIsMollyScene();
			_hasMollySceneCache = true;

			RestoreAllCameras();
			originalById.Clear();
			_cachedCameras = null;
			sharedCullDistances = null;
			sharedValue = -1f;
			_cullNeedsReapply = _cullEnabled || _applyInMenu;
			_wasCullActive = false;

			if (_weatherCoroutineToken != null)
			{
				try { MelonCoroutines.Stop(_weatherCoroutineToken); } catch { }
				_weatherCoroutineToken = null;
			}

			if (_weatherAudioWatchToken != null)
			{
				try { MelonCoroutines.Stop(_weatherAudioWatchToken); } catch { }
				_weatherAudioWatchToken = null;
			}

			if (_disableWeather && this.inValidMap)
			{
				_handledWeather.Clear();
				_weatherCoroutineToken = MelonCoroutines.Start(TryDisableWeather());
			}

			if (_muteWeatherAudio && this.inValidMap && _isMollySceneCached)
			{
				_weatherAudioWatchToken = MelonCoroutines.Start(WeatherAudioWatch());
			}
		}

		public void OnUpdate()
		{
			HandleCullKeyCapture();
			HandleCullToggle();

			bool anyCullActive = _cullEnabled || (inMenu && _applyInMenu);

			if (_wasCullActive && !anyCullActive)
			{
				ApplyCameraSettings();
				_cullNeedsReapply = false;
			}
			else if (anyCullActive && _cullNeedsReapply)
			{
				ApplyCameraSettings();
				_cullNeedsReapply = false;
			}

			_wasCullActive = anyCullActive;
		}

		private void HandleCullKeyCapture()
		{
			if (_isCapturingCullKey)
			{
				for (int i = 0; i < AllKeyCodes.Length; i++)
				{
					var key = AllKeyCodes[i];
					if (Input.GetKeyDown(key))
					{
						_cullToggleKey = key;
						prefCullToggleKey.Value = key;
						prefs.SaveToFile(false);
						_isCapturingCullKey = false;
						break;
					}
				}
			}
		}

		private void HandleCullToggle()
		{
			if (Input.GetKeyDown(_cullToggleKey) && !_isCapturingCullKey)
			{
				if (!_cullOverrideActive)
				{
					_savedCullEnabled = _cullEnabled;
					_savedApplyInMenu = _applyInMenu;

					SetCullEnabled(false, prefs);
					SetApplyInMenu(false, prefs);

					_cullOverrideActive = true;
				}
				else
				{
					SetCullEnabled(_savedCullEnabled, prefs);
					SetApplyInMenu(_savedApplyInMenu, prefs);

					_cullOverrideActive = false;
				}
			}
		}

		private void ApplyCameraSettings()
		{
			bool inActiveScene = inValidMap || inMenu;
			if (!inActiveScene)
				return;

			bool shouldApplyCustom =
				(inValidMap && _cullEnabled) ||
				(inMenu && _applyInMenu);

			if (!shouldApplyCustom)
			{
				if (originalById.Count > 0)
					RestoreAllCameras();
				return;
			}

			EnsureCamerasCached();

			if (_cachedCameras == null || _cachedCameras.Length == 0)
				return;

			float clamped = Mathf.Clamp(_cullRadius, MIN_CULL_RADIUS, MAX_CULL_RADIUS);
			if (sharedCullDistances == null || !Mathf.Approximately(sharedValue, clamped))
			{
				sharedCullDistances = new float[32];
				for (int i = 0; i < 32; i++)
					sharedCullDistances[i] = clamped;
				sharedValue = clamped;
			}

			try
			{
				for (int i = 0; i < _cachedCameras.Length; i++)
				{
					var cam = _cachedCameras[i];
					if (cam == null) continue;

					int id = cam.GetInstanceID();

					if (!originalById.ContainsKey(id))
					{
						CamState st = new CamState
						{
							Far = cam.farClipPlane,
							LayerCullSpherical = cam.layerCullSpherical
						};

						var ld = cam.layerCullDistances;
						if (ld != null && ld.Length == 32)
						{
							st.HadDistances = true;
							st.LayerCullDistances = new float[32];
							for (int j = 0; j < 32; j++)
								st.LayerCullDistances[j] = ld[j];
						}
						else
						{
							st.HadDistances = false;
							st.LayerCullDistances = null;
						}

						originalById[id] = st;
					}

					if (!Mathf.Approximately(cam.farClipPlane, sharedValue))
						cam.farClipPlane = sharedValue;

					if (!cam.layerCullSpherical)
						cam.layerCullSpherical = true;

					cam.layerCullDistances = sharedCullDistances;
				}
			}
			catch { }
		}

		private void EnsureCamerasCached()
		{
			if (_cachedCameras != null)
				return;

			try
			{
				_cachedCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);
			}
			catch
			{
				_cachedCameras = null;
			}
		}

		public void RestoreAllCameras()
		{
			try
			{
				if (_cachedCameras == null)
				{
					_cachedCameras = UnityEngine.Object.FindObjectsOfType<Camera>(true);
				}

				if (_cachedCameras == null || _cachedCameras.Length == 0)
					return;

				foreach (var cam in _cachedCameras)
				{
					if (cam == null) continue;

					int id = cam.GetInstanceID();
					if (!originalById.TryGetValue(id, out CamState st))
						continue;

					cam.farClipPlane = st.Far;
					cam.layerCullSpherical = st.LayerCullSpherical;

					if (st.HadDistances && st.LayerCullDistances != null)
					{
						var restore = new float[st.LayerCullDistances.Length];
						for (int j = 0; j < st.LayerCullDistances.Length; j++)
							restore[j] = st.LayerCullDistances[j];
						cam.layerCullDistances = restore;
					}
				}
			}
			catch { }
		}

		private IEnumerator TryDisableWeather()
		{
			yield return new WaitForSeconds(0.5f);
			DisableNewWeather();

			yield return new WaitForSeconds(2f);
			DisableNewWeather();

			_weatherCoroutineToken = null;
		}

		private void DisableNewWeather()
		{
			ParticleSystem[] particleSystems = null;
			try
			{
				particleSystems = GameObject.FindObjectsOfType<ParticleSystem>(true);
			}
			catch
			{
				return;
			}

			if (particleSystems == null || particleSystems.Length == 0)
				return;

			for (int i = 0; i < particleSystems.Length; i++)
			{
				var ps = particleSystems[i];
				if (ps == null || _handledWeather.Contains(ps))
					continue;

				string name = null;
				try { name = ps.gameObject != null ? ps.gameObject.name : null; } catch { }

				if (string.IsNullOrEmpty(name))
					continue;

				for (int j = 0; j < _weatherKeywords.Length; j++)
				{
					if (name.IndexOf(_weatherKeywords[j], StringComparison.OrdinalIgnoreCase) >= 0)
					{
						try
						{
							var main = ps.main;
							main.loop = false;
							main.playOnAwake = false;
							ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
							ps.gameObject.SetActive(false);
						}
						catch { }

						_handledWeather.Add(ps);
						break;
					}
				}
			}
		}

		private IEnumerator WeatherAudioWatch()
		{
			yield return new WaitForSeconds(1.0f);

			if (_muteWeatherAudio && IsMollyScene())
				MuteWeatherSourcesOnce();

			_weatherAudioWatchToken = null;
		}

		private void MuteWeatherSourcesOnce()
		{
			AudioSource[] sources = null;
			try
			{
				sources = UnityEngine.Object.FindObjectsOfType<AudioSource>(true);
			}
			catch
			{
			}

			if (sources == null || sources.Length == 0)
				return;

			for (int i = 0; i < sources.Length; i++)
			{
				var src = sources[i];
				if (src == null)
					continue;

				AudioClip clip = null;
				try { clip = src.clip; } catch { }

				if (clip == null)
					continue;

				if (ShouldMuteWeatherAudio(src, clip))
				{
					try { src.mute = true; } catch { }
				}
			}
		}

		public void StartCapturingKey()
		{
			_isCapturingCullKey = true;
		}

		public void SetCullEnabled(bool enabled, MelonPreferences_Category prefsCategory)
		{
			_cullEnabled = enabled;
			prefCullEnabled.Value = enabled;
			prefs.SaveToFile(false);
			_cullNeedsReapply = true;
		}

		public void SetApplyInMenu(bool apply, MelonPreferences_Category prefsCategory)
		{
			_applyInMenu = apply;
			prefApplyInMenu.Value = apply;
			prefs.SaveToFile(false);
			_cullNeedsReapply = true;
		}

		public void SetCullRadius(float radius, MelonPreferences_Category prefsCategory)
		{
			_cullRadius = Mathf.Clamp(radius, MIN_CULL_RADIUS, MAX_CULL_RADIUS);
			prefCullRadius.Value = _cullRadius;
			prefs.SaveToFile(false);
			if (_cullEnabled || inMenu)
				_cullNeedsReapply = true;
		}

		public void SetWeatherDisabled(bool disabled, MelonPreferences_Category prefsCategory,
			bool inMenu, bool inValidMap)
		{
			if (!inMenu && prefsCategory != null) return;

			_disableWeather = disabled;
			prefWeatherDisabled.Value = disabled;
			prefs.SaveToFile(false);

			if (_weatherCoroutineToken != null)
			{
				try { MelonCoroutines.Stop(_weatherCoroutineToken); } catch { }
				_weatherCoroutineToken = null;
			}

			if (_disableWeather && inValidMap)
			{
				_handledWeather.Clear();
				_weatherCoroutineToken = MelonCoroutines.Start(TryDisableWeather());
			}
		}

		public void SetMuteWeatherAudio(bool mute, MelonPreferences_Category prefsCategory,
			bool inMenu, bool inValidMap)
		{
			if (!inMenu && prefsCategory != null) return;

			_muteWeatherAudio = mute;
			prefMuteWeatherAudio.Value = mute;
			prefs.SaveToFile(false);

			if (_weatherAudioWatchToken != null)
			{
				try { MelonCoroutines.Stop(_weatherAudioWatchToken); } catch { }
				_weatherAudioWatchToken = null;
			}

			if (_muteWeatherAudio && inValidMap && _isMollySceneCached)
			{
				_weatherAudioWatchToken = MelonCoroutines.Start(WeatherAudioWatch());
			}
		}
	}
}
