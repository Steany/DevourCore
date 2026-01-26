using MelonLoader;
using UnityEngine;
using System.Collections.Generic;

namespace DevourCore
{
	public class Outfit
	{
		private float outfitHue = 0f;
		private float outfitSat = 1f;
		private float outfitVal = 1f;
		private Color currentOutfitColor = Color.white;
		private bool outfitModEnabled = false;

		private Texture2D outfitColorTexture;
		private readonly List<Renderer> trackedRenderers = new List<Renderer>();
		private bool outfitsDirty = false;

		private float lastScanTime = 0f;
		private const float SCAN_INTERVAL_MENU = 0.1f;
		private const float SCAN_INTERVAL_GAME = 0.5f;

		private readonly HashSet<Renderer> processedRenderers = new HashSet<Renderer>();
		private readonly Dictionary<Transform, string> pathCache = new Dictionary<Transform, string>();
		private string currentSceneName = "";
		private bool isInMenuScene = false;

		private static readonly int ID_Color = Shader.PropertyToID("_Color");
		private static readonly int ID_BaseColor = Shader.PropertyToID("_BaseColor");
		private static readonly int ID_MainColor = Shader.PropertyToID("_MainColor");
		private static readonly int ID_TintColor = Shader.PropertyToID("_TintColor");
		private static readonly int ID_EmissionColor = Shader.PropertyToID("_EmissionColor");
		private readonly MaterialPropertyBlock mpb = new MaterialPropertyBlock();

		private readonly HashSet<string> learnedMaterialNames = new HashSet<string>();
		private bool hasLearnedOutfits = false;
		private float lastLobbyLearnTime = 0f;
		private const float LOBBY_LEARN_INTERVAL = 1f;

		private MelonPreferences_Entry<float> prefOutfitHue, prefOutfitSat, prefOutfitVal;
		private MelonPreferences_Entry<bool> prefOutfitModEnabled;
		private Renderer[] rendererCache = null;
		private int rendererCacheScanIndex = 0;
		private float rendererCacheLastRefreshTime = 0f;
		private const float RENDERER_CACHE_LIFETIME = 10f;
		private const int RENDERERS_PER_STEP = 32;
		private float sceneEnterTime = 0f;
		private const float NON_MENU_SCAN_DURATION = 20f;
		private bool nonMenuScanExpired = false;

		public bool OutfitModEnabled => outfitModEnabled;
		public float OutfitHue => outfitHue;
		public float OutfitSat => outfitSat;
		public float OutfitVal => outfitVal;
		public Texture2D OutfitColorTexture => outfitColorTexture;

		public void Initialize(MelonPreferences_Category prefs)
		{
			prefOutfitHue = prefs.CreateEntry("OutfitHue", 1f);
			prefOutfitSat = prefs.CreateEntry("OutfitSaturation", 1f);
			prefOutfitVal = prefs.CreateEntry("OutfitValue", 0f);
			prefOutfitModEnabled = prefs.CreateEntry("OutfitModEnabled", false);

			outfitHue = prefOutfitHue.Value;
			outfitSat = prefOutfitSat.Value;
			outfitVal = prefOutfitVal.Value;
			outfitModEnabled = prefOutfitModEnabled.Value;

			currentOutfitColor = Color.HSVToRGB(outfitHue, outfitSat, outfitVal);

			EnsureOutfitColorTexture();
			outfitsDirty = outfitModEnabled;
		}

		public void ApplyFirstRunDefaults(MelonPreferences_Category prefs)
		{
			SetEnabled(false, prefs);

			outfitHue = 0f;
			outfitSat = 1f;
			outfitVal = 1f;

			prefOutfitHue.Value = outfitHue;
			prefOutfitSat.Value = outfitSat;
			prefOutfitVal.Value = outfitVal;

			currentOutfitColor = Color.HSVToRGB(outfitHue, outfitSat, outfitVal);

			EnsureOutfitColorTexture();
			outfitColorTexture.SetPixel(0, 0, currentOutfitColor);
			outfitColorTexture.Apply();
		}

		public void OnSceneLoaded(string sceneName)
		{
			currentSceneName = sceneName;
			isInMenuScene = (sceneName == "Menu");

			trackedRenderers.Clear();
			pathCache.Clear();
			processedRenderers.Clear();
			rendererCache = null;
			rendererCacheScanIndex = 0;
			rendererCacheLastRefreshTime = 0f;

			sceneEnterTime = Time.time;
			nonMenuScanExpired = false;

			currentOutfitColor = Color.HSVToRGB(outfitHue, outfitSat, outfitVal);
			outfitsDirty = outfitModEnabled;

			if (isInMenuScene)
			{
				hasLearnedOutfits = false;
				lastLobbyLearnTime = Time.time;
				learnedMaterialNames.Clear();
			}
		}

