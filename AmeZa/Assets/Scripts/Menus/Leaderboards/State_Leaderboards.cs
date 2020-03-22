using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Leaderboards : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private UiLeaderboardItem[] legends = null;
    [SerializeField] private UiLeaderboardItem prefabItem = null;
    [SerializeField] private Button playButton = null;

    private static int leagueIndex = 0;
    private static Online.League.Leaderboard leaderboard = null;

    public State_Leaderboards Setup(int index)
    {
        leagueIndex = index;
        return this;
    }

    private void Start()
    {
        Loading.Show();
        UIBackground.Show();
        var info = GlobalConfig.Leagues[leagueIndex];

        title.SetText(info.name);
        prefabItem.gameObject.SetActive(false);
        playButton.onClick.AddListener(() => game.OpenPopup<Popup_LeagueInfo>().Setup(leagueIndex, leaderboard));

        Online.League.GetLeaderboard(info.id, 0, 100, (succeed, board) =>
        {
            Loading.Hide();
            if (succeed)
            {
                UpdatePlayerToList(board);

                board.last.Sort((x, y) => y.score - x.score);
                board.current.Sort((x, y) => y.score - x.score);

                for (int i = 0; i < board.last.Count; i++)
                    board.last[i].rank = i + 1;

                for (int i = 0; i < board.current.Count; i++)
                    board.current[i].rank = i + 1;

                for (int i = 0; i < legends.Length; i++)
                    legends[i].Setup(info.id, i < board.last.Count ? board.last[i] : new Online.League.Profile(), false);

                foreach (var item in board.current)
                    prefabItem.Clone<UiLeaderboardItem>().Setup(info.id, item).gameObject.SetActive(true);
                Destroy(prefabItem.gameObject);

                leaderboard = board;
                UiShowHide.ShowAll(transform);
            }
            else
            {
                Back();
                return;
            }
        });
    }


    private void UpdatePlayerToList(Online.League.Leaderboard board)
    {
        var leaguescore = PlayModel.GetLeagueScore();
        if (leaguescore < 1) return;

        board.current.Sort((x, y) => y.score - x.score);
        if (board.current.Count < 1 || leaguescore < board.current.LastOne().score) return;

        var mine = board.current.Find(x => x.username == Profile.Username);
        if (mine == null)
        {
            board.current.RemoveAt(board.current.LastIndex());
            mine = new Online.League.Profile() { avatar = Profile.Avatar.Json, nickname = Profile.Nickname, status = Profile.Status, username = Profile.Username, score = leaguescore };
            board.current.Add(mine);
        }
        else if (mine.score < leaguescore)
            mine.score = leaguescore;

    }
}
