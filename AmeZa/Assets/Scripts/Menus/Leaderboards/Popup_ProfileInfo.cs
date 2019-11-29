using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ProfileInfo : GameState
{
    [SerializeField] private LocalText nicknameLabel = null;
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText statusLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private LocalText medalLabel = null;
    [SerializeField] private Image medalImage = null;
    [SerializeField] private Button medalButton = null;
    [SerializeField] private Image ballPrefab = null;

    public Popup_ProfileInfo Setup(int leagueId, Online.League.Profile profile, string userdata)
    {
        avatar.Setup(profile.avatar);
        nicknameLabel.SetText(profile.nickname);
        statusLabel.SetText(profile.status);
        scoreLabel.SetText(profile.score.ToString());
        rankLabel.SetText(profile.rank > 0 ? profile.rank.ToString() : "-");

        var league = GlobalFactory.Leagues.GetById(leagueId);
        var subleague = GlobalFactory.Leagues.GetByScore(league, profile.score);
        medalImage.sprite = GlobalFactory.Leagues.GetMedal(league, profile.score);
        medalLabel.SetText(subleague.name);
        medalButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_MedalInfo>().Setup(league));

        if (userdata != null)
        {
            var data = JsonUtility.FromJson<ProfileData.PublicData>(Utilities.DecompressString(userdata, "{}"));
            data.balls.Reverse();
            foreach (var ballId in data.balls)
                ballPrefab.Clone<Image>().sprite = GlobalFactory.Balls.GetSprite(ballId);
            Destroy(ballPrefab.gameObject);
        }
        else ballPrefab.sprite = GlobalFactory.Balls.GetSprite(0);

        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }
}
