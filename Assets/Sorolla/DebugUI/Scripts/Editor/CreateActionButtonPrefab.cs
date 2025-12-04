using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateActionButtonPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Action Button")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("ActionButton");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(140, Theme.buttonHeight);

            // Background
            Image bg = AddImage(root, Theme.cardBackgroundLight);
            bg.type = Image.Type.Sliced;

            // Button component
            var btn = root.AddComponent<Button>();
            btn.targetGraphic = bg;

            // Layout Element
            AddLayoutElement(root, minHeight: Theme.buttonHeight, flexibleWidth: 1);

            // Horizontal layout for icon + text
            AddHorizontalLayout(root, 8,
                new RectOffset(16, 16, 0, 0),
                TextAnchor.MiddleCenter);

            // Icon (optional, starts disabled)
            GameObject iconObj = CreateUIObject("Icon", root.transform);
            Image iconImg = AddImage(iconObj, Theme.textPrimary);
            iconImg.preserveAspect = true;
            AddLayoutElement(iconObj, preferredWidth: 20, preferredHeight: 20);
            iconObj.SetActive(false);

            // Label
            GameObject labelObj = CreateUIObject("Label", root.transform);
            TextMeshProUGUI tmp = AddText(labelObj, "Button", Theme.bodySize,
                Theme.textPrimary, FontStyles.Normal, TextAlignmentOptions.Center);
            AddLayoutElement(labelObj, flexibleWidth: 1, preferredHeight: 20);

            SavePrefab(root, "ActionButton");
        }

        [MenuItem("Sorolla/Debug UI/Prefabs/Create Action Button (Filled)")]
        public static void CreateFilled()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("ActionButtonFilled");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(140, Theme.buttonHeight);

            // Background - Purple filled
            Image bg = AddImage(root, Theme.accentPurple);
            bg.type = Image.Type.Sliced;

            // Button component
            var btn = root.AddComponent<Button>();
            btn.targetGraphic = bg;

            // Layout Element
            AddLayoutElement(root, minHeight: Theme.buttonHeight, flexibleWidth: 1);

            // Horizontal layout
            AddHorizontalLayout(root, 8,
                new RectOffset(16, 16, 0, 0),
                TextAnchor.MiddleCenter);

            // Label
            GameObject labelObj = CreateUIObject("Label", root.transform);
            TextMeshProUGUI tmp = AddText(labelObj, "Button", Theme.bodySize,
                Color.white, FontStyles.Normal, TextAlignmentOptions.Center);
            AddLayoutElement(labelObj, flexibleWidth: 1, preferredHeight: 20);

            SavePrefab(root, "ActionButtonFilled");
        }
    }
}
