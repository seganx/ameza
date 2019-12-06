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
    public AnimationCurve progressCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
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
        res.theme = theme;
        res.index = index;
        res.name = (index + 1).ToString();
        res.progress = progressCurve.Evaluate(index / (float)levelCount);
        res.startBallCount = Mathf.RoundToInt(Mathf.Lerp(startBallCount.x, startBallCount.y, res.progress));
        res.minBlockHealth = Mathf.RoundToInt(Mathf.Lerp(minBlockHealth.x, minBlockHealth.y, res.progress));
        res.maxBlockHealth = Mathf.RoundToInt(Mathf.Lerp(maxBlockHealth.x, maxBlockHealth.y, res.progress));
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
