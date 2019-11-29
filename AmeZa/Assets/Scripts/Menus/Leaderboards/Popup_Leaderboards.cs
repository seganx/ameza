using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Leaderboards : GameState
{
    [SerializeField] private UiLeaderboardItem prefabItem = null;

    private static int leagueId = 0;

    public Popup_Leaderboards Setup(int id)
    {
        leagueId = id;
        return this;
    }

    private void Start()
    {
        Loading.Show();
        Online.League.GetLeaderboard(leagueId, 0, 100, (succeed, list) =>
        {
            Loading.Hide();
            if (succeed)
            {
                foreach (var item in list)
                    prefabItem.Clone<UiLeaderboardItem>().Setup(leagueId, item);
                Destroy(prefabItem.gameObject);
            }
            else
            {
                Back();
                return;
            }
        });
        UiShowHide.ShowAll(transform);
    }

}
