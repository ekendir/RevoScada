namespace Revo.ScadaHelper
{
    partial class frmMain
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
            this.btnClearCache = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnResetLoadNumber = new System.Windows.Forms.Button();
            this.txtResetLoadNumberValue = new System.Windows.Forms.TextBox();
            this.btnInitializeSyncState = new System.Windows.Forms.Button();
            this.txtPlcDeviceIdInitSync = new System.Windows.Forms.TextBox();
            this.txtServerId = new System.Windows.Forms.TextBox();
            this.BtnMonitorPLCSyncItem = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClearCache
            // 
            this.btnClearCache.Location = new System.Drawing.Point(13, 13);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(217, 23);
            this.btnClearCache.TabIndex = 0;
            this.btnClearCache.Text = "Clear Cache";
            this.btnClearCache.UseVisualStyleBackColor = true;
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(504, 12);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(699, 589);
            this.txtOutput.TabIndex = 1;
            // 
            // btnResetLoadNumber
            // 
            this.btnResetLoadNumber.Location = new System.Drawing.Point(13, 42);
            this.btnResetLoadNumber.Name = "btnResetLoadNumber";
            this.btnResetLoadNumber.Size = new System.Drawing.Size(217, 23);
            this.btnResetLoadNumber.TabIndex = 2;
            this.btnResetLoadNumber.Text = "Reset Load Number";
            this.btnResetLoadNumber.UseVisualStyleBackColor = true;
            this.btnResetLoadNumber.Click += new System.EventHandler(this.BtnResetLoadNumber_Click);
            // 
            // txtResetLoadNumberValue
            // 
            this.txtResetLoadNumberValue.Location = new System.Drawing.Point(236, 42);
            this.txtResetLoadNumberValue.Name = "txtResetLoadNumberValue";
            this.txtResetLoadNumberValue.Size = new System.Drawing.Size(113, 20);
            this.txtResetLoadNumberValue.TabIndex = 3;
            // 
            // btnInitializeSyncState
            // 
            this.btnInitializeSyncState.Location = new System.Drawing.Point(13, 71);
            this.btnInitializeSyncState.Name = "btnInitializeSyncState";
            this.btnInitializeSyncState.Size = new System.Drawing.Size(217, 23);
            this.btnInitializeSyncState.TabIndex = 4;
            this.btnInitializeSyncState.Text = "Initialize Sync State";
            this.btnInitializeSyncState.UseVisualStyleBackColor = true;
            this.btnInitializeSyncState.Click += new System.EventHandler(this.BtnInitializeSyncState_Click);
            // 
            // txtPlcDeviceIdInitSync
            // 
            this.txtPlcDeviceIdInitSync.Location = new System.Drawing.Point(236, 71);
            this.txtPlcDeviceIdInitSync.Name = "txtPlcDeviceIdInitSync";
            this.txtPlcDeviceIdInitSync.Size = new System.Drawing.Size(113, 20);
            this.txtPlcDeviceIdInitSync.TabIndex = 5;
            this.txtPlcDeviceIdInitSync.Text = "Plc device Id";
            // 
            // txtServerId
            // 
            this.txtServerId.Location = new System.Drawing.Point(355, 71);
            this.txtServerId.Name = "txtServerId";
            this.txtServerId.Size = new System.Drawing.Size(100, 20);
            this.txtServerId.TabIndex = 6;
            // 
            // BtnMonitorPLCSyncItem
            // 
            this.BtnMonitorPLCSyncItem.Location = new System.Drawing.Point(13, 139);
            this.BtnMonitorPLCSyncItem.Name = "BtnMonitorPLCSyncItem";
            this.BtnMonitorPLCSyncItem.Size = new System.Drawing.Size(172, 23);
            this.BtnMonitorPLCSyncItem.TabIndex = 7;
            this.BtnMonitorPLCSyncItem.Text = "Monitor PLC Sync Item";
            this.BtnMonitorPLCSyncItem.UseVisualStyleBackColor = true;
            this.BtnMonitorPLCSyncItem.Click += new System.EventHandler(this.BtnMonitorPLCSyncItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 601);
            this.Controls.Add(this.BtnMonitorPLCSyncItem);
            this.Controls.Add(this.txtServerId);
            this.Controls.Add(this.txtPlcDeviceIdInitSync);
            this.Controls.Add(this.btnInitializeSyncState);
            this.Controls.Add(this.txtResetLoadNumberValue);
            this.Controls.Add(this.btnResetLoadNumber);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnClearCache);
            this.Name = "frmMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnResetLoadNumber;
        private System.Windows.Forms.TextBox txtResetLoadNumberValue;
        private System.Windows.Forms.Button btnInitializeSyncState;
        private System.Windows.Forms.TextBox txtPlcDeviceIdInitSync;
        private System.Windows.Forms.TextBox txtServerId;
        private System.Windows.Forms.Button BtnMonitorPLCSyncItem;
    }
}

