using Plankton;
using SeganX;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GlobalAnalytics : MonoBehaviour
{
    private const GeneralAnalytics.Provider provider = GeneralAnalytics.Provider.Firebase;

    // game analaytic's remote config key: the angle of shooting
    public const string rc_shoot_angle = "shoot_angle";

    public static readonly string[] rc_all = new string[] {
        rc_shoot_angle,
    };


    private void Awake()
    {
        instance = this;
        GeneralAnalytics.Initialize();

        GameAnalytics.Initialize(
                new GameAnalytics.Builder("49e4082c40a593ddf9e5fceda5cf82a3", "6ea05e72b7bc31ca9326bbede14215ba80dc467d")
                .SetUserId(Core.DeviceId)
                .SetBuildVersion(Application.version)
                .ConfigureResourceCurrencies("gem")
                .ConfigureItemTypes("earn", "profile", "levels", "classic", "ad")
                .ConfigureCustomDimensions01("google", "ios")
                , async () =>
                {
                    Debug.Log($"[Services] [GA] Initialized!");
#if UNITY_IOS
                    GameAnalytics.SetCustomDimension01("ios");
                    GeneralAnalytics.SetUserProperty(provider, "market", "ios");
#else
                    GameAnalytics.SetCustomDimension01("google");
                    GeneralAnalytics.SetUserProperty(provider, "market", "google");
#endif

#if UNITY_EDITOR
                    int numberOfTries = 1;
#else
        int numberOfTries = 60;
#endif
                    while (numberOfTries-- > 0 && GameAnalytics.IsRemoteConfigsReady() == false)
                    {
                        //Debug.Log("[Services] [GA] waiting for RemoteConfigs");
                        await Task.Delay(1000);
                    }

                    if (GameAnalytics.IsRemoteConfigsReady())
                    {
                        var log = "[Services] [GA] received RemoteConfigs:\n";
                        var v = new string[rc_all.Length];
                        for (int i = 0; i < v.Length; i++)
                        {
                            v[i] = GameAnalytics.GetRemoteConfigsValueAsString(rc_all[i], "none");
                            log += $"{rc_all[i]} = {v[i]}\n";
                        }
                        Debug.Log(log);

                        ABTest.OnRecieved?.Invoke();
                    }
                });
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
            GameAnalytics.AddProgressionEvent(GameAnalytics.ProgressionStatus.Start, "Levels", season.ToString(), level.ToString());
        }

        public static void Complete(int season, int level, int stars)
        {
            GameAnalytics.AddProgressionEvent(GameAnalytics.ProgressionStatus.Complete, "Levels", season.ToString(), level.ToString());
        }

        public static void Fail(int season, int level)
        {
            GameAnalytics.AddProgressionEvent(GameAnalytics.ProgressionStatus.Fail, "Levels", season.ToString(), level.ToString());
        }
    }

    public static class Resources
    {
        // A “sink” is when a player loses or spends a resource
        public static void Sink(string name, string gate, string item, int amount)
        {
            GameAnalytics.AddResourceEvent(GameAnalytics.ResourceFlowType.Sink, name, amount, gate, item);
        }

        // A “source” is when a player gains or earns a resource
        public static void Source(string name, string gate, string item, int amount)
        {
            GameAnalytics.AddResourceEvent(GameAnalytics.ResourceFlowType.Source, name, amount, gate, item);
        }
    }

    public static void NewBusinessEvent(int rialAmount, string sku)
    {
        GameAnalytics.AddBusinessEvent("USD", rialAmount, "pack", sku, "shop");
    }

    // A “sink” is when a player loses or spends a resource
    public static void SinkGem(int amount, string placement, string itemId)
    {
        GameAnalytics.AddResourceEvent(GameAnalytics.ResourceFlowType.Sink, "gem", amount, placement, itemId);
    }

    // A “source” is when a player gains or earns a resource
    public static void SourceGem(int amount, string itemId)
    {
        GameAnalytics.AddResourceEvent(GameAnalytics.ResourceFlowType.Source, "gem", amount, "earn", itemId);
    }

}
