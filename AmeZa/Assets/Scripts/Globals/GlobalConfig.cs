﻿using SeganX;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfig : StaticConfig<GlobalConfig>
{
    [System.Serializable]
    public class MarketInfo
    {
        public string storeUrl = string.Empty;
        public string rateUrl = string.Empty;
        public string rsaKey = string.Empty;
    }

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
            public int energy = 10;
            public int bombs = 3;
            public int hammers = 5;
            public int missles = 3;
        }

        [System.Serializable]
        public class Socials
        {
            public string contactSurveyUrl = string.Empty;
            public string contactEmailUrl = string.Empty;
            [PersianPreview(lines = 5)]
            public string invitationText = string.Empty;
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
            public int startBallSpeed = 17;
            public int turnsFactor = 50;
            public int ballsFactor = 100;
            public int classicTurnsFactor = 50;
            public int classicBallsFactor = 200;
            public int winFactorPositive = 5;
            public int winFactorNegative = 15;
            public int loseFactor = 5;
            public Vector2Int seasonBlocksFactors = new Vector2Int(5, 30);
            public Vector2Int seasonBallsFactors = new Vector2Int(5, 20);
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
        public class Energy
        {
            public int interval = 900;
        }

        [System.Serializable]
        public class Advertise
        {
            [System.Serializable]
            public class AdPlace : FunAd.Segment
            {
                public string zoneId = string.Empty;
                public int timerId = 50;
                public int interval = 1800;
                public int rewardGems = 100;
                public override string ZoneId => zoneId;
            }

            public AdPlace shopFreeGems = new AdPlace();
            public AdPlace winClaim = new AdPlace();
        }

        [System.Serializable]
        public class Luckyspin
        {
            public int interval = 28800;
            public int gems = 50;
        }

        [System.Serializable]
        public class OfferConfig
        {
            public int startIndex = 2;
            public int firstDelay = 24 * 60 * 60;
            public int minResource = 2400;
            public int resourceTime = 3 * 60 * 60;
            public int offerDuration = 24 * 60 * 60;
            public int coolTime = 48 * 60 * 60;
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

            [System.Serializable]
            public class VIP
            {
                public string sku = string.Empty;
                public int price = 14000;
                public int days = 7;
            }

            public int nicknamePrice = 1200;
            public int statusPrice = 1200;
            public int ballPriceRatio = 45;
            public int heartPrice = 20;
            public int bombPrice = 20;
            public int misslePrice = 10;
            public int hammerPrice = 5;
            
            public List<VIP> vips = new List<VIP>();
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

        [System.Serializable]
        public class Notifications
        {
            public List<SeganX.LocalNotification.Item> items = new List<SeganX.LocalNotification.Item>();
            public string heartFull = string.Empty;
            public string luckySpine = string.Empty;
        }

        public Update update = new Update();
        public Socials socials = new Socials();
        public List<Season> seasons = new List<Season>();
        public List<Difficulty> difficulty = new List<Difficulty>();
        public List<Energy> energy = new List<Energy>() { new Energy() };
        public List<Advertise> advertises = new List<Advertise>() { new Advertise() };
        public List<Luckyspin> lockyspin = new List<Luckyspin>() { new Luckyspin() };
        public List<OfferConfig> offers = new List<OfferConfig>() { new OfferConfig() };
        public List<League> leagues = new List<League>();
        public Friends friends = new Friends();
        public List<Shop> shop = new List<Shop>();
        public List<ProfilePreset> profilePreset = new List<ProfilePreset>() { new ProfilePreset() };
        public List<Notifications> notification = new List<Notifications>() { new Notifications() };
        [HideInInspector] public List<string> jokes = new List<string>();
    }

    public MarketInfo cafebazaar = new MarketInfo();
    public MarketInfo myket = new MarketInfo();

    [Header("Dynamic Data")]
    [SerializeField] private Data data = new Data();

    public string ConfigUrl => $"http://seganx.com/games/ameza/{version}/config.txt?{System.DateTime.Now.Ticks}";
    public string JokesUrl => $"http://seganx.com/games/ameza/jokes.txt?{System.DateTime.Now.Ticks}";
    public string ApiUrl => $"http://seganx.com/games/ameza/api-v1";

    protected override void OnInitialize()
    {
#if UNITY_EDITOR
        if (offline)
        {
            SaveData(data);
            PlayerPrefsEx.SetInt("GlobalConfig.Group", offlineGroup);
        }
#else
        data = LoadData(data);
        if (DebugMode) Console.Enabled = true;
#endif
    }

#if UNITY_EDITOR
    [Space()]
    [InspectorButton(200, "Export as", "OnExport", "Export level data", "OnExportLevelData", false)]
    public bool offline = false;
    public int offlineGroup = 0;

    public void OnExport(object sender)
    {
        Jokes.Clear();
        var path = System.IO.Directory.GetParent(Application.dataPath).Parent.FullName + "/Configs/" + version;
        if (System.IO.Directory.Exists(path) == false)
            System.IO.Directory.CreateDirectory(path);
        var filename = UnityEditor.EditorUtility.SaveFilePanel("Save exported data", path, "config", "txt");
        if (filename.HasContent(4))
            System.IO.File.WriteAllText(filename, JsonUtility.ToJson(data, false), System.Text.Encoding.UTF8);
    }

    public void OnExportLevelData(object sender)
    {
        int profileskill = 0;
        string str = "season,curve x,level number,skill,balls,max block,min block\n";
        for (int i = 0; i < 5; i++)
        {
            var season = GlobalFactory.Seasons.Get(i);
            for (int l = 0; l < season.levelCount; l++)
            {
                var level = season.GetLevelModel(l, profileskill);

                str += i + "," + level.progress + "," + level.name + "," + profileskill + "," + level.startBallCount + "," + level.maxBlockHealth + "," + level.minBlockHealth + "\n";

                //profileskill += Difficulty.winFactorPositive;
            }
        }

        var path = System.IO.Directory.GetParent(Application.dataPath).Parent.FullName + "/Configs/leveldata.csv";
        System.IO.File.WriteAllText(path, str);
        UnityEditor.EditorUtility.RevealInFinder(path);
    }

#endif

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static Data.Update Update { get { return Instance.data.update; } }
    public static Data.Socials Socials { get { return Instance.data.socials; } }
    public static List<Data.Season> Seasons { get { return Instance.data.seasons; } }
    public static Data.Difficulty Difficulty { get { return Instance.data.difficulty[Group % Instance.data.difficulty.Count]; } }
    public static Data.Energy Energy => Instance.data.energy[Group % Instance.data.energy.Count];
    public static Data.Advertise Advertise { get { return Instance.data.advertises[Group % Instance.data.advertises.Count]; } }
    public static Data.Luckyspin Luckyspin { get { return Instance.data.lockyspin[Group % Instance.data.lockyspin.Count]; } }
    public static Data.OfferConfig Offers { get { return Instance.data.offers[Group % Instance.data.offers.Count]; } }
    public static Data.Friends Friends { get { return Instance.data.friends; } }
    public static Data.Shop Shop { get { return Instance.data.shop[Group % Instance.data.shop.Count]; } }
    public static Data.ProfilePreset ProfilePreset { get { return Instance.data.profilePreset[Group % Instance.data.profilePreset.Count]; } }
    public static List<Data.League> Leagues { get { return Instance.data.leagues; } }
    public static Data.Notifications Notifications { get { return Instance.data.notification[Group % Instance.data.notification.Count]; } }

    public static List<string> Jokes { get { return Instance.data.jokes; } }

    public static MarketInfo Market
    {
        get
        {
#if BAZAAR
            return Instance.cafebazaar;
#elif MYKET
            return Instance.myket;
#endif
            return new MarketInfo();
        }
    }

    public static int Group
    {
        get => PlayerPrefsEx.GetInt("GlobalConfig.Group", 0);
        set => PlayerPrefsEx.SetInt("GlobalConfig.Group", value);
    }

    public static bool DebugMode
    {
        get { return PlayerPrefsEx.GetInt("GlobalConfigs.DebugMode", 0) > 0; }
        set
        {
            PlayerPrefsEx.SetInt("GlobalConfigs.DebugMode", value ? 1 : 0);
            Console.Enabled = value;
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
        Console.Enabled = DebugMode;

        Http.DownloadText(Instance.JokesUrl, jokes =>
        {
            var array = jokes.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 0 && array[0] == "seganx_jokes")
            {
                Instance.data.jokes.Clear();
                Instance.data.jokes.AddRange(array);
                Instance.data.jokes.RemoveAt(0);
                SaveData(Instance.data);
            }
        },
        0,
        2);

        return true;
    }

    public static void Save()
    {
        SaveData(Instance.data);
    }
}
