using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(-1)]
    public partial class Game : GameManager
    {
#if UNITY_EDITOR
        [SerializeField] private bool stopLoading = false;
#endif

        public static int LastVersion
        {
            get { return PlayerPrefs.GetInt("Game.LastVersion", GlobalConfig.Instance.version); }
            set { PlayerPrefs.SetInt("Game.LastVersion", value); }
        }

        // Use this for initialization
        private IEnumerator Start()
        {
#if UNITY_EDITOR
            if (stopLoading)
                yield break;
#endif

            SeganX.Console.Info.OnDisplayInfo = str =>
            {
                return "Ver: " + Application.version + " Group: " + GlobalAnalytics.Group +  "\nId: " + SeganX.Console.Info.DisplayDeviceID;

            };

#if LOCALPUSH
            LocalPush.NotificationManager.CancelAll();
#endif
            yield return new WaitForSeconds(0.1f);

            PurchaseSystem.Initialize(GlobalConfig.Instance.version, GlobalConfig.Instance.cafeBazaarKey, GlobalConfig.Socials.storeUrl);

            Loading.Show();
            Profile.Sync(false, succss =>
            {
                Loading.Hide();
                LastVersion = GlobalConfig.Instance.version;
                OpenState<State_Main>();
            });
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Space))
            {
                var path = Application.dataPath + "/../../Documents/Screenshots/" + System.DateTime.Now.Ticks + ".png";
                ScreenCapture.CaptureScreenshot(path);
                Application.OpenURL(path);
                UnityEditor.EditorUtility.RevealInFinder(path);
            }
        }
#endif

        private void OnApplicationPause(bool pause)
        {
            if (pause) ScheduleLocalPush();
        }

        private void OnApplicationQuit()
        {
            ScheduleLocalPush();
        }


        private bool localPushScheduled = false;
        private void ScheduleLocalPush()
        {
            if (localPushScheduled) return;
            localPushScheduled = true;

#if LOCALPUSH
            // schedule hearts push
            if (Profile.Hearts < GlobalConfig.ProfilePreset.hearts)
            {
                int totalSeconds = GlobalConfig.ProfilePreset.hearts * GlobalConfig.Timers.heart.duration;
                LocalPush.NotificationManager.SendWithAppIcon(System.TimeSpan.FromSeconds(totalSeconds), LocalizationService.Get(111015), LocalizationService.Get(111016), Color.green, LocalPush.NotificationIcon.Heart);
            }

            // schedule lucky spin push
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.luckySpin.id, GlobalConfig.Timers.luckySpin.duration);
            if (seconds > 10)
            {
                LocalPush.NotificationManager.SendWithAppIcon(System.TimeSpan.FromSeconds(seconds), LocalizationService.Get(111019), LocalizationService.Get(111020), Color.green, LocalPush.NotificationIcon.Clock);
            }

            int pushIndex = PlayerPrefs.GetInt(name + ".PushIndex", 0);

            // schedule mid-time no see
            LocalPush.NotificationManager.SendWithAppIcon(System.TimeSpan.FromSeconds(2 * 24 * 60 * 60), LocalizationService.Get(111070), LocalizationService.Get(111071 + pushIndex), Color.green, LocalPush.NotificationIcon.Message);
            pushIndex = (pushIndex + 1) % 9;

            // schedule long time no see
            LocalPush.NotificationManager.SendWithAppIcon(System.TimeSpan.FromSeconds(5 * 24 * 60 * 60), LocalizationService.Get(111070), LocalizationService.Get(111071 + pushIndex), Color.green, LocalPush.NotificationIcon.Message);
            pushIndex = (pushIndex + 1) % 9;

            PlayerPrefs.SetInt(name + ".PushIndex", pushIndex);
#endif
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
