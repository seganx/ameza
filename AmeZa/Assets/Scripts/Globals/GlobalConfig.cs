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
        public class ProfilePreset
        {
            public int gems = 100;
            public int heats = 3;
            public int bombs = 3;
            public int hammers = 3;
            public int missles = 3;
        }

        [System.Serializable]
        public class Update
        {
            public bool whole = false;
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
            public int startScore = 0;
            public int rewardCoins = 0;
        }

        [System.Serializable]
        public class Difficulty
        {
            public int turnsFactor = 50;
            public int ballsFactor = 100;
        }

        [System.Serializable]
        public class Shop
        {
            [System.Serializable]
            public class CoinPackage
            {
                public string sku = string.Empty;
                public int price = 0;
                public int coins = 0;
                public int image = 0;
            }

            public int nicknamePrice = 1200;
            public int statusPrice = 1200;
            public int ballPriceRatio = 45;
            public int heartPrice = 20;
            public int bombPrice = 20;
            public int misslePrice = 10;
            public int hammerPrice = 5;
            public List<CoinPackage> coinPackages = new List<CoinPackage>();
        }

        public Update forceUpdate = new Update();
        public Socials socials = new Socials();
        public Difficulty difficulty = new Difficulty();
        public List<League> leagues = new List<League>();
        public List<Shop> shop = new List<Shop>();
        public List<ProfilePreset> profilePreset = new List<ProfilePreset>() { new ProfilePreset() };
    }

    public int gameId = 0;
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
        var filename = UnityEditor.EditorUtility.SaveFilePanel("Save exported data", System.IO.Path.GetDirectoryName(Application.dataPath), "config", "txt");
        if (filename.HasContent(4))
            System.IO.File.WriteAllText(filename, JsonUtility.ToJson(data, false), System.Text.Encoding.UTF8);
    }
#endif

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static Data.Update ForceUpdate { get { return Instance.data.forceUpdate; } }
    public static Data.Socials Socials { get { return Instance.data.socials; } }
    public static Data.Difficulty Difficulty { get { return Instance.data.difficulty; } }
    public static Data.Shop Shop { get { return Instance.data.shop[Cohort % Instance.data.shop.Count]; } }
    public static Data.ProfilePreset ProfilePreset { get { return Instance.data.profilePreset[Cohort % Instance.data.profilePreset.Count]; } }

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
        return true;
    }


    public static class Leagues
    {
        public static List<Data.League> list { get { return Instance.data.leagues; } }

        public static Data.League GetByScore(int score)
        {
            var index = Mathf.Clamp(GetIndex(score), 0, list.Count);
            return list[index];
        }


        public static Data.League GetByIndex(int index)
        {
            index = Mathf.Clamp(index, 0, list.Count);
            return list[index];
        }

        public static int GetIndex(int score)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (score >= list[i].startScore)
                    return i;
            }
            return 0;
        }
    }

}
