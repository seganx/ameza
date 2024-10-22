using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;


namespace Plankton.Proxy
{
    [Preserve]
    public class Callbacks : MonoBehaviour
    {
        private const string logName = "[Plankton] [Callback]";

        private void Update()
        {
            lock (unityThreadActions)
            {
                for (int i = 0; i < unityThreadActions.Count; i++)
                {
                    try
                    {
                        unityThreadActions[i]?.Invoke();
                    }
                    catch { }
                }
                unityThreadActions.Clear();
            }
        }

#pragma warning disable IDE0051 // unused private members

        [Preserve]
        private void OnAdClosed(string json)
        {
            Debug.Log($"{logName} OnAdClosed : {json}");
            CallInUnityThread(() =>
            {
                onAdClosed?.Invoke(json);
                onAdClosed = null;
            });
        }

        [Preserve]
        private void OnRemoteConfigFetched(string json)
        {
            Debug.Log($"{logName} OnRemoteConfigFetched : {json}");
            CallInUnityThread(() =>
            {
                onRemoteConfigFetched?.Invoke(json);
                onRemoteConfigFetched = null;
            });
        }

        [Preserve]
        private void OnRemoteConfigFetchFailed(string empty)
        {
            Debug.Log($"{logName} OnRemoteConfigFetchFailed : {empty}");
            CallInUnityThread(() =>
            {
                onRemoteConfigFailed?.Invoke();
                onRemoteConfigFailed = null;
            });
        }

        [Preserve]
        private void OnSignInResult(string json)
        {
            Debug.Log($"{logName} OnSignInResult : {json}");
            CallInUnityThread(() =>
            {
                onPlayServiceSignInResult?.Invoke(json);
                onPlayServiceSignInResult = null;
            });
        }

        [Preserve]
        private void OnSignOutResult(string json)
        {
            Debug.Log($"{logName} OnSignOutResult : {json}");
            CallInUnityThread(() =>
            {
                onPlayServiceSignOutResult?.Invoke(json);
                onPlayServiceSignOutResult = null;
            });
        }

        [Preserve]
        private void OnSaveResult(string boolean)
        {
            Debug.Log($"{logName} OnSaveResult : {boolean}");
            CallInUnityThread(() =>
            {
                onPlayServiceSaveResult?.Invoke(boolean.ToLower() == "true");
                onPlayServiceSaveResult = null;
            });
        }

        [Preserve]
        private void OnLoadResult(string data)
        {
            Debug.Log($"{logName} OnLoadResult : {data}");
            CallInUnityThread(() =>
            {
                onPlayServiceLoadData?.Invoke(data);
                onPlayServiceLoadData = null;
            });
        }

        [Preserve]
        private void OnRateAppResult(string boolean)
        {
            Debug.Log($"{logName} OnRateAppResult : {boolean}");
            CallInUnityThread(() =>
            {
                onRateAppResult?.Invoke(boolean.ToLower() == "true");
                onRateAppResult = null;
            });
        }

        [Preserve]
        private void OnPurchaseResult(string json)
        {
            Debug.Log($"{logName} OnPurchaseResult : {json}");
            CallInUnityThread(() =>
            {
                onPurchaseResult?.Invoke(json);
                onPurchaseResult = null;
            });
        }

        [Preserve]
        private void OnSkuDetailsResult(string json)
        {
            Debug.Log($"{logName} OnSkuDetailsResult : {json}");
            CallInUnityThread(() =>
            {
                onSkuDetailsResult?.Invoke(json);
                onSkuDetailsResult = null;
            });
        }

        [Preserve]
        private void OnPurchasesResult(string json)
        {
            Debug.Log($"{logName} OnPurchaseHistoryResult : {json}");
            CallInUnityThread(() =>
            {
                onPurchaseHistoryResult?.Invoke(json);
                onPurchaseHistoryResult = null;
            });
        }

        [Preserve]
        private void OnLogReceived(string log)
        {
            if (string.IsNullOrEmpty(log)) return;

            //if (log[0] == 'E')
            //    SeganX.Console.AddLog(log, Color.red * Color.gray);
            //else if (log[0] == 'W')
            //    SeganX.Console.AddLog(log, Color.yellow * Color.gray);
            //else
            //    SeganX.Console.AddLog(log, Color.gray);
        }

