#if UNITY_ANDROID
using UnityEngine;

namespace Plankton.Proxy
{
    public class ProxyBilling : Proxy
    {
        private const string logName = "[Plankton] [Billing]";

        private readonly AndroidJavaObject objectBilling = null;

        public ProxyBilling(string marketName, string rsaKey = "")
        {
            Debug.Log($"{logName} Initializing market:{marketName}");
            Utils.Jsoner.AddParams("storeName", marketName, "storeKey", rsaKey);
            var json = Utils.Jsoner.GetJsonAndClear();
            objectBilling = GetAndroidObject("getBilling");
#if !UNITY_EDITOR
            if (objectBilling != null)
            {
                objectBilling?.Call("initialize", json);
                Debug.Log($"{logName} Called initialization");
            }
            else Debug.LogWarning($"{logName} objectBilling is null!");
#endif
        }

        public void Purchase(string sku, string payload)
        {
            Debug.Log($"{logName} Purchase {sku} Payload {payload}");
            objectBilling?.Call("purchase", sku, false, false, string.Empty, string.Empty, payload);
        }

        public void Purchase(string sku, bool autoAck = false, bool autoConsume = false, string obfuscatedAccountId = "", string obfuscatedProfiledId = "")
        {
            Debug.Log($"{logName} Purchase sku:{sku} autoAck:{autoAck} autoConsume:{autoConsume} obfuscatedAccountId:{obfuscatedAccountId} obfuscatedProfiledId:{obfuscatedProfiledId}");
            objectBilling?.Call("purchase", sku, autoAck, autoConsume, obfuscatedAccountId, obfuscatedProfiledId, string.Empty);
        }

        public void Acknowledge(string token)
        {
            Debug.Log($"{logName} AcknowledgePurchase token:{token}");
            objectBilling?.Call("acknowledgePurchase", token);
        }

        public void Consume(string token)
        {
            Debug.Log($"{logName} Consume token:{token}");
            objectBilling?.Call("consume", token);
        }

        public void GetSkuDetails(string camaSeparatedSkus)
        {
            Debug.Log($"{logName} GetSkuDetails {camaSeparatedSkus}");
            objectBilling?.Call("getSkuDetails", camaSeparatedSkus);
        }

        public void GetPurchases()
        {
            Debug.Log($"{logName} GetPurchases");
            objectBilling?.Call("getPurchases");
        }
    }
}
#endif