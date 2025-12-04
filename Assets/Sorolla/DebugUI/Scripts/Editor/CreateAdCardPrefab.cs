using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateAdCardPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Ad Card")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("AdCard");

            // Background
            Image bg = AddImage(root, Theme.cardBackground);
            bg.type = Image.Type.Sliced;

            // Content Size Fitter & Layout Element
            AddContentSizeFitter(root);
            AddLayoutElement(root, flexibleWidth: 1, minHeight: 120);

            // Main horizontal layout (accent bar + content)
            AddHorizontalLayout(root,
                childForceExpandHeight: true);

            // Left Accent Bar
            GameObject accentBar = CreateUIObject("AccentBar", root.transform);
            Image accentImg = AddImage(accentBar, Theme.accentRed);
            AddLayoutElement(accentBar, preferredWidth: Theme.accentBarWidth);

            // Content Container
            GameObject content = CreateUIObject("Content", root.transform);
            AddVerticalLayout(content, 12,
                new RectOffset(16, 16, 16, 16),
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(content, flexibleWidth: 1);

            // Header Row
            GameObject headerRow = CreateUIObject("HeaderRow", content.transform);
            AddHorizontalLayout(headerRow, 8,
                childAlignment: TextAnchor.MiddleLeft,
                childForceExpandWidth: false, childForceExpandHeight: false);
            AddLayoutElement(headerRow, flexibleWidth: 1, preferredHeight: 44);

            // Title Column
            GameObject titleColumn = CreateUIObject("TitleColumn", headerRow.transform);
            AddVerticalLayout(titleColumn, 2,
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(titleColumn, flexibleWidth: 1);

            // Title
            GameObject titleObj = CreateUIObject("Title", titleColumn.transform);
            AddText(titleObj, "Interstitial", Theme.titleSize,
                Theme.textPrimary, FontStyles.Bold);
            AddLayoutElement(titleObj, preferredHeight: 22);

            // Subtitle
            GameObject subtitleObj = CreateUIObject("Subtitle", titleColumn.transform);
            AddText(subtitleObj, "Full screen break", Theme.subtitleSize,
                Theme.textSecondary);
            AddLayoutElement(subtitleObj, preferredHeight: 18);

            // Status Badge Placeholder
            GameObject statusBadge = CreateUIObject("StatusBadgePlaceholder", headerRow.transform);
            AddLayoutElement(statusBadge, preferredWidth: 60, preferredHeight: 24);

            // Button Row
            GameObject buttonRow = CreateUIObject("ButtonRow", content.transform);
            AddHorizontalLayout(buttonRow, 12,
                childForceExpandWidth: false, childForceExpandHeight: false);
            AddLayoutElement(buttonRow, flexibleWidth: 1, preferredHeight: Theme.buttonHeight);

            // Load Button Placeholder
            GameObject loadBtn = CreateUIObject("LoadButtonPlaceholder", buttonRow.transform);
            AddImage(loadBtn, Theme.cardBackgroundLight);
            AddLayoutElement(loadBtn, flexibleWidth: 1, preferredHeight: Theme.buttonHeight);

            // Show Button Placeholder
            GameObject showBtn = CreateUIObject("ShowButtonPlaceholder", buttonRow.transform);
            AddImage(showBtn, Theme.accentPurple);
            AddLayoutElement(showBtn, flexibleWidth: 1, preferredHeight: Theme.buttonHeight);

            SavePrefab(root, "AdCard");
        }
    }
}
