#if UNITY_ANDROID
using UnityEngine;

namespace Plankton.Proxy
{
    public class ProxyGameServices : Proxy
    {
        private const string logName = "[Plankton] [PlayServices]";

        private readonly AndroidJavaObject objectPlayServices = null;

        public ProxyGameServices()
        {
            Debug.Log($"{logName} getting android object...");
            objectPlayServices = GetAndroidObject("getPlayServices");
            Debug.Log($"{logName} android object is {objectPlayServices}");
        }

        public void SignIn(bool forceInteractive = false, string serverClientId = "")
        {
            Debug.Log($"{logName} calling SignIn forceInteractive:{forceInteractive} serverClientId:{serverClientId}");
            objectPlayServices?.Call("signIn", serverClientId, forceInteractive);
        }
        
        public void SignOut()
        {
            Debug.Log($"{logName} calling SignOut");
            objectPlayServices?.Call("signOut");
        }

        public void Load(string filename)
        {
            Debug.Log($"{logName} calling Load filename:{filename}");
            objectPlayServices?.Call("load", filename);
        }

        public void Save(string filename, string data, string description)
        {
            Debug.Log($"{logName} calling Save filename:{filename} data:{data} description:{description}");
            objectPlayServices?.Call("save", filename, data, description);
        }
    }
}
#endif