using UnityEngine;
using UnityEngine.UI;

namespace Sorolla.DebugUI
{
    /// <summary>
    ///     Controls the Events tab buttons. Self-sufficient - calls Sorolla tracking API directly.
    /// </summary>
    public class EventsTabController : UIComponentBase
    {
        [Header("Progression Buttons")]
        [SerializeField] Button _startButton;
        [SerializeField] Button _winButton;
        [SerializeField] Button _failButton;

        [Header("Resource Buttons")]
        [SerializeField] Button _addCoinsButton;
        [SerializeField] Button _spendCoinsButton;

        [Header("Custom Event Buttons")]
        [SerializeField] Button _jumpButton;
        [SerializeField] Button _npcTalkButton;

        void Awake()
        {

            // Progression
            _startButton.onClick.AddListener(() => TrackProgression(ProgressionStatus.Start));
            _winButton.onClick.AddListener(() => TrackProgression(ProgressionStatus.Complete));
            _failButton.onClick.AddListener(() => TrackProgression(ProgressionStatus.Fail));

            // Resources
            _addCoinsButton.onClick.AddListener(() => TrackResource(ResourceFlowType.Source, "coins", 100));
            _spendCoinsButton.onClick.AddListener(() => TrackResource(ResourceFlowType.Sink, "coins", 50));

            // Custom
            _jumpButton.onClick.AddListener(() => TrackDesign("player_jump"));
            _npcTalkButton.onClick.AddListener(() => TrackDesign("npc_talk"));
        }

        void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
            _winButton.onClick.RemoveAllListeners();
            _failButton.onClick.RemoveAllListeners();
            _addCoinsButton.onClick.RemoveAllListeners();
            _spendCoinsButton.onClick.RemoveAllListeners();
            _jumpButton.onClick.RemoveAllListeners();
            _npcTalkButton.onClick.RemoveAllListeners();
        }

        void TrackProgression(ProgressionStatus status)
        {
            string levelName = "Level_01";
            Sorolla.TrackProgression(status, levelName);

            string statusName = status.ToString();
            DebugPanelManager.Instance?.Log($"Progression: {statusName} ({levelName})", LogSource.GA);
            SorollaDebugEvents.RaiseShowToast($"Tracked: {statusName}", ToastType.Success);
        }

        void TrackResource(ResourceFlowType flowType, string currency, float amount)
        {
            Sorolla.TrackResource(flowType, currency, amount, "debug", "test_item");

            string action = flowType == ResourceFlowType.Source ? "+" : "-";
            DebugPanelManager.Instance?.Log($"Resource: {action}{amount} {currency}", LogSource.GA);
            SorollaDebugEvents.RaiseShowToast($"{action}{amount} {currency}", ToastType.Info);
        }

        void TrackDesign(string eventName)
        {
            Sorolla.TrackDesign(eventName);

            DebugPanelManager.Instance?.Log($"Design Event: {eventName}", LogSource.GA);
            SorollaDebugEvents.RaiseShowToast($"Tracked: {eventName}", ToastType.Success);
        }
    }

}
