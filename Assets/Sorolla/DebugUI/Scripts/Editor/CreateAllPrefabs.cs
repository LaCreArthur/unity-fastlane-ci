using UnityEditor;
using UnityEngine;

namespace Sorolla.DebugUI.Editor
{
    public static class CreateAllPrefabs
    {
        [MenuItem("Sorolla/Debug UI/3.  Create All Prefabs")]
        public static void CreateAll()
        {
            Debug.Log("=== Creating Sorolla Debug UI Prefabs ===");

            // Ensure theme exists first
            var theme = AssetDatabase.LoadAssetAtPath<SorollaDebugTheme>(
                "Assets/Sorolla/DebugUI/Resources/SorollaDebugTheme.asset");

            if (theme == null)
            {
                Debug.LogError("Please create Theme first: Sorolla/Debug UI/1. Create Theme Asset");
                return;
            }

            // Create all prefabs in order
            CreateToastPrefab.Create();
            CreateStatusBadgePrefab.Create();
            CreateToggleSwitchPrefab.Create();
            CreateSDKHealthIndicatorPrefab.Create();
            CreateNavButtonPrefab.Create();
            CreateSectionCardPrefab.Create();
            CreateActionButtonPrefab.Create();
            CreateActionButtonPrefab.CreateFilled();
            CreateAdCardPrefab.Create();
            CreateIdentityCardPrefab.Create();
            CreateToggleRowPrefab.Create();
            CreateLogEntryPrefab.Create();
            CreateConfigRowPrefab.Create();
            CreateBottomNavBarPrefab.Create();
            CreateDividerPrefab.Create();
            CreateHeaderPrefab.Create();
            CreateLogFilterBarPrefab.Create();
            CreateMediationTesterCardPrefab.Create();

            AssetDatabase.Refresh();

            Debug.Log("=== All Prefabs Created Successfully ===");
            Debug.Log("Next: Assign prefabs to SorollaDebugPrefabs asset, then run 'Create Full Debug Panel'");
        }

        [MenuItem("Sorolla/Debug UI/4. Auto-Assign Prefabs to Config")]
        public static void AutoAssignPrefabs()
        {
            string configPath = "Assets/Sorolla/DebugUI/Resources/SorollaDebugPrefabs.asset";
            var config = AssetDatabase.LoadAssetAtPath<SorollaDebugPrefabs>(configPath);

            if (config == null)
            {
                Debug.LogError("Please create Prefabs Config first: Sorolla/Debug UI/2. Create Prefabs Config Asset");
                return;
            }

            string prefabPath = "Assets/Sorolla/DebugUI/Prefabs/";

            var so = new SerializedObject(config);

            // Assign each prefab
            AssignPrefab(so, "sectionCard", prefabPath + "SectionCard.prefab");
            AssignPrefab(so, "actionButton", prefabPath + "ActionButton.prefab");
            AssignPrefab(so, "toggleSwitch", prefabPath + "ToggleSwitch.prefab");
            AssignPrefab(so, "adCard", prefabPath + "AdCard.prefab");
            AssignPrefab(so, "identityCard", prefabPath + "IdentityCard.prefab");
            AssignPrefab(so, "sdkHealthIndicator", prefabPath + "SDKHealthIndicator.prefab");
            AssignPrefab(so, "configRow", prefabPath + "ConfigRow.prefab");
            AssignPrefab(so, "toggleRow", prefabPath + "ToggleRow.prefab");
            AssignPrefab(so, "navButton", prefabPath + "NavButton.prefab");
            AssignPrefab(so, "bottomNavBar", prefabPath + "BottomNavBar.prefab");
            AssignPrefab(so, "toastNotification", prefabPath + "ToastNotification.prefab");
            AssignPrefab(so, "statusBadge", prefabPath + "StatusBadge.prefab");
            AssignPrefab(so, "logEntry", prefabPath + "LogEntry.prefab");
            AssignPrefab(so, "divider", prefabPath + "Divider.prefab");

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            Debug.Log("Prefabs auto-assigned to config!");
            Selection.activeObject = config;
        }

        static void AssignPrefab(SerializedObject so, string propertyName, string prefabPath)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                so.FindProperty(propertyName).objectReferenceValue = prefab;
            }
            else
            {
                Debug.LogWarning($"Prefab not found: {prefabPath}");
            }
        }
    }
}
