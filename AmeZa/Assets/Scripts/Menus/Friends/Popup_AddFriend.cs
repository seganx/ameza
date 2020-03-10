using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_AddFriend : GameState
{
    [SerializeField] private Button inviteButton = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private InputField usernameInput = null;
    [SerializeField] private Button addButton = null;

    private System.Action<Online.Friends.Friendship> onSuccessFunc = null;

    public Popup_AddFriend Setup(bool enableAddFriend, System.Action<Online.Friends.Friendship> onSuccess)
    {
        onSuccessFunc = onSuccess;
        addButton.SetInteractable(enableAddFriend);
        return this;
    }

    private void Start()
    {
        descLabel.SetFormatedText(GlobalConfig.Friends.maxCount);

        inviteButton.onClick.AddListener(() =>
        {
            var str = GlobalConfig.Socials.storeUrl + string.Format(LocalizationService.Get(111063), Profile.Username);
            SocialAndSharing.ShareText(str);
        });

        addButton.onClick.AddListener(() =>
        {
            var username = usernameInput.text.CleanFromCode();
            if (username.HasContent(2))
            {
                Loading.Show();
                addButton.SetInteractable(false);
                Online.Friends.Add(username, (success, friend) =>
                {
                    Loading.Hide();
                    Back();
                    if (success && onSuccessFunc != null)
                        onSuccessFunc(friend);
                    else
                        gameManager.OpenPopup<Popup_Confirm>().Setup(111124, true, false, null);
                });
            }
        });

        UiShowHide.ShowAll(transform);
    }

}
