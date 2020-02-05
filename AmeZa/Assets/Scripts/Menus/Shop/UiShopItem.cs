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
    private static GlobalConfig.Data.Shop.Package purchasePackage = null;

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
            purchasePackage = pack;
            button.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, purchasePackage.sku, (succeed, token) =>
            {
                if (succeed)
                {
                    Profile.EarnGems(purchasePackage.gems);
                    Profile.Bombs += purchasePackage.bombs;
                    Profile.Hammers += purchasePackage.hammers;
                    Profile.Missiles += purchasePackage.missiles;

                    PurchaseSystem.Consume(purchasePackage.sku, (cSuccseed, smg) =>
                    {
                        if (cSuccseed)
                            GlobalAnalytics.NewBuisinessEvent(Online.Purchase.Provider.Cafebazaar, purchasePackage.sku, purchasePackage.price, token);
                    });

                    Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, purchasePackage.gems, purchasePackage.bombs, purchasePackage.hammers, purchasePackage.missiles, true, () =>
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
