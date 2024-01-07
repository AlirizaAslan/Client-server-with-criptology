using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace veri_ödevi.Encryption
{
    internal class AES
    {
        public static void AES_Encrypt(string srcFile, string encryptedFile, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CFB; // Changed CipherMode to CFB for better security
                AES.Padding = PaddingMode.PKCS7;

                using (FileStream fsInput = new FileStream(srcFile, FileMode.Open, FileAccess.Read))
                using (FileStream fsEncrypted = new FileStream(encryptedFile, FileMode.Create, FileAccess.Write))
                using (var cs = new CryptoStream(fsEncrypted, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] bytearrayinput = new byte[fsInput.Length];
                    fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                    cs.Write(bytearrayinput, 0, bytearrayinput.Length);
                }
            }
        }

        public static void AES_Decrypt(string encryptedFile, string decryptedFile, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CFB; // Changed CipherMode to CFB for better security
                AES.Padding = PaddingMode.PKCS7;

                using (FileStream fsread = new FileStream(encryptedFile, FileMode.Open, FileAccess.Read))
                using (FileStream fsDecrypted = new FileStream(decryptedFile, FileMode.Create, FileAccess.Write))
                using (var cs = new CryptoStream(fsDecrypted, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    byte[] bytearrayinput = new byte[fsread.Length];
                    fsread.Read(bytearrayinput, 0, bytearrayinput.Length);
                    cs.Write(bytearrayinput, 0, bytearrayinput.Length);
                }
            }
        }
    }
}
