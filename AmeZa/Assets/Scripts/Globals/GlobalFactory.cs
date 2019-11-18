using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFactory : StaticConfig<GlobalFactory>
{
    [System.Serializable]
    public class BlocksInfo
    {
        public AnimationCurve moveDownCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });
    }

    [SerializeField] private BlocksInfo blocks = new BlocksInfo();

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
        private static int count = 0;

        public static int Count
        {
            get
            {
                if (count < 1)
                    count = ResourceEx.LoadAll("Balls/", true).LastOne().id + 1;
                return count;
            }
        }

        public static Sprite GetSprite(int id)
        {
            return ResourceEx.Load<Sprite>("Balls/", id);
        }

        public static Ball GetPrefab(int id)
        {
            var res = id > 0 ? ResourceEx.Load<Ball>("Balls/", id) : null;
            if (res == null)
            {
                var sprite = GetSprite(id);
                var color = sprite.GetAverageColor();
                color.a *= 0.5f;

                res = ResourceEx.Load<Ball>("Balls/", 0);
                res.transform.GetComponent<SpriteRenderer>(true, true).sprite = sprite;
                res.transform.GetComponent<TrailRenderer>(true, true).sharedMaterial.color = color;
            }
            return res;
        }

        public static int GetPrice(int id)
        {
            return GlobalConfig.Shop.ballPriceRatio * id;
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
        public static AnimationCurve MoveDownCurve { get { return Instance.blocks.moveDownCurve; } }

        public static BlockBase CreateSimple(Transform parent, int x, int y, int itemIndex, int health)
        {
            var res = Resources.Load<BlockValue>("Blocks/Value").Clone<BlockValue>(parent).Setup(itemIndex, health).SetPosition(x, y);
            Theme.GetSounds(PlayModel.level.theme).Clone<ThemeSounds>(res.transform);
            return res;
        }

        public static BlockBase CreateBall(Transform parent, int x, int y)
        {
            var res = Resources.Load<BlockBall>("Blocks/Ball").Clone<BlockBall>(parent).SetPosition(x, y);
            Theme.GetSounds(PlayModel.level.theme).Clone<ThemeSounds>(res.transform);
            return res;
        }
    }

    public static class Seasons
    {
        private static List<SeasonConfig> all = new List<SeasonConfig>();

        public static List<SeasonConfig> All
        {
            get
            {
                if (all.Count < 1)
                    all = ResourceEx.LoadAllWithId<SeasonConfig>("Game/Seasons/", false);
                return all;
            }
        }

        public static SeasonConfig Get(int id)
        {
            return All.Find(x => x.Id == id);
        }
    }
}
