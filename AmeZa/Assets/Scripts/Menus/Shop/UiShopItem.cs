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

    public UiShopItem Setup(GlobalConfig.Data.Shop.Package pack, System.Action<bool> onClick = null)
    {
        this.pack = pack;

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
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.sku, (succeed, token) =>
            {
                button.SetInteractable(true);
                if (succeed)
                {
                    Profile.EarnGems(pack.gems);
                    Profile.Bombs += pack.bombs;
                    Profile.Hammers += pack.hammers;
                    Profile.Missiles += pack.missiles;
                    PurchaseSystem.Consume(pack.sku);
                    Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, pack.gems, pack.bombs, pack.hammers, pack.missiles, true, () =>
                    {
                        if (onClick != null) onClick(true);
                    });

                    GlobalAnalytics.NewBuisinessEvent(Online.Purchase.Provider.Cafebazaar, pack.sku, pack.price, token);
                }
                else if (onClick != null) onClick(false);
            });
        });

        return this;
    }


    // Use this for initialization
    private IEnumerator Start()
    {
        var wait = new WaitForSeconds(0.5f);
        while (timeLabel && pack.discount > 0)
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
}
