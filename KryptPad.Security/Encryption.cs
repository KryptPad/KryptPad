using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;


namespace KryptPad.Security
{
    public static class Encryption
    {
        /// <summary>
        /// iteration count for deriving key material
        /// </summary>
        private const int KEY_DERIVATION_ITERATION = 4816;
        private const int KEY_SIZE = 256;
        private const int IV_SIZE = 128;
        private const int IV_LENGTH = IV_SIZE / 8;
        private const int SALT_LENGTH = 32;
        private const string ALGORITHM_NAME = "AES256";


        ///// <summary>
        ///// Gets the encryption key material for a password
        ///// </summary>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //private static IBuffer GetEncryptionKeyMaterial(string password, IBuffer saltBuffer)
        //{
        //    //get a password buffer
        //    var pwBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);

        //    //create provider
        //    var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha512);
        //    //create a key based on original key and derivation parmaters
        //    var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);

        //    //using salt and specified iterations
        //    var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, KEY_DERIVATION_ITERATION);
        //    //derive new key
        //    var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);

        //    //return encryption key
        //    return keyMaterial;
        //}

        ///// <summary>
        ///// Takes the resulting encrypted data and creates a payload with salt and iv
        ///// </summary>
        ///// <param name="cypherBuffer"></param>
        ///// <param name="saltBuffer"></param>
        ///// <param name="ivBuffer"></param>
        ///// <returns></returns>
        //private static IBuffer CreatePayload(IBuffer cypherBuffer, IBuffer saltBuffer, IBuffer ivBuffer)
        //{
        //    //var cypherData = new byte[cypherBuffer.Length];
        //    byte[] cypherBytes;
        //    byte[] saltBytes;
        //    byte[] ivBytes;

        //    //convert to bytes
        //    CryptographicBuffer.CopyToByteArray(cypherBuffer, out cypherBytes);
        //    CryptographicBuffer.CopyToByteArray(saltBuffer, out saltBytes);
        //    CryptographicBuffer.CopyToByteArray(ivBuffer, out ivBytes);

        //    //prepend salt and iv
        //    byte[] resultBytes;
        //    using (var ms = new MemoryStream())
        //    {
        //        //write salt, iv, and cypher data
        //        ms.Write(saltBytes, 0, saltBytes.Length);
        //        ms.Write(ivBytes, 0, ivBytes.Length);
        //        ms.Write(cypherBytes, 0, cypherBytes.Length);

        //        //copy bytes to resultBytes
        //        resultBytes = ms.ToArray();
        //    }

        //    //create new IBuffer from bytes
        //    var resultBuffer = CryptographicBuffer.CreateFromByteArray(resultBytes);

        //    return resultBuffer;
        //}

        ///// <summary>
        ///// Extracts the salt, iv, and cypher data
        ///// </summary>
        ///// <param name="cypherBuffer"></param>
        ///// <param name="saltBuffer"></param>
        ///// <param name="ivBuffer"></param>
        ///// <returns></returns>
        //private static IBuffer ExtractPayload(IBuffer cypherBuffer, out IBuffer saltBuffer, out IBuffer ivBuffer)
        //{
        //    byte[] resultBytes;

        //    //convert to bytes
        //    CryptographicBuffer.CopyToByteArray(cypherBuffer, out resultBytes);

        //    //byte arrays for data
        //    byte[] saltBytes = new byte[32];
        //    byte[] ivBytes = new byte[32];
        //    //cypher data array is result byte length minus salt and iv lengths
        //    byte[] cypherBytes = new byte[resultBytes.Length - (saltBytes.Length + ivBytes.Length)];

        //    //read the first 64 bytes to get salt and iv
        //    using (var ms = new MemoryStream(resultBytes))
        //    {
        //        //read salt
        //        ms.Read(saltBytes, 0, saltBytes.Length);
        //        //read iv
        //        ms.Read(ivBytes, 0, ivBytes.Length);
        //        //the rest is cypher data
        //        ms.Read(cypherBytes, 0, cypherBytes.Length);
        //    }

        //    //output
        //    saltBuffer = CryptographicBuffer.CreateFromByteArray(saltBytes);
        //    ivBuffer = CryptographicBuffer.CreateFromByteArray(ivBytes);

        //    return CryptographicBuffer.CreateFromByteArray(cypherBytes);
        //}

        ///// <summary>
        ///// Encrypts data using password and a randomly generated IV and salt
        ///// </summary>
        ///// <param name="plainText"></param>
        ///// <param name="password"></param>
        //public static byte[] Encrypt(string plainText, string password)
        //{
        //    //create buffer of data to encrypt
        //    var dataBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);

        //    var saltBuffer = CryptographicBuffer.GenerateRandom(32);
        //    var ivBuffer = CryptographicBuffer.GenerateRandom(32);

        //    //get key from our random salt
        //    var keyMaterial = GetEncryptionKeyMaterial(password, saltBuffer);

        //    //create a key for encrypting
        //    var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
        //    var symKey = symProvider.CreateSymmetricKey(keyMaterial);

        //    //encrypt the plain text with key and salt material
        //    var cypherBuffer = CryptographicEngine.Encrypt(symKey, dataBuffer, ivBuffer);

        //    //prepend the IV and salt
        //    var resultBuffer = CreatePayload(cypherBuffer, saltBuffer, ivBuffer);

