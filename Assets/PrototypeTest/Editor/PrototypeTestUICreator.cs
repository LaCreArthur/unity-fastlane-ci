using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sorolla.Samples.Editor
{
    /// <summary>
    ///     Editor utility to create the Prototype Test UI in the scene.
    ///     Creates a beautiful, modern UI with dynamic sections based on installed SDKs.
    /// </summary>
    public static class PrototypeTestUICreator
    {

        [MenuItem("Sorolla/Create Prototype Test UI", false, 100)]
        public static void CreatePrototypeTestUI()
        {
            Canvas canvas = FindOrCreateCanvas();

            // Main container
            GameObject root = CreatePanel("SorollaPrototypeTestUI", canvas.transform, BgDark);
            SetAnchors(root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Toggle Button (top-right corner)
            CreateToggleButton(root.transform);

            // Main Panel (scrollable content)
            GameObject mainPanel = CreateMainPanel(root.transform);

            // Header
            CreateHeader(mainPanel.transform);

            // Scrollable Content
            GameObject scrollView = CreateScrollView(mainPanel.transform);
            Transform content = scrollView.transform.Find("Viewport/Content");

            // Create all sections
            CreateStatusSection(content);
            CreateAnalyticsSection(content);
            CreateRemoteConfigSection(content);
            CreateFacebookSection(content);
            CreateCrashlyticsSection(content);
            CreateLogSection(content);

            // Add controller - it self-wires!
            root.AddComponent<PrototypeTestUI>();

            Selection.activeGameObject = root;
            Undo.RegisterCreatedObjectUndo(root, "Create Prototype Test UI");

            Debug.Log("[Sorolla] Prototype Test UI created! ✨");
        }

        #region Canvas Setup

        static Canvas FindOrCreateCanvas()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null) return canvas;

            var go = new GameObject("Canvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var eventGo = new GameObject("EventSystem");
                eventGo.AddComponent<EventSystem>();
                eventGo.AddComponent<StandaloneInputModule>();
            }

            Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
            return canvas;
        }

        #endregion
        #region Color Palette - Modern Dark Theme

        static readonly Color BgDark = new Color(0.11f, 0.11f, 0.14f);
        static readonly Color BgPanel = new Color(0.16f, 0.16f, 0.20f);
        static readonly Color BgSection = new Color(0.20f, 0.20f, 0.24f);
        static readonly Color BgInput = new Color(0.12f, 0.12f, 0.15f);

        static readonly Color AccentPrimary = new Color(0.36f, 0.52f, 0.95f); // Blue
        static readonly Color AccentSuccess = new Color(0.30f, 0.78f, 0.45f); // Green
        static readonly Color AccentWarning = new Color(0.95f, 0.65f, 0.25f); // Orange
        static readonly Color AccentDanger = new Color(0.90f, 0.35f, 0.35f); // Red
        static readonly Color AccentPurple = new Color(0.65f, 0.45f, 0.90f); // Purple
        static readonly Color AccentFacebook = new Color(0.26f, 0.40f, 0.70f); // FB Blue

        static readonly Color TextPrimary = new Color(0.95f, 0.95f, 0.97f);
        static readonly Color TextSecondary = new Color(0.65f, 0.65f, 0.70f);

        #endregion

        #region Main Structure

        static GameObject CreateToggleButton(Transform parent)
        {
            GameObject btn = CreateButton("ToggleBtn", parent, "🧪", AccentPrimary);
            var rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -50);
            rect.sizeDelta = new Vector2(80, 80);

            // Round button style
            var img = btn.GetComponent<Image>();
            img.color = AccentPrimary;

            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            text.fontSize = 36;

            // Remove layout element if present
            var le = btn.GetComponent<LayoutElement>();
            if (le != null) Object.DestroyImmediate(le);

            return btn;
        }

        static GameObject CreateMainPanel(Transform parent)
        {
            GameObject panel = CreatePanel("MainPanel", parent, BgPanel);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.03f, 0.03f);
            rect.anchorMax = new Vector2(0.97f, 0.97f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Rounded corners effect (simulated with padding)
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return panel;
        }

        static GameObject CreateHeader(Transform parent)
        {
            GameObject header = CreatePanel("Header", parent, BgSection);
            header.AddComponent<LayoutElement>().preferredHeight = 140;

            var layout = header.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(25, 25, 20, 15);
            layout.spacing = 8;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;

            // Title row
            GameObject titleRow = CreatePanel("TitleRow", header.transform, Color.clear);
            titleRow.AddComponent<LayoutElement>().preferredHeight = 50;
            var titleLayout = titleRow.AddComponent<HorizontalLayoutGroup>();
            titleLayout.childControlWidth = true;
            titleLayout.childControlHeight = true;
            titleLayout.childForceExpandWidth = false;

            TextMeshProUGUI icon = CreateText("🧪", titleRow.transform, 40);
            icon.gameObject.AddComponent<LayoutElement>().preferredWidth = 50;

            TextMeshProUGUI title = CreateText("Prototype Test", titleRow.transform, 32);
            title.fontStyle = FontStyles.Bold;
            title.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            // Status row
            GameObject statusRow = CreatePanel("StatusRow", header.transform, Color.clear);
            statusRow.AddComponent<LayoutElement>().preferredHeight = 28;
            var statusLayout = statusRow.AddComponent<HorizontalLayoutGroup>();
            statusLayout.spacing = 20;
            statusLayout.childControlWidth = true;
            statusLayout.childControlHeight = true;
            statusLayout.childForceExpandWidth = false;

            CreateStatusItem("StatusValue", "⏳ Loading...", statusRow.transform);
            CreateStatusItem("ModeValue", "🧪 Prototype", statusRow.transform);
            CreateStatusItem("VersionValue", "v2.1.0", statusRow.transform);

            return header;
        }

        static void CreateStatusItem(string name, string defaultText, Transform parent)
        {
            TextMeshProUGUI text = CreateText(defaultText, parent, 18);
            text.gameObject.name = name;
            text.color = TextSecondary;
            text.gameObject.AddComponent<LayoutElement>();
        }

        static GameObject CreateScrollView(Transform parent)
        {
            var scrollGo = new GameObject("ScrollView");
            scrollGo.transform.SetParent(parent, false);

            var scrollRect = scrollGo.AddComponent<RectTransform>();
            var scroll = scrollGo.AddComponent<ScrollRect>();
            scrollGo.AddComponent<Image>().color = Color.clear;
            scrollGo.AddComponent<LayoutElement>().flexibleHeight = 1;

            // Viewport
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollGo.transform, false);
            var vpRect = viewport.AddComponent<RectTransform>();
            SetAnchors(viewport, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            viewport.AddComponent<Image>().color = Color.clear;
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            // Content
            var content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            var contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(20, 20, 15, 20);
            contentLayout.spacing = 20;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = vpRect;
            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.scrollSensitivity = 40;

            return scrollGo;
        }

        #endregion

        #region Sections

        static void CreateStatusSection(Transform parent)
        {
            GameObject section = CreateSection("StatusSection", "📊 SDK Status", parent, AccentPrimary);
            Transform content = section.transform.Find("Content");

            GameObject refreshBtn = CreateButton("RefreshBtn", content, "🔄 Refresh Status", AccentPrimary);
            WireButton(refreshBtn, "RefreshStatus");
        }

        static GameObject CreateAnalyticsSection(Transform parent)
        {
            GameObject section = CreateSection("AnalyticsSection", "📈 Analytics", parent, AccentSuccess);
            Transform content = section.transform.Find("Content");

            // Subsection: Design Events
            CreateSubsectionHeader("Design Events", content);
            WireButton(CreateButton("DesignBtn", content, "Track Design Event", AccentPrimary), "TestTrackDesign");
            WireButton(CreateButton("DesignValueBtn", content, "Track Design + Value", AccentPrimary), "TestTrackDesignWithValue");

            // Subsection: Progression
            CreateSubsectionHeader("Progression", content);
            GameObject progRow = CreateButtonRow(content);
            WireButton(CreateSmallButton("ProgStartBtn", progRow.transform, "▶️ Start", AccentPrimary), "TestProgressionStart");
            WireButton(CreateSmallButton("ProgCompleteBtn", progRow.transform, "✅ Complete", AccentSuccess), "TestProgressionComplete");
            WireButton(CreateSmallButton("ProgFailBtn", progRow.transform, "❌ Fail", AccentDanger), "TestProgressionFail");

            // Subsection: Resources
            CreateSubsectionHeader("Economy", content);
            GameObject resRow = CreateButtonRow(content);
            WireButton(CreateSmallButton("ResSourceBtn", resRow.transform, "💰 Earned", AccentSuccess), "TestResourceSource");
            WireButton(CreateSmallButton("ResSinkBtn", resRow.transform, "💸 Spent", AccentWarning), "TestResourceSink");

            return section;
        }

        static GameObject CreateRemoteConfigSection(Transform parent)
        {
            GameObject section = CreateSection("RemoteConfigSection", "⚙️ Remote Config", parent, AccentPurple);
            Transform content = section.transform.Find("Content");

            WireButton(CreateButton("FetchRCBtn", content, "🔄 Fetch & Activate", AccentPurple), "TestFetchRemoteConfig");
            WireButton(CreateButton("RCReadyBtn", content, "Check Ready Status", AccentPrimary), "TestRemoteConfigReady");

            CreateSubsectionHeader("Get Values", content);
            GameObject row1 = CreateButtonRow(content);
            WireButton(CreateSmallButton("GetStringBtn", row1.transform, "String", AccentPrimary), "TestGetStringConfig");
            WireButton(CreateSmallButton("GetIntBtn", row1.transform, "Int", AccentPrimary), "TestGetIntConfig");

            GameObject row2 = CreateButtonRow(content);
            WireButton(CreateSmallButton("GetBoolBtn", row2.transform, "Bool", AccentPrimary), "TestGetBoolConfig");
            WireButton(CreateSmallButton("GetFloatBtn", row2.transform, "Float", AccentPrimary), "TestGetFloatConfig");

            return section;
        }

        static GameObject CreateFacebookSection(Transform parent)
        {
            GameObject section = CreateSection("FacebookSection", "📘 Facebook", parent, AccentFacebook);
            Transform content = section.transform.Find("Content");

            WireButton(CreateButton("FBStatusBtn", content, "Check FB Status", AccentFacebook), "TestFacebookStatus");

            TextMeshProUGUI note = CreateText("Events sent via TrackDesign() in Prototype mode", content, 16);
            note.color = TextSecondary;
            note.fontStyle = FontStyles.Italic;
            note.gameObject.AddComponent<LayoutElement>().preferredHeight = 30;

            return section;
        }

        static GameObject CreateCrashlyticsSection(Transform parent)
        {
            GameObject section = CreateSection("CrashlyticsSection", "🔥 Crashlytics", parent, AccentWarning);
            Transform content = section.transform.Find("Content");

            WireButton(CreateButton("LogMsgBtn", content, "Log Message", AccentPrimary), "TestLogMessage");
            WireButton(CreateButton("SetKeyBtn", content, "Set Custom Key", AccentPrimary), "TestSetCustomKey");
            WireButton(CreateButton("LogExBtn", content, "Log Exception", AccentWarning), "TestLogException");
            WireButton(CreateButton("CrashBtn", content, "⚠️ Force Crash", AccentDanger), "TestForceCrash");

            return section;
        }

        static GameObject CreateLogSection(Transform parent)
        {
            GameObject section = CreateSection("LogSection", "📝 Log Output", parent, TextSecondary);
            Transform content = section.transform.Find("Content");

            // Log area
            GameObject logArea = CreatePanel("LogArea", content, BgInput);
            logArea.AddComponent<LayoutElement>().preferredHeight = 200;
            var logScroll = logArea.AddComponent<ScrollRect>();

            var logViewport = new GameObject("LogViewport");
            logViewport.transform.SetParent(logArea.transform, false);
            var lvRect = logViewport.AddComponent<RectTransform>();
            SetAnchors(logViewport, Vector2.zero, Vector2.one, new Vector2(12, 10), new Vector2(-12, -10));
            logViewport.AddComponent<Image>().color = Color.clear;
            logViewport.AddComponent<Mask>().showMaskGraphic = false;

            var logContent = new GameObject("LogContent");
            logContent.transform.SetParent(logViewport.transform, false);
            var lcRect = logContent.AddComponent<RectTransform>();
            lcRect.anchorMin = new Vector2(0, 1);
            lcRect.anchorMax = new Vector2(1, 1);
            lcRect.pivot = new Vector2(0, 1);
            lcRect.offsetMin = Vector2.zero;
            lcRect.offsetMax = Vector2.zero;
            logContent.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            TextMeshProUGUI logText = CreateText("", logContent.transform, 18);
            logText.gameObject.name = "LogText";
            logText.alignment = TextAlignmentOptions.TopLeft;
            logText.color = new Color(0.75f, 0.85f, 0.75f);
            var ltRect = logText.GetComponent<RectTransform>();
            ltRect.anchorMin = Vector2.zero;
            ltRect.anchorMax = new Vector2(1, 1);
            ltRect.pivot = new Vector2(0, 1);
            ltRect.offsetMin = Vector2.zero;
            ltRect.offsetMax = Vector2.zero;

            logScroll.viewport = lvRect;
            logScroll.content = lcRect;
            logScroll.horizontal = false;

            // Clear button
            WireButton(CreateButton("ClearLogBtn", content, "🗑️ Clear Log", new Color(0.4f, 0.4f, 0.45f)), null);

            return section;
        }

        #endregion

        #region UI Helpers

        static GameObject CreateSection(string name, string title, Transform parent, Color accentColor)
        {
            GameObject section = CreatePanel(name, parent, BgSection);

            var layout = section.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 18);
            layout.spacing = 12;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            section.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Header with accent bar
            GameObject header = CreatePanel("Header", section.transform, Color.clear);
            header.AddComponent<LayoutElement>().preferredHeight = 36;
            var headerLayout = header.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 10;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;

            // Accent bar
            GameObject bar = CreatePanel("AccentBar", header.transform, accentColor);
            bar.AddComponent<LayoutElement>().preferredWidth = 4;

            // Title
            TextMeshProUGUI titleText = CreateText(title, header.transform, 22);
            titleText.fontStyle = FontStyles.Bold;
            titleText.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            // Content container
            var content = new GameObject("Content");
            content.transform.SetParent(section.transform, false);
            content.AddComponent<RectTransform>();

            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 10;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            return section;
        }

        static void CreateSubsectionHeader(string title, Transform parent)
        {
            TextMeshProUGUI text = CreateText(title, parent, 16);
            text.color = TextSecondary;
            text.fontStyle = FontStyles.Bold;
            var le = text.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 28;
        }

        static GameObject CreateButtonRow(Transform parent)
        {
            var row = new GameObject("ButtonRow");
            row.transform.SetParent(parent, false);
            row.AddComponent<RectTransform>();

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            row.AddComponent<LayoutElement>().preferredHeight = 50;

            return row;
        }

        static GameObject CreateButton(string name, Transform parent, string text, Color color)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);

            var rect = btn.AddComponent<RectTransform>();
            var img = btn.AddComponent<Image>();
            img.color = color;

            var button = btn.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = color * 1.15f;
            colors.pressedColor = color * 0.85f;
            colors.selectedColor = color;
            button.colors = colors;

            btn.AddComponent<LayoutElement>().preferredHeight = 55;

            TextMeshProUGUI textComp = CreateText(text, btn.transform, 20);
            textComp.gameObject.name = "Text";
            textComp.fontStyle = FontStyles.Bold;
            textComp.alignment = TextAlignmentOptions.Center;
            var textRect = textComp.GetComponent<RectTransform>();
            SetAnchors(textComp.gameObject, Vector2.zero, Vector2.one, new Vector2(15, 8), new Vector2(-15, -8));

            return btn;
        }

        static GameObject CreateSmallButton(string name, Transform parent, string text, Color color)
        {
            GameObject btn = CreateButton(name, parent, text, color);
            var le = btn.GetComponent<LayoutElement>();
            le.preferredHeight = 48;
            le.flexibleWidth = 1;

            var textComp = btn.GetComponentInChildren<TextMeshProUGUI>();
            textComp.fontSize = 18;

            return btn;
        }

        static void WireButton(GameObject buttonGo, string methodName)
        {
            // Naming convention: Button name ends with _MethodName
            // The PrototypeTestUI.WireButtons() will find this and connect automatically
            if (!string.IsNullOrEmpty(methodName))
                buttonGo.name = buttonGo.name + "_" + methodName;
        }

        static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            go.AddComponent<Image>().color = color;
            return go;
        }

        static TextMeshProUGUI CreateText(string text, Transform parent, int fontSize)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = TextPrimary;

            return tmp;
        }

        static void SetAnchors(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        #endregion
    }
}
