using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fun.Iab;

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
        if (priceLabel) priceLabel.SetFormatedText(pack.price);
        if (lastPriceLabel) lastPriceLabel.SetFormatedText(pack.lastPrice);
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
            PurchaseSystem.Purchase(PurchaseProvider.Market, sku, (succeed, token) =>
            {
                if (succeed)
                    Purchased(sku, token, () => onClick?.Invoke(true));
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


    public static void Purchased(string sku, string token, System.Action nextTask)
    {
        var pack = GlobalConfig.Shop.GetPackage(sku);
        if (pack == null)
        {
            nextTask?.Invoke();
            return;
        }
        
        Profile.EarnGems(pack.gems);
        Profile.Bombs += pack.bombs;
        Profile.Hammers += pack.hammers;
        Profile.Missiles += pack.missiles;

        Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, pack.gems, pack.bombs, pack.hammers, pack.missiles, true, false, nextTask);

        PurchaseSystem.Consume(pack.sku, (success, msg) =>
        {
            if (success)
            {
                GlobalAnalytics.NewBuisinessEvent(Online.Purchase.Provider.Market, pack.sku, pack.price, token);
                Online.Stats.Set(GlobalConfig.Instance.version, Profile.Gems, Profile.Skill, Profile.GetLevelsPassed(), r => { });
            }
        });

        GlobalAnalytics.SourceGem(pack.gems, pack.sku.Replace("ameza_", ""));
    }
}
