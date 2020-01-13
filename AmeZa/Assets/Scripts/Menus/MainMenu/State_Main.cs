using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class State_Main : GameState
{
    [SerializeField] private Button onlineButton = null;
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
                gameManager.OpenState<State_SelectLeague>();
            else
                tutorial.Display(false, 111034, null);
        });

        levelsButton.onClick.AddListener(() => gameManager.OpenState<State_Levels>());

        luckyButton.onClick.AddListener(() =>
        {
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.luckySpin.id, GlobalConfig.Timers.luckySpin.duration);
            if (seconds > 0)
            {
                gameManager.OpenPopup<Popup_Confirm>().SetText(111018, TimeToString(seconds)).Setup(true, false, null);

            }
            else
            {
                Online.Timer.Set(GlobalConfig.Timers.luckySpin.id, GlobalConfig.Timers.luckySpin.duration);
                gameManager.OpenPopup<Popup_LuckySpine>();
            }
        });


        var wait = new WaitForSeconds(1);
        while (true)
        {
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.luckySpin.id, GlobalConfig.Timers.luckySpin.duration);
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
            tutorial.Display(true, 111031, () => tutorial.Display(true, 111032, () => tutorial.Display(true, 111033, null)));
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
        gameManager.OpenPopup<Popup_Confirm>().Setup(111004, true, true, yes =>
        {
            if (yes)
                Application.Quit();
        });
    }
}
