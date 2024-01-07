using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace veri_ödevi.Encryption
{
    public class _3DES
    {
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (TripleDESCryptoServiceProvider TripleDES = new TripleDESCryptoServiceProvider())
            {
                TripleDES.KeySize = 192;
                TripleDES.BlockSize = 64;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                TripleDES.Key = key.GetBytes(TripleDES.KeySize / 8);
                TripleDES.IV = key.GetBytes(TripleDES.BlockSize / 8);

                TripleDES.Mode = CipherMode.CBC;
                TripleDES.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = TripleDES.CreateEncryptor())
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }
            }
        }

        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (TripleDESCryptoServiceProvider TripleDES = new TripleDESCryptoServiceProvider())
            {
                TripleDES.KeySize = 192;
                TripleDES.BlockSize = 64;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                TripleDES.Key = key.GetBytes(TripleDES.KeySize / 8);
                TripleDES.IV = key.GetBytes(TripleDES.BlockSize / 8);

                TripleDES.Mode = CipherMode.CBC;
                TripleDES.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = TripleDES.CreateDecryptor())
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputStream);
                }
            }
        }
    }
}
