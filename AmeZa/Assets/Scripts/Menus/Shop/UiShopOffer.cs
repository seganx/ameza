﻿using SeganX;
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
            GlobalConfig.OfferConfig.startIndex,
            GlobalConfig.Shop.offers.Count,
            GlobalConfig.OfferConfig.offerDuration,
            GlobalConfig.OfferConfig.coolTime,
            GlobalConfig.OfferConfig.minResource,
            GlobalConfig.OfferConfig.resourceTime,
            GlobalConfig.OfferConfig.lastPurchaseTime);

#if UNITY_EDITOR
        PurchaseOffer.Setup(0, GlobalConfig.Shop.offers.Count, 65, 5, 99999999, 5, 20);
#endif
        pack = GetPackage();
        if (pack != null)
            PopupQueue.Add(1, () => Game.Instance.OpenPopup<Popup_Offer>().Setup(pack));

        button.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Offer>().Setup(pack));

        var wait = new WaitForSeconds(1);
        while (true)
        {
            pack = GetPackage();
            if (pack != null)
            {
                discountLabel.SetFormatedText(pack.discount);
                timeLabel.SetText(Utilities.TimeToString(PurchaseOffer.RemainedTime, 0));
            }
            presenter.SetActive(pack != null && pack.discount > 0);

            yield return wait;
        }
    }

    private static GlobalConfig.Data.Shop.Package GetPackage()
    {
        var index = PurchaseOffer.GetOfferIndex(Profile.Gems);
        if (index.Between(0, GlobalConfig.Shop.offers.Count - 1))
        {
            var pack = GlobalConfig.Shop.offers[index];
            return (pack.discount > 0) ? pack : null;
        }
        return null;
    }
}