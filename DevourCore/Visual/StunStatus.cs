using MelonLoader;
using System;
using System.Collections;
using System.Reflection;
using Il2Cpp;
using UnityEngine;

namespace DevourCore
{
	public sealed class StunStatusIndicator
	{
		private readonly string[] validMaps =
		{
			"Devour", "Molly", "Inn", "Town", "Slaughterhouse", "Manor", "Carnival"
		};

		private bool inValidMap = false;
		private bool isRefreshing = false;

		private SurvivalAzazelBehaviour currentAzazel = null;
		private PropertyInfo propIsEnraged = null;
		private PropertyInfo propHasRedEyes = null;

		private bool isEnraged = false;
		private bool hasRedEyes = false;
		private bool flagsInitialized = false;

		private enum DemonStatus
		{
			Unknown,
			Calm,
			Enraged,
			RedEyes
		}

		private DemonStatus currentStatus = DemonStatus.Unknown;

		private bool guiStylesInitialized = false;
		private GUIStyle statusStyle;
		private float lastAppliedScale = -1f;

		private float barWidth = 150f;
		private float barHeight = 22f;
		private const float BASE_BAR_WIDTH = 150f;
		private const float BASE_BAR_HEIGHT = 22f;

		private MelonPreferences_Category prefs;
		private MelonPreferences_Entry<float> prefPosX;
		private MelonPreferences_Entry<float> prefPosY;
		private float posX = 0f;
		private float posY = 18f;

		private bool wasEditing = false;
		private bool isDragging = false;
		private Vector2 dragOffset;

		private bool stickRight = false;
		private bool stickBottom = false;
		private const float EDGE_EPS = 1.5f;
		private bool lastHideBg = false;


		private Anticheat anticheat;
		private HudManager hudManager;

		public void Initialize(MelonPreferences_Category prefsCategory, Anticheat anticheatRef, HudManager hud)
		{
			prefs = prefsCategory;
			anticheat = anticheatRef;
			hudManager = hud;

			prefPosX = prefs.CreateEntry("StunStatusPosX", 876.08826f);
			prefPosY = prefs.CreateEntry("StunStatusPosY", 8f);

			posX = prefPosX.Value;
			posY = prefPosY.Value;


			if (Mathf.Approximately(posX, 0f) && Mathf.Approximately(posY, 18f))
			{
				posX = -1f;
				posY = 28f;
				prefPosX.Value = posX;
				prefPosY.Value = posY;
				prefs?.SaveToFile(false);
			}
		}

		private bool IsEditMode => anticheat != null && anticheat.IsEditingPosition;

		public void OnSceneLoaded(string sceneName, bool inValidMap)
		{

			this.inValidMap = inValidMap;

			currentAzazel = null;
			propIsEnraged = null;
			propHasRedEyes = null;

			isEnraged = false;
			hasRedEyes = false;
			flagsInitialized = false;
			currentStatus = DemonStatus.Unknown;

			if (this.inValidMap)
			{
				MelonCoroutines.Start(RefreshLoop());
			}
		}

		private IEnumerator RefreshLoop()
		{
			if (isRefreshing) yield break;
			isRefreshing = true;

			yield return new WaitForSeconds(1.5f);

			while (inValidMap)
			{
				FindAzazel();
				yield return new WaitForSeconds(2f);
			}

			isRefreshing = false;
		}

		private void FindAzazel()
		{
			try
			{
				if (currentAzazel != null && currentAzazel.gameObject != null)
					return;

				var azazels = SurvivalAzazelBehaviour.FindObjectsOfType<SurvivalAzazelBehaviour>();
				if (azazels == null || azazels.Length == 0) return;

				foreach (var az in azazels)
				{
					if (az == null || az.gameObject == null) continue;

					currentAzazel = az;

					Type t = az.GetType();
					BindingFlags flags =
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.Instance | BindingFlags.FlattenHierarchy;

					propIsEnraged = t.GetProperty("m_IsEnraged", flags);
					propHasRedEyes = t.GetProperty("hasEnragedEyes", flags);
					break;
				}
			}
			catch
			{
			}
		}

		public void OnUpdate()
		{
			bool editing = IsEditMode;
			if (wasEditing && !editing && prefs != null)
			{
				prefs.SaveToFile(false);
			}
			wasEditing = editing;

			bool forceShow = editing;

			if (!inValidMap && !forceShow) return;

			ReadFlags();
			UpdateStatus();
		}

		private void ReadFlags()
		{
			bool curEnraged = false;
			bool curRedEyes = false;

			if (currentAzazel != null)
			{
				try
				{
					if (propIsEnraged != null)
					{
						object v = propIsEnraged.GetValue(currentAzazel);
						if (v != null) curEnraged = Convert.ToBoolean(v);
					}
					if (propHasRedEyes != null)
					{
						object v2 = propHasRedEyes.GetValue(currentAzazel);
						if (v2 != null) curRedEyes = Convert.ToBoolean(v2);
					}
				}
				catch
				{
				}
			}

			if (!flagsInitialized)
				flagsInitialized = true;

			isEnraged = curEnraged;
			hasRedEyes = curRedEyes;
		}

