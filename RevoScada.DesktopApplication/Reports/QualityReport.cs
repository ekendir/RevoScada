using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace RevoScada.DesktopApplication.Reports
{
    public partial class QualityReport : DevExpress.XtraReports.UI.XtraReport
    {
        public QualityReport()
        {
            InitializeComponent();

            XRTable tb = new XRTable();

            XRTableRow tbrow = new XRTableRow();
            XRTableCell tbcell = new XRTableCell();

            tbcell.Text = "ccc";
            tbcell.Font = new System.Drawing.Font("Tahoma", 7F);
            tbrow.Cells.Add(tbcell);
        }
        private void Load(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

    }
}
