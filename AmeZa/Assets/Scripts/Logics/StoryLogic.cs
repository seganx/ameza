﻿using SeganX;

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
        if (index < season.levelCount - 1 && Profile.Energy.value > 0)
        {
            return SetPlayModel(season, index + 1);
        }
        return false;
    }

    public static bool Reset()
    {
        if (Profile.Energy.value < 1)
        {
            Game.Instance.OpenPopup<Popup_BuyHearts>();
            return false;
        }

        PlayModel.Reset(PlayModel.Type.Levels);
        PlayModel.ballId = Profile.Avatar.BallId;
        PlayModel.level = season.GetLevelModel(index, Profile.Skill);

        PlayModel.onWin = exitplaying =>
        {
            Game.Instance.ClosePopup(true);

            GlobalAnalytics.Levels.Complete(season.id, index, PlayModel.GetRewardStars());
            if (season.id > 0)
                Profile.Skill += (Profile.Skill < 0) ? GlobalConfig.Difficulty.winFactorNegative : GlobalConfig.Difficulty.winFactorPositive;

            CheckCinematics(CinematicConfig.Point.End, () =>
            {
                Game.Instance.OpenPopup<Popup_Win>().SetNextTask(nextlevel =>
                {
                    if (nextlevel && NextLevel())
                    {
                        StartPlaying(() =>
                        {
                            var levelmodel = season.GetLevelModel(index, Profile.Skill);
                            if (levelmodel.IsTargetExist)
                            {
                                Game.Instance.OpenPopup<Popup_LevelInfo>().Setup(season, index, false);
                            }
                        });
                    }
                    else
                        exitplaying(true);
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
            //Game.Instance.OpenPopup<Popup_Confirm>().Setup(111005, true, true, exitplaying).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
            exitplaying(false);
            if (Reset())
            {
                StartPlaying(() => { });
            }
        };

        return true;
    }

    public static void StartPlaying(System.Action nextTask)
    {
        UIBackground.Hide();
        Game.Instance.ClosePopup(true);
        Game.Instance.OpenState<State_Playing>();
        GlobalAnalytics.Levels.Start(season.id, index);
        CheckCinematics(CinematicConfig.Point.Start, nextTask);
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
