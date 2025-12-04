using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateSectionCardPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Section Card")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("SectionCard");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(0, 0); // Driven by layout

            // Background
            Image bg = AddImage(root, Theme.cardBackground);
            bg.type = Image.Type.Sliced;

            // Content Size Fitter
            AddContentSizeFitter(root);

            // Layout Element for external layouts
            AddLayoutElement(root, flexibleWidth: 1);

            // Vertical layout for content
            AddVerticalLayout(root, Theme.smallPadding,
                new RectOffset(
                    (int)Theme.standardPadding,
                    (int)Theme.standardPadding,
                    (int)Theme.standardPadding,
                    (int)Theme.standardPadding),
                childForceExpandWidth: true, childForceExpandHeight: false);

            // Section Title
            GameObject titleObj = CreateUIObject("SectionTitle", root.transform);
            TextMeshProUGUI tmp = AddText(titleObj, "SECTION TITLE", Theme.sectionHeaderSize,
                Theme.textSecondary, FontStyles.Bold);
            tmp.characterSpacing = 2f;
            AddLayoutElement(titleObj, preferredHeight: 16);

            // Content Container (placeholder for custom content)
            GameObject contentContainer = CreateUIObject("ContentContainer", root.transform);
            AddVerticalLayout(contentContainer, Theme.smallPadding,
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddContentSizeFitter(contentContainer);
            AddLayoutElement(contentContainer, flexibleWidth: 1);

            SavePrefab(root, "SectionCard");
        }
    }
}
