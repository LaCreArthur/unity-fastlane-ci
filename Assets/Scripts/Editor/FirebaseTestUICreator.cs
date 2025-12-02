using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace Sorolla.Editor
{
    /// <summary>
    /// Editor utility to create Firebase Test UI in the scene.
    /// </summary>
    public static class FirebaseTestUICreator
    {
        private static readonly Color HeaderColor = new Color(0.2f, 0.2f, 0.25f);
        private static readonly Color PanelColor = new Color(0.15f, 0.15f, 0.18f);
        private static readonly Color ButtonColor = new Color(0.3f, 0.5f, 0.7f);
        private static readonly Color ButtonHoverColor = new Color(0.4f, 0.6f, 0.8f);
        private static readonly Color SuccessColor = new Color(0.3f, 0.7f, 0.4f);
        private static readonly Color WarningColor = new Color(0.9f, 0.5f, 0.3f);
        private static readonly Color DangerColor = new Color(0.8f, 0.3f, 0.3f);

        [MenuItem("Sorolla/Create Firebase Test UI")]
        public static void CreateFirebaseTestUI()
        {
            // Find or create Canvas
            var canvas = FindOrCreateCanvas();
            
            // Create main panel
            var mainPanel = CreateMainPanel(canvas.transform);
            
            // Create header
            CreateHeader(mainPanel.transform);
            
            // Create scroll view for content
            var scrollView = CreateScrollView(mainPanel.transform);
            var content = scrollView.transform.Find("Viewport/Content");
            
            // Create sections
            CreateStatusSection(content);
            CreateAnalyticsSection(content);
            CreateCrashlyticsSection(content);
            CreateRemoteConfigSection(content);
            CreateLogSection(content);
            
            // Create toggle button (outside main panel)
            CreateToggleButton(canvas.transform, mainPanel);
            
            // Add the test controller script
            var controller = mainPanel.AddComponent<FirebaseTestUIController>();
            
            // Select the created object
            Selection.activeGameObject = mainPanel;
            
            Debug.Log("[Sorolla] Firebase Test UI created successfully!");
        }

        private static Canvas FindOrCreateCanvas()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null) return canvas;

            var canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasGO.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Create EventSystem if needed
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            return canvas;
        }

        private static GameObject CreateMainPanel(Transform parent)
        {
            var panel = CreatePanel("FirebaseTestPanel", parent);
            var rect = panel.GetComponent<RectTransform>();
            
            // Full screen with margins for portrait mobile
            rect.anchorMin = new Vector2(0.02f, 0.02f);
            rect.anchorMax = new Vector2(0.98f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = panel.GetComponent<Image>();
            image.color = PanelColor;

            // Add vertical layout
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Undo.RegisterCreatedObjectUndo(panel, "Create Firebase Test Panel");
            return panel;
        }

        private static void CreateHeader(Transform parent)
        {
            var header = CreatePanel("Header", parent);
            var headerRect = header.GetComponent<RectTransform>();
            headerRect.sizeDelta = new Vector2(0, 80);
            
            var headerImage = header.GetComponent<Image>();
            headerImage.color = HeaderColor;

            // Firebase icon placeholder + title
            var title = CreateText("🔥 Firebase Test Panel", header.transform);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = Vector2.zero;
            titleRect.anchorMax = Vector2.one;
            titleRect.offsetMin = new Vector2(20, 10);
            titleRect.offsetMax = new Vector2(-20, -10);
            
            var tmp = title.GetComponent<TextMeshProUGUI>();
            tmp.fontSize = 32;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;

            var layoutElement = header.AddComponent<LayoutElement>();
            layoutElement.minHeight = 80;
            layoutElement.preferredHeight = 80;
        }

        private static GameObject CreateScrollView(Transform parent)
        {
            var scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(parent, false);
            
            var scrollRect = scrollView.AddComponent<RectTransform>();
            var scroll = scrollView.AddComponent<ScrollRect>();
            scrollView.AddComponent<Image>().color = new Color(0, 0, 0, 0.1f);
            
            var layoutElement = scrollView.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1;

            // Viewport
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            var viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
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
            contentLayout.padding = new RectOffset(10, 10, 10, 10);
            contentLayout.spacing = 15;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            var contentSizeFitter = content.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewportRect;
            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.scrollSensitivity = 30;

            return scrollView;
        }

        private static void CreateStatusSection(Transform parent)
        {
            var section = CreateSection("StatusSection", "📊 Status", parent);
            var content = section.transform.Find("Content");

            // Status rows
            CreateStatusRow("SorollaStatus", "Sorolla Initialized:", content);
            CreateStatusRow("FirebaseRCStatus", "Firebase RC Ready:", content);
            CreateStatusRow("GARCStatus", "GA Remote Config:", content);

            CreateButton("RefreshStatusBtn", "🔄 Refresh Status", content, ButtonColor);
        }

        private static void CreateAnalyticsSection(Transform parent)
        {
            var section = CreateSection("AnalyticsSection", "📈 Analytics", parent);
            var content = section.transform.Find("Content");

            CreateButton("TrackDesignBtn", "Track Design Event", content, ButtonColor);
            CreateButton("TrackDesignValueBtn", "Track Design + Value", content, ButtonColor);
            CreateButton("TrackProgressStartBtn", "Track Progress: Start", content, ButtonColor);
            CreateButton("TrackProgressCompleteBtn", "Track Progress: Complete", content, SuccessColor);
            CreateButton("TrackResourceSourceBtn", "Track Resource: Earned", content, SuccessColor);
            CreateButton("TrackResourceSinkBtn", "Track Resource: Spent", content, WarningColor);
        }

        private static void CreateCrashlyticsSection(Transform parent)
        {
            var section = CreateSection("CrashlyticsSection", "🔥 Crashlytics", parent);
            var content = section.transform.Find("Content");

            CreateButton("LogMessageBtn", "Log Message", content, ButtonColor);
            CreateButton("SetCustomKeyBtn", "Set Custom Key", content, ButtonColor);
            CreateButton("LogExceptionBtn", "Log Non-Fatal Exception", content, WarningColor);
            CreateButton("ForceCrashBtn", "⚠️ Force Crash (DANGER!)", content, DangerColor);
        }

        private static void CreateRemoteConfigSection(Transform parent)
        {
            var section = CreateSection("RemoteConfigSection", "⚙️ Remote Config", parent);
            var content = section.transform.Find("Content");

            CreateButton("FetchRCBtn", "🔄 Fetch Remote Config", content, ButtonColor);
            
            var separator = CreatePanel("Separator", content);
            separator.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
            separator.AddComponent<LayoutElement>().preferredHeight = 2;

            CreateButton("GetStringBtn", "Get String Value", content, ButtonColor);
            CreateButton("GetIntBtn", "Get Int Value", content, ButtonColor);
            CreateButton("GetBoolBtn", "Get Bool Value", content, ButtonColor);
            CreateButton("GetFloatBtn", "Get Float Value", content, ButtonColor);
        }

        private static void CreateLogSection(Transform parent)
        {
            var section = CreateSection("LogSection", "📝 Log Output", parent);
            var content = section.transform.Find("Content");

            // Log text area with scroll
            var logArea = CreatePanel("LogArea", content);
            logArea.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            var logAreaLayout = logArea.AddComponent<LayoutElement>();
            logAreaLayout.minHeight = 200;
            logAreaLayout.preferredHeight = 200;

            var logScroll = logArea.AddComponent<ScrollRect>();
            
            var logViewport = new GameObject("LogViewport");
            logViewport.transform.SetParent(logArea.transform, false);
            var logViewportRect = logViewport.AddComponent<RectTransform>();
            logViewportRect.anchorMin = Vector2.zero;
            logViewportRect.anchorMax = Vector2.one;
            logViewportRect.offsetMin = new Vector2(10, 10);
            logViewportRect.offsetMax = new Vector2(-10, -10);
            logViewport.AddComponent<Image>().color = Color.clear;
            logViewport.AddComponent<Mask>().showMaskGraphic = false;

            var logContent = new GameObject("LogContent");
            logContent.transform.SetParent(logViewport.transform, false);
            var logContentRect = logContent.AddComponent<RectTransform>();
            logContentRect.anchorMin = new Vector2(0, 1);
            logContentRect.anchorMax = new Vector2(1, 1);
            logContentRect.pivot = new Vector2(0, 1);
            logContentRect.offsetMin = Vector2.zero;
            logContentRect.offsetMax = Vector2.zero;
            logContent.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var logText = CreateText("", logContent.transform);
            logText.name = "LogText";
            var logTMP = logText.GetComponent<TextMeshProUGUI>();
            logTMP.fontSize = 20;
            logTMP.alignment = TextAlignmentOptions.TopLeft;
            logTMP.color = new Color(0.8f, 0.9f, 0.8f);
            var logTextRect = logText.GetComponent<RectTransform>();
            logTextRect.anchorMin = Vector2.zero;
            logTextRect.anchorMax = new Vector2(1, 1);
            logTextRect.pivot = new Vector2(0, 1);
            logTextRect.offsetMin = Vector2.zero;
            logTextRect.offsetMax = Vector2.zero;

            logScroll.viewport = logViewportRect;
            logScroll.content = logContentRect;
            logScroll.horizontal = false;
            logScroll.vertical = true;

            CreateButton("ClearLogBtn", "🗑️ Clear Log", content, new Color(0.4f, 0.4f, 0.4f));
        }

        private static void CreateToggleButton(Transform parent, GameObject panel)
        {
            var toggleBtn = CreateButton("TogglePanelBtn", "🔥", parent, new Color(0.8f, 0.4f, 0.2f));
            var rect = toggleBtn.GetComponent<RectTransform>();
            
            // Position in top-right corner
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(80, 80);

            // Remove layout element
            Object.DestroyImmediate(toggleBtn.GetComponent<LayoutElement>());

            var tmp = toggleBtn.GetComponentInChildren<TextMeshProUGUI>();
            tmp.fontSize = 40;
        }

        private static GameObject CreateSection(string name, string title, Transform parent)
        {
            var section = CreatePanel(name, parent);
            section.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.22f);

            var layout = section.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 10, 15);
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var sectionLayout = section.AddComponent<LayoutElement>();
            sectionLayout.minHeight = 50;

            var sizeFitter = section.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Header
            var header = CreateText(title, section.transform);
            header.name = "Header";
            var headerTMP = header.GetComponent<TextMeshProUGUI>();
            headerTMP.fontSize = 26;
            headerTMP.fontStyle = FontStyles.Bold;
            headerTMP.color = new Color(0.9f, 0.9f, 0.9f);
            header.AddComponent<LayoutElement>().preferredHeight = 40;

            // Content container
            var content = new GameObject("Content");
            content.transform.SetParent(section.transform, false);
            content.AddComponent<RectTransform>();
            
            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 8;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            
            return section;
        }

        private static void CreateStatusRow(string name, string label, Transform parent)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent, false);
            row.AddComponent<RectTransform>();
            
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.spacing = 10;

            row.AddComponent<LayoutElement>().preferredHeight = 35;

            var labelGO = CreateText(label, row.transform);

            labelGO.name = "Label";
            var labelTMP = labelGO.GetComponent<TextMeshProUGUI>();
            labelTMP.fontSize = 22;
            labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
            labelGO.AddComponent<LayoutElement>().flexibleWidth = 1;

            var valueGO = CreateText("--", row.transform);
            valueGO.name = "Value";
            var valueTMP = valueGO.GetComponent<TextMeshProUGUI>();
            valueTMP.fontSize = 22;
            valueTMP.alignment = TextAlignmentOptions.MidlineRight;
            valueTMP.fontStyle = FontStyles.Bold;
            valueGO.AddComponent<LayoutElement>().preferredWidth = 100;
        }

        private static GameObject CreatePanel(string name, Transform parent)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private static GameObject CreateButton(string name, string text, Transform parent, Color color)
        {
            var buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            var rect = buttonGO.AddComponent<RectTransform>();
            var image = buttonGO.AddComponent<Image>();
            image.color = color;
            
            var button = buttonGO.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = color * 1.2f;
            colors.pressedColor = color * 0.8f;
            button.colors = colors;

            var layout = buttonGO.AddComponent<LayoutElement>();
            layout.minHeight = 55;
            layout.preferredHeight = 55;

            var textGO = CreateText(text, buttonGO.transform);
            textGO.name = "Text";
            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(15, 5);
            textRect.offsetMax = new Vector2(-15, -5);
            
            var tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            return buttonGO;
        }

        private static GameObject CreateText(string text, Transform parent)
        {
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(parent, false);
            textGO.AddComponent<RectTransform>();
            
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.color = Color.white;

            return textGO;
        }
    }
}
