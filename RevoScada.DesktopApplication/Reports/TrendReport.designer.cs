using RevoScada.Configurator;

namespace RevoScada.DesktopApplication.Reports
{
    partial class TrendReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrendReport));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrChartTrendReport = new DevExpress.XtraReports.UI.XRChart();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrPictureBoxReportHeaderLogoImage = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabelLoadNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabelReportHeaderTitleRoot = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRunStart = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelEndRun = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRecipeDescription = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrChartTrendReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrChartTrendReport});
            this.Detail.HeightF = 225.8333F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Load);
            // 
            // xrChartTrendReport
            // 
            this.xrChartTrendReport.BorderColor = System.Drawing.Color.Black;
            this.xrChartTrendReport.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrChartTrendReport.Legend.Name = "Default Legend";
            this.xrChartTrendReport.LocationFloat = new DevExpress.Utils.PointFloat(2.226019F, 0F);
            this.xrChartTrendReport.Name = "xrChartTrendReport";
            this.xrChartTrendReport.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.xrChartTrendReport.SizeF = new System.Drawing.SizeF(772.774F, 225.8333F);
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 7.625008F;
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
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBoxReportHeaderLogoImage,
            this.xrLabelLoadNumber,
            this.xrLabel11,
            this.xrLine1,
            this.xrLabelReportHeaderTitleRoot,
            this.xrLabelRunStart,
            this.xrLabel18,
            this.xrLabel25,
            this.xrLabelEndRun,
            this.xrLabelReportDate,
            this.xrLabel22,
            this.xrLabel21,
            this.xrLabelRecipeDescription,
            this.xrLabel3,
            this.xrLabel8});
            this.ReportHeader.HeightF = 177.1666F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrPictureBox1
            // 
            this.xrPictureBoxReportHeaderLogoImage.LocationFloat = new DevExpress.Utils.PointFloat(578.7083F, 0F);
            this.xrPictureBoxReportHeaderLogoImage.Name = "xrPictureBoxReportHeaderLogoImage";
            this.xrPictureBoxReportHeaderLogoImage.SizeF = new System.Drawing.SizeF(177F, 77F);
            this.xrPictureBoxReportHeaderLogoImage.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;
            this.xrPictureBoxReportHeaderLogoImage.ImageUrl = ApplicationConfigurations.Instance.Configuration.ReportHeaderLogoImage;
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
            // xrLabel11
            // 
            this.xrLabel11.BackColor = System.Drawing.Color.DarkGray;
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(4.720783F, 157.9583F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(760.2792F, 16.75F);
            this.xrLabel11.StylePriority.UseBackColor = false;
            this.xrLabel11.StylePriority.UseFont = false;
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 2F;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(775F, 13.625F);
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
            // xrLabelRunStart
            // 
            this.xrLabelRunStart.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRunStart.LocationFloat = new DevExpress.Utils.PointFloat(116.2623F, 135.2083F);
            this.xrLabelRunStart.Name = "xrLabelRunStart";
            this.xrLabelRunStart.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRunStart.SizeF = new System.Drawing.SizeF(248.4195F, 16.75F);
            this.xrLabelRunStart.StylePriority.UseFont = false;
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
            // xrLabelEndRun
            // 
            this.xrLabelEndRun.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelEndRun.LocationFloat = new DevExpress.Utils.PointFloat(608.5699F, 114.4583F);
            this.xrLabelEndRun.Name = "xrLabelEndRun";
            this.xrLabelEndRun.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelEndRun.SizeF = new System.Drawing.SizeF(156.4301F, 16.75001F);
            this.xrLabelEndRun.StylePriority.UseFont = false;
            // 
            // xrLabelReportDate
            // 
            this.xrLabelReportDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelReportDate.LocationFloat = new DevExpress.Utils.PointFloat(608.5698F, 93.62502F);
            this.xrLabelReportDate.Name = "xrLabelReportDate";
            this.xrLabelReportDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportDate.SizeF = new System.Drawing.SizeF(156.4302F, 16.74998F);
            this.xrLabelReportDate.StylePriority.UseFont = false;
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
            // xrLabelRecipeDescription
            // 
            this.xrLabelRecipeDescription.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRecipeDescription.LocationFloat = new DevExpress.Utils.PointFloat(116.2623F, 114.375F);
            this.xrLabelRecipeDescription.Name = "xrLabelRecipeDescription";
            this.xrLabelRecipeDescription.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRecipeDescription.SizeF = new System.Drawing.SizeF(248.4195F, 16.75F);
            this.xrLabelRecipeDescription.StylePriority.UseFont = false;
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
            // TrendReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.ReportHeader});
            this.Margins = new System.Drawing.Printing.Margins(38, 14, 8, 2);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Version = "19.2";
            ((System.ComponentModel.ISupportInitialize)(this.xrChartTrendReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        public DevExpress.XtraReports.UI.XRChart xrChartTrendReport;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxReportHeaderLogoImage;
        public DevExpress.XtraReports.UI.XRLabel xrLabelLoadNumber;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabelReportHeaderTitleRoot;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRunStart;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        public DevExpress.XtraReports.UI.XRLabel xrLabelEndRun;
        public DevExpress.XtraReports.UI.XRLabel xrLabelReportDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRecipeDescription;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
    }
}
