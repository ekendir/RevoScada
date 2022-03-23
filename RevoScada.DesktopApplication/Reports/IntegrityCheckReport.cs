using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace RevoScada.DesktopApplication.Reports
{
    public partial class IntegrityCheckReport : DevExpress.XtraReports.UI.XtraReport
    {
        public IntegrityCheckReport()
        {
            InitializeComponent();
        }

        private void Load(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

    }
}
