using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Displays remote config key-value pairs.  Updates on fetch.
    /// </summary>
    public class RemoteConfigDisplay : UIComponentBase
    {
        [SerializeField] GameObject _configRowPrefab;
        [SerializeField] Transform _container;
        [SerializeField] Button _fetchButton;

        readonly List<GameObject> _rows = new List<GameObject>();

        public Action OnFetchRequested;

        protected override void Awake()
        {
            base.Awake();
            _fetchButton.onClick.AddListener(HandleFetchClicked);
        }

        void OnDestroy() => _fetchButton.onClick.RemoveListener(HandleFetchClicked);

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnRemoteConfigFetched += HandleConfigFetched;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnRemoteConfigFetched -= HandleConfigFetched;

        void HandleFetchClicked()
        {
            OnFetchRequested?.Invoke();
            _fetchButton.interactable = false;
            DebugPanelManager.Instance?.Log("Fetching Remote Config...", LogSource.Firebase);
        }

        void HandleConfigFetched(bool success)
        {
            _fetchButton.interactable = true;

            if (success)
            {
                SorollaDebugEvents.RaiseShowToast("Remote Config fetched!", ToastType.Success);
            }
            else
            {
                SorollaDebugEvents.RaiseShowToast("Failed to fetch config", ToastType.Error);
            }
        }

        public void SetConfigs(Dictionary<string, object> configs)
        {
            ClearRows();

            foreach (var kvp in configs)
            {
                AddConfigRow(kvp.Key, kvp.Value);
            }
        }

        public void AddConfigRow(string key, object value)
        {
            GameObject row = Instantiate(_configRowPrefab, _container);
            _rows.Add(row);

            // Find and set labels
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
            if (value is string str)
            {
                return $"\"{str}\"";
            }
            if (value is float f)
            {
                return f.ToString("F1");
            }
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
