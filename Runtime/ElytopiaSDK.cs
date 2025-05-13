using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Elytopia
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IGameBridge
    {
        event Action<GameHubCommand, string> OnReceiveEvent;
        event Action<string> PurchaseSuccessEvent;
        event Action<string> PurchaseFailedEvent;
        
        void SendToHub(GameHubCommand command, string values = null);
        void CloseGame();
        Task<ResultContext> ShowInterstitialAdAsync();
        void PurchaseProduct(string productId);
        void SendFpsAnalytics(string data);
    }

    public enum GameHubCommand : byte
    {
        Unknown = 0,
        SystemInitialize = 1,
        CloseGame = 2,
        SaveValue = 3,
        LoadSavableValue = 4,
        ShowInterstitialAd = 5, //result: ResultContext
        ShowRewardedAd = 6, //result: ResultContext
        ShowBannerAd = 7,
        Analytics = 8,
        ThrowExceptionDetected = 9,
        PurchaseExternalProduct = 10, //result: ResultContext
        Ping = 11,
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

#if UNITY_EDITOR && !ELYTOPIA_DISABLE_SDK
        private void Awake() => EditorNamingValidate();
#endif

#if UNITY_WEBGL && !ELYTOPIA_DISABLE_SDK
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
                // Uncomment the line below to get a debug message in the editor 
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

        private void EditorNamingValidate()
        {
            var sdk = GameObject.Find(nameof(ElytopiaSDK));
            if (sdk != null && sdk != gameObject)
                throw new ArgumentException($"Duplicate {sdk.name} name in scene not allowed! Change name or delete gameObject");
        }
        
        private void OnDestroy()
        {
            _habRequestInterstitial?.SetCanceled();
        }

        #region Public

        public event Action<string> PurchaseSuccessEvent;
        public event Action<string> PurchaseFailedEvent;
        
        private TaskCompletionSource<ResultContext> _habRequestInterstitial;

        public void CloseGame() => SendToHub(GameHubCommand.CloseGame);

        public Task<ResultContext> ShowInterstitialAdAsync()
        {
            _habRequestInterstitial?.SetCanceled();
            _habRequestInterstitial = new TaskCompletionSource<ResultContext>();
            SendToHub(GameHubCommand.ShowInterstitialAd);
            return _habRequestInterstitial.Task;
        }

        public void PurchaseProduct(string productId)
        {
            SendToHub(GameHubCommand.PurchaseExternalProduct, productId);
        }

        public void SendFpsAnalytics(string data) => SendToHub(GameHubCommand.Analytics, data);

        private void InternalProcessReceiveFromHub(GameHubCommand command, string values)
        {
            switch (command)
            {
                case GameHubCommand.ShowInterstitialAd:
                    var interstitialResult = values.ToEnum<ResultContext>();
                    _habRequestInterstitial?.SetResult(interstitialResult);
                    _habRequestInterstitial = null;
                    break;
                case GameHubCommand.PurchaseExternalProduct:
                    var json = UnescapeJsonString(values);
                    var purchaseResult = JsonConvert.DeserializeObject<KeyValueContainer<string, ResultContext>>(json);
                    if (purchaseResult?.value == ResultContext.Success)
                        PurchaseSuccessEvent?.Invoke(purchaseResult.key);
                    else
                        PurchaseFailedEvent?.Invoke(purchaseResult?.key);
                    break;
            }
        }

        #endregion
        
        private static string UnescapeJsonString(string input) => input.Replace("\\\"", "\"").Replace("\\\\", "\\");

        [Serializable]
        internal class KeyValueContainer<TKey, TValue>
        {
            public TKey key;
            public TValue value;
        }
    }

    internal static class EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }
    }
}