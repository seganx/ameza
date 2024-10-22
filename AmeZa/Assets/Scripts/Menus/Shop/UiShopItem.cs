using SeganX;
using System.Collections;
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
        if (priceLabel) priceLabel.SetText(pack.StorePriceLabel);
        if (lastPriceLabel) lastPriceLabel.SetFormatedText(pack.StoreLastPriceLabel);
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
            Plankton.Billing.StartPurchase(sku, (status, token) =>
            {
                if (status == Plankton.Billing.PurchaseStatus.Purchased)
                    ShopLogic.Purchased(sku, token, () => onClick?.Invoke(true));
                else 
                    onClick?.Invoke(false);

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
}
