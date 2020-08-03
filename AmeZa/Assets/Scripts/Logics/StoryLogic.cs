using SeganX;

public static class StoryLogic
{
    private static SeasonModel season = null;
    private static int index = 0;

    public static bool SetPlayModel(SeasonModel seasonModel, int levelIndex)
    {
        season = seasonModel;
        index = levelIndex;
        return Reset();
    }

    public static bool NextLevel()
    {
        if (index < season.levelCount - 1)
        {
            return SetPlayModel(season, index + 1);
        }
        return false;
    }

    public static bool Reset()
    {
        if (Profile.Hearts < 1)
        {
            Game.Instance.OpenPopup<Popup_BuyHearts>();
            return false;
        }
        else Profile.Hearts--;

        PlayModel.Reset(PlayModel.Type.Levels);
        PlayModel.ballId = Profile.Avatar.BallId;
        PlayModel.level = season.GetLevelModel(index, Profile.Skill);

        PlayModel.onWin = exitplaying =>
        {
            Game.Instance.ClosePopup(true);

            Profile.Hearts++;
            GlobalAnalytics.Levels.Complete(season.id, index, PlayModel.GetRewardStars());
            if (season.id > 0)
                Profile.Skill += (Profile.Skill < 0) ? GlobalConfig.Difficulty.winFactorNegative : GlobalConfig.Difficulty.winFactorPositive;

            CheckCinematics(CinematicConfig.Point.End, () =>
            {
                Game.Instance.OpenPopup<Popup_Win>().SetNextTask(nextlevel =>
                {
                    if (nextlevel && NextLevel())
                    {
                        StartPlaying();
                    }
                    else exitplaying(true);
                });
            });
        };

        PlayModel.onLose = exitplaying =>
        {
            GlobalAnalytics.Levels.Fail(season.id, index);
            if (season.id > 0)
                Profile.Skill = Profile.Skill - GlobalConfig.Difficulty.loseFactor;
            exitplaying(true);
        };

        PlayModel.onPreLose = exitplaying =>
        {
            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111005, true, true, exitplaying).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
        };

        return true;
    }

    public static void StartPlaying()
    {
        UIBackground.Hide();
        Game.Instance.OpenState<State_Playing>();
        GlobalAnalytics.Levels.Start(season.id, index);
        CheckCinematics(CinematicConfig.Point.Start, null);
    }

    private static void CheckCinematics(CinematicConfig.Point point, System.Action nextTask)
    {
        int levelIndex = GlobalFactory.Seasons.GetLevelNumber(PlayModel.level.season, PlayModel.level.index);
        var cinematic = GlobalFactory.Cinematics.Get(levelIndex, point);
        if (cinematic != null)
            Game.Instance.OpenPopup<Popup_Cinematic>().Setup(cinematic, nextTask);
        else
            nextTask?.Invoke();
    }
}
