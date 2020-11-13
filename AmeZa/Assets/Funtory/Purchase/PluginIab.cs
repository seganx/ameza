
using UnityEngine;

#if UNITY_ANDROID
namespace IabPlugin
{
    public class PluginIab
    {
        private static AndroidJavaObject plugin = null;

        public static string GetVersion()
        {
            return "1.0.3";
        }

        static PluginIab()
        {
#if UNITY_EDITOR
            plugin = null;
#else
            // Get the plugin instance
#if BAZAAR
            using (var pluginClass = new AndroidJavaClass("com.bazaar.BazaarIABPlugin"))
#elif MYKET
            using (var pluginClass = new AndroidJavaClass("com.myket.MyketIABPlugin"))
#endif
                plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
#endif
        }

        // Toggles high detail logging on/off
        public static void EnableLogging(bool enable)
        {
            plugin?.Call("enableLogging", enable);
            if (enable)
                Debug.LogWarning("YOU HAVE ENABLED HIGH DETAIL LOGS. DO NOT DISTRIBUTE THE GENERATED APK PUBLICLY. IT WILL DUMP SENSITIVE INFORMATION TO THE CONSOLE!");
        }

        // Initializes the billing system
        public static void Initialize(string publicKey)
        {
            plugin?.Call("init", publicKey);
        }

        // Unbinds and shuts down the billing service
        public static void UnbindService()
        {
            plugin?.Call("unbindService");
        }

        // Returns whether subscriptions are supported on the current device
        public static bool AreSubscriptionsSupported()
        {
            return plugin != null && plugin.Call<bool>("areSubscriptionsSupported");
        }

        // Sends a request to get all completed purchases and product information as setup in the CafeBazaar portal about the provided skus
        public static void QueryInventory(string[] skus)
        {
            plugin?.Call("queryInventory", new object[] { skus });
        }

        // Sends a request to get all product information as setup in the CafeBazaar portal about the provided skus
        public static void QuerySkuDetails(string[] skus)
        {
            plugin?.Call("querySkuDetails", new object[] { skus });
        }

        // Sends a request to get all completed purchases
        public static void QueryPurchases()
        {
            plugin?.Call("queryPurchases");
        }

        // Sends out a request to purchase the product
        public static void PurchaseProduct(string sku)
        {
            PurchaseProduct(sku, string.Empty);
        }

        public static void PurchaseProduct(string sku, string developerPayload)
        {
            plugin?.Call("purchaseProduct", sku, developerPayload);
        }

        // Sends out a request to consume the product
        public static void ConsumeProduct(string sku)
        {
            plugin?.Call("consumeProduct", sku);
        }

        // Sends out a request to consume all of the provided products
        public static void ConsumeProducts(string[] skus)
        {
            plugin?.Call("consumeProducts", new object[] { skus });
        }
    }
}

#endif
