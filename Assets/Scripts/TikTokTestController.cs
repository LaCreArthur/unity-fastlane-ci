using System.Collections;
using Sorolla.Palette;
using Sorolla.Palette.Adapters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Test script for validating TikTok Business SDK integration.
///     Attach to a GameObject in your test scene.
///     Requires: Canvas with TMP_Text and Buttons
/// </summary>
public class TikTokTestController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button trackEventButton;
    [SerializeField] Button trackPurchaseButton;
    [SerializeField] Button trackAdRevenueButton;
    [SerializeField] Button simulateIlrdButton;
    [SerializeField] Button trackCustomButton;

    [Header("Test Configuration")]
    [Tooltip("Custom event name for the Track Custom button")]
    [SerializeField]
    string customEventName = "LevelComplete";

    void OnDestroy()
    {
        if (trackEventButton) trackEventButton.onClick.RemoveListener(OnTrackEventClicked);
        if (trackPurchaseButton) trackPurchaseButton.onClick.RemoveListener(OnTrackPurchaseClicked);
        if (trackAdRevenueButton) trackAdRevenueButton.onClick.RemoveListener(OnTrackAdRevenueClicked);
        if (simulateIlrdButton) simulateIlrdButton.onClick.RemoveListener(OnSimulateIlrdClicked);
        if (trackCustomButton) trackCustomButton.onClick.RemoveListener(OnTrackCustomClicked);
    }

    void Start()
    {
        if (trackEventButton) trackEventButton.onClick.AddListener(OnTrackEventClicked);
        if (trackPurchaseButton) trackPurchaseButton.onClick.AddListener(OnTrackPurchaseClicked);
        if (trackAdRevenueButton) trackAdRevenueButton.onClick.AddListener(OnTrackAdRevenueClicked);
        if (simulateIlrdButton) simulateIlrdButton.onClick.AddListener(OnSimulateIlrdClicked);
        if (trackCustomButton) trackCustomButton.onClick.AddListener(OnTrackCustomClicked);

        UpdateStatus("Waiting for Palette to initialize...");
        StartCoroutine(WaitForInit());
    }

    IEnumerator WaitForInit()
    {
        while (!Palette.IsInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }

        SorollaConfig config = Palette.Config;
        string mode = config?.isPrototypeMode == true ? "Prototype" : "Full";
        string appId = config?.tiktokAppId;
        string maskedId = string.IsNullOrEmpty(appId) ? "(empty)" :
            appId.Length > 6 ? appId[..3] + "..." + appId[^3..] : "***";

        string tiktokStatus = string.IsNullOrEmpty(appId)
            ? "TikTok: Skipped (no App ID)"
            : $"TikTok: Initialized (appId: {maskedId})";

        UpdateStatus("Palette Initialized!\n" +
                     $"Mode: {mode}\n" +
                     $"{tiktokStatus}\n" +
                     $"Has Consent: {Palette.HasConsent}");
    }

    void OnTrackEventClicked()
    {
        TikTokAdapter.TrackEvent("TestEvent");
        UpdateStatus("TrackEvent: TestEvent\nCheck logcat for [Palette:TikTok]");
    }

    void OnTrackPurchaseClicked()
    {
        TikTokAdapter.TrackPurchase(0.99);
        UpdateStatus("TrackPurchase: $0.99 USD\nCheck logcat for [Palette:TikTok]");
    }

    void OnTrackAdRevenueClicked()
    {
        TikTokAdapter.TrackAdRevenue(0.01);
        UpdateStatus("TrackAdRevenue: $0.01 USD\nCheck logcat for [Palette:TikTok]");
    }

    void OnSimulateIlrdClicked()
    {
        // Simulates what MAX ILRD handler does — calls TrackAdRevenue directly
        TikTokAdapter.TrackAdRevenue(0.01);
        UpdateStatus("Simulate ILRD: TrackAdRevenue $0.01 USD\n(Same path as MAX ILRD forwarding)");
    }

    void OnTrackCustomClicked()
    {
        if (string.IsNullOrEmpty(customEventName))
        {
            UpdateStatus("No custom event name configured!\nSet it in the Inspector.");
            return;
        }

        TikTokAdapter.TrackEvent(customEventName);
        UpdateStatus($"TrackEvent: {customEventName}\nCheck logcat for [Palette:TikTok]");
    }

    void UpdateStatus(string message)
    {
        Debug.Log($"[TikTokTest] {message}");
        if (statusText)
            statusText.text = message;
    }
}
