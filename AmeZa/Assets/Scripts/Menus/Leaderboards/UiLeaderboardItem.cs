using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UiLeaderboardItem : MonoBehaviour
{
    [SerializeField] private Image background = null;
    [SerializeField] private Color playerColor = Color.red;
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText nicknameLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private Image medalImage = null;
    [SerializeField] private Image cupImage = null;
    [SerializeField] private Button button = null;

    public UiLeaderboardItem Setup(int leagueId, Online.League.Profile profile, bool setCupMaterial = true)
    {
        var info = GlobalFactory.Leagues.GetById(leagueId);

        if (background) background.color = profile.username == Profile.Username ? playerColor : Color.white;
        if (avatar) avatar.Setup(profile.avatar);
        if (nicknameLabel) nicknameLabel.SetText(profile.nickname);
        if (descLabel) descLabel.SetText(profile.status);
        if (scoreLabel) scoreLabel.SetText(profile.score.ToString());
        if (rankLabel) rankLabel.SetText(profile.rank > 0 ? profile.rank.ToString() : "-");
        if (medalImage) medalImage.sprite = GlobalFactory.Leagues.GetMedal(info, profile.score);

        if (cupImage)
        {
            cupImage.sprite = GlobalFactory.Leagues.GetCupSprite(info.playType);
            if (setCupMaterial && profile.rank < 4) cupImage.material = null;
        }

        button.onClick.AddListener(() =>
        {
            Loading.Show();
            button.SetInteractable(false);
            Online.Userdata.GetPublic(profile.username, (succeed, data) =>
            {
                Loading.Hide();
                button.SetInteractable(true);
                if (succeed)
                    Game.Instance.OpenPopup<Popup_ProfileInfo>().Setup(leagueId, profile, data);
                else
                    Game.Instance.OpenPopup<Popup_Confirm>().SetText(111010, profile.nickname).Setup(false, true, null);

            });
        });

        return this;
    }
}
