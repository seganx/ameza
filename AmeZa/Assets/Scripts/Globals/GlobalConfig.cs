using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Market : int
{
    Null = 0,
    Irancell = 1,
    Cafebazaar = 2,
    GoogleplayPersian = 2,
    GoogleplayEnglish = 3,
}

public class GlobalConfig : StaticConfig<GlobalConfig>
{

    [System.Serializable]
    public class Data
    {
        [System.Serializable]
        public class Update
        {
            public enum Mode : int
            {
                Null = 0,
                Soft = 1,
                Force = 2,
            }

            public Mode mode = Mode.Null;
            public int rewardGems = 1000;
        }


        [System.Serializable]
        public class ProfilePreset
        {
            public int gems = 100;
            public int hearts = 3;
            public int bombs = 3;
            public int hammers = 5;
            public int missles = 3;
        }

        [System.Serializable]
        public class Socials
        {
            public string storeUrl = string.Empty;
            public string rateUrl = string.Empty;
            public string contactSurveyUrl = string.Empty;
            public string contactEmailUrl = string.Empty;
        }

        [System.Serializable]
        public class League
        {
            [System.Serializable]
            public class SubLeague
            {
                [PersianPreview]
                public string name = string.Empty;
                public int startScore = 0;
                public int rewardGems = 0;
            }

            public int id = 0;
            public PlayModel.Type playType = PlayModel.Type.LeagueBalls;
            public Vector2Int startBallCount = new Vector2Int(10, 30);
            [PersianPreview]
            public string name = string.Empty;
            [PersianPreview]
            public string desc = string.Empty;
            [PersianPreview]
            public string ribbon = string.Empty;

            public List<SubLeague> subleagus = new List<SubLeague>();
        }

        [System.Serializable]
        public class Difficulty
        {
            public int turnsFactor = 50;
            public int ballsFactor = 100;
            public int winFactorPositive = 5;
            public int winFactorNegative = 15;
            public int loseFactor = 5;
            public Vector2Int seasonBlocksFactors = new Vector2Int(5, 30);
            public Vector2Int seasonBallsFactors = new Vector2Int(5, 20);
            public AnimationCurve[] curves = new AnimationCurve[] { new AnimationCurve() };
        }

        [System.Serializable]
        public class Season
        {
            [System.Serializable]
            public class Mission
            {
                public int id = 0;
                public int pattern = 0;
                public int balls = 0;
                public int blocks = 0;
                public int item0 = 0;
                public int item1 = 0;
            }

            public int levels = 20;
            public Vector2Int blocks = Vector2Int.zero;
            public Vector2Int balls = Vector2Int.zero;
            public RewardModel reward = new RewardModel();
            public RewardModel chest = new RewardModel();
            public List<Mission> missions = new List<Mission>();
        }

        [System.Serializable]
        public class Timers
        {
            [System.Serializable]
            public class Timer
            {
                public int id = 0;
                public int duration = 90;
            }

            public Timer heart = new Timer() { id = 1, duration = 600 };
            public Timer luckySpin = new Timer() { id = 2, duration = 86400 };
        }

        [System.Serializable]
        public class OfferConfig
        {
            public int startIndex = 2;
            public int firstDelay = 24 * 60 * 60;
            public int coolTime = 48 * 60 * 60;
            public int minResource = 2400;
            public int resourceTime = 3 * 60 * 60;
            public int offerDuration = 24 * 60 * 60;
            public int lastPurchaseTime = 5 * 24 * 60 * 60;
        }

        [System.Serializable]
        public class Friends
        {
            [System.Serializable]
            public class League
            {
                [PersianPreview]
                public string name = string.Empty;
                public int startLevel = 0;
            }

            public int maxCount = 10;
            public int minRewardLevel = 12;
            public int rewardGems = 50;
            public List<League> leagues = new List<League>();
        }

        [System.Serializable]
        public class Shop
        {
            [System.Serializable]
            public class Package
            {
                public string sku = string.Empty;
                [PersianPreview]
                public string title = string.Empty;
                public int image = 0;
                public int gems = 0;
                public int bombs = 0;
                public int hammers = 0;
                public int missiles = 0;
                public int price = 0;
                public int lastPrice = 0;
                public int discount = 0;
            }

            public int nicknamePrice = 1200;
            public int statusPrice = 1200;
            public int ballPriceRatio = 45;
            public int heartPrice = 20;
            public int bombPrice = 20;
            public int misslePrice = 10;
            public int hammerPrice = 5;
            public List<Package> offers = new List<Package>();
            public List<Package> combinedPackages = new List<Package>();
            public List<Package> gemPackages = new List<Package>();

