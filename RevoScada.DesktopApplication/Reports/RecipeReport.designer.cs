using RevoScada.Configurator;
using System;
using System.Windows.Media.Imaging;

namespace RevoScada.DesktopApplication.Reports
{
    partial class RecipeReport
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
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblrecStartDate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblrecEndDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblrecdesc = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelReportHeaderTitleRoot = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblrecname = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrPictureBoxReportHeaderLogoImage = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblrecReportDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLblCreatedByUser = new DevExpress.XtraReports.UI.XRLabel();
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
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(497.4037F, 112.0833F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(103.1249F, 16.75F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Start Date";
            // 
            // lblrecStartDate
            // 
            this.lblrecStartDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.lblrecStartDate.LocationFloat = new DevExpress.Utils.PointFloat(600.5283F, 112.0833F);
            this.lblrecStartDate.Name = "lblrecStartDate";
            this.lblrecStartDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblrecStartDate.SizeF = new System.Drawing.SizeF(173.4301F, 16.74998F);
            this.lblrecStartDate.StylePriority.UseFont = false;
            // 
            // lblrecEndDate
            // 
            this.lblrecEndDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.lblrecEndDate.LocationFloat = new DevExpress.Utils.PointFloat(600.5284F, 132.9166F);
            this.lblrecEndDate.Name = "lblrecEndDate";
            this.lblrecEndDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblrecEndDate.SizeF = new System.Drawing.SizeF(173.4299F, 16.75F);
            this.lblrecEndDate.StylePriority.UseFont = false;
            // 
            // xrLabel25
            // 
            this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(497.4037F, 132.9166F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(103.125F, 16.75F);
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.Text = "End Date";
            // 
            // lblrecdesc
            // 
            this.lblrecdesc.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.lblrecdesc.LocationFloat = new DevExpress.Utils.PointFloat(104.7206F, 112.1666F);
            this.lblrecdesc.Name = "lblrecdesc";
            this.lblrecdesc.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblrecdesc.SizeF = new System.Drawing.SizeF(323.4195F, 16.75F);
            this.lblrecdesc.StylePriority.UseFont = false;
            // 
            // xrLabelReportHeaderTitleRoot
            // 
            this.xrLabelReportHeaderTitleRoot.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabelReportHeaderTitleRoot.LocationFloat = new DevExpress.Utils.PointFloat(2.226019F, 20.41666F);
            this.xrLabelReportHeaderTitleRoot.Multiline = true;
            this.xrLabelReportHeaderTitleRoot.Name = "xrLabelReportHeaderTitleRoot";
            this.xrLabelReportHeaderTitleRoot.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelReportHeaderTitleRoot.SizeF = new System.Drawing.SizeF(250F, 30F);
            this.xrLabelReportHeaderTitleRoot.StylePriority.UseFont = false;
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
            this.xrLabel10.Text = "Recipe Desc.";
            // 
            // lblrecname
            // 
            this.lblrecname.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.lblrecname.LocationFloat = new DevExpress.Utils.PointFloat(104.7206F, 91.3333F);
            this.lblrecname.Name = "lblrecname";
            this.lblrecname.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblrecname.SizeF = new System.Drawing.SizeF(323.4195F, 16.75F);
            this.lblrecname.StylePriority.UseFont = false;
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
            this.xrLabel1,
            this.xrLblCreatedByUser,
            this.xrPictureBoxReportHeaderLogoImage,
            this.xrLabel2,
            this.lblrecReportDate,
            this.lblrecname,
            this.xrLabel10,
            this.xrLabel11,
            this.xrLine1,
            this.lblrecdesc,
            this.xrLabel25,
            this.lblrecEndDate,
            this.lblrecStartDate,
            this.xrLabel22,
            this.xrLabel8,
            this.xrLine2,
            this.xrLabel3,
            this.xrLabelReportHeaderTitleRoot});
            this.ReportHeader.HeightF = 190.2501F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrPictureBoxReportHeaderLogoImage
            // 
            this.xrPictureBoxReportHeaderLogoImage.LocationFloat = new DevExpress.Utils.PointFloat(588F, 0.7083098F);
            this.xrPictureBoxReportHeaderLogoImage.Name = "xrPictureBoxReportHeaderLogoImage";
            this.xrPictureBoxReportHeaderLogoImage.SizeF = new System.Drawing.SizeF(177F, 77F);
            this.xrPictureBoxReportHeaderLogoImage.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;
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
            // lblrecReportDate
            // 
            this.lblrecReportDate.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.lblrecReportDate.LocationFloat = new DevExpress.Utils.PointFloat(105.3509F, 132.9166F);
            this.lblrecReportDate.Name = "lblrecReportDate";
            this.lblrecReportDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblrecReportDate.SizeF = new System.Drawing.SizeF(322.7892F, 16.74998F);
            this.lblrecReportDate.StylePriority.UseFont = false;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(497.4037F, 91.33333F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(103.1249F, 16.75F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "Created By User";
            // 
            // xrLblCreatedByUser
            // 
            this.xrLblCreatedByUser.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLblCreatedByUser.LocationFloat = new DevExpress.Utils.PointFloat(600.5283F, 91.33333F);
            this.xrLblCreatedByUser.Name = "xrLblCreatedByUser";
            this.xrLblCreatedByUser.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLblCreatedByUser.SizeF = new System.Drawing.SizeF(173.4301F, 16.74998F);
            this.xrLblCreatedByUser.StylePriority.UseFont = false;
            // 
            // RecipeReport
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
        public DevExpress.XtraReports.UI.XRLabel lblrecname;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        public DevExpress.XtraReports.UI.XRLabel lblrecdesc;
        private DevExpress.XtraReports.UI.XRLabel xrLabelReportHeaderTitleRoot;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        public DevExpress.XtraReports.UI.XRLabel lblrecStartDate;
        public DevExpress.XtraReports.UI.XRLabel lblrecEndDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        public DevExpress.XtraReports.UI.XRLabel lblrecReportDate;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxReportHeaderLogoImage;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.UI.XRLabel xrLblCreatedByUser;
    }
}
