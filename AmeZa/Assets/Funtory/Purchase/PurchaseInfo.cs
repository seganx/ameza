#if UNITY_ANDROID
namespace IabPlugin
{
    [System.Serializable]
    public class PurchaseInfo
    {
        public enum State : int
        {
            Purchased,
            Canceled,
            Refunded
        }

        public string packageName = string.Empty;
        public string orderId = string.Empty;
        public string productId = string.Empty;
        public string developerPayload = string.Empty;
        public string type = string.Empty;
        public long purchaseTime = 0;
        public State purchaseState = State.Canceled;
        public string purchaseToken = string.Empty;
        public string signature = string.Empty;
        public string originalJson = string.Empty;

        public override string ToString()
        {
            return string.Format("packageName: {0}, orderId: {1}, productId: {2}, developerPayload: {3}, purchaseToken: {4}, purchaseState: {5}, signature: {6}, type: {7}, json: {8}",
                packageName, orderId, productId, developerPayload, purchaseToken, purchaseState, signature, type, originalJson);
        }
    }
}
#endif