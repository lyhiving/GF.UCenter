using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common
{
    public static class EncryptHashManager
    {
        private const int MinSaltSize = 32;
        private const int MaxSaltSize = 128;

        // This is based on the algorithm choosed, please do not change it.
        private const int HashSizeInBits = 512;

        public static string ComputeHash(string text, byte[] salt = null)
        {
            if (salt == null)
            {
                salt = GenerateSalt();
            }

            byte[] textBytes = Encoding.UTF8.GetBytes(text);

            byte[] textSaltBytes = new byte[textBytes.Length + salt.Length];

            Buffer.BlockCopy(textBytes, 0, textSaltBytes, 0, textBytes.Length);
            Buffer.BlockCopy(salt, 0, textSaltBytes, textBytes.Length, salt.Length);

            using (HashAlgorithm hashManaged = new SHA512Managed())
            {
                byte[] hashBytes = hashManaged.ComputeHash(textSaltBytes);

                byte[] hash = new byte[hashBytes.Length + salt.Length];
                Buffer.BlockCopy(hashBytes, 0, hash, 0, hashBytes.Length);
                Buffer.BlockCopy(salt, 0, hash, hashBytes.Length, salt.Length);

                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyHash(string text, string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);

            var hashSize = HashSizeInBits / 8;

            if (hashBytes.Length < hashSize)
                return false;

            byte[] salt = new byte[hashBytes.Length - hashSize];

            Buffer.BlockCopy(hashBytes, hashSize, salt, 0, salt.Length);

            string expectedHash = ComputeHash(text, salt);

            return SlowCompare(hash, expectedHash);
        }

        public static string GenerateToken()
        {
            var salt1 = GenerateSalt();
            return ComputeHash(Convert.ToBase64String(salt1), GenerateSalt());
        }

        private static byte[] GenerateSalt(int minSize = MinSaltSize, int maxSize = MaxSaltSize)
        {
            Random random = new Random();
            int saltSize = random.Next(minSize, maxSize);
            var saltBytes = new byte[saltSize];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);

            return saltBytes;
        }

        /// <summary>
        /// A slow compare function to make the encrypted password string compare almost with the same time
        /// Even there are difference at the first n chars. This can avoid some attach based on remote timing
        /// https://crypto.stanford.edu/~dabo/papers/ssl-timing.pdf
        /// </summary>
        /// <param name="str1">String 1</param>
        /// <param name="str2">String 2</param>
        /// <returns>If the two strings are equals</returns>
        private static bool SlowCompare(string str1, string str2)
        {
            if (str1 == null || str2 == null)
            {
                throw new InvalidOperationException("The parameter cannot be null");
            }

            var diff = str1.Length ^ str2.Length;
            for (int i = Math.Min(str1.Length, str2.Length) - 1; i >= 0; i--)
            {
                diff |= str1[i] ^ str2[i];
            }

            return diff == 0;
        }
    }
}
