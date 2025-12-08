using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Individual filter button in the log filter bar.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LogFilterButton : UIComponentBase
    {

        static LogFilterButton _currentSelected;
        [SerializeField] Image _background;
        [SerializeField] TextMeshProUGUI _label;
        [SerializeField] LogLevel _filterLevel;

        Button _button;
        bool _isSelected;

        void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        void OnDestroy() => _button.onClick.RemoveListener(OnClick);

        void OnClick()
        {
            if (_currentSelected != null)
            {
                _currentSelected.SetSelected(false);
            }

            SetSelected(true);
            _currentSelected = this;

            SorollaDebugEvents.RaiseLogFilterChanged(_filterLevel);
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            _background.color = selected ? Theme.cardBackgroundLight : Color.clear;
            _label.color = selected ? Theme.textPrimary : Theme.textSecondary;
        }

        public void Initialize(LogLevel level, bool startSelected = false)
        {
            _filterLevel = level;
            _label.text = level.ToString().ToUpper();

            if (startSelected)
            {
                SetSelected(true);
                _currentSelected = this;
            }
        }
    }
}
