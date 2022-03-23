namespace RevoScada.DataLoggerService
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.datalogServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.datalogServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // datalogServiceProcessInstaller
            // 
            this.datalogServiceProcessInstaller.Password = null;
            this.datalogServiceProcessInstaller.Username = null;
            // 
            // datalogServiceInstaller
            // 
            this.datalogServiceInstaller.Description = "RevoScadaDataLoggerService";
            this.datalogServiceInstaller.DisplayName = "RevoScadaDataLoggerService";
            this.datalogServiceInstaller.ServiceName = "RevoScadaDataLoggerService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.datalogServiceProcessInstaller,
            this.datalogServiceInstaller});

        }

        #endregion

        public System.ServiceProcess.ServiceProcessInstaller datalogServiceProcessInstaller;
        public System.ServiceProcess.ServiceInstaller datalogServiceInstaller;
    }
}