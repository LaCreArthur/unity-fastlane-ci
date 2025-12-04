using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls an individual ad card.  Subscribes to ad status events.
    /// </summary>
    public class AdCardController : UIComponentBase
    {
        [Header("References")]
        [SerializeField] Image _accentBar;
        [SerializeField] TextMeshProUGUI _titleLabel;
        [SerializeField] TextMeshProUGUI _subtitleLabel;
        [SerializeField] StatusBadge _statusBadge;
        [SerializeField] Button _loadButton;
        [SerializeField] Button _showButton;

        [Header("Configuration")]
        [SerializeField] AdType _adType;
        [SerializeField] string _title = "Interstitial";
        [SerializeField] string _subtitle = "Full screen break";
        [SerializeField] Color _accentColor;

        AdStatus _currentStatus = AdStatus.Idle;

        public Action OnLoadClicked;
        public Action OnShowClicked;

        protected override void Awake()
        {
            base.Awake();

            _loadButton.onClick.AddListener(HandleLoadClicked);
            _showButton.onClick.AddListener(HandleShowClicked);

            UpdateVisuals();
        }

        void OnDestroy()
        {
            _loadButton.onClick.RemoveListener(HandleLoadClicked);
            _showButton.onClick.RemoveListener(HandleShowClicked);
        }

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnAdStatusChanged += HandleAdStatusChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnAdStatusChanged -= HandleAdStatusChanged;

        void HandleAdStatusChanged(AdType adType, AdStatus status)
        {
            if (adType == _adType)
            {
                SetStatus(status);
            }
        }

        void HandleLoadClicked()
        {
            OnLoadClicked?.Invoke();

            // Log the action
            DebugPanelManager.Instance?.Log($"Loading {_adType}.. .");
        }

        void HandleShowClicked()
        {
            OnShowClicked?.Invoke();

            // Log the action
            DebugPanelManager.Instance?.Log($"Showing {_adType}...");
        }

        public void SetStatus(AdStatus status)
        {
            _currentStatus = status;

            switch (status)
            {
                case AdStatus.Idle:
                    _statusBadge.SetIdle();
                    break;
                case AdStatus.Loading:
                    _statusBadge.SetLoading();
                    break;
                case AdStatus.Loaded:
                    _statusBadge.SetLoaded();
                    break;
                case AdStatus.Showing:
                    _statusBadge.SetStatus("SHOWING", Theme.accentPurple);
                    break;
                case AdStatus.Failed:
                    _statusBadge.SetFailed();
                    break;
            }

            UpdateButtonStates();
        }

        void UpdateButtonStates() => _showButton.interactable = _currentStatus == AdStatus.Loaded;

        void UpdateVisuals()
        {
            _accentBar.color = _accentColor;
            _titleLabel.text = _title;
            _subtitleLabel.text = _subtitle;
        }

        public void Configure(AdType adType, string title, string subtitle, Color accentColor)
        {
            _adType = adType;
            _title = title;
            _subtitle = subtitle;
            _accentColor = accentColor;
            UpdateVisuals();
        }
    }
}
