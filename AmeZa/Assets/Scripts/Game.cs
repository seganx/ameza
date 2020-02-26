using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(-1)]
    public class Game : GameManager<Game>
    {
#if UNITY_EDITOR
        [SerializeField] private bool stopLoading = false;
#endif

        // Use this for initialization
        private IEnumerator Start()
        {
#if UNITY_EDITOR
            if (stopLoading)
                yield break;
#endif

#if LOCALPUSH
            LocalPush.NotificationManager.CancelAll();
#endif

            FirstInitialization();
            yield return new WaitForSeconds(0.1f);

            PurchaseSystem.Initialize(GlobalConfig.Instance.cafeBazaarKey, GlobalConfig.Socials.storeUrl, (succeed, msg) => Debug.Log("Purchase system initialized!"));

            Loading.Show();
            Profile.Sync(false, succss =>
            {
                Loading.Hide();
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

        private void FirstInitialization()
        {
            if (PlayerPrefs.GetInt(name + ".Inited", 0) > 0) return;
            PlayerPrefs.SetInt(name + ".Inited", 1);

            // init timers
            //Online.Timer.SetTimer(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
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
        public static void Reset()
        {
            Application.Quit();
            Console.Logger.ClearData();
            GlobalConfig.DebugMode = true;
            Profile.Reset();
        }
    }
}
