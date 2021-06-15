using System.Collections;
using UnityEngine;
using SeganX;

[DefaultExecutionOrder(-1)]
public class Game : GameManager
{
#if UNITY_EDITOR
    [SerializeField] private bool stopLoading = false;
#endif

    protected virtual void Awake()
    {
        Input.multiTouchEnabled = false;

#if UNITY_EDITOR
        Console.Enabled = true;
#endif
    }

    // Use this for initialization
#if UNITY_EDITOR
    private IEnumerator Start()
    {
        if (stopLoading)
            yield break;
#else
     private void Start()
     {
#endif
        Console.OnDisplayInfo += str =>
        {
            return "Ver: " + Application.version + " Group: " + GlobalConfig.Group + "\nId: " + SeganX.Console.DisplayDeviceID;

        };

        Loading.Show();
        Profile.Sync(false, succss =>
        {
            Loading.Hide();
            OpenState<State_Main>();
        });

#if TAPSELL
        TapsellPlusSDK.TapsellPlus.Initialize("eojghhgttbmtjibqhsjtrkprmpiparomofortdgjrndaqtocafonrglqlhgionkrgbbija",
            adNetworkName => Debug.Log(adNetworkName + " Initialized Successfully."),
            error => Debug.Log(error.ToString()));
#endif
        Fun.Iab.PurchaseSystem.Initialize(GlobalConfig.Instance.version, GlobalConfig.Market.rsaKey, GlobalConfig.Market.storeUrl);

        SeganX.LocalNotification.Initialize(GlobalConfig.Notifications.items);
        SeganX.LocalNotification.OnScheduleNotification += () =>
        {
            // schedule hearts push
            if (Profile.Hearts < GlobalConfig.ProfilePreset.hearts)
            {
                int totalSeconds = GlobalConfig.ProfilePreset.hearts * GlobalConfig.Heart.interval;
                SeganX.LocalNotification.SendNotification(totalSeconds, GlobalConfig.Notifications.heartFull);
            }

            // schedule lucky spin push
            var seconds = SeganX.Online.Timer.GetRemainSeconds(GlobalConfig.Luckyspin.timerId, GlobalConfig.Luckyspin.interval);
            if (seconds > 10)
            {
                SeganX.LocalNotification.SendNotification(seconds, GlobalConfig.Notifications.luckySpine);
            }
        };

        CheckABTest();
        GlobalAnalytics.ABTest.OnRecieved = CheckABTest;

#if UNITY_EDITOR
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Space))
            {
                var path = Application.dataPath + "/../../Documents/Screenshots/" + System.DateTime.Now.Ticks + ".png";
                path = System.IO.Path.GetFullPath(path);
                ScreenCapture.CaptureScreenshot(path);
                yield return new WaitForSeconds(0.5f);
                Application.OpenURL(path);
                UnityEditor.EditorUtility.RevealInFinder(path);
            }
        }
#endif
    }

    private static void CheckABTest()
    {
        GlobalConfig.Group = GlobalAnalytics.ABTest.GetGroup(GlobalConfig.Group);
        GlobalConfig.Save();
    }

    public static void SpendGems(int value, System.Action onSuccess)
    {
        if (Profile.SpendGems(value))
        {
            onSuccess();
        }
        else
        {
            var pack = GetOfferPackage();
            if (pack != null)
            {
                Instance.OpenPopup<Popup_Offer>().Setup(pack, sucess =>
                {
                    if (sucess)
                    {
                        if (Profile.SpendGems(value))
                            onSuccess();
                    }
                    else Instance.OpenPopup<Popup_Shop>().SetOnClose(() =>
                    {
                        if (Profile.SpendGems(value))
                            onSuccess();
                    });
                });
            }
            else Instance.OpenPopup<Popup_Shop>().SetOnClose(() =>
            {
                if (Profile.SpendGems(value))
                    onSuccess();
            });
        }
    }

    public static GlobalConfig.Data.Shop.Package GetOfferPackage()
    {
        var index = SeganX.PurchaseOffer.GetOfferIndex(Profile.Gems);
        if (index.Between(0, GlobalConfig.Shop.offers.Count - 1))
        {
            var pack = GlobalConfig.Shop.offers[index];
            return (pack.discount > 0) ? pack : null;
        }
        return null;
    }

    [SeganX.Console("game", "reset", "Clear all progress data from local/server!")]
    public static void ResetGame()
    {
        Profile.Reset();
        GlobalConfig.DebugMode = true;
    }

    [SeganX.Console("friends", "add")]
    public static void AddFriends(string username)
    {
        SeganX.Online.Friends.Add(username, (success, res) => Debug.Log(res.GetStringDebug()));
    }

    [SeganX.Console("friends", "get")]
    public static void GetFriends()
    {
        SeganX.Online.Friends.Get((success, res) => Debug.Log(res.GetStringDebug()));
    }


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static Game instance = default;
    internal static Game Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Game>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<Game>();
                    instance.gameObject.name = "Game";
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }
}