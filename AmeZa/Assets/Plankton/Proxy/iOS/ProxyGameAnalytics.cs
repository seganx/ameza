#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class ProxyGameAnalytics
    {
        private const string logName = "[Plankton] [GameAnalaytics]";

        public ProxyGameAnalytics(string json)
        {
            Debug.Log($"{logName} Initializing json:{json}");
#if !UNITY_EDITOR
            IosProxy.InitializeGameAnalytics(json);
#endif
            Debug.Log($"{logName} Initialized.");
        }

        public void SetCustomDimension01(string value)
        {
#if !UNITY_EDITOR
            IosProxy.SetCustomDimension01(value);
#endif
        }

        public void SetCustomDimension02(string value)
        {
#if !UNITY_EDITOR
            IosProxy.SetCustomDimension02(value);
#endif
        }

        public void SetCustomDimension03(string value)
        {
#if !UNITY_EDITOR
            IosProxy.SetCustomDimension03(value);
#endif
        }

        public void AddBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddBusinessEvent(currency, amount, itemType, itemId, cartType, "", fields, mergeFields);
#endif
        }

        public void AddBusinessEventWithReceipt(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string store, string signature, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddBusinessEvent(currency, amount, itemType, itemId, cartType, receipt, fields, mergeFields);
#endif
        }

        public void AddResourceEvent(int flowType, string currency, float amount, string itemType, string itemId, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddResourceEvent(flowType, currency, (int)amount, itemType, itemId, fields, mergeFields);
#endif
        }

        public void AddProgressionEvent(int progressionStatus, string progression01, string progression02, string progression03, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddProgressionEvent(progressionStatus, progression01, progression02, progression03, fields, mergeFields);
#endif
        }

        public void AddProgressionEventWithScore(int progressionStatus, string progression01, string progression02, string progression03, double score, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddProgressionEventWithScore(progressionStatus, progression01, progression02, progression03, (int)score, fields, mergeFields);
#endif
        }

        public void AddDesignEvent(string eventId, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddDesignEvent(eventId, fields, mergeFields);
#endif
        }

        public void AddDesignEventWithValue(string eventId, float value, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddDesignEventWithValue(eventId, value, fields, mergeFields);
#endif
        }

        public void AddErrorEvent(int severity, string message, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddErrorEvent(severity, message, fields, mergeFields);
#endif
        }

        public void AddAdEvent(int adAction, int adType, string adSdkName, string adPlacement, string fields, bool mergeFields)
        {
#if !UNITY_EDITOR
            IosProxy.AddAdEvent(adAction, adType, adSdkName, adPlacement, fields, mergeFields);
#endif
        }

        public bool IsRemoteConfigsReady()
        {
#if !UNITY_EDITOR
            return IosProxy.IsGARemoteConfigsReady();
#else
            return false;
#endif
        }

        public string GetRemoteConfigsValueAsString(string key, string defaultValue)
        {
            var buffer = new char[65536];
#if !UNITY_EDITOR
            IosProxy.GetGARemoteConfigValue(buffer, key, defaultValue);
#endif
            var stringBuilder = new System.Text.StringBuilder();
            for (var i = 0; i < buffer.Length && buffer[i] != '\0'; i++)
                stringBuilder.Append(buffer[i]);
            return stringBuilder.ToString();
        }

        public string GetABTestingId()
        {
            var buffer = new char[4096];
#if !UNITY_EDITOR
            IosProxy.GetABTestingIdForGA(buffer);
#endif
            var stringBuilder = new System.Text.StringBuilder();
            for (var i = 0; i < buffer.Length && buffer[i] != '\0'; i++)
                stringBuilder.Append(buffer[i]);
            return stringBuilder.ToString();
        }

        public string GetABTestingVariantId()
        {
            var buffer = new char[4096];
#if !UNITY_EDITOR
            IosProxy.GetABTestingVariantIdForGA(buffer);
#endif
            var stringBuilder = new System.Text.StringBuilder();
            for (var i = 0; i < buffer.Length && buffer[i] != '\0'; i++)
                stringBuilder.Append(buffer[i]);
            return stringBuilder.ToString();
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void InitializeGameAnalytics([MarshalAs(UnmanagedType.LPStr)] string json);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void SetCustomDimension01([MarshalAs(UnmanagedType.LPStr)] string customDimension);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void SetCustomDimension02([MarshalAs(UnmanagedType.LPStr)] string customDimension);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void SetCustomDimension03([MarshalAs(UnmanagedType.LPStr)] string customDimension);

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddBusinessEvent(
                [MarshalAs(UnmanagedType.LPStr)] string currency,
                int amount,
                [MarshalAs(UnmanagedType.LPStr)] string itemType,
                [MarshalAs(UnmanagedType.LPStr)] string itemId,
                [MarshalAs(UnmanagedType.LPStr)] string cartType,
                [MarshalAs(UnmanagedType.LPStr)] string receipt,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddResourceEvent(
                int flowType,
                [MarshalAs(UnmanagedType.LPStr)] string currency,
                int amount,
                [MarshalAs(UnmanagedType.LPStr)] string itemType,
                [MarshalAs(UnmanagedType.LPStr)] string itemId,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddProgressionEvent(
                int progressionStatus,
                [MarshalAs(UnmanagedType.LPStr)] string progression01,
                [MarshalAs(UnmanagedType.LPStr)] string progression02,
                [MarshalAs(UnmanagedType.LPStr)] string progression03,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddProgressionEventWithScore(
                int progressionStatus,
                [MarshalAs(UnmanagedType.LPStr)] string progression01,
                [MarshalAs(UnmanagedType.LPStr)] string progression02,
                [MarshalAs(UnmanagedType.LPStr)] string progression03,
                int score,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddDesignEvent(
                [MarshalAs(UnmanagedType.LPStr)] string eventId,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddDesignEventWithValue(
                [MarshalAs(UnmanagedType.LPStr)] string eventId,
                double value,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddErrorEvent(
                int severity,
                [MarshalAs(UnmanagedType.LPStr)] string message,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void AddAdEvent(
                int adAction,
                int adType,
                [MarshalAs(UnmanagedType.LPStr)] string adSdkName,
                [MarshalAs(UnmanagedType.LPStr)] string adPlacement,
                [MarshalAs(UnmanagedType.LPStr)] string fields,
                bool mergeFields
            );

            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern bool IsGARemoteConfigsReady();

            [DllImport("__Internal", CharSet = CharSet.Ansi)]
            public static extern void GetGARemoteConfigValue(
                [Out][MarshalAs(UnmanagedType.LPArray)] char[] destinationBuffer,
                [MarshalAs(UnmanagedType.LPStr)] string key,
                [MarshalAs(UnmanagedType.LPStr)] string defaultValue);

            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void GetABTestingIdForGA([Out][MarshalAs(UnmanagedType.LPArray)] char[] destinationBuffer);

            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void GetABTestingVariantIdForGA([Out][MarshalAs(UnmanagedType.LPArray)] char[] destinationBuffer);
        }
    }
}
#endif