		private void UpdateStatus()
		{
			if (!flagsInitialized || currentAzazel == null)
			{
				currentStatus = DemonStatus.Unknown;
				return;
			}

			if (hasRedEyes && isEnraged)
				currentStatus = DemonStatus.RedEyes;
			else if (isEnraged)
				currentStatus = DemonStatus.Enraged;
			else
				currentStatus = DemonStatus.Calm;
		}

		public void OnGUI()
		{
			bool editing = IsEditMode;
			bool forceShow = editing;

			if (!inValidMap && !forceShow) return;

			EventType t = Event.current.type;
			if (t != EventType.Repaint &&
				t != EventType.MouseDown &&
				t != EventType.MouseDrag &&
				t != EventType.MouseUp)
				return;

			if (!guiStylesInitialized)
				InitGuiStyles();

			ApplyHudScale();

			DrawStatusBar(editing);
		}

		private void ApplyHudScale()
		{
			float s = hudManager != null ? hudManager.HudScale : HudManager.GlobalHudScale;
			s = Mathf.Clamp(s, 1f, 2f);

			barWidth = BASE_BAR_WIDTH * s;
			barHeight = BASE_BAR_HEIGHT * s;

			if (statusStyle != null && !Mathf.Approximately(lastAppliedScale, s))
			{
				statusStyle.fontSize = Mathf.RoundToInt(13f * s);
				lastAppliedScale = s;
			}
		}

		private void InitGuiStyles()
		{
			try
			{
				statusStyle = new GUIStyle(GUI.skin.label)
				{
					alignment = TextAnchor.MiddleCenter,
					fontStyle = FontStyle.Bold,
					fontSize = Mathf.RoundToInt(13f * (hudManager != null ? hudManager.HudScale : HudManager.GlobalHudScale)),
					richText = true,
					clipping = TextClipping.Overflow,
					padding = new RectOffset(2, 2, 0, 0)
				};
				statusStyle.normal.textColor = Color.white;

				guiStylesInitialized = true;
			}
			catch
			{
			}
		}

