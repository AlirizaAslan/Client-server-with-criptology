namespace veri_ödevi
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServerKapaButton = new System.Windows.Forms.Button();
            this.Portlabel = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.Hostlabel = new System.Windows.Forms.Label();
            this.HostTextBox = new System.Windows.Forms.TextBox();
            this.Client_button = new System.Windows.Forms.Button();
            this.server_button = new System.Windows.Forms.Button();
            this.LogLabel = new System.Windows.Forms.Label();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.listViewReceiver = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // ServerKapaButton
            // 
            this.ServerKapaButton.Location = new System.Drawing.Point(183, 41);
            this.ServerKapaButton.Name = "ServerKapaButton";
            this.ServerKapaButton.Size = new System.Drawing.Size(117, 23);
            this.ServerKapaButton.TabIndex = 23;
            this.ServerKapaButton.Text = "Serverı Kapat";
            this.ServerKapaButton.UseVisualStyleBackColor = true;
            // 
            // Portlabel
            // 
            this.Portlabel.AutoSize = true;
            this.Portlabel.Location = new System.Drawing.Point(6, 40);
            this.Portlabel.Name = "Portlabel";
            this.Portlabel.Size = new System.Drawing.Size(31, 16);
            this.Portlabel.TabIndex = 22;
            this.Portlabel.Text = "Port";
            // 
            // PortTextBox
            // 
            this.PortTextBox.Location = new System.Drawing.Point(56, 40);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(100, 22);
            this.PortTextBox.TabIndex = 21;
            this.PortTextBox.Text = "3000";
            // 
            // Hostlabel
            // 
            this.Hostlabel.AutoSize = true;
            this.Hostlabel.Location = new System.Drawing.Point(6, 12);
            this.Hostlabel.Name = "Hostlabel";
            this.Hostlabel.Size = new System.Drawing.Size(35, 16);
            this.Hostlabel.TabIndex = 20;
            this.Hostlabel.Text = "Host";
            // 
            // HostTextBox
            // 
            this.HostTextBox.Location = new System.Drawing.Point(56, 12);
            this.HostTextBox.Name = "HostTextBox";
            this.HostTextBox.Size = new System.Drawing.Size(100, 22);
            this.HostTextBox.TabIndex = 19;
            this.HostTextBox.Text = "127.0.0.1";
            // 
            // Client_button
            // 
            this.Client_button.Location = new System.Drawing.Point(306, 13);
            this.Client_button.Name = "Client_button";
            this.Client_button.Size = new System.Drawing.Size(117, 23);
            this.Client_button.TabIndex = 18;
            this.Client_button.Text = "Client aç";
            this.Client_button.UseVisualStyleBackColor = true;
            this.Client_button.Click += new System.EventHandler(this.Client_button_Click);
            // 
            // server_button
            // 
            this.server_button.Location = new System.Drawing.Point(183, 12);
            this.server_button.Name = "server_button";
            this.server_button.Size = new System.Drawing.Size(117, 23);
            this.server_button.TabIndex = 17;
            this.server_button.Text = "Serverı Başlat";
            this.server_button.UseVisualStyleBackColor = true;
            this.server_button.Click += new System.EventHandler(this.server_button_Click);
            // 
            // LogLabel
            // 
            this.LogLabel.AutoSize = true;
            this.LogLabel.Location = new System.Drawing.Point(435, 18);
            this.LogLabel.Name = "LogLabel";
            this.LogLabel.Size = new System.Drawing.Size(83, 16);
            this.LogLabel.TabIndex = 28;
            this.LogLabel.Text = "Log Console";
            // 
            // LogTextBox
            // 
            this.LogTextBox.Location = new System.Drawing.Point(435, 37);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.Size = new System.Drawing.Size(353, 146);
            this.LogTextBox.TabIndex = 27;
            // 
            // listViewReceiver
            // 
            this.listViewReceiver.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader8});
            this.listViewReceiver.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.listViewReceiver.GridLines = true;
            this.listViewReceiver.HideSelection = false;
            this.listViewReceiver.Location = new System.Drawing.Point(9, 203);
            this.listViewReceiver.Margin = new System.Windows.Forms.Padding(4);
            this.listViewReceiver.Name = "listViewReceiver";
            this.listViewReceiver.Size = new System.Drawing.Size(568, 247);
            this.listViewReceiver.TabIndex = 29;
            this.listViewReceiver.UseCompatibleStateImageBehavior = false;
            this.listViewReceiver.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "File Size";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 130;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Total Time";
            this.columnHeader8.Width = 115;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1390, 450);
            this.Controls.Add(this.listViewReceiver);
            this.Controls.Add(this.LogLabel);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.ServerKapaButton);
            this.Controls.Add(this.Portlabel);
            this.Controls.Add(this.PortTextBox);
            this.Controls.Add(this.Hostlabel);
            this.Controls.Add(this.HostTextBox);
            this.Controls.Add(this.Client_button);
            this.Controls.Add(this.server_button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ServerKapaButton;
        private System.Windows.Forms.Label Portlabel;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label Hostlabel;
        private System.Windows.Forms.TextBox HostTextBox;
        private System.Windows.Forms.Button Client_button;
        private System.Windows.Forms.Button server_button;
        private System.Windows.Forms.Label LogLabel;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.ListView listViewReceiver;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}

