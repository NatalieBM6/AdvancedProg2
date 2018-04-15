namespace ImageService
{
    partial class ProjectInstaller
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose(); //When managed resources should be disposed.
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /************************************************************************
        *The Input: -.
        *The Output: -
        *The Function operation: The function initiallizing the copmponent.
        *************************************************************************/
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();

            // serviceProcessInstaller1
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;

            // serviceInstaller1
            this.serviceInstaller1.Description = "Image backup service";
            this.serviceInstaller1.DisplayName = "ImageService Display Name";
            this.serviceInstaller1.ServiceName = "ImageService";
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // ProjectInstaller
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});

        }
        #endregion
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}