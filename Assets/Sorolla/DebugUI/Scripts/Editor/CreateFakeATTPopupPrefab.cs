using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.ATT.Editor
{
    /// <summary>
    ///     Creates the Fake ATT dialog prefab for Editor testing.
    ///     Mimics the iOS ATT permission dialog.
    /// </summary>
    public class CreateFakeATTPopupPrefab : ATTPrefabCreator
    {
        [MenuItem("Sorolla/ATT/Create Fake ATT Popup Prefab")]
        public static void Create()
        {
            // Canvas root
            GameObject root = CreateCanvasRoot("FakeATTDialog");

            // Dark overlay
            GameObject overlay = CreateUIObject("Overlay", root.transform);
            AddImage(overlay, OverlayColor);
            SetStretch(overlay.GetComponent<RectTransform>());

            // iOS-style alert panel
            GameObject panel = CreateUIObject("Panel", root.transform);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.5f, 0.5f);
            panelRt.anchorMax = new Vector2(0.5f, 0.5f);
            panelRt.sizeDelta = new Vector2(560, 380);
            AddImage(panel, new Color(0.17f, 0.17f, 0.18f, 1f));

            var vlg = panel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(24, 24, 24, 16);
            vlg.spacing = 16;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            // Title
            GameObject title = CreateUIObject("Title", panel.transform);
            var titleTmp = AddText(title, "Allow \"YourApp\" to track your activity?", 26, TextPrimary);
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.enableWordWrapping = true;
            var titleLe = title.AddComponent<LayoutElement>();
            titleLe.preferredHeight = 70;

            // Description
            GameObject desc = CreateUIObject("Description", panel.transform);
            var descTmp = AddText(desc,
                "Your data will be used to deliver personalized ads to you.",
                22, TextSecondary);
            descTmp.enableWordWrapping = true;
            var descLe = desc.AddComponent<LayoutElement>();
            descLe.preferredHeight = 60;

            // Buttons container
            GameObject buttons = CreateUIObject("Buttons", panel.transform);
            var btnVlg = buttons.AddComponent<VerticalLayoutGroup>();
            btnVlg.spacing = 8;
            btnVlg.childControlWidth = true;
            btnVlg.childControlHeight = false;
            btnVlg.childForceExpandWidth = true;
            var btnLe = buttons.AddComponent<LayoutElement>();
            btnLe.preferredHeight = 120;

            // Allow button (blue, prominent)
            GameObject allowBtn = CreateUIObject("AllowButton", buttons.transform);
            var allowBtnComp = AddButton(allowBtn, ButtonPrimary);
            var allowBtnLe = allowBtn.AddComponent<LayoutElement>();
            allowBtnLe.preferredHeight = 50;

            GameObject allowText = CreateUIObject("Text", allowBtn.transform);
            SetStretch(allowText.GetComponent<RectTransform>());
            var allowTmp = AddText(allowText, "Allow", 24, TextPrimary);
            allowTmp.fontStyle = FontStyles.Bold;

            // Ask App Not to Track button (gray)
            GameObject denyBtn = CreateUIObject("DenyButton", buttons.transform);
            var denyBtnComp = AddButton(denyBtn, ButtonSecondary);
            var denyBtnLe = denyBtn.AddComponent<LayoutElement>();
            denyBtnLe.preferredHeight = 50;

            GameObject denyText = CreateUIObject("Text", denyBtn.transform);
            SetStretch(denyText.GetComponent<RectTransform>());
            AddText(denyText, "Ask App Not to Track", 24, TextSecondary);

            // Add FakeATTDialog component
            var dialog = root.AddComponent<FakeATTDialog>();

            // Wire buttons via SerializedObject
            var so = new SerializedObject(dialog);
            so.FindProperty("allowButton").objectReferenceValue = allowBtnComp;
            so.FindProperty("denyButton").objectReferenceValue = denyBtnComp;
            so.ApplyModifiedProperties();

            SavePrefab(root, "FakeATTDialog");
        }
    }
}
