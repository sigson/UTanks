using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using SimpleJSON;

namespace UTanksServer.Database
{
    public static class HashUtil
    {
        // Some random MD5 stuff
        public static string MD5(string input)
        {
            if (input == null) input = string.Empty;
            using (MD5 hasher = System.Security.Cryptography.MD5.Create()) {
                StringBuilder sb = new StringBuilder();
                foreach(byte bit in hasher.ComputeHash(Encoding.UTF8.GetBytes(input)))
                    sb.Append(bit.ToString("x2"));
                return sb.ToString();
            }
        }
        // end some random MD5 stuff

        static JSONNode HashOptions { get => Program.Config["HashUtilOptions"]; }
        public static int iterations { get => HashOptions["Iterations"]; }
        public static int saltSize { get => HashOptions["SaltSize"]; }
        public static int keySize { get => HashOptions["KeySize"]; }
        public static string Compute(string password)
        {
            using (Rfc2898DeriveBytes algo = new Rfc2898DeriveBytes(
                password,
                saltSize,
                iterations))
            {
                string key = Convert.ToBase64String(algo.GetBytes(keySize));
                string salt = Convert.ToBase64String(algo.Salt);

                return $"{iterations}:{salt}:{key}";
            }
        }

        public static string Hash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              saltSize,
              iterations))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(keySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{iterations}.{salt}.{key}";
            }
        }

        public static HashUtilCheckResult Check(string hash, string password)
        {
            string[] hashPart = hash.Split(':');

            if (hashPart.Length != 3)
                throw new FormatException("Parameter 'hash' needs to be formatted as '{iterations}:{salt}:{hash}'");

            int iterations = int.Parse(hashPart[0]);
            byte[] salt = Convert.FromBase64String(hashPart[1]);
            byte[] key = Convert.FromBase64String(hashPart[2]);

            bool needsUpgrade = iterations != HashUtil.iterations;

            using (Rfc2898DeriveBytes algo = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations))
            {
                byte[] keyToCheck = algo.GetBytes(keySize);
                bool verified = keyToCheck.SequenceEqual(key);
                return new HashUtilCheckResult
                {
                    verified = verified,
                    needsUpgrade = needsUpgrade
                };
            }
        }
    }
    public struct HashUtilCheckResult
    {
        public bool verified;
        public bool needsUpgrade;
    }
}
