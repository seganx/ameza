using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public const int LeftEdge = -3;
    public const int TopEdge = 5;
    public const int BottomEdge = -TopEdge + 1;
    public static List<BlockBase> blocks = new List<BlockBase>(64);
    public static bool IsBlockReachedDown { get; private set; }

    private void Awake()
    {
        blocks.Clear();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnBlocks(PlayModel.stats.totalTurn++);
    }

    private void OnMessage(Messages.Param param)
    {
        blocks.RemoveAll(x => x == null);

        switch (param.type)
        {
            case Messages.Type.TurnEnded:
                IsBlockReachedDown = CheckBlocksReached(1);
                if (IsBlockReachedDown == false)
                {
                    for (int i = 0; i < blocks.Count; i++)
                        blocks[i].GoDown();
                    SpawnBlocks(PlayModel.stats.totalTurn++);
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
                    IsBlockReachedDown = CheckBlocksReached(1);
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
        var patt = PlayModel.level.GetPattern();
        var list = patt.GetBlocks(step);
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
        int additionalHealth = PlayModel.stats.totalTurn * GlobalConfig.Difficulty.turnsFactor / 100 + PlayModel.stats.totalBalls * GlobalConfig.Difficulty.ballsFactor / 100;

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

            case BlockType.RandomValue:
                {
                    var health = Random.Range(PlayModel.level.minBlockHealth, PlayModel.level.maxBlockHealth + additionalHealth);
                    PlayModel.stats.totalLevelHealth += health;
                    return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), health);
                }

            default:
                {
                    var health = PlayModel.level.minBlockHealth + additionalHealth + (int)typeValue;
                    PlayModel.stats.totalLevelHealth += health;
                    return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), health);
                }
        }
    }
}
