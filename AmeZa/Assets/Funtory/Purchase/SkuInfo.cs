#if UNITY_ANDROID
namespace IabPlugin
{
    [System.Serializable]
    public class SkuInfo
    {
        public string title = string.Empty;
        public string price = string.Empty;
        public string type = string.Empty;
        public string description = string.Empty;
        public string productId = string.Empty;

        public override string ToString()
        {
            return string.Format("title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}", title, price, type, description, productId);
        }
    }
}
#endif