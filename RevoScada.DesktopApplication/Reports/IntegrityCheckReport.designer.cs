using RevoScada.Configurator;

namespace RevoScada.DesktopApplication.Reports
{
    partial class IntegrityCheckReport
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

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntegrityCheckReport));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelStartDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelEndDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelLoadNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportHeaderTitleRoot = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRecipeName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrPictureBoxReportHeaderLogoImage = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportDate = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 16.45832F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Load);
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 42F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 2F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 0F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 52.62499F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(204.1667F, 16.75F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Recipe Report";
            // 
            // xrLine2
            // 
            this.xrLine2.LineWidth = 2F;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 176.6251F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(772.774F, 3.625F);
            // 
            // xrLabel22
            // 
            this.xrLabel22.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(498.4453F, 91.3333F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(103.1249F, 16.75F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Start Date";
            // 
            // xrLabelStartDate
            // 
            this.xrLabelStartDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelStartDate.LocationFloat = new DevExpress.Utils.PointFloat(601.57F, 91.3333F);
            this.xrLabelStartDate.Name = "xrLabelStartDate";
            this.xrLabelStartDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelStartDate.SizeF = new System.Drawing.SizeF(173.4301F, 16.74998F);
            this.xrLabelStartDate.StylePriority.UseFont = false;
            // 
            // xrLabelEndDate
            // 
            this.xrLabelEndDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelEndDate.LocationFloat = new DevExpress.Utils.PointFloat(601.5701F, 112.1666F);
            this.xrLabelEndDate.Name = "xrLabelEndDate";
            this.xrLabelEndDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelEndDate.SizeF = new System.Drawing.SizeF(173.4299F, 16.75F);
            this.xrLabelEndDate.StylePriority.UseFont = false;
            // 
            // xrLabel25
            // 
            this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(498.4453F, 112.1666F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(103.125F, 16.75F);
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.Text = "End Date";
            // 
            // xrLabelLoadNumber
            // 
            this.xrLabelLoadNumber.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelLoadNumber.LocationFloat = new DevExpress.Utils.PointFloat(104.7206F, 112.1666F);
            this.xrLabelLoadNumber.Name = "xrLabelLoadNumber";
            this.xrLabelLoadNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelLoadNumber.SizeF = new System.Drawing.SizeF(323.4195F, 16.75F);
            this.xrLabelLoadNumber.StylePriority.UseFont = false;
            // 
            // xrLabel1
            // 
            this.xrLabelReportHeaderTitleRoot.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabelReportHeaderTitleRoot.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 20.41666F);
            this.xrLabelReportHeaderTitleRoot.Multiline = true;
            this.xrLabelReportHeaderTitleRoot.Name = "xrLabelReportHeaderTitleRoot";
            this.xrLabelReportHeaderTitleRoot.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportHeaderTitleRoot.SizeF = new System.Drawing.SizeF(250F, 30F);
            this.xrLabelReportHeaderTitleRoot.StylePriority.UseFont = false;
            this.xrLabelReportHeaderTitleRoot.Text = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 2F;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 77.70831F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(772.774F, 13.625F);
            // 
            // xrLabel11
            // 
            this.xrLabel11.BackColor = System.Drawing.Color.DarkGray;
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 159.8751F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(772.774F, 16.75F);
            this.xrLabel11.StylePriority.UseBackColor = false;
            this.xrLabel11.StylePriority.UseFont = false;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 112.1666F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(102.4946F, 16.75F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Load Number";
            // 
            // xrLabelRecipeName
            // 
            this.xrLabelRecipeName.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRecipeName.LocationFloat = new DevExpress.Utils.PointFloat(104.7206F, 91.3333F);
            this.xrLabelRecipeName.Name = "xrLabelRecipeName";
            this.xrLabelRecipeName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRecipeName.SizeF = new System.Drawing.SizeF(323.4195F, 16.75F);
            this.xrLabelRecipeName.StylePriority.UseFont = false;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 91.3333F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(102.4946F, 16.75F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "Recipe Name";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBoxReportHeaderLogoImage,
            this.xrLabel2,
            this.xrLabelReportDate,
            this.xrLabelRecipeName,
            this.xrLabel10,
            this.xrLabel11,
            this.xrLine1,
            this.xrLabelLoadNumber,
            this.xrLabel25,
            this.xrLabelEndDate,
            this.xrLabelStartDate,
            this.xrLabel22,
            this.xrLabel8,
            this.xrLine2,
            this.xrLabel3,
            this.xrLabelReportHeaderTitleRoot});
            this.ReportHeader.HeightF = 190.2501F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrPictureBox1
            // 
            this.xrPictureBoxReportHeaderLogoImage.LocationFloat = new DevExpress.Utils.PointFloat(588F, 0.7083098F);
            this.xrPictureBoxReportHeaderLogoImage.Name = "xrPictureBoxReportHeaderLogoImage";
            this.xrPictureBoxReportHeaderLogoImage.SizeF = new System.Drawing.SizeF(177F, 77F);
            this.xrPictureBoxReportHeaderLogoImage.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;
            this.xrPictureBoxReportHeaderLogoImage.ImageUrl = ApplicationConfigurations.Instance.Configuration.ReportHeaderLogoImage;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(2.226011F, 132.9166F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(103.125F, 16.75F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.Text = "Report Date";
            // 
            // xrLabelReportDate
            // 
            this.xrLabelReportDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelReportDate.LocationFloat = new DevExpress.Utils.PointFloat(105.3509F, 132.9166F);
            this.xrLabelReportDate.Name = "xrLabelReportDate";
            this.xrLabelReportDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportDate.SizeF = new System.Drawing.SizeF(322.7892F, 16.74998F);
            this.xrLabelReportDate.StylePriority.UseFont = false;
            // 
            // IntegrityCheckReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.ReportHeader});
            this.Margins = new System.Drawing.Printing.Margins(38, 14, 42, 2);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Version = "19.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRecipeName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        public DevExpress.XtraReports.UI.XRLabel xrLabelLoadNumber;
        private DevExpress.XtraReports.UI.XRLabel xrLabelReportHeaderTitleRoot;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        public DevExpress.XtraReports.UI.XRLabel xrLabelStartDate;
        public DevExpress.XtraReports.UI.XRLabel xrLabelEndDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        public DevExpress.XtraReports.UI.XRLabel xrLabelReportDate;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxReportHeaderLogoImage;
    }
}
