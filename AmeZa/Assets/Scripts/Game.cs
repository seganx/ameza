using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(-1)]
    public class Game : GameManager<Game>
    {
        // Use this for initialization
        private IEnumerator Start()
        {
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
            Online.Timer.SetTimer(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
        }


        private bool localPushScheduled = false;
        private void ScheduleLocalPush()
        {
            if (localPushScheduled) return;
            localPushScheduled = true;

            // schedule hearts push
#if LOCALPUSH
            if (Profile.Hearts < GlobalConfig.ProfilePreset.heats)
            {
                int seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.heart.id);
                LocalPush.NotificationManager.SendWithAppIcon(System.TimeSpan.FromSeconds(seconds), LocalizationService.Get(111015), LocalizationService.Get(111016), Color.green, LocalPush.NotificationIcon.Heart);
            }
#endif
        }


        public static void SpendGems(int value, System.Action onSuccess)
        {
            if (Profile.SpendGems(value))
            {
                onSuccess();
            }
            else Instance.OpenPopup<Popup_Shop>().SetOnClose(() =>
            {
                if (Profile.SpendGems(value))
                    onSuccess();
            });
        }
    }
}
