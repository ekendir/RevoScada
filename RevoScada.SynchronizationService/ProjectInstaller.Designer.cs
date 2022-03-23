namespace RevoScada.SynchronizationService
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
            this.syncServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.syncServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // syncServiceProcessInstaller
            // 
            this.syncServiceProcessInstaller.Password = null;
            this.syncServiceProcessInstaller.Username = null;
            // 
            // syncServiceInstaller
            // 
            this.syncServiceInstaller.DelayedAutoStart = true;
            this.syncServiceInstaller.Description = "RevoScadaSynchronizationService";
            this.syncServiceInstaller.DisplayName = "RevoScadaSynchronizationService";
            this.syncServiceInstaller.ServiceName = "RevoScadaSynchronizationService";
            this.syncServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.syncServiceProcessInstaller,
            this.syncServiceInstaller});

        }

        #endregion
        public System.ServiceProcess.ServiceProcessInstaller syncServiceProcessInstaller;
        public System.ServiceProcess.ServiceInstaller syncServiceInstaller;
    }
}