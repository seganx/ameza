using System;
using Plankton.Proxy;
using UnityEngine;
using System.Collections;

namespace Plankton
{
    public class Ad
    {
        private const string logName = "[Ad]";

        private static ProxyAd proxyAd = null;
        private static bool bannerVisibility = false;
        private static readonly Texture2D bannerTexture = new Texture2D(1, 1);
#if UNITY_EDITOR
        private static float editorFullscreenAd = 0;
        private static Place editorCurrentPlace = null;
#endif

        public static Func<Place, bool> CanShow = null;
        public static bool Banner { get; set; } = false;
        public static bool FreezTimeWhenShowingAd { get; set; } = true;
        public static bool IsOnline => proxyAd == null || proxyAd.IsOnline();

        private static bool IsNotInitialized
        {
            get
            {
                if (proxyAd == null)
                    Debug.Log($"{logName} Feature needs to be initialized first!");
                return proxyAd == null;
            }
        }

        public static void Initialize(Provider bannerProvider, string bannerZoneId, Provider interstitialProvider, string interstitialZoneId, Provider rewardedProvider, string rewardedZoneId)
        {
            if (proxyAd != null)
            {
                Debug.Log($"{logName} Feature already initialized!");
                return;
            }

            proxyAd = new ProxyAd(GetProviderName(bannerProvider), bannerZoneId, GetProviderName(interstitialProvider), interstitialZoneId, GetProviderName(rewardedProvider), rewardedZoneId);
            var mono = new GameObject($"Funtory.{nameof(AdMono)}").AddComponent<AdMono>();
            GameObject.DontDestroyOnLoad(mono);
            bannerTexture.SetPixel(0, 0, Color.black);
            bannerTexture.Apply();
        }

        private static string GetProviderName(Provider provider)
        {
            switch (provider)
            {
                case Provider.Admob: return "admob";
                case Provider.AppLovin: return "applovin";
#if TAPSELL
                case Provider.Tapsell: return "tapsellplus";
#endif
                default: return string.Empty;
            }
        }

        public static bool IsLoaded(Type adType)
        {
            return proxyAd != null && proxyAd.IsReady(AdTypeToString(adType));
        }

        //! try to show rewarded ad. callback will be invoked on ad closed with true for rewarded and false for NOT rewarded/
        public static void ShowRewarded(Place adPlace, Action<bool> onAdClosed = null)
        {
            if (IsNotInitialized)
            {
                onAdClosed?.Invoke(false);
                return;
            }

            if (adPlace.type == Type.Rewarded)
                ShowInternal(adPlace, onAdClosed);
            else
                Debug.LogError($"Type mismatch on Ad.ShowRewarded({adPlace})!");
        }

        //! try to show interstitial ad. callback will be invoked on ad closed.
        public static void ShowInterstitial(Place adPlace, Action onAdClosed = null)
        {
            if (IsNotInitialized)
            {
                onAdClosed?.Invoke();
                return;
            }

            if (adPlace.type == Type.Interstitial)
                ShowInternal(adPlace, result => onAdClosed?.Invoke());
            else
                Debug.LogError($"Type mismatch on Ad.ShowInterstitial({adPlace})!");
        }

        private static void ShowInternal(Place adPlace, Action<bool> callback = null)
        {
            if (CanShowAdPlace(adPlace) == false)
            {
                callback?.Invoke(false);
                return;
            }

            var timescale = Time.timeScale;
            Callbacks.onAdClosed = json =>
            {
                var result = Utils.Jsoner.FromJson(json, new AdClosedResult());
                Time.timeScale = timescale;
                callback?.Invoke(result.displayed == "true" && result.reward_earned == "true");
            };

            if (FreezTimeWhenShowingAd)
                Time.timeScale = 0;

#if UNITY_EDITOR
            editorCurrentPlace = adPlace;
            editorFullscreenAd = 3;
#else
            proxyAd.Show(AdTypeToString(adPlace.type), adPlace.name);
#endif
        }

        private static bool CanShowAdPlace(Place adplace)
        {
            if (CanShow != null)
                return CanShow(adplace);

            switch (adplace.type)
            {
#if PLAY_INSTANT
                case Type.Banner: return Time.time > 180;
                case Type.Interstitial: return false;
#else
                case Type.Banner: return true;
                case Type.Interstitial: return true;
#endif
            }

            return true;
        }

        public static void OpenAdMobDebugger()
        {
            if (IsNotInitialized) return;
            proxyAd.TestAdMob();
        }

        public static void OpenAppLovinMaxDebugger()
        {
            if (IsNotInitialized) return;
            proxyAd.TestMax();
        }

#if UNITY_ANDROID
        public static string GetAndroidAdId()
        {
            if (IsNotInitialized) return string.Empty;
            return proxyAd.GetAndroidAdId();
        }
#endif
        private static Type StringToAdType(string adType)
        {
            switch (adType.ToLower())
            {
                case "interstitial": return Type.Interstitial;
                case "rewarded": return Type.Rewarded;
                case "banner": return Type.Banner;
                default: return Type.Null;
            }
        }

        private static string AdTypeToString(Type adType)
        {
            switch (adType)
            {
                case Type.Interstitial: return "interstitial";
                case Type.Rewarded: return "rewarded";
                case Type.Banner: return "banner";
                default: return string.Empty;
            }
        }

        //////////////////////////////////////////////////////
        /// HELPER CLASSES
        //////////////////////////////////////////////////////
        public enum Type
        {
            Null,
            Interstitial,
            Rewarded,
            Banner,
        }

        public enum Provider
        {
            Admob,
            AppLovin,
#if TAPSELL
            Tapsell
#endif
        }

        [Serializable]
        private class AdClosedResult
        {
            public string displayed = string.Empty;
            public string reward_earned = string.Empty;
        }

        public class Place
        {
            public Type type = 0;
            public string name = string.Empty;

            public Place(Type adType, string placeName)
            {
                type = adType;
                name = placeName;
            }

            public static Place Banner = new Place(Type.Banner, "banner");
        }

        private class AdMono : MonoBehaviour
        {
            private IEnumerator Start()
            {
                var wait = new WaitForSecondsRealtime(0.1f);
                while (true)
                {
                    if (Banner && CanShowAdPlace(Place.Banner))
                    {
                        if (bannerVisibility == false)
                        {
                            bannerVisibility = true;
                            proxyAd.Show("banner", "main_banner");
                        }
                    }
                    else
                    {
                        if (bannerVisibility == true)
                        {
                            bannerVisibility = false;
                            proxyAd.Hide("banner");
                        }
                    }

                    yield return wait;
                }
            }

            private void OnGUI()
            {
#if UNITY_EDITOR
                if (editorCurrentPlace != null && editorFullscreenAd > 0)
                {
                    editorFullscreenAd -= Time.unscaledDeltaTime;
                    var rect = new Rect(0, 0, Screen.width, Screen.height);
                    GUI.Box(rect, $"{editorCurrentPlace.name}\n{editorCurrentPlace.type}");
                    if (editorFullscreenAd <= 0)
                    {
                        editorCurrentPlace = null;
                        var result = new AdClosedResult()
                        {
                            displayed = "true",
                            reward_earned = "true",
                        };

                        Callbacks.onAdClosed?.Invoke(JsonUtility.ToJson(result));
                    }
                }
                else
#endif
                if (bannerVisibility)
                {
                    int heigh = Mathf.RoundToInt(Screen.height * 0.075f);
                    var rect = new Rect(0, Screen.height - heigh, Screen.width, heigh);
                    GUI.DrawTexture(rect, bannerTexture);
                }
            }
        }
    }
}