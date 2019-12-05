using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Profile : GameState
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private InputField nicknameInput = null;
    [SerializeField] private LocalText nicknamePrice = null;
    [SerializeField] private Button nicknameButton = null;
    [SerializeField] private InputField statusInput = null;
    [SerializeField] private LocalText statusPrice = null;
    [SerializeField] private Button statusButton = null;
    [SerializeField] private Slider hairSlider = null;
    [SerializeField] private ScrollRect ballsScroller = null;

    private UiProfileBallItem itemPrefab = null;
    private System.Action onCloseFunc = null;
    private string lastAvatar = string.Empty;

    public Popup_Profile SetOnClose(System.Action onClose)
    {
        onCloseFunc = onClose;
        return this;
    }

    private void Awake()
    {
        lastAvatar = Profile.Avatar.Json;
        itemPrefab = ballsScroller.content.GetComponent<UiProfileBallItem>(true, true);
        itemPrefab.gameObject.SetActive(false);
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);
        avatar.Setup(Profile.Avatar.Current);

        nicknameInput.text = Profile.HasNickname ? Profile.Nickname : Profile.Username;
        nicknamePrice.transform.parent.gameObject.SetActive(Profile.HasNickname);
        nicknamePrice.SetText(GlobalConfig.Shop.nicknamePrice.ToString());
        nicknameButton.onClick.AddListener(() =>
        {
            if (nicknameInput.text.ComputeMD5(Core.Salt) != "AC9CD53769E38CBBD8707CA1108BA10D")
            {
                var nickname = nicknameInput.text.Trim().CleanFromCode().CleanForPersian();
                if (nickname.HasContent(3) && nickname.IsLetterOrDigit() && BadWordsFinder.HasBadWord(nickname) == false)
                {
                    if (Profile.HasNickname)
                        Game.SpendGems(GlobalConfig.Shop.nicknamePrice, () => SendNickname(nickname));
                    else
                        SendNickname(nickname);
                }
                else gameManager.OpenPopup<Popup_Confirm>().Setup(111001, false, true, null);
            }
            else GlobalConfig.DebugMode = true;
        });

        statusInput.text = Profile.Status;
        statusPrice.transform.parent.gameObject.SetActive(Profile.HasStatus);
        statusPrice.SetText(GlobalConfig.Shop.statusPrice.ToString());
        statusButton.onClick.AddListener(() =>
        {
            var status = statusInput.text.Trim().CleanFromCode().CleanForPersian();
            if (status.HasContent(3) && status.IsLetterOrDigit() && BadWordsFinder.HasBadWord(status) == false)
            {
                if (Profile.HasStatus)
                    Game.SpendGems(GlobalConfig.Shop.statusPrice, () => SendStatus(status));
                else
                    SendStatus(status);
            }
            else gameManager.OpenPopup<Popup_Confirm>().Setup(111002, false, true, null);
        });

        hairSlider.minValue = 0;
        hairSlider.maxValue = 360;
        hairSlider.value = Profile.Avatar.Angle;
        hairSlider.onValueChanged.AddListener(value =>
        {
            Profile.Avatar.Angle = value.ToInt();
            avatar.Setup(Profile.Avatar.Current);
            gameManager.CurrentState.Broadcast(Messages.Type.AvatarChanged, Profile.Avatar.Current);
        });

        for (int i = 0; i < GlobalFactory.Balls.Count; i++)
        {
            if (GlobalFactory.Balls.GetSprite(i) != null)
                itemPrefab.Clone<UiProfileBallItem>().Setup(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);
        ballsScroller.content.anchoredPosition = Vector2.down * (UiProfileBallItem.unlockedPosition.y + 100);
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.AvatarChanged))
        {
            avatar.Setup(Profile.Avatar.Current);
        }
    }

    private void SendNickname(string nickname)
    {
        Popup_Loading.Show();
        Online.Profile.SetNickname(nickname, success =>
        {
            Popup_Loading.Hide();
            if (success)
            {
                Profile.Nickname = nickname;
                nicknamePrice.transform.parent.gameObject.SetActive(true);
            }
        });
    }

    private void SendStatus(string status)
    {
        Popup_Loading.Show();
        Online.Profile.SetStatus(status, success =>
        {
            Popup_Loading.Hide();
            if (success)
            {
                Profile.Status = status;
                statusPrice.transform.parent.gameObject.SetActive(true);
            }
        });
    }

    private void SendAvatar()
    {
        var newAvatar = JsonUtility.ToJson(Profile.Avatar.Current);
        if (lastAvatar == newAvatar) return;
        Online.Profile.SetAvatar(newAvatar, res => { });
    }

    public override void Back()
    {
        SendAvatar();
        base.Back();
        if (onCloseFunc != null)
            onCloseFunc();
        Profile.Sync(true, ok => { });
    }
}
