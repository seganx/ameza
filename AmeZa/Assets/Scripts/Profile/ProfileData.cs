using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfileData
{
    [System.Serializable]
    public class AvatarData
    {
        public int angle = 0;
        public int ballId = 0;
    }

    [System.Serializable]
    public class SeasonData
    {
        public int id = 0;
        public int rewarded = 0;
        public List<int> levels = new List<int>();
    }

    [System.Serializable]
    public class FriendData
    {
        public int id = 0;
        public int rewarded = 0;
    }

    [System.Serializable]
    public class PrivateData
    {
        public CryptoInt version = 0;
        public CryptoInt sessions = 0;
        public CryptoInt gems = 0;
        public CryptoInt hearts = 0;
        public CryptoInt bombs = 0;
        public CryptoInt missles = 0;
        public CryptoInt hammers = 0;
        public CryptoInt skill = 0;
        public CryptoInt classicScore = 0;
        public List<int> balls = new List<int>() { 0 };
        public List<FriendData> friends = new List<FriendData>();
        public List<SeasonData> seasons = new List<SeasonData>();

        public string Datahash
        {
            get { return "H" + gems.Value + "|" + hearts.Value + "|" + bombs.Value + "|" +  missles.Value + "|" + hammers.Value + "|" + balls.Count + "|" + Http.userheader; }
        }
    }

    [System.Serializable]
    public class PublicData
    {
        public List<int> balls = new List<int>();
    }

    public Online.Profile.Data _info = new Online.Profile.Data();
    public AvatarData avatar = new AvatarData();
    public PrivateData privateData = new PrivateData();
    public PublicData publicData = new PublicData();
    public Online.Profile.Data info
    {
        get { return _info; }
        set
        {
            _info = value;
            if (_info.avatar.HasContent())
                avatar = JsonUtility.FromJson<AvatarData>(_info.avatar);
        }
    }
}
