using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopItemFree : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText timeLabel = null;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        gemsLabel.SetFormatedText(GlobalConfig.Advertise.shopFreeGems.rewardGems);

        button.onClick.AddListener(() =>
        {
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Advertise.shopFreeGems.timerId, GlobalConfig.Advertise.shopFreeGems.interval);
            if (seconds > 0)
            {
                Game.Instance.OpenPopup<Popup_Confirm>().SetText(111018, TimeToString(seconds)).Setup(true, false, null);
            }
            else
            {
                Game.Instance.OpenPopup<Popup_VideoAd>().Setup(GlobalConfig.Advertise.shopFreeGems, "shopad", success =>
                {
                    if (success == false) return;
                    Online.Timer.Set(GlobalConfig.Advertise.shopFreeGems.timerId, GlobalConfig.Advertise.shopFreeGems.interval);
                    Profile.EarnGems(GlobalConfig.Advertise.shopFreeGems.rewardGems);
                    Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, GlobalConfig.Advertise.shopFreeGems.rewardGems, 0, 0, 0, true);
                    GlobalAnalytics.SourceGem(GlobalConfig.Advertise.shopFreeGems.rewardGems, "shopad");
                });
            }
        });

        var wait = new WaitForSeconds(1);
        while (true)
        {
            var seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Advertise.shopFreeGems.timerId, GlobalConfig.Advertise.shopFreeGems.interval);
            if (seconds > 0)
            {
                timeLabel.transform.SetActiveChild(1);
                var h = seconds / 3600;
                var m = (seconds % 3600) / 60;
                var s = seconds % 60;
                timeLabel.SetFormatedText(h, m, s);
            }
            else
            {
                timeLabel.transform.SetActiveChild(0);
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
}
