using GodSharp.Encryption;
using System;
using System.Security.Cryptography;

namespace Lib.encryption.Symmetric
{
    /// <summary>
    /// Symmetric/AES encryption.
    /// </summary>
    public class AESHelper : __XES__
    {
        public const int KEY_SIZE = 256;
        public const int BLOCK_SIZE = 128;
        public const CipherMode MODE = CipherMode.CBC;
        public const PaddingMode PADDING = PaddingMode.PKCS7;

        static void CheckKeySize(int? keySize)
        {
            if (keySize != null)
            {
                if (!(keySize == 256 || keySize == 192 || keySize == 128))
                    throw new ArgumentOutOfRangeException(nameof(keySize), "keySize only can be 128, 192, or 256");
            }
        }

        public static (byte[] iv, byte[] key) GenericIvKey(
            int? keySize = null,
            int? blockSize = null)
        {
            CheckKeySize(keySize);
            return __GenericIvKey__<AesCryptoServiceProvider>(keySize: keySize, blockSize: blockSize);
        }

        public static byte[] Decrypt(byte[] encryptedData, byte[] key, byte[] iv,
            int? keySize = null,
            int? blockSize = null,
            CipherMode? mode = null,
            PaddingMode? padding = null)
        {
            CheckKeySize(keySize);
            return __Decrypt__<AesCryptoServiceProvider>(encryptedData: encryptedData, key: key, iv: iv,
                keySize: keySize,
                blockSize: blockSize,
                mode: mode,
                padding: padding);
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv,
            int? keySize = null,
            int? blockSize = null,
            CipherMode? mode = null,
            PaddingMode? padding = null)
        {
            CheckKeySize(keySize);
            return __Encrypt__<AesCryptoServiceProvider>(data: data, key: key, iv: iv,
                keySize: keySize,
                blockSize: blockSize,
                mode: mode,
                padding: padding);
        }
    }
}
