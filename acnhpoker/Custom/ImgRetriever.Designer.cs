﻿namespace ACNHPoker
{
    partial class ImgRetriever
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImgRetriever));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.yesBtn = new System.Windows.Forms.Button();
            this.noBtn = new System.Windows.Forms.Button();
            this.msg = new System.Windows.Forms.Label();
            this.waitmsg = new System.Windows.Forms.Label();
            this.configBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Black;
            this.progressBar.ForeColor = System.Drawing.Color.Pink;
            this.progressBar.Location = new System.Drawing.Point(4, 125);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(425, 23);
            this.progressBar.TabIndex = 0;
            // 
            // yesBtn
            // 
            this.yesBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.yesBtn.FlatAppearance.BorderSize = 0;
            this.yesBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.yesBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.yesBtn.ForeColor = System.Drawing.Color.Transparent;
            this.yesBtn.Location = new System.Drawing.Point(87, 85);
            this.yesBtn.Name = "yesBtn";
            this.yesBtn.Size = new System.Drawing.Size(116, 34);
            this.yesBtn.TabIndex = 1;
            this.yesBtn.Text = "Yes";
            this.yesBtn.UseVisualStyleBackColor = false;
            this.yesBtn.Click += new System.EventHandler(this.yesBtn_Click);
            // 
            // noBtn
            // 
            this.noBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.noBtn.FlatAppearance.BorderSize = 0;
            this.noBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.noBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.noBtn.ForeColor = System.Drawing.Color.White;
            this.noBtn.Location = new System.Drawing.Point(238, 85);
            this.noBtn.Name = "noBtn";
            this.noBtn.Size = new System.Drawing.Size(116, 34);
            this.noBtn.TabIndex = 2;
            this.noBtn.Text = "No";
            this.noBtn.UseVisualStyleBackColor = false;
            this.noBtn.Click += new System.EventHandler(this.noBtn_Click);
            // 
            // msg
            // 
            this.msg.AutoSize = true;
            this.msg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.msg.ForeColor = System.Drawing.Color.White;
            this.msg.Location = new System.Drawing.Point(0, 3);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(438, 76);
            this.msg.TabIndex = 3;
            this.msg.Text = "It appears that you do not have the images downloaded. \r\nWould you like to downlo" +
    "ad them now?\r\n\r\n(You can find this dialog in           again if needed)";
            this.msg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waitmsg
            // 
            this.waitmsg.AutoSize = true;
            this.waitmsg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.waitmsg.ForeColor = System.Drawing.Color.Pink;
            this.waitmsg.Location = new System.Drawing.Point(56, 122);
            this.waitmsg.Name = "waitmsg";
            this.waitmsg.Size = new System.Drawing.Size(327, 38);
            this.waitmsg.TabIndex = 4;
            this.waitmsg.Text = "Download Finished! \r\nPlease wait patiently for the file extraction.";
            this.waitmsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.waitmsg.Visible = false;
            // 
            // configBtn
            // 
            this.configBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.configBtn.FlatAppearance.BorderSize = 0;
            this.configBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.configBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.configBtn.ForeColor = System.Drawing.Color.White;
            this.configBtn.Image = global::ACNHPoker.Properties.Resources.gear;
            this.configBtn.Location = new System.Drawing.Point(243, 52);
            this.configBtn.Name = "configBtn";
            this.configBtn.Size = new System.Drawing.Size(30, 30);
            this.configBtn.TabIndex = 156;
            this.configBtn.UseVisualStyleBackColor = false;
            // 
            // ImageDownloader
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(434, 167);
            this.Controls.Add(this.configBtn);
            this.Controls.Add(this.waitmsg);
            this.Controls.Add(this.msg);
            this.Controls.Add(this.noBtn);
            this.Controls.Add(this.yesBtn);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImgRetriever";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Images";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button yesBtn;
        private System.Windows.Forms.Button noBtn;
        private System.Windows.Forms.Label msg;
        private System.Windows.Forms.Label waitmsg;
        private System.Windows.Forms.Button configBtn;
    }
}