using veri_ödevi.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DES = veri_ödevi.Encryption.DES;
using veri_ödevi.Encryption;   



namespace veri_ödevi.Helpers
{
    public static class GetFolderFilePath
    {
        public static async Task EncryptFileAsync(this string path, string password, string method)
        {
            await Task.Run(() =>
            {
                switch (method.ToUpperInvariant())
                {
                    case "AES":
                        AES.AES_Encrypt(path, path + ".aes", password);
                        break;
                    case "DES":
                        DES.EncryptFile(path, path + ".des", password);
                        break;
                    case "3DES":
                        _3DES.EncryptFile(path, path + ".3des", password);
                        break;
                    default:
                        throw new ArgumentException("Invalid encryption method. Supported methods are AES, DES, and TripleDES.");
                }
            });
        }
        public static async Task EncryptFilesAsync(this IEnumerable<string> paths, string password, string method)
        {
            await Task.Run(async () =>
            {
                foreach (var path in paths)
                {
                    await path.EncryptFileAsync(password, method );
                    FileLogs.Log(path + " Encrypted.");

                }
            });
        }

        public static string RemoveExtension(this string path)
        {
            var outputpath = Path.ChangeExtension(path, "").TrimEnd(new char[] { '.' });
            return outputpath;
        }

        public static async Task DecryptFileAsync(this string path, string password, string method)
        {
            var outputpath = path.RemoveExtension();
            await Task.Run(() =>
            {
                switch (method.ToUpperInvariant())
                {
                    case "AES":
                        AES.AES_Decrypt(path, outputpath, password);
                        break;
                    case "DES":
                        DES.DecryptFile(path, outputpath, password);
                        break;
                    case "3DES":
                        _3DES.DecryptFile(path, outputpath, password);
                        break;
                    default:
                        throw new ArgumentException("Invalid decryption method. Supported methods are AES, DES, and TripleDES.");
                }
            });
        }

        public static async Task DecryptFilesAsync(this IEnumerable<string> paths, string password, string method)
        {
            await Task.Run(async () =>
            {
                foreach (var path in paths)
                {
                    await path.DecryptFileAsync(password, method);

                }
            });
        }


        public static IEnumerable<string> GetFolderFilesPaths(this string folder, bool followSubDirs = true)
        {
            var paths = new List<string>();
            if (!Directory.Exists(folder))
                return paths;
            if (followSubDirs)
            {
                var subFolders = Directory.GetDirectories(folder);
                if (subFolders != null)
                {

                    foreach (var path in subFolders)
                    {
                        paths.AddRange(GetFolderFilesPaths(path));

                    }

                }
            }
            var subFiles = Directory.GetFiles(folder);
            if (subFiles != null)
            {
                paths.AddRange(subFiles);
            }


            return paths;
        }

        public static IEnumerable<string> ParseExtensions(this string extentions)
        {
            var exts = new List<string>();
            var type = Regex.Match(extentions, @"\(.+\)").Value;
            if (!String.IsNullOrWhiteSpace(type))
                extentions = extentions.Replace(type, String.Empty);

            var matches = Regex.Matches(extentions, @"\b(\w+)\b");

            foreach (var ext in matches)
            {
                exts.Add("." + ext.ToString());

            }

            return exts;
        }


        public static bool CheckExtension(this string path, IEnumerable<string> extensions)
        {
            if (extensions == null)
                return true;
            if (extensions.Count() == 0)
                return true;

            foreach (var ext in extensions)
            {
                if (path.ToLower().EndsWith(ext.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
