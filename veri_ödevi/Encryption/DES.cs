using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace veri_ödevi.Encryption
{
    internal class DES
    {

        // Effective key size in DES is 56 bits, the key is often specified as 64 bits, with 8 bits used for parity checking. 
        // For .NET, the DESCryptoServiceProvider class expects the key size to be a multiple of 8, only 56 bits are used for encryption.

        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.KeySize = 64;
                des.BlockSize = 64;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                des.Key = key.GetBytes(des.KeySize / 8);
                des.IV = key.GetBytes(des.BlockSize / 8);

                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = des.CreateEncryptor())
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

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.KeySize = 64;
                des.BlockSize = 64;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                des.Key = key.GetBytes(des.KeySize / 8);
                des.IV = key.GetBytes(des.BlockSize / 8);

                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = des.CreateDecryptor())
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
