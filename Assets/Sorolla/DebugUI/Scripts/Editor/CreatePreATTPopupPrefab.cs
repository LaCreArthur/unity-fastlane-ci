using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.ATT.Editor
{
    /// <summary>
    ///     Creates the PreATT warm-up popup prefab.
    ///     This is shown before the system ATT dialog to explain why tracking helps.
    /// </summary>
    public class CreatePreATTPopupPrefab : ATTPrefabCreator
    {
        [MenuItem("Sorolla/ATT/Create PreATT Popup Prefab")]
        public static void Create()
        {
            // Canvas root
            GameObject root = CreateCanvasRoot("ContextScreen");

            // Dark overlay (blocks input)
            GameObject overlay = CreateUIObject("Overlay", root.transform);
            AddImage(overlay, OverlayColor);
            SetStretch(overlay.GetComponent<RectTransform>());

            // Centered panel
            GameObject panel = CreateUIObject("Panel", root.transform);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.5f, 0.5f);
            panelRt.anchorMax = new Vector2(0.5f, 0.5f);
            panelRt.sizeDelta = new Vector2(600, 500);
            AddImage(panel, PanelColor);

            var vlg = panel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(40, 40, 40, 40);
            vlg.spacing = 24;
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            // Icon placeholder
            GameObject icon = CreateUIObject("Icon", panel.transform);
            var iconImg = AddImage(icon, new Color(0.3f, 0.6f, 1f, 1f));
            var iconLe = icon.AddComponent<LayoutElement>();
            iconLe.preferredWidth = 80;
            iconLe.preferredHeight = 80;

            // Title
            GameObject title = CreateUIObject("Title", panel.transform);
            var titleTmp = AddText(title, "Personalized Ads", 36, TextPrimary);
            titleTmp.fontStyle = FontStyles.Bold;
            var titleLe = title.AddComponent<LayoutElement>();
            titleLe.preferredHeight = 50;

            // Description
            GameObject desc = CreateUIObject("Description", panel.transform);
            var descTmp = AddText(desc, 
                "We use your data to show you relevant ads and support free gameplay.\n\nYour privacy is important to us.",
                24, TextSecondary);
            descTmp.enableWordWrapping = true;
            var descLe = desc.AddComponent<LayoutElement>();
            descLe.preferredHeight = 120;

            // Continue button
            GameObject btnGo = CreateUIObject("ContinueButton", panel.transform);
            var btnRt = btnGo.GetComponent<RectTransform>();
            btnRt.sizeDelta = new Vector2(400, 60);
            var btn = AddButton(btnGo, ButtonPrimary);
            var btnLe = btnGo.AddComponent<LayoutElement>();
            btnLe.preferredWidth = 400;
            btnLe.preferredHeight = 60;

            GameObject btnText = CreateUIObject("Text", btnGo.transform);
            SetStretch(btnText.GetComponent<RectTransform>());
            var btnTmp = AddText(btnText, "Continue", 28, TextPrimary);
            btnTmp.fontStyle = FontStyles.Bold;

            // Add ContextScreenView component and wire button
            var view = root.AddComponent<ContextScreenView>();
            var so = new SerializedObject(view);
            so.FindProperty("button").objectReferenceValue = btn;
            so.ApplyModifiedProperties();

            SavePrefab(root, "ContextScreen");
        }
    }
}
