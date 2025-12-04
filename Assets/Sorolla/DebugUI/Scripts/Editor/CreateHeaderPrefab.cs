using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateHeaderPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Header")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("Header");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(0, 72);

            // Layout Element
            AddLayoutElement(root, flexibleWidth: 1, preferredHeight: 72);

            // Horizontal layout
            AddHorizontalLayout(root, 12,
                new RectOffset(16, 16, 12, 12));

            // Logo Container
            GameObject logoContainer = CreateUIObject("LogoContainer", root.transform);
            var logoRt = logoContainer.GetComponent<RectTransform>();
            AddLayoutElement(logoContainer, preferredWidth: 48, preferredHeight: 48);

            // Logo Background
            Image logoBg = AddImage(logoContainer, Theme.accentPurple);
            logoBg.type = Image.Type.Sliced;

            // Logo Icon placeholder
            GameObject logoIcon = CreateUIObject("LogoIcon", logoContainer.transform);
            var iconRt = logoIcon.GetComponent<RectTransform>();
            SetStretch(iconRt);
            iconRt.offsetMin = new Vector2(8, 8);
            iconRt.offsetMax = new Vector2(-8, -8);
            Image iconImg = AddImage(logoIcon, Color.white);
            iconImg.preserveAspect = true;

            // Title Column
            GameObject titleColumn = CreateUIObject("TitleColumn", root.transform);
            AddVerticalLayout(titleColumn, 2,
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(titleColumn, flexibleWidth: 1);

            // Title
            GameObject titleObj = CreateUIObject("Title", titleColumn.transform);
            AddText(titleObj, "Sorolla Debug", Theme.titleSize,
                Theme.textPrimary, FontStyles.Bold);
            AddLayoutElement(titleObj, preferredHeight: 22);

            // Mode Row
            GameObject modeRow = CreateUIObject("ModeRow", titleColumn.transform);
            AddHorizontalLayout(modeRow, 6,
                childAlignment: TextAnchor.MiddleLeft,
                childForceExpandWidth: false, childForceExpandHeight: false);
            AddLayoutElement(modeRow, preferredHeight: 18);

            // Mode Dot
            GameObject modeDot = CreateUIObject("ModeDot", modeRow.transform);
            Image dotImg = AddImage(modeDot, Theme.accentGreen);
            AddLayoutElement(modeDot, preferredWidth: 8, preferredHeight: 8);

            // Mode Label
            GameObject modeLabel = CreateUIObject("ModeLabel", modeRow.transform);
            AddText(modeLabel, "FULL MODE", Theme.captionSize,
                Theme.accentGreen, FontStyles.Bold);
            AddContentSizeFitter(modeLabel,
                ContentSizeFitter.FitMode.PreferredSize,
                ContentSizeFitter.FitMode.Unconstrained);

            // Refresh Button
            GameObject refreshBtn = CreateUIObject("RefreshButton", modeRow.transform);
            Image refreshBg = AddImage(refreshBtn, new Color(1, 1, 1, 0));
            var btn = refreshBtn.AddComponent<Button>();
            btn.targetGraphic = refreshBg;
            AddLayoutElement(refreshBtn, preferredWidth: 20, preferredHeight: 20);

            GameObject refreshIcon = CreateUIObject("RefreshIcon", refreshBtn.transform);
            var refreshIconRt = refreshIcon.GetComponent<RectTransform>();
            SetStretch(refreshIconRt);
            AddImage(refreshIcon, Theme.textSecondary);

            // Spacer
            GameObject spacer = CreateUIObject("Spacer", root.transform);
            AddLayoutElement(spacer, flexibleWidth: 1);

            // Network Status Icon
            GameObject networkBtn = CreateUIObject("NetworkStatus", root.transform);
            var networkRt = networkBtn.GetComponent<RectTransform>();
            AddLayoutElement(networkBtn, preferredWidth: 40, preferredHeight: 40);

            // Network background circle
            Image networkBg = AddImage(networkBtn, Theme.accentGreen);

            // Network icon
            GameObject networkIcon = CreateUIObject("NetworkIcon", networkBtn.transform);
            var netIconRt = networkIcon.GetComponent<RectTransform>();
            SetStretch(netIconRt);
            netIconRt.offsetMin = new Vector2(8, 8);
            netIconRt.offsetMax = new Vector2(-8, -8);
            AddImage(networkIcon, Theme.canvasBackground);

            // Resize Button
            GameObject resizeBtn = CreateUIObject("ResizeButton", root.transform);
            Image resizeBg = AddImage(resizeBtn, new Color(1, 1, 1, 0));
            var resizeButton = resizeBtn.AddComponent<Button>();
            resizeButton.targetGraphic = resizeBg;
            AddLayoutElement(resizeBtn, preferredWidth: 32, preferredHeight: 32);

            GameObject resizeIcon = CreateUIObject("ResizeIcon", resizeBtn.transform);
            var resizeIconRt = resizeIcon.GetComponent<RectTransform>();
            SetStretch(resizeIconRt);
            AddImage(resizeIcon, Theme.textPrimary);

            SavePrefab(root, "Header");
        }
    }
}
