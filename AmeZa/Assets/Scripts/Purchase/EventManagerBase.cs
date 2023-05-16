using UnityEngine;

namespace IabPlugin
{
    public abstract class EventManagerBase : MonoBehaviour
    {
        private static GameObject iapGameObject = null;

#if BAZAAR
        private const string objectName = "BazaarPlugin.IABEventManager";
#elif MYKET
        private const string objectName = "BazaarPlugin.IABEventManager";//"MyketPlugin.IABEventManager";
#else
        private const string objectName = "NO IAB PLUGIN";
#endif

        public static void Initialize(System.Type type)
        {
            if (iapGameObject != null) return;

            iapGameObject = GameObject.Find(objectName);
            if (iapGameObject != null) return;

            iapGameObject = new GameObject(objectName, type);
            DontDestroyOnLoad(iapGameObject);
        }
    }
}