        //    //encode to base64
        //    byte[] resultBytes;
        //    CryptographicBuffer.CopyToByteArray(resultBuffer, out resultBytes);

        //    //return result
        //    return resultBytes;
        //}

        ///// <summary>
        ///// Decrypts data using password
        ///// </summary>
        ///// <param name="cypherText"></param>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //public static string Decrypt(byte[] cypherData, string password)
        //{
        //    //no cypher text? return null
        //    if (cypherData == null) return null;

        //    //create a buffer from the cypherText
        //    var cypherBuffer = CryptographicBuffer.CreateFromByteArray(cypherData);

        //    IBuffer saltBuffer;
        //    IBuffer ivBuffer;
        //    //extract cypher data
        //    var dataBuffer = ExtractPayload(cypherBuffer, out saltBuffer, out ivBuffer);

        //    //get key from our extracted salt
        //    var keyMaterial = GetEncryptionKeyMaterial(password, saltBuffer);

        //    //create a key for decrypting
        //    var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
        //    var symKey = symProvider.CreateSymmetricKey(keyMaterial);

        //    //decrypt the plain text with key and salt material
        //    var resultBuffer = CryptographicEngine.Decrypt(symKey, dataBuffer, ivBuffer);

        //    var result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, resultBuffer);

        //    return result;
        //}


        /// <summary>
        /// Generates a key from a password and salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltBytes"></param>
        /// <returns></returns>
        public static ParametersWithIV GenerateKey(string password, byte[] saltBytes)
        {
            var passBytes = PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password.ToCharArray());

            //create key generator
            var generator = new Pkcs5S2ParametersGenerator();
            //initialize
            generator.Init(passBytes, saltBytes, KEY_DERIVATION_ITERATION);

            //generate with a 256bit key, and a 128bit IV
            var kp = (ParametersWithIV)generator.GenerateDerivedParameters(ALGORITHM_NAME, KEY_SIZE, IV_SIZE);

            return kp;
        }

        /// <summary>
        /// Generates a key from a password and salt and IV
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltBytes"></param>
        /// <param name="ivBytes"></param>
        /// <returns></returns>
        public static ParametersWithIV GenerateKey(string password, byte[] saltBytes, byte[] ivBytes)
        {
            var passBytes = PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password.ToCharArray());

            //create key generator
            var generator = new Pkcs5S2ParametersGenerator();
            //initialize
            generator.Init(passBytes, saltBytes, KEY_DERIVATION_ITERATION);

            //generate with a 256bit key, and a 128bit IV
            var kp = new ParametersWithIV(generator.GenerateDerivedParameters(ALGORITHM_NAME, KEY_SIZE), ivBytes);

            return kp;
        }

        /// <summary>
        /// Encrypts using AES256Cbc and a password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string plainText, string password)
        {
            byte[] saltBytes = new byte[SALT_LENGTH];

            //create random byte generator
            var rand = new SecureRandom();
            //get random bytes for our salt
            rand.NextBytes(saltBytes);

            //create cipher engine
            var cipher = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(
                    new AesEngine()));

            //get the key parameters from the password
            var key = GenerateKey(password, saltBytes);

            //initialize for encryption with the key
            cipher.Init(true, key);

            //get the message as bytes
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            MemoryStream cipherStream;
            //process the input
            using (cipherStream = new MemoryStream())
            {
                //write iv
                cipherStream.Write(key.GetIV(), 0, key.GetIV().Length);
                //write salt
                cipherStream.Write(saltBytes, 0, saltBytes.Length);

                byte[] outputBytes;
                //get output
                outputBytes = cipher.ProcessBytes(plainBytes);
                //write the data to the stream
                cipherStream.Write(outputBytes, 0, outputBytes.Length);

                //do the final block
                outputBytes = cipher.DoFinal();
                //write the data to the stream
                cipherStream.Write(outputBytes, 0, outputBytes.Length);


            }

            //return the bytes
            return cipherStream.ToArray();
        }

        public static string Decrypt(byte[] cipherData, string password)
        {
            //extract the iv and salt
            byte[] ivBytes = new byte[IV_LENGTH];
            byte[] saltBytes = new byte[SALT_LENGTH];
            byte[] cipherBytes = new byte[cipherData.Length - (ivBytes.Length + saltBytes.Length)];

            //process the input
            using (var cipherStream = new MemoryStream(cipherData))
            {
                //read iv
                cipherStream.Read(ivBytes, 0, ivBytes.Length);
                //read salt
                cipherStream.Read(saltBytes, 0, saltBytes.Length);
                //read cipher bytes
                cipherStream.Read(cipherBytes, 0, cipherBytes.Length);

            }

            //create cipher engine
            var cipher = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(
                    new AesEngine()));

            //get the key parameters from the password
            var key = GenerateKey(password, saltBytes, ivBytes);

            //initialize for decryption with the key
            cipher.Init(false, key);

            MemoryStream plainStream;
            //process the input
            using (plainStream = new MemoryStream())
            {
                byte[] outputBytes;
                //get output
                outputBytes = cipher.ProcessBytes(cipherBytes);
                //write the data to the stream
                plainStream.Write(outputBytes, 0, outputBytes.Length);

                //do the final block
                outputBytes = cipher.DoFinal();
                //write the data to the stream
                plainStream.Write(outputBytes, 0, outputBytes.Length);


            }


            return Encoding.UTF8.GetString(plainStream.ToArray());
        }

    }
}
