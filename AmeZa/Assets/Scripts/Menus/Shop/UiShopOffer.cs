using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopOffer : MonoBehaviour
{
    [SerializeField] private GameObject presenter = null;
    [SerializeField] private LocalText timeLabel = null;
    [SerializeField] private LocalText discountLabel = null;
    [SerializeField] private Button button = null;

    private GlobalConfig.Data.Shop.Package pack = null;

    private void Awake()
    {
        presenter.SetActive(false);
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        PurchaseOffer.Setup(
            GlobalConfig.Offers.startIndex,
            GlobalConfig.Shop.offers.Count,
            GlobalConfig.Offers.firstDelay,
            GlobalConfig.Offers.offerDuration,
            GlobalConfig.Offers.coolTime,
            GlobalConfig.Offers.minResource,
            GlobalConfig.Offers.resourceTime,
            GlobalConfig.Offers.lastPurchaseTime);

#if UNITY_EDITOR
        PurchaseOffer.Setup(1, GlobalConfig.Shop.offers.Count, 10, 15, 10, 99999999, 5, 60);
#endif

        pack = Game.GetOfferPackage();
        if (pack != null)
        {
            PopupQueue.Add(0.5f, () => Game.Instance.OpenPopup<Popup_Offer>().Setup(pack));
        }

        button.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Offer>().Setup(pack));

        var wait = new WaitForSeconds(1);
        while (true)
        {
            pack = Game.GetOfferPackage();
            if (pack != null)
            {
                discountLabel.SetFormatedText(pack.discount);
                timeLabel.SetText(Utilities.TimeToString(PurchaseOffer.RemainedTime, 0));
            }
            presenter.SetActive(pack != null && pack.discount > 0);

            yield return wait;
        }
    }
}
