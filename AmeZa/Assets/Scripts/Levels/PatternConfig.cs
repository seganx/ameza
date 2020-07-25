using SeganX;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Pattern")]
public class PatternConfig : ScriptableObject
{
    public enum WrapMode { Clamp, Repeat, Mirror }

    [System.Serializable]
    public class RandomBehave
    {
        public bool activated = false;
        public int startStep = 6;
    }

    public const int width = 7;
    public int height = 6;
    public int startLength = 6;
    public WrapMode wrapMode = WrapMode.Clamp;
    public RandomBehave horizontalRandom = new RandomBehave();
    public RandomBehave verticalRandom = new RandomBehave();
    public List<BlockType> blocks = new List<BlockType>();

    public System.Random randomer = null;
    private int resultStep = -1;
    private List<BlockType> resultList = new List<BlockType>(width * width);

    //public int Id { get; set; }

    public List<BlockType> GetBlocks(int step)
    {
        if (ResultExist(step))
            return resultList;

        if (startLength > 0)
        {
            if (step > 0)
                AddToResult(startLength + step - 1);
            else
                for (int i = startLength - 1; i >= 0; i--)
                    AddToResult(i);
        }
        else AddToResult(step);


        return resultList;
    }

    private bool ResultExist(int step)
    {
        if (step != 0 && resultStep == step) return true;
        resultStep = step;
        resultList.Clear();
        return false;
    }

    private void AddToResult(int step)
    {
        if (wrapMode == WrapMode.Clamp && (step < 0 || step >= height))
        {
            resultList.AddRange(new List<BlockType>(width));
            return;
        }

        List<BlockType> tmp = null;

        if (verticalRandom.activated && verticalRandom.startStep < step)
        {
            if (randomer == null)
                tmp = GetRow(Random.Range(0, 1000) % height);
            else
                tmp = GetRow(randomer.Next(0, 1000) % height);
        }
        else tmp = GetRow(step);

        if (horizontalRandom.activated && horizontalRandom.startStep < step)
            tmp.Shuffle(randomer);

        resultList.AddRange(tmp);
    }

    private List<BlockType> GetRow(int y)
    {
        var res = new List<BlockType>(width);

        switch (wrapMode)
        {
            case WrapMode.Clamp:
                if (y < 0 || y >= height) return res;
                y = (height - (y % height) - 1);
                break;

            case WrapMode.Repeat:
                y = (height - (y % height) - 1);
                break;

            case WrapMode.Mirror:
                y = ((y / height) % 2 == 0) ? (height - (y % height) - 1) : (y % height);
                break;
        }

        int offset = y * width;
        for (int i = 0; i < width; i++)
            res.Add(blocks[offset + i]);

        return res;
    }
}
