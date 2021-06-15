//////////////////////////////////////////////////////
/// Funtory infrastructure class
/// !! DO NOT CHANGE THIS FILE !!
//////////////////////////////////////////////////////
using SeganX;
using System.Collections;
using UnityEngine;

#if ABR || TAPSELL
using TapsellPlusSDK;
#endif

//////////////////////////////////////////////////////
/// Funtory infrastructure class
/// !! DO NOT CHANGE THIS FILE !!
//////////////////////////////////////////////////////
public static class FunAd
{
    public delegate bool OnPreRequestCallback(Segment segment);

    public abstract class Segment
    {
        public string ResponseZone { get; set; }
        public bool IsReady => ResponseZone.HasContent();
        public abstract string ZoneId { get; }
    }

    public class NativeData
    {
        public string title = null;
        public string description = null;
        public string callToActionText = null;
        public Sprite portraitBannerImage = null;
        public Sprite landscapeBannerImage = null;
        public Sprite iconImage = null;
        public System.Action onClick = null;
    }

    public class AdMono : MonoBehaviour
    {
        public void Call(System.Action<bool> callback, bool arg = true)
        {
            if (callback != null) StartCoroutine(DoDelayCall(callback, arg));
        }

        private IEnumerator DoDelayCall(System.Action<bool> callback, bool arg)
        {
            yield return new WaitForSeconds(0.1f);
            callback?.Invoke(arg);
        }
    }


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    /// Funtory infrastructure class
    /// !! DO NOT CHANGE THIS FILE !!
    //////////////////////////////////////////////////////
    private static AdMono admono = null;
    public static OnPreRequestCallback OnPreRequest = segment => true;
    public static System.Action<Segment> OnError = null;
    public static System.Action<Segment> OnLoaded = null;
    public static System.Action<Segment> OnOpen = null;
    public static System.Action<Segment> OnClose = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnRuntimeInitialized()
    {
        Object.DontDestroyOnLoad(admono = new GameObject("FunAd").AddComponent<AdMono>());
    }

    public static class Rewarded
    {
        public static void Request(Segment segment, System.Action<bool> callback)
        {
            var zone = segment.ZoneId;
            Debug.Log("FunAd.Rewarded.Request: " + zone);

            if (zone.IsNullOrEmpty())
            {
                admono.Call(callback, false);
                return;
            }

            if (segment.IsReady)
            {
                admono.Call(callback, true);
                return;
            }

#if ABR || TAPSELL
            TapsellPlus.RequestRewardedVideoAd(zone,
                zoneId =>
                {
                    segment.ResponseZone = zoneId.zoneId;
                    callback?.Invoke(true);
                    OnLoaded?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("Tapsell.RequestRewardedVideo.Error: " + error.message);
                    callback?.Invoke(false);
                    OnError?.Invoke(segment);
                });
#elif UAD
            UnityAd.Load(zone, success =>
            {
                if (success)
                {
                    segment.ResponseZone = zone;
                    callback?.Invoke(true);
                    OnLoaded?.Invoke(segment);
                }
                else
                {
                    OnError?.Invoke(segment);
                    callback?.Invoke(false);
                }
            });
#elif ADMOB
            AdMob.RequestRewarded(zone,
                zid =>
                {
                    segment.ResponseZone = zone;
                    callback?.Invoke(true);
                    OnLoaded?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("AdMob.RequestRewarded.Error: " + error);
                    callback?.Invoke(false);
                    OnError?.Invoke(segment);
                });
#else
            callback?.Invoke(false);
#endif
        }

        public static void Show(Segment segment, System.Action<bool> callback)
        {
            if (segment.IsReady)
            {
#if ABR || TAPSELL
                TapsellPlus.ShowRewardedVideoAd(
                    segment.ResponseZone,
                    zid =>
                    {
                        Debug.Log("Tapsell.ShowAd.OnOpenAd: " + zid);
                        OnOpen?.Invoke(segment);
                    },
                    zid =>
                    {
                        Debug.Log("Tapsell.ShowAd.OnRewarded: " + zid);
                        if (segment.ResponseZone.HasContent())
                        {
                            segment.ResponseZone = null;
                            callback?.Invoke(true);
                            OnClose?.Invoke(segment);
                        }
                    },
                    zid =>
                    {
                        Debug.Log("Tapsell.ShowAd.OnCloseAd: " + zid);
                        if (segment.ResponseZone.HasContent())
                        {
                            segment.ResponseZone = null;
                            callback?.Invoke(false);
                            OnClose?.Invoke(segment);
                        }
                    },
                    error =>
                    {
                        Debug.Log("AdMob.ShowAd.Error: " + error);
                        if (segment.ResponseZone.HasContent())
                        {
                            segment.ResponseZone = null;
                            callback?.Invoke(false);
                            OnError?.Invoke(segment);
                        }
                    });
#else
            callback?.Invoke(false);
#endif
            }
        }
    }

    public static class Interstitial
    {
        public static void Request(Segment segment, System.Action<bool> callback = null)
        {
            if (OnPreRequest(segment) == false) return;

            var zone = segment.ZoneId;
            Debug.Log("FunAd.Interstitial.Request: " + zone);

            if (zone.IsNullOrEmpty())
            {
                admono.Call(callback, false);
                return;
            }

            if (segment.IsReady)
            {
                admono.Call(callback, true);
                return;
            }

#if ABR || TAPSELL
            TapsellPlus.RequestInterstitialAd(zone,
                zoneId =>
                {
                    segment.ResponseZone = zoneId.zoneId;
                    callback?.Invoke(true);
                    OnLoaded?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("Tapsell.RequestInterstitial.Error: " + error.message);
                    callback?.Invoke(false);
                    OnError?.Invoke(segment);
                });
#elif UAD
            UnityAd.Load(zone, success =>
            {
                if (success)
                {
                    segment.ResponseZone = zone;
                    callback?.Invoke(true);
                    OnLoaded?.Invoke(segment);
                }
                else
                {
                    OnError?.Invoke(segment);
                    callback?.Invoke(false);
                }
            });
#elif ADMOB
            AdMob.RequestInterstitial(zone,
                zid =>
                {
                    segment.ResponseZone = zone;
                    callback?.Invoke(true);
                    OnLoaded?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("AdMob.RequestInterstitial.Error: " + error);
                    callback?.Invoke(false);
                    OnError?.Invoke(segment);
                });
#else
            callback?.Invoke(false);
#endif
        }

        public static void Show(Segment segment, System.Action<bool> callback = null)
        {
            if (segment.IsReady)
            {
#if ABR || TAPSELL
                TapsellPlus.ShowInterstitialAd(
                    segment.ResponseZone,
                    zid =>
                    {
                        Debug.Log("Tapsell.ShowAd.OnOpenAd: " + zid);
                        OnOpen?.Invoke(segment);
                    },
                    zid =>
                    {
                        Debug.Log("Tapsell.ShowAd.OnCloseAd: " + zid);
                        if (segment.ResponseZone.HasContent())
                        {
                            segment.ResponseZone = null;
                            callback?.Invoke(false);
                            OnClose?.Invoke(segment);
                        }
                    },
                    error =>
                    {
                        Debug.Log("AdMob.ShowAd.Error: " + error);
                        if (segment.ResponseZone.HasContent())
                        {
                            segment.ResponseZone = null;
                            callback?.Invoke(false);
                            OnError?.Invoke(segment);
                        }
                    });
#else
            callback?.Invoke(false);
#endif
            }
        }
    }
}