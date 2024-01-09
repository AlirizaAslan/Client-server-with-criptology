using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace veri_ödevi.Encryption
{
    public class RC4
    {
        public string RC4Criptology(string input, string key)
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
    }
}
