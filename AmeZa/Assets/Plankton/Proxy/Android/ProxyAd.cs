#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Plankton.Proxy
{
    public class ProxyAd : Proxy
    {
        private const string logName = "[Plankton] [Ad]";

        private readonly AndroidJavaObject objectAdProvider = null;

        public ProxyAd(string bannerProvider, string bannerZoneId, string interstitialProvider, string intrestitialZoneId, string rewardedProvider, string rewardedZoneId)
        {
            Utils.Jsoner.AddParams("bannerProvider", bannerProvider, "bannerZoneId", bannerZoneId, "interstitialProvider", interstitialProvider, "interstitialZoneId", intrestitialZoneId, "rewardedProvider", rewardedProvider, "rewardedZoneId", rewardedZoneId);
            var json = Utils.Jsoner.GetJsonAndClear();
            Debug.Log($"{logName} Initializing with json:{json}");
            objectAdProvider = GetAndroidObject("getAd");
#if !UNITY_EDITOR
            if (objectAdProvider != null)
            {
                objectAdProvider.Call("initialize", json);
                Debug.Log($"{logName} Called initialization");
            }
            else Debug.LogWarning($"{logName} objectAdProvider is null!");
#endif
        }

        public bool IsOnline()
        {
            if (objectAdProvider != null)
                return objectAdProvider.Call<bool>("isOnline");
            else
                return true;
        }

        // Available types: banner, interstitial, rewarded
        public bool IsReady(string type)
        {
            return objectAdProvider != null && objectAdProvider.Call<bool>("isReady", type);
        }

        // Available types: banner, interstitial, rewarded
        public void Show(string type, string placement)
        {
            Debug.Log($"{logName} Show type:{type} placement:{placement}");
            objectAdProvider?.Call("show", type, placement);
        }

        // Available types: banner
        public void Hide(string type)
        {
            Debug.Log($"{logName} Hide type:{type}");
            objectAdProvider?.Call("hide", type);
        }

        public void TestAdMob()
        {
            Debug.Log($"{logName} TestAdMob");
            objectAdProvider?.Call("showAdMobDebugger");
        }

        public void TestMax()
        {
            Debug.Log($"{logName} TestMax");
            objectAdProvider?.Call("showMaxDebugger");
        }

        public string GetAndroidAdId()
        {
            string adId = objectAdProvider?.Call<string>("getAndroidAdId");
            Debug.Log($"{logName} Getting Android AdID: {adId}");
            return adId;
        }
    }
}
#endif