using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_LeagueInfo : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private Transform icons = null;
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


    private static int index;
    private static Online.League.Data data = null;

    public State_LeagueInfo SetLeagueIndex(int leagueIndex)
    {
        data = null;
        index = leagueIndex;
        return this;
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        UIBackground.Show();
        var info = GlobalConfig.Leagues[index];

        title.SetText(info.name);
        descLabel.SetText(info.desc);
        icons.SetActiveChild(index);
        rewardButton.SetInteractable(false);
        boardButton.SetInteractable(false);
        playButton.SetInteractable(false);

        medalButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_MedalInfo>().Setup(info));
        rewardButton.onClick.AddListener(() => OnRewardButton(info));
        boardButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Leaderboards>().Setup(info.id));
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
                    UpdateView(info);
                    UiShowHide.ShowAll(transform);
                }
                else Back();
            });
        }
        else UiShowHide.ShowAll(transform);


        var wait = new WaitForSeconds(1);
        while (true)
        {
            UpdateView(info);
            yield return wait;
        }

    }

    private void UpdateView(GlobalConfig.Data.League info)
    {
        if (data == null) return;
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
        PlayModel.Reset(info.playType);
        PlayModel.ballId = Profile.Avatar.BallId;
        PlayModel.level.name = info.name;
        PlayModel.level.theme = Random.Range(0, 1000);
        PlayModel.level.pattern = GlobalFactory.Patterns.GetLeaguePattern();
        PlayModel.onLose = () => OnPlayerLose(info.id);

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
                gameManager.OpenPopup<Popup_Rewards>().Setup(0, subleague.rewardGems, 0, 0, 0, true, () => Rateus.Joy += 2);
                data.end_score = data.end_rank = 0;
            }
            else rewardButton.SetInteractable(true);
        });
    }

    private static void OnPlayerLose(int id)
    {
        int score = 0;
        switch (PlayModel.type)
        {
            case PlayModel.Type.LeagueBalls: score = PlayModel.stats.totalBalls; break;
            case PlayModel.Type.LeagueBlocks: score = PlayModel.stats.totalBlocks; break;
        }

        if (score < data.score) return;

        var hashbase = "seganx_" + data.score + "&" + score + "#(" + id;
        Online.League.SetScore(id, data.score, score, hashbase.ComputeMD5(null).ToLower(), (success, value) => { });
        data.score += score;
    }
}
