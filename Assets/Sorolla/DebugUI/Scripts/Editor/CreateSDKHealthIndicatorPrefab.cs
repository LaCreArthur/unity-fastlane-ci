using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateSDKHealthIndicatorPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create SDK Health Indicator")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("SDKHealthIndicator");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(160, 48);

            // Background
            Image bg = AddImage(root, Theme.cardBackground);
            bg.type = Image.Type.Sliced;

            // Horizontal layout
            AddHorizontalLayout(root, 8,
                new RectOffset(16, 16, 12, 12));

            // SDK Name Label
            GameObject labelObj = CreateUIObject("SDKNameLabel", root.transform);
            TextMeshProUGUI tmp = AddText(labelObj, "GameAnalytics", Theme.bodySize,
                Theme.textPrimary);
            AddLayoutElement(labelObj, flexibleWidth: 1, preferredHeight: 20);

            // Status Dot
            GameObject dotObj = CreateUIObject("StatusDot", root.transform);
            Image dotImg = AddImage(dotObj, Theme.statusActive);
            AddLayoutElement(dotObj, preferredWidth: 10, preferredHeight: 10);

            // Add component
            var indicator = root.AddComponent<SDKHealthIndicator>();

            var so = new SerializedObject(indicator);
            so.FindProperty("_background").objectReferenceValue = bg;
            so.FindProperty("_sdkNameLabel").objectReferenceValue = tmp;
            so.FindProperty("_statusDot").objectReferenceValue = dotImg;
            so.ApplyModifiedProperties();

            SavePrefab(root, "SDKHealthIndicator");
        }
    }
}
