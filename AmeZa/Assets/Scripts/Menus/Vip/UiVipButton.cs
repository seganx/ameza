using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UiVipButton : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private Text daysLabel = null;
    [SerializeField] private Text priceLabel = null;

    public UiVipButton Setup(GlobalConfig.Data.Shop.VIP pack, System.Action onPurchased)
    {
        daysLabel.text = daysLabel.text.Replace("{0}", pack.days.ToString("#,0"));
        priceLabel.text = priceLabel.text.Replace("{0}", pack.price.ToString("#,0"));

        button.onClick.AddListener(() =>
        {
            button.SetInteractable(false);
            Plankton.Billing.StartPurchase(pack.sku, (status, token) =>
            {
                if (status == Plankton.Billing.PurchaseStatus.Purchased)
                    ShopLogic.Purchased(pack.sku, token, onPurchased);

                button.SetInteractable(true);
            });
        });

        return this;
    }
}
