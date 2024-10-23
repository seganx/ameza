using SeganX;
using System.Collections;
using UnityEngine;

public class Profile : MonoBehaviour
{
    private void Awake()
    {
        LoadLocal();
        StartSession();
    }

    private IEnumerator Start()
    {
        var wait = new WaitForSeconds(1);

        while (true)
        {
            yield return wait;

            // update hearts
            int seconds = Online.Timer.GetRemainSeconds(Timers.Energy, GlobalConfig.Energy.interval);
            if (seconds < 0)
            {
                int addhearts = 1 - seconds / GlobalConfig.Energy.interval;
                SetEnergy(Mathf.Clamp(Energy.value + addhearts, 0, GlobalConfig.ProfilePreset.energy));
                Online.Timer.Set(Timers.Energy, GlobalConfig.Energy.interval);
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLocal();
        }
    }

    private void OnApplicationQuit()
    {
        SaveLocal();
    }

    ////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////
    private static ProfileData data = new ProfileData();
    private static System.DateTime lastGetPrfoileTime = System.DateTime.MinValue;
    private static System.DateTime lastSetProfileTime = System.DateTime.MinValue;
    public static bool IsGlobalConfigUpdated { get; set; }
    public static bool IsLoggedIn { get; set; }

    public static bool IsFirstSession => data.privateData.sessions.Get.value == 1;

    public static bool HasNickname => data.nickname.HasContent();
    public static IntResult Gems => data.privateData.gems.Get;

    public static string Nickname
    {
        get { return HasNickname ? data.nickname : "player"; }
        set { data.nickname = value; }
    }

    public static int Version
    {
        get { return data.privateData.version.Get.value; }
        set { data.privateData.version = value; }
    }

    public static int MaxClassicScore
    {
        get { return data.privateData.classicScore.Get.value; }
        set { data.privateData.classicScore = value; }
    }

    public static int Skill
    {
        get => data.privateData.skill.Get.value;
        set => data.privateData.skill = value;
    }

    private static string LastHashdata
    {
        get { return PlayerPrefs.GetString("Profile.LastHashdata", string.Empty); }
        set { PlayerPrefs.SetString("Profile.LastHashdata", value); }
    }

    public static int Sessions
    {
        get => data.privateData.sessions.Get.value;
        set => data.privateData.sessions = value;
    }

    public static BoolResult IsVIP => BoolResult.Set(true);
    public static LongResult VipSeconds => LongResult.Set(999);// data.privateData.vip - Online.Timer.CurrentSeconds);
    public static void SetVip(long value) => data.privateData.vip = value + Online.Timer.CurrentSeconds;

    public static IntResult Energy => data.privateData.energy.Get;
    public static void SetEnergy(int value) => data.privateData.energy.Encrypt(value);

    public static IntResult Bombs => data.privateData.bombs.Get;
    public static void SetBombs(int value) => data.privateData.bombs.Encrypt(value);

    public static IntResult Hammers => data.privateData.hammers.Get;
    public static void SetHammers(int value) => data.privateData.hammers.Encrypt(value);

    public static IntResult Missiles => data.privateData.missles.Get;
    public static void SetMissiles(int value) => data.privateData.missles.Encrypt(value);

    public static void EarnGems(int value)
    {
        if (value > 0)
            data.privateData.gems = data.privateData.gems.Get.value + value;
    }

    public static void SpendGems(int value, System.Action onSuccess, System.Action onFailed = null)
    {
        if (Gems.value >= value)
        {
            data.privateData.gems = data.privateData.gems.Get.value - value;
            onSuccess?.Invoke();
        }
        else onFailed?.Invoke();
    }

    public static BoolResult HasBall(int id)
    {
        return BoolResult.Set(data.privateData.balls.Contains(id));
    }

    public static void AddBall(int id)
    {
        if (data.privateData.balls.Contains(id) == false)
            data.privateData.balls.Add(id);
    }

    public static IntResult GetSeasonRewarded(int seasonId)
    {
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return IntResult.Set(0);
        return IntResult.Set(season.rewarded);
    }

    public static void SetSeasonRewarded(int seasonId, int rewarded)
    {
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return;
        season.rewarded = rewarded;
        SaveLocal();
    }

    public static BoolResult CanOpenLevel(int seasonId, int index)
    {
        if (index < 0) return BoolResult.Set(false);
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null)
            data.privateData.seasons.Add(season = new ProfileData.SeasonData() { id = seasonId });
        return BoolResult.Set(index <= season.levels.Count);
    }

