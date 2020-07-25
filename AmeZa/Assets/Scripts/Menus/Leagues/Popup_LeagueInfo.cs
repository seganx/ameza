using SeganX;
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
    [SerializeField] private LocalText medalLabel = null;
    [SerializeField] private Image medalImage = null;
    [SerializeField] private Button medalButton = null;
    [SerializeField] private Button rewardButton = null;
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
        rewardButton.SetInteractable(false);
        boardButton.SetInteractable(false);
        playButton.SetInteractable(false);

        medalButton.onClick.AddListener(() => game.OpenPopup<Popup_MedalInfo>().Setup(LeagueLogics.info));
        rewardButton.onClick.AddListener(() => OnRewardButton(LeagueLogics.info));
        boardButton.onClick.AddListener(() => game.OpenPopup<State_Leaderboards>().Setup(LeagueLogics.info.id));

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
                else Back();
            });
        }
        else UiShowHide.ShowAll(transform);

        tutorial.Display(1, true, 111060, null);

        var wait = new WaitForSeconds(1);
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
        var subleague = GlobalFactory.Leagues.GetByScore(LeagueLogics.info, LeagueLogics.data.score);
        medalImage.sprite = GlobalFactory.Leagues.GetMedal(LeagueLogics.info, LeagueLogics.data.score);
        medalLabel.SetText(subleague.name);
        timeLabel.SetFormatedText(TimeToString(seconds));
        scoreLabel.SetFormatedText(LeagueLogics.data.score);
        rankLabel.SetFormatedText((LeagueLogics.data.rank > 0 && LeagueLogics.data.score > 0) ? LeagueLogics.data.rank.ToString() : "-");
        rewardLabel.SetFormatedText(subleague.rewardGems);
        rewardButton.SetInteractable(LeagueLogics.data.end_rank > 0);
        boardButton.SetInteractable(true);
        playButton.SetInteractable(true);
    }

    private string TimeToString(long secconds)
    {
        if (secconds / 86400 > 0) return string.Format(LocalizationService.Get(111006), secconds / 86400);
        if (secconds / 3600 > 0) return string.Format(LocalizationService.Get(111007), secconds / 3600);
        if (secconds / 60 > 0) return string.Format(LocalizationService.Get(111008), secconds / 60);
        return string.Format(LocalizationService.Get(111009), secconds);
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
                game.OpenPopup<Popup_Rewards>().Setup(0, subleague.rewardGems, 0, 0, 0, true, () => Rateus.AddJoy(2));
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
