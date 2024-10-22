#if UNITY_ANDROID
using UnityEngine;

namespace Plankton.Proxy
{
    public class ProxyGoogleUmp : Proxy
    {
        private const string logName = "[Plankton] [GoogleUmp]";

        private readonly AndroidJavaObject objectConsent = null;

        public ProxyGoogleUmp()
        {
            Debug.Log($"{logName} getting android objects...");
            objectConsent = GetAndroidObject("getConsentManager");
            Debug.Log($"{logName} Collected android objects consent:{objectConsent}");
        }

        public void RequestConsentInfoUpdate(bool setDebugGeography = false, string testDeviceHashedId = "", bool setTagForUnderAgeOfConsent = false)
        {
            Debug.Log($"{logName} calling RequestConsentInfoUpdate setDebugGeography:{setDebugGeography} testDeviceHashedId:{testDeviceHashedId} setTagForUnderAgeOfConsent:{setTagForUnderAgeOfConsent}");
            objectConsent?.Call("requestConsentInfoUpdate", setDebugGeography, testDeviceHashedId, setTagForUnderAgeOfConsent);
        }

        public void LoadAndShowConsentFormIfRequired()
        {
            Debug.Log($"{logName} calling LoadAndShowConsentFormIfRequired");
            objectConsent?.Call("loadAndShowConsentFormIfRequired");
        }

        public void ShowPrivacyOptionsForm()
        {
            Debug.Log($"{logName} calling ShowPrivacyOptionsForm");
            objectConsent?.Call("showPrivacyOptionsForm");
        }

        public void ResetConsentInformation()
        {
            Debug.Log($"{logName} calling ResetConsentInformation");
            objectConsent?.Call("resetConsentInformation");
        }

        public bool CanRequestAds()
        {
            Debug.Log($"{logName} calling CanRequestAds");
            return objectConsent != null && objectConsent.Call<bool>("canRequestAds");
        }

        public bool IsPrivacyOptionsRequired()
        {
            Debug.Log($"{logName} calling IsPrivacyOptionsRequired");
            return objectConsent != null && objectConsent.Call<bool>("isPrivacyOptionsRequired");
        }
    }
}
#endif