using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;
using UnityEngine.UI;

public class Popup_Vip : GameState
{
    [SerializeField] private UiVipButton prefab = null;
    [SerializeField] private Transform status = null;
    [SerializeField] private LocalText timerLabel = null;

    private System.Action onClose = null;

    public Popup_Vip Setup(System.Action onClose)
    {
        this.onClose = onClose;
        return this;
    }

    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);

        foreach (var item in GlobalConfig.Shop.vips)
            prefab.Clone<UiVipButton>().Setup(item, PostPurchased);
        Destroy(prefab.gameObject);

        var wait = new WaitForSecondsRealtime(1);
        while (true)
        {
            status.SetActiveChild(Profile.IsVIP ? 0 : 1);
            timerLabel.SetFormatedText("00");
            yield return wait;
        }
    }

    private void PostPurchased()
    {

    }

    public override void Back()
    {
        base.Back();
        onClose?.Invoke();
    }
}
