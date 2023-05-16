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

    [System.Serializable]
    public class SkuDetailsData
    {
        [System.Serializable]
        public class SkuDetail
        {
            public string sku = string.Empty;
            public string price = string.Empty;
        }
        public List<SkuDetail> list = new List<SkuDetail>();
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
            DispachCallback(skuDetailsCallback);
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
        private static string StoreUrl { get; set; } = string.Empty;
        private static readonly CallbackCaller purchaseCallback = new CallbackCaller();
        private static readonly CallbackCaller consumeCallback = new CallbackCaller();
        private static readonly CallbackCaller queryCallback = new CallbackCaller();
        private static readonly CallbackCaller skuDetailsCallback = new CallbackCaller();

        public static bool IsInitialized { get; private set; } = false;
        public static PurchaseProvider PurchaseProvider { get; private set; } = PurchaseProvider.Null;

        static PurchaseSystem()
        {
            DontDestroyOnLoad(Game.Instance.gameObject.AddComponent<PurchaseSystem>());
        }

        public static void Initialize(string rsaKey, string storeUrl)
        {
            if (IsInitialized) return;
            IsInitialized = true;
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
                        MarketProxy.Purchase(sku);
                    }
                    else
                    {
                        Application.OpenURL(StoreUrl);
                        callback(false, "Market Not Supported!");
                    }
                    break;
                case PurchaseProvider.Gateway:
                    callback(false, "faketoken");
                    break;

#if UNITY_EDITOR
                default:
                    MarketProxy.Purchase(sku);
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
                    if (MarketProxy.Supported)
                    {
                        MarketProxy.QueryPurchases();
                    }
                    else
                    {
#if UNITY_EDITOR
                        callback(true, "{}");
#else
                        callback(false, "{}");
#endif
                    }
                    break;

                case PurchaseProvider.Gateway:
                    callback?.Invoke(true, "{}");
                    break;
            }
        }

        public static void QuerySkuDetails(PurchaseProvider provider, string[] skus, Callback callback)
        {
            PurchaseProvider = provider;
            skuDetailsCallback.Setup(callback);

            switch (PurchaseProvider)
            {
                case PurchaseProvider.Market:
                    if (MarketProxy.Supported)
                    {
                        MarketProxy.GetSkuDetails(skus);
                    }
                    else
                    {
#if UNITY_EDITOR
                        callback(true, "{}");
#else
                        callback(false, "{}");
#endif
                    }
                    break;

                case PurchaseProvider.Gateway:
                    callback?.Invoke(true, "{}");
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        //  implementations
        ////////////////////////////////////////////////////////////////////////////////////////////
        public static class Payload
        {
            private static readonly List<string> list = new List<string>();

            static Payload()
            {
                list = PlayerPrefsEx.GetObject("PurchaseSystem.Payload.list", new List<string>());
            }

            public static string Get(string sku, int count)
            {
                var res = $"{sku}|{Profile.Username}|{count}|{Application.version}";
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
            public static bool Supported { get; private set; } = true;

            private static int PurchasedCount
            {
                get => PlayerPrefs.GetInt("MarketProxy.PurchasedCount", 1);
                set => PlayerPrefs.SetInt("MarketProxy.PurchasedCount", value);
            }

            public static void Initialize(string key)
            {
#if UNITY_EDITOR
                Supported = false;
#endif

#if UNITY_ANDROID
                IabPlugin.EventManager.BillingSupportedEvent += () => Supported = true;

                IabPlugin.EventManager.BillingNotSupportedEvent += (error) =>
                {
                    Supported = false;
                    Debug.LogWarning($"[IAB] [MarketProxy] Billing not supported: {error}");
                };

                IabPlugin.EventManager.PurchaseSucceededEvent += (res) =>
                {
                    Debug.Log("Verifying purchase: " + res);
                    purchaseCallback.Call(Payload.IsValid(res.developerPayload), res.purchaseToken);
                };

                IabPlugin.EventManager.PurchaseFailedEvent += (error) =>
                {
                    Debug.LogWarning("Purachse Failed: " + error);
                    purchaseCallback.Call(false, error);
                };

                IabPlugin.EventManager.ConsumePurchaseSucceededEvent += (res) =>
                {
                    PurchasedCount++;
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

                IabPlugin.EventManager.QuerySkuDetailsSucceededEvent += (details) =>
                {
                    Debug.Log("Query sku details succeed: " + details.Count);
                    var res = new SkuDetailsData();
                    foreach (var item in details)
                        res.list.Add(new SkuDetailsData.SkuDetail() { sku = item.productId, price = item.price });
                    queryCallback.Call(true, JsonUtility.ToJson(res));
                };

                IabPlugin.EventManager.QuerySkuDetailsFailedEvent += (error) =>
                {
                    Debug.LogWarning("Query sku details Failed: " + error);
                    skuDetailsCallback.Call(false, error);
                };

                IabPlugin.PluginIab.Initialize(key);
#endif
            }

            public static void Purchase(string sku)
            {
                var payload = Payload.Get(sku, PurchasedCount);
                Debug.Log($"Purchase started for {sku} payload={payload}");
#if UNITY_ANDROID
                IabPlugin.PluginIab.PurchaseProduct(sku, payload);
#endif
            }

            public static void Consume(string sku)
            {
                Debug.Log("Consume started for " + sku);
#if UNITY_ANDROID
                IabPlugin.PluginIab.ConsumeProduct(sku);
#endif
            }

            public static void QueryPurchases()
            {
                Debug.Log("Query purchases started!");
#if UNITY_ANDROID
                IabPlugin.PluginIab.QueryPurchases();
#endif
            }

            public static void GetSkuDetails(string[] skus)
            {
                Debug.Log("Query sku details started!");
#if UNITY_ANDROID
                IabPlugin.PluginIab.QuerySkuDetails(skus);
#endif
            }
        }
    }
}
