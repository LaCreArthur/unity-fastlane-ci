using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateDividerPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Divider")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("Divider");

            // Layout Element
            AddLayoutElement(root, flexibleWidth: 1, preferredHeight: 1);

            // Line Image
            Image line = AddImage(root, new Color(Theme.textSecondary.r, Theme.textSecondary.g, Theme.textSecondary.b, 0.2f));

            SavePrefab(root, "Divider");
        }
    }
}
