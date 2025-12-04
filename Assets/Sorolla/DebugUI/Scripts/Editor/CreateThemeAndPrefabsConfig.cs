using UnityEditor;
using UnityEngine;

namespace Sorolla.DebugUI.Editor
{
    public static class CreateThemeAndPrefabsConfig
    {
        [MenuItem("Sorolla/Debug UI/1. Create Theme Asset")]
        public static void CreateThemeAsset()
        {
            // Ensure directories
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla"))
                AssetDatabase.CreateFolder("Assets", "Sorolla");
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla/DebugUI"))
                AssetDatabase.CreateFolder("Assets/Sorolla", "DebugUI");
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla/DebugUI/Resources"))
                AssetDatabase.CreateFolder("Assets/Sorolla/DebugUI", "Resources");

            string path = "Assets/Sorolla/DebugUI/Resources/SorollaDebugTheme.asset";

            var existing = AssetDatabase.LoadAssetAtPath<SorollaDebugTheme>(path);
            if (existing != null)
            {
                Debug.Log("Theme already exists at: " + path);
                Selection.activeObject = existing;
                return;
            }

            var theme = ScriptableObject.CreateInstance<SorollaDebugTheme>();
            AssetDatabase.CreateAsset(theme, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = theme;
            Debug.Log("Created SorollaDebugTheme at: " + path);
        }

        [MenuItem("Sorolla/Debug UI/2.  Create Prefabs Config Asset")]
        public static void CreatePrefabsConfigAsset()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Sorolla/DebugUI/Resources"))
            {
                Debug.LogError("Run 'Create Theme Asset' first to create directories.");
                return;
            }

            string path = "Assets/Sorolla/DebugUI/Resources/SorollaDebugPrefabs.asset";

            var existing = AssetDatabase.LoadAssetAtPath<SorollaDebugPrefabs>(path);
            if (existing != null)
            {
                Debug.Log("Prefabs config already exists at: " + path);
                Selection.activeObject = existing;
                return;
            }

            var prefabs = ScriptableObject.CreateInstance<SorollaDebugPrefabs>();
            AssetDatabase.CreateAsset(prefabs, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = prefabs;
            Debug.Log("Created SorollaDebugPrefabs at: " + path);
        }
    }
}
