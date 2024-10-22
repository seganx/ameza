#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class ProxyGameServices
    {
        private const string logName = "[Plankton] [GameCenter]";

        public void SignIn(bool androidForceInteractive = false, string androidServerClientId = "")
        {
            Debug.Log($"{logName} SignIn");
#if !UNITY_EDITOR
            IosProxy.SignIn();
#endif
        }

        public void SignOut()
        {
            // Apple's Game Center doesn't provide a way to sign-out programmatically
        }

        public void Load(string fileName)
        {
            Debug.Log($"{logName} Load fileName:{fileName}");
#if !UNITY_EDITOR
            IosProxy.Load(fileName);
#endif
        }

        public void Save(string fileName, string data, string description)
        {
            Debug.Log($"{logName} Save fileName:{fileName} data:{data}");
#if !UNITY_EDITOR
            IosProxy.Save(fileName, data);
#endif
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void SignIn();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void Load([MarshalAs(UnmanagedType.LPStr)] string fileName);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void Save([MarshalAs(UnmanagedType.LPStr)] string fileName, [MarshalAs(UnmanagedType.LPStr)] string data);
        }
    }
}

#endif