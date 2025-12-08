using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class NavButton : UIComponentBase, IPointerClickHandler
    {
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _label;
        [SerializeField] int _tabIndex;

        Image _backgroundHighlight;

        void Awake() => _backgroundHighlight = GetComponent<Image>();

        public void OnPointerClick(PointerEventData eventData) => SorollaDebugEvents.RaiseTabChanged(_tabIndex);

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnTabChanged += HandleTabChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnTabChanged -= HandleTabChanged;

        void HandleTabChanged(int tabIndex) => SetSelected(tabIndex == _tabIndex);

        void SetSelected(bool selected)
        {
            _backgroundHighlight.enabled = selected;

            Color tint = selected ? Theme.accentPurple : Theme.textSecondary;
            _icon.color = tint;
            _label.color = tint;
        }
    }
}
