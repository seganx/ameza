using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Lose : GameState
{
    [SerializeField] private Button bombButton = null;
    [SerializeField] private LocalText bombLabel = null;
    [SerializeField] private Button hammerButton = null;
    [SerializeField] private LocalText hammerLabel = null;
    [SerializeField] private Button missleButton = null;
    [SerializeField] private LocalText missleLabel = null;
    [SerializeField] private GameObject backButton = null;
    [SerializeField] private GameObject homeContent = null;
    [SerializeField] private Button homeButton = null;


    private System.Action<AbilityType> callbackFunc = null;

    public Popup_Lose Setup(bool displayHomeButton, System.Action<AbilityType> callback)
    {
        callbackFunc = callback;

        UpdateTexts();

        bombButton.onClick.AddListener(() =>
        {
            if (Profile.Bombs > 0)
            {
                Profile.Bombs--;
                callback(AbilityType.Bomb);
                base.Back();
            }
            else BuyBooster(GlobalConfig.ProfilePreset.bombs, GlobalConfig.Shop.bombPrice, count => Profile.Bombs += count);
        });

        hammerButton.onClick.AddListener(() =>
        {
            if (Profile.Hammers > 0)
            {
                Profile.Hammers--;
                callback(AbilityType.Hammer);
                base.Back();
            }
            else BuyBooster(GlobalConfig.ProfilePreset.hammers, GlobalConfig.Shop.hammerPrice, count => Profile.Hammers += count);
        });

        missleButton.onClick.AddListener(() =>
        {
            if (Profile.Missiles > 0)
            {
                Profile.Missiles--;
                callback(AbilityType.Missle);
                base.Back();
            }
            else BuyBooster(GlobalConfig.ProfilePreset.missles, GlobalConfig.Shop.misslePrice, count => Profile.Missiles += count);
        });

        homeButton.onClick.AddListener(() =>
        {
            var score = PlayModel.GetLeagueScore();
            if (score > 0)
            {
                PlayModel.onPreLose(ok => { if (ok) Back(); });
            }
            else Back();
        });

        backButton.SetActive(displayHomeButton == false);
        homeContent.SetActive(displayHomeButton == true);

        UiShowHide.ShowAll(transform);
        return this;
    }

    public override void Back()
    {
        base.Back();
        callbackFunc(AbilityType.Null);
    }

    private void UpdateTexts()
    {
        bombLabel.SetText(Profile.Bombs > 0 ? Profile.Bombs.ToString() : "+");
        hammerLabel.SetText(Profile.Hammers > 0 ? Profile.Hammers.ToString() : "+");
        missleLabel.SetText(Profile.Missiles > 0 ? Profile.Missiles.ToString() : "+");
    }

    private void BuyBooster(int count, int price, System.Action<int> onSuccess)
    {
        gameManager.OpenPopup<Popup_Confirm>().SetText(111003, count, price).Setup(true, true, yes =>
        {
            if (yes == false) return;
            Game.SpendGems(price, () =>
            {
                onSuccess(count);
                UpdateTexts();
            });
        });
    }
}
