using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class LogEntryView : UIComponentBase
    {
        [SerializeField] Image _accentBar;
        [SerializeField] TextMeshProUGUI _timestampLabel;
        [SerializeField] Image _sourceBadgeBackground;
        [SerializeField] TextMeshProUGUI _sourceBadgeLabel;
        [SerializeField] TextMeshProUGUI _messageLabel;

        public void SetData(LogEntryData data)
        {
            _accentBar.color = data.accentColor;
            _timestampLabel.text = data.timestamp;
            _sourceBadgeLabel.text = data.source.ToString().ToUpper();
            _messageLabel.text = data.message;
            _messageLabel.color = GetMessageColor(data.level);
        }

        Color GetMessageColor(LogLevel level) => level switch
        {
            LogLevel.Warning => Theme.accentYellow,
            LogLevel.Error => Theme.accentRed,
            _ => Theme.textPrimary,
        };
    }
}
