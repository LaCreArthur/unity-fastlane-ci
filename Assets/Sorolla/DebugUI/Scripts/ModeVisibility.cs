using UnityEngine;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls visibility of UI elements based on SDK mode (Prototype vs Full).
    ///     Attach to any GameObject that should only show in specific modes.
    /// </summary>
    public class ModeVisibility : UIComponentBase
    {
        public enum VisibilityMode
        {
            AlwaysVisible,
            PrototypeOnly,
            FullOnly,
        }

        [SerializeField] VisibilityMode _mode = VisibilityMode.AlwaysVisible;

        void Start() => UpdateVisibility();

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnModeChanged += HandleModeChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnModeChanged -= HandleModeChanged;

        void HandleModeChanged(SorollaMode mode) => UpdateVisibility();

        void UpdateVisibility()
        {
            bool isPrototype = Sorolla.Config == null || Sorolla.Config.isPrototypeMode;

            bool shouldShow = _mode switch
            {
                VisibilityMode.AlwaysVisible => true,
                VisibilityMode.PrototypeOnly => isPrototype,
                VisibilityMode.FullOnly => !isPrototype,
                _ => true,
            };

            gameObject.SetActive(shouldShow);
        }
    }
}
