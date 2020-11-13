using System.Collections.Generic;
using UnityEngine;
using SeganX;

namespace Fun.Iab
{
    public enum PurchaseProvider : int
    {
        Null = 0,
        Market = 1,
        Gateway = 2
    }

    [System.Serializable]
    public class PurchasedData
    {
        [System.Serializable]
        public class PurchasedDetail
        {
            public string sku = string.Empty;
            public string token = string.Empty;
        }
        public List<PurchasedDetail> list = new List<PurchasedDetail>();
    }

    public class PurchaseSystem : MonoBehaviour
    {
        public delegate void Callback(bool success, string msg);

        private class CallbackCaller
        {
            public bool invoke = false;
            public Callback callbackFunc = null;
            public bool success = false;
            public string msg = string.Empty;

            public void Setup(Callback callback)
            {
                invoke = false;
                callbackFunc = callback;
                success = false;
                msg = string.Empty;
            }

            public void Call(bool result, string resmsg)
            {
                success = result;
                msg = resmsg;
                invoke = true;
            }
        }

        public void Update()
        {
            DispachCallback(purchaseCallback);
            DispachCallback(consumeCallback);
            DispachCallback(queryCallback);
        }

        private void DispachCallback(CallbackCaller callback)
        {
            if (callback.invoke)
            {
                callback.invoke = false;
                if (callback.callbackFunc != null)
                {
                    callback.callbackFunc(callback.success, callback.msg);
                    callback.Setup(null);
                }
            }
        }


        ////////////////////////////////////////////////////////
        /// STATIC MEMBER
        ////////////////////////////////////////////////////////
        private static int Version { get; set; } = 0;
        private static string StoreUrl { get; set; } = string.Empty;
        private static readonly CallbackCaller purchaseCallback = new CallbackCaller();
        private static readonly CallbackCaller consumeCallback = new CallbackCaller();
        private static readonly CallbackCaller queryCallback = new CallbackCaller();

        public static bool IsInitialized { get; private set; } = false;
        public static PurchaseProvider PurchaseProvider { get; private set; } = PurchaseProvider.Null;

        static PurchaseSystem()
        {
            DontDestroyOnLoad(Game.Instance.gameObject.AddComponent<PurchaseSystem>());
        }

        public static void Initialize(int version, string rsaKey, string storeUrl)
        {
            if (IsInitialized) return;
                IsInitialized = true;
            Version = version;
            StoreUrl = storeUrl;
            MarketProxy.Initialize(rsaKey);
        }

        public static void Purchase(PurchaseProvider provider, string sku, Callback callback)
        {
            PurchaseProvider = provider;
            purchaseCallback.Setup(callback);

#if UNITY_EDITOR
            provider = PurchaseProvider.Null;
#endif

            switch (provider)
            {
                case PurchaseProvider.Market:
                    if (MarketProxy.Supported)
                    {
                        Online.Purchase.Start(Online.Purchase.Provider.Market, () => MarketProxy.Purchase(sku));
                    }
                    else
                    {
                        Application.OpenURL(StoreUrl);
                        callback(false, "Bazaar Not Supported!");
                    }
                    break;
                case PurchaseProvider.Gateway:
                    callback(false, "faketoken");
                    break;

#if UNITY_EDITOR
                default:
                    callback(true, "faketoken");
                    break;
#endif
            }

        }

        public static void Consume(string sku, Callback callback)
        {
            consumeCallback.Setup(callback);

            switch (PurchaseProvider)
            {
                case PurchaseProvider.Market:
                    MarketProxy.Consume(sku);
                    break;

                case PurchaseProvider.Gateway:
                    break;
            }
        }

        public static void QueryPurchases(PurchaseProvider provider, Callback callback)
        {
            PurchaseProvider = provider;
            queryCallback.Setup(callback);

            switch (PurchaseProvider)
            {
                case PurchaseProvider.Market:
                    MarketProxy.QueryPurchases();
                    break;

                case PurchaseProvider.Gateway:
                    break;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////
        //  implementations
        ////////////////////////////////////////////////////////////////////////////////////////////
        public static class Payload
        {
            private static List<string> list = new List<string>();

            static Payload()
            {
                list = PlayerPrefsEx.GetObject("PurchaseSystem.Payload.list", new List<string>());
            }

            public static string Get(string salt)
            {
                var res = System.DateTime.Now.Ticks.ToString().ComputeMD5(salt);
                list.Add(res);
                PlayerPrefsEx.SetObject("PurchaseSystem.Payload.list", list);
                return res;
            }

            public static bool IsValid(string payload)
            {
                return list.IndexOf(payload) >= 0;
            }

            public static bool Remove(string payload)
            {
                var res = IsValid(payload);
                list.Remove(payload);
                PlayerPrefsEx.SetObject("PurchaseSystem.Payload.list", list);
                return res;
            }
        }

        private static class MarketProxy
        {
            public static bool Supported { get; private set; }

            public static void Initialize(string key)
            {
                Supported = true;

                IabPlugin.EventManager.BillingSupportedEvent += () => Supported = true;
                IabPlugin.EventManager.BillingNotSupportedEvent += (error) => Supported = false;

                IabPlugin.EventManager.PurchaseSucceededEvent += (res) =>
                {
                    Debug.Log("Verifying purchase: " + res);
                    if (Payload.IsValid(res.developerPayload))
                        Online.Purchase.End(Online.Purchase.Provider.Market, Version, res.productId, res.purchaseToken, (success, payload) => purchaseCallback.Call(success && payload == res.developerPayload, res.purchaseToken));
                    else
                        purchaseCallback.Call(false, res.purchaseToken);
                };

                IabPlugin.EventManager.PurchaseFailedEvent += (error) =>
                {
                    Debug.LogWarning("Purachse Failed: " + error);
                    purchaseCallback.Call(false, error);
                };

                IabPlugin.EventManager.ConsumePurchaseSucceededEvent += (res) =>
                {
                    Debug.Log("Consume succeed: " + res);
                    consumeCallback.Call(Payload.Remove(res.developerPayload), res.purchaseToken);
                };

                IabPlugin.EventManager.ConsumePurchaseFailedEvent += (error) =>
                {
                    Debug.LogWarning("Consume failed: " + error);
                    consumeCallback.Call(false, error);
                };

                IabPlugin.EventManager.QueryPurchasesSucceededEvent += (purchases) =>
                {
                    Debug.Log("Query purachses succeed: " + purchases.Count);
                    var res = new PurchasedData();
                    foreach (var item in purchases)
                    {
                        if (item.purchaseState == IabPlugin.PurchaseInfo.State.Purchased)
                        {
                            res.list.Add(new PurchasedData.PurchasedDetail() { sku = item.productId, token = item.purchaseToken });
                        }
                    }
                    queryCallback.Call(true, JsonUtility.ToJson(res));
                };

                IabPlugin.EventManager.QueryPurchasesFailedEvent += (error) =>
                {
                    Debug.LogWarning("Query purachses Failed: " + error);
                    queryCallback.Call(false, error);
                };

                IabPlugin.PluginIab.Initialize(key);
            }

            public static void Purchase(string sku)
            {
                Debug.Log("Purchase started for " + sku);
                IabPlugin.PluginIab.PurchaseProduct(sku, Payload.Get(Core.Salt));
            }

            public static void Consume(string sku)
            {
                Debug.Log("Consume started for " + sku);
                IabPlugin.PluginIab.ConsumeProduct(sku);
            }

            public static void QueryPurchases()
            {
                Debug.Log("Query purchases started!");
                IabPlugin.PluginIab.QueryPurchases();
            }
        }
    }
}