		private void DrawStatusBar(bool editing)
		{
			bool hideBg = hudManager != null && hudManager.HideStunBackground;

			float rectX = posX < 0f ? (Screen.width - barWidth) * 0.5f : posX;
			Rect rect = new Rect(rectX, posY, barWidth, barHeight);

			Color bgColor;
			Color borderColor;
			string text;
			Color textColor;

			switch (currentStatus)
			{
				case DemonStatus.Calm:
					bgColor = new Color(0.04f, 0.07f, 0.14f, 0.90f);
					borderColor = new Color(0.22f, 0.40f, 0.80f, 1.00f);
					text = Loc.GUI.Stun_Calm;
					textColor = new Color(0.75f, 0.92f, 1f);
					break;

				case DemonStatus.Enraged:
					bgColor = new Color(0.16f, 0.01f, 0.01f, 0.93f);
					borderColor = new Color(0.90f, 0.16f, 0.16f, 1.00f);
					text = Loc.GUI.Stun_Enraged;
					textColor = new Color(1f, 0.45f, 0.35f);
					break;

				case DemonStatus.RedEyes:
					bgColor = new Color(0.19f, 0.00f, 0.00f, 0.96f);
					borderColor = new Color(1.00f, 0.12f, 0.16f, 1.00f);
					text = Loc.GUI.Stun_RedEyes;
					textColor = new Color(1f, 0.32f, 0.35f);
					break;

				default:
					bgColor = new Color(0.04f, 0.07f, 0.14f, 0.90f);
					borderColor = new Color(0.22f, 0.40f, 0.80f, 1.00f);
					text = Loc.GUI.Stun_Calm;
					textColor = new Color(0.75f, 0.92f, 1f);
					break;
			}

			Rect hitboxRect = rect;
			if (hideBg && statusStyle != null)
			{
				Vector2 textSize = statusStyle.CalcSize(new GUIContent(text));
				float horizPad = statusStyle.padding != null ? (statusStyle.padding.left + statusStyle.padding.right) : 0f;
				float vertPad = statusStyle.padding != null ? (statusStyle.padding.top + statusStyle.padding.bottom) : 0f;
				float w = textSize.x + horizPad;
				float h = textSize.y + vertPad;
				hitboxRect = new Rect(
					rect.x + (rect.width - w) * 0.5f,
					rect.y + (rect.height - h) * 0.5f,
					w,
					h
				);
			}

			lastHideBg = hideBg;


			if (!editing)
			{
				float maxX = Mathf.Max(0f, Screen.width - rect.width);
				float maxY = Mathf.Max(0f, Screen.height - rect.height);

				if (posX >= 0f && Mathf.Abs(posX - maxX) <= EDGE_EPS)
					rect.x = maxX;
				if (Mathf.Abs(posY - maxY) <= EDGE_EPS)
					rect.y = maxY;
			}


			if (editing)
			{
				HandleDrag(ref rect, hitboxRect);
			}


			float maxXClamp = Mathf.Max(0f, Screen.width - rect.width);
			float maxYClamp = Mathf.Max(0f, Screen.height - rect.height);
			rect.x = Mathf.Clamp(rect.x, 0f, maxXClamp);
			rect.y = Mathf.Clamp(rect.y, 0f, maxYClamp);


			if (editing)
			{
				if (posX >= 0f)
					stickRight = Mathf.Abs(rect.x - maxXClamp) <= EDGE_EPS;
				stickBottom = Mathf.Abs(rect.y - maxYClamp) <= EDGE_EPS;
			}


			posX = rect.x;
			posY = rect.y;
			if (prefPosX != null) prefPosX.Value = posX;
			if (prefPosY != null) prefPosY.Value = posY;

			if (!hideBg)
			{
				Color prevCol = GUI.color;

				GUI.color = bgColor;
				GUI.DrawTexture(rect, Texture2D.whiteTexture);

				GUI.color = borderColor;

				GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1.5f), Texture2D.whiteTexture);
				GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1.5f, rect.width, 1.5f), Texture2D.whiteTexture);

				float sideLen = 8f;

				GUI.DrawTexture(new Rect(rect.x, rect.y, sideLen, 1.5f), Texture2D.whiteTexture);
				GUI.DrawTexture(new Rect(rect.x, rect.y, 1.5f, sideLen), Texture2D.whiteTexture);

				GUI.DrawTexture(new Rect(rect.xMax - sideLen, rect.y, sideLen, 1.5f), Texture2D.whiteTexture);
				GUI.DrawTexture(new Rect(rect.xMax - 1.5f, rect.y, 1.5f, sideLen), Texture2D.whiteTexture);

				GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1.5f, sideLen, 1.5f), Texture2D.whiteTexture);
				GUI.DrawTexture(new Rect(rect.x, rect.yMax - sideLen, 1.5f, sideLen), Texture2D.whiteTexture);

				GUI.DrawTexture(new Rect(rect.xMax - sideLen, rect.yMax - 1.5f, sideLen, 1.5f), Texture2D.whiteTexture);
				GUI.DrawTexture(new Rect(rect.xMax - 1.5f, rect.yMax - sideLen, 1.5f, sideLen), Texture2D.whiteTexture);

				GUI.color = prevCol;
			}

			DrawOutlinedCenteredText(rect, text, statusStyle, textColor, new Color(0f, 0f, 0f, 0.9f), 1);
		}

		private void HandleDrag(ref Rect rect, Rect hitbox)
		{
			Event e = Event.current;
			int controlId = GUIUtility.GetControlID(FocusType.Passive);

			switch (e.GetTypeForControl(controlId))
			{
				case EventType.MouseDown:
					if (e.button == 0 && hitbox.Contains(e.mousePosition))
					{
						GUIUtility.hotControl = controlId;
						isDragging = true;
						dragOffset = e.mousePosition - new Vector2(rect.x, rect.y);
						e.Use();
					}
					break;

				case EventType.MouseDrag:
					if (isDragging && GUIUtility.hotControl == controlId && e.button == 0)
					{
						Vector2 newPos = e.mousePosition - dragOffset;
						float maxX = Mathf.Max(0f, Screen.width - rect.width);
						float maxY = Mathf.Max(0f, Screen.height - rect.height);
						rect.x = Mathf.Clamp(newPos.x, 0f, maxX);
						rect.y = Mathf.Clamp(newPos.y, 0f, maxY);

						if (prefPosX != null) prefPosX.Value = rect.x;
						if (prefPosY != null) prefPosY.Value = rect.y;

						e.Use();
					}
					break;

				case EventType.MouseUp:
					if (isDragging && GUIUtility.hotControl == controlId && e.button == 0)
					{
						isDragging = false;
						if (GUIUtility.hotControl == controlId)
							GUIUtility.hotControl = 0;
						e.Use();
					}
					break;
			}
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


		public void ApplyFirstRunDefaults(MelonPreferences_Category prefsCategory)
		{
			if (prefsCategory == null) return;
			if (prefPosX != null) prefPosX.Value = 876.08826f;
			if (prefPosY != null) prefPosY.Value = 8f;
			posX = (prefPosX != null) ? prefPosX.Value : posX;
			posY = (prefPosY != null) ? prefPosY.Value : posY;
			prefsCategory.SaveToFile(false);
		}



		public void ResetPositionToDefaults(MelonPreferences_Category prefs)
		{
			const float DEFAULT_X = 876.08826f;
			const float DEFAULT_Y = 8f;

			posX = DEFAULT_X;
			posY = DEFAULT_Y;

			if (prefPosX != null) prefPosX.Value = posX;
			if (prefPosY != null) prefPosY.Value = posY;
			prefs?.SaveToFile(false);
		}
	}
}