using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateLogFilterBarPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Log Filter Bar")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root
            GameObject root = CreateUIObject("LogFilterBar");

            // Layout Element
            AddLayoutElement(root, flexibleWidth: 1, preferredHeight: 44);

            // Horizontal layout
            AddHorizontalLayout(root, 8,
                new RectOffset(12, 12, 8, 8));

            // Terminal Icon
            GameObject terminalIcon = CreateUIObject("TerminalIcon", root.transform);
            AddImage(terminalIcon, Theme.textSecondary);
            AddLayoutElement(terminalIcon, preferredWidth: 20, preferredHeight: 20);

            // Filter Button Group
            GameObject filterGroup = CreateUIObject("FilterButtonGroup", root.transform);
            AddHorizontalLayout(filterGroup);
            AddContentSizeFitter(filterGroup,
                ContentSizeFitter.FitMode.PreferredSize,
                ContentSizeFitter.FitMode.Unconstrained);

            // Filter Buttons
            string[] filters = { "ALL", "INFO", "WARNING", "ERROR" };
            for (int i = 0; i < filters.Length; i++)
            {
                GameObject filterBtn = CreateUIObject($"Filter_{filters[i]}", filterGroup.transform);

                // Background (selected state for first one)
                bool isSelected = i == 0;
                Image btnBg = AddImage(filterBtn, isSelected ? Theme.cardBackgroundLight : new Color(0, 0, 0, 0));
                btnBg.type = Image.Type.Sliced;

                var btn = filterBtn.AddComponent<Button>();
                btn.targetGraphic = btnBg;

                // Padding
                AddHorizontalLayout(filterBtn, 0,
                    new RectOffset(12, 12, 6, 6),
                    TextAnchor.MiddleCenter);

                // Label
                GameObject labelObj = CreateUIObject("Label", filterBtn.transform);
                AddText(labelObj, filters[i], Theme.captionSize,
                    isSelected ? Theme.textPrimary : Theme.textSecondary,
                    FontStyles.Bold, TextAlignmentOptions.Center);
                AddContentSizeFitter(labelObj,
                    ContentSizeFitter.FitMode.PreferredSize,
                    ContentSizeFitter.FitMode.Unconstrained);
            }

            // Spacer
            GameObject spacer = CreateUIObject("Spacer", root.transform);
            AddLayoutElement(spacer, flexibleWidth: 1);

            // Clear Button
            GameObject clearBtn = CreateUIObject("ClearButton", root.transform);
            Image clearBg = AddImage(clearBtn, new Color(1, 1, 1, 0));
            var clearButton = clearBtn.AddComponent<Button>();
            clearButton.targetGraphic = clearBg;
            AddLayoutElement(clearBtn, preferredWidth: 32, preferredHeight: 32);

            GameObject trashIcon = CreateUIObject("TrashIcon", clearBtn.transform);
            var trashRt = trashIcon.GetComponent<RectTransform>();
            SetStretch(trashRt);
            trashRt.offsetMin = new Vector2(4, 4);
            trashRt.offsetMax = new Vector2(-4, -4);
            AddImage(trashIcon, Theme.textSecondary);

            SavePrefab(root, "LogFilterBar");
        }
    }
}
