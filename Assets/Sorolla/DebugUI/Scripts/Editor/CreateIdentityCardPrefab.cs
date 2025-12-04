using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateIdentityCardPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Identity Card")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("IdentityCard");

            // Background with border
            Image bg = AddImage(root, Theme.cardBackground);
            bg.type = Image.Type.Sliced;

            // Outline (simulated with another image or use Outline component)
            var outline = root.AddComponent<Outline>();
            outline.effectColor = new Color(Theme.accentYellow.r, Theme.accentYellow.g, Theme.accentYellow.b, 0.5f);
            outline.effectDistance = new Vector2(1, 1);

            // Content Size Fitter
            AddContentSizeFitter(root);
            AddLayoutElement(root, flexibleWidth: 1, minHeight: 70);

            // Horizontal layout
            AddHorizontalLayout(root, 12,
                new RectOffset(16, 16, 12, 12));

            // Text Column
            GameObject textColumn = CreateUIObject("TextColumn", root.transform);
            AddVerticalLayout(textColumn, 4,
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(textColumn, flexibleWidth: 1);

            // Label (e.g., "IDFA / GAID")
            GameObject labelObj = CreateUIObject("Label", textColumn.transform);
            AddText(labelObj, "IDFA / GAID", Theme.captionSize,
                Theme.textSecondary);
            AddLayoutElement(labelObj, preferredHeight: 14);

            // Value
            GameObject valueObj = CreateUIObject("Value", textColumn.transform);
            TextMeshProUGUI valueTmp = AddText(valueObj, "A1B2-C3D4-E5F6-G7H8", Theme.bodySize,
                Theme.textPrimary);
            valueTmp.fontStyle = FontStyles.Normal;
            AddLayoutElement(valueObj, preferredHeight: 20);

            // Copy Button
            GameObject copyBtn = CreateUIObject("CopyButton", root.transform);
            Image copyBtnBg = AddImage(copyBtn, new Color(1, 1, 1, 0)); // Transparent
            var btn = copyBtn.AddComponent<Button>();
            btn.targetGraphic = copyBtnBg;
            AddLayoutElement(copyBtn, preferredWidth: 32, preferredHeight: 32);

            // Copy Icon
            GameObject copyIcon = CreateUIObject("CopyIcon", copyBtn.transform);
            var iconRt = copyIcon.GetComponent<RectTransform>();
            SetStretch(iconRt);
            iconRt.offsetMin = new Vector2(4, 4);
            iconRt.offsetMax = new Vector2(-4, -4);
            Image iconImg = AddImage(copyIcon, Theme.textSecondary);
            iconImg.preserveAspect = true;

            SavePrefab(root, "IdentityCard");
        }
    }
}
