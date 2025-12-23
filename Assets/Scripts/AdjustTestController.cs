using System.Collections;
using Sorolla;
using Sorolla.Adapters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Test script for validating Adjust SDK integration.
///     Attach to a GameObject in your test scene.
///     Requires: Canvas with TMP_Text and Buttons
/// </summary>
public class AdjustTestController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_Text attributionText;
    [SerializeField] Button getAdidButton;
    [SerializeField] Button getAttributionButton;
    [SerializeField] Button trackEventButton;
    [SerializeField] Button trackRevenueButton;

    [Header("Test Configuration")]
    [Tooltip("Event token from Adjust Dashboard (create one for testing)")]
    [SerializeField]
    string testEventToken = "";

    [Tooltip("Revenue event token from Adjust Dashboard")]
    [SerializeField]
    string revenueEventToken = "";

    string _adid = "Not retrieved yet";
    string _attribution = "Not retrieved yet";

    void OnDestroy()
    {
        if (getAdidButton) getAdidButton.onClick.RemoveListener(OnGetAdidClicked);
        if (getAttributionButton) getAttributionButton.onClick.RemoveListener(OnGetAttributionClicked);
        if (trackEventButton) trackEventButton.onClick.RemoveListener(OnTrackEventClicked);
        if (trackRevenueButton) trackRevenueButton.onClick.RemoveListener(OnTrackRevenueClicked);
    }

    void Start()
    {
        // Setup button listeners
        if (getAdidButton) getAdidButton.onClick.AddListener(OnGetAdidClicked);
        if (getAttributionButton) getAttributionButton.onClick.AddListener(OnGetAttributionClicked);
        if (trackEventButton) trackEventButton.onClick.AddListener(OnTrackEventClicked);
        if (trackRevenueButton) trackRevenueButton.onClick.AddListener(OnTrackRevenueClicked);

        // Initial status
        UpdateStatus("Waiting for Sorolla to initialize...");

        // Wait for initialization then update
        StartCoroutine(WaitForInit());
    }

    IEnumerator WaitForInit()
    {
        while (!SorollaSDK.IsInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }

        SorollaConfig config = SorollaSDK.Config;
        string mode = config != null && config.adjustSandboxMode ? "SANDBOX" : "PRODUCTION";

        UpdateStatus("✅ Sorolla Initialized!\n" +
                     $"Mode: {(config?.isPrototypeMode == true ? "Prototype" : "Full")}\n" +
                     $"Adjust Environment: {mode}\n" +
                     $"Has Consent: {SorollaSDK.HasConsent}");

        // Auto-fetch ADID on init
        OnGetAdidClicked();
    }

    void OnGetAdidClicked()
    {
        UpdateStatus("Fetching Adjust Device ID (ADID)...");

        AdjustAdapter.GetAdid(adid =>
        {
            _adid = string.IsNullOrEmpty(adid) ? "NULL (not available)" : adid;
            UpdateStatus($"✅ ADID Retrieved:\n{_adid}");
        });
    }

    void OnGetAttributionClicked()
    {
        UpdateStatus("Fetching Attribution data...");

        AdjustAdapter.GetAttribution(attribution =>
        {
            if (attribution == null)
            {
                _attribution = "NULL (no attribution yet)";
            }
            else
            {
                #if SOROLLA_ADJUST_ENABLED
                _attribution = $"Network: {attribution.Network ?? "N/A"}\n" +
                               $"Campaign: {attribution.Campaign ?? "N/A"}\n" +
                               $"Adgroup: {attribution.Adgroup ?? "N/A"}\n" +
                               $"Creative: {attribution.Creative ?? "N/A"}\n" +
                               $"Tracker: {attribution.TrackerName ?? "N/A"}";
                #endif
            }

            if (attributionText)
                attributionText.text = $"Attribution:\n{_attribution}";

            UpdateStatus("✅ Attribution Retrieved");
        });
    }


    void OnTrackEventClicked()
    {
        if (string.IsNullOrEmpty(testEventToken))
        {
            UpdateStatus("❌ No test event token configured!\n" +
                         "Create an event in Adjust Dashboard and\n" +
                         "paste the token in the inspector.");
            return;
        }

        AdjustAdapter.TrackEvent(testEventToken);
        UpdateStatus($"✅ Event Tracked!\nToken: {testEventToken}\n\nCheck Adjust Testing Console.");
    }

    void OnTrackRevenueClicked()
    {
        if (string.IsNullOrEmpty(revenueEventToken))
        {
            UpdateStatus("❌ No revenue event token configured!\n" +
                         "Create a revenue event in Adjust Dashboard.");
            return;
        }

        // Track a test revenue event ($0.99 USD)
        AdjustAdapter.TrackRevenue(revenueEventToken, 0.99);
        UpdateStatus($"✅ Revenue Tracked!\nToken: {revenueEventToken}\n" +
                     "Amount: $0.99 USD\n\nCheck Adjust Testing Console.");
    }

    void UpdateStatus(string message)
    {
        Debug.Log($"[AdjustTest] {message}");
        if (statusText)
            statusText.text = message;
    }
}
