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
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button button = null;


    public UiShopItem Setup(GlobalConfig.Data.Shop.Package pack)
    {
        images.SetActiveChild(pack.image);
        if (title) title.SetText(pack.title);
        if (gemsLabel) gemsLabel.SetFormatedText(pack.gems);
        if (bombLabel) bombLabel.SetFormatedText(pack.bombs);
        if (hammerLabel) hammerLabel.SetFormatedText(pack.hammers);
        if (missileLabel) missileLabel.SetFormatedText(pack.missiles);
        priceLabel.SetFormatedText(pack.price);

        button.onClick.AddListener(() =>
        {
            button.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.sku, (succeed, msg) =>
            {
                button.SetInteractable(true);
                if (succeed)
                {
                    Profile.EarnGems(pack.gems);
                    Profile.Bombs += pack.bombs;
                    Profile.Hammers += pack.hammers;
                    Profile.Missiles += pack.missiles;
                    PurchaseSystem.Consume(pack.sku);
                    Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, pack.gems, pack.bombs, pack.hammers, pack.missiles, true);
                }
            });
        });

        return this;
    }
}
