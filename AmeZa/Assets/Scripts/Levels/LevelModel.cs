using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType : int
{
    BoxKill = -33,
    HorizontalKill = -32,
    VerticalKill = -31,
    CrossKill = -30,
    HorizontalDamage = -22,
    VerticalDamage = -21,
    CrossDamage = -20,
    Ball = -10,
    Null = -1,
    RandomValue = 0,
    Value = 1
}

public class LevelModel
{
    public int theme = 0;
    public PatternConfig pattern = null;
    public int season = 0;
    public int index = 0;
    public float progress = 0;
    public int rewardGems = 0;
    public int startBallCount = 1;
    public int startBallSpeed = 15;
    public int minBlockHealth = 1;
    public int maxBlockHealth = 5;
    public int targetTurns = 0;  // 0 means infinit
    public int targetBalls = 0;
    public int targetBlocks = 0;
    public int targetItem0 = 0;
    public int targetItem1 = 0;

    public PatternConfig GetPattern()
    {
        if (pattern != null)
            return pattern;
        else
            return GlobalFactory.Patterns.Get(index);
    }
}
