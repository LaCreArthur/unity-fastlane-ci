using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class ToastNotification : UIComponentBase
    {
        [SerializeField] Image _background;
        [SerializeField] Image _dotIndicator;
        [SerializeField] TextMeshProUGUI _messageText;
        [SerializeField] CanvasGroup _canvasGroup;

        [SerializeField] float _displayDuration = 3f;
        [SerializeField] float _fadeDuration = 0.3f;

        Coroutine _hideCoroutine;

        public void Show(string message, ToastType type)
        {
            gameObject.SetActive(true);
            _messageText.text = message;

            Color toastColor = type switch
            {
                ToastType.Success => Theme.accentGreen,
                ToastType.Warning => Theme.accentYellow,
                ToastType.Error => Theme.accentRed,
                _ => Theme.accentPurple,
            };

            _dotIndicator.color = toastColor;
            _background.color = new Color(toastColor.r * 0.3f, toastColor.g * 0.3f, toastColor.b * 0.3f, 0.95f);

            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        IEnumerator HideAfterDelay()
        {
            _canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(_displayDuration);

            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = 1f - elapsed / _fadeDuration;
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
