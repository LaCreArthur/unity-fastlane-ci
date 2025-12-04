using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls an identity card with copy-to-clipboard functionality.
    /// </summary>
    public class IdentityCardController : UIComponentBase
    {
        [SerializeField] TextMeshProUGUI _labelText;
        [SerializeField] TextMeshProUGUI _valueText;
        [SerializeField] Button _copyButton;

        string _value;

        protected override void Awake()
        {
            base.Awake();
            _copyButton.onClick.AddListener(CopyToClipboard);
        }

        void OnDestroy() => _copyButton.onClick.RemoveListener(CopyToClipboard);

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
