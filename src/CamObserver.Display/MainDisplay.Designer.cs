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
            this.PicBox1 = new System.Windows.Forms.PictureBox();
            this.BtnSync = new MetroFramework.Controls.MetroTile();
            this.BtnSave = new MetroFramework.Controls.MetroTile();
            this.BtnOpen = new MetroFramework.Controls.MetroTile();
            this.BtnStop = new MetroFramework.Controls.MetroTile();
            this.BtnStart = new MetroFramework.Controls.MetroTile();
            this.TxtStatus = new MetroFramework.Controls.MetroLabel();
            this.BtnConfig = new MetroFramework.Controls.MetroTile();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // blazorWebView1
            // 
            this.blazorWebView1.Location = new System.Drawing.Point(274, 15);
            this.blazorWebView1.Name = "blazorWebView1";
            this.blazorWebView1.Size = new System.Drawing.Size(517, 416);
            this.blazorWebView1.TabIndex = 0;
            this.blazorWebView1.Text = "blazorWebView1";
            // 
            // TxtInfo
            // 
            this.TxtInfo.Location = new System.Drawing.Point(12, 350);
            this.TxtInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TxtInfo.Name = "TxtInfo";
            this.TxtInfo.Size = new System.Drawing.Size(256, 72);
            this.TxtInfo.TabIndex = 8;
            this.TxtInfo.Text = "";
            // 
            // PicBox1
            // 
            this.PicBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PicBox1.Location = new System.Drawing.Point(797, 15);
            this.PicBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PicBox1.Name = "PicBox1";
            this.PicBox1.Size = new System.Drawing.Size(416, 416);
            this.PicBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PicBox1.TabIndex = 5;
            this.PicBox1.TabStop = false;
            // 
            // BtnSync
            // 
            this.BtnSync.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSync.Location = new System.Drawing.Point(12, 239);
            this.BtnSync.Name = "BtnSync";
            this.BtnSync.Size = new System.Drawing.Size(256, 50);
            this.BtnSync.TabIndex = 22;
            this.BtnSync.Text = "Sync to Clou&d";
            this.BtnSync.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSync.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnSync.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnSync.UseVisualStyleBackColor = true;
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(12, 183);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(256, 50);
            this.BtnSave.TabIndex = 20;
            this.BtnSave.Text = "Save &Log";
            this.BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSave.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnSave.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnSave.UseVisualStyleBackColor = true;
            // 
            // BtnOpen
            // 
            this.BtnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOpen.Location = new System.Drawing.Point(12, 127);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(256, 50);
            this.BtnOpen.TabIndex = 21;
            this.BtnOpen.Text = "&Open File (Dev only)";
            this.BtnOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnOpen.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnOpen.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnOpen.UseVisualStyleBackColor = true;
            // 
            // BtnStop
            // 
            this.BtnStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStop.Location = new System.Drawing.Point(12, 71);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(256, 50);
            this.BtnStop.TabIndex = 24;
            this.BtnStop.Text = "S&top";
            this.BtnStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnStop.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnStop.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnStop.UseVisualStyleBackColor = true;
            // 
            // BtnStart
            // 
            this.BtnStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStart.Location = new System.Drawing.Point(12, 15);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(256, 50);
            this.BtnStart.TabIndex = 23;
            this.BtnStart.Text = "&Start";
            this.BtnStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnStart.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnStart.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // TxtStatus
            // 
            this.TxtStatus.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtStatus.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.TxtStatus.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.TxtStatus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.TxtStatus.Location = new System.Drawing.Point(12, 434);
            this.TxtStatus.Name = "TxtStatus";
            this.TxtStatus.Size = new System.Drawing.Size(1206, 27);
            this.TxtStatus.TabIndex = 25;
            this.TxtStatus.Text = "Cam Observer v0.1";
            this.TxtStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.TxtStatus.UseStyleColors = true;
            // 
            // BtnConfig
            // 
            this.BtnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnConfig.Location = new System.Drawing.Point(12, 295);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(256, 50);
            this.BtnConfig.TabIndex = 26;
            this.BtnConfig.Text = "Save &Config";
            this.BtnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnConfig.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnConfig.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnConfig.UseVisualStyleBackColor = true;
            // 
            // MainDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 471);
            this.Controls.Add(this.BtnConfig);
            this.Controls.Add(this.TxtStatus);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.BtnSync);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.BtnOpen);
            this.Controls.Add(this.TxtInfo);
            this.Controls.Add(this.PicBox1);
            this.Controls.Add(this.blazorWebView1);
            this.Name = "MainDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cam Observer v0.1";
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView1;
        private RichTextBox TxtInfo;
        private PictureBox PicBox1;
        private MetroFramework.Controls.MetroTile BtnSync;
        private MetroFramework.Controls.MetroTile BtnSave;
        private MetroFramework.Controls.MetroTile BtnOpen;
        private MetroFramework.Controls.MetroTile BtnStop;
        private MetroFramework.Controls.MetroTile BtnStart;
        private MetroFramework.Controls.MetroLabel TxtStatus;
        private MetroFramework.Controls.MetroTile BtnConfig;
    }
}