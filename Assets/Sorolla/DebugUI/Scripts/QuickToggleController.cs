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
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] ToggleSwitch toggle;

        public event Action<bool> OnToggleChanged;

        void Awake() => toggle.OnValueChanged += HandleToggleChanged;
        void OnDestroy() => toggle.OnValueChanged -= HandleToggleChanged;

        void HandleToggleChanged(bool value)
        {
            OnToggleChanged?.Invoke(value);

            DebugPanelManager.Instance?.Log($"{label.text}: {(value ? "ON" : "OFF")}");
            if (value)
                SorollaDebugEvents.RaiseShowToast($"{label.text} Enabled", ToastType.Success);
        }
    }
}
