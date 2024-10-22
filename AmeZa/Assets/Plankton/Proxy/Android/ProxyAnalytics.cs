#if UNITY_ANDROID
using UnityEngine;

namespace Plankton.Proxy
{
    public class ProxyAnalytics : Proxy
    {
        private const string logName = "[Plankton] [Analytics]";

        private readonly AndroidJavaObject objectAnalytics = null;
        private readonly AndroidJavaObject objectRemoteConfig = null;

        public ProxyAnalytics()
        {
            Debug.Log($"{logName} Getting android objects...");
            objectAnalytics = GetAndroidObject("getAnalytics");
            objectRemoteConfig = GetAndroidObject("getRemoteConfig");
#if !UNITY_EDITOR
            if (objectAnalytics == null) Debug.LogWarning($"{logName} objectAnalytics is null!");
            if (objectRemoteConfig == null) Debug.LogWarning($"{logName} objectRemoteConfig is null!");
#endif
            Debug.Log($"{logName} Collected android objects objectAnalytics:{objectAnalytics} objectRemoteConfig:{objectRemoteConfig}");
        }

        public void SetUserProperty(string providers, string key, string value)
        {
            objectAnalytics?.Call("setUserProperty", providers, key, value);
        }

        public void LogEvent(string providers, string eventName, string jsonParams)
        {
            objectAnalytics?.Call("logEvent", providers, eventName, jsonParams);
        }

        public void TrackRevenue(string providers, string eventName, double amount, string currency)
        {
            objectAnalytics?.Call("trackRevenue", providers, eventName, amount, currency);
        }

        public string GetAppsFlyerUserId()
        {
            return objectAnalytics?.Call<string>("getAppsFlyerUID");
        }

        public void SetRemoteConfigMinimumFetchInterval(long interval)
        {
            objectRemoteConfig?.Call("setRemoteConfigMinimumFetchInterval", interval);
        }

        public void FetchRemoteConfig()
        {
            objectRemoteConfig?.Call("fetchRemoteConfig");
        }

        public string GetRemoteConfigValue(string key, string defaultValue)
        {
            return objectRemoteConfig?.Call<string>("getRemoteConfigValue", key, defaultValue);
        }
    }
}
#endif