using System;
using UnityEngine;

namespace Sorolla.DebugUI
{
    // Enums
    public enum ToastType { Info, Success, Warning, Error }
    public enum AdType { Interstitial, Rewarded, Banner }
    public enum AdStatus { Idle, Loading, Loaded, Showing, Failed }
    public enum SorollaMode { Prototype, Full }
    public enum LogSource { UI, GA, Game, Sorolla, Firebase }
    public enum LogLevel { All, Info, Warning, Error }

    // Data structures
    [Serializable]
    public struct LogEntryData
    {
        public string timestamp;
        public LogSource source;
        public LogLevel level;
        public string message;
        public Color accentColor;
    }
}
