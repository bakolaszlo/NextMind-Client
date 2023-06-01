using MLAPI.Cryptography.KeyExchanges;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using ECDiffieHellman = MLAPI.Cryptography.KeyExchanges.ECDiffieHellman;

namespace Assets.Scripts.Crypto
{
    internal class KeyManager : MonoBehaviour
    {

        private void Start()
        {
            StartCoroutine(GetServerPublicKey());
        }

        public static ECDiffieHellman clientDiffie = new ECDiffieHellman();
        private static byte[] sharedKey;
        private static byte[] serverPublicKey;
        private static byte[] salt = { 198, 154, 92, 97, 99, 104, 126, 161, 170, 57, 37, 222, 211, 118, 55, 26, 8, 71, 35, 222, 111, 247, 211, 200, 43, 59, 72, 142, 96, 120, 47, 222 };
        private static byte[] info = new System.Text.UTF8Encoding().GetBytes("Message");


        public static IEnumerator GetServerPublicKey()
        {
            UnityWebRequest www = new UnityWebRequest(Constants.API_URL + Constants.PUBLIC_KEY, "POST");
            www.downloadHandler = new DownloadHandlerBuffer();


            string jsonData = JsonConvert.SerializeObject(Convert.ToBase64String(clientDiffie.GetPublicKey()));
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);

            www.SetRequestHeader("Content-Type", "application/json");
            // Send the request
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.downloadHandler.text.ToString());
                Debug.Log(www.error);
                yield break;
            }
            else
            {
                Debug.Log(www.downloadHandler.text.ToString());
                Debug.Log(www.error);
                serverPublicKey = Convert.FromBase64String(www.downloadHandler.text);
                sharedKey = clientDiffie.GetSharedSecretRaw(serverPublicKey);

            }
            www.Dispose();

        }

        public static byte[] DeriveKey(int length)
        {
            // Use HMAC-SHA256 to derive a one-time pad encryption key
            using (var hmac = new HMACSHA256(sharedKey))
            {
                byte[] prk = hmac.ComputeHash(salt);
                byte[] key = new byte[length];
                byte[] t = new byte[0];

                for (int i = 0; i < length; i++)
                {
                    if (i % 32 == 0)
                    {
                        t = hmac.ComputeHash(Concat(prk, Concat(t, info, new byte[] { (byte)(i / 32 + 1) })));
                    }
                    key[i] = t[i % 32];
                }

                return key;
            }
        }

        private static byte[] Concat(params byte[][] arrays)
        {
            int length = 0;
            foreach (byte[] array in arrays)
            {
                length += array.Length;
            }

            byte[] result = new byte[length];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }
    }
}
