using System;

namespace UMML.Modding
{
    public static class AssetCrypto
    {
        private static readonly byte[] BaseKeys =
        [
            0x53, 0x2B, 0x46, 0x31, 0xE4, 0xA7, 0xB9, 0x47, 0x3E, 0x7C, 0xFB
        ];

        public static byte[]? DeriveKey(long encryptionKey)
        {
            if (encryptionKey == 0) return null;

            byte[] keyBytes = BitConverter.GetBytes(encryptionKey);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(keyBytes);
            }

            byte[] keys = new byte[BaseKeys.Length * 8];
            for (int i = 0; i < BaseKeys.Length; i++)
            {
                byte b = BaseKeys[i];
                for (int j = 0; j < 8; j++)
                {
                    keys[i * 8 + j] = (byte)(b ^ keyBytes[j]);
                }
            }
            return keys;
        }

        public static void EncryptData(Span<byte> data, long encryptionKey)
        {
            if (encryptionKey == 0 || data.Length <= 256) return;

            byte[]? keys = DeriveKey(encryptionKey);
            if (keys == null || keys.Length == 0) return;

            int klen = keys.Length;
            for (int i = 256; i < data.Length; i++)
            {
                data[i] ^= keys[i % klen];
            }
        }
    }
}
