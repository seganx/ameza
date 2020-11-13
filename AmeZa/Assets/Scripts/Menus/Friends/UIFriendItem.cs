using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : MonoBehaviour
{
    [SerializeField] private Image background = null;
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText nicknameLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Image medalImage = null;
    [SerializeField] private Image cupImage = null;
    [SerializeField] private LocalText levelLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private LocalText rewardDescLabel = null;
    [SerializeField] private Button rewardButton = null;
    [SerializeField] private Button button = null;

    public UIFriendItem Setup(Online.Friends.Friendship friend, int rank)
    {
        if (avatar) avatar.Setup(friend.avatar);
        if (nicknameLabel) nicknameLabel.SetText(friend.nickname);
        if (descLabel) descLabel.SetText(friend.status);
        if (levelLabel) levelLabel.SetText(friend.level);
        if (rankLabel) rankLabel.SetText(rank.ToString());
        if (cupImage && rank < 4) cupImage.material = null;
        if (background && friend.username == Profile.Username) background.color = Color.green;

        int leagueIndex = GlobalFactory.Friends.GetLeagueIndex(friend.level.ToInt());
        medalImage.sprite = GlobalFactory.Friends.GetMedalSprite(leagueIndex);

        if (friend.username == Profile.Username || Profile.IsFriendRewarded(friend.id.ToInt(), friend.level.ToInt()))
        {
            descLabel.gameObject.SetActive(true);
            medalImage.gameObject.SetActive(true);
            cupImage.gameObject.SetActive(true);
            rewardButton.transform.parent.gameObject.SetActive(false);
        }
        else if (friend.level.ToInt() < GlobalConfig.Friends.minRewardLevel)
        {
            descLabel.gameObject.SetActive(false);
            medalImage.gameObject.SetActive(false);
            cupImage.gameObject.SetActive(false);
            rewardButton.transform.parent.gameObject.SetActive(true);
            rewardButton.SetInteractable(false);
            rewardDescLabel.gameObject.SetActive(true);
            rewardDescLabel.SetFormatedText(friend.level.ToInt(), GlobalConfig.Friends.minRewardLevel);
        }
        else
        {
            descLabel.gameObject.SetActive(false);
            medalImage.gameObject.SetActive(false);
            cupImage.gameObject.SetActive(false);
            rewardButton.transform.parent.gameObject.SetActive(true);
            rewardButton.SetInteractable(true);
            rewardDescLabel.gameObject.SetActive(true);
            rewardDescLabel.SetFormatedText(friend.level.ToInt(), GlobalConfig.Friends.minRewardLevel);

            rewardButton.onClick.AddListener(() =>
            {
                rewardButton.gameObject.SetActive(false);
                Profile.EarnGems(GlobalConfig.Friends.rewardGems);
                Profile.SetFriendRewarded(friend.id.ToInt(), GlobalConfig.Friends.rewardGems);
                Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, GlobalConfig.Friends.rewardGems, 0, 0, 0, true, false, () =>
                {
                    descLabel.gameObject.SetActive(true);
                    medalImage.gameObject.SetActive(true);
                    cupImage.gameObject.SetActive(true);
                    rewardButton.transform.parent.gameObject.SetActive(false);
                });
                GlobalAnalytics.SourceGem(GlobalConfig.Friends.rewardGems, "friend");
            });
        }

        if (friend.username == Profile.Username)
        {
            return this;
        }

        button.onClick.AddListener(() =>
        {
            Loading.Show();
            button.SetInteractable(false);
            Online.Userdata.GetPublic(friend.username, (succeed, data) =>
            {
                Loading.Hide();
                button.SetInteractable(true);
                if (succeed)
                    Game.Instance.OpenPopup<Popup_ProfileInfo>().Setup(friend, data, rank);
                else
                    Game.Instance.OpenPopup<Popup_Confirm>().SetText(111010, friend.nickname).Setup(false, true, null);

            });
        });

        return this;
    }
}
