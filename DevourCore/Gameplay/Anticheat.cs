using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

using BoltEntityIl2Cpp = Il2CppPhoton.Bolt.BoltEntity;

namespace DevourCore
{
	public class Anticheat
	{
		private class PlayerSpeedInfo
		{
			public string DisplayName = Loc.Anti.UnknownName;
			public Vector3 LastPos;
			public float LastTime;
			public float AverageSpeed;
			public float SumSpeed;
			public int SampleCount;
			public bool AlertShown;
		}

		private class Alert
		{
			public string Text;
			public float Until;
		}

		private readonly Dictionary<NolanBehaviour, PlayerSpeedInfo> _players =
			new Dictionary<NolanBehaviour, PlayerSpeedInfo>(64);

		private readonly List<Alert> _alerts = new List<Alert>(8);

		private NolanBehaviour[] _cachedNolans = null;
		private float _lastScanTime = 0f;

		private MelonPreferences_Entry<bool> prefEnabled;
		private MelonPreferences_Entry<float> prefAlertDuration;

		private MelonPreferences_Entry<float> prefAlertPosX;
		private MelonPreferences_Entry<float> prefAlertPosY;

		private float _alertPosX = 120f;
		private float _alertPosY = 20f;

		private bool _editPositionMode = false;
		private bool _isDragging = false;
		private Vector2 _dragOffset;

		private const float SPEED_THRESHOLD = 5f;
		private const float TELEPORT_DISTANCE_THRESHOLD = 15f;
		private const float TELEPORT_SPEED_THRESHOLD = 25f;
		private const float SMOOTHING = 0.15f;
		private const float MIN_DELTA_TIME = 0.05f;
		private const float SCAN_INTERVAL = 0.5f;
		private const float DEVOUR_GRACE_SECONDS = 12f;

		private string _currentSceneName = "";
		private float _sceneEnterTime = 0f;

		public bool Enabled => prefEnabled?.Value ?? false;
		public float SpeedThreshold => SPEED_THRESHOLD;
		public float AlertDuration => prefAlertDuration?.Value ?? 8f;

		public bool IsEditingPosition => _editPositionMode;

		private GUIStyle alertTitleStyle;
		private GUIStyle alertTextStyle;
		private bool stylesInitialized = false;

		public void Initialize(MelonPreferences_Category prefs)
		{
			prefEnabled = prefs.CreateEntry("AnticheatEnabled", false);
			prefAlertDuration = prefs.CreateEntry("AnticheatAlertDuration", 8f);
			prefAlertPosX = prefs.CreateEntry("AnticheatAlertPosX", 120f);
			prefAlertPosY = prefs.CreateEntry("AnticheatAlertPosY", 20f);

			_alertPosX = prefAlertPosX.Value;
			_alertPosY = prefAlertPosY.Value;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			prefEnabled.Value = false;
			prefAlertDuration.Value = 8f;

			if (prefAlertPosX != null) prefAlertPosX.Value = 120f;
			if (prefAlertPosY != null) prefAlertPosY.Value = 20f;
			_alertPosX = 120f;
			_alertPosY = 20f;

			_players.Clear();
			_alerts.Clear();
			_cachedNolans = null;
			_lastScanTime = 0f;
			_currentSceneName = "";
			_sceneEnterTime = 0f;

			prefsCategory.SaveToFile(false);
		}

		public void SetEnabled(bool enabled, MelonPreferences_Category prefs)
		{
			prefEnabled.Value = enabled;
			prefs.SaveToFile(false);

			if (!enabled)
			{
				ClearAlerts();
				_players.Clear();
				_cachedNolans = null;
			}
		}

		public void SetAlertDuration(float duration, MelonPreferences_Category prefs)
		{
			prefAlertDuration.Value = duration;
			prefs.SaveToFile(false);
		}

		public void ClearAlerts()
		{
			_alerts.Clear();
			foreach (var kvp in _players)
				kvp.Value.AlertShown = false;
		}

		public int GetAlertCount() => _alerts.Count;
		public int GetTrackedPlayerCount() => _players.Count;

		public void OnSceneWasLoaded()
		{
			_players.Clear();
			_alerts.Clear();
			_cachedNolans = null;
			_lastScanTime = 0f;

			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			if (scene.IsValid())
			{
				_currentSceneName = scene.name;
				_sceneEnterTime = Time.time;
			}
			else
			{
				_currentSceneName = "";
				_sceneEnterTime = 0f;
			}
		}

