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

        int syncSeconds = 0;
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

            syncSeconds++;
            if (syncSeconds % 10 == 0)
                Sync(true, ok => { });
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

    public static bool IsFirstSession { get { return data.privateData.sessions.Get.value == 1; } }

    public static string Username { get { return data.Info.username; } }

    public static string Password { get { return data.Info.password; } }

    public static bool HasNickname { get { return data.Info.nickname.HasContent(); } }

    public static bool HasStatus { get { return data.Info.status.HasContent(); } }

    public static IntResult Gems => data.privateData.gems.Get;

    public static string Nickname
    {
        get { return HasNickname ? data.Info.nickname : (data.Info.username.HasContent() ? data.Info.username : "player"); }
        set { data.Info.nickname = value; }
    }

    public static string Status
    {
        get { return data.Info.status; }
        set { data.Info.status = value; }
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

    public static BoolResult IsVIP => BoolResult.Set(VipSeconds.value > 0);
    public static LongResult VipSeconds => LongResult.Set(data.privateData.vip - Online.Timer.CurrentSeconds);
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
        if (season == null) return;
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
        Online.Profile.SetNickname(string.Empty, successName =>
        Online.Profile.SetStatus(string.Empty, successStatus =>
        Online.Profile.SetAvatar(string.Empty, successAvatar =>
        {
            Application.Quit();
            PlayerPrefs.DeleteAll();
            PlayerPrefsEx.ClearData();
            data.privateData = new ProfileData.PrivateData();
            data.publicData = new ProfileData.PublicData();
            data.avatar = new ProfileData.AvatarData();
            data.Info.avatar = JsonUtility.ToJson(data.avatar);
            StartSession();
            SaveLocal();
        })));
    }

    private static void StartSession()
    {
        data.privateData.sessions = data.privateData.sessions.Get.value + 1;
        if (IsFirstSession)
        {
            EarnGems(GlobalConfig.ProfilePreset.gems);
            SetEnergy(GlobalConfig.ProfilePreset.energy);
            SetBombs(GlobalConfig.ProfilePreset.bombs);
            SetHammers(GlobalConfig.ProfilePreset.hammers);
            SetMissiles(GlobalConfig.ProfilePreset.missles);

            GlobalAnalytics.SourceGem(GlobalConfig.ProfilePreset.gems, "first");
        }
    }

    public static void Sync(bool sendProfile, System.Action<bool> nextTask)
    {
        SaveLocal();

        if (IsGlobalConfigUpdated == false)
        {
            Http.DownloadText(GlobalConfig.Instance.ConfigUrl, json =>
            {
                IsGlobalConfigUpdated = (json != null && GlobalConfig.SetData(json));
                if (IsGlobalConfigUpdated)
                    LoginToServer(sendProfile, nextTask);
                else
                    nextTask(false);
            });
        }
        else LoginToServer(sendProfile, nextTask);
    }

    private static void LoginToServer(bool sendProfile, System.Action<bool> nextTask)
    {
        if (IsLoggedIn)
        {
            GetProfile(sendProfile, nextTask);
            return;
        }

        Online.Login(Core.GameId, Core.DeviceId, success =>
        {
            IsLoggedIn = success;
            if (IsLoggedIn)
                GetProfile(sendProfile, nextTask);
            else
                nextTask(false);
        });
    }

    private static void GetProfile(bool sendProfile, System.Action<bool> nextTask)
    {
        if ((System.DateTime.Now - lastGetPrfoileTime).TotalMinutes < 10)
        {
            SyncProfile(sendProfile, data.Info, nextTask);
            return;
        }

        Online.Profile.Get((success, srverdata) =>
        {
            if (success && srverdata != null)
            {
                lastGetPrfoileTime = System.DateTime.Now;
                SyncProfile(sendProfile, srverdata, nextTask);
            }
            else nextTask(false);
        });
    }

    private static void SyncProfile(bool sendProfile, Online.Profile.Data serverProfile, System.Action<bool> nextTask)
    {
        // check to see if local data is empty then ther are 2 states
        // 1- we are in first run
        // 2- we lost our cache files
        if (Username.IsNullOrEmpty())
        {
            // check to see if we are in first run
            if (serverProfile.datahash.IsNullOrEmpty())
            {
                data.Info = serverProfile;
                RunFirstSission();
                SendProfileData(sendProfile, nextTask);
            }
            // we have data on server but we have lost data on local
            // so try to load from server to local
            else
            {
                data.Info = serverProfile;
                Online.Userdata.Get((sucess, privateStr, publicStr) =>
                {
                    if (sucess)
                    {
                        data.privateData = JsonUtility.FromJson<ProfileData.PrivateData>(Utilities.DecompressString(privateStr, "{}"));
                        data.publicData = JsonUtility.FromJson<ProfileData.PublicData>(Utilities.DecompressString(publicStr, "{}"));
                        SaveLocal();
                        nextTask(true);
                    }
                    else nextTask(false);
                });
            }
        }
        // local data is not empty
        else
        {
            data.Info = serverProfile;

            // check to see if player is marked as hacher?
            if (serverProfile.datahash == "shame")
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefsEx.ClearData();
                data.privateData = new ProfileData.PrivateData();
                data.publicData = new ProfileData.PublicData();
                StartSession();
                SaveLocal();
            }

            // finally just send data to server
            SendProfileData(sendProfile, nextTask);
        }
    }

    private static void SendProfileData(bool sendProfile, System.Action<bool> nextTask)
    {
        if (sendProfile == false || LastHashdata == data.privateData.Datahash || (System.DateTime.Now - lastSetProfileTime).TotalMinutes < 3)
        {
            nextTask(true);
            return;
        }

        lastSetProfileTime = System.DateTime.Now;
        data.publicData.balls = data.privateData.balls;
        string privateString = Utilities.CompressString(JsonUtility.ToJson(data.privateData), null);
        string publicString = Utilities.CompressString(JsonUtility.ToJson(data.publicData), null);
        Online.Userdata.Set(data.privateData.Datahash, privateString, publicString, success =>
        {
            if (success)
            {
                LastHashdata = data.privateData.Datahash;
                nextTask(true);
            }
            else nextTask(false);
        });
    }

    private static void RunFirstSission()
    {
        if (IsFirstSession)
        {
            Online.Timer.Set(Timers.Luckyspin, 60);
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
                data.Info.avatar = JsonUtility.ToJson(data.avatar);
            }
        }

        public static int Angle
        {
            get { return data.avatar.angle; }
            set
            {
                data.avatar.angle = value;
                data.Info.avatar = JsonUtility.ToJson(data.avatar);
            }
        }

        public static string Json { get { return data.Info.avatar; } }
        public static ProfileData.AvatarData Current { get { return data.avatar; } }
    }
}
