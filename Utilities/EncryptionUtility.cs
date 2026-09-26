using System.Security.Cryptography;
using EncryptionOracleVulnerability.Utilities.Interfaces;

namespace EncryptionOracleVulnerability.Utilities
{
    public class EncryptionUtility : IEncryptionUtility
    {
        private const int KeySize = 32;
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const int Iterations = 600_000;

        // Output layout: [nonce (12 bytes)][tag (16 bytes)][ciphertext]
        public byte[] EncryptDataAesGcm(byte[] data, string password, byte[] salt)
        {
            ArgumentNullException.ThrowIfNull(data);
            ValidateKeyInputs(password, salt);

            byte[] key = DeriveKey(password, salt);
            try
            {
                byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
                byte[] tag = new byte[TagSize];
                byte[] cipherText = new byte[data.Length];

                using (var aes = new AesGcm(key, TagSize))
                {
                    aes.Encrypt(nonce, data, cipherText, tag);
                }

                byte[] result = new byte[NonceSize + TagSize + cipherText.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
                Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
                Buffer.BlockCopy(cipherText, 0, result, NonceSize + TagSize, cipherText.Length);

                return result;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
            }
        }

        public byte[] DecryptDataAesGcm(byte[] data, string password, byte[] salt)
        {
            ArgumentNullException.ThrowIfNull(data);
            ValidateKeyInputs(password, salt);

            if (data.Length < NonceSize + TagSize)
            {
                throw new CryptographicException("The encrypted data is invalid.");
            }

            ReadOnlySpan<byte> nonce = data.AsSpan(0, NonceSize);
            ReadOnlySpan<byte> tag = data.AsSpan(NonceSize, TagSize);
            ReadOnlySpan<byte> cipherText = data.AsSpan(NonceSize + TagSize);

            byte[] key = DeriveKey(password, salt);
            try
            {
                byte[] plainText = new byte[cipherText.Length];

                using (var aes = new AesGcm(key, TagSize))
                {
                    // Throws AuthenticationTagMismatchException if the data was tampered with or the key is wrong.
                    aes.Decrypt(nonce, cipherText, tag, plainText);
                }

                return plainText;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
            }
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        }

        private static void ValidateKeyInputs(string password, byte[] salt)
        {
            ArgumentException.ThrowIfNullOrEmpty(password);
            ArgumentNullException.ThrowIfNull(salt);

            if (salt.Length < 16)
            {
                throw new ArgumentException("Salt must be at least 16 bytes.", nameof(salt));
            }
        }
    }
}
