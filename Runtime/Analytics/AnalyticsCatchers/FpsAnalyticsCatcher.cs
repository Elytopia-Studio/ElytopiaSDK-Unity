#if !ELYTOPIA_DISABLE_FPS_ANALYTICS
using UnityEngine;

namespace Elytopia.Analytics.AnalyticsCatchers
{
    internal class FpsAnalyticsCatcher : MonoBehaviour
    {
        private readonly float _timeBtwRequests = 5f;

        private int _framesCount;
        private int _framesCount20;
        private int _framesCount30;
        private float _time;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            var go = new GameObject();
            go.AddComponent<FpsAnalyticsCatcher>();
            go.name = nameof(ElytopiaSDK) + "FpsCatcher";
            DontDestroyOnLoad(go);
        }

        private void Update()
        {
            _framesCount++;
            if (Time.deltaTime >= 0.033)
            {
                _framesCount30++;
                if (Time.deltaTime >= 0.05)
                {
                    _framesCount20++;
                }
            }

            _time += Time.deltaTime;
            if (_time >= _timeBtwRequests)
            {
                Tick();
            }
        }

        private void Tick()
        {
            _time = 0;

            SendData();
        }

        private void SendData()
        {
            var data = new AnalyticsData.FpsEvent()
            {
                AvgFps = _framesCount / _timeBtwRequests,
                Fps30 = _framesCount30 / _timeBtwRequests,
                Fps20 = _framesCount20 / (float)_framesCount,
            };
            
            ElytopiaSDK.Instance.SendToHub(GameHubCommand.Analytics, JsonUtility.ToJson(data));

            ClearData();
        }

        private void ClearData()
        {
            _framesCount = 0;
            _framesCount20 = 0;
            _framesCount30 = 0;
        }
    }
}
#endif