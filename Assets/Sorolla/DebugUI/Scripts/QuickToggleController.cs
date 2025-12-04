using System;
using TMPro;
using UnityEngine;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls a toggle row for quick settings.
    /// </summary>
    public class QuickToggleController : UIComponentBase
    {
        [SerializeField] TextMeshProUGUI _label;
        [SerializeField] ToggleSwitch _toggle;
        [SerializeField] string _toggleId;
        [SerializeField] string _labelText;

        public Action<bool> OnToggleChanged;

        public bool IsOn => _toggle.IsOn;

        protected override void Awake()
        {
            base.Awake();
            _toggle.OnValueChanged += HandleToggleChanged;
            UpdateVisuals();
        }

        void OnDestroy() => _toggle.OnValueChanged -= HandleToggleChanged;

        void HandleToggleChanged(bool value)
        {
            OnToggleChanged?.Invoke(value);

            DebugPanelManager.Instance?.Log(
                $"{_labelText}: {(value ? "ON" : "OFF")}"
            );

            if (value)
            {
                SorollaDebugEvents.RaiseShowToast($"{_labelText} Enabled", ToastType.Success);
            }
        }

        public void Setup(string toggleId, string labelText, bool initialValue = false)
        {
            _toggleId = toggleId;
            _labelText = labelText;
            UpdateVisuals();
            _toggle.SetValue(initialValue);
        }

        void UpdateVisuals() => _label.text = _labelText;
    }
}
