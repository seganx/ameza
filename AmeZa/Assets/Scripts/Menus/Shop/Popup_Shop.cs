using SeganX;
using System.Collections;
using System.Collections.Generic;
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
        if (onCloseFunc != null)
            onCloseFunc();
    }

    private IEnumerator Start()
    {
        var index = PurchaseOffer.GetOfferIndex(Profile.Gems);
        if (index.Between(0, GlobalConfig.Shop.offers.Count - 1))
        {
            combinedItem.Clone<UiShopItem>().Setup(GlobalConfig.Shop.offers[index]);
        }

        foreach (var item in GlobalConfig.Shop.packages)
        {
            if (item.bombs > 0)
                combinedItem.Clone<UiShopItem>().Setup(item);
            else
                gemsItem.Clone<UiShopItem>().Setup(item);
        }

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
