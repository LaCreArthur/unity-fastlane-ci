using UnityEditor;
using UnityEngine;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Toggle to switch SDK mode (Prototype/Full) in Editor/Debug builds.
    ///     Actual SDK behavior won't change until app restart.
    /// </summary>
    public class ModeSwitchController : UIComponentBase
    {
        [SerializeField] ToggleSwitch _toggle;
        [SerializeField] GameObject _container; // Hide entire row in release builds

        bool IsPrototype => Sorolla.Config == null || Sorolla.Config.isPrototypeMode;

        void Awake()
        {

#if !UNITY_EDITOR && !DEBUG
            // Hide in release builds
            _container.SetActive(false);
            return;
#endif

            _toggle.OnValueChanged += HandleToggleChanged;
            _toggle.SetValue(!IsPrototype); // Toggle ON = Full mode
        }

        void OnDestroy() => _toggle.OnValueChanged -= HandleToggleChanged;

        void HandleToggleChanged(bool isFullMode)
        {
#if UNITY_EDITOR
            // Actually change the config in Editor
            if (Sorolla.Config != null)
            {
                Sorolla.Config.isPrototypeMode = !isFullMode;
                EditorUtility.SetDirty(Sorolla.Config);
            }
#endif

            // Notify UI to refresh
            SorollaMode newMode = isFullMode ? SorollaMode.Full : SorollaMode.Prototype;
            SorollaDebugEvents.RaiseModeChanged(newMode);

            // Show warning about SDK behavior
            string modeName = isFullMode ? "Full" : "Prototype";
            SorollaDebugEvents.RaiseShowToast($"UI switched to {modeName} mode", ToastType.Info);
            DebugPanelManager.Instance?.Log($"Mode: {modeName} (SDK unchanged until restart)", LogSource.Sorolla, LogLevel.Warning);
        }
    }
}
