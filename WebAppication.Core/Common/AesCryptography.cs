using System.Security.Cryptography;
using System.Text;

namespace WebAppication.Core.Common
{
    public class AesCryptography
    {
        private string _key;
        private string _iv;

        public AesCryptography(string key, string iv)
        {
            _key = key;
            _iv = iv;
        }

        public string GetDecryptedFileContent(string filePath)
        {
            SymmetricAlgorithm aesAlgorithm = Aes.Create();
            var key = Encoding.UTF8.GetBytes(_key);
            var iv = Encoding.UTF8.GetBytes(_iv);
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(key, iv);

            byte[] encryptedDataBuffer = File.ReadAllBytes(filePath);

            // Create a memorystream to write the decrypted data in it 
            using (MemoryStream ms = new MemoryStream(encryptedDataBuffer))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        // Reutrn all the data from the streamreader 
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                SymmetricAlgorithm aesAlgorithm = Aes.Create();
                var key = Encoding.UTF8.GetBytes(_key);
                var iv = Encoding.UTF8.GetBytes(_iv);
                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(key, iv);

                // Create a memory stream to save the encrypted data in it
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cs))
                        {
                            // Write the text in the stream writer 
                            writer.Write(File.ReadAllText(inputFile));
                        }
                    }

                    // Get the result as a byte array from the memory stream 
                    byte[] encryptedDataBuffer = ms.ToArray();

                    // Write the data to a file 
                    File.WriteAllBytes(outputFile, encryptedDataBuffer);
                }

            }
            catch
            {
            }
        }

    }
}
