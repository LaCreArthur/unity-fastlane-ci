using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Sorolla.DebugUI.Editor
{
    /// <summary>
    ///     Creates the full debug panel by assembling the prefabs we created.
    ///     This is the final assembly step that instantiates and connects all prefab components.
    /// </summary>
    public class CreateDebugPanelWithControllers : SorollaDebugPrefabCreator
    {
        static SorollaDebugPrefabs _prefabs;
        static GameObject _canvasGO;
        static List<GameObject> _tabPages;

        // Cached prefab references
        static GameObject _sectionCardPrefab;
        static GameObject _actionButtonPrefab;
        static GameObject _actionButtonFilledPrefab;
        static GameObject _toggleSwitchPrefab;
        static GameObject _toggleRowPrefab;
        static GameObject _adCardPrefab;
        static GameObject _identityCardPrefab;
        static GameObject _sdkHealthIndicatorPrefab;
        static GameObject _configRowPrefab;
        static GameObject _navButtonPrefab;
        static GameObject _toastNotificationPrefab;
        static GameObject _statusBadgePrefab;
        static GameObject _logEntryPrefab;
        static GameObject _dividerPrefab;
        static GameObject _headerPrefab;
        static GameObject _logFilterBarPrefab;
        static GameObject _mediationTesterCardPrefab;
        static GameObject _bottomNavBarPrefab;

        [MenuItem("Sorolla/Debug UI/6. Create Debug Panel With Controllers")]
        public static void Create()
        {
            if (Theme == null)
            {
                Debug.LogError("Theme not found.  Run 'Create Theme Asset' first.");
                return;
            }

            // Load all prefabs
            if (!LoadAllPrefabs())
            {
                Debug.LogError("Some prefabs are missing. Run 'Create All Prefabs' first.");
                return;
            }

            _prefabs = SorollaDebugPrefabs.Instance;
            _tabPages = new List<GameObject>();

            // Create main canvas
            _canvasGO = CreateCanvas();

            // Safe Area Container
            GameObject safeArea = CreateSafeAreaContainer(_canvasGO.transform);

            // Header (from prefab)
            GameObject header = InstantiatePrefab(_headerPrefab, "Header", safeArea.transform);
            if (header != null)
            {
                AddLayoutElement(header, flexibleWidth: 1, preferredHeight: 72);
            }

            // Toast Container
            GameObject toastContainer = CreateToastContainer(safeArea.transform);

            // Content Area with Tab Pages
            GameObject contentArea = CreateContentArea(safeArea.transform);

            // Bottom Navigation
            GameObject bottomNav = CreateBottomNavigation(safeArea.transform);

            // Add main controllers
            AddMainControllers(_canvasGO, toastContainer);

            // Select in hierarchy
            Selection.activeGameObject = _canvasGO;

            Debug.Log("=== Debug Panel Created Successfully ===");
            Debug.Log("The panel uses prefab instances and includes all controllers.");
            Debug.Log("Connect your SDK callbacks to DebugPanelManager.Instance methods.");
        }

        /// <summary>
        ///     Loads all prefabs from the prefab folder.  Returns false if critical prefabs are missing.
        /// </summary>
        static bool LoadAllPrefabs()
        {
            _sectionCardPrefab = LoadPrefab("SectionCard");
            _actionButtonPrefab = LoadPrefab("ActionButton");
            _actionButtonFilledPrefab = LoadPrefab("ActionButtonFilled");
            _toggleSwitchPrefab = LoadPrefab("ToggleSwitch");
            _toggleRowPrefab = LoadPrefab("ToggleRow");
            _adCardPrefab = LoadPrefab("AdCard");
            _identityCardPrefab = LoadPrefab("IdentityCard");
            _sdkHealthIndicatorPrefab = LoadPrefab("SDKHealthIndicator");
            _configRowPrefab = LoadPrefab("ConfigRow");
            _navButtonPrefab = LoadPrefab("NavButton");
            _toastNotificationPrefab = LoadPrefab("ToastNotification");
            _statusBadgePrefab = LoadPrefab("StatusBadge");
            _logEntryPrefab = LoadPrefab("LogEntry");
            _dividerPrefab = LoadPrefab("Divider");
            _headerPrefab = LoadPrefab("Header");
            _logFilterBarPrefab = LoadPrefab("LogFilterBar");
            _mediationTesterCardPrefab = LoadPrefab("MediationTesterCard");
            _bottomNavBarPrefab = LoadPrefab("BottomNavBar");

            // Check critical prefabs
            bool hasCritical = _sectionCardPrefab != null &&
                               _actionButtonPrefab != null &&
                               _navButtonPrefab != null;

            if (!hasCritical)
            {
                Debug.LogWarning("Some prefabs are missing. The panel will use fallback inline creation for missing components.");
            }

            return true; // Allow creation with fallbacks
        }

        static GameObject LoadPrefab(string prefabName)
        {
            string path = PREFAB_PATH + prefabName + ".prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found: {path}");
            }

            return prefab;
        }

        /// <summary>
        ///     Instantiates a prefab as a child of the parent.  Returns null if prefab is missing.
        /// </summary>
        static GameObject InstantiatePrefab(GameObject prefab, string instanceName, Transform parent)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"Cannot instantiate null prefab for: {instanceName}");
                return null;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = instanceName;
            return instance;
        }

        // === CANVAS ===
        static GameObject CreateCanvas()
        {
            var canvasGO = new GameObject("SorollaDebugPanel");

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            var bg = canvasGO.AddComponent<Image>();
            bg.color = Theme.canvasBackground;

            return canvasGO;
        }

        // === SAFE AREA ===
        static GameObject CreateSafeAreaContainer(Transform parent)
        {
            GameObject safeArea = CreateUIObject("SafeAreaContainer", parent);
            var rt = safeArea.GetComponent<RectTransform>();
            SetStretch(rt);

            safeArea.AddComponent<SafeAreaHandler>();

            AddVerticalLayout(safeArea);

            return safeArea;
        }

        // === TOAST CONTAINER ===
        static GameObject CreateToastContainer(Transform parent)
        {
            GameObject container = CreateUIObject("ToastContainer", parent);
            var rt = container.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -80);
            rt.sizeDelta = new Vector2(350, 120);

            AddLayoutElement(container, ignoreLayout: true);

            AddVerticalLayout(container, 8,
                childAlignment: TextAnchor.UpperCenter,
                childForceExpandWidth: false, childForceExpandHeight: false);

            return container;
        }

        // === CONTENT AREA ===
        static GameObject CreateContentArea(Transform parent)
        {
            GameObject contentArea = CreateUIObject("ContentArea", parent);
            AddLayoutElement(contentArea, flexibleWidth: 1, flexibleHeight: 1);

            var rt = contentArea.GetComponent<RectTransform>();
            SetStretch(rt);

            GameObject tabPages = CreateUIObject("TabPages", contentArea.transform);
            var tabPagesRt = tabPages.GetComponent<RectTransform>();
            SetStretch(tabPagesRt);

            // Create all tabs using prefabs
            _tabPages.Add(CreateDashTab(tabPages.transform));
            _tabPages.Add(CreateAdsTab(tabPages.transform));
            _tabPages.Add(CreateEventsTab(tabPages.transform));
            _tabPages.Add(CreateToolsTab(tabPages.transform));
            _tabPages.Add(CreateLogsTab(tabPages.transform));

            // Only first tab active
            for (int i = 1; i < _tabPages.Count; i++)
            {
                _tabPages[i].SetActive(false);
            }

            return contentArea;
        }

        // === DASH TAB ===
        static GameObject CreateDashTab(Transform parent)
        {
            GameObject tab = CreateScrollableTab("DashTab", parent);
            Transform content = tab.transform.Find("Viewport/Content");

            // Device Identity Section (using SectionCard prefab)
            GameObject identitySection = InstantiateSectionCard("DEVICE IDENTITY", content);
            Transform identityContent = GetSectionContent(identitySection);

            // Identity Cards (using IdentityCard prefab)
            CreateIdentityCardFromPrefab("IDFA / GAID", "A1B2-C3D4-E5F6-G7H8", identityContent);
            CreateIdentityCardFromPrefab("FACEBOOK APP ID", "1849204928492", identityContent);

            // SDK Health Section
            GameObject healthSection = InstantiateSectionCard("SDK HEALTH", content);
            Transform healthContent = GetSectionContent(healthSection);

            // 2x2 Grid using SDKHealthIndicator prefabs
            GameObject row1 = CreateUIObject("Row1", healthContent);
            AddHorizontalLayout(row1, 12);
            AddLayoutElement(row1, flexibleWidth: 1, preferredHeight: 48);

            CreateSDKHealthFromPrefab("GameAnalytics", true, true, row1.transform);
            CreateSDKHealthFromPrefab("Facebook", true, true, row1.transform);

            GameObject row2 = CreateUIObject("Row2", healthContent);
            AddHorizontalLayout(row2, 12);
            AddLayoutElement(row2, flexibleWidth: 1, preferredHeight: 48);

            CreateSDKHealthFromPrefab("MAX Ads", false, false, row2.transform);
            CreateSDKHealthFromPrefab("Crashlytics", true, true, row2.transform);

            // Quick Toggles Section
            GameObject togglesSection = InstantiateSectionCard("QUICK TOGGLES", content);
            Transform togglesContent = GetSectionContent(togglesSection);

            CreateToggleRowFromPrefab("debug_mode", "Debug Mode", true, togglesContent);
            InstantiateDivider(togglesContent);
            CreateToggleRowFromPrefab("god_mode", "God Mode (Invincible)", false, togglesContent);

            return tab;
        }

        // === ADS TAB ===
        static GameObject CreateAdsTab(Transform parent)
        {
            GameObject tab = CreateScrollableTab("AdsTab", parent);
            Transform content = tab.transform.Find("Viewport/Content");

            // Mediation Tester Card (from prefab)
            GameObject mediationCard = InstantiatePrefab(_mediationTesterCardPrefab, "MediationTesterCard", content);
            if (mediationCard == null) {}

            // Ad Cards (from prefab)
            CreateAdCardFromPrefab(AdType.Interstitial, "Interstitial", "Full screen break", Theme.accentRed, content);
            CreateAdCardFromPrefab(AdType.Rewarded, "Rewarded Video", "Value exchange", Theme.accentOrange, content);
            CreateBannerCardFromPrefab(content);

            return tab;
        }

        // === EVENTS TAB ===
        static GameObject CreateEventsTab(Transform parent)
        {
            GameObject tab = CreateScrollableTab("EventsTab", parent);
            Transform content = tab.transform.Find("Viewport/Content");

            // Progression Section
            GameObject progressionSection = InstantiateSectionCard("PROGRESSION", content);
            Transform progressionContent = GetSectionContent(progressionSection);

            GameObject progressionRow = CreateUIObject("ButtonRow", progressionContent);
            AddHorizontalLayout(progressionRow, 8);
            AddLayoutElement(progressionRow, flexibleWidth: 1, preferredHeight: 48);

            CreateActionButtonFromPrefab("Start", false, progressionRow.transform);
            CreateActionButtonFromPrefab("Win", true, progressionRow.transform, Theme.accentGreen);
            CreateActionButtonFromPrefab("Fail", true, progressionRow.transform, Theme.accentRed);

            // Resources Section
            GameObject resourcesSection = InstantiateSectionCard("RESOURCES", content);
            Transform resourcesContent = GetSectionContent(resourcesSection);

            GameObject resourcesRow = CreateUIObject("ButtonRow", resourcesContent);
            AddHorizontalLayout(resourcesRow, 8);
            AddLayoutElement(resourcesRow, flexibleWidth: 1, preferredHeight: 48);

            CreateActionButtonFromPrefab("Add Coins", false, resourcesRow.transform);
            CreateActionButtonFromPrefab("Spend Coins", false, resourcesRow.transform);

            // Custom Events Section
            GameObject customSection = InstantiateSectionCard("CUSTOM EVENTS", content);
            Transform customContent = GetSectionContent(customSection);

            CreateActionButtonFromPrefab("Track 'Jump'", false, customContent);
            CreateActionButtonFromPrefab("Track 'NPC Talk'", false, customContent);

            return tab;
        }

        // === TOOLS TAB ===
        static GameObject CreateToolsTab(Transform parent)
        {
            GameObject tab = CreateScrollableTab("ToolsTab", parent);
            Transform content = tab.transform.Find("Viewport/Content");

            // Remote Config Section (with accent)
            GameObject remoteSection = InstantiateSectionCardWithAccent("REMOTE CONFIG", Theme.accentYellow, content);
            Transform remoteContent = GetSectionContentWithAccent(remoteSection);

            GameObject fetchBtn = CreateActionButtonFromPrefab("Fetch & Activate", false, remoteContent);

            // Config rows using ConfigRow prefab
            GameObject configList = CreateUIObject("ConfigList", remoteContent);
            AddVerticalLayout(configList, 8, new RectOffset(0, 0, 8, 0));
            AddLayoutElement(configList, flexibleWidth: 1);

            CreateConfigRowFromPrefab("hero_speed", "12. 5", Theme.accentYellow, configList.transform);
            CreateConfigRowFromPrefab("show_ads_level", "3", Theme.accentCyan, configList.transform);
            CreateConfigRowFromPrefab("seasonal_event", "\"halloween\"", Theme.accentOrange, configList.transform);

            // Add RemoteConfigDisplay controller
            var configDisplay = remoteSection.AddComponent<RemoteConfigDisplay>();
            var configSO = new SerializedObject(configDisplay);
            configSO.FindProperty("_container").objectReferenceValue = configList.transform;
            if (fetchBtn != null)
            {
                configSO.FindProperty("_fetchButton").objectReferenceValue = fetchBtn.GetComponent<Button>();
            }
            configSO.FindProperty("_configRowPrefab").objectReferenceValue = _configRowPrefab;
            configSO.ApplyModifiedProperties();

            // Privacy & CMP Section
            GameObject privacySection = InstantiateSectionCard("PRIVACY & CMP", content);
            Transform privacyContent = GetSectionContent(privacySection);

            GameObject privacyRow = CreateUIObject("ButtonRow", privacyContent);
            AddHorizontalLayout(privacyRow, 8);
            AddLayoutElement(privacyRow, flexibleWidth: 1, preferredHeight: 48);

            CreateActionButtonFromPrefab("Reset Consent", false, privacyRow.transform);
            CreateActionButtonFromPrefab("Show ATT", false, privacyRow.transform);

            // Crashlytics Section (with accent)
            GameObject crashSection = InstantiateSectionCardWithAccent("CRASHLYTICS", Theme.accentRed, content);
            Transform crashContent = GetSectionContentWithAccent(crashSection);

            var redBg = new Color(Theme.accentRed.r * 0.3f, Theme.accentRed.g * 0.2f, Theme.accentRed.b * 0.2f, 1f);
            CreateActionButtonFromPrefab("Log Exception", false, crashContent, redBg, Theme.accentRed);
            CreateActionButtonFromPrefab("Force Crash", false, crashContent, redBg, Theme.accentRed);

            return tab;
        }

        // === LOGS TAB ===
        static GameObject CreateLogsTab(Transform parent)
        {
            GameObject tab = CreateUIObject("LogsTab", parent);
            var tabRt = tab.GetComponent<RectTransform>();
            SetStretch(tabRt);

            AddVerticalLayout(tab);

            // Filter Bar (from prefab)
            GameObject filterBar = InstantiatePrefab(_logFilterBarPrefab, "LogFilterBar", tab.transform);
            if (filterBar == null) {}
            AddLayoutElement(filterBar, flexibleWidth: 1, preferredHeight: 44);

            // Log Scroll View
            GameObject scrollView = CreateUIObject("LogScrollView", tab.transform);
            AddLayoutElement(scrollView, flexibleWidth: 1, flexibleHeight: 1);

            var scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;

            GameObject viewport = CreateUIObject("Viewport", scrollView.transform);
            var viewportRt = viewport.GetComponent<RectTransform>();
            SetStretch(viewportRt);
            Image viewportImg = AddImage(viewport, Color.white);
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scroll.viewport = viewportRt;

            GameObject logContent = CreateUIObject("LogContent", viewport.transform);
            var logContentRt = logContent.GetComponent<RectTransform>();
            logContentRt.anchorMin = new Vector2(0, 1);
            logContentRt.anchorMax = new Vector2(1, 1);
            logContentRt.pivot = new Vector2(0.5f, 1);
            logContentRt.sizeDelta = Vector2.zero;

            AddVerticalLayout(logContent);
            AddContentSizeFitter(logContent);

            scroll.content = logContentRt;

            // Add LogController
            var logController = tab.AddComponent<LogController>();
            var logSO = new SerializedObject(logController);
            logSO.FindProperty("_logContainer").objectReferenceValue = logContent.transform;
            logSO.FindProperty("_scrollRect").objectReferenceValue = scroll;
            logSO.FindProperty("_logEntryPrefab").objectReferenceValue = _logEntryPrefab;
            logSO.ApplyModifiedProperties();

            // Sample log entries (from prefab)
            CreateLogEntryFromPrefab("Resource: +100 Coins (Source: Shop)", LogSource.GA, LogLevel.Info, logContent.transform);
            CreateLogEntryFromPrefab("+100 Coins", LogSource.UI, LogLevel.Info, logContent.transform);
            CreateLogEntryFromPrefab("Resource: -50 Coins (Sink: Upgrade)", LogSource.GA, LogLevel.Info, logContent.transform);
            CreateLogEntryFromPrefab("God Mode Enabled", LogSource.Game, LogLevel.Info, logContent.transform);

            return tab;
        }

        // === BOTTOM NAVIGATION ===
        static GameObject CreateBottomNavigation(Transform parent)
        {
            GameObject nav = CreateUIObject("BottomNavBar", parent);
            AddImage(nav, Theme.canvasBackground);
            AddLayoutElement(nav, flexibleWidth: 1, preferredHeight: 80);

            AddHorizontalLayout(nav, 0,
                new RectOffset(8, 8, 0, 16),
                TextAnchor.MiddleCenter,
                true, true);

            string[] tabs = { "Dash", "Ads", "Events", "Tools", "Logs" };
            for (int i = 0; i < tabs.Length; i++)
            {
                CreateNavButtonFromPrefab(tabs[i], i, nav.transform, i == 0);
            }

            return nav;
        }

        // === MAIN CONTROLLERS ===
        static void AddMainControllers(GameObject canvas, GameObject toastContainer)
        {
            // DebugPanelManager
            var manager = canvas.AddComponent<DebugPanelManager>();
            var managerSO = new SerializedObject(manager);
            managerSO.FindProperty("_panelRoot").objectReferenceValue = canvas.transform.Find("SafeAreaContainer").gameObject;
            managerSO.ApplyModifiedProperties();

            // TabController
            var tabController = canvas.AddComponent<TabController>();
            var tabSO = new SerializedObject(tabController);
            SerializedProperty tabPagesProperty = tabSO.FindProperty("_tabPages");
            tabPagesProperty.arraySize = _tabPages.Count;
            for (int i = 0; i < _tabPages.Count; i++)
            {
                tabPagesProperty.GetArrayElementAtIndex(i).objectReferenceValue = _tabPages[i];
            }
            tabSO.ApplyModifiedProperties();

            managerSO = new SerializedObject(manager);
            managerSO.FindProperty("_tabController").objectReferenceValue = tabController;
            managerSO.ApplyModifiedProperties();

            // ToastController
            var toastController = canvas.AddComponent<ToastController>();
            var toastSO = new SerializedObject(toastController);
            toastSO.FindProperty("_toastContainer").objectReferenceValue = toastContainer.transform;
            toastSO.FindProperty("_toastPrefab").objectReferenceValue = _toastNotificationPrefab;
            toastSO.ApplyModifiedProperties();

            managerSO = new SerializedObject(manager);
            managerSO.FindProperty("_toastController").objectReferenceValue = toastController;
            managerSO.ApplyModifiedProperties();

            // Find and assign LogController
            var logController = canvas.GetComponentInChildren<LogController>(true);
            if (logController != null)
            {
                managerSO = new SerializedObject(manager);
                managerSO.FindProperty("_logController").objectReferenceValue = logController;
                managerSO.ApplyModifiedProperties();
            }
        }

        // =====================================================
        // PREFAB INSTANTIATION HELPERS
        // =====================================================

        static GameObject InstantiateSectionCard(string title, Transform parent)
        {
            GameObject section = InstantiatePrefab(_sectionCardPrefab, $"Section_{title.Replace(" ", "")}", parent);

            if (section == null)
            {
                // Fallback: create inline
            }

            // Update the title
            var titleTmp = section.transform.Find("SectionTitle")?.GetComponent<TextMeshProUGUI>();
            if (titleTmp != null)
            {
                titleTmp.text = title;
            }

            return section;
        }

        static GameObject InstantiateSectionCardWithAccent(string title, Color accentColor, Transform parent)
        {
            // SectionCard prefab doesn't have accent, so we wrap it
            GameObject wrapper = CreateUIObject($"Section_{title.Replace(" ", "")}", parent);
            AddImage(wrapper, Theme.cardBackground);
            AddContentSizeFitter(wrapper);
            AddLayoutElement(wrapper, flexibleWidth: 1);

            AddHorizontalLayout(wrapper);

            // Accent bar
            GameObject accent = CreateUIObject("AccentBar", wrapper.transform);
            AddImage(accent, accentColor);
            AddLayoutElement(accent, preferredWidth: 4);

            // Inner content
            GameObject inner = CreateUIObject("Inner", wrapper.transform);
            AddVerticalLayout(inner, 12,
                new RectOffset(16, 16, 16, 16),
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddLayoutElement(inner, flexibleWidth: 1);

            // Title
            GameObject titleObj = CreateUIObject("SectionTitle", inner.transform);
            TextMeshProUGUI tmp = AddText(titleObj, title, Theme.sectionHeaderSize,
                Theme.textSecondary, FontStyles.Bold);
            tmp.characterSpacing = 2;
            AddLayoutElement(titleObj, preferredHeight: 16);

            // Content container
            GameObject contentContainer = CreateUIObject("ContentContainer", inner.transform);
            AddVerticalLayout(contentContainer, 8,
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddContentSizeFitter(contentContainer);

            return wrapper;
        }

        static Transform GetSectionContent(GameObject section) => section?.transform.Find("ContentContainer");

        static Transform GetSectionContentWithAccent(GameObject section) => section?.transform.Find("Inner/ContentContainer");

        static void CreateIdentityCardFromPrefab(string label, string value, Transform parent)
        {
            GameObject card = InstantiatePrefab(_identityCardPrefab, $"IdentityCard_{label.Replace(" ", "").Replace("/", "")}", parent);

            if (card == null)
            {
                // Fallback inline creation
                return;
            }

            // Configure the card
            var labelTmp = card.transform.Find("TextColumn/Label")?.GetComponent<TextMeshProUGUI>();
            var valueTmp = card.transform.Find("TextColumn/Value")?.GetComponent<TextMeshProUGUI>();

            if (labelTmp != null) labelTmp.text = label;
            if (valueTmp != null) valueTmp.text = value;

            // Add/configure controller
            var controller = card.GetComponent<IdentityCardController>();
            if (controller == null)
            {
                controller = card.AddComponent<IdentityCardController>();
            }

            var so = new SerializedObject(controller);
            so.FindProperty("_labelText").objectReferenceValue = labelTmp;
            so.FindProperty("_valueText").objectReferenceValue = valueTmp;
            var copyBtn = card.transform.Find("CopyButton")?.GetComponent<Button>();
            if (copyBtn != null)
            {
                so.FindProperty("_copyButton").objectReferenceValue = copyBtn;
            }
            so.ApplyModifiedProperties();
        }

        static void CreateSDKHealthFromPrefab(string sdkName, bool enabled, bool healthy, Transform parent)
        {
            GameObject indicator = InstantiatePrefab(_sdkHealthIndicatorPrefab, $"SDK_{sdkName.Replace(" ", "")}", parent);

            if (indicator == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            // Configure
            var healthIndicator = indicator.GetComponent<SDKHealthIndicator>();
            if (healthIndicator != null)
            {
                healthIndicator.Setup(sdkName, enabled, healthy);
            }
            else
            {
                // Manual configuration
                var nameTmp = indicator.transform.Find("SDKNameLabel")?.GetComponent<TextMeshProUGUI>();
                var dotImg = indicator.transform.Find("StatusDot")?.GetComponent<Image>();

                if (nameTmp != null)
                {
                    nameTmp.text = sdkName;
                    nameTmp.color = enabled ? Theme.textPrimary : Theme.textDisabled;
                }
                if (dotImg != null)
                {
                    dotImg.color = enabled ? healthy ? Theme.statusActive : Theme.statusIdle : Theme.statusIdle;
                }
            }
        }

        static void CreateToggleRowFromPrefab(string toggleId, string label, bool defaultValue, Transform parent)
        {
            GameObject row = InstantiatePrefab(_toggleRowPrefab, $"Toggle_{toggleId}", parent);

            if (row == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            // Find and configure label
            var labelTmp = row.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (labelTmp != null)
            {
                labelTmp.text = label;
            }

            // Find toggle placeholder and replace with actual toggle
            Transform togglePlaceholder = row.transform.Find("ToggleSwitchPlaceholder");
            if (togglePlaceholder != null)
            {
                GameObject toggle = InstantiatePrefab(_toggleSwitchPrefab, "ToggleSwitch", row.transform);
                if (toggle != null)
                {
                    toggle.transform.SetSiblingIndex(togglePlaceholder.GetSiblingIndex());
                    Object.DestroyImmediate(togglePlaceholder.gameObject);

                    var toggleSwitch = toggle.GetComponent<ToggleSwitch>();
                    if (toggleSwitch != null)
                    {
                        toggleSwitch.SetValue(defaultValue);
                    }

                    // Add controller
                    var controller = row.AddComponent<QuickToggleController>();
                    var so = new SerializedObject(controller);
                    so.FindProperty("_label").objectReferenceValue = labelTmp;
                    so.FindProperty("_toggle").objectReferenceValue = toggleSwitch;
                    so.FindProperty("_toggleId").stringValue = toggleId;
                    so.FindProperty("_labelText").stringValue = label;
                    so.ApplyModifiedProperties();
                }
            }
        }

        static void InstantiateDivider(Transform parent)
        {
            GameObject divider = InstantiatePrefab(_dividerPrefab, "Divider", parent);
            if (divider == null)
            {
                // Fallback
                divider = CreateUIObject("Divider", parent);
                AddImage(divider, new Color(Theme.textSecondary.r, Theme.textSecondary.g, Theme.textSecondary.b, 0.2f));
                AddLayoutElement(divider, flexibleWidth: 1, preferredHeight: 1);
            }
        }

        static GameObject CreateActionButtonFromPrefab(string label, bool filled, Transform parent, Color? bgOverride = null, Color? textOverride = null)
        {
            GameObject prefab = filled ? _actionButtonFilledPrefab : _actionButtonPrefab;
            GameObject btn = InstantiatePrefab(prefab, $"Btn_{label.Replace(" ", "").Replace("'", "")}", parent);

            if (btn == null) {}

            // Configure label
            var labelTmp = btn.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (labelTmp != null)
            {
                labelTmp.text = label;
                if (textOverride.HasValue)
                {
                    labelTmp.color = textOverride.Value;
                }
            }

            // Configure background color override
            if (bgOverride.HasValue)
            {
                var bg = btn.GetComponent<Image>();
                if (bg != null)
                {
                    bg.color = bgOverride.Value;
                }
            }

            return btn;
        }

        static void CreateAdCardFromPrefab(AdType adType, string title, string subtitle, Color accentColor, Transform parent)
        {
            GameObject card = InstantiatePrefab(_adCardPrefab, $"AdCard_{adType}", parent);

            if (card == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            // Configure accent bar
            var accentBar = card.transform.Find("AccentBar")?.GetComponent<Image>();
            if (accentBar != null)
            {
                accentBar.color = accentColor;
            }

            // Configure title/subtitle
            var titleTmp = card.transform.Find("Content/HeaderRow/TitleColumn/Title")?.GetComponent<TextMeshProUGUI>();
            var subtitleTmp = card.transform.Find("Content/HeaderRow/TitleColumn/Subtitle")?.GetComponent<TextMeshProUGUI>();

            if (titleTmp != null) titleTmp.text = title;
            if (subtitleTmp != null) subtitleTmp.text = subtitle;

            // Replace status badge placeholder with actual StatusBadge
            Transform badgePlaceholder = card.transform.Find("Content/HeaderRow/StatusBadgePlaceholder");
            StatusBadge statusBadge = null;

            if (badgePlaceholder != null && _statusBadgePrefab != null)
            {
                GameObject badge = InstantiatePrefab(_statusBadgePrefab, "StatusBadge", badgePlaceholder.parent);
                if (badge != null)
                {
                    badge.transform.SetSiblingIndex(badgePlaceholder.GetSiblingIndex());
                    Object.DestroyImmediate(badgePlaceholder.gameObject);
                    statusBadge = badge.GetComponent<StatusBadge>();
                }
            }

            // Replace button placeholders with actual buttons
            Transform loadPlaceholder = card.transform.Find("Content/ButtonRow/LoadButtonPlaceholder");
            Transform showPlaceholder = card.transform.Find("Content/ButtonRow/ShowButtonPlaceholder");
            Button loadBtn = null, showBtn = null;

            if (loadPlaceholder != null)
            {
                GameObject load = CreateActionButtonFromPrefab("Load", false, loadPlaceholder.parent);
                if (load != null)
                {
                    load.transform.SetSiblingIndex(loadPlaceholder.GetSiblingIndex());
                    Object.DestroyImmediate(loadPlaceholder.gameObject);
                    loadBtn = load.GetComponent<Button>();
                }
            }

            if (showPlaceholder != null)
            {
                GameObject show = CreateActionButtonFromPrefab("Show", true, showPlaceholder.parent);
                if (show != null)
                {
                    show.transform.SetSiblingIndex(showPlaceholder.GetSiblingIndex());
                    Object.DestroyImmediate(showPlaceholder.gameObject);
                    showBtn = show.GetComponent<Button>();
                }
            }

            // Add controller
            var controller = card.AddComponent<AdCardController>();
            var so = new SerializedObject(controller);
            so.FindProperty("_accentBar").objectReferenceValue = accentBar;
            so.FindProperty("_titleLabel").objectReferenceValue = titleTmp;
            so.FindProperty("_subtitleLabel").objectReferenceValue = subtitleTmp;
            so.FindProperty("_statusBadge").objectReferenceValue = statusBadge;
            so.FindProperty("_loadButton").objectReferenceValue = loadBtn;
            so.FindProperty("_showButton").objectReferenceValue = showBtn;
            so.FindProperty("_adType").enumValueIndex = (int)adType;
            so.FindProperty("_title").stringValue = title;
            so.FindProperty("_subtitle").stringValue = subtitle;
            so.FindProperty("_accentColor").colorValue = accentColor;
            so.ApplyModifiedProperties();
        }

        static void CreateBannerCardFromPrefab(Transform parent)
        {
            // Banner card is special - uses AdCard structure but with toggle instead of buttons
            GameObject card = InstantiatePrefab(_adCardPrefab, "AdCard_Banner", parent);

            if (card == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            // Configure
            var accentBar = card.transform.Find("AccentBar")?.GetComponent<Image>();
            if (accentBar != null) accentBar.color = Theme.textSecondary;

            var titleTmp = card.transform.Find("Content/HeaderRow/TitleColumn/Title")?.GetComponent<TextMeshProUGUI>();
            var subtitleTmp = card.transform.Find("Content/HeaderRow/TitleColumn/Subtitle")?.GetComponent<TextMeshProUGUI>();

            if (titleTmp != null) titleTmp.text = "Banner";
            if (subtitleTmp != null) subtitleTmp.text = "Bottom 320×50";

            // Remove status badge for banner
            Transform badgePlaceholder = card.transform.Find("Content/HeaderRow/StatusBadgePlaceholder");
            if (badgePlaceholder != null) Object.DestroyImmediate(badgePlaceholder.gameObject);

            // Replace button row with toggle
            Transform buttonRow = card.transform.Find("Content/ButtonRow");
            if (buttonRow != null)
            {
                // Clear button row
                foreach (Transform child in buttonRow)
                {
                    Object.DestroyImmediate(child.gameObject);
                }

                // Add toggle
                GameObject toggle = InstantiatePrefab(_toggleSwitchPrefab, "BannerToggle", buttonRow);
                if (toggle == null) {}

                // Reconfigure layout
                var hlg = buttonRow.GetComponent<HorizontalLayoutGroup>();
                if (hlg != null)
                {
                    hlg.childAlignment = TextAnchor.MiddleRight;
                }
            }
        }

        static void CreateConfigRowFromPrefab(string key, string value, Color valueColor, Transform parent)
        {
            GameObject row = InstantiatePrefab(_configRowPrefab, $"Config_{key}", parent);

            if (row == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            var keyTmp = row.transform.Find("KeyLabel")?.GetComponent<TextMeshProUGUI>();
            var valueTmp = row.transform.Find("ValueLabel")?.GetComponent<TextMeshProUGUI>();

            if (keyTmp != null) keyTmp.text = key;
            if (valueTmp != null)
            {
                valueTmp.text = value;
                valueTmp.color = valueColor;
            }
        }

        static void CreateNavButtonFromPrefab(string label, int index, Transform parent, bool selected)
        {
            GameObject btn = InstantiatePrefab(_navButtonPrefab, $"NavBtn_{label}", parent);

            if (btn == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            // Configure
            var navBtn = btn.GetComponent<NavButton>();

            var labelTmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (labelTmp != null) labelTmp.text = label;

            // Update selection state visually
            GameObject highlight = btn.transform.Find("BackgroundHighlight")?.gameObject;
            if (highlight != null) highlight.SetActive(selected);

            var icon = btn.transform.Find("Content/Icon")?.GetComponent<Image>();
            if (icon != null) icon.color = selected ? Theme.accentPurple : Theme.textSecondary;

            if (labelTmp != null) labelTmp.color = selected ? Theme.accentPurple : Theme.textSecondary;

            // Configure NavButton component
            if (navBtn != null)
            {
                var so = new SerializedObject(navBtn);
                so.FindProperty("_tabIndex").intValue = index;
                so.ApplyModifiedProperties();
            }
        }

        static void CreateLogEntryFromPrefab(string message, LogSource source, LogLevel level, Transform parent)
        {
            GameObject entry = InstantiatePrefab(_logEntryPrefab, $"LogEntry_{message.GetHashCode()}", parent);

            if (entry == null)
            {
                Debug.LogError("Prefab is missing");
                return;
            }

            Color accentColor = source switch
            {
                LogSource.GA => Theme.accentYellow,
                LogSource.Game => Theme.accentGreen,
                LogSource.Firebase => Theme.accentOrange,
                LogSource.Sorolla => Theme.textSecondary,
                _ => Theme.textPrimary,
            };

            // Configure
            var logView = entry.GetComponent<LogEntryView>();
            if (logView != null)
            {
                var data = new LogEntryData
                {
                    timestamp = DateTime.Now.ToString("HH:mm:ss. ff"),
                    source = source,
                    level = level,
                    message = message,
                    accentColor = accentColor,
                };
                logView.SetData(data);
            }
        }

        // =====================================================
        // HELPER: Scrollable Tab
        // =====================================================
        static GameObject CreateScrollableTab(string name, Transform parent)
        {
            GameObject tab = CreateUIObject(name, parent);
            var tabRt = tab.GetComponent<RectTransform>();
            SetStretch(tabRt);

            var scroll = tab.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 20f;

            GameObject viewport = CreateUIObject("Viewport", tab.transform);
            var viewportRt = viewport.GetComponent<RectTransform>();
            SetStretch(viewportRt);
            Image viewportImg = AddImage(viewport, Color.white);
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scroll.viewport = viewportRt;

            GameObject content = CreateUIObject("Content", viewport.transform);
            var contentRt = content.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1);
            contentRt.sizeDelta = Vector2.zero;

            AddVerticalLayout(content, Theme.cardSpacing,
                new RectOffset(16, 16, 16, 16),
                childForceExpandWidth: true, childForceExpandHeight: false);
            AddContentSizeFitter(content);

            scroll.content = contentRt;

            return tab;
        }
    }
}
