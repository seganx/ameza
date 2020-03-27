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
    [SerializeField] private Sprite[] leagueCups = null;

    protected override void OnInitialize()
    {

    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static class Theme
    {
        private static ThemeConfig selected = null;
        private static List<ResourceEx.File> all = new List<ResourceEx.File>();

        public static ThemeConfig Selected { get { return selected; } }

        public static List<ResourceEx.File> All
        {
            get
            {
                if (all.Count < 1)
                    all = ResourceEx.LoadAll("Game/Themes/", false);
                return all;
            }
        }

        public static ThemeConfig Select(int index)
        {
            var file = All[index % All.Count];
            selected = ResourceEx.Load<ThemeConfig>(file.path);
            return selected;
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

    public static class Blocks
    {
        public static AnimationCurve MoveDownCurve { get { return Instance.blocks.moveDownCurve; } }

        public static BlockBase CreateObstacle(Transform parent, int x, int y)
        {
            var res = Resources.Load<BlockObstacle>("Blocks/Obstacle").Clone<BlockObstacle>(parent).SetPosition(x, y);
            return res;
        }

        public static BlockBase CreateSimple(Transform parent, int x, int y, int itemIndex, int health)
        {
            var res = Resources.Load<BlockValue>("Blocks/Value").Clone<BlockValue>(parent).Setup(itemIndex, health).SetPosition(x, y);
            return res;
        }

        public static BlockBase CreateBall(Transform parent, int x, int y)
        {
            var res = Resources.Load<BlockBall>("Blocks/Ball").Clone<BlockBall>(parent).SetPosition(x, y);
            return res;
        }

        public static BlockBase CreateHorizontalDamage(Transform parent, int x, int y, int health)
        {
            var res = Resources.Load<BlockHorizontalDamage>("Blocks/HorizontalDamage").Clone<BlockHorizontalDamage>(parent).Setup(health).SetPosition(x, y);
            return res;
        }

        public static BlockBase CreateVerticalDamage(Transform parent, int x, int y, int health)
        {
            var res = Resources.Load<BlockVerticalDamage>("Blocks/VerticalDamage").Clone<BlockVerticalDamage>(parent).Setup(health).SetPosition(x, y);
            return res;
        }
    }

    public static class Patterns
    {
        public static class Levels
        {
            private static List<PatternConfig> all = new List<PatternConfig>();
            public static List<PatternConfig> All
            {
                get
                {
                    if (all.Count < 1)
                    {
                        all = ResourceEx.LoadAllWithId<PatternConfig>("Game/Patterns/Levels/", false);
                        all.Sort((x, y) => x.Id - y.Id);
                    }

                    return all;
                }
            }

            public static PatternConfig Get(int index)
            {
                return All[index % All.Count];
            }

#if UNITY_EDITOR
            [UnityEditor.MenuItem("SeganX/Game/Rename All Level Patterns")]
            public static void RenameAllPatterns()
            {
                // avoid name confilict
                for (int i = 0; i < All.Count; i++)
                {
                    var item = All[i];
                    item.name = ((i + 1) * 10000).ToString();
                    string assetPath = UnityEditor.AssetDatabase.GetAssetPath(item.GetInstanceID());
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, item.name);
                }

                for (int i = 0; i < All.Count; i++)
                {
                    var item = All[i];
                    item.name = ((i + 1) * 10).ToString();
                    string assetPath = UnityEditor.AssetDatabase.GetAssetPath(item.GetInstanceID());
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, item.name);
                }
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif

        }

        public static class Leagues
        {
            public static PatternConfig Get()
            {
                var list = ResourceEx.LoadAll("Game/Patterns/Leagues/", true);
                return Resources.Load<PatternConfig>(list.RandomOne().path);
            }
        }

        public static class Randoms
        {
            public static PatternConfig Get(int index)
            {
                var list = ResourceEx.LoadAll("Game/Patterns/Randoms/", true);
                return Resources.Load<PatternConfig>(list[index % list.Count].path);
            }
        }

        public static class Missions
        {
            public static PatternConfig Get(int id)
            {
                return ResourceEx.Load<PatternConfig>("Game/Patterns/Missions/", id);
            }
        }
    }

    public static class Seasons
    {
        private static SeasonModel CreateFrom(int id, GlobalConfig.Data.Season season)
        {
            var res = new SeasonModel();
            res.id = id;
            res.levelCount = season.levels;
            res.maxBlockHealth = season.blocks;
            res.startBallCount = season.balls;
            res.levelReward = season.reward;
            res.finalReward = season.chest;
            foreach (var mission in season.missions)
            {
                var item = new SeasonModel.Mission();
                item.index = mission.id;
                item.patternId = mission.pattern;
                item.targetBalls = mission.balls;
                item.targetBlocks = mission.blocks;
                item.targetItem0 = mission.item0;
                item.targetItem1 = mission.item1;
                res.missions.Add(item);
            }
            return res;
        }

        public static SeasonModel Get(int index)
        {
            if (index < 0) return null;
            var source = (index < GlobalConfig.Seasons.Count) ? GlobalConfig.Seasons[index] : GlobalConfig.Seasons.LastOne();
            var res = CreateFrom(index, source);
            if (index >= GlobalConfig.Seasons.Count)
            {
                int factor = index - GlobalConfig.Seasons.Count + 1;
                res.startBallCount += GlobalConfig.Difficulty.seasonBallsFactors * factor;
                res.maxBlockHealth += GlobalConfig.Difficulty.seasonBlocksFactors * factor;
            }
            return res;
        }

        public static int GetLevelNumber(int seasonId, int levelIndex)
        {
            int res = 0;
            for (int i = 0; i < seasonId; i++)
            {
                var config = Get(i);
                if (config != null)
                    res += config.levelCount;
            }
            return res + levelIndex;
        }
    }

    public static class Leagues
    {
        public static Sprite GetCupSprite(PlayModel.Type type)
        {
            switch (type)
            {
                case PlayModel.Type.Levels: return Instance.leagueCups[0];
                case PlayModel.Type.OneShot: return Instance.leagueCups[1];
                case PlayModel.Type.LeagueBalls: return Instance.leagueCups[0];
                case PlayModel.Type.LeagueBlocks: return Instance.leagueCups[1];
                case PlayModel.Type.LeagueLegends: return Instance.leagueCups[2];
            }
            return Instance.leagueCups[0];
        }

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

        public static GlobalConfig.Data.League.SubLeague GetNextByScore(GlobalConfig.Data.League league, int score)
        {
            var index = Mathf.Clamp(GetSubIndex(league, score) + 1, 0, league.subleagus.Count);
            return league.subleagus[index];
        }
    }

    public static class Friends
    {
        public static string GetMedalName(int index)
        {
            index = Mathf.Clamp(index, 0, GlobalConfig.Friends.leagues.Count - 1);
            return GlobalConfig.Friends.leagues[index].name;
        }

        public static Sprite GetMedalSprite(int index)
        {
            index = Mathf.Clamp(index, 0, Instance.leagueMedals.Length);
            return Instance.leagueMedals[index];
        }

        public static int GetLeagueIndex(int level)
        {
            int res = 0;
            for (int i = 1; i < GlobalConfig.Friends.leagues.Count; i++)
                if (level >= GlobalConfig.Friends.leagues[i].startLevel)
                    res = i;
            return res;
        }
    }

    public static class Jokes
    {
        private static System.DateTime lastTime = new System.DateTime(0);
        private static string[] current = null;

        public static bool Exist
        {
            get
            {
                // verify that current one is consumed
                if (current != null) return true;

                // verification and chance
                if (GlobalConfig.Jokes == null || GlobalConfig.Jokes.Count < 1) return false;
                var deltatime = System.DateTime.Now - lastTime;
                if (deltatime.TotalMinutes < 5) return false;
                if (Random.Range(0, 100) > 25) return false;

                // save time and return new one
                lastTime = System.DateTime.Now;
                var i = Index++ % GlobalConfig.Jokes.Count;
                var joke = GlobalConfig.Jokes[i];
                current = joke.Split('_');
                return true;
            }
        }

        private static int Index
        {
            get { return PlayerPrefs.GetInt("Joke.Index", 0); }
            set { PlayerPrefs.SetInt("Joke.Index", value); }
        }

        public static string[] Pick()
        {
            var res = current;
            current = null;
            return res;
        }
    }

    public static class Cinematics
    {
        private static List<CinematicConfig> all = new List<CinematicConfig>();

        public static List<CinematicConfig> All
        {
            get
            {
                if (all.Count < 1)
                    all = ResourceEx.LoadAll<CinematicConfig>("Game/Cinematics/", false);
                return all;
            }
        }

        public static CinematicConfig Get(int season, int level, CinematicConfig.Point point)
        {
            foreach (var item in All)
            {
                if (item.Check(season, level, point))
                    return item;
            }
            return null;
        }
    }
}
