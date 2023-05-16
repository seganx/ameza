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

    public static class Banner
    {
        public static void Show(Segment segment)
        {
            if (OnPreRequest(segment) == false) return;

            var zone = segment.ZoneId;
            Debug.Log("FunAd.Banner.Show: " + zone);

            if (zone.IsNullOrEmpty())
            {
                Hide();
                return;
            }
#if ABR || TAPSELL
            TapsellPlus.showBannerAd(zone, BannerType.BANNER_320x50, Gravity.BOTTOM, Gravity.CENTER,
                zoneId =>
                {
                    OnLoaded?.Invoke(segment);
                    TapsellPlus.displayBanner();
                    OnOpen?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("Tapsell.showBannerAd.Error: " + error.message);
                    OnError?.Invoke(segment);
                });

#elif UAD
            UnityAd.Load(zone, success =>
            {
                if (success)
                {
                    OnLoaded?.Invoke(segment);
                    UnityAd.ShowBanner(zone);
                    OnOpen?.Invoke(segment);
                }
                else OnError?.Invoke(segment);
            });
#elif ADMOB
            AdMob.RequestBanner(zone,
                zid =>
                {
                    OnLoaded?.Invoke(segment);
                    AdMob.ShowBanner();
                    OnOpen?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("AdMob.RequestBanner.Error: " + error);
                    OnError?.Invoke(segment);
                });
#endif
        }

        public static void Hide()
        {
#if ABR || TAPSELL
            TapsellPlus.hideBanner();
#elif UAD
            UnityAd.HideBanner();
#elif ADMOB
            AdMob.HideBanner();
#endif
        }
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
            TapsellPlus.requestRewardedVideo(zone,
                zoneId =>
                {
                    segment.ResponseZone = zoneId;
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
            ShowAd(false, segment, callback);
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
            TapsellPlus.requestInterstitial(zone,
                zoneId =>
                {
                    segment.ResponseZone = zoneId;
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
            ShowAd(true, segment, callback);
        }
    }

    public static class Native
    {
        public static void Request(Segment segment, System.Action<NativeData, string> callback)
        {
            var zone = segment.ZoneId;
            Debug.Log("FunAd.Native.Request: " + zone);

            if (zone.IsNullOrEmpty())
            {
                admono.Call(ok => callback(null, null));
                return;
            }

#if ABR || TAPSELL
            TapsellPlus.requestNativeBanner(admono, zone,
                result =>
                {
                    var res = new NativeData()
                    {
                        title = result.title,
                        description = result.description,
                        callToActionText = result.callToActionText,
                        portraitBannerImage = Sprite.Create(result.portraitBannerImage, new Rect(0, 0, result.portraitBannerImage.width, result.portraitBannerImage.height), Vector2.one * 0.5f),
                        landscapeBannerImage = Sprite.Create(result.landscapeBannerImage, new Rect(0, 0, result.landscapeBannerImage.width, result.landscapeBannerImage.height), Vector2.one * 0.5f),
                        iconImage = Sprite.Create(result.iconImage, new Rect(0, 0, result.iconImage.width, result.iconImage.height), Vector2.one * 0.5f),
                        onClick = result.clicked
                    };
                    OnLoaded?.Invoke(segment);
                },
                error =>
                {
                    Debug.Log("Tapsell.RequestInterstitial.Error: " + error.message);
                    callback(null, error.message);
                    OnError?.Invoke(segment);
                });
#elif ADMOB
#else
            callback?.Invoke(null, "No ad service installed!");
#endif
        }
    }

    private static void ShowAd(bool interestitial, Segment segment, System.Action<bool> callback)
    {
        Debug.Log("FunAd.ShowAd: " + segment.ResponseZone);

        if (segment.IsReady)
        {
#if ABR || TAPSELL
            TapsellPlus.showAd(
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
                        callback?.Invoke(interestitial);
                        OnClose?.Invoke(segment);
                    }
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
                error =>
                {
                    Debug.Log("Tapsell.ShowAd.Error: " + error.message);
                    if (segment.ResponseZone.HasContent())
                    {
                        segment.ResponseZone = null;
                        callback?.Invoke(false);
                        OnError?.Invoke(segment);
                    }
                });
#elif UAD
            OnOpen?.Invoke(segment);
            UnityAd.Show(segment.ResponseZone, success =>
            {
                if (segment.ResponseZone.HasContent())
                {
                    segment.ResponseZone = null;
                    callback?.Invoke(success);
                    OnClose?.Invoke(segment);
                }
            });
#elif ADMOB
            AdMob.Show(
                segment.ResponseZone,
                zid =>
                {
                    Debug.Log("AdMob.ShowAd.OnOpenAd: " + zid);
                    OnOpen?.Invoke(segment);
                },
                zid =>
                {
                    Debug.Log("AdMob.ShowAd.OnCloseAd: " + zid);
                    if (segment.ResponseZone.HasContent())
                    {
                        segment.ResponseZone = null;
                        callback?.Invoke(interestitial);
                        OnClose?.Invoke(segment);
                    }
                },
                zid =>
                {
                    Debug.Log("AdMob.ShowAd.OnRewarded: " + zid);
                    if (segment.ResponseZone.HasContent())
                    {
                        segment.ResponseZone = null;
                        callback?.Invoke(true);
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
#endif
        }
        else
        {
            admono.Call(callback, false);
        }
    }
}