using UnityEngine.UI;
using TMPro;
using Sorolla;
using Sorolla.Adapters;

/// <summary>
///     Test script for validating Adjust SDK integration.
///     Attach to a GameObject in your test scene.
///     Requires: Canvas with TMP_Text and Buttons
/// </summary>
public class AdjustTestController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text attributionText;
    [SerializeField] private Button getAdidButton;
    [SerializeField] private Button getAttributionButton;
    [SerializeField] private Button trackEventButton;
    [SerializeField] private Button trackRevenueButton;

    [Header("Test Configuration")]
    [Tooltip("Event token from Adjust Dashboard (create one for testing)")]
    [SerializeField] private string testEventToken = "";
    
    [Tooltip("Revenue event token from Adjust Dashboard")]
    [SerializeField] private string revenueEventToken = "";

    private string _adid = "Not retrieved yet";
    private string _attribution = "Not retrieved yet";

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

    private System.Collections.IEnumerator WaitForInit()
    {
        while (!Sorolla.Sorolla.IsInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }

        var config = Sorolla.Sorolla.Config;
        var mode = config != null && config.adjustSandboxMode ? "SANDBOX" : "PRODUCTION";
        
        UpdateStatus($"✅ Sorolla Initialized!\n" +
                     $"Mode: {(config?.isPrototypeMode == true ? "Prototype" : "Full")}\n" +
                     $"Adjust Environment: {mode}\n" +
                     $"Has Consent: {Sorolla.Sorolla.HasConsent}");
        
        // Auto-fetch ADID on init
        OnGetAdidClicked();
    }

    private void OnGetAdidClicked()
    {
        UpdateStatus("Fetching Adjust Device ID (ADID)...");
        
        AdjustAdapter.GetAdid(adid =>
        {
            _adid = string.IsNullOrEmpty(adid) ? "NULL (not available)" : adid;
            UpdateStatus($"✅ ADID Retrieved:\n{_adid}");
        });
    }

    private void OnGetAttributionClicked()
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
                _attribution = $"Network: {attribution.Network ?? "N/A"}\n" +
                              $"Campaign: {attribution.Campaign ?? "N/A"}\n" +
                              $"Adgroup: {attribution.Adgroup ?? "N/A"}\n" +
                              $"Creative: {attribution.Creative ?? "N/A"}\n" +
                              $"Tracker: {attribution.TrackerName ?? "N/A"}";
            }
            
            if (attributionText) 
                attributionText.text = $"Attribution:\n{_attribution}";
            
            UpdateStatus($"✅ Attribution Retrieved");
        });
    }


    private void OnTrackEventClicked()
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

    private void OnTrackRevenueClicked()
    {
        if (string.IsNullOrEmpty(revenueEventToken))
        {
            UpdateStatus("❌ No revenue event token configured!\n" +
                        "Create a revenue event in Adjust Dashboard.");
            return;
        }

        // Track a test revenue event ($0.99 USD)
        AdjustAdapter.TrackRevenue(revenueEventToken, 0.99, "USD");
        UpdateStatus($"✅ Revenue Tracked!\nToken: {revenueEventToken}\n" +
                    $"Amount: $0.99 USD\n\nCheck Adjust Testing Console.");
    }

    private void UpdateStatus(string message)
    {
        Debug.Log($"[AdjustTest] {message}");
        if (statusText) 
            statusText.text = message;
    }

    void OnDestroy()
    {
        if (getAdidButton) getAdidButton.onClick.RemoveListener(OnGetAdidClicked);
        if (getAttributionButton) getAttributionButton.onClick.RemoveListener(OnGetAttributionClicked);
        if (trackEventButton) trackEventButton.onClick.RemoveListener(OnTrackEventClicked);
        if (trackRevenueButton) trackRevenueButton.onClick.RemoveListener(OnTrackRevenueClicked);
    }
}
