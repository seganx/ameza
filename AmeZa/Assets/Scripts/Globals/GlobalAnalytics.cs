using GameAnalyticsSDK;
using SeganX;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GlobalAnalytics : MonoBehaviour
{
    private void Awake()
    {
        instance = this;
        GameAnalytics.Initialize();

        GameAnalytics.OnRemoteConfigsUpdatedEvent += () =>
        {
            if (GameAnalytics.IsRemoteConfigsReady() == false) return;
            ABTest.OnRecieved?.Invoke();

            var remoteConfig = GameAnalytics.GetRemoteConfigsContentAsString();
            if (remoteConfig.HasContent(5))
            {
                Debug.Log("GameAnalytics remote config recieved:\n" + GameAnalytics.GetRemoteConfigsContentAsString());
            }
        };
    }


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static GlobalAnalytics instance = null;

    private static void SetGroup(int index)
    {
        GameAnalytics.SetCustomDimension01("group_" + Mathf.Clamp(index, 0, 3));
    }

    public static class ABTest
    {
        public static System.Action OnRecieved = null;

        public static string Info
        {
            get
            {
                var id = GameAnalytics.GetABTestingId();
                var group = GameAnalytics.GetABTestingVariantId();
                return string.IsNullOrEmpty(id) ? "None" : ("Id: " + id + " Group: " + group);
            }
        }

        public static string Get(string key, string defaultValue)
        {
            if (GameAnalytics.IsRemoteConfigsReady() == false) return defaultValue;
            var res = GameAnalytics.GetRemoteConfigsValueAsString(key, defaultValue);
            Debug.Log("GameAnalytics remote config: " + key + " = " + defaultValue + " ~> " + res);
            return res;
        }

        public static int Get(string key, int defaultValue)
        {
            return Get(key, defaultValue.ToString()).ToInt(defaultValue);
        }

        public static float Get(string key, float defaultValue)
        {
            return Get(key, defaultValue.ToString()).ToFloat(defaultValue);
        }

        public static int GetGroup(int defaultValue)
        {
            int res = Get("group", defaultValue);
            SetGroup(res);
            return res;
        }
    }

    public static class Levels
    {
        public static void Start(int season, int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Levels", season.ToString(), level.ToString(), 0);
        }

        public static void Complete(int season, int level, int stars)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Levels", season.ToString(), level.ToString(), stars);
        }

        public static void Fail(int season, int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Levels", season.ToString(), level.ToString(), 0);
        }
    }

    public static class Resources
    {
        // A “sink” is when a player loses or spends a resource
        public static void Sink(string name, string gate, string item, int amount)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, name, amount, gate, item);
        }

        // A “source” is when a player gains or earns a resource
        public static void Source(string name, string gate, string item, int amount)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, name, amount, gate, item);
        }
    }

    public static class Ad
    {
        public static class Rewarded
        {
            public static void Request(string placement)
            {
                GameAnalytics.NewAdEvent(GAAdAction.Request, GAAdType.RewardedVideo, "tapsell", placement);
            }

            public static void RequestFailed(string placement)
            {
                GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "tapsell", placement);
            }

            public static void Show(string placement)
            {
                GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "tapsell", placement);
            }

            public static void RewardReceived(string placement)
            {
                GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "tapsell", placement);
            }
        }

        public static class Interstitial
        {
            public static void Show(string placement)
            {
                GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "tapsell", placement);
            }
        }
    }

    public static void NewBusinessEvent(int rialAmount, string sku)
    {
        GameAnalytics.NewBusinessEvent("USD", rialAmount / 10, "pack", sku, "shop");
    }

    // A “sink” is when a player loses or spends a resource
    public static void SinkGem(int amount, string placement, string itemId)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "gem", amount, placement, itemId);
    }

    // A “source” is when a player gains or earns a resource
    public static void SourceGem(int amount, string itemId)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "gem", amount, "earn", itemId);
    }

}
