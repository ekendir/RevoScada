using RevoScada.Configurator;

namespace RevoScada.DesktopApplication.Reports
{
    partial class NumericReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumericReport));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRecipeDescription = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelEndRun = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRunStart = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportHeaderTitleRoot = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelLoadNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPictureBoxReportHeaderLogoImage = new DevExpress.XtraReports.UI.XRPictureBox();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 4.374981F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Load);
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel8,
            this.xrLabel3,
            this.xrLabelRecipeDescription,
            this.xrLabel21,
            this.xrLabel22,
            this.xrLabelReportDate,
            this.xrLabelEndRun,
            this.xrLabel25,
            this.xrLabel18,
            this.xrLabelRunStart,
            this.xrLabelReportHeaderTitleRoot,
            this.xrLine1,
            this.xrLabel11,
            this.xrLabelLoadNumber,
            this.xrPictureBoxReportHeaderLogoImage});
            this.TopMargin.HeightF = 179.75F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(35.01236F, 93.625F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(81.24998F, 16.75F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "LoadNumber";
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(33.76299F, 42.20835F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(204.1667F, 16.75F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Numeric Report - Primary Sensors";
            // 
            // xrLabelRecipeDescription
            // 
            this.xrLabelRecipeDescription.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRecipeDescription.LocationFloat = new DevExpress.Utils.PointFloat(116.2623F, 114.375F);
            this.xrLabelRecipeDescription.Name = "xrLabelRecipeDescription";
            this.xrLabelRecipeDescription.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRecipeDescription.SizeF = new System.Drawing.SizeF(248.4195F, 16.75F);
            this.xrLabelRecipeDescription.StylePriority.UseFont = false;
            // 
            // xrLabel21
            // 
            this.xrLabel21.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(34.36193F, 114.375F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(81.90038F, 16.75F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.Text = "Recipe Desc.";
            // 
            // xrLabel22
            // 
            this.xrLabel22.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(504.4453F, 93.62502F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(103.1249F, 16.75F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Report Date";
            // 
            // xrLabelReportDate
            // 
            this.xrLabelReportDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelReportDate.LocationFloat = new DevExpress.Utils.PointFloat(608.5698F, 93.62502F);
            this.xrLabelReportDate.Name = "xrLabelReportDate";
            this.xrLabelReportDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportDate.SizeF = new System.Drawing.SizeF(173.4301F, 16.74998F);
            this.xrLabelReportDate.StylePriority.UseFont = false;
            // 
            // xrLabelEndRun
            // 
            this.xrLabelEndRun.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelEndRun.LocationFloat = new DevExpress.Utils.PointFloat(608.5699F, 114.4583F);
            this.xrLabelEndRun.Name = "xrLabelEndRun";
            this.xrLabelEndRun.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelEndRun.SizeF = new System.Drawing.SizeF(173.4299F, 16.75F);
            this.xrLabelEndRun.StylePriority.UseFont = false;
            // 
            // xrLabel25
            // 
            this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(504.4453F, 114.4583F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(103.125F, 16.75F);
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.Text = "Run End";
            // 
            // xrLabel18
            // 
            this.xrLabel18.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(35.01233F, 135.2083F);
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(81.24998F, 16.75F);
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.Text = "Run Start";
            // 
            // xrLabelRunStart
            // 
            this.xrLabelRunStart.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRunStart.LocationFloat = new DevExpress.Utils.PointFloat(116.2623F, 135.2083F);
            this.xrLabelRunStart.Name = "xrLabelRunStart";
            this.xrLabelRunStart.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRunStart.SizeF = new System.Drawing.SizeF(248.4195F, 16.75F);
            this.xrLabelRunStart.StylePriority.UseFont = false;
            // 
            // xrLabel1
            // 
            this.xrLabelReportHeaderTitleRoot.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabelReportHeaderTitleRoot.LocationFloat = new DevExpress.Utils.PointFloat(32.92903F, 10.00001F);
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
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(785.9877F, 13.625F);
            // 
            // xrLabel11
            // 
            this.xrLabel11.BackColor = System.Drawing.Color.DarkGray;
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(4.720783F, 157.9583F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(778.071F, 16.75F);
            this.xrLabel11.StylePriority.UseBackColor = false;
            this.xrLabel11.StylePriority.UseFont = false;
            // 
            // xrLabelLoadNumber
            // 
            this.xrLabelLoadNumber.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelLoadNumber.LocationFloat = new DevExpress.Utils.PointFloat(116.2623F, 93.625F);
            this.xrLabelLoadNumber.Name = "xrLabelLoadNumber";
            this.xrLabelLoadNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelLoadNumber.SizeF = new System.Drawing.SizeF(109.375F, 16.75F);
            this.xrLabelLoadNumber.StylePriority.UseFont = false;
            // 
            // xrPictureBox1
            // 
            this.xrPictureBoxReportHeaderLogoImage.LocationFloat = new DevExpress.Utils.PointFloat(578.7083F, 0F);
            this.xrPictureBoxReportHeaderLogoImage.Name = "xrPictureBoxReportHeaderLogoImage";
            this.xrPictureBoxReportHeaderLogoImage.SizeF = new System.Drawing.SizeF(177F, 77F);
            this.xrPictureBoxReportHeaderLogoImage.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;
            this.xrPictureBoxReportHeaderLogoImage.ImageUrl = ApplicationConfigurations.Instance.Configuration.ReportHeaderLogoImage;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 19F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 3.208478F;
            this.PageHeader.Name = "PageHeader";
            // 
            // NumericReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader});
            this.Margins = new System.Drawing.Printing.Margins(15, 20, 180, 19);
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
        public DevExpress.XtraReports.UI.XRLabel xrLabelLoadNumber;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabelReportHeaderTitleRoot;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRecipeDescription;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        public DevExpress.XtraReports.UI.XRLabel xrLabelReportDate;
        public DevExpress.XtraReports.UI.XRLabel xrLabelEndRun;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRunStart;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxReportHeaderLogoImage;
    }
}
