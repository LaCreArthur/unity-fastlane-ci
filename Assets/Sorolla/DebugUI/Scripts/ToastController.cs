using System.Collections.Generic;
using UnityEngine;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Manages toast notification display. Pools toast instances and handles queuing.
    /// </summary>
    public class ToastController : UIComponentBase
    {
        [SerializeField] GameObject _toastPrefab;
        [SerializeField] Transform _toastContainer;
        [SerializeField] int _maxVisibleToasts = 3;

        readonly Queue<ToastNotification> _toastPool = new Queue<ToastNotification>();
        readonly List<ToastNotification> _activeToasts = new List<ToastNotification>();

        void Awake()
        {

            // Pre-warm pool
            for (int i = 0; i < _maxVisibleToasts; i++)
            {
                CreatePooledToast();
            }
        }

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnShowToast += HandleShowToast;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnShowToast -= HandleShowToast;

        void HandleShowToast(string message, ToastType type) => ShowToast(message, type);

        public void ShowToast(string message, ToastType type)
        {
            ToastNotification toast = GetToastFromPool();
            toast.Show(message, type);
            _activeToasts.Add(toast);

            // Limit visible toasts
            while (_activeToasts.Count > _maxVisibleToasts)
            {
                ToastNotification oldest = _activeToasts[0];
                _activeToasts.RemoveAt(0);
                ReturnToPool(oldest);
            }
        }

        ToastNotification GetToastFromPool()
        {
            if (_toastPool.Count > 0)
            {
                return _toastPool.Dequeue();
            }
            return CreatePooledToast();
        }

        void ReturnToPool(ToastNotification toast)
        {
            toast.gameObject.SetActive(false);
            _toastPool.Enqueue(toast);
        }

        ToastNotification CreatePooledToast()
        {
            GameObject toastGO = Instantiate(_toastPrefab, _toastContainer);
            toastGO.SetActive(false);
            var toast = toastGO.GetComponent<ToastNotification>();
            _toastPool.Enqueue(toast);
            return toast;
        }
    }
}
