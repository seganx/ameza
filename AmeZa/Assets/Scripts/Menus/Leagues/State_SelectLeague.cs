using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class State_SelectLeague : GameState
{
    [SerializeField] private Button friendsButton = null;

    private void Start()
    {
        friendsButton.onClick.AddListener(() =>
        {
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
                            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111122, true, false, null);
                    });
                });
            }
        });

        UIBackground.Show();
        if (Profile.IsLoggedIn)
        {
            UiShowHide.ShowAll(transform);
        }
        else
        {
            Loading.Show();
            Profile.Sync(false, success =>
            {
                Loading.Hide();
                if (success)
                {
                    UiShowHide.ShowAll(transform);
                }
                else
                {
                    Back();
                }
            });
        }

    }
}
