using SeganX;
using System.Collections;
using UnityEngine;

public class Popup_Shop : GameState
{
    [SerializeField] public LocalText gemsLabel = null;
    [SerializeField] public UiShopItem combinedItem = null;
    [SerializeField] public UiShopItem gemsItem = null;

    private System.Action onCloseFunc = null;

    public Popup_Shop SetOnClose(System.Action onClose)
    {
        onCloseFunc = onClose;
        return this;
    }

    public override void Back()
    {
        base.Back();
        onCloseFunc?.Invoke();
    }

    private IEnumerator Start()
    {
        var index = PurchaseOffer.GetOfferIndex(Profile.Gems);
        if (index.Between(0, GlobalConfig.Shop.offers.Count - 1))
        {
            var item = combinedItem.Clone<UiShopItem>();
            item.Setup(GlobalConfig.Shop.offers[index].sku, success =>
            {
                PurchaseOffer.SetPurchaseResult(success);
                item.gameObject.SetActive(success == false);
            });
        }

        //GlobalConfig.Shop.combinedPackages.Sort((x, y) => y.price - x.price);
        foreach (var item in GlobalConfig.Shop.combinedPackages)
            combinedItem.Clone<UiShopItem>().Setup(item.sku);

        foreach (var item in GlobalConfig.Shop.gemPackages)
            gemsItem.Clone<UiShopItem>().Setup(item.sku);
        gemsItem.transform.parent.SetAsLastSibling();

        Destroy(combinedItem.gameObject);
        Destroy(gemsItem.gameObject);

        UiShowHide.ShowAll(transform);

        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            gemsLabel.SetText(Profile.Gems.ToString("#,0"));
            yield return wait;
        }
    }

}
