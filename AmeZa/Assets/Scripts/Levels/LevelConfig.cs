using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class LevelConfig : ScriptableObject
{
    public LevelModel.Pattern pattern = LevelModel.Pattern.Indexed;
    public Vector2Int targetBalls = Vector2Int.zero;
    public Vector2Int targetBlocks = Vector2Int.zero;
    public Vector2Int targetItem0 = Vector2Int.zero;
    public Vector2Int targetItem1 = Vector2Int.zero;

    public void SetToModel(LevelModel model)
    {
        model.pattern = pattern;
        model.targetBalls = Mathf.RoundToInt(Mathf.Lerp(targetBalls.x, targetBalls.y, model.progress));
        model.targetBlocks = Mathf.RoundToInt(Mathf.Lerp(targetBlocks.x, targetBlocks.y, model.progress));
        model.targetItem0 = Mathf.RoundToInt(Mathf.Lerp(targetItem0.x, targetItem0.y, model.progress));
        model.targetItem1 = Mathf.RoundToInt(Mathf.Lerp(targetItem1.x, targetItem1.y, model.progress));
    }
}
