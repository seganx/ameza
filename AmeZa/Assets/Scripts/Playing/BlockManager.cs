using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Base
{
    public const int LeftEdge = -3;
    public const int TopEdge = 5;
    public const int BottomEdge = -TopEdge + 1;
    public static List<BlockBase> blocks = new List<BlockBase>(64);
    public static bool IsBlockReachedDown { get; private set; }
    public static bool IsBlockReachedWarn { get; private set; }

    private int usedAbilityCount = 0;

    private void Awake()
    {
        blocks.Clear();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnBlocks(PlayModel.stats.totalTurn);
    }

    private void OnMessage(Messages.Param param)
    {
        blocks.RemoveAll(x => x == null);

        switch (param.type)
        {
            case Messages.Type.TurnEnded:
                IsBlockReachedWarn = CheckBlocksReached(2);
                IsBlockReachedDown = CheckBlocksReached(1);
                if (IsBlockReachedDown == false)
                {
                    for (int i = 0; i < blocks.Count; i++)
                        blocks[i].GoDown();
                    SpawnBlocks(++PlayModel.stats.totalTurn);
                }
                SendMessageUpwards("CheckMission");
                break;

            case Messages.Type.BlockDead:
                blocks.Remove(param.As<BlockBase>());
                break;

            case Messages.Type.UseAbility:
                {
                    var ability = param.As<AbilityType>();
                    var tmplist = new List<BlockBase>(blocks);
                    foreach (var block in tmplist)
                        block.UsedAbility(ability);
                    IsBlockReachedWarn = CheckBlocksReached(2);
                    IsBlockReachedDown = CheckBlocksReached(1);
                    switch (ability)
                    {
                        case AbilityType.Bomb:
                            usedAbilityCount += 15;
                            DelayCall(1, () => SpawnBlocks(++PlayModel.stats.totalTurn));
                            break;
                        case AbilityType.Missle: usedAbilityCount += 7; break;
                        case AbilityType.Hammer: usedAbilityCount += 3; break;
                    }
                }
                break;
        }
    }

    private bool CheckBlocksReached(int offset)
    {
        var edge = BottomEdge + offset;
        for (int i = 0; i < blocks.Count; i++)
            if (blocks[i].Type != BlockType.Ball && blocks[i].Position.y < edge)
                return true;
        return false;
    }

    public void SpawnBlocks(int step)
    {
        var list = PlayModel.level.pattern.GetBlocks(step);

        if (PlayModel.level.pattern.wrapMode == PatternConfig.WrapMode.Clamp)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != BlockType.RandomValue) continue;

                // computing exact row index of the whole pattern
                int row = (list.Count - i - 1) / PatternConfig.width;
                row += step == 0 ? 0 : (PlayModel.level.pattern.startLength + step - 1);

                // interpolate between min and max
                var rowlowe = Mathf.Max(0, row - 3);
                var rowhigh = Mathf.Min(row + 3, PlayModel.level.pattern.height);
                int vallowe = (int)Mathf.LerpUnclamped(PlayModel.level.minBlockHealth, PlayModel.level.maxBlockHealth, (float)rowlowe / PlayModel.level.pattern.height);
                int valhigh = (int)Mathf.LerpUnclamped(PlayModel.level.minBlockHealth, PlayModel.level.maxBlockHealth, (float)rowhigh / PlayModel.level.pattern.height);

                //list[i] = (BlockType)Utilities.RandomDoubleHigh(vallowe, valhigh);
                list[i] = (BlockType)Random.Range(vallowe, valhigh);
            }
        }
        else
        {
            int stepFactor = Mathf.Max(7 - step, 1);
            int turnFactor = (PlayModel.stats.totalTurn - usedAbilityCount) * GlobalConfig.Difficulty.turnsFactor / 100;
            int ballFactor = PlayModel.stats.totalBalls * GlobalConfig.Difficulty.ballsFactor / 100;
            int difficultyHealth = turnFactor + ballFactor;

            // ignore difficulty for first season on campain
            if (PlayModel.type == PlayModel.Type.Levels && PlayModel.level.season < 1)
                difficultyHealth = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == BlockType.RandomValue)
                {
                    int max = (PlayModel.level.maxBlockHealth + difficultyHealth) / stepFactor;
                    list[i] = (BlockType)Utilities.RandomDoubleHigh(PlayModel.level.minBlockHealth, max);
                }
            }
        }

        DisplayBlocks(list);
    }

    private void DisplayBlocks(List<BlockType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int x = LeftEdge + (i % PatternConfig.width);
            int y = TopEdge - (i / PatternConfig.width);
            var block = CreateBlock(x, y, list[i]);
            if (block != null) blocks.Add(block);
        }
    }

    private BlockBase CreateBlock(int x, int y, BlockType typeValue)
    {
        switch (typeValue)
        {
            case BlockType.BoxKill: return null;
            case BlockType.HorizontalKill: return null;
            case BlockType.VerticalKill: return null;
            case BlockType.CrossKill: return null;
            case BlockType.HorizontalDamage: return null;
            case BlockType.VerticalDamage: return null;
            case BlockType.CrossDamage: return null;
            case BlockType.Ball: return GlobalFactory.Blocks.CreateBall(transform, x, y);
            case BlockType.Null: return null;

            default:
                {
                    var health = (int)typeValue;
                    PlayModel.stats.totalLevelHealth += health;
                    return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), health);
                }
        }
    }
}
