using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Manages log entries with object pooling.  Handles filtering and clearing.
    /// </summary>
    public class LogController : UIComponentBase
    {
        [SerializeField] GameObject _logEntryPrefab;
        [SerializeField] Transform _logContainer;
        [SerializeField] ScrollRect _scrollRect;
        [SerializeField] int _maxLogEntries = 100;
        [SerializeField] int _poolSize = 20;

        readonly Queue<LogEntryView> _pool = new Queue<LogEntryView>();
        readonly List<LogEntryView> _activeEntries = new List<LogEntryView>();
        readonly List<LogEntryData> _allLogs = new List<LogEntryData>();

        LogLevel _currentFilter = LogLevel.All;

        void Awake() => InitializePool();

        protected override void SubscribeToEvents()
        {
            SorollaDebugEvents.OnLogAdded += HandleLogAdded;
            SorollaDebugEvents.OnLogsClear += HandleLogsClear;
            SorollaDebugEvents.OnLogFilterChanged += HandleFilterChanged;
        }

        protected override void UnsubscribeFromEvents()
        {
            SorollaDebugEvents.OnLogAdded -= HandleLogAdded;
            SorollaDebugEvents.OnLogsClear -= HandleLogsClear;
            SorollaDebugEvents.OnLogFilterChanged -= HandleFilterChanged;
        }

        void InitializePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                CreatePooledEntry();
            }
        }

        LogEntryView CreatePooledEntry()
        {
            GameObject entryGO = Instantiate(_logEntryPrefab, _logContainer);
            entryGO.SetActive(false);
            var entry = entryGO.GetComponent<LogEntryView>();
            _pool.Enqueue(entry);
            return entry;
        }

        LogEntryView GetFromPool()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }
            return CreatePooledEntry();
        }

        void ReturnToPool(LogEntryView entry)
        {
            entry.gameObject.SetActive(false);
            entry.transform.SetAsLastSibling();
            _pool.Enqueue(entry);
        }

        void HandleLogAdded(LogEntryData data) => AddLog(data);

        void HandleLogsClear() => ClearLogs();

        void HandleFilterChanged(LogLevel level) => SetFilter(level);

        public void AddLog(LogEntryData data)
        {
            _allLogs.Add(data);

            // Trim old logs
            while (_allLogs.Count > _maxLogEntries)
            {
                _allLogs.RemoveAt(0);
            }

            // Check if passes current filter
            if (PassesFilter(data, _currentFilter))
            {
                DisplayLogEntry(data);
            }

            // Auto-scroll to bottom
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        void DisplayLogEntry(LogEntryData data)
        {
            LogEntryView entry = GetFromPool();
            entry.SetData(data);
            entry.gameObject.SetActive(true);
            entry.transform.SetAsLastSibling();
            _activeEntries.Add(entry);

            // Trim displayed entries
            while (_activeEntries.Count > _poolSize)
            {
                LogEntryView oldest = _activeEntries[0];
                _activeEntries.RemoveAt(0);
                ReturnToPool(oldest);
            }
        }

        public void SetFilter(LogLevel level)
        {
            _currentFilter = level;
            RefreshDisplay();
        }

        void RefreshDisplay()
        {
            // Return all to pool
            foreach (LogEntryView entry in _activeEntries)
            {
                ReturnToPool(entry);
            }
            _activeEntries.Clear();

            // Re-display filtered logs
            int startIndex = Mathf.Max(0, _allLogs.Count - _poolSize);
            for (int i = startIndex; i < _allLogs.Count; i++)
            {
                if (PassesFilter(_allLogs[i], _currentFilter))
                {
                    DisplayLogEntry(_allLogs[i]);
                }
            }
        }

        bool PassesFilter(LogEntryData data, LogLevel filter)
        {
            if (filter == LogLevel.All) return true;
            return data.level == filter;
        }

        public void ClearLogs()
        {
            _allLogs.Clear();

            foreach (LogEntryView entry in _activeEntries)
            {
                ReturnToPool(entry);
            }
            _activeEntries.Clear();
        }

        // Public method to add log from external code
        public void Log(string message, LogSource source, LogLevel level = LogLevel.Info)
        {
            Color accentColor = source switch
            {
                LogSource.GA => Theme.accentYellow,
                LogSource.Game => Theme.accentGreen,
                LogSource.Firebase => Theme.accentOrange,
                LogSource.Sorolla => Theme.textSecondary,
                _ => Theme.textPrimary,
            };

            var data = new LogEntryData
            {
                timestamp = DateTime.Now.ToString("HH:mm:ss. ff"),
                source = source,
                level = level,
                message = message,
                accentColor = accentColor,
            };

            SorollaDebugEvents.RaiseLogAdded(data);
        }
    }
}
