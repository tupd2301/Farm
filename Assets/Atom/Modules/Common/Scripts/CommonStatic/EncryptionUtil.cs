using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Atom
{
    public class EncryptionUtil
    {
        private static string KEY = "bubbleshooter_password_protected";

        private static byte[] KEY_BYTES = Encoding.UTF8.GetBytes(KEY);

        public static string Encrypt(string plainText)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                //throw new ArgumentNullException("plainText");
                return "";
            }

            byte[] encrypted;
            using (Rijndael algorithm = Rijndael.Create())
            {
                algorithm.Key = KEY_BYTES;

                var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            msEncrypt.Write(algorithm.IV, 0, algorithm.IV.Length);
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                //throw new ArgumentNullException("cipherText");
                return "";
            }

            string plaintext = null;

            using (Rijndael algorithm = Rijndael.Create())
            {
                algorithm.Key = KEY_BYTES;

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    byte[] IV = new byte[16];
                    msDecrypt.Read(IV, 0, IV.Length);

                    algorithm.IV = IV;

                    var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
