using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Leaderboards : GameState
{
    [SerializeField] private UiLeaderboardItem[] legends = null;
    [SerializeField] private UiLeaderboardItem prefabItem = null;

    private static int leagueId = 0;

    public Popup_Leaderboards Setup(int id)
    {
        leagueId = id;
        return this;
    }

    private void Start()
    {
        prefabItem.gameObject.SetActive(false);
        Loading.Show();
        Online.League.GetLeaderboard(leagueId, 0, 100, (succeed, board) =>
        {
            Loading.Hide();
            if (succeed)
            {
                board.last.Sort((x, y) => y.score - x.score);
                board.current.Sort((x, y) => y.score - x.score);

                for (int i = 0; i < board.last.Count; i++)
                    board.last[i].rank = i + 1;

                for (int i = 0; i < board.current.Count; i++)
                    board.current[i].rank = i + 1;

                for (int i = 0; i < legends.Length; i++)
                    legends[i].Setup(leagueId, i < board.last.Count ? board.last[i] : new Online.League.Profile(), false);

                foreach (var item in board.current)
                    prefabItem.Clone<UiLeaderboardItem>().Setup(leagueId, item).gameObject.SetActive(true);
                Destroy(prefabItem.gameObject);

                UiShowHide.ShowAll(transform);
            }
            else
            {
                Back();
                return;
            }
        });
    }

}
