using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class State_Main : GameState
{
    [SerializeField] private Button onlineButton = null;
    [SerializeField] private Button classicButton = null;
    [SerializeField] private Button levelsButton = null;
    [SerializeField] private Button luckyButton = null;
    [SerializeField] private LocalText luckyTimeLabel = null;
    [SerializeField] private UiTutorial tutorial = null;

    private IEnumerator Start()
    {
        UIBackground.Show();
        UiShowHide.ShowAll(transform);

        onlineButton.onClick.AddListener(() =>
        {
            if (Profile.GetSeasonRewarded(0) > 0)
                game.OpenState<State_SelectLeague>();
            else
                tutorial.Display(0, false, 111034, null);
        });

        classicButton.onClick.AddListener(() =>
        {
            if (ClassicLogics.SetPlayModel())
            {
                UIBackground.Hide();
                Game.Instance.OpenState<State_Playing>();
            }
        });

        if (Profile.IsFirstSession) levelsButton.GetComponent<Animation>().Play();
        levelsButton.onClick.AddListener(() => game.OpenState<State_Levels>());

        luckyButton.onClick.AddListener(() =>
        {
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Luckyspin.timerId, GlobalConfig.Luckyspin.interval);
            if (seconds > 0)
            {
                game.OpenPopup<Popup_Confirm>().SetText(111018, TimeToString(seconds)).Setup(true, false, null);
            }
            else
            {
                Online.Timer.Set(GlobalConfig.Luckyspin.timerId, GlobalConfig.Luckyspin.interval);
                game.OpenPopup<Popup_LuckySpine>();
            }
        });


        if (Profile.Version > 5 && 
            Profile.Version < GlobalConfig.Instance.version && 
            GlobalConfig.Update.mode == GlobalConfig.Data.Update.Mode.Null &&
            GlobalConfig.Update.rewardGems > 0)
        {
            Profile.Version = GlobalConfig.Instance.version;
            game.OpenPopup<Popup_Confirm>().Setup(111129, true, false, ok =>
            {
                game.OpenPopup<Popup_Rewards>().Setup(0, GlobalConfig.Update.rewardGems, 0, 0, 0, true, () => Profile.EarnGems(GlobalConfig.Update.rewardGems));
                GlobalAnalytics.SourceGem(GlobalConfig.Update.rewardGems, "update");
            });
        }

        var displayed = tutorial.Display(1, true, 111031, () => tutorial.Display(0, true, 111032, null));
        if (displayed == false) tutorial.DisplayJoke(1);

        var wait = new WaitForSeconds(1);
        while (true)
        {
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Luckyspin.timerId, GlobalConfig.Luckyspin.interval);
            if (seconds > 0)
            {
                luckyTimeLabel.transform.SetActiveChild(1);
                var h = seconds / 3600;
                var m = (seconds % 3600) / 60;
                var s = seconds % 60;
                luckyTimeLabel.SetFormatedText(h, m, s);
            }
            else
            {
                luckyTimeLabel.transform.SetActiveChild(0);
            }

            yield return wait;
        }
    }

    private string TimeToString(long secconds)
    {
        if (secconds / 86400 > 0) return string.Format(LocalizationService.Get(111006), secconds / 86400);
        if (secconds / 3600 > 0) return string.Format(LocalizationService.Get(111007), secconds / 3600);
        if (secconds / 60 > 0) return string.Format(LocalizationService.Get(111008), secconds / 60);
        return string.Format(LocalizationService.Get(111009), secconds);
    }

    public override void Back()
    {
        if (tutorial.Hide()) return;

        game.OpenPopup<Popup_Confirm>().Setup(111004, true, true, yes =>
        {
            if (yes)
                Application.Quit();
        });
    }
}
