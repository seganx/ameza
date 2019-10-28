using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public const int OriginX = -3;
    public const int OriginY = 5;

    private void Start()
    {
        SpawnBlocks(0);
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
            CreateBlock(x, y, patt.blocks[i]);
        }
    }

    private object CreateBlock(int x, int y, BlockType typeValue)
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
            case BlockType.Ball: return null;
            case BlockType.Null: return null;
            case BlockType.RandomValue: return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), Random.Range(PlayModel.level.minBlockHealth, PlayModel.level.maxBlockHealth));
        }

        return GlobalFactory.Blocks.CreateSimple(transform, x, y, Random.Range(0, 100), (int)typeValue);
    }
}
