﻿using Fun.Iab;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShopLogic 
{
    public static void Purchased(string sku, System.Action nextTask)
    {
        var pack = GlobalConfig.Shop.GetPackage(sku);
        if (pack != null)
        {
            PurchasedPackage(pack, nextTask);
            return;
        }

        var vip = GlobalConfig.Shop.vips.Find(x => x.sku == sku);
        if (vip != null)
        {
            PurchasedVip(vip, nextTask);
            return                 ;
        }

        nextTask?.Invoke();
    }

    private static void PurchasedPackage(GlobalConfig.Data.Shop.Package pack, System.Action nextTask)
    {
        Profile.EarnGems(pack.gems);
        Profile.Bombs += pack.bombs;
        Profile.Hammers += pack.hammers;
        Profile.Missiles += pack.missiles;

        Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, pack.gems, pack.bombs, pack.hammers, pack.missiles, true, false, nextTask);

        PurchaseSystem.Consume(pack.sku, (success, msg) =>
        {
            if (success)
            {
                GlobalAnalytics.NewBusinessEvent(pack.price, pack.sku);
                Online.Stats.Set(GlobalConfig.Instance.version, Profile.Gems, Profile.Skill, Profile.GetLevelsPassed(), r => { });
            }
        });

        GlobalAnalytics.SourceGem(pack.gems, pack.sku.Replace("ameza_", ""));
    }

    private static void PurchasedVip(GlobalConfig.Data.Shop.VIP pack, System.Action nextTask)
    {

    }
}