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
            Missles= GlobalConfig.ProfilePreset.missles;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveLocal();
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

    public static int Missles
    {
        get { return data.privateData.missles; }
        set { data.privateData.missles = value; }
    }

    public static ProfileData.AvatarData Avatar
    {
        get { return data.avatar; }
        set { data.avatar = value; }
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

    public static void SaveLocal()
    {
        PlayerPrefsEx.Serialize("Profile.Data", data);
    }

    public static void LoadLocal()
    {
        data = PlayerPrefsEx.Deserialize("Profile.Data", new ProfileData());
    }



    public static void SyncWidthServer(bool sendProfile, System.Action<bool> nextTask)
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

        Online.Login(GlobalConfig.Instance.gameId, Core.DeviceId, success =>
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
        if ((System.DateTime.Now - lastGetPrfoileTime).TotalMinutes < 5)
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
            if (serverData.datahash.HasContent())
            {
#if OFF
                Popup_Loading.Hide();
                Game.Instance.OpenPopup<Popup_AccountSelection>().Setup(newprofile, yes =>
                {
                    if (yes)
                    {
                        Profile.Data = newprofile;
                        PlayerPrefs.DeleteAll();
                        PlayerPrefsEx.ClearData();
                        SaveToLocal();
                        SocialLogic.ForceDownloadFromServer = true;
                        Game.Instance.OpenPopup<Popup_Confirm>().Setup(111068, false, ok => Application.Quit());
                    }
                    else
                    {
                        var selfdata = Profile.Data.data;
                        Profile.Data = newprofile;
                        Profile.Data.data = selfdata;
                        SendProfileData(sendProfile, nextTask);
                    }
                });
#endif
                Online.Userdata.Get((sucess, privateStr, publicStr) =>
                {
                    if (sucess)
                    {
                        data.info = serverData;
                        data.privateData = JsonUtility.FromJson<ProfileData.PrivateData>(Utilities.DecompressString(privateStr, "{}"));
                        data.publicData = JsonUtility.FromJson<ProfileData.PublicData>(Utilities.DecompressString(publicStr, "{}"));
                    }
                    else nextTask(false);
                });
            }
            else
            {
                data.info = serverData;
                SendProfileData(sendProfile, nextTask);
            }
        }
        else if (Username == serverData.username)
        {
            data.info = serverData;
            SendProfileData(sendProfile, nextTask);
        }

#if OFF
        else if (Profile.UserId != newprofile.userId)
        {
            Popup_Loading.Hide();
            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111067, false, yes =>
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefsEx.ClearData();
                Profile.Data = new ProfileData();
                Application.Quit();

                //  I don't know if it works. but let it be for sure :)
                PlayerPrefs.DeleteAll();
                PlayerPrefsEx.ClearData();
            });
        }
#endif
    }

    private static void SendProfileData(bool sendProfile, System.Action<bool> nextTask)
    {
        if (sendProfile == false || LastHashdata == data.privateData.Datahash)
        {
            nextTask(true);
            return;
        }

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
}
