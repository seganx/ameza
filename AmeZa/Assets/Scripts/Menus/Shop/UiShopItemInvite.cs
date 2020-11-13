using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopItemInvite : MonoBehaviour
{
    [SerializeField] private Button friendsButton = null;
    [SerializeField] private LocalText gemsLabel = null;

    // Start is called before the first frame update
    private void Start()
    {
        gemsLabel.SetFormatedText(GlobalConfig.Friends.rewardGems);

        friendsButton.onClick.AddListener(() =>
        {
            Game.Instance.ClosePopup(true);

            if (Profile.HasNickname)
            {
                Game.Instance.OpenState<State_Friends>();
            }
            else
            {
                Game.Instance.OpenPopup<Popup_Confirm>().Setup(111121, true, false, ok =>
                {
                    if (ok) Game.Instance.OpenPopup<Popup_Profile>().SetOnClose(() =>
                    {
                        if (Profile.HasNickname)
                            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111122, true, false, done => Game.Instance.OpenState<State_Friends>());
                    });
                });
            }
        });
    }

}
