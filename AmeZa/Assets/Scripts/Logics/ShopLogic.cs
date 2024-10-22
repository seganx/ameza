using Plankton;
using SeganX;
using System.Collections.Generic;
using UnityEngine;

public static class ShopLogic
{
    public static void Purchased(string sku, string token, System.Action nextTask)
    {
        var pack = GlobalConfig.Shop.GetPackage(sku);
        if (pack != null)
        {
            PurchasedPackage(pack, token, nextTask);
            return;
        }

        var vip = GlobalConfig.Shop.vips.Find(x => x.sku == sku);
        if (vip != null)
        {
            PurchasedVip(vip, token, nextTask);
            return;
        }

        nextTask?.Invoke();
    }

    private static void PurchasedPackage(GlobalConfig.Data.Shop.Package pack, string token, System.Action nextTask)
    {
        Profile.EarnGems(pack.gems);
        Profile.SetBombs(Profile.Bombs.value + pack.bombs);
        Profile.SetHammers(Profile.Hammers.value + pack.hammers);
        Profile.SetMissiles(Profile.Missiles.value + pack.missiles);

        Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, pack.gems, pack.bombs, pack.hammers, pack.missiles, true, false, nextTask);

        Billing.FinishPurchase(pack.sku, (success, msg) =>
        {
            if (success)
            {
                GlobalAnalytics.NewBusinessEvent(pack.price, pack.sku);
            }
        });

        GlobalAnalytics.SourceGem(pack.gems, pack.sku.Replace("ameza_", ""));
    }

    private static void PurchasedVip(GlobalConfig.Data.Shop.VIP pack, string token, System.Action nextTask)
    {
        var remained = Profile.VipSeconds.value;
        if (remained < 0) remained = 0;
        var newtime = pack.days * 86400;
        Profile.SetVip(remained + newtime);
        nextTask?.Invoke();

        Plankton.Billing.FinishPurchase(pack.sku, (success, msg) =>
        {
            if (success)
            {
                GlobalAnalytics.NewBusinessEvent(pack.price, pack.sku);
            }
        });
    }

    public static void Initialize(System.Action nextTask)
    {
        var all = GlobalConfig.Shop.GetAll();

        foreach (var item in all)
        {
            var price = item.price / 100.0f;
            var realPrice = Mathf.Ceil((price * 100) / (100 - item.discount));
            item.StorePriceLabel = $"${price:#,0.00}";
            item.StoreDiscountLabel = $"%{item.discount} Discount!";
            item.StoreLastPriceLabel = $"instead of ${realPrice:#,0.00}";
            Debug.Log($"{item.sku} | {item.StorePriceLabel} | {item.StoreDiscountLabel} | {item.StoreLastPriceLabel}");
        }

        GetSkuDetails(all, nextTask);
    }

    public static void GetSkuDetails(List<GlobalConfig.Data.Shop.PricedPackage> all, System.Action nextTask)
    {
        var skus = new List<string>();
        all.ForEach(x => skus.Add(x.sku));
        Debug.Log($"skus: {string.Join(',', skus)}");
        Billing.GetSkuDetails((succeed, list) =>
        {
            if (list.Count > 0)
            {
                list.ForEach(x => Debug.Log(x));
                all.ForEach(x => UpdatePackagePrice(x, list.Find(y => y.sku == x.sku)));
            }
            nextTask?.Invoke();
        },
        skus.ToArray());
    }

    private static void UpdatePackagePrice(GlobalConfig.Data.Shop.PricedPackage pack, Billing.Detail details)
    {
        if (details == null) return;

        var realPrice = Mathf.Ceil((details.priceAmount * 100) / (100 - pack.discount));
        pack.StorePriceLabel = details.priceFormatted;
        pack.StoreDiscountLabel = $"%{pack.discount} Discount!";
        pack.StoreLastPriceLabel = $"instead of {realPrice:#,0.00}";

        Debug.Log($"{pack.sku} | {pack.StorePriceLabel} | {pack.StoreDiscountLabel} | {pack.StoreLastPriceLabel}");
    }
}
