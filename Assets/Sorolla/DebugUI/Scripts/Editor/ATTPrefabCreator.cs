using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.ATT.Editor
{
    /// <summary>
    ///     Base class for ATT popup prefab creators.
    ///     Provides common helper methods for UI construction.
    /// </summary>
    public abstract class ATTPrefabCreator
    {
        protected const string PREFAB_PATH = "Assets/Resources/";

        // Colors
        protected static readonly Color OverlayColor = new(0, 0, 0, 0.7f);
        protected static readonly Color PanelColor = new(0.15f, 0.15f, 0.18f, 1f);
        protected static readonly Color TextPrimary = new(1f, 1f, 1f, 1f);
        protected static readonly Color TextSecondary = new(0.7f, 0.7f, 0.7f, 1f);
        protected static readonly Color ButtonPrimary = new(0.2f, 0.5f, 1f, 1f);
        protected static readonly Color ButtonSecondary = new(0.3f, 0.3f, 0.35f, 1f);

        protected static void EnsureDirectoriesExist()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
        }

        protected static GameObject CreateCanvasRoot(string name)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        protected static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        protected static Image AddImage(GameObject go, Color color)
        {
            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }

        protected static TextMeshProUGUI AddText(GameObject go, string text, float fontSize, Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center)
        {
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.enableAutoSizing = false;
            tmp.overflowMode = TextOverflowModes.Overflow;
            return tmp;
        }

        protected static void SetStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        protected static Button AddButton(GameObject go, Color bgColor)
        {
            AddImage(go, bgColor);
            var btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            return btn;
        }

        protected static void SavePrefab(GameObject go, string fileName)
        {
            EnsureDirectoriesExist();
            string path = PREFAB_PATH + fileName + ".prefab";

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
                AssetDatabase.DeleteAsset(path);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"[Sorolla:ATT] Created prefab: {path}");
        }
    }
}
