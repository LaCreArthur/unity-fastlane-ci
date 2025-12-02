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
    [Header("Remote Config Test Keys")]
    public string testStringKey = "welcome_message";

    public string testIntKey = "daily_reward";
    public string testBoolKey = "feature_enabled";
    public string testFloatKey = "difficulty_multiplier";

    readonly int _maxLogLines = 100;
    TextMeshProUGUI _firebaseRCStatus;
    TextMeshProUGUI _gaRCStatus;
    string _logOutput = "";
    ScrollRect _logScrollRect;
    TextMeshProUGUI _logText;

    GameObject _panel;

    TextMeshProUGUI _sorollaStatus;
    GameObject _toggleButton;

    void Awake()
    {
        _panel = gameObject;
        CacheReferences();
        BindButtons();
    }

    void Start()
    {
        Log("Firebase Test UI initialized");
        RefreshStatus();
    }

    void CacheReferences()
    {
        // Find toggle button (sibling)
        _toggleButton = transform.parent?.Find("TogglePanelBtn")?.gameObject;
        Debug.Log($"[FirebaseTestUI] Toggle button found: {_toggleButton != null}");

        // Find status text elements
        Transform statusSection = transform.Find("ScrollView/Viewport/Content/StatusSection/Content");
        Debug.Log($"[FirebaseTestUI] Status section found: {statusSection != null}");

        if (statusSection != null)
        {
            _sorollaStatus = statusSection.Find("SorollaStatus/Value")?.GetComponent<TextMeshProUGUI>();
            _firebaseRCStatus = statusSection.Find("FirebaseRCStatus/Value")?.GetComponent<TextMeshProUGUI>();
            _gaRCStatus = statusSection.Find("GARCStatus/Value")?.GetComponent<TextMeshProUGUI>();
            Debug.Log($"[FirebaseTestUI] Status refs - Sorolla: {_sorollaStatus != null}, FirebaseRC: {_firebaseRCStatus != null}, GARC: {_gaRCStatus != null}");
        }

        // Find log elements
        Transform logSection = transform.Find("ScrollView/Viewport/Content/LogSection/Content");
        Debug.Log($"[FirebaseTestUI] Log section found: {logSection != null}");

        if (logSection != null)
        {
            _logText = logSection.Find("LogArea/LogViewport/LogContent/LogText")?.GetComponent<TextMeshProUGUI>();
            _logScrollRect = logSection.Find("LogArea")?.GetComponent<ScrollRect>();
            Debug.Log($"[FirebaseTestUI] Log refs - LogText: {_logText != null}, ScrollRect: {_logScrollRect != null}");
        }

        // If references not found, try alternate paths (debug)
        if (_logText == null)
        {
            Debug.LogWarning("[FirebaseTestUI] LogText not found at expected path. Searching children...");
            var allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI t in allTexts)
            {
                if (t.name == "LogText")
                {
                    _logText = t;
                    Debug.Log($"[FirebaseTestUI] Found LogText via search at: {GetPath(t.transform)}");
                    break;
                }
            }
        }
    }

    string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null && t.parent != transform)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    void BindButtons()
    {
        // Toggle button
        BindButton("TogglePanelBtn", TogglePanel, transform.parent);

        Transform content = transform.Find("ScrollView/Viewport/Content");
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

    void BindButton(string path, Action action, Transform root)
    {
        var btn = root.Find(path)?.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => action());
    }

    #region Toggle

    void TogglePanel() => _panel.SetActive(!_panel.activeSelf);

    #endregion

    #region Status

    void RefreshStatus()
    {
        Log("--- Status Refresh ---");

        bool sorollaInit = Sorolla.Sorolla.IsInitialized;
        bool rcReady = Sorolla.Sorolla.IsRemoteConfigReady();

        Log($"Sorolla.IsInitialized: {sorollaInit}");
        Log($"Remote Config Ready: {rcReady}");

        if (_sorollaStatus != null)
        {
            _sorollaStatus.text = sorollaInit ? "🟢 Yes" : "🟡 No";
            _sorollaStatus.color = sorollaInit ? Color.green : Color.red;
        }

        if (_firebaseRCStatus != null)
        {
            _firebaseRCStatus.text = rcReady ? "🟢 Yes" : "🟡 No";
            _firebaseRCStatus.color = rcReady ? Color.green : Color.yellow;
        }

        if (_gaRCStatus != null)
        {
            _gaRCStatus.text = rcReady ? "🟢 Yes" : "🟡 No";
            _gaRCStatus.color = rcReady ? Color.green : Color.yellow;
        }
    }

    #endregion

    #region Analytics

    void TrackDesign()
    {
        Sorolla.Sorolla.TrackDesign("test_button_click");
        Log("Sent: TrackDesign('test_button_click')");
    }

    void TrackDesignWithValue()
    {
        Sorolla.Sorolla.TrackDesign("test_score", 42.5f);
        Log("Sent: TrackDesign('test_score', 42.5)");
    }

    void TrackProgressStart()
    {
        Sorolla.Sorolla.TrackProgression(ProgressionStatus.Start, "World_01", "Level_01");
        Log("Sent: TrackProgression(Start, 'World_01', 'Level_01')");
    }

    void TrackProgressComplete()
    {
        Sorolla.Sorolla.TrackProgression(ProgressionStatus.Complete, "World_01", "Level_01", score: 1000);
        Log("Sent: TrackProgression(Complete, score: 1000)");
    }

    void TrackResourceSource()
    {
        Sorolla.Sorolla.TrackResource(ResourceFlowType.Source, "coins", 100, "reward", "level_complete");
        Log("Sent: TrackResource(Source, coins, 100)");
    }

    void TrackResourceSink()
    {
        Sorolla.Sorolla.TrackResource(ResourceFlowType.Sink, "coins", 50, "shop", "power_up");
        Log("Sent: TrackResource(Sink, coins, 50)");
    }

    #endregion

    #region Crashlytics

    void LogMessage()
    {
        Sorolla.Sorolla.LogCrashlytics("Test log message from Firebase Test UI");
        Log("Sent: LogCrashlytics('Test log message')");
    }

    void SetCustomKey()
    {
        Sorolla.Sorolla.SetCrashlyticsKey("test_key", "test_value_" + DateTime.Now.Ticks);
        Log("Sent: SetCrashlyticsKey('test_key', 'test_value_...')");
    }

    void LogNonFatalException()
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

    void ForceCrash()
    {
        Log("⚠️ Forcing crash in 2 seconds...");
        Invoke(nameof(DoCrash), 2f);
    }

    void DoCrash()
    {
        Debug.LogError("Forcing crash for Crashlytics test!");
        throw new Exception("Forced crash from Firebase Test UI");
    }

    #endregion

    #region Remote Config

    void FetchRemoteConfig()
    {
        Log("Fetching Remote Config...");
        Sorolla.Sorolla.FetchRemoteConfig(success =>
        {
            Log($"Fetch result: {(success ? "✓ SUCCESS" : "✗ FAILED")}");
            RefreshStatus();
        });
    }

    void GetStringValue()
    {
        string value = Sorolla.Sorolla.GetRemoteConfig(testStringKey, "default_message");
        Log($"'{testStringKey}' = \"{value}\"");
    }

    void GetIntValue()
    {
        int value = Sorolla.Sorolla.GetRemoteConfigInt(testIntKey);
        Log($"'{testIntKey}' = {value}");
    }

    void GetBoolValue()
    {
        bool value = Sorolla.Sorolla.GetRemoteConfigBool(testBoolKey);
        Log($"'{testBoolKey}' = {value}");
    }

    void GetFloatValue()
    {
        float value = Sorolla.Sorolla.GetRemoteConfigFloat(testFloatKey, 1.0f);
        Log($"'{testFloatKey}' = {value}");
    }

    #endregion

    #region Log

    void ClearLog()
    {
        _logOutput = "";
        if (_logText != null)
            _logText.text = "";
        Log("Log cleared");
    }

    void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string logLine = $"<color=#888>[{timestamp}]</color> {message}";

        Debug.Log($"[FirebaseTest] {message}");

        _logOutput = logLine + "\n" + _logOutput;

        // Trim old lines
        string[] lines = _logOutput.Split('\n');
        if (lines.Length > _maxLogLines) _logOutput = string.Join("\n", lines, 0, _maxLogLines);

        if (_logText != null)
            _logText.text = _logOutput;
    }

    #endregion
}
