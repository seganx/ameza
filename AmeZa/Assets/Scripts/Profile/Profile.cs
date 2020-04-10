using SeganX;
using System.Collections;
using System.Collections.Generic;
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
            int seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
            if (seconds < 0)
            {
                int addhearts = 1 - seconds / GlobalConfig.Timers.heart.duration;
                Hearts = Mathf.Clamp(Hearts + addhearts, 0, GlobalConfig.ProfilePreset.hearts);
                Online.Timer.Set(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
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

    public static bool IsFirstSession { get { return data.privateData.sessions == 1; } }

    public static string Username { get { return data.info.username; } }

    public static string Password { get { return data.info.password; } }

    public static int Gems { get { return data.privateData.gems; } }

    public static bool HasNickname { get { return data.info.nickname.HasContent(); } }

    public static bool HasStatus { get { return data.info.status.HasContent(); } }

    public static string Nickname
    {
        get { return HasNickname ? data.info.nickname : (data.info.username.HasContent() ? data.info.username : "player"); }
        set { data.info.nickname = value; }
    }

    public static string Status
    {
        get { return data.info.status; }
        set { data.info.status = value; }
    }

    public static int Version
    {
        get { return data.privateData.version; }
        set { data.privateData.version = value; }
    }

    public static int Hearts
    {
        get { return data.privateData.hearts; }
        set { data.privateData.hearts = value; }
    }

    public static int Bombs
    {
        get { return data.privateData.bombs; }
        set { data.privateData.bombs = value; }
    }

    public static int Hammers
    {
        get { return data.privateData.hammers; }
        set { data.privateData.hammers = value; }
    }

    public static int Missiles
    {
        get { return data.privateData.missles; }
        set { data.privateData.missles = value; }
    }

    public static int Skill
    {
        get { return data.privateData.skill; }
        set { data.privateData.skill = value; }
    }

    private static string LastHashdata
    {
        get { return PlayerPrefs.GetString("Profile.LastHashdata", string.Empty); }
        set { PlayerPrefs.SetString("Profile.LastHashdata", value); }
    }

    public static int Sessions
    {
        get { return data.privateData.sessions; }
        set { data.privateData.sessions = value; }
    }

    public static void EarnGems(int value)
    {
        if (value > 0)
            data.privateData.gems = data.privateData.gems.Value + value;
    }

    public static bool SpendGems(int value)
    {
        if (Gems >= value)
        {
            data.privateData.gems = data.privateData.gems.Value - value;
            return true;
        }
        return false;
    }

    public static bool HasBall(int id)
    {
        return data.privateData.balls.Contains(id);
    }

    public static void AddBall(int id)
    {
        if (data.privateData.balls.Contains(id) == false)
            data.privateData.balls.Add(id);
    }

    public static int GetSeasonRewarded(int seasonId)
    {
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return 0;
        return season.rewarded;
    }

    public static void SetSeasonRewarded(int seasonId, int rewarded)
    {
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return;
        season.rewarded = rewarded;
        SaveLocal();
    }

    public static bool CanOpenLevel(int seasonId, int index)
    {
        if (index < 0) return false;
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null)
            data.privateData.seasons.Add(season = new ProfileData.SeasonData() { id = seasonId });
        return index <= season.levels.Count;
    }

    public static int GetLevelsPassed()
    {
        if (data.privateData.seasons.Count < 1) return 0;
        data.privateData.seasons.Sort((x, y) => x.id - y.id);
        var season = data.privateData.seasons.LastOne();
        return GlobalFactory.Seasons.GetLevelNumber(season.id, season.levels.Count);
    }

    public static int GetSeasonProgress(int seasonId)
    {
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        return season == null ? 0 : season.levels.Count;
    }

    public static bool IsLevelPassed(int seasonId, int index)
    {
        if (index < 0) return false;
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null) return false;
        return index < season.levels.Count;
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

    public static bool IsFriendRewarded(int id, int level)
    {
        if (level < GlobalConfig.Friends.minRewardLevel) return false;
        var item = data.privateData.friends.Find(x => x.id == id);
        if (item == null)
        {
            item = new ProfileData.FriendData();
            item.id = id;
            data.privateData.friends.Add(item);
        }
        return item.rewarded > 0;
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
            data.info.avatar = JsonUtility.ToJson(data.avatar);
            StartSession();
            SaveLocal();
        })));
    }

    private static void StartSession()
    {
        data.privateData.sessions++;
        if (IsFirstSession)
        {
            EarnGems(GlobalConfig.ProfilePreset.gems);
            Hearts = GlobalConfig.ProfilePreset.hearts;
            //Bombs = Mathf.FloorToInt(GlobalConfig.ProfilePreset.bombs * 0.5f);
            //Hammers = Mathf.FloorToInt(GlobalConfig.ProfilePreset.hammers * 0.5f);
            //Missiles = Mathf.FloorToInt(GlobalConfig.ProfilePreset.missles * 0.5f);
            Bombs = GlobalConfig.ProfilePreset.bombs / 2;
            Hammers = GlobalConfig.ProfilePreset.hammers / 2;
            Missiles = GlobalConfig.ProfilePreset.missles / 2;

            GlobalAnalytics.Source(GlobalConfig.ProfilePreset.gems, "first");
        }
    }

    public static void Sync(bool sendProfile, System.Action<bool> nextTask)
    {
        SaveLocal();

        if (IsGlobalConfigUpdated == false)
        {
            var address = GlobalConfig.Instance.address + GlobalConfig.Instance.version + "/config.txt?" + System.DateTime.Now.Ticks;
            Http.DownloadText(address, null, null, json =>
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
        if ((System.DateTime.Now - lastGetPrfoileTime).TotalMinutes < 2)
        {
            SyncProfile(sendProfile, data.info, nextTask);
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
                data.info = serverProfile;
                RunFirstSission();
                SendProfileData(sendProfile, nextTask);
            }
            // we have data on server but we have lost data on local
            // so try to load from server to local
            else
            {
                data.info = serverProfile;
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
            data.info = serverProfile;

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
            Online.Timer.Set(GlobalConfig.Timers.luckySpin.id, 3600);
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
                data.info.avatar = JsonUtility.ToJson(data.avatar);
            }
        }

        public static int Angle
        {
            get { return data.avatar.angle; }
            set
            {
                data.avatar.angle = value;
                data.info.avatar = JsonUtility.ToJson(data.avatar);
            }
        }

        public static string Json { get { return data.info.avatar; } }
        public static ProfileData.AvatarData Current { get { return data.avatar; } }
    }
}
