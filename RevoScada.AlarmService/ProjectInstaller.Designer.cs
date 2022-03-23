namespace RevoScada.AlarmService
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
            this.alarmServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.DelayedAutoStart = true;
            this.serviceInstaller.Description = "RevoScadaAlarmService";
            this.serviceInstaller.DisplayName = "RevoScadaAlarmService";
            this.serviceInstaller.ServiceName = "RevoScadaAlarmService";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceInstaller,
            this.alarmServiceProcessInstaller});

        }

        #endregion

        public System.ServiceProcess.ServiceProcessInstaller alarmServiceProcessInstaller;
        public System.ServiceProcess.ServiceInstaller serviceInstaller;
    }
}