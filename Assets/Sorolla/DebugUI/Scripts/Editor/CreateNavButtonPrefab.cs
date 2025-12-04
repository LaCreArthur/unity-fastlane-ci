using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateNavButtonPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Nav Button")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("NavButton");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(72, 64);

            AddLayoutElement(root, flexibleWidth: 1, minHeight: 64);

            // Background Highlight (shown when selected)
            GameObject highlight = CreateUIObject("BackgroundHighlight", root.transform);
            var highlightRt = highlight.GetComponent<RectTransform>();
            highlightRt.anchorMin = new Vector2(0.5f, 0.5f);
            highlightRt.anchorMax = new Vector2(0.5f, 0.5f);
            highlightRt.sizeDelta = new Vector2(64, 56);
            Image highlightImg = AddImage(highlight, new Color(Theme.accentPurple.r, Theme.accentPurple.g, Theme.accentPurple.b, 0.2f));
            highlightImg.type = Image.Type.Sliced;
            highlight.SetActive(false);

            // Content container (vertical)
            GameObject content = CreateUIObject("Content", root.transform);
            var contentRt = content.GetComponent<RectTransform>();
            SetStretch(contentRt);

            AddVerticalLayout(content, 4,
                new RectOffset(0, 0, 8, 8),
                TextAnchor.MiddleCenter);

            // Icon
            GameObject iconObj = CreateUIObject("Icon", content.transform);
            Image iconImg = AddImage(iconObj, Theme.textSecondary);
            iconImg.preserveAspect = true;
            AddLayoutElement(iconObj, preferredWidth: 24, preferredHeight: 24);

            // Label
            GameObject labelObj = CreateUIObject("Label", content.transform);
            TextMeshProUGUI tmp = AddText(labelObj, "Tab", Theme.captionSize,
                Theme.textSecondary, FontStyles.Normal, TextAlignmentOptions.Center);
            AddLayoutElement(labelObj, preferredHeight: 14);

            // Add component
            var navBtn = root.AddComponent<NavButton>();

            var so = new SerializedObject(navBtn);
            so.FindProperty("_backgroundHighlight").objectReferenceValue = highlightImg;
            so.FindProperty("_icon").objectReferenceValue = iconImg;
            so.FindProperty("_label").objectReferenceValue = tmp;
            so.ApplyModifiedProperties();

            SavePrefab(root, "NavButton");
        }
    }
}
