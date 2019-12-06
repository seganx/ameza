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
    [SerializeField] private Sprite[] leagueMedals = null;

    protected override void OnInitialize()
    {

    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static class Theme
    {
        public const int Count = 2;

        public static int GetSpriteCount(int themeId)
        {
            themeId = themeId % Count;
            return Resources.LoadAll<Sprite>("Game/Themes/" + themeId + "/Items").Length;
        }

        public static Sprite GetSprite(int themeId, int index)
        {
            themeId = themeId % Count;
            var sprites = Resources.LoadAll<Sprite>("Game/Themes/" + themeId + "/Items");
            return sprites[index % sprites.Length];
        }

        public static Sprite GetBackground(int themeId)
        {
            themeId = themeId % Count;
            return Resources.Load<Sprite>("Game/Themes/" + themeId + "/Background");
        }

        public static ThemeSounds GetSounds(int themeId)
        {
            themeId = themeId % Count;
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
            int ratio = GlobalConfig.Shop.ballPriceRatio;
            return ratio * id * id / 5;
        }
    }

    public static class Patterns
    {
        private static List<PatternConfig> allClamps = new List<PatternConfig>();
        public static List<PatternConfig> AllClamps
        {
            get
            {
                if (allClamps.Count < 1)
                    allClamps = ResourceEx.LoadAll<PatternConfig>("Game/Patterns/Clamps/", false);
                return allClamps;
            }
        }

        public static PatternConfig GetClamp(int index)
        {
            return AllClamps[index % AllClamps.Count];
        }

        public static PatternConfig GetPatternByName(string name)
        {
            return Resources.Load<PatternConfig>("Game/Patterns/" + name);
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

    public static class Leagues
    {
        public static Sprite GetMedalSprite(int index)
        {
            index = Mathf.Clamp(index, 0, Instance.leagueMedals.Length);
            return Instance.leagueMedals[index];
        }

        public static Sprite GetMedal(GlobalConfig.Data.League league, int score)
        {
            return GetMedalSprite(GetSubIndex(league, score));
        }

        public static GlobalConfig.Data.League GetById(int id)
        {
            return GlobalConfig.Leagues.Find(x => x.id == id);
        }

        public static int GetSubIndex(GlobalConfig.Data.League league, int score)
        {
            int res = 0;
            for (int i = 1; i < league.subleagus.Count; i++)
                if (score >= league.subleagus[i].startScore)
                    res = i;
            return res;
        }

        public static GlobalConfig.Data.League.SubLeague GetByScore(GlobalConfig.Data.League league, int score)
        {
            var index = Mathf.Clamp(GetSubIndex(league, score), 0, league.subleagus.Count);
            return league.subleagus[index];
        }
    }
}
