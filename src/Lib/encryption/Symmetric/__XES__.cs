using System;
using System.Security.Cryptography;
using System.Text;

namespace GodSharp.Encryption
{
    /// <summary>
    /// Symmetric/XES encryption.
    /// </summary>
    public abstract class __XES__
    {
        internal static (byte[] iv, byte[] key) __GenericIvKey__<T>(
            int? keySize = null,
            int? blockSize = null) where T : SymmetricAlgorithm, new()
        {
            using (var aes = new T())
            {
                if (keySize != null)
                    aes.KeySize = keySize.Value;
                if (blockSize != null)
                    aes.BlockSize = blockSize.Value;

                aes.GenerateIV();
                aes.GenerateKey();

                return (aes.IV, aes.Key);
            }
        }

        internal static byte[] __Decrypt__<T>(byte[] encryptedData, byte[] key, byte[] iv,
            int? keySize = null,
            int? blockSize = null,
            CipherMode? mode = null,
            PaddingMode? padding = null) where T : SymmetricAlgorithm, new()
        {
            using (var aes = new T())
            {
                if (keySize != null)
                    aes.KeySize = keySize.Value;
                if (blockSize != null)
                    aes.BlockSize = blockSize.Value;
                if (mode != null)
                    aes.Mode = mode.Value;
                if (padding != null)
                    aes.Padding = padding.Value;

                aes.IV = iv;
                aes.Key = key;
                //根据设置好的数据生成解密器实例
                using (ICryptoTransform transform = aes.CreateDecryptor())
                {
                    //解密
                    byte[] bs = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                    return bs;
                }
            }
        }

        internal static byte[] __Encrypt__<T>(byte[] data, byte[] key, byte[] iv,
            int? keySize = null,
            int? blockSize = null,
            CipherMode? mode = null,
            PaddingMode? padding = null) where T : SymmetricAlgorithm, new()
        {
            using (var aes = new T())
            {
                if (keySize != null)
                    aes.KeySize = keySize.Value;
                if (blockSize != null)
                    aes.BlockSize = blockSize.Value;
                if (mode != null)
                    aes.Mode = mode.Value;
                if (padding != null)
                    aes.Padding = padding.Value;

                aes.IV = iv;
                aes.Key = key;
                //根据设置好的数据生成解密器实例
                using (ICryptoTransform transform = aes.CreateEncryptor())
                {
                    //解密
                    byte[] bs = transform.TransformFinalBlock(data, 0, data.Length);

                    return bs;
                }
            }
        }

        /// <summary>
        /// XES encryption.
        /// </summary>
        /// <typeparam name="T">The <see cref="T:System.Security.Cryptography.SymmetricAlgorithm"/> sub-class.</typeparam>
        /// <param name="data">The string to be encrypted,not null.</param>
        /// <param name="password">The password to derive the key for.</param>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding"/>,default is Encoding.UTF8.</param>
        /// <param name="model">The mode for operation of the symmetric algorithm.</param>
        /// <param name="hashName">The hash algorithm to use to derive the key.</param>
        /// <param name="keySize">The size, in bits, of the secret key used by the symmetric algorithm.</param>
        /// <param name="blockSize">The block size, in bits, of the cryptographic operation.</param>
        /// <param name="salt">The key salt to use to derive the key.</param>
        /// <param name="vector">The initialization vector (IV) to use to derive the key.</param>
        /// <returns>The encrypted string.</returns>
        [Obsolete]
        internal static string Encrypt<T>(string data, string password,
            Encoding encoding = null,
            CipherMode model = CipherMode.ECB,
            string hashName = "SHA1",
            int keySize = 128,
            int blockSize = 128,
            string salt = null,
            string vector = null,
            int iterations = 1000) where T : SymmetricAlgorithm, new()
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] valueBytes = encoding.GetBytes(data);

            byte[] encrypted;
            using (T cipher = new T())
            {
                cipher.KeySize = keySize;
                cipher.BlockSize = blockSize;
                cipher.Mode = model;
                //cipher.Padding = PaddingMode.Zeros;

                int keyLength = keySize / 8;

                byte[] saltBytes = salt == null ? encoding.GetBytes(password) : encoding.GetBytes(salt);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, saltBytes, hashName, iterations);
                byte[] keyBytes = pdb.GetBytes(keyLength);

                cipher.Key = keyBytes;

                if (vector != null)
                {
                    int vectorLength = blockSize / 8;
                    byte[] bVector = new byte[vectorLength];
                    Array.Copy(encoding.GetBytes(vector.PadRight(vectorLength)), bVector, vectorLength);
                    cipher.IV = bVector;
                }

                using (ICryptoTransform encryptor = cipher.CreateEncryptor())
                {
                    encrypted = encryptor.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                }

                cipher.Clear();
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// XES decryption.
        /// </summary>
        /// <typeparam name="T">The <see cref="T:System.Security.Cryptography.SymmetricAlgorithm"/> sub-class.</typeparam>
        /// <param name="data">The string to be decrypted,not null.</param>
        /// <param name="password">The password to derive the key for.</param>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding"/>,default is Encoding.UTF8.</param>
        /// <param name="model">The mode for operation of the symmetric algorithm.</param>
        /// <param name="hashName">The hash algorithm to use to derive the key.</param>
        /// <param name="keySize">The size, in bits, of the secret key used by the symmetric algorithm.</param>
        /// <param name="blockSize">The block size, in bits, of the cryptographic operation.</param>
        /// <param name="salt">The key salt to use to derive the key.</param>
        /// <param name="vector">The initialization vector (IV) to use to derive the key.</param>
        /// <returns>The decryption string.</returns>
        [Obsolete]
        internal static string Decrypt<T>(string data, string password,
            Encoding encoding = null,
            CipherMode model = CipherMode.ECB,
            string hashName = "SHA1",
            int keySize = 128,
            int blockSize = 128,
            string salt = null,
            string vector = null,
            int iterations = 1000) where T : SymmetricAlgorithm, new()
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] valueBytes = Convert.FromBase64String(data);

            byte[] decrypted;

            using (T cipher = new T())
            {
                cipher.KeySize = keySize;
                cipher.BlockSize = blockSize;
                cipher.Mode = model;
                //cipher.Padding = PaddingMode.Zeros;

                int keyLength = keySize / 8;

                byte[] saltBytes = salt == null ? encoding.GetBytes(password) : encoding.GetBytes(salt);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, saltBytes, hashName, iterations);
                byte[] keyBytes = pdb.GetBytes(keyLength);

                cipher.Key = keyBytes;

                if (vector != null)
                {
                    int vectorLength = blockSize / 8;
                    byte[] bVector = new byte[vectorLength];
                    Array.Copy(encoding.GetBytes(vector.PadRight(vectorLength)), bVector, vectorLength);
                    cipher.IV = bVector;
                }

                using (ICryptoTransform decryptor = cipher.CreateDecryptor())
                {
                    decrypted = decryptor.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                }

                cipher.Clear();
            }

            return encoding.GetString(decrypted);
        }
    }
}
