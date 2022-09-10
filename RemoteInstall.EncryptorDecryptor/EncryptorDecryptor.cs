using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RemoteInstall.EncryptorDecryptor
{
    public class EncryptorDecryptorAes
    {
        private const string _key = "Supersecretkeyforsupersecretprojectwithmassivecharstakecarefully";
        private const string _vector = "supersecretstringforvector";

        public byte[] EncryptStringToByteArrayAes(string stringToEncrypt)
        {
            byte[] key = Encoding.UTF8.GetBytes(_key.ToCharArray(), 0, 32);
            byte[] vector = Encoding.UTF8.GetBytes(_vector.ToCharArray(), 0, 16);
            byte[] endcypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = vector;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(stringToEncrypt);
                        }

                        endcypted = memoryStream.ToArray();
                    }
                }
            }

            return endcypted;
        }

        public string DecryptStringFromByteArrayAes(byte[] encryptedData)
        {
            byte[] key = Encoding.UTF8.GetBytes(_key.ToCharArray(), 0, 32);
            byte[] vector = Encoding.UTF8.GetBytes(_vector.ToCharArray(), 0, 16);
            string decryptedData;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = vector;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            decryptedData = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedData;
        }
    }
}
