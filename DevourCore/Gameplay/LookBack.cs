using Il2Cpp;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DevourCore
{
	public sealed class RunLookBackFeature
	{
		private bool inValidMap = false;
		private bool isRefreshing = false;

		private Camera mainCam = null;
		private Camera lookBackCam = null;

		private Light lookBackLight = null;

		private int mainCamOriginalMask = -1;

		private KeyCode lookBackKey = KeyCode.Q;

		private const float LookBackFovOffset = 10f;
		private const float LookBackMinFov = 40f;
		private const float LookBackMaxFov = 110f;

		private const float LookBackLightIntensity = 0.6f;
		private const float LookBackLightRange = 6f;
		private const float LookBackLightSpotAngle = 50f;

		private bool lookBackEnabled = true;
		private bool toggleMode = false;
		private bool isToggledOn = false;

		private MelonPreferences_Category prefs;
		private MelonPreferences_Entry<KeyCode> prefLookBackKey;
		private MelonPreferences_Entry<bool> prefLookBackEnabled;
		private MelonPreferences_Entry<bool> prefLookBackToggleMode;

		private bool isCapturingLookBackKey = false;

		private static readonly KeyCode[] AllKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

		private NolanBehaviour _cachedLocalPlayer;
		private Survival _cachedSurvival;
		private FieldInfo _gameEndedField;
		private float _nextCacheTime = 0f;

		public KeyCode LookBackKey => lookBackKey;
		public bool IsCapturingKey => isCapturingLookBackKey;
		public bool Enabled => lookBackEnabled;
		public bool ToggleMode => toggleMode;

		public void Initialize(MelonPreferences_Category prefsCategory)
		{
			prefs = prefsCategory;

			prefLookBackKey = prefs.CreateEntry("LookBackKey", KeyCode.Q);
			lookBackKey = prefLookBackKey.Value;

			prefLookBackEnabled = prefs.CreateEntry("LookBackEnabled", true);
			lookBackEnabled = prefLookBackEnabled.Value;

			prefLookBackToggleMode = prefs.CreateEntry("LookBackToggleMode", false);
			toggleMode = prefLookBackToggleMode.Value;
		}

		public void StartCapturingKey()
		{
			isCapturingLookBackKey = true;
		}

		public void SetEnabled(bool enabled)
		{
			lookBackEnabled = enabled;

			if (!enabled)
			{
				isToggledOn = false;
				SetLookBackActive(false);
			}

			if (prefLookBackEnabled != null)
				prefLookBackEnabled.Value = enabled;
			if (prefs != null)
				prefs.SaveToFile(false);
		}

		public void SetToggleMode(bool enabled)
		{
			toggleMode = enabled;
			if (!toggleMode)
				isToggledOn = false;

			if (prefLookBackToggleMode != null)
				prefLookBackToggleMode.Value = enabled;
			if (prefs != null)
				prefs.SaveToFile(false);
		}

		public void OnSceneLoaded(string sceneName, bool inValidMap)
		{
			this.inValidMap = inValidMap;

			mainCam = null;
			lookBackCam = null;
			lookBackLight = null;
			mainCamOriginalMask = -1;
			isToggledOn = false;
			_cachedLocalPlayer = null;
			_cachedSurvival = null;
			_gameEndedField = null;

			SetLookBackActive(false);

			if (inValidMap)
			{
				MelonCoroutines.Start(RefreshLoop());
			}
		}

		private IEnumerator RefreshLoop()
		{
			if (isRefreshing) yield break;
			isRefreshing = true;

			yield return new WaitForSeconds(1.0f);

			while (inValidMap)
			{
				FindOrCreateCamerasAndLight();
				yield return new WaitForSeconds(2.0f);
			}

			isRefreshing = false;
		}

		private void FindOrCreateCamerasAndLight()
		{
			try
			{
				if (mainCam == null || mainCam.gameObject == null)
				{
					mainCam = Camera.main;

					if (mainCam == null)
					{
						var cams = UnityEngine.Object.FindObjectsOfType<Camera>();
						if (cams != null && cams.Length > 0)
							mainCam = cams[0];
					}

					if (mainCam != null)
					{
						mainCamOriginalMask = mainCam.cullingMask;
					}
				}

				if (mainCam != null && (lookBackCam == null || lookBackCam.gameObject == null))
				{
					GameObject go = new GameObject("RunLookBackCamera");
					lookBackCam = go.AddComponent<Camera>();

					lookBackCam.CopyFrom(mainCam);
					lookBackCam.transform.position = mainCam.transform.position;
					lookBackCam.transform.rotation = mainCam.transform.rotation;

					lookBackCam.depth = mainCam.depth + 1f;
					lookBackCam.enabled = false;

					var al = lookBackCam.GetComponent<AudioListener>();
					if (al != null) UnityEngine.Object.Destroy(al);
				}

				if (lookBackCam != null && (lookBackLight == null || lookBackLight.gameObject == null))
				{
					GameObject lightObj = new GameObject("RunLookBackLight");
					lightObj.transform.SetParent(lookBackCam.transform, false);

					lookBackLight = lightObj.AddComponent<Light>();
					lookBackLight.type = LightType.Spot;
					lookBackLight.color = new Color(0.7f, 0.75f, 0.8f);
					lookBackLight.intensity = LookBackLightIntensity;
					lookBackLight.range = LookBackLightRange;
					lookBackLight.spotAngle = LookBackLightSpotAngle;
					lookBackLight.shadows = LightShadows.None;

					lookBackLight.enabled = false;
				}
			}
			catch
			{
			}
		}

		private void HandleKeyCapture()
		{
			if (!isCapturingLookBackKey)
				return;

			for (int i = 0; i < AllKeyCodes.Length; i++)
			{
				var key = AllKeyCodes[i];
				if (Input.GetKeyDown(key))
				{
					lookBackKey = key;
					if (prefLookBackKey != null)
						prefLookBackKey.Value = key;
					if (prefs != null)
						prefs.SaveToFile(false);

					isCapturingLookBackKey = false;
					break;
				}
			}
		}
		private bool IsLookBackBlocked()
		{
			if (_cachedSurvival == null)
			{
				_cachedSurvival = UnityEngine.Object.FindObjectOfType<Survival>();
				if (_cachedSurvival != null)
				{
					_gameEndedField = typeof(Survival).GetField("m_GameEnded", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				}
			}

			{
				try
				{
					bool gameEnded = (bool)_gameEndedField.GetValue(_cachedSurvival);
					if (gameEnded) return true;
				}
				catch { }
			}

			if (Time.time >= _nextCacheTime || _cachedLocalPlayer == null || _cachedLocalPlayer.Equals(null))
			{
				var players = NolanBehaviour.FindObjectsOfType<NolanBehaviour>();
				foreach (var p in players)
				{
					if (p.entity != null && p.entity.IsOwner)
					{
						_cachedLocalPlayer = p;
						break;
					}
				}
				_nextCacheTime = Time.time + 1.0f;
			}

			if (_cachedLocalPlayer != null && !_cachedLocalPlayer.Equals(null))
			{
				if (_cachedLocalPlayer.IsCrawling() ||
					_cachedLocalPlayer.IsBeingKnockedOut())
				{
					return true;
				}
			}

			return false;
		}

		private void SetLookBackActive(bool active)
		{
			if (mainCam == null || lookBackCam == null)
				return;

			if (active)
			{
				if (mainCamOriginalMask == -1 && mainCam != null)
					mainCamOriginalMask = mainCam.cullingMask;

				if (mainCam != null)
					mainCam.cullingMask = 0;

				lookBackCam.enabled = true;

				if (lookBackLight != null)
					lookBackLight.enabled = true;
			}
			else
			{
				if (mainCam != null && mainCamOriginalMask != -1)
					mainCam.cullingMask = mainCamOriginalMask;

				if (lookBackCam != null)
					lookBackCam.enabled = false;

				if (lookBackLight != null)
					lookBackLight.enabled = false;
			}
		}

		public void OnUpdate()
		{
			HandleKeyCapture();

			if (!inValidMap) return;
			if (mainCam == null || lookBackCam == null) return;
			if (!lookBackEnabled || IsLookBackBlocked())
			{
				isToggledOn = false;
				SetLookBackActive(false);
				return;
			}

			bool active;

			if (toggleMode)
			{
				if (Input.GetKeyDown(lookBackKey))
					isToggledOn = !isToggledOn;

				active = isToggledOn;
			}
			else
			{
				active = Input.GetKey(lookBackKey);

				if (!active)
					isToggledOn = false;
			}

			SetLookBackActive(active);
		}

		public void OnLateUpdate()
		{
			if (!inValidMap) return;
			if (!lookBackEnabled) return;
			if (mainCam == null || lookBackCam == null) return;
			if (!lookBackCam.enabled) return;

			lookBackCam.transform.position = mainCam.transform.position;

			Quaternion baseRot = mainCam.transform.rotation;
			Quaternion lookRot = Quaternion.AngleAxis(180f, Vector3.up) * baseRot;
			lookBackCam.transform.rotation = lookRot;

			float baseFov = mainCam.fieldOfView;
			float targetFov = Mathf.Clamp(baseFov - LookBackFovOffset, LookBackMinFov, LookBackMaxFov);
			lookBackCam.fieldOfView = targetFov;

			lookBackCam.nearClipPlane = mainCam.nearClipPlane;
			lookBackCam.farClipPlane = mainCam.farClipPlane;
			lookBackCam.cullingMask = mainCamOriginalMask != -1 ? mainCamOriginalMask : mainCam.cullingMask;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			if (prefsCategory == null) return;
			if (prefLookBackKey != null) prefLookBackKey.Value = KeyCode.Q;
			if (prefLookBackEnabled != null) prefLookBackEnabled.Value = true;
			if (prefLookBackToggleMode != null) prefLookBackToggleMode.Value = false;
			lookBackKey = KeyCode.Q;
			lookBackEnabled = true;
			toggleMode = false;
			prefsCategory.SaveToFile(false);
		}

	}
}