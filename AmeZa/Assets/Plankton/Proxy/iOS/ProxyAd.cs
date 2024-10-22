#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class ProxyAd
    {
        private const string logName = "[Plankton] [Ad]";

        public ProxyAd(string bannerProvider, string bannerZoneId, string interstitialProvider, string intrestitialZoneId, string rewardedProvider, string rewardedZoneId)
        {
            Utils.Jsoner.AddParams("bannerProvider", bannerProvider, "bannerZoneId", bannerZoneId, "interstitialProvider", interstitialProvider, "interstitialZoneId", intrestitialZoneId, "rewardedProvider", rewardedProvider, "rewardedZoneId", rewardedZoneId);
            var json = Utils.Jsoner.GetJsonAndClear();
            Debug.Log($"{logName} Initializing with json:{json}");
#if !UNITY_EDITOR
            IosProxy.InitializeAds(json);
#endif
        }

        public bool IsOnline()
        {
#if !UNITY_EDITOR
            return IosProxy.IsOnline();
#else
            return true;
#endif
        }

        // Available types: banner, interstitial, rewarded
        public bool IsReady(string type)
        {
#if !UNITY_EDITOR
            return IosProxy.IsAdReady(type);
#else
            return true;
#endif
        }

        // Available types: banner, interstitial, rewarded
        public void Show(string type, string placement)
        {
#if !UNITY_EDITOR
            IosProxy.ShowAd(type, placement);
#endif
        }

        // Available types: banner
        public void Hide(string type)
        {
#if !UNITY_EDITOR
            IosProxy.HideAd(type);
#endif
        }

        public void TestAdMob()
        {
#if !UNITY_EDITOR
            IosProxy.ShowAdMobDebugger();
#endif
        }

        public void TestMax()
        {
#if !UNITY_EDITOR
            IosProxy.ShowMaxDebugger();
#endif
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void InitializeAds([MarshalAs(UnmanagedType.LPStr)] string json);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern bool IsOnline();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern bool IsAdReady([MarshalAs(UnmanagedType.LPStr)] string type);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void ShowAd([MarshalAs(UnmanagedType.LPStr)] string type, [MarshalAs(UnmanagedType.LPStr)] string placement);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void HideAd([MarshalAs(UnmanagedType.LPStr)] string type);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void ShowAdMobDebugger();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void ShowMaxDebugger();
        }
    }
}

#endif