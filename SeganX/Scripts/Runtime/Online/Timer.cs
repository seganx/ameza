using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static partial class Online
    {
        public class Timer : MonoBehaviour
        {
            [Serializable]
            private class TimeData
            {
                public int id = 0;
                public long startTime = 0;
                public float duration = 0;
            }

            private void Awake()
            {
                instance = this;
                Load();
            }

            private void OnApplicationQuit()
            {
                Save();
            }

            private void OnApplicationPause(bool pause)
            {
                Save();
            }

#if SX_ONLINE
            private void Start()
            {
                if (Core.GameId < 1) return;

                DownloadData<long>("time.php", null, (success, res) =>
                {
                    if (success && res > 0)
                    {
                        Synced = true;
                        var now = LocalTimeSeconds;
                        deltaTime = CurrentSeconds - now;
                        Invoke("Start", 120);
                    }
                    else Invoke("Start", 5);
                });
            }
#endif

            ////////////////////////////////////////////////////////
            /// STATIC MEMBER
            ////////////////////////////////////////////////////////
            private static Timer instance = null;
            private static List<TimeData> data = new List<TimeData>();
            private static long deltaTime = 0;

            public static bool Synced { get; private set; }
            private static long LocalTimeSeconds => DateTimeOffset.Now.ToUnixTimeSeconds() + 12600; // +3:30 Tehran time
            public static long CurrentSeconds => LocalTimeSeconds + deltaTime;

            public static void Set(int timerId, float duration, long startTime = 0)
            {
                var timer = data.Find(x => x.id == timerId);
                if (timer == null)
                {
                    timer = new TimeData();
                    timer.id = timerId;
                    data.Add(timer);
                }

                timer.duration = duration;
                timer.startTime = startTime == 0 ? CurrentSeconds : startTime;

                Save();
            }

            public static int GetRemainSeconds(int timerId, int duration)
            {
                var timer = data.Find(x => x.id == timerId);
                if (timer == null)
                {
                    Set(timerId, duration);
                    timer = data.Find(x => x.id == timerId);
                }

                return Mathf.FloorToInt(timer.startTime - CurrentSeconds + timer.duration);
            }

            public static bool Exist(int timerId)
            {
                return data.Exists(x => x.id == timerId);
            }

            public static void Remove(int timerId)
            {
                data.RemoveAll(x => x.id == timerId);
            }

            public static DateTime UnixTimeToLocalTime(long date)
            {
                DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                res = res.AddSeconds(date);
                return res.ToLocalTime();
            }

            private static void Save()
            {
                PlayerPrefsEx.SetObject("SeganX.Online.Timer", data);
            }

            private static void Load()
            {
                data = PlayerPrefsEx.GetObject("SeganX.Online.Timer", data);
            }

            internal static void Init()
            {
                if (instance == null)
                    DontDestroyOnLoad(instance = GameManager.Game.gameObject.AddComponent<Timer>());
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeTimer()
        {
            Timer.Init();
        }
    }
}
