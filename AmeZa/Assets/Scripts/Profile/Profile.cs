using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profile : MonoBehaviour
{
    private void Awake()
    {
        LoadLocal();
        data.privateData.sessions++;
        if (IsFirstSession)
        {
            EarnGems(GlobalConfig.ProfilePreset.gems);
            Hearts = GlobalConfig.ProfilePreset.heats;
            Bombs = GlobalConfig.ProfilePreset.bombs;
            Hammers = GlobalConfig.ProfilePreset.hammers;
            Missiles = GlobalConfig.ProfilePreset.missles;
        }
    }

    private IEnumerator Start()
    {
        var wait = new WaitForSeconds(1);

        int syncSeconds = 0;
        while (true)
        {
            yield return wait;

            // update hearts
            int seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.heart.id);
            if (seconds < 0)
            {
                int addhearts = 1 - seconds / GlobalConfig.Timers.heart.duration;
                Hearts = Mathf.Clamp(Hearts + addhearts, 0, GlobalConfig.ProfilePreset.heats);
                Online.Timer.SetTimer(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
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
    private static System.DateTime lastSetPrfoileTime = System.DateTime.MinValue;
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

    public static int Plusballs
    {
        get { return data.privateData.plusballs; }
        set { data.privateData.plusballs = value; }
    }

    private static string LastHashdata
    {
        get { return PlayerPrefs.GetString("Profile.LastHashdata", string.Empty); }
        set { PlayerPrefs.GetString("Profile.LastHashdata", value); }
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
    }

    public static bool CanOpenLevel(int seasonId, int index)
    {
        if (index < 0) return false;
        var season = data.privateData.seasons.Find(x => x.id == seasonId);
        if (season == null)
            data.privateData.seasons.Add(season = new ProfileData.SeasonData() { id = seasonId });
        return index <= season.levels.Count;
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
    }

    public static void SaveLocal()
    {
        PlayerPrefsEx.SetObject("Profile.Data", data);
    }

    public static void LoadLocal()
    {
        data = PlayerPrefsEx.GetObject("Profile.Data", data);
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
            if (success)
            {
                lastGetPrfoileTime = System.DateTime.Now;
                SyncProfile(sendProfile, srverdata, nextTask);
            }
            else nextTask(false);
        });
    }

    private static void SyncProfile(bool sendProfile, Online.Profile.Data serverData, System.Action<bool> nextTask)
    {
        if (Username.IsNullOrEmpty())
        {
            data.info = serverData;

            if (serverData.datahash.HasContent())
            {
                Online.Userdata.Get((sucess, privateStr, publicStr) =>
                {
                    if (sucess)
                    {
                        data.info = serverData;
                        data.privateData = JsonUtility.FromJson<ProfileData.PrivateData>(Utilities.DecompressString(privateStr, "{}"));
                        data.publicData = JsonUtility.FromJson<ProfileData.PublicData>(Utilities.DecompressString(publicStr, "{}"));
                        nextTask(true);
                    }
                    else nextTask(false);
                });
            }
            else
            {
                SendProfileData(sendProfile, nextTask);
            }
        }
        else
        {
            data.info = serverData;
            SendProfileData(sendProfile, nextTask);
        }
    }

    private static void SendProfileData(bool sendProfile, System.Action<bool> nextTask)
    {
        if (sendProfile == false || LastHashdata == data.privateData.Datahash || (System.DateTime.Now - lastSetPrfoileTime).TotalMinutes < 3)
        {
            nextTask(true);
            return;
        }

        lastSetPrfoileTime = System.DateTime.Now;
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
