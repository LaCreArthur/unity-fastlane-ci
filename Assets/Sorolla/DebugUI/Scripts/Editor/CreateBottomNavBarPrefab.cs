using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateBottomNavBarPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Bottom Nav Bar")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("BottomNavBar");
            var rootRt = root.GetComponent<RectTransform>();

            // Anchor to bottom, stretch horizontally
            rootRt.anchorMin = new Vector2(0, 0);
            rootRt.anchorMax = new Vector2(1, 0);
            rootRt.pivot = new Vector2(0.5f, 0);
            rootRt.sizeDelta = new Vector2(0, 80);
            rootRt.anchoredPosition = Vector2.zero;

            // Background
            Image bg = AddImage(root, Theme.canvasBackground);

            // Horizontal layout for nav buttons
            AddHorizontalLayout(root, 0,
                new RectOffset(8, 8, 0, 0),
                TextAnchor.MiddleCenter,
                true, true);

            // Create 5 placeholder slots for nav buttons
            string[] tabNames = { "Dash", "Ads", "Events", "Tools", "Logs" };
            for (int i = 0; i < tabNames.Length; i++)
            {
                GameObject navSlot = CreateUIObject($"NavSlot_{tabNames[i]}", root.transform);
                AddLayoutElement(navSlot, flexibleWidth: 1, preferredHeight: 64);
                // NavButton prefabs will be instantiated here at runtime or in assembly
            }

            SavePrefab(root, "BottomNavBar");
        }
    }
}
