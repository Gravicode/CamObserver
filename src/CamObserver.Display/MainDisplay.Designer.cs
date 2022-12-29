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
            this.ProcessWorker = new System.ComponentModel.BackgroundWorker();
            this.ChkPushToCloud = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.right_panel = new System.Windows.Forms.Panel();
            this.left_panel = new System.Windows.Forms.Panel();
            this.main_container = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.right_panel.SuspendLayout();
            this.left_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.main_container)).BeginInit();
            this.main_container.Panel1.SuspendLayout();
            this.main_container.Panel2.SuspendLayout();
            this.main_container.SuspendLayout();
            this.SuspendLayout();
            // 
            // blazorWebView1
            // 
            this.blazorWebView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blazorWebView1.Location = new System.Drawing.Point(234, 24);
            this.blazorWebView1.Name = "blazorWebView1";
            this.blazorWebView1.Size = new System.Drawing.Size(405, 386);
            this.blazorWebView1.TabIndex = 0;
            this.blazorWebView1.Text = "blazorWebView1";
            // 
            // TxtInfo
            // 
            this.TxtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtInfo.Location = new System.Drawing.Point(25, 425);
            this.TxtInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TxtInfo.Name = "TxtInfo";
            this.TxtInfo.Size = new System.Drawing.Size(614, 102);
            this.TxtInfo.TabIndex = 8;
            this.TxtInfo.Text = "";
            // 
            // PicBox1
            // 
            this.PicBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PicBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PicBox1.Location = new System.Drawing.Point(0, 0);
            this.PicBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PicBox1.Name = "PicBox1";
            this.PicBox1.Size = new System.Drawing.Size(706, 689);
            this.PicBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PicBox1.TabIndex = 5;
            this.PicBox1.TabStop = false;
            // 
            // BtnSync
            // 
            this.BtnSync.Location = new System.Drawing.Point(26, 293);
            this.BtnSync.Name = "BtnSync";
            this.BtnSync.Size = new System.Drawing.Size(196, 50);
            this.BtnSync.TabIndex = 22;
            this.BtnSync.Text = "Sync to Clou&d";
            this.BtnSync.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSync.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnSync.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnSync.UseVisualStyleBackColor = true;
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(26, 227);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(196, 50);
            this.BtnSave.TabIndex = 20;
            this.BtnSave.Text = "Save &Log";
            this.BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSave.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnSave.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnSave.UseVisualStyleBackColor = true;
            // 
            // BtnOpen
            // 
            this.BtnOpen.Location = new System.Drawing.Point(26, 160);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(196, 50);
            this.BtnOpen.TabIndex = 21;
            this.BtnOpen.Text = "&Open File (Dev only)";
            this.BtnOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnOpen.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnOpen.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnOpen.UseVisualStyleBackColor = true;
            // 
            // BtnStop
            // 
            this.BtnStop.Location = new System.Drawing.Point(26, 93);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(196, 50);
            this.BtnStop.TabIndex = 24;
            this.BtnStop.Text = "S&top";
            this.BtnStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnStop.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnStop.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnStop.UseVisualStyleBackColor = true;
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(26, 24);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(196, 50);
            this.BtnStart.TabIndex = 23;
            this.BtnStart.Text = "&Start";
            this.BtnStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnStart.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnStart.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnStart.UseVisualStyleBackColor = true;
            // 
            // TxtStatus
            // 
            this.TxtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtStatus.Font = new System.Drawing.Font("Bahnschrift", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtStatus.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.TxtStatus.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.TxtStatus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.TxtStatus.Location = new System.Drawing.Point(20, 554);
            this.TxtStatus.Name = "TxtStatus";
            this.TxtStatus.Size = new System.Drawing.Size(619, 27);
            this.TxtStatus.TabIndex = 25;
            this.TxtStatus.Text = "Cam Observer v0.1";
            this.TxtStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.TxtStatus.UseStyleColors = true;
            // 
            // BtnConfig
            // 
            this.BtnConfig.Location = new System.Drawing.Point(26, 360);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(196, 50);
            this.BtnConfig.TabIndex = 26;
            this.BtnConfig.Text = "Save &Config";
            this.BtnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnConfig.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.BtnConfig.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.BtnConfig.UseVisualStyleBackColor = true;
            // 
            // ProcessWorker
            // 
            this.ProcessWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ProcessWorker_DoWork);
            this.ProcessWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ProcessWorker_RunWorkerCompleted);
            // 
            // ChkPushToCloud
            // 
            this.ChkPushToCloud.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkPushToCloud.AutoSize = true;
            this.ChkPushToCloud.Location = new System.Drawing.Point(25, 532);
            this.ChkPushToCloud.Name = "ChkPushToCloud";
            this.ChkPushToCloud.Size = new System.Drawing.Size(137, 19);
            this.ChkPushToCloud.TabIndex = 27;
            this.ChkPushToCloud.Text = "&Push Frame to Cloud";
            this.ChkPushToCloud.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.main_container);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1375, 689);
            this.panel1.TabIndex = 28;
            // 
            // right_panel
            // 
            this.right_panel.Controls.Add(this.PicBox1);
            this.right_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.right_panel.Location = new System.Drawing.Point(0, 0);
            this.right_panel.Name = "right_panel";
            this.right_panel.Size = new System.Drawing.Size(706, 689);
            this.right_panel.TabIndex = 28;
            // 
            // left_panel
            // 
            this.left_panel.Controls.Add(this.BtnStart);
            this.left_panel.Controls.Add(this.BtnSync);
            this.left_panel.Controls.Add(this.TxtStatus);
            this.left_panel.Controls.Add(this.BtnSave);
            this.left_panel.Controls.Add(this.ChkPushToCloud);
            this.left_panel.Controls.Add(this.BtnStop);
            this.left_panel.Controls.Add(this.TxtInfo);
            this.left_panel.Controls.Add(this.BtnOpen);
            this.left_panel.Controls.Add(this.BtnConfig);
            this.left_panel.Controls.Add(this.blazorWebView1);
            this.left_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.left_panel.Location = new System.Drawing.Point(0, 0);
            this.left_panel.Name = "left_panel";
            this.left_panel.Size = new System.Drawing.Size(665, 689);
            this.left_panel.TabIndex = 29;
            // 
            // main_container
            // 
            this.main_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main_container.Location = new System.Drawing.Point(0, 0);
            this.main_container.Name = "main_container";
            // 
            // main_container.Panel1
            // 
            this.main_container.Panel1.Controls.Add(this.left_panel);
            // 
            // main_container.Panel2
            // 
            this.main_container.Panel2.Controls.Add(this.right_panel);
            this.main_container.Size = new System.Drawing.Size(1375, 689);
            this.main_container.SplitterDistance = 665;
            this.main_container.TabIndex = 30;
            // 
            // MainDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1375, 689);
            this.Controls.Add(this.panel1);
            this.Name = "MainDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cam Observer v0.1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.right_panel.ResumeLayout(false);
            this.left_panel.ResumeLayout(false);
            this.left_panel.PerformLayout();
            this.main_container.Panel1.ResumeLayout(false);
            this.main_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.main_container)).EndInit();
            this.main_container.ResumeLayout(false);
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
        private System.ComponentModel.BackgroundWorker ProcessWorker;
        private CheckBox ChkPushToCloud;
        private Panel panel1;
        private Panel right_panel;
        private Panel left_panel;
        private SplitContainer main_container;
    }
}