﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public const int OriginX = -3;
    public const int OriginY = 5;
    public const int BottomEdge = -OriginY + 1;
    public static List<BlockBase> blocks = new List<BlockBase>(64);
    public static bool IsBlockReachedDown { get; private set; }

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
                IsBlockReachedDown = CheckBlocksReached(1);
                if (IsBlockReachedDown == false)
                    for (int i = 0; i < blocks.Count; i++)
                        blocks[i].GoDown();
                SpawnBlocks(PlayModel.stats.totalTurn);
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
                    IsBlockReachedDown = CheckBlocksReached(0);
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
        if (step == 0 && PlayModel.level.pattern == LevelModel.Pattern.Indexed)
        {
            DisplayPattern(GlobalFactory.Patterns.Get(PlayModel.level.index));
        }
        else if (step == 0 && PlayModel.level.pattern == LevelModel.Pattern.Random)
        {
            DisplayPattern(GlobalFactory.Patterns.GetRandom());
        }
        else if (PlayModel.level.pattern == LevelModel.Pattern.Procedural)
        {

        }
    }

    private void DisplayPattern(PatternConfig patt)
    {
        for (int i = 0; i < patt.blocks.Count; i++)
        {
            int x = OriginX + (i % PatternConfig.Width);
            int y = OriginY - (i / PatternConfig.Width);
            var block = CreateBlock(x, y, patt.blocks[i]);
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
            case BlockType.RandomValue: return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), Random.Range(PlayModel.level.minBlockHealth, PlayModel.level.maxBlockHealth));
        }

        return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), PlayModel.level.minBlockHealth + (int)typeValue);
    }
}