		public void OnUpdate()
		{
			if (!outfitModEnabled)
				return;

			bool isSlaughterhouse = string.Equals(currentSceneName, "Slaughterhouse", System.StringComparison.OrdinalIgnoreCase);

			if (isInMenuScene && Time.time - lastLobbyLearnTime > LOBBY_LEARN_INTERVAL)
			{
				lastLobbyLearnTime = Time.time;
				LearnOutfitMaterialsFromLobby_Simple();
			}

			if (!isInMenuScene && !isSlaughterhouse)
			{
				if (!nonMenuScanExpired && Time.time - sceneEnterTime > NON_MENU_SCAN_DURATION)
				{
					nonMenuScanExpired = true;
				}
			}

			float scanInterval = isInMenuScene ? SCAN_INTERVAL_MENU : SCAN_INTERVAL_GAME;
			if (Time.time - lastScanTime >= scanInterval)
			{
				lastScanTime = Time.time;

				if (isInMenuScene)
				{
					ScanForOutfits_Simple();
					outfitsDirty = true;
				}
				else if (isSlaughterhouse)
				{
					ScanForOutfits_Incremental();
					outfitsDirty = true;
				}
				else
				{
					if (!nonMenuScanExpired)
					{
						ScanForOutfits_Simple();
						outfitsDirty = true;
					}
				}
			}

			if (outfitsDirty)
			{
				ApplyColorToOutfits();
				outfitsDirty = false;
			}

			if (Time.frameCount % 600 == 0)
			{
				CleanupNullReferences();
			}
		}

		public void SetEnabled(bool enabled, MelonPreferences_Category prefs)
		{
			outfitModEnabled = enabled;
			prefOutfitModEnabled.Value = enabled;
			prefs.SaveToFile(false);

			if (outfitModEnabled)
			{
				currentOutfitColor = Color.HSVToRGB(outfitHue, outfitSat, outfitVal);
				processedRenderers.Clear();
				trackedRenderers.Clear();

				if (isInMenuScene)
					ScanForOutfits_Simple();
				else if (string.Equals(currentSceneName, "Slaughterhouse", System.StringComparison.OrdinalIgnoreCase))
					ScanForOutfits_Incremental();
				else
					ScanForOutfits_Simple();

				ApplyColorToOutfits();
			}
			else
			{
				foreach (var r in trackedRenderers)
				{
					if (r != null)
					{
						var mats = r.sharedMaterials;
						if (mats != null)
						{
							for (int i = 0; i < mats.Length; i++)
							{
								r.SetPropertyBlock(null, i);
							}
						}
					}
				}

				processedRenderers.Clear();
				trackedRenderers.Clear();
				outfitsDirty = false;
			}
		}

		public void SetHSV(float h, float s, float v, MelonPreferences_Category prefs)
		{
			outfitHue = h;
			outfitSat = s;
			outfitVal = v;

			currentOutfitColor = Color.HSVToRGB(outfitHue, outfitSat, outfitVal);

			prefOutfitHue.Value = outfitHue;
			prefOutfitSat.Value = outfitSat;
			prefOutfitVal.Value = outfitVal;
			prefs.SaveToFile(false);

			EnsureOutfitColorTexture();
			outfitColorTexture.SetPixel(0, 0, currentOutfitColor);
			outfitColorTexture.Apply();
			outfitsDirty = outfitModEnabled;
		}

		private void LearnOutfitMaterialsFromLobby_Simple()
		{
			var allRenderers = UnityEngine.Object.FindObjectsOfType<Renderer>();

			foreach (var renderer in allRenderers)
			{
				if (renderer == null) continue;

				if (renderer.sharedMaterials != null)
				{
					foreach (var mat in renderer.sharedMaterials)
					{
						if (mat != null)
						{
							string matName = mat.name.Replace(" (Instance)", "").Trim();
							string matLower = matName.ToLower();

							if (matLower.Contains("robe") &&
								!matLower.Contains("sandal") &&
								!matLower.Contains("shoe") &&
								!matLower.Contains("body") &&
								!matLower.Contains("splash") &&
								!matLower.Contains("rain") &&
								!matLower.Contains("wind") &&
								!matLower.Contains("particle") &&
								!matLower.Contains("snow") &&
								!matLower.Contains("leaf") &&
								!matLower.Contains("debris"))
							{
								learnedMaterialNames.Add(matName);
							}
						}
					}
				}
			}

			if (learnedMaterialNames.Count > 0)
			{
				hasLearnedOutfits = true;
			}
		}

		private bool IsLearnedOutfitMaterial(Renderer renderer)
		{
			if (isInMenuScene)
				return true;

			if (!hasLearnedOutfits || learnedMaterialNames.Count == 0)
				return false;

			var materials = renderer.sharedMaterials;
			if (materials == null || materials.Length == 0)
				return false;

			foreach (var mat in materials)
			{
				if (mat != null)
				{
					string matName = mat.name.Replace(" (Instance)", "").Trim();
					if (learnedMaterialNames.Contains(matName))
						return true;
				}
			}

			return false;
		}

		private void CleanupNullReferences()
		{
			for (int i = trackedRenderers.Count - 1; i >= 0; i--)
			{
				if (trackedRenderers[i] == null)
				{
					trackedRenderers.RemoveAt(i);
				}
			}

			processedRenderers.RemoveWhere(r => r == null);

			if (pathCache.Count > 1000)
			{
				pathCache.Clear();
			}
		}

