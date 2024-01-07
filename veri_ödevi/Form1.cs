using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace veri_ödevi
{
    public partial class Form1 : Form
    {

        private TcpListener tcpListener;
        private Thread listenerThread;

        private byte[] buffer = new byte[4096];

        public Form1()
        {
            InitializeComponent();
        }

        private void ListenForClients()
        {
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            NetworkStream clientStream = tcpClient.GetStream();

            // Dosya adını ve uzunluğunu al
            byte[] fileNameLengthBytes = new byte[4];
            clientStream.Read(fileNameLengthBytes, 0, 4);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

            byte[] fileNameBytes = new byte[fileNameLength];
            clientStream.Read(fileNameBytes, 0, fileNameLength);
            string fileName = Encoding.UTF8.GetString(fileNameBytes);

            // Dosya boyutunu al
            byte[] fileSizeBytes = new byte[8];
            clientStream.Read(fileSizeBytes, 0, 8);
            long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

            // Dosya algoritmasını al
            byte[] algorithmBytes = new byte[4];
            clientStream.Read(algorithmBytes, 0, 4);
            string algorithm = Encoding.UTF8.GetString(algorithmBytes);

            // Anahtar değerini al
            byte[] keyBytes = new byte[32];
            clientStream.Read(keyBytes, 0, 32);
            string key = Encoding.UTF8.GetString(keyBytes);

            // Dosyayı al
            using (FileStream fileStream = File.Create(fileName))
            {
                int bytesRead;
                long totalBytesRead = 0;

                while (totalBytesRead < fileSize && (bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }
            }

            // Gelen dosyayı ListView'e ekle
            ListViewItem item = new ListViewItem(new string[] { fileName, fileSize.ToString(), algorithm, key });
            listViewReceiver.Invoke((MethodInvoker)delegate
            {
                listViewReceiver.Items.Add(item);
            });

            tcpClient.Close();
        }

        //private void HandleClientComm(object clientObj)
        //{
        //    TcpClient tcpClient = (TcpClient)clientObj;
        //    NetworkStream clientStream = tcpClient.GetStream();

        //    try
        //    {
        //        // Dosya adını ve uzunluğunu al
        //        byte[] fileNameLengthBytes = new byte[4];
        //        if (clientStream.Read(fileNameLengthBytes, 0, 4) != 4)
        //        {
        //            throw new IOException("Dosya adı uzunluğu okunamadı.");
        //        }
        //        int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

        //        byte[] fileNameBytes = new byte[fileNameLength];
        //        if (clientStream.Read(fileNameBytes, 0, fileNameLength) != fileNameLength)
        //        {
        //            throw new IOException("Dosya adı okunamadı.");
        //        }
        //        string fileName = Encoding.UTF8.GetString(fileNameBytes);

        //        // Dosya boyutunu al
        //        byte[] fileSizeBytes = new byte[8];
        //        if (clientStream.Read(fileSizeBytes, 0, 8) != 8)
        //        {
        //            throw new IOException("Dosya boyutu okunamadı.");
        //        }
        //        long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

        //        // Dosya algoritmasını al
        //        byte[] algorithmBytes = new byte[4];
        //        if (clientStream.Read(algorithmBytes, 0, 4) != 4)
        //        {
        //            throw new IOException("Dosya algoritması okunamadı.");
        //        }
        //        string algorithm = Encoding.UTF8.GetString(algorithmBytes);

        //        // Anahtar değerini al
        //        byte[] keyBytes = new byte[32];
        //        if (clientStream.Read(keyBytes, 0, 32) != 32)
        //        {
        //            throw new IOException("Anahtar değeri okunamadı.");
        //        }
        //        string key = Encoding.UTF8.GetString(keyBytes);

        //        // Dosyayı al
        //        using (FileStream fileStream = File.Create(fileName))
        //        {
        //            int bytesRead;
        //            long totalBytesRead = 0;

        //            while (totalBytesRead < fileSize && (bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                fileStream.Write(buffer, 0, bytesRead);
        //                totalBytesRead += bytesRead;
        //            }
        //        }
        //            // Gelen dosyayı ListView'e ekle
        //            ListViewItem item = new ListViewItem(new string[] { fileName, fileSize.ToString(), algorithm, key });
        //            listViewReceiver.Invoke((MethodInvoker)delegate
        //            {
        //                listViewReceiver.Items.Add(item);
        //            });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Hata durumunda işlemleri burada ele alabilir veya loglayabilirsiniz.
        //        Console.WriteLine($"Hata: {ex.Message}");
        //    }
        //    finally
        //    {
        //        // İşlemler tamamlandığında gerekirse bağlantıyı kapatabilirsiniz.
        //        tcpClient.Close();
        //    }
        //}


            private void Client_button_Click(object sender, EventArgs e)
        {
            Form client = new Client();
            client.Show();
        }

        private void server_button_Click(object sender, EventArgs e)
        {
            int port = int.Parse(PortTextBox.Text);
            tcpListener = new TcpListener(IPAddress.Any, port);
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();

            LogTextBox.Text += "Server Başlatıldı.\r\n";
        }
    }
}
