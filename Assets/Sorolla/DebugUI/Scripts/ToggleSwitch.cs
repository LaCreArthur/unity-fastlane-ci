using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    public class ToggleSwitch : UIComponentBase, IPointerClickHandler
    {
        [SerializeField] Image trackImage;
        [SerializeField] Image knobImage;

        [SerializeField] string toggleId;
        [SerializeField] bool isOn;

        [SerializeField] float animationDuration = 0.15f;

        float _knobOffX;
        float _knobOnX;
        Coroutine _animationCoroutine;
        RectTransform _knobTransform;
        public event Action<bool> OnValueChanged;

        void Awake()
        {
            _knobTransform = knobImage.GetComponent<RectTransform>();

            // Calculate knob positions based on track width
            float trackWidth = trackImage.rectTransform.rect.width;
            _knobOffX = -trackWidth / 5f;
            _knobOnX = trackWidth / 5f;

            UpdateVisual(false);
        }

        public void OnPointerClick(PointerEventData eventData) => SetValue(!isOn, true);

        public void SetValue(bool value, bool animate = false)
        {
            isOn = value;
            UpdateVisual(animate);
            OnValueChanged?.Invoke(isOn);

            if (!string.IsNullOrEmpty(toggleId))
            {
                SorollaDebugEvents.RaiseToggleChanged(toggleId, isOn);
            }
        }

        void UpdateVisual(bool animate)
        {
            Color trackColor = isOn ? Theme.accentPurple : Theme.cardBackgroundLight;
            trackImage.color = trackColor;

            float targetX = isOn ? _knobOnX : _knobOffX;

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

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                Vector2 pos = _knobTransform.anchoredPosition;
                pos.x = Mathf.Lerp(startX, targetX, t);
                _knobTransform.anchoredPosition = pos;
                yield return null;
            }
        }
    }
}
