using System;

namespace SeganX
{
    [Serializable]
    public struct CryptoInt
    {
        public int k;
        public int v;
        public int c;

        public IntResult Get => IntResult.Set(Decrypt(v, k));

        public void Decrypt(Action<int> result)
        {
            int res = Decrypt(v, k);
            result?.Invoke(res == c ? res : 0);
        }

        public void Encrypt(int value)
        {
            var rand = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());
            do { k = rand.Next(int.MinValue, int.MaxValue); } while (k == 0);
            v = Encrypt(value, k);
            c = value;
        }

        public override string ToString()
        {
            int res = Decrypt(v, k);
            return res.ToString();
        }

        public static implicit operator CryptoInt(int value)
        {
            var result = new CryptoInt();
            result.Encrypt(value);
            return result;
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static int Encrypt(int value, int key)
        {
            var v = (value ^ key);
            var res = v + key;
            return res;
        }

        private static int Decrypt(int value, int key)
        {
            var v = (value - key);
            var res = v ^ key;
            return res;
        }
    }
}
