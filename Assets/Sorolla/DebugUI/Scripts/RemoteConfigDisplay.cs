using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Displays remote config key-value pairs. Self-sufficient - calls Sorolla API directly.
    /// </summary>
    public class RemoteConfigDisplay : UIComponentBase
    {
        [SerializeField] GameObject _configRowPrefab;
        [SerializeField] Transform _container;
        [SerializeField] Button _fetchButton;

        [Header("Config Keys to Display")]
        [SerializeField] string[] _keysToDisplay = { "feature_enabled", "ad_frequency", "level_config" };

        readonly List<GameObject> _rows = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
            _fetchButton.onClick.AddListener(HandleFetchClicked);
        }

        void OnDestroy() => _fetchButton.onClick.RemoveListener(HandleFetchClicked);

        void Start()
        {
            // Show initial values if already available
            if (Sorolla.IsRemoteConfigReady())
            {
                RefreshConfigDisplay();
            }
        }

        void HandleFetchClicked()
        {
            _fetchButton.interactable = false;
            DebugPanelManager.Instance?.Log("Fetching Remote Config...", LogSource.Firebase);

#if UNITY_EDITOR
            // Editor mock
            Invoke(nameof(MockFetchComplete), 1f);
#else
            Sorolla.FetchRemoteConfig(OnFetchComplete);
#endif
        }

        void OnFetchComplete(bool success)
        {
            _fetchButton.interactable = true;

            if (success)
            {
                SorollaDebugEvents.RaiseShowToast("Remote Config fetched!", ToastType.Success);
                DebugPanelManager.Instance?.Log("Remote Config fetch successful", LogSource.Firebase);
                RefreshConfigDisplay();
            }
            else
            {
                SorollaDebugEvents.RaiseShowToast("Failed to fetch config", ToastType.Error);
                DebugPanelManager.Instance?.Log("Remote Config fetch failed", LogSource.Firebase, LogLevel.Error);
            }
        }

        void RefreshConfigDisplay()
        {
            ClearRows();

            foreach (string key in _keysToDisplay)
            {
                string value = Sorolla.GetRemoteConfig(key, "—");
                AddConfigRow(key, value);
            }
        }

#if UNITY_EDITOR
        void MockFetchComplete()
        {
            _fetchButton.interactable = true;
            SorollaDebugEvents.RaiseShowToast("Remote Config fetched (mock)", ToastType.Success);
            DebugPanelManager.Instance?.Log("Remote Config fetch successful (mock)", LogSource.Firebase);

            // Show mock values
            ClearRows();
            AddConfigRow("feature_enabled", true);
            AddConfigRow("ad_frequency", 3);
            AddConfigRow("level_config", "easy");
            AddConfigRow("reward_multiplier", 1.5f);
        }
#endif

        public void AddConfigRow(string key, object value)
        {
            GameObject row = Instantiate(_configRowPrefab, _container);
            _rows.Add(row);

            var texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = key;
                texts[1].text = FormatValue(value);
                texts[1].color = GetColorForType(value);
            }
        }

        void ClearRows()
        {
            foreach (GameObject row in _rows)
            {
                Destroy(row);
            }
            _rows.Clear();
        }

        string FormatValue(object value)
        {
            if (value is string str) return $"\"{str}\"";
            if (value is float f) return f.ToString("F1");
            if (value is bool b) return b ? "true" : "false";
            return value?.ToString() ?? "null";
        }

        Color GetColorForType(object value)
        {
            if (value is string) return Theme.accentOrange;
            if (value is float or double) return Theme.accentYellow;
            if (value is int or long) return Theme.accentCyan;
            if (value is bool) return Theme.accentPurple;
            return Theme.textPrimary;
        }
    }
}
