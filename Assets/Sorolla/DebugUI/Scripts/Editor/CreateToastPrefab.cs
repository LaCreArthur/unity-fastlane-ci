using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateToastPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Toast Notification")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("ToastNotification");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(320, 48);

            var cg = root.AddComponent<CanvasGroup>();

            // Background
            Image bg = AddImage(root, new Color(0.2f, 0.1f, 0.1f, 0.95f));
            bg.type = Image.Type.Sliced;
            // Note: Assign rounded rect sprite manually

            // Content horizontal layout
            AddHorizontalLayout(root, 12,
                new RectOffset(16, 16, 12, 12),
                childControlWidth: true, childControlHeight: false);

            // Dot indicator
            GameObject dot = CreateUIObject("DotIndicator", root.transform);
            Image dotImg = AddImage(dot, Theme.accentRed);
            AddLayoutElement(dot, preferredWidth: 10, preferredHeight: 10);

            // Message text
            GameObject textObj = CreateUIObject("Message", root.transform);
            TextMeshProUGUI tmp = AddText(textObj, "Toast message here", Theme.bodySize,
                Theme.textPrimary);
            AddLayoutElement(textObj, flexibleWidth: 1, preferredHeight: 24);

            // Add component
            var toast = root.AddComponent<ToastNotification>();

            // Use SerializedObject to set private fields
            var so = new SerializedObject(toast);
            so.FindProperty("_background").objectReferenceValue = bg;
            so.FindProperty("_dotIndicator").objectReferenceValue = dotImg;
            so.FindProperty("_messageText").objectReferenceValue = tmp;
            so.FindProperty("_canvasGroup").objectReferenceValue = cg;
            so.ApplyModifiedProperties();

            SavePrefab(root, "ToastNotification");
        }
    }
}