		private bool IsChildOfPlayerObject(GameObject obj)
		{
			if (obj == null) return false;

			Transform current = obj.transform;
			while (current != null)
			{
				if (current.CompareTag("Player"))
					return true;

				current = current.parent;
			}

			return false;
		}

		private bool IsRobeMaterialName(string matName)
		{
			string matLower = matName.ToLower();

			if (!matLower.Contains("robe"))
				return false;

			if (matLower.Contains("sandal")) return false;
			if (matLower.Contains("shoe")) return false;

			return true;
		}

		private void ScanForOutfits_Simple()
		{
			var allRenderers = UnityEngine.Object.FindObjectsOfType<Renderer>();

			foreach (var renderer in allRenderers)
			{
				if (renderer == null || processedRenderers.Contains(renderer))
					continue;

				processedRenderers.Add(renderer);

				if (!IsChildOfPlayerObject(renderer.gameObject))
					continue;

				if (!trackedRenderers.Contains(renderer))
				{
					if (renderer.sharedMaterials != null)
					{
						foreach (var mat in renderer.sharedMaterials)
						{
							if (mat != null)
							{
								string matName = mat.name.Replace(" (Instance)", "").Trim();

								if (IsRobeMaterialName(matName))
								{
									if (IsLearnedOutfitMaterial(renderer))
									{
										trackedRenderers.Add(renderer);
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		private Renderer[] GetOrRefreshRendererCache()
		{
			if (rendererCache == null || Time.time - rendererCacheLastRefreshTime > RENDERER_CACHE_LIFETIME)
			{
				rendererCache = UnityEngine.Object.FindObjectsOfType<Renderer>();
				rendererCacheLastRefreshTime = Time.time;
				rendererCacheScanIndex = 0;
			}
			return rendererCache;
		}

		private void ScanForOutfits_Incremental()
		{
			var allRenderers = GetOrRefreshRendererCache();
			if (allRenderers == null || allRenderers.Length == 0)
				return;

			int processedThisCall = 0;
			int length = allRenderers.Length;

			while (rendererCacheScanIndex < length && processedThisCall < RENDERERS_PER_STEP)
			{
				var renderer = allRenderers[rendererCacheScanIndex++];
				if (renderer == null || processedRenderers.Contains(renderer))
					continue;

				processedRenderers.Add(renderer);

				if (!IsChildOfPlayerObject(renderer.gameObject))
					continue;

				if (!trackedRenderers.Contains(renderer))
				{
					if (renderer.sharedMaterials != null)
					{
						foreach (var mat in renderer.sharedMaterials)
						{
							if (mat != null)
							{
								string matName = mat.name.Replace(" (Instance)", "").Trim();

								if (IsRobeMaterialName(matName))
								{
									if (IsLearnedOutfitMaterial(renderer))
									{
										trackedRenderers.Add(renderer);
										break;
									}
								}
							}
						}
					}
				}

				processedThisCall++;
			}
		}

		private string GetFullPath(Transform transform)
		{
			if (pathCache.TryGetValue(transform, out string cachedPath))
				return cachedPath;

			System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
			Transform current = transform;

			while (current != null)
			{
				if (sb.Length > 0)
					sb.Insert(0, '/');
				sb.Insert(0, current.name);
				current = current.parent;
			}

			string path = sb.ToString();
			pathCache[transform] = path;
			return path;
		}

		private void ApplyColorToOutfits()
		{
			for (int i = 0; i < trackedRenderers.Count; i++)
			{
				var r = trackedRenderers[i];
				if (r != null)
					ApplyColorToRenderer(r);
			}
		}

		private bool ApplyColorToRenderer(Renderer renderer)
		{
			if (renderer == null) return false;

			var mats = renderer.sharedMaterials;
			if (mats == null || mats.Length == 0) return false;

			if (outfitModEnabled)
			{
				Color c = currentOutfitColor;
				c.a = 1f;

				bool changedAny = false;

				for (int i = 0; i < mats.Length; i++)
				{
					var mat = mats[i];
					if (mat == null) continue;

					string matName = mat.name.Replace(" (Instance)", "").Trim();

					if (!IsRobeMaterialName(matName))
						continue;

					mpb.Clear();
					mpb.SetColor(ID_Color, c);
					mpb.SetColor(ID_BaseColor, c);
					mpb.SetColor(ID_MainColor, c);
					mpb.SetColor(ID_TintColor, c);
					mpb.SetColor(ID_EmissionColor, c);
					renderer.SetPropertyBlock(mpb, i);
					changedAny = true;
				}

				return changedAny;
			}
			else
			{
				for (int i = 0; i < mats.Length; i++)
				{
					renderer.SetPropertyBlock(null, i);
				}
				return false;
			}
		}

		private void EnsureOutfitColorTexture()
		{
			if (outfitColorTexture != null) return;

			currentOutfitColor = Color.HSVToRGB(outfitHue, outfitSat, outfitVal);
			outfitColorTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			outfitColorTexture.SetPixel(0, 0, currentOutfitColor);
			outfitColorTexture.Apply();
			outfitColorTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
			UnityEngine.Object.DontDestroyOnLoad(outfitColorTexture);
		}
	}
}