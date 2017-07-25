using System;
using System.Security.Cryptography;
using System.Text;

namespace Mvc5StarterKit.IzendaBoundary
{
    public static class StringCipher
    {
        private static readonly AesCryptoServiceProvider Crypto = new AesCryptoServiceProvider();
        private static byte[] Key = new byte[] { 0x77, 0x0A, 0x8A, 0x65, 0xDA, 0x15, 0x6D, 0x24, 0xEE, 0x2A, 0x09, 0x32, 0x77, 0x53, 0x01, 0x42 };
        private static byte[] IV = new byte[] { 0x77, 0x0A, 0x8A, 0x65, 0xDA, 0x15, 0x6D, 0x24, 0xEE, 0x2A, 0x09, 0x32, 0x77, 0x53, 0x01, 0x42 };

        public static string Encrypt(string raw, string key)
        {
            byte[] inBlock = Encoding.UTF8.GetBytes(raw);
            ICryptoTransform xfrm = Crypto.CreateEncryptor(Key, IV);
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);
            return Convert.ToBase64String(outBlock);
        }

        public static string Decrypt(string encrypted, string key)
        {
            byte[] inBytes = Convert.FromBase64String(encrypted);
            ICryptoTransform xfrm = Crypto.CreateDecryptor(Key, IV);
            byte[] outBlock = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);

            return Encoding.UTF8.GetString(outBlock);
        }
    }
}