		public void OnLateUpdate()
		{
			if (!Enabled) return;

			float now = Time.time;

			try
			{
				if (_cachedNolans == null || now - _lastScanTime >= SCAN_INTERVAL)
				{
					_lastScanTime = now;

					try
					{
						_cachedNolans = UnityEngine.Object.FindObjectsOfType<NolanBehaviour>();
					}
					catch
					{
						_cachedNolans = null;
					}

					var toRemove = new List<NolanBehaviour>();
					foreach (var kvp in _players)
					{
						if (kvp.Key == null)
							toRemove.Add(kvp.Key);
					}
					for (int i = 0; i < toRemove.Count; i++)
						_players.Remove(toRemove[i]);
				}

				if (_cachedNolans == null || _cachedNolans.Length == 0)
					return;

				bool inDevour = string.Equals(_currentSceneName, "Devour", StringComparison.OrdinalIgnoreCase);
				bool inDevourGrace = inDevour && (now - _sceneEnterTime < DEVOUR_GRACE_SECONDS);

				foreach (var nolan in _cachedNolans)
				{
					if (nolan == null)
						continue;

					if (!_players.TryGetValue(nolan, out var info))
					{
						info = new PlayerSpeedInfo
						{
							LastPos = nolan.transform.position,
							LastTime = now,
							DisplayName = ResolvePlayerName(nolan),
							SumSpeed = 0f,
							SampleCount = 0,
							AverageSpeed = 0f,
							AlertShown = false
						};
						_players[nolan] = info;
						continue;
					}

					float dt = now - info.LastTime;
					if (dt < MIN_DELTA_TIME)
						continue;

					Vector3 pos = nolan.transform.position;
					float dist = Vector3.Distance(pos, info.LastPos);
					float instSpeed = dist / dt;

					if (dist > TELEPORT_DISTANCE_THRESHOLD || instSpeed > TELEPORT_SPEED_THRESHOLD)
					{
						info.LastPos = pos;
						info.LastTime = now;
						continue;
					}

					info.SumSpeed += instSpeed;
					info.SampleCount++;

					float avgSpeed = info.SumSpeed / Mathf.Max(1, info.SampleCount);

					if (info.AverageSpeed <= 0f)
						info.AverageSpeed = instSpeed;
					else
						info.AverageSpeed = Mathf.Lerp(info.AverageSpeed, instSpeed, SMOOTHING);

					info.LastPos = pos;
					info.LastTime = now;

					if (inDevourGrace)
						continue;

					if (!info.AlertShown && avgSpeed > SPEED_THRESHOLD)
					{
						info.AlertShown = true;

						string msg = string.Format(Loc.Anti.SuspiciousSpeedFormat, info.DisplayName, avgSpeed);
						AddAlert(msg, now + AlertDuration);
					}
				}
			}
			catch
			{
			}
		}
		public void ToggleEditPositionMode(MelonPreferences_Category prefs)
		{
			
			SetEditPositionMode(!_editPositionMode, prefs);
		}

		public void SetEditPositionMode(bool edit, MelonPreferences_Category prefs)
		{
			if (_editPositionMode == edit)
				return;

			_editPositionMode = edit;
			_isDragging = false;

			if (!edit)
			{
				if (prefAlertPosX != null) prefAlertPosX.Value = _alertPosX;
				if (prefAlertPosY != null) prefAlertPosY.Value = _alertPosY;
				prefs.SaveToFile(false);
			}
		}

		public void ResetAlertPosition(MelonPreferences_Category prefs)
		{
			_alertPosX = 120f;
			_alertPosY = 20f;

			if (prefAlertPosX != null) prefAlertPosX.Value = _alertPosX;
			if (prefAlertPosY != null) prefAlertPosY.Value = _alertPosY;

			prefs.SaveToFile(false);
		}

