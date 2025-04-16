using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Elytopia
{
    public interface IGameBridge
    {
        event Action<GameHubCommand, string> OnReceiveEvent;
        void SendToHub(GameHubCommand command, string values = null);
        void CloseGame();
        Task<ResultContext> ShowInterstitialAdAsync();
        void SendFpsAnalytics(string data);
    }

    public enum GameHubCommand : byte
    {
        Unknown = 0,
        SystemInitialize = 1,
        CloseGame = 2,
        SaveValue = 3,
        LoadSavableValue = 4,
        ShowInterstitialAd = 5,
        ShowRewardedAd = 6, //result: ResultContext
        ShowBannerAd = 7,
        Analytics = 8,
        ThrowExceptionDetected = 9,
    }

    public enum ResultContext : byte
    {
        Unknown = 0,
        Started = 1,
        Success = 2,
        Failed = 3,
    }

    public sealed class ElytopiaSDK : MonoBehaviour, IGameBridge
    {
        public event Action<GameHubCommand, string> OnReceiveEvent;

        public static IGameBridge Instance { get; private set; }

#if UNITY_WEBGL && !ELYTOPIA_DISABLE_SDK
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void ElytopiaSDKExplorer(string command, string values);
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSingleton()
        {
#if UNITY_EDITOR
            if (Instance != null)
                Debug.LogError("Initialize With Not null instance");
            if (FindObjectOfType<ElytopiaSDK>() != null)
                Debug.LogError("Singleton Error");
#endif
            if (Instance != null)
                return;

#if !ELYTOPIA_DISABLE_SERIALAZER
            Serialization.JsonProjectSettings.ApplySettings();
#endif
            
#if !ELYTOPIA_DISABLE_SDK
            var bridge = new GameObject().AddComponent<ElytopiaSDK>();
            bridge.name = nameof(ElytopiaSDK);
            Instance = bridge;
            DontDestroyOnLoad(bridge.gameObject);
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Disposer() => Instance = null;

#if  UNITY_WEBGL && !ELYTOPIA_DISABLE_SDK
        private void Start() => SendToHub(GameHubCommand.SystemInitialize);
#endif

        public void SendToHub(GameHubCommand command, string values = null)
        {
            try
            {
                values ??= "null";
#if UNITY_WEBGL && !UNITY_EDITOR && !ELYTOPIA_DISABLE_SDK
                ElytopiaSDKExplorer(command.ToString(), values);
#else
                // Debug.Log($"[{nameof(ElytopiaSDK)}] Send: command={command}; values: {values}");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(ElytopiaSDK)}] Error send! error={e.Message}; command={command}; values: {values}");
            }
        }

        public void OnReceiveFromHub(string eventData)
        {
            var splitMessage = eventData.Split(";");
            var eventCode = splitMessage[0];
            var value = splitMessage[1];

            var eventType = eventCode.ToEnum<GameHubCommand>();
            InternalProcessReceiveFromHub(eventType, value);
            
            OnReceiveEvent?.Invoke(eventType, value);
        }

        public void OnReceiveFromConsole(string exceptionMessage)
        {
            // if (exceptionMessage.Contains("uniwebview"))
            //     return;
            // SendToHub(GameHubCommand.ThrowExceptionDetected, exceptionMessage);
        }

        private void OnDestroy()
        {
            _habRequest?.SetCanceled();
            _habRequest = null;
        }

        #region Public

        private TaskCompletionSource<ResultContext> _habRequest;
        
        public void CloseGame() => SendToHub(GameHubCommand.CloseGame);

        public Task<ResultContext> ShowInterstitialAdAsync()
        {
            _habRequest = new TaskCompletionSource<ResultContext>();
            SendToHub(GameHubCommand.CloseGame);
            return _habRequest.Task;
        }

        public void SendFpsAnalytics(string data) => SendToHub(GameHubCommand.Analytics, data);

        private void InternalProcessReceiveFromHub(GameHubCommand command, string values)
        {
            switch (command)
            {
                case GameHubCommand.ShowInterstitialAd:
                    var result = values.ToEnum<ResultContext>();
                    _habRequest.SetResult(result);
                    _habRequest = null;
                    break;
            }
        }
        #endregion
    }
    
    internal static class EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }
    }
}