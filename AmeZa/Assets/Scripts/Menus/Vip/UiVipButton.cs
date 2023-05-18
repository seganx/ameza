using Fun.Iab;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiVipButton : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private Text daysLabel = null;
    [SerializeField] private Text priceLabel = null;

    public UiVipButton Setup(GlobalConfig.Data.Shop.VIP pack, System.Action onPurchased)
    {
        daysLabel.text = daysLabel.text.Replace("{0}", pack.days.ToString("#,0").Persian());
        priceLabel.text = priceLabel.text.Replace("{0}", pack.price.ToString("#,0").Persian());

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
