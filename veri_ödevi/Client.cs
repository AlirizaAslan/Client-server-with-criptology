using veri_ödevi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace veri_ödevi
{
    public partial class Client : Form
    {
        private TcpClient tcpClient;
        public Client()
        {
            InitializeComponent();

           

            FileLogs.SetLogViewer(LogTextBox);
            extentions_comboBox.SelectedItem = "(All Files)";


            
        }


        private void TreeViewDosyalariGoster(string klasorYolu, TreeNode parentNode = null)
        {
            try
            {
                // Klasördeki tüm dosyaları ve alt klasörleri al
                string[] dosyaYollari = Directory.GetFiles(klasorYolu);
                string[] altKlasorler = Directory.GetDirectories(klasorYolu);

                // Klasörü TreeView'a ekle
                TreeNode klasorNode = new TreeNode(Path.GetFileName(klasorYolu));
                if (parentNode == null)
                {
                    treeViewDosyalar.Nodes.Add(klasorNode);
                }
                else
                {
                    parentNode.Nodes.Add(klasorNode);
                }

                // Dosyaları TreeView'a ekle
                foreach (var dosyaYolu in dosyaYollari)
                {
                    TreeNode dosyaNode = new TreeNode(Path.GetFileName(dosyaYolu));
                    klasorNode.Nodes.Add(dosyaNode);
                }

                // Alt klasörleri işle
                foreach (var altKlasor in altKlasorler)
                {
                    TreeViewDosyalariGoster(altKlasor, klasorNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void ŞifreliDosyalariTreeViewEkle(string klasorYolu)
        {
            treeViewDosyalar.Nodes.Clear(); // TreeView'ı temizle

            try
            {
                // Klasördeki tüm dosyaların yollarını al
                string[] dosyaYollari = Directory.GetFiles(klasorYolu);

                foreach (var dosyaYolu in dosyaYollari)
                {
                    if (!DosyaSifreliMi(dosyaYolu))
                    {
                        string dosyaAdi = Path.GetFileName(dosyaYolu);

                        // Dosya adını TreeNode olarak ekleyerek TreeView'a ekle
                        TreeNode dosyaNode = new TreeNode(dosyaAdi);
                        treeViewDosyalar.Nodes.Add(dosyaNode);
                    }
                }

                //MessageBox.Show("Dosyalar başarıyla TreeView'a eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //buton tuşu şifresiz dosyaları gösterecek en son bak
        //private void btnKlasorSec_Click(object sender, EventArgs e)
        //{
        //    using (var folderBrowserDialog = new FolderBrowserDialog())
        //    {
        //        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            txtKlasorYolu.Text = folderBrowserDialog.SelectedPath;
        //        }
        //    }
        //}


        private bool DosyaSifreliMi(string dosyaYolu)
        {
            string uzanti = Path.GetExtension(dosyaYolu).ToLower();

            // Şifreleme kontrolü yapılacak uzantılar
            string[] sifreliUzantilar = { ".aes", ".crypt", ".secure" }; // Örnek uzantılar

            // Dosyanın uzantısı şifreli uzantılardan birisiyle eşleşiyorsa true döner
            return sifreliUzantilar.Contains(uzanti);
        }

        // Kullanıcıdan input almak için kullanılan metod
        private string ShowInputDialog(string prompt, string title)
        {
            Form promptForm = new Form();
            promptForm.Width = 500;
            promptForm.Height = 150;
            promptForm.Text = title;

            Label textLabel = new Label() { Left = 50, Top = 20, Text = prompt };
            System.Windows.Forms.TextBox inputBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 400 };
            System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "OK", Left = 350, Width = 100, Top = 70 };

            confirmation.Click += (sender, e) => { promptForm.Close(); };

            promptForm.Controls.Add(confirmation);
            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);

            promptForm.ShowDialog();

            return inputBox.Text;
        }

        public string RC4(string input, string key)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                int x, y, j = 0;
                int[] box = new int[256];
                for (int i = 0; i < 256; i++)
                    box[i] = i;

                for (int i = 0; i < 256; i++)
                {
                    j = (key[i % key.Length] + box[i] + j) % 256;
                    x = box[i];
                    box[i] = box[j];
                    box[j] = x;
                }

                for (int i = 0; i < input.Length; i++)
                {
                    y = i % 256;
                    j = (box[y] + j) % 256;
                    x = box[y];
                    box[y] = box[j];
                    box[j] = x;
                    result.Append((char)(input[i] ^ box[(box[y] + box[j]) % 256]));
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                // Hata durumunda yapılacak işlemler
                Console.WriteLine("Hata: " + ex.Message);
                return string.Empty; // veya başka bir hata durumu değeri
            }
        }



        //BU KOD SİLİNEBİLİR ÇALIŞMIYOR 
        private async Task SendFileToServerAsync(string filePath)
        {
            try
            {
                // Sunucunun IP adresi ve port numarası
                string serverIp = "127.0.0.1";
                int serverPort = 3000;

                // TCP sunucu ile bağlantı kur
                using (TcpClient client = new TcpClient(serverIp, serverPort))
                using (NetworkStream stream = client.GetStream())
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Dosya boyutunu gönder
                    byte[] fileSizeBytes = BitConverter.GetBytes(fileStream.Length);
                    await stream.WriteAsync(fileSizeBytes, 0, fileSizeBytes.Length);

                    // Dosyayı parça parça oku ve gönder
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda işlemler burada yapılabilir
                Console.WriteLine("Error sending file to server: " + ex.Message);
            }
        }


        private void BaglaButton_Click(object sender, EventArgs e)
        {
            string host = HostTextBox.Text;
            int port = int.Parse(PortTextBox.Text);

            try
            {
                tcpClient = new TcpClient(host, port);
                LogTextBox.Text += "Bağlantı sağlandı.\r\n";

                TreeViewDosyalariGoster(@"C:\Users\mucur\source\repos\Veri Güvenliği\Veri Güvenliği\bin\Debug\Sunucu");
            }
            catch (Exception ex)
            {
                LogTextBox.Text += "Bağlantı hatası: " + ex.Message + "\r\n";
            }
        }

        private void BaglantıKopar_Click(object sender, EventArgs e)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                tcpClient.Close();
                LogTextBox.Text += "Bağlantı koptu.\r\n";
            }
        }

        private void DosyaEkleButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        string fileName = Path.GetFileName(filePath);
                        long fileSize = new FileInfo(filePath).Length;

                        // Kaydetmek istediğiniz klasör yolu
                        string saveFolderPath = @"C:\Users\mucur\source\repos\Veri Güvenliği\Veri Güvenliği\bin\Debug\Sunucu";

                        // Dosyayı belirli klasöre taşı
                        string destinationPath = Path.Combine(saveFolderPath, fileName);
                        File.Copy(filePath, destinationPath, true);

                        treeViewDosyalar.Nodes.Clear();
                       TreeViewDosyalariGoster(@"C:\Users\mucur\source\repos\Veri Güvenliği\Veri Güvenliği\bin\Debug\Sunucu");


                        LogTextBox.Text += $"Dosya '{fileName}' başarıyla kaydedildi: {destinationPath}\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    string filePath = openFileDialog.FileName;
            //    string fileName = Path.GetFileName(filePath);
            //    long fileSize = new FileInfo(filePath).Length;

            //    // Dosya adını gönder
            //    byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            //    byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
            //    tcpClient.GetStream().Write(fileNameLengthBytes, 0, 4);
            //    tcpClient.GetStream().Write(fileNameBytes, 0, fileNameBytes.Length);

            //    // Dosya boyutunu gönder
            //    byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
            //    tcpClient.GetStream().Write(fileSizeBytes, 0, 8);

            //    // Dosya içeriğini gönder
            //    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            //    {
            //        byte[] buffer = new byte[4096];
            //        int bytesRead;

            //        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            tcpClient.GetStream().Write(buffer, 0, bytesRead);
            //        }
            //    }

            //    LogTextBox.Text += $"Dosya '{fileName}' başarıyla yüklendi.\r\n";
            //}

        }

        private void add_folder_button_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select A Folder";
                folderDialog.ShowNewFolderButton = true;
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    var folderPath = folderDialog.SelectedPath;
                    if (!String.IsNullOrEmpty(folderPath))
                    {
                        var items = item_file_path_list_box.Items;
                        if (!items.Contains(folderPath))
                            item_file_path_list_box.Items.Add(folderPath);
                        else
                            FileLogs.Log(folderPath + " is already exist in the list.");
                    }
                }
            }
        }

        private void add_files_button_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select your File(s)";
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Multiselect = true;
                fileDialog.SupportMultiDottedExtensions = true;
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    var files = fileDialog.FileNames;

                    if (files != null && files.Length > 0)
                    {
                        foreach (var filePath in files)
                        {
                            var items = item_file_path_list_box.Items;
                            if (!items.Contains(filePath))
                                item_file_path_list_box.Items.Add(filePath);
                            else
                                FileLogs.Log(filePath + " is already exist in the list.");
                        }
                    }
                }
            }
        }

        private async void encrypt_button_Click(object sender, EventArgs e)
        {
            var count = 0;
            var paths = item_file_path_list_box.Items;

            if (!string.IsNullOrEmpty(encryption_password_textbox2.Text))
            {
                if (aes_checkbox.Checked == false && TripleDES_checkbox.Checked == false && DES_checkbox.Checked == false)
                {
                    MessageBox.Show("Please Choose an Encryption Method");
                }
                else
                {
                    if (aes_checkbox.Checked)
                    {
                        FileLogs.Log("Encryption Started.");
                        if (paths != null && paths.Count > 0)
                        {
                            foreach (string path in paths)
                            {

                                if (File.Exists(path)) // Is File 
                                {
                                    if (path.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                    {
                                        try
                                        {
                                            await path.EncryptFileAsync(encryption_password_textbox2.Text, "AES");
                                            FileLogs.Log(path + " Encrypted.");
                                            count++;


                                            await SendFileToServerAsync(path);

                                            if (delete_orginal_files_checkbox.Checked)
                                                File.Delete(path);
                                        }
                                        catch (Exception ex)
                                        {

                                            FileLogs.Log(path + " " + ex.Message);
                                        }
                                    }
                                }
                                if (Directory.Exists(path)) // Is Folder
                                {
                                    var followSubDirs = follow_sub_folders_checkbox.Checked ? true : false;

                                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                                    foreach (var file in allfiles)
                                    {
                                        if (file.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                        {
                                            if (!file.EndsWith(".aes"))
                                            {
                                                try
                                                {
                                                    await file.EncryptFileAsync(encryption_password_textbox2.Text, "AES");
                                                    FileLogs.Log(file + " Encrypted.");
                                                    count++;

                                                    await SendFileToServerAsync(path);
                                                    if (delete_orginal_files_checkbox.Checked)
                                                        File.Delete(file);
                                                }
                                                catch (Exception ex)
                                                {

                                                    FileLogs.Log(file + " " + ex.Message);
                                                }
                                            }
                                            else
                                            {
                                                FileLogs.Log(file + " Ignored.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //await SendFileToServerAsync(Convert.ToString(path));
                        FileLogs.Log($"Finished : {count} File(s) Encrypted.");
                    }
                    if (DES_checkbox.Checked)
                    {
                        FileLogs.Log("Encryption Started.");
                        if (paths != null && paths.Count > 0)
                        {
                            foreach (string path in paths)
                            {

                                if (File.Exists(path)) // Is File 
                                {
                                    if (path.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                    {
                                        try
                                        {
                                            await path.EncryptFileAsync(encryption_password_textbox2.Text, "DES");
                                            FileLogs.Log(path + " Encrypted.");
                                            count++;

                                            if (delete_orginal_files_checkbox.Checked)
                                                File.Delete(path);
                                        }
                                        catch (Exception ex)
                                        {

                                            FileLogs.Log(path + " " + ex.Message);
                                        }
                                    }
                                }
                                if (Directory.Exists(path)) // Is Folder
                                {
                                    var followSubDirs = follow_sub_folders_checkbox.Checked ? true : false;

                                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                                    foreach (var file in allfiles)
                                    {
                                        if (file.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                        {
                                            if (!file.EndsWith(".des"))
                                            {
                                                try
                                                {
                                                    await file.EncryptFileAsync(encryption_password_textbox2.Text, "DES");
                                                    FileLogs.Log(file + " Encrypted.");
                                                    count++;

                                                    if (delete_orginal_files_checkbox.Checked)
                                                        File.Delete(file);
                                                }
                                                catch (Exception ex)
                                                {

                                                    FileLogs.Log(file + " " + ex.Message);
                                                }
                                            }
                                            else
                                            {
                                                FileLogs.Log(file + " Ignored.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        FileLogs.Log($"Finished : {count} File(s) Encrypted.");
                    }
                    if (TripleDES_checkbox.Checked)
                    {
                        FileLogs.Log("Encryption Started.");
                        if (paths != null && paths.Count > 0)
                        {
                            foreach (string path in paths)
                            {

                                if (File.Exists(path)) // Is File 
                                {
                                    if (path.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                    {
                                        try
                                        {
                                            await path.EncryptFileAsync(encryption_password_textbox2.Text, "3DES");
                                            FileLogs.Log(path + " Encrypted.");
                                            count++;

                                            if (delete_orginal_files_checkbox.Checked)
                                                File.Delete(path);
                                        }
                                        catch (Exception ex)
                                        {

                                            FileLogs.Log(path + " " + ex.Message);
                                        }
                                    }
                                }
                                if (Directory.Exists(path)) // Is Folder
                                {
                                    var followSubDirs = follow_sub_folders_checkbox.Checked ? true : false;

                                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                                    foreach (var file in allfiles)
                                    {
                                        if (file.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                        {
                                            if (!file.EndsWith(".3des"))
                                            {
                                                try
                                                {
                                                    await file.EncryptFileAsync(encryption_password_textbox2.Text, "3DES");
                                                    FileLogs.Log(file + " Encrypted.");
                                                    count++;

                                                    if (delete_orginal_files_checkbox.Checked)
                                                        File.Delete(file);
                                                }
                                                catch (Exception ex)
                                                {

                                                    FileLogs.Log(file + " " + ex.Message);
                                                }
                                            }
                                            else
                                            {
                                                FileLogs.Log(file + " Ignored.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        FileLogs.Log($"Finished : {count} File(s) Encrypted.");
                    }
                }

            }
            else
            {
                MessageBox.Show("Please Enter A Encryption Password");
            }
        }

        private async void decrypt_button_Click(object sender, EventArgs e)
        {
            var count = 0;
            var paths = item_file_path_list_box.Items;

            if (!string.IsNullOrEmpty(encryption_password_textbox2.Text))
            {
                if (aes_checkbox.Checked == false && TripleDES_checkbox.Checked == false && DES_checkbox.Checked == false)
                {
                    MessageBox.Show("Please Choose an Decryption Method");
                }
                else
                {
                    if (aes_checkbox.Checked)
                    {
                        FileLogs.Log("Decryption Started.");

                        if (paths.Count > 0)
                        {
                            foreach (string path in paths)
                            {

                                if (File.Exists(path) && path.EndsWith(".aes")) // Is Encrypted File 
                                {
                                    try
                                    {
                                        await path.DecryptFileAsync(encryption_password_textbox2.Text, "AES");
                                        FileLogs.Log(path + " Decrypted.");
                                        count++;

                                        if (delete_orginal_files_checkbox.Checked)
                                            File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        FileLogs.Log(path + " " + ex.Message);
                                        if (File.Exists(path.RemoveExtension()))
                                            File.Delete(path.RemoveExtension());
                                    }
                                }
                                if (Directory.Exists(path)) // Is Folder
                                {
                                    var followSubDirs = follow_sub_folders_checkbox.Checked ? true : false;

                                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                                    foreach (var file in allfiles)
                                    {
                                        if (file.RemoveExtension().CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                        {
                                            if (file.EndsWith(".aes"))
                                            {
                                                try
                                                {
                                                    await file.DecryptFileAsync(encryption_password_textbox2.Text, "AES");
                                                    FileLogs.Log(file + " Decrypted.");
                                                    count++;

                                                    if (delete_orginal_files_checkbox.Checked)
                                                        File.Delete(file);
                                                }
                                                catch (Exception ex)
                                                {
                                                    FileLogs.Log(file + " " + ex.Message);
                                                    if (File.Exists(file.RemoveExtension()))
                                                        File.Delete(file.RemoveExtension());
                                                }
                                            }

                                        }
                                        else
                                        {
                                            FileLogs.Log(file + " Ignored.");
                                        }
                                    }
                                }
                            }
                        }
                        FileLogs.Log($"Finished : {count} File(s) Decrypted.");
                    }
                    if (DES_checkbox.Checked)
                    {
                        FileLogs.Log("Decryption Started.");

                        if (paths.Count > 0)
                        {
                            foreach (string path in paths)
                            {

                                if (File.Exists(path) && path.EndsWith(".des")) // Is Encrypted File 
                                {
                                    try
                                    {
                                        await path.DecryptFileAsync(encryption_password_textbox2.Text, "DES");
                                        FileLogs.Log(path + " Decrypted.");
                                        count++;

                                        if (delete_orginal_files_checkbox.Checked)
                                            File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        FileLogs.Log(path + " " + ex.Message);
                                        if (File.Exists(path.RemoveExtension()))
                                            File.Delete(path.RemoveExtension());
                                    }


                                }
                                if (Directory.Exists(path)) // Is Folder
                                {
                                    var followSubDirs = follow_sub_folders_checkbox.Checked ? true : false;

                                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                                    foreach (var file in allfiles)
                                    {
                                        if (file.RemoveExtension().CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                        {
                                            if (file.EndsWith(".des"))
                                            {
                                                try
                                                {
                                                    await file.DecryptFileAsync(encryption_password_textbox2.Text, "DES");
                                                    FileLogs.Log(file + " Decrypted.");
                                                    count++;

                                                    if (delete_orginal_files_checkbox.Checked)
                                                        File.Delete(file);
                                                }
                                                catch (Exception ex)
                                                {
                                                    FileLogs.Log(file + " " + ex.Message);
                                                    if (File.Exists(file.RemoveExtension()))
                                                        File.Delete(file.RemoveExtension());
                                                }
                                            }

                                        }
                                        else
                                        {
                                            FileLogs.Log(file + " Ignored.");
                                        }
                                    }
                                }
                            }
                        }
                        FileLogs.Log($"Finished : {count} File(s) Decrypted.");
                    }
                    if (TripleDES_checkbox.Checked)
                    {
                        FileLogs.Log("Decryption Started.");

                        if (paths.Count > 0)
                        {
                            foreach (string path in paths)
                            {

                                if (File.Exists(path) && path.EndsWith(".3des")) // Is Encrypted File 
                                {
                                    try
                                    {
                                        await path.DecryptFileAsync(encryption_password_textbox2.Text, "3DES");
                                        FileLogs.Log(path + " Decrypted.");
                                        count++;

                                        if (delete_orginal_files_checkbox.Checked)
                                            File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        FileLogs.Log(path + " " + ex.Message);
                                        if (File.Exists(path.RemoveExtension()))
                                            File.Delete(path.RemoveExtension());
                                    }
                                }
                                if (Directory.Exists(path)) // Is Folder
                                {
                                    var followSubDirs = follow_sub_folders_checkbox.Checked ? true : false;

                                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                                    foreach (var file in allfiles)
                                    {
                                        if (file.RemoveExtension().CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                                        {
                                            if (file.EndsWith(".3des"))
                                            {
                                                try
                                                {
                                                    await file.DecryptFileAsync(encryption_password_textbox2.Text, "3DES");
                                                    FileLogs.Log(file + " Decrypted.");
                                                    count++;

                                                    if (delete_orginal_files_checkbox.Checked)
                                                        File.Delete(file);
                                                }
                                                catch (Exception ex)
                                                {
                                                    FileLogs.Log(file + " " + ex.Message);
                                                    if (File.Exists(file.RemoveExtension()))
                                                        File.Delete(file.RemoveExtension());
                                                }
                                            }

                                        }
                                        else
                                        {
                                            FileLogs.Log(file + " Ignored.");
                                        }
                                    }
                                }
                            }
                        }
                        FileLogs.Log($"Finished : {count} File(s) Decrypted.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Enter A Decryption Password");
            }
        }

        private void save_logs_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Loads Faster ;)
                switch (string.IsNullOrEmpty(LogTextBox.Text))
                {
                    case true:
                        MessageBox.Show("No logs found.");
                        break;

                    case false:
                        {
                            using (var saveFileDialog = new SaveFileDialog())
                            {
                                saveFileDialog.Title = "Select your File(s)";
                                saveFileDialog.CheckFileExists = false; // Allow the user to specify a new file
                                saveFileDialog.CheckPathExists = true;
                                saveFileDialog.SupportMultiDottedExtensions = true;
                                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                                    {
                                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                                        {
                                            writer.Write(LogTextBox.Text);
                                        }
                                        MessageBox.Show("Text saved to file successfully!");
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void btnRc4Şifrele_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dosyaYolu = openFileDialog.FileName;
                    // RC4 için anahtarın bir kısmını paroladan oluştur
                    string userPassword = ShowInputDialog("Lütfen bir parola girin:", "Parola");

                    var şifrelenmiş = RC4(encryption_password_textbox.Text, userPassword);
                    //bu şifrelenmiş değeri txt ye yazdır 

                    DosyaYazdir(dosyaYolu, şifrelenmiş);
                }
            }

        }

        private void btnRc4Deşifrele_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    string dosyaYolu = openFileDialog.FileName;
                    // RC4 için anahtarın bir kısmını paroladan oluştur
                    string userPassword = ShowInputDialog("Lütfen bir parola girin:", "Parola");

                    //txtdeki şifrelenmiş veriyi oku

                    var deşifreleme = RC4(DosyaOku(dosyaYolu), userPassword);

                    encryption_password_textbox.Text = deşifreleme;
                }
            }
        }

        static void DosyaYazdir(string dosyaYolu, string deger)
        {
            // Dosyaya yazdırma (mevcut içeriği siler)
            File.WriteAllText(dosyaYolu, deger);
        }

        static string DosyaOku(string dosyaYolu)
        {
            // Dosyayı okuma
            return File.ReadAllText(dosyaYolu);
        }

        private void SilButton_Click(object sender, EventArgs e)
        {
            // Seçilen öğeyi sil
            TreeNode selectedNode = treeViewDosyalar.SelectedNode;

            if (selectedNode != null)
            {
                // Dosya veya klasör adını al
                string ad = selectedNode.Text;

                // Dosya mı klasör mü olduğunu kontrol et
                bool klasor = (selectedNode.Tag != null && (bool)selectedNode.Tag);

                DosyaKlasorSil(ad, klasor);
            }
            else
            {
                MessageBox.Show("Lütfen bir dosya veya klasör seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DosyaKlasorSil(string ad, bool klasor)
        {
            try
            {
                if (klasor)
                {
                    // Klasörü sil
                    Directory.Delete(ad, true);
                }
                else
                {
                    try
                    {
                        // Dosyayı sil
                        File.Delete(ad);
                        MessageBox.Show("Dosya veya klasör başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya veya klasör başarıyla silinmedi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }

                // TreeView'dan öğeyi kaldır
                treeViewDosyalar.SelectedNode.Remove();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsAccessible = true;
        private void btnŞifresizDosyalar_Click(object sender, EventArgs e)
        {
            if (IsAccessible)
            {
                // İlk durumu gerçekleştir
                treeViewDosyalar.Nodes.Clear();
                TreeViewDosyalariGoster(@"C:\Users\mucur\source\repos\Veri Güvenliği\Veri Güvenliği\bin\Debug\Sunucu");
                btnŞifresizDosyalar.Text = "şifreli dosyaları göster";

            }
            else
            {
                // İkinci durumu gerçekleştir
                treeViewDosyalar.Nodes.Clear();
                ŞifreliDosyalariTreeViewEkle(@"C:\Users\mucur\source\repos\Veri Güvenliği\Veri Güvenliği\bin\Debug\Sunucu");
                btnŞifresizDosyalar.Text="Tüm dosyaları göster";

            }

            // İkinci durumu gerçekleştirmek üzere anahtarı tersine çevir
            IsAccessible = !IsAccessible;
        }
    }
}
