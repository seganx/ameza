#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class ProxyGoogleUmp
    {
        private const string logName = "[Plankton] [Utils]";

        public void RequestConsentInfoUpdate(bool setDebugGeography = false, string testDeviceHashedId = "", bool setTagForUnderAgeOfConsent = false)
        {
            Debug.Log($"{logName} calling RequestConsentInfoUpdate setDebugGeography:{setDebugGeography} testDeviceHashedId:{testDeviceHashedId} setTagForUnderAgeOfConsent:{setTagForUnderAgeOfConsent}");
#if !UNITY_EDITOR
            IosProxy.RequestConsentInfoUpdate(setDebugGeography, testDeviceHashedId, setTagForUnderAgeOfConsent);
#endif
        }

        public void LoadAndShowConsentFormIfRequired()
        {
            Debug.Log($"{logName} calling LoadAndShowConsentFormIfRequired");
#if !UNITY_EDITOR
            IosProxy.LoadAndShowConsentFormIfRequired();
#endif
        }

        public void ShowPrivacyOptionsForm()
        {
            Debug.Log($"{logName} calling ShowPrivacyOptionsForm");
#if !UNITY_EDITOR
            IosProxy.ShowPrivacyOptionsForm();
#endif
        }

        public void ResetConsentInformation()
        {
            Debug.Log($"{logName} calling ResetConsentInformation");
#if !UNITY_EDITOR
            IosProxy.ResetConsentInformation();
#endif
        }

        public bool CanRequestAds()
        {
            Debug.Log($"{logName} calling CanRequestAds");
#if !UNITY_EDITOR
            return IosProxy.CanRequestAds();
#else
            return true;    
#endif
        }

        public bool IsPrivacyOptionsRequired()
        {
            Debug.Log($"{logName} calling IsPrivacyOptionsRequired");
#if !UNITY_EDITOR
            return IosProxy.IsPrivacyOptionsRequired();
#else
            return true;
#endif
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern bool CanRequestAds();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern bool IsPrivacyOptionsRequired();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void ResetConsentInformation();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void RequestConsentInfoUpdate(bool setDebugGeography, [MarshalAs(UnmanagedType.LPStr)] string testDeviceHashedId, bool setTagForUnderAgeOfConsent);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void LoadAndShowConsentFormIfRequired();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void ShowPrivacyOptionsForm();
        }
    }
}

#endif