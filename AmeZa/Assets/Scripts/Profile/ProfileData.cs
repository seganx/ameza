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
    public class PrivateData
    {
        public CryptoInt sessions = 0;
        public CryptoInt gems = 0;
        public CryptoInt seasonsId = 0;
        public CryptoInt levelId = 0;
        public List<int> balls = new List<int>() { 0 };

        public string Datahash
        {
            get { return "H" + gems.Value + "|" + seasonsId + "|" + levelId.Value + "|" + balls.Count; }
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
