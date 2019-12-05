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
        public LevelConfig config = null;
    }

    public int theme = 0;
    public int levelCount = 100;
    public Vector2Int startBallCount = Vector2Int.zero;
    public Vector2Int minBlockHealth = Vector2Int.zero;
    public Vector2Int maxBlockHealth = Vector2Int.zero;
    public RewardModel levelReward = new RewardModel();
    public RewardModel finalReward = new RewardModel();

    public List<SpecialLevel> specialLevels = new List<SpecialLevel>();

    public int Id { get; set; }

    public LevelModel GetLevelModel(int index)
    {
        var res = new LevelModel();
        res.season = Id;
        res.index = index;
        res.progress = index / (float)levelCount;
        res.name = (index + 1).ToString();
        res.startBallCount = Mathf.RoundToInt(Mathf.Lerp(startBallCount.x, startBallCount.y, res.progress));
        res.minBlockHealth = Mathf.RoundToInt(Mathf.Lerp(minBlockHealth.x, minBlockHealth.y, res.progress));
        res.maxBlockHealth = Mathf.RoundToInt(Mathf.Lerp(maxBlockHealth.x, maxBlockHealth.y, res.progress));
        res.reward = levelReward;

        var specialLevel = specialLevels.Find(x => x.index == index);
        if (specialLevel != null)
        {
            specialLevel.config.SetToModel(res);
        }
        else
        {
            res.targetTurns = 0;
        }

        return res;
    }
}
