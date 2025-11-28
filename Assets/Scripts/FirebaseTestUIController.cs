using System;
using Sorolla;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Runtime controller for the Firebase Test UI panel.
///     Handles button clicks and updates status displays.
/// </summary>
public class FirebaseTestUIController : MonoBehaviour
{
    [Header("Remote Config Test Keys")] public string testStringKey = "welcome_message";

    public string testIntKey = "daily_reward";
    public string testBoolKey = "feature_enabled";
    public string testFloatKey = "difficulty_multiplier";

    private readonly int _maxLogLines = 100;
    private TextMeshProUGUI _firebaseRCStatus;
    private TextMeshProUGUI _gaRCStatus;
    private string _logOutput = "";
    private ScrollRect _logScrollRect;
    private TextMeshProUGUI _logText;

    private GameObject _panel;

    private TextMeshProUGUI _sorollaStatus;
    private GameObject _toggleButton;

    private void Awake()
    {
        _panel = gameObject;
        CacheReferences();
        BindButtons();
    }

    private void Start()
    {
        Log("Firebase Test UI initialized");
        RefreshStatus();
    }

    private void CacheReferences()
    {
        // Find toggle button (sibling)
        _toggleButton = transform.parent.Find("TogglePanelBtn")?.gameObject;

        // Find status text elements
        var statusSection = transform.Find("ScrollView/Viewport/Content/StatusSection/Content");
        if (statusSection != null)
        {
            _sorollaStatus = statusSection.Find("SorollaStatus/Value")?.GetComponent<TextMeshProUGUI>();
            _firebaseRCStatus = statusSection.Find("FirebaseRCStatus/Value")?.GetComponent<TextMeshProUGUI>();
            _gaRCStatus = statusSection.Find("GARCStatus/Value")?.GetComponent<TextMeshProUGUI>();
        }

        // Find log elements
        var logSection = transform.Find("ScrollView/Viewport/Content/LogSection/Content");
        if (logSection != null)
        {
            _logText = logSection.Find("LogArea/LogViewport/LogContent/LogText")?.GetComponent<TextMeshProUGUI>();
            _logScrollRect = logSection.Find("LogArea")?.GetComponent<ScrollRect>();
        }
    }

    private void BindButtons()
    {
        // Toggle button
        BindButton("TogglePanelBtn", TogglePanel, transform.parent);

        var content = transform.Find("ScrollView/Viewport/Content");
        if (content == null) return;

        // Status
        BindButton("StatusSection/Content/RefreshStatusBtn", RefreshStatus, content);

        // Analytics
        BindButton("AnalyticsSection/Content/TrackDesignBtn", TrackDesign, content);
        BindButton("AnalyticsSection/Content/TrackDesignValueBtn", TrackDesignWithValue, content);
        BindButton("AnalyticsSection/Content/TrackProgressStartBtn", TrackProgressStart, content);
        BindButton("AnalyticsSection/Content/TrackProgressCompleteBtn", TrackProgressComplete, content);
        BindButton("AnalyticsSection/Content/TrackResourceSourceBtn", TrackResourceSource, content);
        BindButton("AnalyticsSection/Content/TrackResourceSinkBtn", TrackResourceSink, content);

        // Crashlytics
        BindButton("CrashlyticsSection/Content/LogMessageBtn", LogMessage, content);
        BindButton("CrashlyticsSection/Content/SetCustomKeyBtn", SetCustomKey, content);
        BindButton("CrashlyticsSection/Content/LogExceptionBtn", LogNonFatalException, content);
        BindButton("CrashlyticsSection/Content/ForceCrashBtn", ForceCrash, content);

        // Remote Config
        BindButton("RemoteConfigSection/Content/FetchRCBtn", FetchRemoteConfig, content);
        BindButton("RemoteConfigSection/Content/GetStringBtn", GetStringValue, content);
        BindButton("RemoteConfigSection/Content/GetIntBtn", GetIntValue, content);
        BindButton("RemoteConfigSection/Content/GetBoolBtn", GetBoolValue, content);
        BindButton("RemoteConfigSection/Content/GetFloatBtn", GetFloatValue, content);

        // Log
        BindButton("LogSection/Content/ClearLogBtn", ClearLog, content);
    }

