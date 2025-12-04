using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateMediationTesterCardPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Mediation Tester Card")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("MediationTesterCard");

            // Background - Purple tinted
            var purpleBg = new Color(Theme.accentPurple.r * 0.3f, Theme.accentPurple.g * 0.3f, Theme.accentPurple.b * 0.5f, 1f);
            Image bg = AddImage(root, purpleBg);
            bg.type = Image.Type.Sliced;

            // Outline
            var outline = root.AddComponent<Outline>();
            outline.effectColor = Theme.accentPurple;
            outline.effectDistance = new Vector2(1, 1);

            // Layout Element
            AddLayoutElement(root, flexibleWidth: 1, preferredHeight: 64);

            // Horizontal layout
            AddHorizontalLayout(root, 12,
                new RectOffset(16, 16, 12, 12));

            // Shield Icon
            GameObject shieldIcon = CreateUIObject("ShieldIcon", root.transform);
            AddImage(shieldIcon, Theme.textSecondary);
            AddLayoutElement(shieldIcon, preferredWidth: 24, preferredHeight: 24);

            // Text Column
            GameObject textColumn = CreateUIObject("TextColumn", root.transform);
            AddVerticalLayout(textColumn, 2,
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(textColumn, flexibleWidth: 1);

            // Title
            GameObject titleObj = CreateUIObject("Title", textColumn.transform);
            AddText(titleObj, "MEDIATION TESTER", Theme.captionSize,
                Theme.textPrimary, FontStyles.Bold);
            AddLayoutElement(titleObj, preferredHeight: 14);

            // Subtitle
            GameObject subtitleObj = CreateUIObject("Subtitle", textColumn.transform);
            AddText(subtitleObj, "AppLovin MAX • Unity Ads • Ironsource", Theme.captionSize,
                Theme.accentPurple);
            AddLayoutElement(subtitleObj, preferredHeight: 14);

            // Make it clickable
            var btn = root.AddComponent<Button>();
            btn.targetGraphic = bg;

            SavePrefab(root, "MediationTesterCard");
        }
    }
}
