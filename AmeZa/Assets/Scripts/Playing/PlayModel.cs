using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayModel
{
    public enum Type { Levels, LeagueBalls, LeagueBlocks, OneShot }

    public class Stats
    {
        public int totalShoots = 0;
        public int totalBalls = 0;
        public int totalBlocks = 0;
        public int totalItem0 = 0;
        public int totalItem1 = 0;
    }

    public static Type type = Type.Levels;
    public static Action onWin = null;
    public static Action onLose = null;
    public static Action onLeave = null;
    public static Stats stats = new Stats();
    public static LevelModel level = new LevelModel();

    public static void Reset(Type newType)
    {
        type = newType;
        stats = new Stats();
        level = new LevelModel();
        onWin = null;
        onLose = null;
        onLeave = null;
    }
}
