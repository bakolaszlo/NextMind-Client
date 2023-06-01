using System;
using System.Text;
using Assets.Scripts.Crypto;
using CryptSharp;

namespace Assets.Scripts.Communication
{
    public class OneTimePad
    {
        public static byte[] Cipher(byte[] ciphertext)
        {
            byte[] key = KeyManager.DeriveKey(ciphertext.Length);
            // Decrypt the ciphertext using the one-time pad
            byte[] plaintext = new byte[ciphertext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = (byte)(ciphertext[i] ^ key[i]);
            }

            return plaintext;
        }
    }
}
