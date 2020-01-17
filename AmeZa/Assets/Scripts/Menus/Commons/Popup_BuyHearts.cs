using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_BuyHearts : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private LocalText hearts = null;
    [SerializeField] private LocalText price = null;
    [SerializeField] private Button buyButton = null;

    // Use this for initialization
    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);

        buyButton.onClick.AddListener(() =>
        {
            Game.SpendGems(GlobalConfig.Shop.heartPrice, () =>
            {
                Profile.Hearts += GlobalConfig.ProfilePreset.hearts;
                Back();
            });
        });


        price.SetText(GlobalConfig.Shop.heartPrice.ToString());
        var wait = new WaitForSeconds(1);
        while (true)
        {
            hearts.SetText(Profile.Hearts.ToString());
            int seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
            title.SetFormatedText(seconds / 60, seconds % 60);
            yield return wait;
        }
    }

}