		public void OnGUI()
		{
			if (!Enabled && !_editPositionMode)
				return;

			if (!stylesInitialized)
				InitializeAlertStyles();

			float now = Time.time;
			for (int i = _alerts.Count - 1; i >= 0; i--)
			{
				if (_alerts[i].Until <= now)
					_alerts.RemoveAt(i);
			}

			bool hasAlerts = _alerts.Count > 0;
			if (!hasAlerts && !_editPositionMode)
				return;

			// Display text
			string titleText = "Suspicious Players";
			var lines = new List<string>();

			if (hasAlerts)
			{
				foreach (var alert in _alerts)
				{
					float timeLeft = alert.Until - now;
					string timeStr = timeLeft > 0 ? $" ({timeLeft:F0}s)" : "";
					lines.Add("• " + alert.Text + timeStr);
				}
			}
			else if (_editPositionMode)
			{
				// Position eiditng
				lines.Add("Drag this box to set alert position");
			}

			if (lines.Count == 0)
				return;

			float paddingH = 10f;
			float paddingV = 10f; 
			float spacingTitleToText = 5f;

			GUIContent titleContent = new GUIContent(titleText);
			Vector2 titleSize = alertTitleStyle.CalcSize(titleContent);

			float maxTextWidth = titleSize.x;

			foreach (var line in lines)
			{
				Vector2 s = alertTextStyle.CalcSize(new GUIContent(line));
				if (s.x > maxTextWidth)
					maxTextWidth = s.x;
			}

			float textLineHeight = alertTextStyle.CalcSize(new GUIContent("Ay")).y;

			float boxWidth = maxTextWidth + paddingH * 2f;
			float boxHeight = paddingV + titleSize.y + spacingTitleToText + lines.Count * textLineHeight + paddingV;

			float maxX = Mathf.Max(0f, Screen.width - boxWidth);
			float maxY = Mathf.Max(0f, Screen.height - boxHeight);
			_alertPosX = Mathf.Clamp(_alertPosX, 0f, maxX);
			_alertPosY = Mathf.Clamp(_alertPosY, 0f, maxY);

			Rect areaRect = new Rect(_alertPosX, _alertPosY, boxWidth, boxHeight);

			if (_editPositionMode)
			{
				Event e = Event.current;
				int controlId = GUIUtility.GetControlID(FocusType.Passive);

				switch (e.GetTypeForControl(controlId))
				{
					case EventType.MouseDown:
						if (e.button == 0 && areaRect.Contains(e.mousePosition))
						{
							GUIUtility.hotControl = controlId;
							_isDragging = true;
							_dragOffset = e.mousePosition - new Vector2(_alertPosX, _alertPosY);
							e.Use();
						}
						break;

					case EventType.MouseDrag:
						if (_isDragging && GUIUtility.hotControl == controlId && e.button == 0)
						{
							Vector2 newPos = e.mousePosition - _dragOffset;
							_alertPosX = Mathf.Clamp(newPos.x, 0f, maxX);
							_alertPosY = Mathf.Clamp(newPos.y, 0f, maxY);
							e.Use();
						}
						break;

					case EventType.MouseUp:
						if (_isDragging && GUIUtility.hotControl == controlId && e.button == 0)
						{
							_isDragging = false;
							GUIUtility.hotControl = 0;
							e.Use();
						}
						break;
				}

				areaRect = new Rect(_alertPosX, _alertPosY, boxWidth, boxHeight);
			}

			GUILayout.BeginArea(areaRect);
			GUILayout.Space(5);

			// Title 
			GUILayout.Label(titleText, alertTitleStyle);

			GUILayout.Space(5);

			// Text lines
			foreach (var line in lines)
			{
				GUILayout.Label(line, alertTextStyle);
			}

			GUILayout.EndArea();
		}

		private void InitializeAlertStyles()
		{
			// Alert title style
			alertTitleStyle = new GUIStyle(GUI.skin.label);
			alertTitleStyle.fontSize = 14;
			alertTitleStyle.fontStyle = FontStyle.Bold;
			alertTitleStyle.normal.textColor = new Color(1f, 0.85f, 0.2f);
			alertTitleStyle.alignment = TextAnchor.MiddleCenter;

			// Alert text style
			alertTextStyle = new GUIStyle(GUI.skin.label);
			alertTextStyle.fontSize = 12;
			alertTextStyle.normal.textColor = Color.white;
			alertTextStyle.alignment = TextAnchor.MiddleCenter;
			alertTextStyle.padding = new RectOffset(0, 0, 0, 0);

			stylesInitialized = true;
		}

		private void AddAlert(string text, float untilTime)
		{
			string playerName = text.Split('-')[0].Trim();
			for (int i = 0; i < _alerts.Count; i++)
			{
				if (_alerts[i].Text.StartsWith(playerName))
				{
					_alerts[i].Text = text;
					_alerts[i].Until = untilTime;
					return;
				}
			}

			_alerts.Add(new Alert
			{
				Text = text,
				Until = untilTime
			});
		}

		private string ResolvePlayerName(NolanBehaviour nolan)
		{
			try
			{
				var entity = nolan.GetComponent<BoltEntityIl2Cpp>();
				if (entity != null)
				{
					var state = entity.GetState<Il2CppPhoton.Bolt.IPlayerState>();
					if (state != null)
					{
						string name = state.PlayerName;
						if (!string.IsNullOrEmpty(name))
							return name;

						string pid = state.PlayerId;
						if (!string.IsNullOrEmpty(pid))
							return pid;
					}
				}
			}
			catch
			{
			}

			return nolan != null ? nolan.gameObject.name : Loc.Anti.UnknownName;
		}
	}
}