    private void BindButton(string path, Action action, Transform root)
    {
        var btn = root.Find(path)?.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => action());
    }

    #region Toggle

    private void TogglePanel()
    {
        _panel.SetActive(!_panel.activeSelf);
    }

    #endregion

    #region Status

    private void RefreshStatus()
    {
        Log("--- Status Refresh ---");

        var sorollaInit = Sorolla.Sorolla.IsInitialized;
        var firebaseRC = Sorolla.Sorolla.IsFirebaseRemoteConfigReady();
        var gaRC = Sorolla.Sorolla.IsRemoteConfigReady();

        Log($"Sorolla.IsInitialized: {sorollaInit}");
        Log($"Firebase RC Ready: {firebaseRC}");
        Log($"GA RC Ready: {gaRC}");

        if (_sorollaStatus != null)
        {
            _sorollaStatus.text = sorollaInit ? "✓ Yes" : "✗ No";
            _sorollaStatus.color = sorollaInit ? Color.green : Color.red;
        }

        if (_firebaseRCStatus != null)
        {
            _firebaseRCStatus.text = firebaseRC ? "✓ Yes" : "✗ No";
            _firebaseRCStatus.color = firebaseRC ? Color.green : Color.yellow;
        }

        if (_gaRCStatus != null)
        {
            _gaRCStatus.text = gaRC ? "✓ Yes" : "✗ No";
            _gaRCStatus.color = gaRC ? Color.green : Color.yellow;
        }
    }

    #endregion

    #region Analytics

    private void TrackDesign()
    {
        Sorolla.Sorolla.TrackDesign("test_button_click");
        Log("Sent: TrackDesign('test_button_click')");
    }

    private void TrackDesignWithValue()
    {
        Sorolla.Sorolla.TrackDesign("test_score", 42.5f);
        Log("Sent: TrackDesign('test_score', 42.5)");
    }

    private void TrackProgressStart()
    {
        Sorolla.Sorolla.TrackProgression(ProgressionStatus.Start, "World_01", "Level_01");
        Log("Sent: TrackProgression(Start, 'World_01', 'Level_01')");
    }

    private void TrackProgressComplete()
    {
        Sorolla.Sorolla.TrackProgression(ProgressionStatus.Complete, "World_01", "Level_01", score: 1000);
        Log("Sent: TrackProgression(Complete, score: 1000)");
    }

    private void TrackResourceSource()
    {
        Sorolla.Sorolla.TrackResource(ResourceFlowType.Source, "coins", 100, "reward", "level_complete");
        Log("Sent: TrackResource(Source, coins, 100)");
    }

    private void TrackResourceSink()
    {
        Sorolla.Sorolla.TrackResource(ResourceFlowType.Sink, "coins", 50, "shop", "power_up");
        Log("Sent: TrackResource(Sink, coins, 50)");
    }

    #endregion

    #region Crashlytics

    private void LogMessage()
    {
        Sorolla.Sorolla.LogCrashlytics("Test log message from Firebase Test UI");
        Log("Sent: LogCrashlytics('Test log message')");
    }

    private void SetCustomKey()
    {
        Sorolla.Sorolla.SetCrashlyticsKey("test_key", "test_value_" + DateTime.Now.Ticks);
        Log("Sent: SetCrashlyticsKey('test_key', 'test_value_...')");
    }

    private void LogNonFatalException()
    {
        try
        {
            throw new InvalidOperationException("Test exception from Firebase Test UI");
        }
        catch (Exception ex)
        {
            Sorolla.Sorolla.LogException(ex);
            Log($"Sent: LogException({ex.GetType().Name})");
        }
    }

    private void ForceCrash()
    {
        Log("⚠️ Forcing crash in 2 seconds...");
        Invoke(nameof(DoCrash), 2f);
    }

    private void DoCrash()
    {
        Debug.LogError("Forcing crash for Crashlytics test!");
        throw new Exception("Forced crash from Firebase Test UI");
    }

    #endregion

    #region Remote Config

    private void FetchRemoteConfig()
    {
        Log("Fetching Firebase Remote Config...");
        Sorolla.Sorolla.FetchFirebaseRemoteConfig(success =>
        {
            Log($"Fetch result: {(success ? "✓ SUCCESS" : "✗ FAILED")}");
            RefreshStatus();
        });
    }

    private void GetStringValue()
    {
        var value = Sorolla.Sorolla.GetFirebaseRemoteConfig(testStringKey, "default_message");
        Log($"'{testStringKey}' = \"{value}\"");
    }

    private void GetIntValue()
    {
        var value = Sorolla.Sorolla.GetFirebaseRemoteConfigInt(testIntKey);
        Log($"'{testIntKey}' = {value}");
    }

    private void GetBoolValue()
    {
        var value = Sorolla.Sorolla.GetFirebaseRemoteConfigBool(testBoolKey);
        Log($"'{testBoolKey}' = {value}");
    }

    private void GetFloatValue()
    {
        var value = Sorolla.Sorolla.GetFirebaseRemoteConfigFloat(testFloatKey, 1.0f);
        Log($"'{testFloatKey}' = {value}");
    }

    #endregion

    #region Log

    private void ClearLog()
    {
        _logOutput = "";
        if (_logText != null)
            _logText.text = "";
        Log("Log cleared");
    }

    private void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logLine = $"<color=#888>[{timestamp}]</color> {message}";

        Debug.Log($"[FirebaseTest] {message}");

        _logOutput = logLine + "\n" + _logOutput;

        // Trim old lines
        var lines = _logOutput.Split('\n');
        if (lines.Length > _maxLogLines) _logOutput = string.Join("\n", lines, 0, _maxLogLines);

        if (_logText != null)
            _logText.text = _logOutput;
    }

    #endregion
}