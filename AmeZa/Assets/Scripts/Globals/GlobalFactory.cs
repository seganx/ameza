using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFactory : StaticConfig<GlobalFactory>
{
    protected override void OnInitialize()
    {

    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static class Theme
    {
        public static int GetSpriteCount(int themeId)
        {
            return Resources.LoadAll<Sprite>("Game/Themes/" + themeId + "/Items").Length;
        }

        public static Sprite GetSprite(int themeId, int index)
        {
            var sprites = Resources.LoadAll<Sprite>("Game/Themes/" + themeId + "/Items");
            return sprites[index % sprites.Length];
        }

        public static Sprite GetBackground(int themeId)
        {
            return Resources.Load<Sprite>("Game/Themes/" + themeId + "/Background");
        }

        public static ThemeSounds GetSounds(int themeId)
        {
            return Resources.Load<ThemeSounds>("Game/Themes/" + themeId + "/Sounds");
        }
    }

    public static class Balls
    {
        public static int GetCount()
        {
            return ResourceEx.LoadAll("Balls/", true).Count;
        }

        public static Sprite GetSprite(int id)
        {
            return ResourceEx.Load<Sprite>("Balls/", id);
        }

        public static Ball GetPrefab(int id)
        {
            var res = ResourceEx.Load<Ball>("Balls/", id);
            if (res == null)
            {
                res = ResourceEx.Load<Ball>("Balls/", 0);
                res.transform.GetComponent<SpriteRenderer>(true, true).sprite = GetSprite(id);
            }
            return res;
        }
    }

    public static class Patterns
    {
        private static List<PatternConfig> all = new List<PatternConfig>();
        private static int lastRandom = -1;

        public static List<PatternConfig> All
        {
            get
            {
                if (all.Count < 1)
                    all = ResourceEx.LoadAll<PatternConfig>("Game/Patterns/", false);
                return all;
            }
        }

        public static PatternConfig Get(int index)
        {
            return All[index % All.Count];
        }

        public static PatternConfig GetRandom()
        {
            var index = Random.Range(0, All.Count);
            if (index == lastRandom) Random.Range(0, All.Count);
            if (index == lastRandom) Random.Range(0, All.Count);
            if (index == lastRandom) Random.Range(0, All.Count);
            if (index == lastRandom) Random.Range(0, All.Count);
            lastRandom = index;
            return Get(index);
        }
    }

    public static class Blocks
    {
        public static BlockSimple CreateSimple(Transform parent, int x, int y, int itemIndex, int health)
        {
            return Resources.Load<BlockSimple>("Blocks/Simple").Clone<BlockSimple>(parent).Setup(x, y, itemIndex, health);
        }
    }
}
