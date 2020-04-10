using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public static class LeagueLogics
{
    public static int LeagueIndex;
    public static Online.League.Data data = null;
    public static GlobalConfig.Data.League info = null;
    public static Online.League.Leaderboard leaderboard = null;


    public static bool SetPlayerModel()
    {
        if (Profile.Hearts < 1)
        {
            Game.Instance.OpenPopup<Popup_BuyHearts>();
            return false;
        }
        else Profile.Hearts--;

        PlayModel.Reset(info.playType);
        PlayModel.ballId = Profile.Avatar.BallId;
        PlayModel.level.name = info.name;
        PlayModel.level.theme = Random.Range(0, 1000);
        PlayModel.level.pattern = GlobalFactory.Patterns.Leagues.Get();
        PlayModel.level.startBallCount = Random.Range(info.startBallCount.x, info.startBallCount.y);
        PlayModel.level.minBlockHealth = PlayModel.level.startBallCount / 2;
        PlayModel.level.maxBlockHealth = PlayModel.level.startBallCount * 3 / 2;

        PlayModel.onPreLose = OnPreLose;
        PlayModel.onLose = OnPlayerLose;

        return true;
    }

    private static void OnPlayerLose(System.Action<bool> callback)
    {
        int score = PlayModel.GetLeagueScore();
        if (score > data.score)
        {
            var hashbase = "seganx_" + data.score + "&" + score + "#(" + info.id;
            Online.League.SetScore(info.id, data.score, score, hashbase.ComputeMD5(null).ToLower(), (success, value) => { });
            data.score += score;
        }

        if (score > 0)
        {
            callback(false);
            Game.Instance.OpenPopup<Popup_LeagueLose>();
        }
        else callback(true);
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

        Game.Instance.OpenPopup<Popup_Confirm>().Setup(confirmStr, true, true, ok => callback(ok)).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
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
}
