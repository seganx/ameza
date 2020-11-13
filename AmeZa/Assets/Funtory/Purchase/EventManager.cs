using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID
namespace IabPlugin
{
    public class EventManager : EventManagerBase
    {
        [Serializable]
        public class QueryInventoryReault
        {
            public List<PurchaseInfo> purchases = new List<PurchaseInfo>();
            public List<SkuInfo> skus = new List<SkuInfo>();
        }

        [Serializable]
        public class QuerySkuReault
        {
            public List<SkuInfo> skus = new List<SkuInfo>();
        }

        [Serializable]
        public class QueryPurchasesReault
        {
            public List<PurchaseInfo> purchases = new List<PurchaseInfo>();
        }

        // Fired after init is called when billing is supported on the device
        public static event Action BillingSupportedEvent = null;

        // Fired after init is called when billing is not supported on the device
        public static event Action<string> BillingNotSupportedEvent = null;

        // Fired when the inventory and purchase history query has returned
        public static event Action<List<PurchaseInfo>, List<SkuInfo>> QueryInventorySucceededEvent = null;

        // Fired when the inventory and purchase history query fails
        public static event Action<string> QueryInventoryFailedEvent = null;

        // Fired when the SkuDetails query has returned
        public static event Action<List<SkuInfo>> QuerySkuDetailsSucceededEvent = null;

        // Fired when the SkuDetails query fails
        public static event Action<string> QuerySkuDetailsFailedEvent = null;

        // Fired when the purchase history query has returned
        public static event Action<List<PurchaseInfo>> QueryPurchasesSucceededEvent = null;

        // Fired when the purchase history query fails
        public static event Action<string> QueryPurchasesFailedEvent = null;

        // Fired when a purchase succeeds
        public static event Action<PurchaseInfo> PurchaseSucceededEvent = null;

        // Fired when a purchase fails
        public static event Action<string> PurchaseFailedEvent = null;

        // Fired when a call to consume a product succeeds
        public static event Action<PurchaseInfo> ConsumePurchaseSucceededEvent = null;

        // Fired when a call to consume a product fails
        public static event Action<string> ConsumePurchaseFailedEvent = null;


        static EventManager()
        {
            Initialize(typeof(EventManager));
        }

        public void billingSupported(string empty)
        {
            BillingSupportedEvent.SafeInvoke();
        }

        public void billingNotSupported(string error)
        {
            BillingNotSupportedEvent.SafeInvoke(error);
        }

        public void queryInventorySucceeded(string jsonStr)
        {
            Debug.Log("queryInventorySucceeded=" + jsonStr);
            var resutl = JsonUtility.FromJson<QueryInventoryReault>(jsonStr);
            QueryInventorySucceededEvent.SafeInvoke(resutl.purchases, resutl.skus);
        }

        public void queryInventoryFailed(string error)
        {
            QueryInventoryFailedEvent.SafeInvoke(error);
        }

        public void querySkuDetailsSucceeded(string jsonStr)
        {
            Debug.Log("querySkuDetailsSucceeded=" + jsonStr);
            var result = JsonUtility.FromJson<QuerySkuReault>("{\"skus\":" + jsonStr + "}");
            QuerySkuDetailsSucceededEvent.SafeInvoke(result.skus);
        }

        public void querySkuDetailsFailed(string error)
        {
            QuerySkuDetailsFailedEvent.SafeInvoke(error);
        }

        public void queryPurchasesSucceeded(string jsonStr)
        {
            Debug.Log("queryPurchasesSucceeded=" + jsonStr);
            var result = JsonUtility.FromJson<QueryPurchasesReault>("{\"purchases\":" + jsonStr + "}");
            QueryPurchasesSucceededEvent.SafeInvoke(result.purchases);
        }

        public void queryPurchasesFailed(string error)
        {
            QueryPurchasesFailedEvent.SafeInvoke(error);
        }

        public void purchaseSucceeded(string jsonStr)
        {
            Debug.Log("purchaseSucceeded=" + jsonStr);
            PurchaseSucceededEvent.SafeInvoke(JsonUtility.FromJson<PurchaseInfo>(jsonStr));
        }

        public void purchaseFailed(string error)
        {
            PurchaseFailedEvent.SafeInvoke(error);
        }

        public void consumePurchaseSucceeded(string jsonStr)
        {
            Debug.Log("consumePurchaseSucceeded=" + jsonStr);
            ConsumePurchaseSucceededEvent.SafeInvoke(JsonUtility.FromJson<PurchaseInfo>(jsonStr));
        }

        public void consumePurchaseFailed(string error)
        {
            ConsumePurchaseFailedEvent.SafeInvoke(error);
        }
    }
}
#endif
