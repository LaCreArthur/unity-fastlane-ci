using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls an identity card with copy-to-clipboard functionality.
    ///     Self-sufficient - can auto-populate with real device info.
    /// </summary>
    public class IdentityCardController : UIComponentBase
    {
        public enum IdentityType
        {
            Custom,
            DeviceId,
            Platform,
            AppVersion,
            SorollaMode,
            IDFA,
            AdjustId,
        }

        [SerializeField] TextMeshProUGUI _labelText;
        [SerializeField] TextMeshProUGUI _valueText;
        [SerializeField] Button _copyButton;
        [SerializeField] IdentityType _identityType = IdentityType.Custom;
        [SerializeField] string _customLabel;

        string _value;

        void Awake() => _copyButton.onClick.AddListener(CopyToClipboard);

        void OnDestroy() => _copyButton.onClick.RemoveListener(CopyToClipboard);

        void Start()
        {
            if (_identityType != IdentityType.Custom)
            {
                AutoPopulate();
            }
        }

        void AutoPopulate()
        {
            string label;
            string value;

            switch (_identityType)
            {
                case IdentityType.DeviceId:
                    label = "Device ID";
                    value = SystemInfo.deviceUniqueIdentifier;
                    break;
                case IdentityType.Platform:
                    label = "Platform";
                    value = $"{Application.platform} ({SystemInfo.operatingSystem})";
                    break;
                case IdentityType.AppVersion:
                    label = "App Version";
                    value = $"{Application.version} ({Application.unityVersion})";
                    break;
                case IdentityType.SorollaMode:
                    label = "SDK Mode";
                    bool isPrototype = Sorolla.Config == null || Sorolla.Config.isPrototypeMode;
                    value = isPrototype ? "Prototype" : "Full";
                    break;
                case IdentityType.IDFA:
                    label = "IDFA";
                    value = GetAdvertisingId();
                    break;
                case IdentityType.AdjustId:
                    label = "Adjust ID";
                    value = GetAdjustId();
                    break;
                default:
                    label = _customLabel;
                    value = "—";
                    break;
            }

            Setup(label, value);
        }

        string GetAdvertisingId()
        {
#if UNITY_IOS
            return UnityEngine.iOS.Device.advertisingIdentifier;
#elif UNITY_ANDROID
            // Android requires async call - show placeholder
            return "Tap to fetch...";
#else
            return "Not available";
#endif
        }

        string GetAdjustId() =>
            // This would need Adjust SDK integration
            "N/A";

        public void Setup(string label, string value)
        {
            _value = value;
            _labelText.text = label;
            _valueText.text = value;
        }

        void CopyToClipboard()
        {
            if (string.IsNullOrEmpty(_value)) return;

            GUIUtility.systemCopyBuffer = _value;

            SorollaDebugEvents.RaiseShowToast("Copied to clipboard!", ToastType.Success);
            DebugPanelManager.Instance?.Log($"Copied: {_value}");
        }
    }
}
