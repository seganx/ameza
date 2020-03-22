using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopItem : MonoBehaviour
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private Transform images = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText bombLabel = null;
    [SerializeField] private LocalText hammerLabel = null;
    [SerializeField] private LocalText missileLabel = null;
    [SerializeField] private LocalText discountLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private LocalText lastPriceLabel = null;
    [SerializeField] private LocalText timeLabel = null;
    [SerializeField] private Button button = null;

    private GlobalConfig.Data.Shop.Package pack = null;

    public UiShopItem Setup(string sku, System.Action<bool> onClick = null)
    {
        pack = GlobalConfig.Shop.GetPackage(sku);
        if (pack == null) return this;

        images.SetActiveChild(pack.image);
        if (title) title.SetText(pack.title);
        if (gemsLabel) gemsLabel.SetFormatedText(pack.gems);
        if (bombLabel) bombLabel.SetFormatedText(pack.bombs);
        if (hammerLabel) hammerLabel.SetFormatedText(pack.hammers);
        if (missileLabel) missileLabel.SetFormatedText(pack.missiles);
        if (priceLabel) priceLabel.SetFormatedText(pack.price);
        if (lastPriceLabel) lastPriceLabel.SetFormatedText(pack.lastPrice);
        if (discountLabel)
        {
            if (pack.discount > 0)
                discountLabel.SetFormatedText(pack.discount);
            else
                discountLabel.transform.parent.gameObject.SetActive(false);
        }

        button.onClick.AddListener(() =>
        {
            button.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, sku, (succeed, token) =>
            {
                if (succeed)
                {
                    Purchased(sku, token, () =>
                    {
                        if (onClick != null) onClick(true);
                    });
                }
                else if (onClick != null) onClick(false);

                button.SetInteractable(true);
            });
        });

        return this;
    }


    // Use this for initialization
    private IEnumerator Start()
    {
        var wait = new WaitForSeconds(0.5f);
        while (pack != null && timeLabel && pack.discount > 0)
        {
            int seconds = PurchaseOffer.RemainedTime;
            if (seconds < 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                timeLabel.SetText(Utilities.TimeToString(seconds, 0));
            }
            yield return wait;
        }
    }


    public static void Purchased(string sku, string token, System.Action nextTask)
    {
        var pack = GlobalConfig.Shop.GetPackage(sku);
        if (pack == null)
        {
            if (nextTask != null) nextTask();
            return;
        }

        Profile.EarnGems(pack.gems);
        Profile.Bombs += pack.bombs;
        Profile.Hammers += pack.hammers;
        Profile.Missiles += pack.missiles;

        Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, pack.gems, pack.bombs, pack.hammers, pack.missiles, true, nextTask);

        PurchaseSystem.Consume(pack.sku, (success, msg) =>
        {
            if (success)
            {
                Online.Stats.Set(GlobalConfig.Instance.version, Profile.Gems, Profile.Skill, Profile.GetLevelsPassed(), r => { });
                GlobalAnalytics.NewBuisinessEvent(Online.Purchase.Provider.Cafebazaar, pack.sku, pack.price, token);
            }
        });
    }
}
