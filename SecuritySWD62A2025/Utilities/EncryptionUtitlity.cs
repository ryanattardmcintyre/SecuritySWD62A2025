using System.Text;
using System.Security.Cryptography;


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
            using (CryptoStream cryptoStream = new CryptoStream(outputStream, alg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                //Step 5 - this code depends on which approach you have chosen
                //Feeding the clear data into the cryptoStream
                //that the data will be scrambled automatically
                inputStream.CopyTo(cryptoStream);

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

    }

    public class SymmetricKeys
    {
        public byte[] SecretKey { get; set; }
        public byte[] IV { get; set; }
    }
}
