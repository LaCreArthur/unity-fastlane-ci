using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.ATT.Editor
{
    /// <summary>
    ///     Creates the Fake CMP dialog prefab for Editor testing.
    ///     Mimics GDPR/CMP consent flow.
    /// </summary>
    public class CreateFakeCMPPopupPrefab : ATTPrefabCreator
    {
        [MenuItem("Sorolla/ATT/Create Fake CMP Popup Prefab")]
        public static void Create()
        {
            // Canvas root
            GameObject root = CreateCanvasRoot("FakeCMPDialog");

            // Dark overlay
            GameObject overlay = CreateUIObject("Overlay", root.transform);
            AddImage(overlay, OverlayColor);
            SetStretch(overlay.GetComponent<RectTransform>());

            // CMP-style panel (bottom sheet style)
            GameObject panel = CreateUIObject("Panel", root.transform);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0, 0);
            panelRt.anchorMax = new Vector2(1, 0);
            panelRt.pivot = new Vector2(0.5f, 0);
            panelRt.sizeDelta = new Vector2(0, 520);
            panelRt.anchoredPosition = Vector2.zero;
            AddImage(panel, new Color(0.12f, 0.12f, 0.14f, 1f));

            var vlg = panel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(32, 32, 32, 32);
            vlg.spacing = 20;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            // Title
            GameObject title = CreateUIObject("Title", panel.transform);
            var titleTmp = AddText(title, "We value your privacy", 32, TextPrimary);
            titleTmp.fontStyle = FontStyles.Bold;
            var titleLe = title.AddComponent<LayoutElement>();
            titleLe.preferredHeight = 50;

            // Description
            GameObject desc = CreateUIObject("Description", panel.transform);
            var descTmp = AddText(desc,
                "We and our partners use cookies and similar technologies to:\n\n" +
                "• Personalize content and ads\n" +
                "• Provide social media features\n" +
                "• Analyze our traffic\n\n" +
                "You can choose to accept or reject these cookies.",
                20, TextSecondary, TextAlignmentOptions.Left);
            descTmp.enableWordWrapping = true;
            var descLe = desc.AddComponent<LayoutElement>();
            descLe.preferredHeight = 200;

            // Buttons container
            GameObject buttons = CreateUIObject("Buttons", panel.transform);
            var btnVlg = buttons.AddComponent<VerticalLayoutGroup>();
            btnVlg.spacing = 12;
            btnVlg.childControlWidth = true;
            btnVlg.childControlHeight = false;
            btnVlg.childForceExpandWidth = true;
            var btnLe = buttons.AddComponent<LayoutElement>();
            btnLe.preferredHeight = 130;

            // Accept All button (green, prominent)
            GameObject acceptBtn = CreateUIObject("AcceptAllButton", buttons.transform);
            var acceptBtnComp = AddButton(acceptBtn, new Color(0.2f, 0.7f, 0.3f, 1f));
            var acceptBtnLe = acceptBtn.AddComponent<LayoutElement>();
            acceptBtnLe.preferredHeight = 56;

            GameObject acceptText = CreateUIObject("Text", acceptBtn.transform);
            SetStretch(acceptText.GetComponent<RectTransform>());
            var acceptTmp = AddText(acceptText, "Accept All", 26, TextPrimary);
            acceptTmp.fontStyle = FontStyles.Bold;

            // Reject All button (gray outline style)
            GameObject rejectBtn = CreateUIObject("RejectAllButton", buttons.transform);
            var rejectBtnComp = AddButton(rejectBtn, ButtonSecondary);
            var rejectBtnLe = rejectBtn.AddComponent<LayoutElement>();
            rejectBtnLe.preferredHeight = 56;

            GameObject rejectText = CreateUIObject("Text", rejectBtn.transform);
            SetStretch(rejectText.GetComponent<RectTransform>());
            AddText(rejectText, "Reject All", 26, TextSecondary);

            // Add FakeCMPDialog component
            var dialog = root.AddComponent<FakeCMPDialog>();

            // Wire buttons via SerializedObject
            var so = new SerializedObject(dialog);
            so.FindProperty("acceptAllButton").objectReferenceValue = acceptBtnComp;
            so.FindProperty("rejectAllButton").objectReferenceValue = rejectBtnComp;
            so.ApplyModifiedProperties();

            SavePrefab(root, "FakeCMPDialog");
        }
    }
}
