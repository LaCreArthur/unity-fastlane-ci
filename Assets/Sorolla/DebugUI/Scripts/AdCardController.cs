using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls an individual ad card. Self-sufficient - calls Sorolla API directly.
    /// </summary>
    public class AdCardController : UIComponentBase
    {
        [Header("References")]
        [SerializeField] Image accentBar;
        [SerializeField] StatusBadge statusBadge;
        [SerializeField] Button loadButton;
        [SerializeField] Button showButton;

        [Header("Configuration")]
        [SerializeField] AdType adType;
        [SerializeField] Color accentColor;

        AdStatus _currentStatus = AdStatus.Idle;

        void Awake()
        {
            loadButton.onClick.AddListener(HandleLoadClicked);
            showButton.onClick.AddListener(HandleShowClicked);
        }

        void OnDestroy()
        {
            loadButton.onClick.RemoveListener(HandleLoadClicked);
            showButton.onClick.RemoveListener(HandleShowClicked);
        }

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnAdStatusChanged += HandleAdStatusChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnAdStatusChanged -= HandleAdStatusChanged;

        void HandleAdStatusChanged(AdType adType, AdStatus status)
        {
            if (adType == this.adType)
            {
                SetStatus(status);
            }
        }

        void HandleLoadClicked()
        {
            SetStatus(AdStatus.Loading);
            DebugPanelManager.Instance?.Log($"Loading {adType}...", LogSource.Sorolla);

#if UNITY_EDITOR
            // Editor mock: simulate ad loaded after delay
            Invoke(nameof(MockAdLoaded), 1f);
#endif
            // Real SDK auto-loads ads - this is just for UI feedback
        }

        void HandleShowClicked()
        {
            SetStatus(AdStatus.Showing);
            DebugPanelManager.Instance?.Log($"Showing {adType}...", LogSource.Sorolla);

#if UNITY_EDITOR
            // Editor mock: simulate ad completion
            Invoke(nameof(MockAdComplete), 2f);
#else
            ShowAdViaSDK();
#endif
        }

        void ShowAdViaSDK()
        {
            switch (adType)
            {
                case AdType.Interstitial:
                    Sorolla.ShowInterstitialAd(() =>
                    {
                        SetStatus(AdStatus.Idle);
                        SorollaDebugEvents.RaiseShowToast("Interstitial completed", ToastType.Success);
                        DebugPanelManager.Instance?.Log("Interstitial completed", LogSource.Sorolla);
                    });
                    break;

                case AdType.Rewarded:
                    Sorolla.ShowRewardedAd(
                        () =>
                        {
                            SetStatus(AdStatus.Idle);
                            SorollaDebugEvents.RaiseShowToast("Rewarded ad completed!", ToastType.Success);
                            DebugPanelManager.Instance?.Log("Rewarded ad - reward granted", LogSource.Sorolla);
                        },
                        () =>
                        {
                            SetStatus(AdStatus.Failed);
                            SorollaDebugEvents.RaiseShowToast("Rewarded ad failed", ToastType.Error);
                            DebugPanelManager.Instance?.Log("Rewarded ad failed", LogSource.Sorolla, LogLevel.Error);
                        });
                    break;
            }
        }

        public void SetStatus(AdStatus status)
        {
            _currentStatus = status;

            switch (status)
            {
                case AdStatus.Idle:
                    statusBadge.SetIdle();
                    break;
                case AdStatus.Loading:
                    statusBadge.SetLoading();
                    break;
                case AdStatus.Loaded:
                    statusBadge.SetLoaded();
                    break;
                case AdStatus.Showing:
                    statusBadge.SetStatus("SHOWING", Theme.accentPurple);
                    break;
                case AdStatus.Failed:
                    statusBadge.SetFailed();
                    break;
            }

            UpdateButtonStates();
        }

        void UpdateButtonStates() => showButton.interactable = _currentStatus == AdStatus.Loaded;


#if UNITY_EDITOR
        void MockAdLoaded()
        {
            SetStatus(AdStatus.Loaded);
            SorollaDebugEvents.RaiseShowToast($"{adType} ready (mock)", ToastType.Info);
        }

        void MockAdComplete()
        {
            SetStatus(AdStatus.Idle);
            SorollaDebugEvents.RaiseShowToast($"{adType} completed (mock)", ToastType.Success);
            DebugPanelManager.Instance?.Log($"{adType} completed (mock)", LogSource.Sorolla);
        }
#endif
    }
}
