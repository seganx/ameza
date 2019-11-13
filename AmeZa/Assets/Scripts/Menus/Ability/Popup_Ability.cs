using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Ability : GameState
{
    [SerializeField] private Button bombButton = null;
    [SerializeField] private LocalText bombLabel = null;
    [SerializeField] private Button missleButton = null;
    [SerializeField] private LocalText missleLabel = null;
    [SerializeField] private Button hammerButton = null;
    [SerializeField] private LocalText hammerLabel = null;

    private System.Action<AbilityType> callbackFunc = null;

    public Popup_Ability Setup(System.Action<AbilityType> callback)
    {
        callbackFunc = callback;

        bombLabel.SetText(GlobalConfig.Shop.bombPrice.ToString());
        missleLabel.SetText(GlobalConfig.Shop.bombPrice.ToString());
        hammerLabel.SetText(GlobalConfig.Shop.bombPrice.ToString());

        bombButton.onClick.AddListener(() => Game.SpendGems(GlobalConfig.Shop.bombPrice, () =>
        {
            callback(AbilityType.Bomb);
            base.Back();
        }));

        missleButton.onClick.AddListener(() => Game.SpendGems(GlobalConfig.Shop.misslePrice, () =>
        {
            callback(AbilityType.Missle);
            base.Back();
        }));

        hammerButton.onClick.AddListener(() => Game.SpendGems(GlobalConfig.Shop.hammerPrice, () =>
        {
            callback(AbilityType.Hammer);
            base.Back();
        }));

        UiShowHide.ShowAll(transform);
        return this;
    }

    public override void Back()
    {
        base.Back();
        callbackFunc(AbilityType.Null);
    }
}
