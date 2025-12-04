using System.Collections.Generic;
using UnityEngine;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Manages tab switching.  Listens to OnTabChanged event and activates/deactivates tab pages.
    ///     No direct references to tab pages needed - finds them by convention.
    /// </summary>
    public class TabController : UIComponentBase
    {
        [SerializeField] List<GameObject> _tabPages = new List<GameObject>();
        [SerializeField] int _defaultTabIndex;

        public int CurrentTabIndex { get; private set; } = -1;
        public int TabCount => _tabPages.Count;

        protected override void Awake()
        {
            base.Awake();

            // Auto-find tab pages if not assigned
            if (_tabPages.Count == 0)
            {
                AutoFindTabPages();
            }
        }

        void Start()
        {
            // Set default tab
            SetActiveTab(_defaultTabIndex, false);
            SorollaDebugEvents.RaiseTabChanged(_defaultTabIndex);
        }

        protected override void SubscribeToEvents() => SorollaDebugEvents.OnTabChanged += HandleTabChanged;

        protected override void UnsubscribeFromEvents() => SorollaDebugEvents.OnTabChanged -= HandleTabChanged;

        void HandleTabChanged(int tabIndex) => SetActiveTab(tabIndex, true);

        void SetActiveTab(int index, bool raiseEvent)
        {
            if (index < 0 || index >= _tabPages.Count) return;
            if (index == CurrentTabIndex) return;

            CurrentTabIndex = index;

            for (int i = 0; i < _tabPages.Count; i++)
            {
                _tabPages[i].SetActive(i == index);
            }
        }

        void AutoFindTabPages()
        {
            // Look for TabPages container
            Transform tabPagesContainer = transform.Find("SafeAreaContainer/ContentArea/TabPages");

            if (tabPagesContainer == null)
            {
                // Try alternative paths
                tabPagesContainer = transform.Find("ContentArea/TabPages");
            }

            if (tabPagesContainer != null)
            {
                _tabPages.Clear();
                foreach (Transform child in tabPagesContainer)
                {
                    _tabPages.Add(child.gameObject);
                }
            }
        }

        public void SetTabPages(List<GameObject> pages) => _tabPages = pages;
    }
}
