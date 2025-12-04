using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class SDKHealthIndicator : UIComponentBase
    {
        [SerializeField] Image _background;
        [SerializeField] TextMeshProUGUI _sdkNameLabel;
        [SerializeField] Image _statusDot;

        [SerializeField] string _sdkName;
        [SerializeField] bool _isEnabled = true;
        [SerializeField] bool _isHealthy;

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnSDKHealthChanged += HandleSDKHealthChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnSDKHealthChanged -= HandleSDKHealthChanged;

        void HandleSDKHealthChanged(string sdkName, bool isHealthy)
        {
            if (sdkName == _sdkName)
            {
                SetHealth(isHealthy);
            }
        }

        public void Setup(string sdkName, bool enabled, bool healthy)
        {
            _sdkName = sdkName;
            _isEnabled = enabled;
            _isHealthy = healthy;
            UpdateVisual();
        }

        public void SetHealth(bool healthy)
        {
            _isHealthy = healthy;
            UpdateVisual();
        }

        void UpdateVisual()
        {
            _sdkNameLabel.text = _sdkName;
            _sdkNameLabel.color = _isEnabled ? Theme.textPrimary : Theme.textDisabled;
            _statusDot.color = _isEnabled
                ? _isHealthy ? Theme.statusActive : Theme.statusIdle
                : Theme.statusIdle;
        }
    }
}
