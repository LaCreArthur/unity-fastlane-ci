using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateStatusBadgePrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Status Badge")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("StatusBadge");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(60, 24);

            // Background
            Image bg = AddImage(root, new Color(0.3f, 0.3f, 0.3f, 0.5f));
            bg.type = Image.Type.Sliced;

            // Content size fitter
            AddContentSizeFitter(root,
                ContentSizeFitter.FitMode.PreferredSize);

            // Horizontal layout for padding
            AddHorizontalLayout(root, 0,
                new RectOffset(10, 10, 4, 4),
                TextAnchor.MiddleCenter);

            // Label
            GameObject labelObj = CreateUIObject("Label", root.transform);
            TextMeshProUGUI tmp = AddText(labelObj, "IDLE", Theme.captionSize,
                Theme.textSecondary, FontStyles.Bold, TextAlignmentOptions.Center);

            // Add component
            var badge = root.AddComponent<StatusBadge>();

            var so = new SerializedObject(badge);
            so.FindProperty("_background").objectReferenceValue = bg;
            so.FindProperty("_label").objectReferenceValue = tmp;
            so.ApplyModifiedProperties();

            SavePrefab(root, "StatusBadge");
        }
    }
}
