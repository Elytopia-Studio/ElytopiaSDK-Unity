using System;
using UnityEngine.Scripting;

namespace Elytopia.Analytics
{
    [Preserve]
    [Serializable]
    public class AnalyticsEventData
    {
        [Preserve] public AnalyticsDataType Type;
    }
}