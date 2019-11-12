using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Game : GameManager<Game>
{
    public int levelindex = 0;

    private void StartLevel()
    {
        PlayModel.Reset(PlayModel.Type.Levels);
        PlayModel.level = GlobalFactory.Seasons.Get(0).GetLevelModel(levelindex);

        PlayModel.onWin = () =>
        {
            Debug.Log("Play Wins");
            levelindex++;
            StartLevel();
        };

        PlayModel.onLose = () =>
        {
            Debug.Log("Play Lose");
            StartLevel();
        };

        Profile.EarnGems(1000);

        OpenState<State_Main>();
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        Loading.Show();
        Profile.SyncWidthServer(false, succss =>
        {
            Loading.Hide();
            StartLevel();
        });
    }



    public static void SpendGems(int value, System.Action onSuccess)
    {
        if (Profile.SpendGems(value))
        {
            onSuccess();
        }
        else Instance.OpenPopup<Popup_Shop>().SetOnClose(() =>
        {
            if (Profile.SpendGems(value))
                onSuccess();
        });
    }
}
