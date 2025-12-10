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

        static LogFilterButton s_currentSelected;
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] LogLevel filterLevel;

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
            if (s_currentSelected != null)
            {
                s_currentSelected.SetSelected(false);
            }

            SetSelected(true);
            s_currentSelected = this;

            SorollaDebugEvents.RaiseLogFilterChanged(filterLevel);
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            background.color = selected ? Theme.cardBackgroundLight : Color.clear;
            label.color = selected ? Theme.textPrimary : Theme.textSecondary;
        }

        public void Initialize(LogLevel level, bool startSelected = false)
        {
            filterLevel = level;
            label.text = level.ToString().ToUpper();

            if (startSelected)
            {
                SetSelected(true);
                s_currentSelected = this;
            }
        }
    }
}
