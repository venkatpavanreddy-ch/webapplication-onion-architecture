using System.IO.Compression;
using WebAppication.Core.Common;

namespace WebAppication.Infrastructure.Common
{
    public static class FileManager
    {
        public static string GetSecrets(string key, string iv, string filePath)
        {
            AesCryptography aescrypto = new AesCryptography(key, iv);
            return aescrypto.GetDecryptedFileContent(filePath);
        }

        public static MemoryStream CreateZipFile(string[] filenames)
        {
            var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var file in filenames)
                {
                    var entry = archive.CreateEntry(Path.GetFileName(file), CompressionLevel.Fastest);
                    using (var zipStream = entry.Open())
                    {
                        var bytes = File.ReadAllBytes(file);
                        zipStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            return ms;
        }

        public static void EncryptFile(string key, string iv, string inputFile, string outputFile)
        {
            AesCryptography aescrypto = new AesCryptography(key, iv);
            aescrypto.EncryptFile(inputFile, outputFile);
        }
    }
}
