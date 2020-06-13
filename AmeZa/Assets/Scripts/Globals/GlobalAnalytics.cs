using GameAnalyticsSDK;
using SeganX;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GlobalAnalytics : MonoBehaviour
{
    private static GlobalAnalytics instance = null;

    public static int Group { get; private set; }

    private void Awake()
    {
        instance = this;
        GameAnalytics.Initialize();
    }

    private IEnumerator SendBuisinessEvent(Online.Purchase.Provider provider, string sku, int price, string token)
    {
        yield return new WaitForSecondsRealtime(10);
        Online.Purchase.Verify(provider, sku, token, (success, payload) =>
        {
            if (success) GameAnalytics.NewBusinessEvent("USD", price / 100, "pack", sku, "shop");
        });
    }

    public static void SetGroup(int index)
    {
        Group = Mathf.Clamp(index, 0, 3);
        GameAnalytics.SetCustomDimension01("group_" + Group);
    }

    public static void NewBuisinessEvent(Online.Purchase.Provider provider, string sku, int price, string token)
    {
        instance.StartCoroutine(instance.SendBuisinessEvent(provider, sku, price, token));
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

    // A “sink” is when a player loses or spends a resource
    public static void Sink(int amount, string placement, string itemId)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "gem", amount, placement, itemId);
    }

    // A “source” is when a player gains or earns a resource
    public static void Source(int amount, string itemId)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "gem", amount, "earn", itemId);
    }
}
