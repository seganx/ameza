using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLeaderboardItem : MonoBehaviour
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText nicknameLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private Image medalImage = null;
    [SerializeField] private Image cupImage = null;
    [SerializeField] private Button button = null;


    public UiLeaderboardItem Setup(int leagueId, Online.League.Profile profile)
    {
        avatar.Setup(profile.avatar);
        nicknameLabel.SetText(profile.nickname);
        descLabel.SetText(profile.status);
        scoreLabel.SetText(profile.score.ToString());
        rankLabel.SetText(profile.rank > 0 ? profile.rank.ToString() : "-");
        if (profile.rank < 4) cupImage.material = null;
        var info = GlobalFactory.Leagues.GetById(leagueId);
        medalImage.sprite = GlobalFactory.Leagues.GetMedal(info, profile.score);

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
