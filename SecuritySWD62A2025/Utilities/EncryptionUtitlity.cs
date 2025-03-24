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

    }

    public class SymmetricKeys
    {
        public byte[] SecretKey { get; set; }
        public byte[] IV { get; set; }
    }
}