        [Preserve]
        private void OnGameAnalyticsInitialized()
        {
            Debug.Log($"{logName} OnGameAnalyticsInitialized");
            CallInUnityThread(() =>
            {
                onGameAnalyticsInitialized?.Invoke();
                onGameAnalyticsInitialized = null;
            });
        }

        [Preserve]
        private void OnBillingInitializationResult(string json)
        {
            Debug.Log($"{logName} OnBillingInitializationResult : {json}");
            CallInUnityThread(() =>
            {
                onBillingInitializationResult?.Invoke(json);
                onBillingInitializationResult = null;
            });
        }

        [Preserve]
        private void OnConsumeOrAcknowledgeResult(string json)
        {

            Debug.Log($"{logName} OnConsumeOrAcknowledgeResult : {json}");
            CallInUnityThread(() =>
            {
                onConsumeOrAcknowledgeResult?.Invoke(json);
                onConsumeOrAcknowledgeResult = null;
            });
        }

        [Preserve]
        private void OnRequestConsentInfoUpdateResult(string json)
        {

            Debug.Log($"{logName} OnRequestConsentInfoUpdateResult : {json}");
            CallInUnityThread(() =>
            {
                onRequestConsentInfoUpdateResult?.Invoke(json);
                onRequestConsentInfoUpdateResult = null;
            });
        }

        [Preserve]
        private void OnConsentFormDismissed(string json)
        {

            Debug.Log($"{logName} OnConsentFormDismissed : {json}");
            CallInUnityThread(() =>
            {
                onConsentFormDismissed?.Invoke(json);
                onConsentFormDismissed = null;
            });
        }

        [Preserve]
        private void OnRestorePurchasesSuccess(string json)
        {
            Debug.Log($"{logName} OnRestorePurchasesSuccess : {json}");
            CallInUnityThread(() => onRestorePurchasesSuccess?.Invoke(json));
        }

        [Preserve]
        private void OnFcmTokenReceived(string json)
        {
            Debug.Log($"{logName} OnFcmTokenReceived : {json}");
            CallInUnityThread(() => onFcmTokenReceived?.Invoke(json));
        }
        
        [Preserve]
        private void OnFcmMessageReceived(string json)
        {
            Debug.Log($"{logName} OnFcmMessageReceived : {json}");
            CallInUnityThread(() => onFcmMessageReceived?.Invoke(json));
        }



#pragma warning restore IDE0051 // unused private members


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static readonly List<Action> unityThreadActions = new List<Action>(16);

        public static Action<string> onAdClosed = null;
        public static Action<string> onRemoteConfigFetched = null;
        public static Action onRemoteConfigFailed = null;
        public static Action<string> onPlayServiceSignInResult = null;
        public static Action<string> onPlayServiceSignOutResult = null;
        public static Action<bool> onPlayServiceSaveResult = null;
        public static Action<string> onPlayServiceLoadData = null;
        public static Action<bool> onRateAppResult = null;
        public static Action<string> onPurchaseResult = null;
        public static Action<string> onSkuDetailsResult = null;
        public static Action<string> onPurchaseHistoryResult = null;
        public static Action onGameAnalyticsInitialized = null;
        public static Action<string> onBillingInitializationResult = null;
        public static Action<string> onConsumeOrAcknowledgeResult = null;
        public static Action<string> onRequestConsentInfoUpdateResult = null;
        public static Action<string> onConsentFormDismissed = null;
        public static Action<string> onRestorePurchasesSuccess = null;
        public static Action<string> onFcmTokenReceived = null;
        public static Action<string> onFcmMessageReceived = null;

        public static void CallInUnityThread(Action action)
        {
            lock (unityThreadActions)
            {
                unityThreadActions.Add(action);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeInitialize()
        {
            var mono = new GameObject("Plankton", typeof(Callbacks));
            DontDestroyOnLoad(mono);

#if UNITY_EDITOR
#else
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#endif
        }
    }
}
