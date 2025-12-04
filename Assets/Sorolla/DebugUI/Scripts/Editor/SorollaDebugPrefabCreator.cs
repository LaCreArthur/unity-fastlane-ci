using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    /// <summary>
    ///     Base class for prefab creation utilities.
    ///     Provides common helper methods for UI construction.
    /// </summary>
    public abstract class SorollaDebugPrefabCreator
    {
        protected const string PREFAB_PATH = "Assets/Sorolla/DebugUI/Prefabs/";
        protected const string RESOURCES_PATH = "Assets/Sorolla/DebugUI/Resources/";

        protected static SorollaDebugTheme _theme;
        protected static SorollaDebugTheme Theme
        {
            get {
                if (_theme == null)
                {
                    _theme = AssetDatabase.LoadAssetAtPath<SorollaDebugTheme>(
                        RESOURCES_PATH + "SorollaDebugTheme.asset");

                    if (_theme == null)
                    {
                        Debug.LogError("SorollaDebugTheme not found!  Create it first via Sorolla/Debug UI/Create Theme");
                    }
                }
                return _theme;
            }
        }

        // Helper: Ensure directories exist
        protected static void EnsureDirectoriesExist()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla"))
                AssetDatabase.CreateFolder("Assets", "Sorolla");
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla/DebugUI"))
                AssetDatabase.CreateFolder("Assets/Sorolla", "DebugUI");
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla/DebugUI/Prefabs"))
                AssetDatabase.CreateFolder("Assets/Sorolla/DebugUI", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla/DebugUI/Resources"))
                AssetDatabase.CreateFolder("Assets/Sorolla/DebugUI", "Resources");
        }

        // Helper: Create basic GameObject with RectTransform
        protected static GameObject CreateUIObject(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent == null)
            {
                // Find the Canvas
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    EditorUtility.DisplayDialog("Error", "No Canvas found in scene. Please create a Canvas first.", "OK");
                    return null;
                }
                parent = canvas.transform;
            }
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        // Helper: Create Image component
        protected static Image AddImage(GameObject go, Color color, Sprite sprite = null)
        {
            var img = go.AddComponent<Image>();
            img.color = color;
            img.sprite = sprite;
            if (sprite != null)
            {
                img.type = Image.Type.Sliced;
            }
            return img;
        }

        // Helper: Create TextMeshProUGUI component
        protected static TextMeshProUGUI AddText(GameObject go, string text, float fontSize,
            Color color, FontStyles style = FontStyles.Normal, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.fontStyle = style;
            tmp.alignment = alignment;
            tmp.enableAutoSizing = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            return tmp;
        }

        // Helper: Setup RectTransform to stretch
        protected static void SetStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Helper: Setup RectTransform with anchors and size
        protected static void SetRectTransform(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 pivot, Vector2 sizeDelta, Vector2 anchoredPosition)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.sizeDelta = sizeDelta;
            rt.anchoredPosition = anchoredPosition;
        }

        // Helper: Add Horizontal Layout Group
        protected static HorizontalLayoutGroup AddHorizontalLayout(GameObject go, float spacing = 0,
            RectOffset padding = null, TextAnchor childAlignment = TextAnchor.MiddleLeft,
            bool childForceExpandWidth = false, bool childForceExpandHeight = false,
            bool childControlWidth = true, bool childControlHeight = true)
        {
            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = spacing;
            hlg.padding = padding ?? new RectOffset();
            hlg.childAlignment = childAlignment;
            hlg.childForceExpandWidth = childForceExpandWidth;
            hlg.childForceExpandHeight = childForceExpandHeight;
            hlg.childControlWidth = childControlWidth;
            hlg.childControlHeight = childControlHeight;
            return hlg;
        }

        // Helper: Add Vertical Layout Group
        protected static VerticalLayoutGroup AddVerticalLayout(GameObject go, float spacing = 0,
            RectOffset padding = null, TextAnchor childAlignment = TextAnchor.UpperLeft,
            bool childForceExpandWidth = true, bool childForceExpandHeight = false,
            bool childControlWidth = true, bool childControlHeight = true)
        {
            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = spacing;
            vlg.padding = padding ?? new RectOffset();
            vlg.childAlignment = childAlignment;
            vlg.childForceExpandWidth = childForceExpandWidth;
            vlg.childForceExpandHeight = childForceExpandHeight;
            vlg.childControlWidth = childControlWidth;
            vlg.childControlHeight = childControlHeight;
            return vlg;
        }

        // Helper: Add Layout Element
        protected static LayoutElement AddLayoutElement(GameObject go,
            float minWidth = -1, float minHeight = -1,
            float preferredWidth = -1, float preferredHeight = -1,
            float flexibleWidth = -1, float flexibleHeight = -1,
            bool ignoreLayout = false)
        {
            var le = go.AddComponent<LayoutElement>();
            le.minWidth = minWidth;
            le.minHeight = minHeight;
            le.preferredWidth = preferredWidth;
            le.preferredHeight = preferredHeight;
            le.flexibleWidth = flexibleWidth;
            le.flexibleHeight = flexibleHeight;
            le.ignoreLayout = ignoreLayout;
            return le;
        }

        // Helper: Add Content Size Fitter
        protected static ContentSizeFitter AddContentSizeFitter(GameObject go,
            ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained,
            ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.PreferredSize)
        {
            var csf = go.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = horizontalFit;
            csf.verticalFit = verticalFit;
            return csf;
        }

        // Helper: Save prefab
        protected static void SavePrefab(GameObject go, string fileName)
        {
            EnsureDirectoriesExist();
            string path = PREFAB_PATH + fileName + ".prefab";

            // Remove existing
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existingPrefab != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);

            Debug.Log($"Created prefab: {path}");
        }
    }
}
