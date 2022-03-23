using RevoScada.Configurator;

namespace RevoScada.DesktopApplication.Reports
{
    partial class QualityReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QualityReport));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.xrPictureBoxReportHeaderLogoImage = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabelRunStart = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRunEnd = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelRecipeName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLblOperatorName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelLoadNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportHeaderTitleRoot = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 28.6246F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Load);
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine4,
            this.xrPictureBoxReportHeaderLogoImage,
            this.xrLabelRunStart,
            this.xrLabel10,
            this.xrLabel21,
            this.xrLabelRunEnd,
            this.xrLabelRecipeName,
            this.xrLblOperatorName,
            this.xrLabel14,
            this.xrLabel2,
            this.xrLabelReportDate,
            this.xrLabelLoadNumber,
            this.xrLabel22,
            this.xrLabel8,
            this.xrLine1,
            this.xrLabel3,
            this.xrLabelReportHeaderTitleRoot});
            this.TopMargin.HeightF = 189F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLine4
            // 
            this.xrLine4.LineWidth = 2F;
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(0.0001033147F, 169.9999F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(722.9999F, 3.625015F);
            // 
            // xrPicBoxTAIIcon
            // 
            this.xrPictureBoxReportHeaderLogoImage.LocationFloat = new DevExpress.Utils.PointFloat(546F, 10.00001F);
            this.xrPictureBoxReportHeaderLogoImage.Name = "xrPictureBoxReportHeaderLogoImage";
            this.xrPictureBoxReportHeaderLogoImage.SizeF = new System.Drawing.SizeF(177F, 77F);
            this.xrPictureBoxReportHeaderLogoImage.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;
            this.xrPictureBoxReportHeaderLogoImage.ImageUrl = ApplicationConfigurations.Instance.Configuration.ReportHeaderLogoImage;
            // 
            // xrLabelRunStart
            // 
            this.xrLabelRunStart.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRunStart.LocationFloat = new DevExpress.Utils.PointFloat(111.6146F, 153.2499F);
            this.xrLabelRunStart.Name = "xrLabelRunStart";
            this.xrLabelRunStart.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRunStart.SizeF = new System.Drawing.SizeF(160.4167F, 16.75F);
            this.xrLabelRunStart.StylePriority.UseFont = false;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(0F, 153.2499F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(103.1249F, 16.75F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Run Start";
            // 
            // xrLabel21
            // 
            this.xrLabel21.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(457.7916F, 136.4997F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(100F, 16.75F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.Text = "Recipe ";
            // 
            // xrLabelRunEnd
            // 
            this.xrLabelRunEnd.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRunEnd.LocationFloat = new DevExpress.Utils.PointFloat(557.7916F, 153.2499F);
            this.xrLabelRunEnd.Name = "xrLabelRunEnd";
            this.xrLabelRunEnd.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRunEnd.SizeF = new System.Drawing.SizeF(165.2084F, 16.75F);
            this.xrLabelRunEnd.StylePriority.UseFont = false;
            // 
            // xrLabelRecipeName
            // 
            this.xrLabelRecipeName.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelRecipeName.LocationFloat = new DevExpress.Utils.PointFloat(557.7918F, 136.4998F);
            this.xrLabelRecipeName.Name = "xrLabelRecipeName";
            this.xrLabelRecipeName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelRecipeName.SizeF = new System.Drawing.SizeF(165.2082F, 16.75003F);
            this.xrLabelRecipeName.StylePriority.UseFont = false;
            // 
            // xrLblOperatorName
            // 
            this.xrLblOperatorName.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLblOperatorName.LocationFloat = new DevExpress.Utils.PointFloat(557.7918F, 119.7497F);
            this.xrLblOperatorName.Name = "xrLblOperatorName";
            this.xrLblOperatorName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLblOperatorName.SizeF = new System.Drawing.SizeF(165.2082F, 16.75003F);
            this.xrLblOperatorName.StylePriority.UseFont = false;
            // 
            // xrLabel14
            // 
            this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(457.7916F, 153.2498F);
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(100F, 16.75F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.Text = "Run End";
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(457.7916F, 119.7497F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(100F, 16.75F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.Text = "OperatorName";
            // 
            // xrLabelReportDate
            // 
            this.xrLabelReportDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelReportDate.LocationFloat = new DevExpress.Utils.PointFloat(111.6146F, 136.4999F);
            this.xrLabelReportDate.Name = "xrLabelReportDate";
            this.xrLabelReportDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportDate.SizeF = new System.Drawing.SizeF(160.4167F, 16.75F);
            this.xrLabelReportDate.StylePriority.UseFont = false;
            // 
            // xrLabelLoadNumber
            // 
            this.xrLabelLoadNumber.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabelLoadNumber.LocationFloat = new DevExpress.Utils.PointFloat(111.6146F, 119.7499F);
            this.xrLabelLoadNumber.Name = "xrLabelLoadNumber";
            this.xrLabelLoadNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelLoadNumber.SizeF = new System.Drawing.SizeF(160.4167F, 16.75F);
            this.xrLabelLoadNumber.StylePriority.UseFont = false;
            // 
            // xrLabel22
            // 
            this.xrLabel22.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(0F, 136.4999F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(103.1249F, 16.75F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Report Date";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0F, 119.75F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(81.24998F, 16.75F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "LoadNumber";
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 2F;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 82.45827F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(723F, 13.625F);
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 102.6249F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(204.1667F, 16.75F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Batch Report";
            // 
            // printQualityReportFixHeader
            // 
            this.xrLabelReportHeaderTitleRoot.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabelReportHeaderTitleRoot.LocationFloat = new DevExpress.Utils.PointFloat(0F, 26.75001F);
            this.xrLabelReportHeaderTitleRoot.Multiline = true;
            this.xrLabelReportHeaderTitleRoot.Name = "xrLabelReportHeaderTitleRoot";
            this.xrLabelReportHeaderTitleRoot.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportHeaderTitleRoot.SizeF = new System.Drawing.SizeF(250F, 30F);
            this.xrLabelReportHeaderTitleRoot.StylePriority.UseFont = false;
            this.xrLabelReportHeaderTitleRoot.Text = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot + "\r\nQuality Report - Parts";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo1});
            this.BottomMargin.HeightF = 37F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(681.3333F, 10.00001F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(41.66666F, 23F);
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 12.70847F;
            this.PageHeader.Name = "PageHeader";
            // 
            // ReportHeader
            // 
            this.ReportHeader.HeightF = 13.54167F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // QualityReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.ReportHeader});
            this.Margins = new System.Drawing.Printing.Margins(50, 54, 189, 37);
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
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        public DevExpress.XtraReports.UI.XRLabel xrLblOperatorName;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRecipeName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        public DevExpress.XtraReports.UI.XRLabel xrLabelReportDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRunEnd;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        public DevExpress.XtraReports.UI.XRLabel xrLabelRunStart;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxReportHeaderLogoImage;
        private DevExpress.XtraReports.UI.XRLine xrLine4;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        public DevExpress.XtraReports.UI.XRLabel xrLabelReportHeaderTitleRoot;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
    }
}
