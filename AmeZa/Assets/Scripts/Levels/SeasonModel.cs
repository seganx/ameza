using System.Collections.Generic;
using UnityEngine;

public class SeasonModel
{
    [System.Serializable]
    public class Mission
    {
        public int index = 0;
        public int patternId = 0;
        public int targetBalls = 0;
        public int targetBlocks = 0;
        public int targetItem0 = 0;
        public int targetItem1 = 0;
    }

    public int id = 0;
    public int levelCount = 100;
    public Vector2Int maxBlockHealth = Vector2Int.zero;
    public Vector2Int startBallCount = Vector2Int.zero;
    public RewardModel levelReward = new RewardModel();
    public RewardModel finalReward = new RewardModel();
    public List<Mission> missions = new List<Mission>();

    public LevelModel GetLevelModel(int index, int skillFactor)
    {
        int levelIndex = GlobalFactory.Seasons.GetLevelNumber(id, index);
        var difficultyFactors = id == 0 ? GlobalConfig.Difficulty.levelFactor0 : (id == 1 ? GlobalConfig.Difficulty.levelFactor1 : GlobalConfig.Difficulty.levelFactor2);
        var difficultyValue = difficultyFactors[index % difficultyFactors.Length] * 0.01f;

        var res = new LevelModel();
        res.season = id;
        res.theme = id;
        res.index = index;
        res.name = (levelIndex + 1).ToString();
        res.progress = (index + 1) / (float)levelCount;
        res.startBallCount = Mathf.RoundToInt(Mathf.Lerp(startBallCount.x, startBallCount.y, res.progress));
        res.minBlockHealth = res.startBallCount / 4;
        res.maxBlockHealth = Mathf.RoundToInt(Mathf.Lerp(maxBlockHealth.x, maxBlockHealth.y, difficultyValue));
        res.reward = levelReward;

        res.minBlockHealth = Mathf.Max(res.minBlockHealth + res.minBlockHealth * skillFactor / 5000, 1);
        res.maxBlockHealth = Mathf.Max(res.maxBlockHealth + res.maxBlockHealth * skillFactor / 1000, 5);

        var specialLevel = missions.Find(x => x.index == index);
        if (specialLevel != null)
        {
            res.pattern = GlobalFactory.Patterns.Missions.Get(specialLevel.patternId);
            res.targetBalls = specialLevel.targetBalls;
            res.targetBlocks = specialLevel.targetBlocks;
            res.targetItem0 = specialLevel.targetItem0;
            res.targetItem1 = specialLevel.targetItem1;
        }
        else
        {
            if (levelIndex < GlobalFactory.Patterns.Levels.All.Count)
                res.pattern = GlobalFactory.Patterns.Levels.Get(levelIndex);
            else
                res.pattern = GlobalFactory.Patterns.Randoms.Get(levelIndex);
            res.pattern.randomer = new System.Random(levelIndex * 12345);
            res.targetTurns = 0;
        }

        return res;
    }
}
