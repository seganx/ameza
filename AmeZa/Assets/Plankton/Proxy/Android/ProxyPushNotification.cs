#if UNITY_ANDROID
using UnityEngine;

namespace Plankton.Proxy
{
    public class ProxyPushNotification : Proxy
    {
        private const string logName = "[Plankton] [PushNotification]";

        private readonly AndroidJavaObject objectPush = null;

        public ProxyPushNotification()
        {
            Debug.Log($"{logName} getting android object...");
            objectPush = GetAndroidObject("getFirebaseMessaging");
            Debug.Log($"{logName} android object is {objectPush}");
        }

        public void GetToken()
        {
            Debug.Log($"{logName} calling GetFirebaseToken");
            objectPush?.Call("getToken");
        }
    }
}
#endif