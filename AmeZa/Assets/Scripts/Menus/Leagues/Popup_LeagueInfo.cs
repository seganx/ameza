using SeganX;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Popup_LeagueInfo : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private LocalText timeLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private LocalText rewardLabel = null;
    [SerializeField] private LocalText rewardLabelTop = null;
    [SerializeField] private LocalText medalLabel = null;
    [SerializeField] private Image medalImage = null;
    [SerializeField] private Button medalButton = null;
    [SerializeField] private Button rewardButton = null;
    [SerializeField] private Button rewardsButton = null;
    [SerializeField] private Button boardButton = null;
    [SerializeField] private Button playButton = null;
    [SerializeField] private UiTutorial tutorial = null;


    public Popup_LeagueInfo Setup(int leagueIndex, Online.League.Leaderboard board)
    {
        LeagueLogics.data = null;
        LeagueLogics.LeagueIndex = leagueIndex;
        LeagueLogics.leaderboard = board;
        return this;
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        UIBackground.Show();
        LeagueLogics.info = GlobalConfig.Leagues[LeagueLogics.LeagueIndex];

        title.SetText(LeagueLogics.info.name);
        descLabel.SetText(LeagueLogics.info.desc);
        iconImage.sprite = GlobalFactory.Leagues.GetCupSprite(LeagueLogics.info.playType);
        boardButton.SetInteractable(false);
        playButton.SetInteractable(false);

        medalButton.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_MedalInfo>().Setup(LeagueLogics.info));
        rewardButton.onClick.AddListener(() => OnRewardButton(LeagueLogics.info));
        boardButton.onClick.AddListener(() => Game.Instance.OpenPopup<State_Leaderboards>().Setup(LeagueLogics.info.id));
        rewardsButton.onClick = medalButton.onClick;

        playButton.onClick.AddListener(() =>
        {
            if (LeagueLogics.SetPlayerModel())
            {
                base.Back();
                UIBackground.Hide();
                Game.Instance.OpenState<State_Playing>();
            }
        });

        if (LeagueLogics.data == null)
        {
            Loading.Show();
            Online.League.GetData(LeagueLogics.info.id, (success, res) =>
            {
                Loading.Hide();
                if (success)
                {
                    LeagueLogics.data = res;
                    UpdateView();
                    UiShowHide.ShowAll(transform);
                }
                else base.Back();
            });
        }
        else UiShowHide.ShowAll(transform);

        tutorial.Display(1, true, 111060, null);

        var wait = new WaitForSecondsRealtime(1);
        while (true)
        {
            UpdateView();
            yield return wait;
        }
    }

    private void UpdateView()
    {
        if (LeagueLogics.data == null) return;

        var mine = LeagueLogics.leaderboard.current.Find(x => x.username == Profile.Username);
        if (mine != null)
        {
            if (mine.score > LeagueLogics.data.score) LeagueLogics.data.score = mine.score;
            if (mine.rank > LeagueLogics.data.rank) LeagueLogics.data.rank = mine.rank;
        }

        var seconds = Online.League.GetRemainedSeconds(LeagueLogics.data.start_time, LeagueLogics.data.duration, Online.Timer.CurrentSeconds);

#if UNITY_EDITOR
        {
            var D = seconds / 86400;
            var H = (seconds % 86400) / 3600;
            var M = (seconds % 3600) / 60;
            var S = seconds % 60;
            Debug.LogWarning("Seconds: " + seconds + " D:" + D + " H:" + H + " M:" + M + " S:" + S);
        }
#endif


        var subleague = GlobalFactory.Leagues.GetByScore(LeagueLogics.info, LeagueLogics.data.score);
        medalImage.sprite = GlobalFactory.Leagues.GetMedal(LeagueLogics.info, LeagueLogics.data.score);
        medalLabel.SetText(subleague.name);
        timeLabel.SetFormatedText(Game.TimeToString(seconds));
        scoreLabel.SetFormatedText(LeagueLogics.data.score);
        rankLabel.SetFormatedText((LeagueLogics.data.rank > 0 && LeagueLogics.data.score > 0) ? LeagueLogics.data.rank.ToString() : "-");
        rewardLabel.SetFormatedText(subleague.rewardGems, subleague.name);
        rewardButton.gameObject.SetActive(LeagueLogics.data.end_rank > 0);
        rewardsButton.gameObject.SetActive(LeagueLogics.data.end_rank <= 0);
        boardButton.SetInteractable(true);
        playButton.SetInteractable(true);

        var topleague = GlobalFactory.Leagues.GetByScore(LeagueLogics.info, 9999999);
        rewardLabelTop.SetFormatedText(topleague.rewardGems, topleague.name);
    }

    private void OnRewardButton(GlobalConfig.Data.League info)
    {
        if (LeagueLogics.data == null && LeagueLogics.data.end_rank > 0) return;
        Loading.Show();
        rewardButton.SetInteractable(false);
        Online.League.SetRewarded(info.id, done =>
        {
            Loading.Hide();
            if (done)
            {
                var subleague = GlobalFactory.Leagues.GetByScore(info, LeagueLogics.data.end_score);
                Profile.EarnGems(subleague.rewardGems);
                Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, subleague.rewardGems, 0, 0, 0, true, false, () => Rateus.AddJoy(2));
                LeagueLogics.data.end_score = LeagueLogics.data.end_rank = 0;
                GlobalAnalytics.SourceGem(subleague.rewardGems, "league");
            }
            else rewardButton.SetInteractable(true);
        });
    }

    public override void Back()
    {
        if (tutorial.Hide()) return;
        base.Back();
    }
}
