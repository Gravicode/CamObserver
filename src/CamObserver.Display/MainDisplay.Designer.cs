namespace CamObserver.Display
{
    partial class MainDisplay
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
            this.blazorWebView1 = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            this.TxtInfo = new System.Windows.Forms.RichTextBox();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            this.PicBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // blazorWebView1
            // 
            this.blazorWebView1.Location = new System.Drawing.Point(12, 12);
            this.blazorWebView1.Name = "blazorWebView1";
            this.blazorWebView1.Size = new System.Drawing.Size(517, 435);
            this.blazorWebView1.TabIndex = 0;
            this.blazorWebView1.Text = "blazorWebView1";
            // 
            // TxtInfo
            // 
            this.TxtInfo.Location = new System.Drawing.Point(535, 38);
            this.TxtInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TxtInfo.Name = "TxtInfo";
            this.TxtInfo.Size = new System.Drawing.Size(168, 225);
            this.TxtInfo.TabIndex = 8;
            this.TxtInfo.Text = "";
            // 
            // BtnStop
            // 
            this.BtnStop.Location = new System.Drawing.Point(621, 12);
            this.BtnStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(82, 22);
            this.BtnStop.TabIndex = 7;
            this.BtnStop.Text = "S&top";
            this.BtnStop.UseVisualStyleBackColor = true;
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(533, 12);
            this.BtnStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(82, 22);
            this.BtnStart.TabIndex = 6;
            this.BtnStart.Text = "&Start";
            this.BtnStart.UseVisualStyleBackColor = true;
            // 
            // PicBox1
            // 
            this.PicBox1.Location = new System.Drawing.Point(709, 11);
            this.PicBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PicBox1.Name = "PicBox1";
            this.PicBox1.Size = new System.Drawing.Size(416, 416);
            this.PicBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PicBox1.TabIndex = 5;
            this.PicBox1.TabStop = false;
            // 
            // MainDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1346, 450);
            this.Controls.Add(this.TxtInfo);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.PicBox1);
            this.Controls.Add(this.blazorWebView1);
            this.Name = "MainDisplay";
            this.Text = "MainDisplay";
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView1;
        private RichTextBox TxtInfo;
        private Button BtnStop;
        private Button BtnStart;
        private PictureBox PicBox1;
    }
}