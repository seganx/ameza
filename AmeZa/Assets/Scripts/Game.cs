using System.Collections;
using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(-1)]
    public partial class Game : GameManager
    {
#if UNITY_EDITOR
        [SerializeField] private bool stopLoading = false;
#endif

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
            Console.Info.OnDisplayInfo = str =>
            {
                return "Ver: " + Application.version + " Group: " + GlobalConfig.Group + "\nId: " + SeganX.Console.Info.DisplayDeviceID;

            };

            Loading.Show();
            Profile.Sync(false, succss =>
            {
                Loading.Hide();
                OpenState<State_Main>();
            });

            PurchaseSystem.Initialize(GlobalConfig.Instance.version, GlobalConfig.Instance.cafeBazaarKey, GlobalConfig.Socials.storeUrl);

            LocalNotification.Initialize(GlobalConfig.Notifications.items);
            LocalNotification.OnScheduleNotification += () =>
            {
                // schedule hearts push
                if (Profile.Hearts < GlobalConfig.ProfilePreset.hearts)
                {
                    int totalSeconds = GlobalConfig.ProfilePreset.hearts * GlobalConfig.Heart.interval;
                    LocalNotification.SendNotification(totalSeconds, GlobalConfig.Notifications.heartFull);
                }

                // schedule lucky spin push
                var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Luckyspin.timerId, GlobalConfig.Luckyspin.interval);
                if (seconds > 10)
                {
                    LocalNotification.SendNotification(seconds, GlobalConfig.Notifications.luckySpine);
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
            var index = PurchaseOffer.GetOfferIndex(Profile.Gems);
            if (index.Between(0, GlobalConfig.Shop.offers.Count - 1))
            {
                var pack = GlobalConfig.Shop.offers[index];
                return (pack.discount > 0) ? pack : null;
            }
            return null;
        }

        [Console("game", "reset", "Clear all progress data from local/server!")]
        public static void ResetGame()
        {
            Profile.Reset();
            GlobalConfig.DebugMode = true;
        }

        [Console("friends", "add")]
        public static void AddFriends(string username)
        {
            Online.Friends.Add(username, (success, res) => Debug.Log(res.GetStringDebug()));
        }

        [Console("friends", "get")]
        public static void GetFriends()
        {
            Online.Friends.Get((success, res) => Debug.Log(res.GetStringDebug()));
        }
    }
}
