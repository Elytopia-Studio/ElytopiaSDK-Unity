using System;

namespace Elytopia.Analytics.AnalyticsData
{
    [Serializable]
    internal class FpsEvent : AnalyticsEventData
    {
        public float AvgFps;
        public float Fps20; //slowFrames20 / totalFrames
        public float Fps30;
    }
}