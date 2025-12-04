using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateLogEntryPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Log Entry")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("LogEntry");

            // Content Size Fitter
            AddContentSizeFitter(root);
            AddLayoutElement(root, flexibleWidth: 1, minHeight: 50);

            // Horizontal layout (accent bar + content)
            AddHorizontalLayout(root,
                childForceExpandHeight: true);

            // Accent Bar
            GameObject accentBar = CreateUIObject("AccentBar", root.transform);
            Image accentImg = AddImage(accentBar, Theme.accentYellow);
            AddLayoutElement(accentBar, preferredWidth: 3);

            // Content Container
            GameObject content = CreateUIObject("Content", root.transform);
            AddVerticalLayout(content, 4,
                new RectOffset(12, 12, 8, 8),
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(content, flexibleWidth: 1);

            // Header Row (timestamp + source badge)
            GameObject headerRow = CreateUIObject("HeaderRow", content.transform);
            AddHorizontalLayout(headerRow, 8,
                childAlignment: TextAnchor.MiddleLeft,
                childForceExpandWidth: false, childForceExpandHeight: false);
            AddLayoutElement(headerRow, flexibleWidth: 1, preferredHeight: 18);

            // Timestamp
            GameObject timestampObj = CreateUIObject("Timestamp", headerRow.transform);
            TextMeshProUGUI timestampTmp = AddText(timestampObj, "09:41:01. 45", Theme.captionSize,
                Theme.textSecondary);
            // Use monospace font if available
            AddLayoutElement(timestampObj, preferredHeight: 14);
            AddContentSizeFitter(timestampObj,
                ContentSizeFitter.FitMode.PreferredSize,
                ContentSizeFitter.FitMode.Unconstrained);

            // Source Badge
            GameObject sourceBadge = CreateUIObject("SourceBadge", headerRow.transform);
            Image badgeBg = AddImage(sourceBadge, new Color(0.3f, 0.3f, 0.3f, 0.8f));
            badgeBg.type = Image.Type.Sliced;

            AddHorizontalLayout(sourceBadge, 0,
                new RectOffset(6, 6, 2, 2),
                TextAnchor.MiddleCenter);
            AddContentSizeFitter(sourceBadge,
                ContentSizeFitter.FitMode.PreferredSize);

            // Source Label
            GameObject sourceLabelObj = CreateUIObject("SourceLabel", sourceBadge.transform);
            TextMeshProUGUI sourceTmp = AddText(sourceLabelObj, "GA", Theme.captionSize - 2,
                Theme.textPrimary, FontStyles.Bold, TextAlignmentOptions.Center);
            AddContentSizeFitter(sourceLabelObj,
                ContentSizeFitter.FitMode.PreferredSize,
                ContentSizeFitter.FitMode.Unconstrained);

            // Message Text
            GameObject messageObj = CreateUIObject("Message", content.transform);
            TextMeshProUGUI messageTmp = AddText(messageObj, "Resource: +100 Coins (Source: Shop)",
                Theme.bodySize, Theme.textPrimary);
            messageTmp.enableWordWrapping = true;
            AddLayoutElement(messageObj, flexibleWidth: 1);
            AddContentSizeFitter(messageObj);

            // Add LogEntryView component
            var logEntryView = root.AddComponent<LogEntryView>();

            var so = new SerializedObject(logEntryView);
            so.FindProperty("_accentBar").objectReferenceValue = accentImg;
            so.FindProperty("_timestampLabel").objectReferenceValue = timestampTmp;
            so.FindProperty("_sourceBadgeBackground").objectReferenceValue = badgeBg;
            so.FindProperty("_sourceBadgeLabel").objectReferenceValue = sourceTmp;
            so.FindProperty("_messageLabel").objectReferenceValue = messageTmp;
            so.ApplyModifiedProperties();

            SavePrefab(root, "LogEntry");
        }
    }
}