            public Package GetPackage(string sku)
            {
                var res = offers.Find(x => x.sku == sku);
                if (res != null) return res;
                res = combinedPackages.Find(x => x.sku == sku);
                if (res != null) return res;
                res = gemPackages.Find(x => x.sku == sku);
                if (res != null) return res;
                return null;
            }
        }


        public Update update = new Update();
        public Socials socials = new Socials();
        public List<Season> seasons = new List<Season>();
        public Difficulty difficulty = new Difficulty();
        public Timers timers = new Timers();
        public OfferConfig offers = new OfferConfig();
        public List<League> leagues = new List<League>();
        public Friends friends = new Friends();
        public List<Shop> shop = new List<Shop>();
        public List<ProfilePreset> profilePreset = new List<ProfilePreset>() { new ProfilePreset() };
        public List<string> jokes = new List<string>();
    }

    public Market market = 0;
    public string address = "http://seganx.com/games/ameza/";
    public string cafeBazaarKey = "";

    [Header("Dynamic Data")]
    [SerializeField] private Data data = new Data();

    protected override void OnInitialize()
    {
#if UNITY_EDITOR
        if (offline)
        {
            SaveData(data);
            Cohort = offlineCohort;
        }
        else
#endif
            data = LoadData(data);
        if (DebugMode) SeganX.Console.Logger.Enabled = true;
    }

#if UNITY_EDITOR
    [Space()]
    [InspectorButton(100, "Export as", "OnExport")]
    public bool offline = false;
    public int offlineCohort = 0;

    public void OnExport(object sender)
    {
        var path = System.IO.Directory.GetParent(Application.dataPath).Parent.FullName + "/Configs/" + version;
        if (System.IO.Directory.Exists(path) == false)
            System.IO.Directory.CreateDirectory(path);
        var filename = UnityEditor.EditorUtility.SaveFilePanel("Save exported data", path, "config", "txt");
        if (filename.HasContent(4))
            System.IO.File.WriteAllText(filename, JsonUtility.ToJson(data, false), System.Text.Encoding.UTF8);
    }
#endif

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static Data.Update Update { get { return Instance.data.update; } }
    public static Data.Socials Socials { get { return Instance.data.socials; } }
    public static List<Data.Season> Seasons { get { return Instance.data.seasons; } }
    public static Data.Difficulty Difficulty { get { return Instance.data.difficulty; } }
    public static Data.Timers Timers { get { return Instance.data.timers; } }
    public static Data.OfferConfig Offers { get { return Instance.data.offers; } }
    public static Data.Friends Friends { get { return Instance.data.friends; } }
    public static Data.Shop Shop { get { return Instance.data.shop[Cohort % Instance.data.shop.Count]; } }
    public static Data.ProfilePreset ProfilePreset { get { return Instance.data.profilePreset[Cohort % Instance.data.profilePreset.Count]; } }
    public static List<Data.League> Leagues { get { return Instance.data.leagues; } }
    public static List<string> Jokes { get { return Instance.data.jokes; } }


    private static int Cohort { get; set; }

    public static bool DebugMode
    {
        get { return PlayerPrefsEx.GetInt("GlobalConfigs.DebugMode", 0) > 0; }
        set
        {
            PlayerPrefsEx.SetInt("GlobalConfigs.DebugMode", value ? 1 : 0);
            SeganX.Console.Logger.Enabled = value;
        }
    }

    public static bool SetData(string json)
    {
#if UNITY_EDITOR
        if (Instance.offline)
        {
            //SeganX.Console.Logger.Enabled = Server.debug;
            return true;
        }
#endif

        var newdata = JsonUtility.FromJson<Data>(json);
        if (newdata == null) return false;

        //  set new data
        newdata.jokes = Instance.data.jokes;
        Instance.data = newdata;
        SaveData(newdata);
        SeganX.Console.Logger.Enabled = DebugMode;

        // select cohort
        Cohort = PlayerPrefsEx.GetInt("GlobalConfig.Cohort", -1);
        if (Cohort < 0)
        {
            Cohort = Random.Range(0, 100) % Instance.data.shop.Count;
            PlayerPrefsEx.SetInt("GlobalConfig.Cohort", Cohort);
        }

        var address = Instance.address + "jokes.txt?" + System.DateTime.Now.Ticks;
        Http.DownloadText(address, null, null, jokes =>
        {
            var array = jokes.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 0 && array[0] == "seganx_jokes")
            {
                Instance.data.jokes.Clear();
                Instance.data.jokes.AddRange(array);
                Instance.data.jokes.RemoveAt(0);
                SaveData(Instance.data);
            }
        });

        return true;
    }

}
