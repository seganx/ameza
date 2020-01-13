using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Offer : GameState
{
    [SerializeField] private UiShopItem presenter = null;
    [SerializeField] private LocalText hint = null;

    private static int HintId
    {
        get { return PlayerPrefs.GetInt("Popup_Offer.HintId", 0); }
        set { PlayerPrefs.SetInt("Popup_Offer.HintId", value); }
    }

    public Popup_Offer Setup(GlobalConfig.Data.Shop.Package pack)
    {
        if (pack != null)
        {
            presenter.Setup(pack, success =>
            {
                PurchaseOffer.SetPurchaseResult(success);
                if (success) Back();
            });
        }
        else
        {
            Back();
        }

        return this;
    }

    private void Awake()
    {
        hint.gameObject.SetActive(false);
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);

        yield return new WaitForSeconds(1);
        hint.gameObject.SetActive(true);
        hint.SetText(LocalizationService.Get(111045 + HintId++ % 5));

        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            if (PurchaseOffer.RemainedTime < 0)
            {
                Back();
            }
            yield return wait;
        }
    }
}
