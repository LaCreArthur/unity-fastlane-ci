using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class ToggleSwitch : UIComponentBase, IPointerClickHandler
    {
        [SerializeField] Image _trackImage;
        [SerializeField] Image _knobImage;
        [SerializeField] RectTransform _knobTransform;

        [SerializeField] string _toggleId;
        [SerializeField] bool _isOn;

        [SerializeField] float _animationDuration = 0.15f;

        float _knobOffX;
        float _knobOnX;
        Coroutine _animationCoroutine;
        public event Action<bool> OnValueChanged;

        public bool IsOn => _isOn;

        protected override void Awake()
        {
            base.Awake();
            // Calculate knob positions based on track width
            float trackWidth = _trackImage.rectTransform.rect.width;
            float knobWidth = _knobTransform.rect.width;
            _knobOffX = -trackWidth / 4f;
            _knobOnX = trackWidth / 4f;

            UpdateVisual(false);
        }

        public void OnPointerClick(PointerEventData eventData) => SetValue(!_isOn, true);

        public void SetValue(bool value, bool animate = false)
        {
            _isOn = value;
            UpdateVisual(animate);
            OnValueChanged?.Invoke(_isOn);

            if (!string.IsNullOrEmpty(_toggleId))
            {
                SorollaDebugEvents.RaiseToggleChanged(_toggleId, _isOn);
            }
        }

        void UpdateVisual(bool animate)
        {
            Color trackColor = _isOn ? Theme.accentPurple : Theme.cardBackgroundLight;
            _trackImage.color = trackColor;

            float targetX = _isOn ? _knobOnX : _knobOffX;

            if (animate && gameObject.activeInHierarchy)
            {
                if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
                _animationCoroutine = StartCoroutine(AnimateKnob(targetX));
            }
            else
            {
                Vector2 pos = _knobTransform.anchoredPosition;
                pos.x = targetX;
                _knobTransform.anchoredPosition = pos;
            }
        }

        IEnumerator AnimateKnob(float targetX)
        {
            float startX = _knobTransform.anchoredPosition.x;
            float elapsed = 0f;

            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _animationDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                Vector2 pos = _knobTransform.anchoredPosition;
                pos.x = Mathf.Lerp(startX, targetX, t);
                _knobTransform.anchoredPosition = pos;
                yield return null;
            }
        }
    }
}
