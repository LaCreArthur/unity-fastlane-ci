using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI.Editor
{
    public class CreateToggleSwitchPrefab : SorollaDebugPrefabCreator
    {
        [MenuItem("Sorolla/Debug UI/Prefabs/Create Toggle Switch")]
        public static void Create()
        {
            if (Theme == null) return;

            // Root - sized like iOS toggle
            GameObject root = CreateUIObject("ToggleSwitch");
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(56, 32);

            // Track (background)
            GameObject track = CreateUIObject("Track", root.transform);
            var trackRt = track.GetComponent<RectTransform>();
            SetStretch(trackRt);
            Image trackImg = AddImage(track, Theme.cardBackgroundLight);
            trackImg.type = Image.Type.Sliced;
            // Assign rounded pill sprite manually

            // Knob
            GameObject knob = CreateUIObject("Knob", root.transform);
            var knobRt = knob.GetComponent<RectTransform>();
            knobRt.anchorMin = new Vector2(0.5f, 0.5f);
            knobRt.anchorMax = new Vector2(0.5f, 0.5f);
            knobRt.pivot = new Vector2(0.5f, 0.5f);
            knobRt.sizeDelta = new Vector2(26, 26);
            knobRt.anchoredPosition = new Vector2(-12, 0); // Off position

            Image knobImg = AddImage(knob, Color.white);
            knobImg.type = Image.Type.Sliced;

            // Add component
            var toggle = root.AddComponent<ToggleSwitch>();

            var so = new SerializedObject(toggle);
            so.FindProperty("_trackImage").objectReferenceValue = trackImg;
            so.FindProperty("_knobImage").objectReferenceValue = knobImg;
            so.FindProperty("_knobTransform").objectReferenceValue = knobRt;
            so.ApplyModifiedProperties();

            SavePrefab(root, "ToggleSwitch");
        }
    }
}
