#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class ProxyPushNotification
    {
        private const string logName = "[Plankton] [PushNotification]";

        public void GetToken()
        {
            Debug.Log($"{logName} calling GetFCMToken");
#if !UNITY_EDITOR
            IosProxy.GetFCMToken();
#endif
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void GetFCMToken();
        }
    }
}
#endif