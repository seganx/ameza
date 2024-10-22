#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class Proxy
    {
        private const string Version = "2.0.3";
        private const string logName = "[Plankton]";

        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        private static extern void Initialize();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeInitialize()
        {
            Debug.Log($"{logName} Initializing Plankton Version {Version}");
#if !UNITY_EDITOR
            Initialize();
#endif
            Debug.Log($"{logName} Initialized Plankton.");
        }
    }
}
#endif