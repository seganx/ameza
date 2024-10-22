#if UNITY_IOS
using UnityEngine;
using UnityEngine.Scripting;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    [Preserve]
    public class ProxyAnalytics
    {
        private const string logName = "[Plankton] [Analytics]";

        private static class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void InitializeAnalytics();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void SetUserProperty([MarshalAs(UnmanagedType.LPStr)] string providers, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void LogEvent([MarshalAs(UnmanagedType.LPStr)] string providers, [MarshalAs(UnmanagedType.LPStr)] string eventName, [MarshalAs(UnmanagedType.LPStr)] string paramsJson);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void TrackRevenue([MarshalAs(UnmanagedType.LPStr)] string providers, [MarshalAs(UnmanagedType.LPStr)] string eventName, [MarshalAs(UnmanagedType.LPStr)] string currency, double amount);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void SetRemoteConfigMinimumFetchInterval(long interval);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void FetchRemoteConfig();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void GetRemoteConfigValue([Out][MarshalAs(UnmanagedType.LPArray)] char[] destinationBuffer, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string defaultValue);
        }

        public ProxyAnalytics() {
#if !UNITY_EDITOR
            IosProxy.InitializeAnalytics();
#endif
        }

        public void SetUserProperty(string providers, string key, string value)
        {
            Debug.Log($"{logName} SetUserProperty providers:{providers} key:{key} value:{value}");
#if !UNITY_EDITOR
            IosProxy.SetUserProperty(providers, key, value);
#endif
        }

        public void LogEvent(string providers, string eventName, string jsonParams)
        {
#if !UNITY_EDITOR
            IosProxy.LogEvent(providers, eventName, jsonParams);
#endif
        }

        public void TrackRevenue(string providers, string eventName, double amount, string currency)
        {
#if !UNITY_EDITOR
            IosProxy.TrackRevenue(providers, eventName, currency, amount);
#endif
        }

        public string GetAppsFlyerUserId()
        {
            return string.Empty;
        }

        public void SetRemoteConfigMinimumFetchInterval(long interval)
        {
#if !UNITY_EDITOR
            IosProxy.SetRemoteConfigMinimumFetchInterval(interval);
#endif
        }

        public void FetchRemoteConfig()
        {
#if !UNITY_EDITOR
            IosProxy.FetchRemoteConfig();
#endif
        }

        public string GetRemoteConfigValue(string key, string defaultValue)
        {
            var buffer = new char[65536];
#if !UNITY_EDITOR
            IosProxy.GetRemoteConfigValue(buffer, key, defaultValue);
#endif
            var stringBuilder = new System.Text.StringBuilder();
            for (var i = 0; i < buffer.Length && buffer[i] != '\0'; i++)
                stringBuilder.Append(buffer[i]);
            return stringBuilder.ToString();
        }

    }

}

#endif