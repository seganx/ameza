using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayModel
{
    public enum Type { Levels, OneShot, LeagueBalls, LeagueBlocks }

    public class Stats
    {
        public int totalTurn = 0;
        public int totalBalls = 0;
        public int totalBlocks = 0;
        public int totalLevelHealth = 0;
        public int totalItem0 = 0;
        public int totalItem1 = 0;
    }

    public static Type type = Type.Levels;
    public static int ballId = 0;
    public static System.Action onWin = null;
    public static System.Action onLose = null;
    public static Stats stats = new Stats();
    public static LevelModel level = new LevelModel();


    public static bool IsClearBlocks { get { return type == Type.Levels && IsTargetExist == false; } }

    public static bool IsTurnsFinished { get { return level.targetTurns > 0 && stats.totalTurn >= level.targetTurns; } }

    public static bool IsTargetExist { get { return level.targetBalls > 0 || level.targetBlocks > 0 || level.targetItem0 > 0 || level.targetItem1 > 0; } }

    public static bool IsTargetsReached
    {
        get
        {
            if (level.targetBalls > 0 && stats.totalBalls < level.targetBalls)
                return false;
            if (level.targetBlocks > 0 && stats.totalBlocks < level.targetBlocks)
                return false;
            if (level.targetItem0 > 0 && stats.totalItem0 < level.targetItem0)
                return false;
            if (level.targetItem1 > 0 && stats.totalItem1 < level.targetItem1)
                return false;
            return true;
        }
    }

    public static void Reset(Type newType)
    {
        type = newType;
        ballId = 0;
        stats = new Stats();
        level = new LevelModel();
        onWin = null;
        onLose = null;
    }

    public static int GetRewardStars()
    {
        var blocks = Mathf.Max(4, stats.totalBlocks);
        var factor = stats.totalTurn / (blocks + 0.1f);
        if (factor < 0.35) return 3;
        if (factor < 0.66f) return 2;
        return 1;
#if OFF
        var turnballs = stats.totalTurn * (level.startBallCount + stats.totalBalls);
        var diff = stats.totalLevelHealth - turnballs;
        if (diff < 1) return 1;
        if (diff < stats.totalLevelHealth * 0.7f) return 2;
        return 3;
#endif
    }

    public static RewardModel.Result GetReward()
    {
        return level.reward.GetResult(level.progress);

    }
}
