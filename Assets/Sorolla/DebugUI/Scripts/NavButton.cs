using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class NavButton : UIComponentBase, IPointerClickHandler
    {
        [SerializeField] Image _backgroundHighlight;
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _label;
        [SerializeField] int _tabIndex;

        bool _isSelected;

        public void OnPointerClick(PointerEventData eventData) => SorollaDebugEvents.RaiseTabChanged(_tabIndex);

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnTabChanged += HandleTabChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnTabChanged -= HandleTabChanged;

        void HandleTabChanged(int tabIndex) => SetSelected(tabIndex == _tabIndex);

        public void Setup(int tabIndex, Sprite iconSprite, string labelText)
        {
            _tabIndex = tabIndex;
            _icon.sprite = iconSprite;
            _label.text = labelText;
        }

        void SetSelected(bool selected)
        {
            _isSelected = selected;
            _backgroundHighlight.gameObject.SetActive(selected);

            Color tint = selected ? Theme.accentPurple : Theme.textSecondary;
            _icon.color = tint;
            _label.color = tint;
        }
    }
}
