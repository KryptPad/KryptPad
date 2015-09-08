using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace KryptPad.Security
{
    public class Encryption
    {
        /// <summary>
        /// iteration count for deriving key material
        /// </summary>
        private const int KEY_DERIVATION_ITERATION = 147592;

        /// <summary>
        /// Gets the encryption key material for a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static IBuffer GetEncryptionKeyMaterial(string password, IBuffer saltBuffer)
        {
            //get a password buffer
            var pwBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            
            //create provider
            var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha512);
            //create a key based on original key and derivation parmaters
            var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);

            //using salt and specified iterations
            var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, KEY_DERIVATION_ITERATION);
            //derive new key
            var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            
            //return encryption key
            return keyMaterial;
        }

        /// <summary>
        /// Takes the resulting encrypted data and creates a payload with salt and iv
        /// </summary>
        /// <param name="cypherBuffer"></param>
        /// <param name="saltBuffer"></param>
        /// <param name="ivBuffer"></param>
        /// <returns></returns>
        private static IBuffer CreatePayload(IBuffer cypherBuffer, IBuffer saltBuffer, IBuffer ivBuffer)
        {
            //var cypherData = new byte[cypherBuffer.Length];
            byte[] cypherBytes;
            byte[] saltBytes;
            byte[] ivBytes;

            //convert to bytes
            CryptographicBuffer.CopyToByteArray(cypherBuffer, out cypherBytes);
            CryptographicBuffer.CopyToByteArray(saltBuffer, out saltBytes);
            CryptographicBuffer.CopyToByteArray(ivBuffer, out ivBytes);

            //prepend salt and iv
            byte[] resultBytes;
            using (var ms = new MemoryStream())
            {
                //write salt, iv, and cypher data
                ms.Write(saltBytes, 0, saltBytes.Length);
                ms.Write(ivBytes, 0, ivBytes.Length);
                ms.Write(cypherBytes, 0, cypherBytes.Length);

                //copy bytes to resultBytes
                resultBytes = ms.ToArray();
            }

            //create new IBuffer from bytes
            var resultBuffer = CryptographicBuffer.CreateFromByteArray(resultBytes);

            return resultBuffer;
        }

        /// <summary>
        /// Extracts the salt, iv, and cypher data
        /// </summary>
        /// <param name="cypherBuffer"></param>
        /// <param name="saltBuffer"></param>
        /// <param name="ivBuffer"></param>
        /// <returns></returns>
        private static IBuffer ExtractPayload(IBuffer cypherBuffer, out IBuffer saltBuffer, out IBuffer ivBuffer)
        {
            byte[] resultBytes;
            
            //convert to bytes
            CryptographicBuffer.CopyToByteArray(cypherBuffer, out resultBytes);

            //byte arrays for data
            byte[] saltBytes = new byte[32];
            byte[] ivBytes = new byte[32];
            //cypher data array is result byte length minus salt and iv lengths
            byte[] cypherBytes = new byte[resultBytes.Length - (saltBytes.Length + ivBytes.Length)];

            //read the first 64 bytes to get salt and iv
            using (var ms = new MemoryStream(resultBytes))
            {
                //read salt
                ms.Read(saltBytes, 0, saltBytes.Length);
                //read iv
                ms.Read(ivBytes, 0, ivBytes.Length);
                //the rest is cypher data
                ms.Read(cypherBytes, 0, cypherBytes.Length);
            }

            //output
            saltBuffer = CryptographicBuffer.CreateFromByteArray(saltBytes);
            ivBuffer = CryptographicBuffer.CreateFromByteArray(ivBytes);

            return CryptographicBuffer.CreateFromByteArray(cypherBytes);
        }

        /// <summary>
        /// Encrypts data using password and a randomly generated IV and salt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="password"></param>
        public static byte[] Encrypt(string plainText, string password)
        {
            //create buffer of data to encrypt
            var dataBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);

            var saltBuffer = CryptographicBuffer.GenerateRandom(32);
            var ivBuffer = CryptographicBuffer.GenerateRandom(32);

            //get key from our random salt
            var keyMaterial = GetEncryptionKeyMaterial(password, saltBuffer);

            //create a key for encrypting
            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var symKey = symProvider.CreateSymmetricKey(keyMaterial);

            //encrypt the plain text with key and salt material
            var cypherBuffer = CryptographicEngine.Encrypt(symKey, dataBuffer, ivBuffer);

            //prepend the IV and salt
            var resultBuffer = CreatePayload(cypherBuffer, saltBuffer, ivBuffer);

            //encode to base64
            byte[] resultBytes;
            CryptographicBuffer.CopyToByteArray(resultBuffer, out resultBytes);

            //return result
            return resultBytes;
        }

        /// <summary>
        /// Decrypts data using password
        /// </summary>
        /// <param name="cypherText"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Decrypt(byte[] cypherData, string password)
        {
            //no cypher text? return null
            if (cypherData == null) return null;

            //create a buffer from the cypherText
            var cypherBuffer = CryptographicBuffer.CreateFromByteArray(cypherData);

            IBuffer saltBuffer;
            IBuffer ivBuffer;
            //extract cypher data
            var dataBuffer = ExtractPayload(cypherBuffer, out saltBuffer, out ivBuffer);

            //get key from our extracted salt
            var keyMaterial = GetEncryptionKeyMaterial(password, saltBuffer);

            //create a key for decrypting
            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var symKey = symProvider.CreateSymmetricKey(keyMaterial);

            //decrypt the plain text with key and salt material
            var resultBuffer = CryptographicEngine.Decrypt(symKey, dataBuffer, ivBuffer);

            var result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, resultBuffer);

            return result;
        }

    }
}