    public static IntResult GetLevelsPassed()
    {
        if (data.privateData.seasons.Count < 1) return IntResult.Set(0);
        data.privateData.seasons.Sort((x, y) => x.id - y.id);
        var season = data.privateData.seasons.LastOne();
        return IntResult.Set(GlobalFactory.Seasons.GetLevelNumber(season.id, season.levels.Count));
    }

    public static IntResult GetSeasonProgress(int seasonId)
    {
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        return IntResult.Set(season == null ? 0 : season.levels.Count);
    }

    public static BoolResult IsLevelPassed(int seasonId, int index)
    {
        if (index < 0) return BoolResult.Set(false);
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return BoolResult.Set(false);
        return BoolResult.Set(index < season.levels.Count);
    }

    // return 0 if no level found
    public static int GetLevelStars(int seasonId, int index)
    {
        if (index < 0) return 0;
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return 0;
        if (season.levels.Count <= index) return 0;
        return season.levels[index];
    }

    public static void SetLevelStars(int seasonId, int index, int stars)
    {
        if (index < 0) return;
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null)
            data.privateData.seasons.Add(season = new ProfileData.SeasonData() { id = seasonId });
        if (season.levels.Count < index) return;
        if (season.levels.Count == index)
            season.levels.Add(stars);
        else if (season.levels[index] < stars)
            season.levels[index] = stars;
        SaveLocal();
    }

    public static void ResetLevels()
    {
        data.privateData.seasons.Clear();
        SaveLocal();
    }

    public static BoolResult IsFriendRewarded(int id, int level)
    {
        if (level < GlobalConfig.Friends.minRewardLevel) return BoolResult.Set(false);
        var item = data.privateData.friends.Find(x => x.id == id);
        if (item == null)
        {
            item = new ProfileData.FriendData();
            item.id = id;
            data.privateData.friends.Add(item);
        }
        return BoolResult.Set(item.rewarded > 0);
    }

    public static void SetFriendRewarded(int id, int gems)
    {
        var item = data.privateData.friends.Find(x => x.id == id);
        if (item == null)
        {
            item = new ProfileData.FriendData();
            item.id = id;
            data.privateData.friends.Add(item);
        }
        item.rewarded = gems;
        SaveLocal();
    }

    public static void SaveLocal()
    {
        PlayerPrefsEx.SetObject("Profile.Data", data);
    }

    public static void LoadLocal()
    {
        data = PlayerPrefsEx.GetObject("Profile.Data", data);
    }

    public static void Reset()
    {
        Loading.Show();
        Application.Quit();
        PlayerPrefs.DeleteAll();
        PlayerPrefsEx.ClearData();
        data = new ProfileData();
        SaveLocal();
    }

    private static void StartSession()
    {
        data.privateData.sessions = data.privateData.sessions.Get.value + 1;
        if (IsFirstSession)
        {
            Online.Timer.Set(Timers.Luckyspin, 300);
            EarnGems(GlobalConfig.ProfilePreset.gems);
            SetEnergy(GlobalConfig.ProfilePreset.energy);
            SetBombs(GlobalConfig.ProfilePreset.bombs);
            SetHammers(GlobalConfig.ProfilePreset.hammers);
            SetMissiles(GlobalConfig.ProfilePreset.missles);

            GlobalAnalytics.SourceGem(GlobalConfig.ProfilePreset.gems, "first");
        }
    }
    [Console("profile", "display", "Display all profile data")]
    public static void Display()
    {
        //Debug.Log(data.GetStringDebug(DebugEx.MemberType.Field, 5));
        Debug.Log(JsonUtility.ToJson(data, true));
    }


    public static class Avatar
    {
        public static int BallId
        {
            get { return data.avatar.ballId; }
            set
            {
                data.avatar.ballId = value;
            }
        }

        public static int Angle
        {
            get { return data.avatar.angle; }
            set
            {
                data.avatar.angle = value;
            }
        }

        public static ProfileData.AvatarData Current { get { return data.avatar; } }
    }
}
