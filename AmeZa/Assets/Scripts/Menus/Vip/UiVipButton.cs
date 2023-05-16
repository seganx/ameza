using Fun.Iab;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiVipButton : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private LocalText daysLabel = null;
    [SerializeField] private LocalText priceLabel = null;

    public UiVipButton Setup(GlobalConfig.Data.Shop.VIP pack, System.Action onPurchased)
    {
        daysLabel.SetFormatedText(pack.days);
        priceLabel.SetFormatedText(pack.price);
        button.onClick.AddListener(() =>
        {
            button.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Market, pack.sku, (succeed, token) =>
            {
                if (succeed)
                    ShopLogic.Purchased(pack.sku, onPurchased);

                button.SetInteractable(true);
            });
        });

        return this;
    }
}
