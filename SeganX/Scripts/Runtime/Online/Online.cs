﻿#if SX_ONLINE
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static partial class Online
    {
        [System.Serializable]
        private class Response<T>
        {
            public string msg = string.Empty;
            public T data = default(T);
        }

        [System.Serializable]
        private class LoginData
        {
            public int game_id = 0;
            public string device_id = string.Empty;
        }

        public static string error { get; private set; }

        private static Dictionary<string, string> header = new Dictionary<string, string>();

        private static void DownloadData<T>(string uri, object post, System.Action<bool, T> callback, int timeout = 0, int efforts = 1)
        {
            Http.DownloadText($"{Core.OnlineDomain}/{uri}", post == null ? null : JsonUtility.ToJson(post), header, (status, resjson) =>
            {
                if (resjson != null)
                {
                    Response<T> res = null;
                    try
                    {
                        res = JsonUtility.FromJson<Response<T>>(resjson);
                    }
                    catch (System.Exception e)
                    {
                        error = e.Message;
                        Debug.LogError(error);
                    }

                    if (res != null)
                    {
                        if (res.msg != Message.ok)
                        {
                            error = res.msg;
                            callback(false, res.data);
                        }
                        else callback(true, res.data);
                    }
                    else callback(false, default(T));
                }
                else
                {
                    error = Message.TranslateHttp(status);
                    callback(false, default);
                }
            },
            null, timeout, efforts);
        }


        public static void GetTime(System.Action<bool, long> callback)
        {
            DownloadData<long>("time.php", null, callback);
        }

        public static void Login(int gameId, string deviceId, System.Action<bool> callback)
        {
            var post = new LoginData();
            post.game_id = gameId;
            post.device_id = deviceId;
            DownloadData<string>("login.php", post, (success, res) =>
            {
                if (success)
                    header["token"] = res;
                callback(success);
            });
        }

    }
}
#endif