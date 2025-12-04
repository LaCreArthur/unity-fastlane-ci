using UnityEngine;

namespace Sorolla.DebugUI
{
    [CreateAssetMenu(fileName = "SorollaDebugPrefabs", menuName = "Sorolla/Debug UI Prefabs")]
    public class SorollaDebugPrefabs : ScriptableObject
    {

        // Singleton access
        static SorollaDebugPrefabs _instance;
        [Header("Core Prefabs")]
        public GameObject sectionCard;
        public GameObject actionButton;
        public GameObject iconButton;
        public GameObject toggleSwitch;

        [Header("Card Prefabs")]
        public GameObject adCard;
        public GameObject identityCard;
        public GameObject sdkHealthIndicator;
        public GameObject configRow;
        public GameObject toggleRow;

        [Header("Navigation")]
        public GameObject navButton;
        public GameObject bottomNavBar;

        [Header("Feedback")]
        public GameObject toastNotification;
        public GameObject statusBadge;

        [Header("Logs")]
        public GameObject logEntry;

        [Header("Misc")]
        public GameObject accentBar;
        public GameObject divider;
        public static SorollaDebugPrefabs Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = Resources.Load<SorollaDebugPrefabs>("SorollaDebugPrefabs");
                }
                return _instance;
            }
        }
    }
}
