using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GlobalAnalytics : MonoBehaviour
{
    private void Awake()
    {
        GameAnalytics.Initialize();
    }

    public static void NewBuisinessEvent(Online.Purchase.Provider provider, string sku, int price, string token)
    {
        Online.Purchase.Verify(provider, sku, token, (success, payload) =>
        {
            if (success) GameAnalytics.NewBusinessEvent("USD", price / 100, sku, "1", "cartType");
        });
    }

    public static void LevelStart(int season, int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Levels", season.ToString(), level.ToString(), 0);
    }

    public static void LevelWin(int season, int level, int stars)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Levels", season.ToString(), level.ToString(), stars);
    }

    public static void LevelFailed(int season, int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Levels", season.ToString(), level.ToString(), 0);
    }
}
