using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.Samples
{
    /// <summary>
    ///     Beautiful, dynamic test UI for Sorolla SDK Prototype mode.
    ///     Automatically detects installed SDKs and shows relevant test buttons.
    /// </summary>
    public class PrototypeTestUI : MonoBehaviour
    {

        #region Button Wiring

        void WireButtons()
        {
            // Main
            toggleBtn.onClick.AddListener(TogglePanel);

            // Status
            refreshStatusBtn.onClick.AddListener(RefreshStatus);

            // Analytics
            trackDesignBtn.onClick.AddListener(TestTrackDesign);
            trackDesignValueBtn.onClick.AddListener(TestTrackDesignWithValue);
            progressionStartBtn.onClick.AddListener(TestProgressionStart);
            progressionCompleteBtn.onClick.AddListener(TestProgressionComplete);
            progressionFailBtn.onClick.AddListener(TestProgressionFail);
            resourceSourceBtn.onClick.AddListener(TestResourceSource);
            resourceSinkBtn.onClick.AddListener(TestResourceSink);

            // Remote Config
            fetchRemoteConfigBtn.onClick.AddListener(TestFetchRemoteConfig);
            remoteConfigReadyBtn.onClick.AddListener(TestRemoteConfigReady);
            getStringConfigBtn.onClick.AddListener(TestGetStringConfig);
            getIntConfigBtn.onClick.AddListener(TestGetIntConfig);
            getBoolConfigBtn.onClick.AddListener(TestGetBoolConfig);
            getFloatConfigBtn.onClick.AddListener(TestGetFloatConfig);

            // Crashlytics
            logMessageBtn.onClick.AddListener(TestLogMessage);
            setCustomKeyBtn.onClick.AddListener(TestSetCustomKey);
            logExceptionBtn.onClick.AddListener(TestLogException);
            forceCrashBtn.onClick.AddListener(TestForceCrash);

            // Log
            clearLogBtn.onClick.AddListener(ClearLog);
        }

        #endregion

        #region Serialized Fields

        [Header("Main Panel")]
        [SerializeField] GameObject mainPanel;
        [SerializeField] Button toggleBtn;

        [Header("Header Status")]
        [SerializeField] TextMeshProUGUI sdkStatusText;
        [SerializeField] TextMeshProUGUI modeText;
        [SerializeField] TextMeshProUGUI versionText;

        [Header("Sections")]
        [SerializeField] GameObject analyticsSection;
        [SerializeField] GameObject remoteConfigSection;
        [SerializeField] GameObject crashlyticsSection;

        [Header("Status Buttons")]
        [SerializeField] Button refreshStatusBtn;

        [Header("Analytics Buttons")]
        [SerializeField] Button trackDesignBtn;
        [SerializeField] Button trackDesignValueBtn;
        [SerializeField] Button progressionStartBtn;
        [SerializeField] Button progressionCompleteBtn;
        [SerializeField] Button progressionFailBtn;
        [SerializeField] Button resourceSourceBtn;
        [SerializeField] Button resourceSinkBtn;

        [Header("Remote Config Buttons")]
        [SerializeField] Button fetchRemoteConfigBtn;
        [SerializeField] Button remoteConfigReadyBtn;
        [SerializeField] Button getStringConfigBtn;
        [SerializeField] Button getIntConfigBtn;
        [SerializeField] Button getBoolConfigBtn;
        [SerializeField] Button getFloatConfigBtn;

        [Header("Crashlytics Buttons")]
        [SerializeField] Button logMessageBtn;
        [SerializeField] Button setCustomKeyBtn;
        [SerializeField] Button logExceptionBtn;
        [SerializeField] Button forceCrashBtn;

        [Header("Log")]
        [SerializeField] TextMeshProUGUI logText;
        [SerializeField] ScrollRect logScrollRect;
        [SerializeField] Button clearLogBtn;

        #endregion

        #region Private Fields

        readonly List<string> _logLines = new List<string>();
        const int MaxLogLines = 50;

        // SDK Detection Cache
        bool _hasGameAnalytics;
        bool _hasFacebook;
        bool _hasFirebaseAnalytics;
        bool _hasFirebaseCrashlytics;
        bool _hasFirebaseRemoteConfig;

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            DetectInstalledSdks();
            SetupUI();
            WireButtons();
        }

        void Start()
        {
            RefreshStatus();
            Log("🎮 Sorolla Prototype Test UI Ready");
            Log($"   Mode: {(IsPrototypeMode() ? "Prototype" : "Full")}");
            LogInstalledSdks();
        }

        #endregion

        #region SDK Detection

        void DetectInstalledSdks()
        {
            // GameAnalytics - always expected in Prototype mode
#if GAMEANALYTICS_INSTALLED
            _hasGameAnalytics = true;
#endif

            // Facebook - expected in Prototype mode
#if SOROLLA_FACEBOOK_ENABLED
            _hasFacebook = true;
#endif

            // Firebase (optional)
#if FIREBASE_ANALYTICS_INSTALLED
            _hasFirebaseAnalytics = true;
#endif

#if FIREBASE_CRASHLYTICS_INSTALLED
            _hasFirebaseCrashlytics = true;
#endif

#if FIREBASE_REMOTE_CONFIG_INSTALLED
            _hasFirebaseRemoteConfig = true;
#endif
        }

        void LogInstalledSdks()
        {
            Log("📦 Detected SDKs:");
            Log($" • GameAnalytics: {(_hasGameAnalytics ? "✅" : "❌")}");
            Log($" • Facebook: {(_hasFacebook ? "✅" : "❌")}");
            Log($" • Firebase Analytics: {(_hasFirebaseAnalytics ? "✅" : "⬜")}");
            Log($" • Firebase Crashlytics: {(_hasFirebaseCrashlytics ? "✅" : "⬜")}");
            Log($" • Firebase Remote Config: {(_hasFirebaseRemoteConfig ? "✅" : "⬜")}");
        }

        bool IsPrototypeMode() => Sorolla.Config == null || Sorolla.Config.isPrototypeMode;

        #endregion

        #region UI Setup

        void SetupUI()
        {
            // Show/hide sections based on installed SDKs
            analyticsSection.SetActive(_hasGameAnalytics);
            crashlyticsSection.SetActive(_hasFirebaseCrashlytics);
            remoteConfigSection.SetActive(_hasGameAnalytics || _hasFirebaseRemoteConfig);
        }

        public void RefreshStatus()
        {
            bool initialized = Sorolla.IsInitialized;
            sdkStatusText.text = initialized ? "Initialized" : "Not Ready";
            ColorUtility.TryParseHtmlString("34D399", out Color successColor);
            ColorUtility.TryParseHtmlString("FBC024", out Color errorColor);
            sdkStatusText.color = initialized ? successColor : errorColor;

            modeText.text = IsPrototypeMode() ? "PROTOTYPE MODE" : "FULL MODE";
            versionText.text = "v2.1.0"; //Todo: get it dynamically
        }

        void TogglePanel()
        {
            mainPanel.SetActive(!mainPanel.activeSelf);
            Log(mainPanel.activeSelf ? "Panel shown" : "Panel hidden");
        }

        #endregion

        #region Analytics Tests

        public void TestTrackDesign()
        {
            Sorolla.TrackDesign("test_button_click");
            Log("📊 TrackDesign('test_button_click')");
        }

        public void TestTrackDesignWithValue()
        {
            Sorolla.TrackDesign("test_score", 42.5f);
            Log("📊 TrackDesign('test_score', 42.5)");
        }

        public void TestProgressionStart()
        {
            Sorolla.TrackProgression(ProgressionStatus.Start, "World_01", "Level_01");
            Log("🎯 TrackProgression(Start, 'World_01', 'Level_01')");
        }

        public void TestProgressionComplete()
        {
            Sorolla.TrackProgression(ProgressionStatus.Complete, "World_01", "Level_01", score: 1000);
            Log("🎯 TrackProgression(Complete, 'World_01', 'Level_01', score: 1000)");
        }

        public void TestProgressionFail()
        {
            Sorolla.TrackProgression(ProgressionStatus.Fail, "World_01", "Level_01");
            Log("🎯 TrackProgression(Fail, 'World_01', 'Level_01')");
        }

        public void TestResourceSource()
        {
            Sorolla.TrackResource(ResourceFlowType.Source, "coins", 100, "reward", "level_complete");
            Log("💰 TrackResource(Source, 'coins', 100, 'reward', 'level_complete')");
        }

        public void TestResourceSink()
        {
            Sorolla.TrackResource(ResourceFlowType.Sink, "coins", 50, "shop", "power_up");
            Log("💸 TrackResource(Sink, 'coins', 50, 'shop', 'power_up')");
        }

        #endregion

        #region Remote Config Tests

        public void TestFetchRemoteConfig()
        {
            Log("⚙️ Fetching Remote Config...");
            Sorolla.FetchRemoteConfig(success =>
            {
                Log(success ? "⚙️ Remote Config: ✅ Fetched!" : "⚙️ Remote Config: ❌ Failed");
                RefreshStatus();
            });
        }

        public void TestGetStringConfig()
        {
            string value = Sorolla.GetRemoteConfig("welcome_message", "default");
            Log($"⚙️ GetRemoteConfig('welcome_message') = \"{value}\"");
        }

        public void TestGetIntConfig()
        {
            int value = Sorolla.GetRemoteConfigInt("daily_reward", 100);
            Log($"⚙️ GetRemoteConfigInt('daily_reward') = {value}");
        }

        public void TestGetBoolConfig()
        {
            bool value = Sorolla.GetRemoteConfigBool("feature_enabled");
            Log($"⚙️ GetRemoteConfigBool('feature_enabled') = {value}");
        }

        public void TestGetFloatConfig()
        {
            float value = Sorolla.GetRemoteConfigFloat("difficulty_multiplier", 1.0f);
            Log($"⚙️ GetRemoteConfigFloat('difficulty_multiplier') = {value}");
        }

        public void TestRemoteConfigReady()
        {
            bool ready = Sorolla.IsRemoteConfigReady();
            Log($"⚙️ IsRemoteConfigReady() = {ready}");
        }

        #endregion

        #region Crashlytics Tests

        public void TestLogMessage()
        {
            Sorolla.LogCrashlytics("Test log from PrototypeTestUI");
            Log("🔥 LogCrashlytics('Test log from PrototypeTestUI')");
        }

        public void TestSetCustomKey()
        {
            string timestamp = DateTime.Now.Ticks.ToString();
            Sorolla.SetCrashlyticsKey("test_key", timestamp);
            Log($"🔥 SetCrashlyticsKey('test_key', '{timestamp}')");
        }

        public void TestLogException()
        {
            try
            {
                throw new InvalidOperationException("Test exception from PrototypeTestUI");
            }
            catch (Exception ex)
            {
                Sorolla.LogException(ex);
                Log($"🔥 LogException({ex.GetType().Name})");
            }
        }

        public void TestForceCrash()
        {
            Log("⚠️ Force crash in 2 seconds...");
            Invoke(nameof(DoCrash), 2f);
        }

        void DoCrash()
        {
            Debug.LogError("[PrototypeTestUI] Forcing crash!");
            throw new Exception("Forced crash from PrototypeTestUI");
        }

        #endregion

        #region Logging

        void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string line = $"<color=#888>[{timestamp}]</color>  {message}";

            // Add at end (newest at bottom, like Unity Console)
            _logLines.Add(line);
            if (_logLines.Count > MaxLogLines)
                _logLines.RemoveAt(0);

            logText.text = string.Join("\n", _logLines);

            // Auto-scroll to bottom (must wait a frame for layout to update)
            StartCoroutine(ScrollToBottomNextFrame());

            Debug.Log($"[PrototypeTest] {message}");
        }

        void ClearLog()
        {
            _logLines.Clear();
            logText.text = "";
            Log("Log cleared");
        }

        IEnumerator ScrollToBottomNextFrame()
        {
            // Wait for end of frame so layout can recalculate
            yield return new WaitForEndOfFrame();

            // Force rebuild layout before scrolling
            Canvas.ForceUpdateCanvases();

            // Scroll to bottom (0 = bottom, 1 = top)
            logScrollRect.verticalNormalizedPosition = 0f;
        }

        #endregion
    }
}
