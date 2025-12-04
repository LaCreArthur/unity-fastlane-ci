using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateConfigRowPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Config Row")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("ConfigRow");

            // Layout Element
            AddLayoutElement(root, flexibleWidth: 1, preferredHeight: 24);

            // Horizontal layout
            AddHorizontalLayout(root, 8,
                childAlignment: TextAnchor.MiddleLeft,
                childForceExpandWidth: false, childForceExpandHeight: false);

            // Key Label
            GameObject keyObj = CreateUIObject("KeyLabel", root.transform);
            TextMeshProUGUI keyTmp = AddText(keyObj, "hero_speed", Theme.bodySize,
                Theme.textSecondary);
            // Monospace style for config keys
            AddLayoutElement(keyObj, flexibleWidth: 1, preferredHeight: 20);

            // Value Label
            GameObject valueObj = CreateUIObject("ValueLabel", root.transform);
            TextMeshProUGUI valueTmp = AddText(valueObj, "12. 5", Theme.bodySize,
                Theme.accentYellow, FontStyles.Normal, TextAlignmentOptions.Right);
            AddContentSizeFitter(valueObj,
                ContentSizeFitter.FitMode.PreferredSize,
                ContentSizeFitter.FitMode.Unconstrained);

            SavePrefab(root, "ConfigRow");
        }
    }
}
