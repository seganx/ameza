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
        prefabItem.gameObject.SetActive(false);
        Loading.Show();
        Online.League.GetLeaderboard(leagueId, 0, 100, (succeed, list) =>
        {
            Loading.Hide();
            if (succeed)
            {
                list.Sort((x, y) => y.score - x.score);
                for (int i = 0; i < list.Count; i++)
                    list[i].rank = i + 1;

                foreach (var item in list)
                    prefabItem.Clone<UiLeaderboardItem>().Setup(leagueId, item).gameObject.SetActive(true);
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
