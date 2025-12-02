using System;
using UnityEngine;
using Sorolla;

/// <summary>
/// Test script for Firebase integration via Sorolla SDK.
/// Attach to a GameObject in your scene to test Firebase functionality.
/// </summary>
public class FirebaseTestUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private bool showUI = true;
    [SerializeField] private int requiredTaps = 3;
    [SerializeField] private float tapTimeWindow = 0.5f;
    
    [Header("Remote Config Test Keys")]
    [SerializeField] private string testStringKey = "welcome_message";
    [SerializeField] private string testIntKey = "daily_reward";
    [SerializeField] private string testBoolKey = "feature_enabled";
    [SerializeField] private string testFloatKey = "difficulty_multiplier";

    private Vector2 _scrollPos;
    private string _logOutput = "";
    private readonly int _maxLogLines = 50;
    
    // Multi-tap detection
    private int _tapCount;
    private float _lastTapTime;
    private Rect _toggleButtonRect;

    private void Start()
    {
        Log("FirebaseTestUI initialized. Triple-tap top-right corner to toggle.");
        Log($"Sorolla.IsInitialized: {Sorolla.Sorolla.IsInitialized}");
        
        // Toggle button in top-right corner
        _toggleButtonRect = new Rect(Screen.width - 100, 0, 100, 100);
    }

    private void Update()
    {
        // Update toggle button position on screen resize
        _toggleButtonRect = new Rect(Screen.width - 100, 0, 100, 100);
        
        // Handle touch input for mobile
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTap(touch.position);
            }
        }
        // Handle mouse click for editor testing
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        // Convert to GUI coordinates (Y is inverted)
        var guiPosition = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
        
        // Check if tap is in toggle button area
        if (!_toggleButtonRect.Contains(guiPosition))
        {
            _tapCount = 0;
            return;
        }

        var currentTime = Time.unscaledTime;
        
        if (currentTime - _lastTapTime > tapTimeWindow)
        {
            _tapCount = 0;
        }
        
        _tapCount++;
        _lastTapTime = currentTime;
        
        if (_tapCount >= requiredTaps)
        {
            showUI = !showUI;
            _tapCount = 0;
            Log($"Panel {(showUI ? "shown" : "hidden")}");
        }
    }

    private void OnGUI()
    {
        // Always draw the toggle button hint
        DrawToggleButton();
        
        if (!showUI) return;

        // Scale UI for mobile screens
        var scale = Mathf.Min(Screen.width / 450f, Screen.height / 800f);
        scale = Mathf.Clamp(scale, 1f, 3f);
        
        var windowWidth = Mathf.Min(420 * scale, Screen.width - 20);
        var windowHeight = Mathf.Min(650 * scale, Screen.height - 20);
        var windowRect = new Rect(10, 10, windowWidth, windowHeight);
        
        // Scale GUI matrix for touch-friendly buttons
        var originalMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));
        
        windowRect = new Rect(10 / scale, 10 / scale, windowWidth / scale, windowHeight / scale);
        GUI.Window(0, windowRect, DrawWindow, "Firebase Test Panel");
        
        GUI.matrix = originalMatrix;
    }

    private void DrawToggleButton()
    {
        // Draw a subtle indicator in top-right corner
        var style = new GUIStyle(GUI.skin.box);
        style.fontSize = 24;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = showUI ? Color.green : Color.gray;
        
        GUI.Box(_toggleButtonRect, "🔥", style);
        
        // Show tap hint when panel is hidden
        if (!showUI)
        {
            var hintStyle = new GUIStyle(GUI.skin.label);
            hintStyle.fontSize = 12;
            hintStyle.alignment = TextAnchor.MiddleCenter;
            hintStyle.normal.textColor = Color.gray;
            GUI.Label(new Rect(_toggleButtonRect.x - 50, _toggleButtonRect.yMax, 200, 20), "Triple-tap to open", hintStyle);
        }
    }

    private void DrawWindow(int windowId)
    {
        GUILayout.BeginVertical();

        // Status Section
        DrawStatusSection();
        GUILayout.Space(10);

        // Analytics Section
        DrawAnalyticsSection();
        GUILayout.Space(10);

        // Crashlytics Section
        DrawCrashlyticsSection();
        GUILayout.Space(10);

        // Remote Config Section
        DrawRemoteConfigSection();
        GUILayout.Space(10);

        // Log Output
        DrawLogSection();

        GUILayout.EndVertical();
    }

    private void DrawStatusSection()
    {
        GUILayout.Label("=== Status ===", GUI.skin.box);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Sorolla Initialized:", GUILayout.Width(150));
        GUILayout.Label(Sorolla.Sorolla.IsInitialized ? "✓ Yes" : "✗ No");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Remote Config Ready:", GUILayout.Width(150));
        GUILayout.Label(Sorolla.Sorolla.IsRemoteConfigReady() ? "✓ Yes" : "✗ No");
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh Status"))
        {
            Log($"--- Status Refresh ---");
            Log($"Sorolla.IsInitialized: {Sorolla.Sorolla.IsInitialized}");
            Log($"Remote Config Ready: {Sorolla.Sorolla.IsRemoteConfigReady()}");
        }
    }

    private void DrawAnalyticsSection()
    {
        GUILayout.Label("=== Analytics ===", GUI.skin.box);

        if (GUILayout.Button("Track Design Event: test_button_click"))
        {
            Sorolla.Sorolla.TrackDesign("test_button_click");
            Log("Sent: TrackDesign('test_button_click')");
        }

        if (GUILayout.Button("Track Design Event with Value"))
        {
            Sorolla.Sorolla.TrackDesign("test_score", 42.5f);
            Log("Sent: TrackDesign('test_score', 42.5)");
        }

        if (GUILayout.Button("Track Progression: Level Start"))
        {
            Sorolla.Sorolla.TrackProgression(ProgressionStatus.Start, "World_01", "Level_01");
            Log("Sent: TrackProgression(Start, 'World_01', 'Level_01')");
        }

        if (GUILayout.Button("Track Progression: Level Complete"))
        {
            Sorolla.Sorolla.TrackProgression(ProgressionStatus.Complete, "World_01", "Level_01", score: 1000);
            Log("Sent: TrackProgression(Complete, 'World_01', 'Level_01', score: 1000)");
        }

        if (GUILayout.Button("Track Resource: Coins Earned"))
        {
            Sorolla.Sorolla.TrackResource(ResourceFlowType.Source, "coins", 100, "reward", "level_complete");
            Log("Sent: TrackResource(Source, 'coins', 100, 'reward', 'level_complete')");
        }

        if (GUILayout.Button("Track Resource: Coins Spent"))
        {
            Sorolla.Sorolla.TrackResource(ResourceFlowType.Sink, "coins", 50, "shop", "power_up");
            Log("Sent: TrackResource(Sink, 'coins', 50, 'shop', 'power_up')");
        }
    }

    private void DrawCrashlyticsSection()
    {
        GUILayout.Label("=== Crashlytics ===", GUI.skin.box);

        if (GUILayout.Button("Log Message"))
        {
            Sorolla.Sorolla.LogCrashlytics("Test log message from FirebaseTestUI");
            Log("Sent: LogCrashlytics('Test log message')");
        }

        if (GUILayout.Button("Set Custom Key"))
        {
            Sorolla.Sorolla.SetCrashlyticsKey("test_key", "test_value");
            Log("Sent: SetCrashlyticsKey('test_key', 'test_value')");
        }

        if (GUILayout.Button("Log Non-Fatal Exception"))
        {
            try
            {
                throw new InvalidOperationException("Test exception from FirebaseTestUI");
            }
            catch (Exception ex)
            {
                Sorolla.Sorolla.LogException(ex);
                Log($"Sent: LogException({ex.GetType().Name})");
            }
        }

        GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("⚠️ Force Crash (CAUTION!)"))
        {
            Log("Forcing crash in 2 seconds...");
            Invoke(nameof(ForceCrash), 2f);
        }
        GUI.backgroundColor = Color.white;
    }

    private void ForceCrash()
    {
        // This will cause an actual crash for testing Crashlytics
        Debug.LogError("Forcing crash for Crashlytics test!");
        throw new Exception("Forced crash from FirebaseTestUI for Crashlytics testing");
    }

    private void DrawRemoteConfigSection()
    {
        GUILayout.Label("=== Remote Config ===", GUI.skin.box);

        if (GUILayout.Button("Fetch Remote Config"))
        {
            Log("Fetching Remote Config...");
            Sorolla.Sorolla.FetchRemoteConfig(success =>
            {
                Log($"Fetch result: {(success ? "SUCCESS" : "FAILED")}");
            });
        }

        GUILayout.Space(5);
        GUILayout.Label("Test Values:", GUI.skin.label);

        if (GUILayout.Button($"Get String: '{testStringKey}'"))
        {
            var value = Sorolla.Sorolla.GetRemoteConfig(testStringKey, "default_message");
            Log($"'{testStringKey}' = \"{value}\"");
        }

        if (GUILayout.Button($"Get Int: '{testIntKey}'"))
        {
            var value = Sorolla.Sorolla.GetRemoteConfigInt(testIntKey, 0);
            Log($"'{testIntKey}' = {value}");
        }

        if (GUILayout.Button($"Get Bool: '{testBoolKey}'"))
        {
            var value = Sorolla.Sorolla.GetRemoteConfigBool(testBoolKey, false);
            Log($"'{testBoolKey}' = {value}");
        }

        if (GUILayout.Button($"Get Float: '{testFloatKey}'"))
        {
            var value = Sorolla.Sorolla.GetRemoteConfigFloat(testFloatKey, 1.0f);
            Log($"'{testFloatKey}' = {value}");
        }
    }

    private void DrawLogSection()
    {
        GUILayout.Label("=== Log ===", GUI.skin.box);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(35)))
            _logOutput = "";
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Hide Panel", GUILayout.Height(35)))
            showUI = false;
        GUILayout.EndHorizontal();

        _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(150));
        GUILayout.TextArea(_logOutput, GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();
    }

    private void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logLine = $"[{timestamp}] {message}";
        
        Debug.Log($"[FirebaseTest] {message}");
        
        _logOutput = logLine + "\n" + _logOutput;
        
        // Trim old lines
        var lines = _logOutput.Split('\n');
        if (lines.Length > _maxLogLines)
        {
            _logOutput = string.Join("\n", lines, 0, _maxLogLines);
        }
    }
}
