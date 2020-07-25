using SeganX;
using UnityEngine;

public static class ClassicLogics
{
    public static bool SetPlayModel()
    {
        if (Profile.Hearts < 1)
        {
            Game.Instance.OpenPopup<Popup_BuyHearts>();
            return false;
        }
        else Profile.Hearts--;

        PlayModel.Reset(PlayModel.Type.Classic);
        PlayModel.ballId = Profile.Avatar.BallId;
        PlayModel.level.name = string.Format(LocalizationService.Get(111127), Profile.MaxClassicScore);
        PlayModel.level.theme = Random.Range(0, 1000);
        PlayModel.level.pattern = GlobalFactory.Patterns.Classic.Get();
        PlayModel.level.startBallCount = 5;
        PlayModel.level.minBlockHealth = 1;
        PlayModel.level.maxBlockHealth = 10;

        PlayModel.onPreLose = OnPreLose;
        PlayModel.onLose = OnLose;

        return true;
    }

    private static void OnPreLose(System.Action<bool> exitplaying)
    {
        if (PlayModel.GetScore() < Profile.MaxClassicScore)
        {
            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111128, true, true, ok => exitplaying(ok)).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
        }
        else exitplaying(true);
    }

    private static void OnLose(System.Action<bool> exitplaying)
    {
        int score = PlayModel.GetScore();

        if (score > Profile.MaxClassicScore)
            Profile.MaxClassicScore = score;

        if (score > 0)
        {
            exitplaying(false);
            Game.Instance.OpenPopup<Popup_Lose>().Setup(replayFunc => OnReward(() =>
            {
                if (SetPlayModel())
                    replayFunc();
            }),
            () => OnReward(() => Game.Instance.OpenState<State_Main>(true)));
        }
        else exitplaying(true);
    }

    private static void OnReward(System.Action onClose)
    {
        var reward = PlayModel.GetReward();
        Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, reward.gems, 0, 0, 0, true, onClose);
    }
}
