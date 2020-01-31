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

[System.Serializable]
public class RewardModel
{
    public class Result
    {
        public int gems = 0;
        public int bombs = 0;
        public int hammers = 0;
        public int missiles = 0;
        public bool exist { get { return gems > 0 || bombs > 0 || hammers > 0 || missiles > 0; } }
    }

    [Tooltip("x:Chance y:Min, z:Max")]
    public Vector3Int gems = Vector3Int.zero;
    [Tooltip("x:Chance y:Min, z:Max")]
    public Vector3Int bombs = Vector3Int.zero;
    [Tooltip("x:Chance y:Min, z:Max")]
    public Vector3Int hammers = Vector3Int.zero;
    [Tooltip("x:Chance y:Min, z:Max")]
    public Vector3Int missiles = Vector3Int.zero;

    public Result GetResult()
    {
        var res = new Result();
        if (Random.Range(0, 100) <= gems.x) res.gems = Random.Range(gems.y, gems.z + 1) * 50 / 50;
        if (Random.Range(0, 100) <= bombs.x) res.bombs = Random.Range(bombs.y, bombs.z + 1);
        if (Random.Range(0, 100) <= hammers.x) res.hammers = Random.Range(hammers.y, hammers.z + 1);
        if (Random.Range(0, 100) <= missiles.x) res.missiles = Random.Range(missiles.y, missiles.z + 1);
        return res;
    }
}


public class LevelModel
{
    public int theme = 0;
    public PatternConfig pattern = null;
    public int season = 0;
    public int index = 0;
    public string name = string.Empty;
    public float progress = 0;
    public int startBallCount = 1;
    public int startBallSpeed = 15;
    public int minBlockHealth = 1;
    public int maxBlockHealth = 5;
    public int targetTurns = 0;  // 0 means infinit
    public int targetBalls = 0;
    public int targetBlocks = 0;
    public int targetItem0 = 0;
    public int targetItem1 = 0;
    public RewardModel reward = new RewardModel();
    public bool IsTargetExist { get { return targetBalls > 0 || targetBlocks > 0 || targetItem0 > 0 || targetItem1 > 0; } }
}
