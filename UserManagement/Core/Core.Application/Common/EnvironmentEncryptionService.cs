
using System.Security.Cryptography;
using System.Text;

namespace Core.Application.Common
{
     public class EnvironmentEncryptionService
    {
        private readonly string _encryptionKey;

        public EnvironmentEncryptionService()
        {
            // Fetch Encryption Key from Environment Variable or use a default secure key
            _encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? "MySuperSecureKey1234567890123456";
        }

        public string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] iv = new byte[16]; // IV must be 16 bytes

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                byte[] iv = new byte[16];

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception)
            {
                return "Decryption Failed!";
            }
        }
    }
}