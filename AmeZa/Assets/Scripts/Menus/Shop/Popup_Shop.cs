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
