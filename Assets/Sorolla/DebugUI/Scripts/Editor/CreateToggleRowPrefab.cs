using UnityEditor;
using UnityEngine;

namespace Sorolla.DebugUI.Editor
{
    public class CreateToggleRowPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Toggle Row")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("ToggleRow");

            // Layout Element
            AddLayoutElement(root, flexibleWidth: 1, preferredHeight: 56);

            // Horizontal layout
            AddHorizontalLayout(root, 12,
                new RectOffset(0, 0, 16, 16));

            // Label
            GameObject labelObj = CreateUIObject("Label", root.transform);
            AddText(labelObj, "Toggle Label", Theme.bodySize,
                Theme.textPrimary);
            AddLayoutElement(labelObj, flexibleWidth: 1, preferredHeight: 20);

            // Toggle Switch Placeholder
            GameObject togglePlaceholder = CreateUIObject("ToggleSwitchPlaceholder", root.transform);
            AddLayoutElement(togglePlaceholder, preferredWidth: 56, preferredHeight: 32);

            SavePrefab(root, "ToggleRow");
        }
    }
}
