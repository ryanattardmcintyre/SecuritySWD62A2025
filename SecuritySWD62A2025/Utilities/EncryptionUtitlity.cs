using System.Text;
using System.Security.Cryptography;
using SecuritySWD62A2025.Models.DatabaseModels;


namespace SecuritySWD62A2025.Utilities
{
    public class EncryptionUtility
    {
        public byte[] Hash(byte[] input)
        {
            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(input);
            return digest;
        }

        /// <summary>
        /// this method wraps the above method and does any conversions necessary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Hash(string input)
        {
            byte[] inputAsBytes = Encoding.UTF32.GetBytes(input);
            byte[] digest = Hash(inputAsBytes);
            //best data format to use after there's data output from ANY cryptographic algorithm is base64
            string result = Convert.ToBase64String(digest);
            return result;
        }

        //technique 1 - will use a user particular to generate keys out of it
        public SymmetricKeys GenerateSymmetricKeys(SymmetricAlgorithm myAlg, string id, byte[] salt)
        {
            Rfc2898DeriveBytes myGenAlg = new Rfc2898DeriveBytes(id, salt);
            SymmetricKeys symmetricKeys = new SymmetricKeys()
            {
                IV = myGenAlg.GetBytes(myAlg.BlockSize/ 8),
                SecretKey = myGenAlg.GetBytes(myAlg.KeySize/ 8)
            };
            return symmetricKeys;
        }

        //technique 2 - will generate a random key BUT then you need to think where you are going to store them securely
        public SymmetricKeys GenerateSymmetricKeys(SymmetricAlgorithm myAlg)
        {
            myAlg.GenerateIV(); myAlg.GenerateKey();
            SymmetricKeys symmetricKeys = new SymmetricKeys()
            {
                IV = myAlg.IV,
                SecretKey = myAlg.Key
            };
            return symmetricKeys;
        }


        public MemoryStream SymmetricEncrypt(MemoryStream inputStream, SymmetricAlgorithm alg, SymmetricKeys keys)
        {
            //Creating a memorystream to be used throughout the method, because it gives us
            //two important methods that facilitate the work a lot
            //1. CopyTo()
            //2. ToArray()
            //Step 1 - We reset the position of the inputStream so we make sure that we start encrypting byte no 0
            inputStream.Position = 0;

            //Step 2 - inject the received keys into the algorithm
            alg.Key = keys.SecretKey;
            alg.IV = keys.IV;

            //Step 3 - Preparing a stream object where to store the cipher data
            MemoryStream outputStream = new MemoryStream();

            //Step 4 - start the encryption engine
            //Approach 1 - are you going to read from the inputStream
            // CryptoStream cryptoStream = new CryptoStream(inputStream, alg.CreateEncryptor(), CryptoStreamMode.Read);

            //Approach 2 - are you going to write to the outputStream
            using (CryptoStream cryptoStream = new CryptoStream(inputStream, alg.CreateEncryptor(), CryptoStreamMode.Read))
            {
                //Step 5 - this code depends on which approach you have chosen
                //Feeding the clear data into the cryptoStream
                //that the data will be scrambled automatically
                cryptoStream.CopyTo(outputStream);

                //Step 6 - finding a way how to flush the data out of the CryptoStream into the outputStream
                cryptoStream.Flush();
            }

            //Step 7 - return the cipher
            outputStream.Position = 0; //to ensure  next time i access the outputstream it starts reading from 0
            return outputStream;
        }

        public string SymmetricEncrypt(string input, SymmetricAlgorithm alg, SymmetricKeys keys)
        {
            //Convert from string -> memorystream
            byte[] inputBytes = Encoding.UTF32.GetBytes(input);
            MemoryStream inputStream = new MemoryStream(inputBytes);

            //Encrypt
            MemoryStream cipherStream = SymmetricEncrypt(inputStream, alg, keys); //base64 encrypted data


            //Convert from memorystream -> string
            byte[] cipherBytes = cipherStream.ToArray();
            return Convert.ToBase64String(cipherBytes);
        }



        public MemoryStream SymmetricDecrypt(MemoryStream inputEncryptedStream, SymmetricAlgorithm alg, SymmetricKeys keys)
        {

            //1. in the alg assign the keys

            //2. reset the postiion of inputEncryptedStream to 0

            //3. prepare an output Stream that will contain clear data

            //4. create a CryptoStream passing the (inputEncryptedStream, DECRYPTOR: alg.CreateDecryptor(), Read)

            //5. create a using {...} in which you will copy from cryptoStream to outputStream the dec bytes

            //6. reset the position of the output stream to 0

            //7. return outputstream
            return null; //<< this is temporary
        }

        public AsymmetricKeys GenerateAsymmetricKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            AsymmetricKeys keys = new AsymmetricKeys();
            keys.PublicKey = rsa.ToXmlString(false);
            keys.PrivateKey = rsa.ToXmlString(true);

            return keys;

        }

        //hello world X = input has to be base64
        public byte[] AsymmetricEncryption(byte[] input, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            byte [] cipher = rsa.Encrypt(input, true);
            return cipher;
        }

        public byte[] AsymmetricDecryption(byte[] cipher, string privateKey)
        {
            return null;
        }

        public MemoryStream HybridEncrypt(MemoryStream input, string publicKey)
        {
            //1. generate the symm keys
            var symmKeys = GenerateSymmetricKeys(Aes.Create() );

            //2. symm enc the input
            var cipher = SymmetricEncrypt(input, Aes.Create(), symmKeys);

            //3. asymm enc the keys
            var encKey = AsymmetricEncryption(symmKeys.SecretKey, publicKey);
            var encIv = AsymmetricEncryption(symmKeys.IV, publicKey);

            //4. package everything in the same memorystream
            MemoryStream output = new MemoryStream();
            output.Write(encKey, 0, encKey.Length); //0
            output.Write(encIv, 0, encIv.Length); //where it has left from the prior line
            cipher.Position = 0;
            cipher.CopyTo(output);
            output.Position = 0;
            output.Close();
            
            return output;
        }

        public string DigitallySign(MemoryStream input, string privateKey)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);
            byte[] inputAsBytes = input.ToArray();
            byte[] signatureAsBytes = RSA.SignData(inputAsBytes,
                new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signatureAsBytes);
        }


        public bool DigitallyVerify(MemoryStream input, string publicKey, string signature)
        {
            byte[] signatureAsBytes = Convert.FromBase64String(signature);
            //incomplete method
            return true; //<<change this with VerifyData(...);
        }

    }

    public class SymmetricKeys
    {
        public byte[] SecretKey { get; set; }
        public byte[] IV { get; set; }
    }
}
