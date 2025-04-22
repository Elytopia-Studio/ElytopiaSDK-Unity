using System;
using UnityEngine.Scripting;

namespace Elytopia.Analytics.AnalyticsData
{
    [Preserve]
    [Serializable]
    internal class FpsEvent : AnalyticsEventData
    {
        public float AvgFps;
        public float Fps20; //slowFrames20 / totalFrames
        public float Fps30;
    }
}