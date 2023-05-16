using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;
using UnityEngine.UI;

public class Popup_Vip : GameState
{
    [SerializeField] private UiVipButton prefab = null;

    private IEnumerator Start()
    {
        foreach (var item in GlobalConfig.Shop.vips)
            prefab.Clone<UiVipButton>().Setup(item, PostPurchased);

        yield return null;
    }

    private void PostPurchased()
    {

    }
}
