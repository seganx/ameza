using SeganX;
using System.Collections;
using System.Collections.Generic;
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


    private static int index;
    private static Online.League.Data data = null;
    private static GlobalConfig.Data.League info = null;
    private static Online.League.Leaderboard leaderboard = null;

    public Popup_LeagueInfo Setup(int leagueIndex, Online.League.Leaderboard board)
    {
        data = null;
        index = leagueIndex;
        leaderboard = board;
        return this;
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        UIBackground.Show();
        info = GlobalConfig.Leagues[index];

        title.SetText(info.name);
        descLabel.SetText(info.desc);
        iconImage.sprite = GlobalFactory.Leagues.GetCupSprite(info.playType);
        rewardButton.SetInteractable(false);
        boardButton.SetInteractable(false);
        playButton.SetInteractable(false);

        medalButton.onClick.AddListener(() => game.OpenPopup<Popup_MedalInfo>().Setup(info));
        rewardButton.onClick.AddListener(() => OnRewardButton(info));
        boardButton.onClick.AddListener(() => game.OpenPopup<State_Leaderboards>().Setup(info.id));
        playButton.onClick.AddListener(() => StartGame(info));

        if (data == null)
        {
            Loading.Show();
            Online.League.GetData(info.id, (success, res) =>
            {
                Loading.Hide();
                if (success)
                {
                    data = res;
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
        if (data == null) return;

        var mine = leaderboard.current.Find(x => x.username == Profile.Username);
        if (mine != null)
        {
            if (mine.score > data.score) data.score = mine.score;
            if (mine.rank > data.rank) data.rank = mine.rank;
        }

        var seconds = Online.League.GetRemainedSeconds(data.start_time, data.duration, Online.Timer.CurrentSeconds);
        var subleague = GlobalFactory.Leagues.GetByScore(info, data.score);
        medalImage.sprite = GlobalFactory.Leagues.GetMedal(info, data.score);
        medalLabel.SetText(subleague.name);
        timeLabel.SetFormatedText(TimeToString(seconds));
        scoreLabel.SetFormatedText(data.score);
        rankLabel.SetFormatedText((data.rank > 0 && data.score > 0) ? data.rank.ToString() : "-");
        rewardLabel.SetFormatedText(subleague.rewardGems);
        rewardButton.SetInteractable(data.end_rank > 0);
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

    private void StartGame(GlobalConfig.Data.League info)
    {
        if (Profile.Hearts < 1)
        {
            Game.Instance.OpenPopup<Popup_BuyHearts>();
            return;
        }

        PlayModel.Reset(info.playType);
        PlayModel.ballId = Profile.Avatar.BallId;
        PlayModel.level.name = info.name;
        PlayModel.level.theme = Random.Range(0, 1000);
        PlayModel.level.pattern = GlobalFactory.Patterns.Leagues.Get();
        PlayModel.level.startBallCount = Random.Range(info.startBallCount.x, info.startBallCount.y);
        PlayModel.level.minBlockHealth = PlayModel.level.startBallCount / 2;
        PlayModel.level.maxBlockHealth = PlayModel.level.startBallCount * 3 / 2;

        PlayModel.onPreLose = OnPreLose;
        PlayModel.onLose = () => OnPlayerLose(info.id);

        Back();
        UIBackground.Hide();
        Game.Instance.OpenState<State_Playing>();
    }

    private void OnRewardButton(GlobalConfig.Data.League info)
    {
        if (data == null && data.end_rank > 0) return;
        Loading.Show();
        rewardButton.SetInteractable(false);
        Online.League.SetRewarded(info.id, done =>
        {
            Loading.Hide();
            if (done)
            {
                var subleague = GlobalFactory.Leagues.GetByScore(info, data.end_score);
                Profile.EarnGems(subleague.rewardGems);
                game.OpenPopup<Popup_Rewards>().Setup(0, subleague.rewardGems, 0, 0, 0, true, () => Rateus.AddJoy(2));
                data.end_score = data.end_rank = 0;
            }
            else rewardButton.SetInteractable(true);
        });
    }

    private static void OnPlayerLose(int id)
    {
        int score = PlayModel.GetLeagueScore();
        if (score < data.score) return;

        var hashbase = "seganx_" + data.score + "&" + score + "#(" + id;
        Online.League.SetScore(id, data.score, score, hashbase.ComputeMD5(null).ToLower(), (success, value) => { });
        data.score += score;
    }

    private static void OnPreLose(System.Action<bool> callback)
    {
        var score = PlayModel.GetLeagueScore();
        var maxdelta = PlayModel.type == PlayModel.Type.LeagueBalls ? 10 : 50;
        var nextProfile = GetNextNearProfile(score, maxdelta); // 111058
        var nextMedal = GetNextNearMedal(score, maxdelta); // 111059
        if (nextProfile == null && nextMedal == null)
        {
            callback(true);
            return;
        }

        string confirmStr = string.Empty;
        if (nextMedal != null)
        {
            var scoreDelta = nextMedal.startScore - score;
            var strformat = LocalizationService.Get(111059);
            confirmStr = string.Format(strformat, Profile.Nickname, nextMedal.name, scoreDelta);
        }
        else
        {
            var scoreDelta = nextProfile.score - score;
            var strformat = LocalizationService.Get(111058);
            confirmStr = string.Format(strformat, Profile.Nickname, nextProfile.nickname, scoreDelta);
        }

        game.OpenPopup<Popup_Confirm>().Setup(confirmStr, true, true, ok => callback(ok)).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
    }

    public static Online.League.Profile GetNextNearProfile(int score, int maxScoreDistance)
    {
        Online.League.Profile res = null;
        foreach (var item in leaderboard.current)
        {
            if (item.score <= score) continue;
            var delta = item.score - score;
            if (delta >= maxScoreDistance) continue;
            res = item;
        }
        return res;
    }

    public static GlobalConfig.Data.League.SubLeague GetNextNearMedal(int score, int maxScoreDelta)
    {
        var curr = GlobalFactory.Leagues.GetByScore(info, score);
        var next = GlobalFactory.Leagues.GetNextByScore(info, score);
        if (curr == next) return null;
        var delta = next.startScore - score;
        if (delta <= 0 || delta >= maxScoreDelta) return null;
        return next;
    }

    public override void Back()
    {
        if (tutorial.Hide()) return;
        base.Back();
    }
}
