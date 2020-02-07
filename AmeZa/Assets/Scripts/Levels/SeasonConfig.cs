using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Season")]
public class SeasonConfig : ScriptableObject, IResource
{
    [System.Serializable]
    public class SpecialLevel
    {
        public int index = 0;
        public int targetBalls = 0;
        public int targetBlocks = 0;
        public int targetItem0 = 0;
        public int targetItem1 = 0;
        public PatternConfig pattern = null;
    }

    public int theme = 0;
    public int levelCount = 100;
    public Vector2Int maxBlockHealth = Vector2Int.zero;
    public Vector2Int startBallCount = Vector2Int.zero;
    public RewardModel levelReward = new RewardModel();
    public RewardModel finalReward = new RewardModel();

    public List<SpecialLevel> specialLevels = new List<SpecialLevel>();

    public int Id { get; set; }

    public LevelModel GetLevelModel(int index)
    {
        int difficultyCurveId = Mathf.Clamp(Id, 0, GlobalConfig.Difficulty.curves.Length - 1);
        var difficultyCurve = GlobalConfig.Difficulty.curves[difficultyCurveId];

        var res = new LevelModel();
        res.season = Id;
        res.theme = theme;
        res.index = index;
        res.name = GlobalFactory.Seasons.GetLevelNumber(Id, index + 1).ToString();
        res.progress = (index + 1) / (float)levelCount;
        res.startBallCount = Mathf.RoundToInt(Mathf.Lerp(startBallCount.x, startBallCount.y, res.progress));
        res.minBlockHealth = res.startBallCount / 2;
        res.maxBlockHealth = Mathf.RoundToInt(Mathf.Lerp(maxBlockHealth.x, maxBlockHealth.y, difficultyCurve.Evaluate(res.progress)));
        res.reward = levelReward;

        var specialLevel = specialLevels.Find(x => x.index == index);
        if (specialLevel != null)
        {
            res.pattern = specialLevel.pattern;
            res.targetBalls = specialLevel.targetBalls;
            res.targetBlocks = specialLevel.targetBlocks;
            res.targetItem0 = specialLevel.targetItem0;
            res.targetItem1 = specialLevel.targetItem1;
        }
        else
        {
            res.pattern = GlobalFactory.Patterns.GetClamp(index + Id * 4);
            res.targetTurns = 0;
        }

        return res;
    }
}
