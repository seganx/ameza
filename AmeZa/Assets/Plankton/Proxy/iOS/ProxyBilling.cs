#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;

namespace Plankton.Proxy
{
    public class ProxyBilling
    {
        private const string logName = "[Plankton] [Billing]";

        public ProxyBilling()
        {
            Debug.Log($"{logName} Initializing market:AppStore");
#if !UNITY_EDITOR
            IosProxy.InitializeBilling();
#endif
        }

        public void Purchase(string sku)
        {
            Debug.Log($"{logName} Purchase {sku}");
#if !UNITY_EDITOR
            IosProxy.Purchase(sku);
#endif
        }

        public void FinishTransaction(string transactionId)
        {
            Debug.Log($"{logName} FinishTransaction {transactionId}");
#if !UNITY_EDITOR
            IosProxy.FinishTransaction(transactionId);
#endif
        }

        public void RestorePurchases()
        {
            Debug.Log($"{logName} RestorePurchases");
#if !UNITY_EDITOR
            IosProxy.RestorePurchases();
#endif
        }

        public void GetSkuDetails(string camaSeparatedSkus)
        {
            Debug.Log($"{logName} GetSkuDetails {camaSeparatedSkus}");
#if !UNITY_EDITOR
            IosProxy.GetSkuDetails(camaSeparatedSkus);
#endif
        }

        public void GetPurchases()
        {
            Debug.Log($"{logName} GetPurchases");
#if !UNITY_EDITOR
            IosProxy.GetPurchases();
#endif
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static class IosProxy
        {
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void InitializeBilling();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void GetSkuDetails([MarshalAs(UnmanagedType.LPStr)] string skus);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void Purchase([MarshalAs(UnmanagedType.LPStr)] string sku);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void GetPurchases();
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void FinishTransaction([MarshalAs(UnmanagedType.LPStr)] string token);
            [DllImport("__Internal", CharSet = CharSet.Ansi)] public static extern void RestorePurchases();
        }
    }
}
#endif