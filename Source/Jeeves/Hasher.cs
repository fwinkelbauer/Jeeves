using System;
using System.IO;
using System.Security.Cryptography;

namespace Jeeves
{
    internal static class Hasher
    {
        public static string Hash(string secret, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(secret, saltBytes, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);

                return Convert.ToBase64String(hash);
            }
        }

        public static string LoadSalt(string saltPath)
        {
            if (!File.Exists(saltPath))
            {
                WriteSalt(saltPath);
            }

            return File.ReadAllText(saltPath);
        }

        private static void WriteSalt(string saltPath)
        {
            byte[] salt = new byte[16];

            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(salt);
            }

            File.WriteAllText(saltPath, Convert.ToBase64String(salt));
        }
    }
}
