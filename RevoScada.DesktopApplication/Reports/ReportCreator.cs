using DevExpress.XtraReports.UI;
using Revo.Core.Data;
using RevoScada.Business;
using RevoScada.Business.Report;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraCharts;
using static RevoScada.DesktopApplication.Reports.QualityBatchReportCreator;
using RevoScada.Entities.Enums;

namespace RevoScada.DesktopApplication.Reports
{

    //todo:l REFACTOR
    public class ReportCreator
    {
        private readonly int _batchId;
        private readonly string _connectionString;
        public ReportCreator(int batchId, string connectionString)
        {
            _batchId = batchId;
            _connectionString = connectionString;
        }
        public ReportCreator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public XtraReport IntegrityCheckReport()
        {
            XtraReport xtraReport = null;
            IntegrityCheckReport integrityCheckReport = new IntegrityCheckReport();
            //RecipeReport integrityCheckReport = new RecipeReport();


            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            NumericReportService numericReportService = new NumericReportService(connectionString);

            BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);

            if (batchNumericReportModel.IntegratedCheckReportItems == null)
                return null;

            DataTable integratedCheckResultsDataTable = new DataTable();

            DataConverter dataConverter = new DataConverter();

            integratedCheckResultsDataTable = dataConverter.ConvertToDataTable(batchNumericReportModel.IntegratedCheckReportItems);

            integrityCheckReport.xrLabelRecipeName.Text = batchNumericReportModel.NumericReportHeaderInfo.RecipeName;
            integrityCheckReport.xrLabelLoadNumber.Text = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
            integrityCheckReport.xrLabelStartDate.Text = batchNumericReportModel.NumericReportHeaderInfo.StartDate.ToString();
            integrityCheckReport.xrLabelEndDate.Text = batchNumericReportModel.NumericReportHeaderInfo.EndDate.ToString();
            integrityCheckReport.xrLabelReportDate.Text = DateTime.Now.ToString();

            if (integratedCheckResultsDataTable == null)
            {

                InitRecipeReportBands(integrityCheckReport);
                XRLabel xrLabelSkipDate = new XRLabel();
                xrLabelSkipDate.WidthF = 300;
                xrLabelSkipDate.LeftF = 100;
                xrLabelSkipDate.AutoWidth = true;
                xrLabelSkipDate.KeepTogether = true;
                xrLabelSkipDate.Text = $"Skip Date : {batchNumericReportModel.SkippedIntegratedCheckReportItem.SkipDate.ToString()}";
                integrityCheckReport.Bands[BandKind.PageHeader].Controls.Add(xrLabelSkipDate);

                xtraReport = integrityCheckReport;
                return xtraReport;
            }

            integratedCheckResultsDataTable.TableName = "IntegrityCheck";
            integrityCheckReport.DataSource = integratedCheckResultsDataTable;
            integrityCheckReport.DataMember = "IntegrityCheck";

            DataSet integrityCheckReportDataSet = new DataSet();
            integrityCheckReportDataSet.Tables.Add(integratedCheckResultsDataTable);
            integrityCheckReport.DataSource = integrityCheckReportDataSet;
            integrityCheckReport.DataMember = "IntegrityCheck";

            InitRecipeReportBands(integrityCheckReport);

            InitRecipeReportDetailsBasedonXRTable(integrityCheckReport);


            xtraReport = integrityCheckReport;

            return xtraReport;
        }

        public XtraReport IntegrityCheckReport(List<IntegrityChecksItemsTableRow> integrityChecksItemsTableRow, string loadNumber, string recipeName)
        {
            if (integrityChecksItemsTableRow == null)
                return null;

            XtraReport xtraReport = null;
            IntegrityCheckReport integrityCheckReport = new IntegrityCheckReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            NumericReportService numericReportService = new NumericReportService(connectionString);

            DataTable integratedCheckResultsDataTable = new DataTable();

            DataConverter dataConverter = new DataConverter();

            integratedCheckResultsDataTable = dataConverter.ConvertToDataTable(integrityChecksItemsTableRow);

            integrityCheckReport.xrLabelRecipeName.Text = recipeName;
            integrityCheckReport.xrLabelLoadNumber.Text = loadNumber;

            integrityCheckReport.xrLabelReportDate.Text = DateTime.Now.ToString();

            if (integratedCheckResultsDataTable == null)
            {
                InitRecipeReportBands(integrityCheckReport);
                XRLabel xrLabelSkipDate = new XRLabel();
                xrLabelSkipDate.WidthF = 300;
                xrLabelSkipDate.LeftF = 100;
                xrLabelSkipDate.AutoWidth = true;
                xrLabelSkipDate.KeepTogether = true;
                xrLabelSkipDate.Text = $"Is Skiped!";
                integrityCheckReport.Bands[BandKind.PageHeader].Controls.Add(xrLabelSkipDate);

                xtraReport = integrityCheckReport;
                return xtraReport;
            }
            else
            {
                integratedCheckResultsDataTable.Columns.Remove("BagId");
                integratedCheckResultsDataTable.Columns.Remove("BatchId");
                integratedCheckResultsDataTable.Columns.Remove("IsItSelected");
            }

            integratedCheckResultsDataTable.TableName = "IntegrityCheck";
            integrityCheckReport.DataSource = integratedCheckResultsDataTable;
            integrityCheckReport.DataMember = "IntegrityCheck";

            DataSet integrityCheckReportDataSet = new DataSet();
            integrityCheckReportDataSet.Tables.Add(integratedCheckResultsDataTable);
            integrityCheckReport.DataSource = integrityCheckReportDataSet;
            integrityCheckReport.DataMember = "IntegrityCheck";

            InitRecipeReportBands(integrityCheckReport);

            InitRecipeReportDetailsBasedonXRTable(integrityCheckReport);


            xtraReport = integrityCheckReport;

            return xtraReport;
        }


        public XtraReport CalibrationReport(List<CalibrationDataGrid> calibrationDataGrid)
        {
            if (calibrationDataGrid == null)
                return null;

            XtraReport xtraReport = null;
            CalibrationReport calibrationReport = new CalibrationReport();

            DataConverter dataConverter = new DataConverter();

            DataTable calibrationDataTable = dataConverter.ConvertToDataTable(calibrationDataGrid);
            calibrationDataTable.TableName = "Calibration";
            calibrationReport.DataSource = calibrationDataTable;
            calibrationReport.DataMember = "Calibration";
            calibrationReport.xrTableCellSensorType.DataBindings.Add("Text", null, "calibrationDataGrid.SensorTypeLiteral");
            calibrationReport.xrTableCellSensorName.DataBindings.Add("Text", null, "calibrationDataGrid.Sensor");
            calibrationReport.xrTableCellOldOffset.DataBindings.Add("Text", null, "calibrationDataGrid.OldCallOffset");
            calibrationReport.xrTableCellNewOffset.DataBindings.Add("Text", null, "calibrationDataGrid.NewCallOffset");
            calibrationReport.xrTableCellOldGain.DataBindings.Add("Text", null, "calibrationDataGrid.OldGain");
            calibrationReport.xrTableCellNewGain.DataBindings.Add("Text", null, "calibrationDataGrid.NewGain");
            calibrationReport.xrTableCellSensorValue.DataBindings.Add("Text", null, "calibrationDataGrid.SensorValue");
            calibrationReport.xrTableCellRawValue.DataBindings.Add("Text", null, "calibrationDataGrid.SensorRawValue");



            xtraReport = calibrationReport;

            return xtraReport;
        }

        public XtraReport BatchReport()
        {
            XtraReport xtraReport = null;
            BatchReport batchReport = new BatchReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            BatchReportService batchReportService = new BatchReportService(connectionString);
            BatchReportModel batchReportModel = batchReportService.BatchReport(_batchId);

            if (batchReportModel.BagSensorAndPartDetails == null)
                return null;

            DataTable BagSensorAndPartDetailDataTable = new DataTable();
            DataConverter dataConverter = new DataConverter();

            BagSensorAndPartDetailDataTable = dataConverter.ConvertToDataTable(batchReportModel.BagSensorAndPartDetails);
            BagSensorAndPartDetailDataTable.TableName = "Batch";
            batchReport.DataSource = BagSensorAndPartDetailDataTable;
            batchReport.DataMember = "Batch";

            batchReport.xrLabelRecipeName.Text = batchReportModel.ReportHeaderInfo.RecipeName;
            batchReport.xrLabelLoadNumber.Text = batchReportModel.ReportHeaderInfo.LoadNumber;
            batchReport.xrLabelStartDate.Text = batchReportModel.ReportHeaderInfo.StartDate.ToString();
            batchReport.xrLabelEndDate.Text = batchReportModel.ReportHeaderInfo.EndDate.ToString();
            batchReport.xrLabelReportDate.Text = DateTime.Now.ToString();

            batchReport.xrTableCellBagName.DataBindings.Add("Text", null, "BagName");
            batchReport.xrTableCellPartTc.DataBindings.Add("Text", null, "BagSensorAndPartDetails.BagSensors.AllPTCs");
            batchReport.xrTableCellMonitor.DataBindings.Add("Text", null, "BagSensorAndPartDetails.BagSensors.AllMONs");
            batchReport.xrTableCellSrc.DataBindings.Add("Text", null, "BagSensorAndPartDetails.BagSensors.AllVACs");
            batchReport.xrTableCellSoirName.DataBindings.Add("Text", null, "BagSensorAndPartDetails.LotProperties.SoirNumber");
            batchReport.xrTableCellToolName.DataBindings.Add("Text", null, "BagSensorAndPartDetails.LotProperties.ToolName");
            batchReport.xrTableCellPartName.DataBindings.Add("Text", null, "BagSensorAndPartDetails.LotProperties.PartName");

            xtraReport = batchReport;

            return xtraReport;
        }

        public XtraReport BatchReportByEnterParts(int batchId)
        {
            XtraReport xtraReport = null;
            BatchReport batchReport = new BatchReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            BatchReportService batchReportService = new BatchReportService(connectionString);
            BatchReportModel batchReportModel = batchReportService.BatchReport(batchId);

            if (batchReportModel.BagSensorAndPartDetails == null)
                return null;

            DataTable BagSensorAndPartDetailDataTable = new DataTable();
            DataConverter dataConverter = new DataConverter();

            BagSensorAndPartDetailDataTable = dataConverter.ConvertToDataTable(batchReportModel.BagSensorAndPartDetails);
            BagSensorAndPartDetailDataTable.TableName = "Batch";
            batchReport.DataSource = BagSensorAndPartDetailDataTable;
            batchReport.DataMember = "Batch";

            batchReport.xrLabelRecipeName.Text = batchReportModel.ReportHeaderInfo == null ? "" : batchReportModel.ReportHeaderInfo.RecipeName;
            batchReport.xrLabelLoadNumber.Text = batchReportModel.ReportHeaderInfo == null ? "" : batchReportModel.ReportHeaderInfo.LoadNumber;
            batchReport.xrLabelStartDate.Text = batchReportModel.ReportHeaderInfo == null ? "" : batchReportModel.ReportHeaderInfo.StartDate.ToString();
            batchReport.xrLabelEndDate.Text = batchReportModel.ReportHeaderInfo == null ? "" : batchReportModel.ReportHeaderInfo.EndDate.ToString();
            batchReport.xrLabelReportDate.Text = DateTime.Now.ToString();

            batchReport.xrTableCellBagName.DataBindings.Add("Text", null, "BagName");
            batchReport.xrTableCellPartTc.DataBindings.Add("Text", null, "BagSensorAndPartDetails.BagSensors.PTCs");
            batchReport.xrTableCellMonitor.DataBindings.Add("Text", null, "BagSensorAndPartDetails.BagSensors.MONs");
            batchReport.xrTableCellSrc.DataBindings.Add("Text", null, "BagSensorAndPartDetails.BagSensors.VACs");
            batchReport.xrTableCellSoirName.DataBindings.Add("Text", null, "BagSensorAndPartDetails.LotProperties.SoirNumber");
            batchReport.xrTableCellToolName.DataBindings.Add("Text", null, "BagSensorAndPartDetails.LotProperties.ToolName");
            batchReport.xrTableCellPartName.DataBindings.Add("Text", null, "BagSensorAndPartDetails.LotProperties.PartName");

            xtraReport = batchReport;

            return xtraReport;
        }

        public XtraReport RecipeReport()
        {
            XtraReport xtraReport = null;

            RecipeReport recipeReport = new RecipeReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            RecipeReportService recipeReportService = new RecipeReportService(connectionString);
            UserService userService = new UserService(connectionString);

            DataTable recipeReportModel = recipeReportService.RecipeReport(_batchId);

            if (recipeReportModel == null)
                return null;

            recipeReportModel.Columns.Remove("Order");
            recipeReportModel.Columns.Remove("Unit");

            ReportHeaderInfo reportHeaderInfo = recipeReportService.ReportHeaderInfo(_batchId);
            recipeReport.lblrecname.Text = reportHeaderInfo.RecipeName;
            recipeReport.lblrecdesc.Text = reportHeaderInfo.RecipeDescription;
            recipeReport.lblrecStartDate.Text = reportHeaderInfo.StartDate.ToString();
            recipeReport.lblrecEndDate.Text = reportHeaderInfo.EndDate.ToString();
            recipeReport.lblrecReportDate.Text = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString();
            recipeReport.xrLblCreatedByUser.Text = userService.GetById(reportHeaderInfo.CreatedByUserId).UserName;

            DataSet recipeReportDataSet = new DataSet();
            recipeReportDataSet.Tables.Add(recipeReportModel);
            recipeReport.DataSource = recipeReportDataSet;
            recipeReport.DataMember = "RecipeReportTable";

            InitRecipeReportBands(recipeReport);

            InitRecipeReportDetailsBasedonXRTable(recipeReport);

            xtraReport = recipeReport;

            return xtraReport;

        }

        /// <summary>
        /// Report for recipe editor.
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        public XtraReport RecipeReportByRecipe(int recipeId)
        {
            XtraReport xtraReport = null;

            RecipeReport recipeReport = new RecipeReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            RecipeReportService recipeReportService = new RecipeReportService(connectionString);
            RecipeService recipeService = new RecipeService(connectionString);
            UserService userService = new UserService(connectionString);
            var recipeInfo = recipeService.GetById(recipeId);

            DataTable recipeReportModel = recipeReportService.RecipeReportByRecipe(recipeId);
           
            


            if (recipeReportModel == null)
                return null;

            recipeReportModel.Columns.Remove("Order");
            recipeReportModel.Columns.Remove("Unit");

            recipeReport.lblrecname.Text = recipeInfo.RecipeName;
            recipeReport.xrLblCreatedByUser.Text = userService.GetById(recipeInfo.CreatedByUserId).UserName;
          

            //recipeReport.lblrecdesc.Text = reportHeaderInfo.RecipeDescription;
            //recipeReport.lblrecStartDate.Text = reportHeaderInfo.StartDate.ToString();
            //recipeReport.lblrecEndDate.Text = reportHeaderInfo.EndDate.ToString();
            recipeReport.lblrecReportDate.Text = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString();

            DataSet recipeReportDataSet = new DataSet();
            recipeReportDataSet.Tables.Add(recipeReportModel);
            recipeReport.DataSource = recipeReportDataSet;
            recipeReport.DataMember = "RecipeReportTable";

            InitRecipeReportBands(recipeReport);

            InitRecipeReportDetailsBasedonXRTable(recipeReport);

            xtraReport = recipeReport;

            return xtraReport;

        }


        //todo:l refactor
        public XtraReport NumericReport()
        {
            XtraReport xtraReport = null;
            NumericReport numericReport = new NumericReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            NumericReportService numericReportService = new NumericReportService(connectionString);

            BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);

            if(batchNumericReportModel.NumericDataTable == null)
                return null;

            DataSet numericReportDataSet = new DataSet();
            numericReportDataSet.Tables.Add(batchNumericReportModel.NumericDataTable);

            numericReport.DataSource = numericReportDataSet;
            numericReport.DataMember = "BatchNumericReportTable";

            numericReport.xrLabelLoadNumber.Text = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
            numericReport.xrLabelRecipeDescription.Text = batchNumericReportModel.NumericReportHeaderInfo.RecipeDescription;
            numericReport.xrLabelReportDate.Text = DateTime.Now.ToString();
            numericReport.xrLabelRunStart.Text = batchNumericReportModel.NumericReportHeaderInfo.StartDate.ToString();
            numericReport.xrLabelEndRun.Text = batchNumericReportModel.NumericReportHeaderInfo.EndDate.ToString();


            XRTable tableHeader, tableDetail;

            NumericReportCreateTable(out tableHeader, out tableDetail, numericReport);

            NumericReportAddTable(tableHeader, tableDetail, numericReport);

            xtraReport = numericReport;

            return xtraReport;
        }

        public XtraReport TrendReport()
        {
            XtraReport xtraReport = null;
            TrendReport trendReport = new TrendReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            NumericReportService numericReportService = new NumericReportService(connectionString);

            BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);

            if (batchNumericReportModel.NumericDataTable == null)
                return null;

            DataSet trendReportDataSet = new DataSet();
            trendReportDataSet.Tables.Add(batchNumericReportModel.NumericDataTable);

            trendReport.xrChartTrendReport.Series.Clear();

            trendReport.xrChartTrendReport.DataSource = trendReportDataSet;
            //trendReport.DataMember = "BatchNumericReportTable";

            trendReport.xrLabelLoadNumber.Text = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
            trendReport.xrLabelRecipeDescription.Text = batchNumericReportModel.NumericReportHeaderInfo.RecipeDescription;
            trendReport.xrLabelReportDate.Text = DateTime.Now.ToString();
            trendReport.xrLabelRunStart.Text = batchNumericReportModel.NumericReportHeaderInfo.StartDate.ToString();
            trendReport.xrLabelEndRun.Text = batchNumericReportModel.NumericReportHeaderInfo.EndDate.ToString();


            TrendReportCreateChartSeries(trendReportDataSet, trendReport);

            xtraReport = trendReport;

            xtraReport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            xtraReport.Landscape = true;

            return xtraReport;
        }


        private void TrendReportCreateChartSeries(DataSet dataSet, TrendReport trendReport)
        {
            try
            {
                for (int i = 0; i < dataSet.Tables[0].Columns.Count; i++)
                {
                    string colName = dataSet.Tables[0].Columns[i].Caption;


                    Series series = new Series(colName, ViewType.Line);
                    series.ArgumentDataMember = "Time";
                    series.ValueDataMembers.AddRange(new string[] { colName });
                    ((LineSeriesView)series.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.False;
                    series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
                    trendReport.xrChartTrendReport.Series.Add(series);
                    series.CheckedInLegend = true;



                    XYDiagram diagram = (XYDiagram)trendReport.xrChartTrendReport.Diagram;
                    diagram.EnableAxisXZooming = true;
                    diagram.EnableAxisXScrolling = true;
                    diagram.ScrollingOptions.UseMouse = true;

                    diagram.AxisX.Label.TextPattern = "{A:dd.MM.yyyy \n HH:mm:ss}";
                    diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Millisecond;


                    //trendReport.xrChartTrendReport.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
                    //trendReport.xrChartTrendReport.Legend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
                    ////trendReport.xrChartTrendReport.Legend.DockTarget = diagram.DefaultPane;
                    //trendReport.xrChartTrendReport.Legend.MaxCrosshairContentWidth = 250;

                    Legend legend = trendReport.xrChartTrendReport.Legend;


                    legend.Visibility = DevExpress.Utils.DefaultBoolean.False;


                    legend.Margins.All = 8;
                    legend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
                    legend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
                    legend.MaxCrosshairContentWidth = 250;
                    legend.Direction = LegendDirection.LeftToRight;
                    legend.EquallySpacedItems = true;
                    legend.HorizontalIndent = 100;
                    legend.VerticalIndent = 100;
                    legend.TextVisible = true;
                    legend.TextOffset = 100;
                    legend.MarkerVisible = true;
                    legend.MarkerSize = new Size(10, 10);
                    legend.Padding.All = 4;

                    legend.MaxHorizontalPercentage = 50;
                    legend.MaxVerticalPercentage = 50;

                    legend.BackColor = Color.Beige;
                    legend.FillStyle.FillMode = FillMode.Gradient;
                    ((RectangleGradientFillOptions)legend.FillStyle.Options).Color2 = Color.Bisque;

                    legend.Border.Visibility = DevExpress.Utils.DefaultBoolean.True;
                    legend.Border.Color = Color.DarkBlue;
                    legend.Border.Thickness = 2;

                    legend.Shadow.Visible = true;
                    legend.Shadow.Color = Color.LightGray;
                    legend.Shadow.Size = 2;
                    legend.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
                    legend.Font = new Font("Arial", 9, FontStyle.Bold);
                }
            }
            catch
            {

            }

        }


        //todo:l refactor
        private void NumericReportCreateTable(out XRTable tableHeader, out XRTable tableDetail, NumericReport numericReport)
        {
            // ExportLogQryReports.logDataTable();

            DataSet ds = ((DataSet)numericReport.DataSource);

            int colCount = ds.Tables[0].Columns.Count;
            int colWidth = (numericReport.PageWidth - (numericReport.Margins.Left + numericReport.Margins.Right)) / 11;
            //int colWidth = (report.PageWidth - (100 + 100)) / colCount;

            tableHeader = new XRTable();
            tableHeader.BeginInit();

            tableHeader.LocationF = new PointF(0F, 30);
            tableHeader.Height = 20;
            tableHeader.WidthF = (numericReport.PageWidth - numericReport.Margins.Left - numericReport.Margins.Right);

            XRTableRow headerRow = new XRTableRow();
            // headerRow.WidthF = tableHeader.WidthF;
            // headerRow.BackColor = Color.FromArgb(255, 222, 121);


            for (int i = 0; i < colCount; i++)
            {

                XRTableCell headerCell = new XRTableCell();
                headerCell.Font = new Font("Tahoma", 7F, FontStyle.Bold);
                headerCell.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
                headerCell.WidthF = tableHeader.WidthF / 11;
                headerCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                headerCell.Text = ds.Tables[0].Columns[i].Caption;
                headerRow.Cells.Add(headerCell);
            }
            tableHeader.Rows.Add(headerRow);

            tableHeader.AdjustSize();
            tableHeader.PerformLayout();
            tableHeader.EndInit();

            tableDetail = new XRTable();
            tableDetail.BeginInit();
            tableDetail.Width = (numericReport.PageWidth - (numericReport.Margins.Left + numericReport.Margins.Right));


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                XRTableRow detailRow = new XRTableRow();
                //  detailRow.Width = tableDetail.Width;

                tableDetail.EvenStyleName = "EvenStyle";
                tableDetail.OddStyleName = "OddStyle";


                for (int i = 0; i < colCount; i++)
                {
                    XRTableCell detailCell = new XRTableCell();
                    detailCell.Font = new System.Drawing.Font("Tahoma", 7F, FontStyle.Regular);

                    detailCell.WidthF = tableDetail.WidthF / 11;
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

                    double val = 0;
                    if (double.TryParse(row[i].ToString(), out val))
                        detailCell.Text = String.Format("{0:0.00}", val);
                    else
                        detailCell.Text = row[i].ToString();

                    detailRow.Cells.Add(detailCell);
                }
                tableDetail.Rows.Add(detailRow);

            }

            tableDetail.AdjustSize();
            tableDetail.PerformLayout();
            tableDetail.EndInit();
        }


        //todo:l refactor
        private void NumericReportAddTable(XRTable tableHeader, XRTable tableDetail, XtraReport xtraReport)
        {
            int level = 0;

            DetailReportBand DetailReportBand1 = new DevExpress.XtraReports.UI.DetailReportBand();
            DetailReportBand1.Level = level;

            GroupHeaderBand GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            tableHeader});
            GroupHeader1.Height = 20;

            GroupHeader1.Name = "GroupHeader" + level.ToString();
            GroupHeader1.RepeatEveryPage = true;

            DetailBand Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            Detail1.Height = 20;

            Detail1.PageBreak = PageBreak.AfterBand;
            Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            tableDetail});

            DetailReportBand1.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            GroupHeader1,
               Detail1});

            xtraReport.Bands.Add(DetailReportBand1);
        }



        private void InitRecipeReportBands(XtraReport rep)
        {
            DetailBand detail = new DetailBand();
            PageHeaderBand pageHeader = new PageHeaderBand();
            ReportFooterBand reportFooter = new ReportFooterBand();
            detail.Height = 20;
            reportFooter.Height = 380;
            pageHeader.Height = 20;


            rep.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] { detail, pageHeader, reportFooter });
        }

        private void InitRecipeReportDetailsBasedonXRTable(XtraReport rep)
        {
            try
            {

                DataSet ds = ((DataSet)rep.DataSource);
                int colCount = ds.Tables[0].Columns.Count;
                int colWidth = (rep.PageWidth - (rep.Margins.Left + rep.Margins.Right)) / colCount;

                XRTable tableHeader = new XRTable();
                tableHeader.Height = 20;
                tableHeader.Width = (rep.PageWidth - (rep.Margins.Left + rep.Margins.Right));
                tableHeader.Font = new Font(tableHeader.Font, FontStyle.Bold);
                XRTableRow headerRow = new XRTableRow();
                headerRow.Width = tableHeader.Width;
                tableHeader.Rows.Add(headerRow);

                tableHeader.BeginInit();
                XRTable tableDetail = new XRTable();
                tableDetail.Height = 20;
                tableDetail.Width = (rep.PageWidth - (rep.Margins.Left + rep.Margins.Right));
                XRTableRow detailRow = new XRTableRow();
                detailRow.Width = tableDetail.Width;
                tableDetail.Rows.Add(detailRow);
                tableDetail.EvenStyleName = "EvenStyle";
                tableDetail.OddStyleName = "OddStyle";

                tableDetail.BeginInit();

                for (int i = 0; i < colCount; i++)
                {
                    XRTableCell headerCell = new XRTableCell();
                    headerCell.Width = colWidth;
                    if (ds.Tables[0].Columns[i].Caption == "SelectedSensorName")
                    {
                        headerCell.Text = "Sensor Name";
                    }
                    else { headerCell.Text = ds.Tables[0].Columns[i].Caption; }
                    XRTableCell detailCell = new XRTableCell();
                    detailCell.DataBindings.Add("Text", null, ds.Tables[0].Columns[i].Caption);

                    headerRow.Cells.Add(headerCell);
                    detailRow.Cells.Add(detailCell);
                }
                tableHeader.EndInit();
                tableDetail.EndInit();

                rep.Bands[BandKind.PageHeader].Controls.Add(tableHeader);
                rep.Bands[BandKind.Detail].Controls.Add(tableDetail);
            }
            catch (Exception ex)
            {
            }

        }


        int indexNo = 0;
        public XtraReport NumericBagReport()
        {

            XtraReport xtraReport = null;
            NumericBagReport numericBagReport = new NumericBagReport();

            NumericReportService numericReportService = new NumericReportService(_connectionString);

            BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);

            if (batchNumericReportModel.NumericDataTable == null)
                return null;

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(batchNumericReportModel.NumericDataTable);

            numericBagReport.DataSource = dataSet;

            numericBagReport.DataMember = "BatchNumericReportTable";


            IEnumerable<Bag> bags = batchNumericReportModel.Bags;

            foreach (var bagItem in bags)
            {
              //  var cc = numericReportService.NumericReportByBag(batchId:_batchId, bagId: bagItem.id, 100000000, 1);


                numericBagReport.xrlblReportDate.Text = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString();
                // report.xrLabel4.Text = bagadi[indexno].ToString();

                XRTable tableHeader, tableDetail;

                NumericBagCreateReportTable(out tableHeader, out tableDetail, numericBagReport);


                NumericBagAddTable(tableHeader, tableDetail, numericBagReport, bagItem.BagName);

                level++;
                indexNo++;
            }

            xtraReport = numericBagReport;
            return xtraReport;
        }

        private void NumericBagCreateReportTable(out XRTable tableHeader, out XRTable tableDetail, NumericBagReport numericBagReport)
        {
            tableHeader = new XRTable();
            tableDetail = new XRTable();

            try
            {
                int colCount = ((DataSet)numericBagReport.DataSource).Tables[0].Columns.Count;   // dataSet1.Tables[tableName].Columns.Count;
                int colWidth = (numericBagReport.PageWidth - (numericBagReport.Margins.Left + numericBagReport.Margins.Right)) / 10;
                //int colWidth = (report.PageWidth - (100 + 100)) / colCount;

                tableHeader.BeginInit();

                tableHeader.LocationF = new PointF(0F, 30);
                tableHeader.Height = 20;
                tableHeader.WidthF = (numericBagReport.PageWidth - numericBagReport.Margins.Left - numericBagReport.Margins.Right);

                XRTableRow headerRow = new XRTableRow();
                // headerRow.WidthF = tableHeader.WidthF;
                // headerRow.BackColor = Color.FromArgb(255, 222, 121);


                for (int i = 0; i < colCount; i++)
                {

                    XRTableCell headerCell = new XRTableCell();
                    headerCell.Font = new System.Drawing.Font("Tahoma", 7F, FontStyle.Bold);
                    headerCell.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
                    headerCell.WidthF = tableHeader.WidthF / 10;
                    headerCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                    headerCell.Text = ((DataSet)numericBagReport.DataSource).Tables[0].Columns[i].Caption;
                    headerRow.Cells.Add(headerCell);
                }
                tableHeader.Rows.Add(headerRow);

                tableHeader.AdjustSize();
                tableHeader.PerformLayout();
                tableHeader.EndInit();


                tableDetail.BeginInit();
                tableDetail.Width = (numericBagReport.PageWidth - (numericBagReport.Margins.Left + numericBagReport.Margins.Right));


                foreach (DataRow row in ((DataSet)numericBagReport.DataSource).Tables[0].Rows)
                {
                    XRTableRow detailRow = new XRTableRow();
                    //  detailRow.Width = tableDetail.Width;

                    tableDetail.EvenStyleName = "EvenStyle";
                    tableDetail.OddStyleName = "OddStyle";


                    for (int i = 0; i < colCount; i++)
                    {
                        XRTableCell detailCell = new XRTableCell();
                        detailCell.Font = new System.Drawing.Font("Tahoma", 7F, FontStyle.Bold);
                        detailCell.WidthF = tableDetail.WidthF / 10;
                        detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        detailCell.Text = row[i].ToString();
                        detailRow.Cells.Add(detailCell);
                    }
                    tableDetail.Rows.Add(detailRow);

                }

                tableDetail.AdjustSize();
                tableDetail.PerformLayout();
                tableDetail.EndInit();
            }
            catch
            {

            }

        }

        static int level = 0;
        private void NumericBagAddTable(XRTable tableHeader, XRTable tableDetail, NumericBagReport numericBagReport, string bagName)
        {
            DetailReportBand DetailReportBand1 = new DevExpress.XtraReports.UI.DetailReportBand();
            DetailReportBand1.Level = level;

            XRLabel label = new XRLabel();
            label.Text = bagName;
            numericBagReport.xrLabelBagName.Text = bagName;
            GroupHeaderBand GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            tableHeader,label});
            GroupHeader1.Height = 20;

            GroupHeader1.Name = "GroupHeader" + level.ToString();
            GroupHeader1.RepeatEveryPage = true;

            DetailBand Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            Detail1.Height = 20;

            Detail1.PageBreak = PageBreak.AfterBand;
            Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            tableDetail});

            DetailReportBand1.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            GroupHeader1,
               Detail1});

            numericBagReport.Bands.Add(DetailReportBand1);

        }

        QualityReport _qualityReport;


        public XtraReport QualityBatchReport(int qualityCardId)
        {

            QualityBatchReportCreator qualityBatchReportCreator = new QualityBatchReportCreator();

            XtraReport xtraReport = null;

            level = 0;

            _qualityReport = new QualityReport();

            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            QualityReportService qualityReportService = new QualityReportService(_connectionString);

            BatchNumericReportModel batchNumericReportModel = qualityReportService.BatchNumericReport(_batchId);

            BatchQualityDetailService batchQualityDetailService = new BatchQualityDetailService(_connectionString);

            #region public."BatchQualityDetails"

            if (batchNumericReportModel.NumericDataTable != null)
            {
                var selectedQualityCardPhases = batchQualityDetailService.GetAllByQualityBatchId(qualityCardId).OrderBy(x => x.SortOrder);

                int i = 1;
                int startTime = 0;
                int endTime = 0;
                int colCount = 2;
                int diffTime = 0;

                foreach (var item in selectedQualityCardPhases)
                {
                    bool isError = false;
                    string HeaderReport = item.PhaseName;
                    ///DataRow[] getTimesDataRowqq = batchNumericReportModel.NumericDataTable.Select("Mins >= 7 AND 176 <=  Mins ");

                    int currentDurationTimes = qualityBatchReportCreator.getTimes(item.PhaseChange, item.PhaseCriteria, item.PhaseCriteriaValue, endTime, batchNumericReportModel.NumericDataTable);

                    if (currentDurationTimes >= endTime)
                        diffTime = currentDurationTimes - endTime;
                    else if (currentDurationTimes == -1)
                    {
                        isError = true;
                        diffTime = 0;
                    }
                    else
                        diffTime = 0;

                    string resultHeader = "";

                    endTime += diffTime;
                    resultHeader = "Phase #" + i + ":" + HeaderReport + ", Start:" + startTime + " Mins., End:" + endTime + " Mins., Duration:" + diffTime + " Mins."
                         + " Change Criteria " + item.PhaseChange + item.PhaseCriteria + item.PhaseCriteriaValue
                        ;
                    XRTable tableHeader = new XRTable();
                    XRTable tableDetail = new XRTable();
                    i++;

                    createReportTable(out tableHeader, out tableDetail);
                    if (isError)
                    {
                        string[,] rowData = new string[1, 2];
                        string sectionName = "There is no suitable data for phase change criteria.";
                        addReportTableRows(tableHeader.WidthF, tableDetail, rowData, colCount, sectionName);
                    }

                    if (!isError && Convert.ToBoolean(item.PhaseStyle)) //2 satır Time Range
                    {
                        try
                        {
                            string[,] rowData = new string[2, 2];
                            string warningMins = "";
                            string warningGeneral = "***";
                            if (Convert.ToInt16(diffTime) < Convert.ToDouble(item.PhaseMinTime))
                                warningMins = warningGeneral;
                            if (Convert.ToInt16(diffTime) > Convert.ToDouble(item.PhaseMaxTime))
                                warningMins = warningGeneral;

                            rowData[0, 0] = $"{warningMins} Phase Time = {diffTime} mins";
                            rowData[0, 1] = $"(Req {item.PhaseMinTime} between {item.PhaseMaxTime} mins";

                            string sectionName = item.PhaseTitle;
                            addReportTableRows(tableHeader.WidthF, tableDetail, rowData, colCount, sectionName);
                        }
                        catch
                        {

                        }
                    }

                    if (!isError && Convert.ToBoolean(item.ProbeStyle)) //4 satır Vacuum Control
                    {
                        try
                        {
                            string[,] rowData = new string[5, 2];
                            string[] vacStartMaxValue = qualityBatchReportCreator.getMaxMonValue(startTime, batchNumericReportModel.NumericDataTable);/*.Except*/
                            string[] vacStartMinValue = qualityBatchReportCreator.getMinMonValue(startTime, batchNumericReportModel.NumericDataTable);
                            string[] vacEndMaxValue = qualityBatchReportCreator.getMaxMonValue(endTime, batchNumericReportModel.NumericDataTable);
                            string[] vacEndMinValue = qualityBatchReportCreator.getMinMonValue(endTime, batchNumericReportModel.NumericDataTable);

                            #region pressure style
                            string[] pressureStartMaxValue = qualityBatchReportCreator.getMaxPressureValue(startTime, batchNumericReportModel.NumericDataTable);/*.Except*/
                            string[] pressureStartMinValue = qualityBatchReportCreator.getMinPressureValue(startTime, batchNumericReportModel.NumericDataTable);
                            string[] pressureEndMaxValue = qualityBatchReportCreator.getMaxPressureValue(endTime, batchNumericReportModel.NumericDataTable);
                            string[] pressureEndMinValue = qualityBatchReportCreator.getMinPressureValue(endTime, batchNumericReportModel.NumericDataTable);

                            string[] pressureMaxDuring = qualityBatchReportCreator.getMaxPressureDuringValue(startTime, endTime, batchNumericReportModel.NumericDataTable);
                            string[] pressureMinDuring = qualityBatchReportCreator.getMinPressureDuringValue(startTime, endTime, batchNumericReportModel.NumericDataTable);
                            #endregion pressure style


                            string warningStartMax = "";
                            string warningStartMin = "";
                            string warningEndMax = "";
                            string warningEndMin = "";
                            string warningGeneral = "***";
                            if (Convert.ToInt16(item.ProbePhaseStartMax) < Convert.ToDouble(vacStartMaxValue[0]))
                                warningStartMax = warningGeneral;
                            if (Convert.ToInt16(item.ProbePhaseStartMin) > Convert.ToDouble(vacStartMinValue[0]))
                                warningStartMin = warningGeneral;
                            if (Convert.ToInt16(item.ProbePhaseEndMax) < Convert.ToDouble(vacEndMaxValue[0]))
                                warningEndMax = warningGeneral;
                            if (Convert.ToInt16(item.ProbePhaseEndMin) > Convert.ToDouble(vacEndMinValue[0]))
                                warningEndMin = warningGeneral;


                            rowData[0, 0] = $"{warningStartMax} Max Vac at Phase Start = {vacStartMaxValue[0]} inHg From {vacStartMaxValue[1]}  at {startTime.ToString()} mins";
                            rowData[0, 1] = $"(Req < {item.ProbePhaseStartMax} inHg)";
                            rowData[1, 0] = $"{warningStartMin} Min Vac at Phase Start = {vacStartMinValue[0]} inHg From {vacStartMinValue[1]}  at {startTime.ToString()} mins";
                            rowData[1, 1] = $"(Req > {item.ProbePhaseStartMin} inHg";
                            rowData[2, 0] = $"{warningEndMax} Max Vac at Phase End = { vacEndMaxValue[0]} inHg From {vacEndMaxValue[1]} at { endTime.ToString()} mins";
                            rowData[2, 1] = $"(Req < { item.ProbePhaseEndMax} inHg)";
                            rowData[3, 0] = $"{warningEndMin} Min Vac at Phase End = {vacEndMinValue[0]} inHg From {vacEndMinValue[1]} at {endTime.ToString()} mins";
                            rowData[3, 1] = $"(Req > {item.ProbePhaseEndMin} inHg)";

                            string sectionName = item.ProbeTitle;
                            addReportTableRows(tableHeader.WidthF, tableDetail, rowData, colCount, sectionName);
                        }
                        catch
                        {

                        }
                    }

                    if (!isError && Convert.ToBoolean(item.PressureStyle)) // Pressure Control
                    {
                        try
                        {
                            string[,] rowData = new string[7, 2];
                            string[] pressureStartMaxValue = qualityBatchReportCreator.getMaxPressureValue(startTime, batchNumericReportModel.NumericDataTable);/*.Except*/
                            string[] pressureStartMinValue = qualityBatchReportCreator.getMinPressureValue(startTime, batchNumericReportModel.NumericDataTable);
                            string[] pressureEndMaxValue = qualityBatchReportCreator.getMaxPressureValue(endTime, batchNumericReportModel.NumericDataTable);
                            string[] pressureEndMinValue = qualityBatchReportCreator.getMinPressureValue(endTime, batchNumericReportModel.NumericDataTable);

                            string[] pressureMaxDuring = qualityBatchReportCreator.getMaxPressureDuringValue(startTime, endTime, batchNumericReportModel.NumericDataTable);
                            string[] pressureMinDuring = qualityBatchReportCreator.getMinPressureDuringValue(startTime, endTime, batchNumericReportModel.NumericDataTable);

                            string warningStartMax = "";
                            string warningStartMin = "";
                            string warningEndMax = "";
                            string warningEndMin = "";
                            string warningGeneral = "***";
                            if (Convert.ToInt16(item.PressurePhaseStartMax) < Convert.ToDouble(pressureStartMaxValue[0]))
                                warningStartMax = warningGeneral;
                            if (Convert.ToInt16(item.PressurePhaseStartMin) > Convert.ToDouble(pressureStartMinValue[0]))
                                warningStartMin = warningGeneral;
                            if (Convert.ToInt16(item.PressurePhaseEndMax) < Convert.ToDouble(pressureEndMaxValue[0]))
                                warningEndMax = warningGeneral;
                            if (Convert.ToInt16(item.PressurePhaseEndMin) > Convert.ToDouble(pressureEndMinValue[0]))
                                warningEndMin = warningGeneral;
                            if (Convert.ToInt16(item.PressurePhaseStartMax) < Convert.ToDouble(pressureMaxDuring[0]))
                                warningStartMax = warningGeneral;
                            if (Convert.ToInt16(item.PressurePhaseStartMin) > Convert.ToDouble(pressureMinDuring[0]))
                                warningStartMin = warningGeneral;


                            rowData[0, 0] = $"{warningStartMax} Max Pressure at Phase Start = {pressureStartMaxValue[0]} inHg From {pressureStartMaxValue[1] } at {startTime.ToString()} mins";
                            rowData[0, 1] = $"(Req < {item.PressurePhaseStartMax} InHg)";
                            rowData[1, 0] = $"{warningStartMin} Min Pressure at Phase Start = {pressureStartMinValue[0] } inHg From {pressureStartMinValue[1] } at {startTime.ToString()} mins";
                            rowData[1, 1] = $"(Req > {item.PressurePhaseStartMin} InHg)";
                            rowData[2, 0] = $"{warningEndMax} Max Pressure at Phase End = {pressureEndMaxValue[0]} inHg From {pressureEndMaxValue[1]} at {endTime.ToString()} mins";
                            rowData[2, 1] = $"(Req < {item.PressurePhaseEndMax} InHg)";
                            rowData[3, 0] = $"{ warningEndMin} Min Pressure at Phase End = {pressureEndMinValue[0]} inHg From {pressureEndMinValue[1]} at {endTime.ToString()} mins";
                            rowData[3, 1] = $"(Req > {item.PressurePhaseEndMin} InHg)";
                            rowData[4, 0] = $"{warningStartMax} Max Pressure During {pressureMaxDuring[0]}";
                            rowData[4, 1] = $"(Req {item.PressurePhaseStartMax} InHg";
                            rowData[5, 0] = $"{warningStartMin} Min Pressure During {pressureMinDuring[0]}";
                            rowData[5, 1] = $"(Req {item.PressurePhaseStartMin} InHg";

                            string sectionName = item.PressureTitle.ToUpper();
                            addReportTableRows(tableHeader.WidthF, tableDetail, rowData, colCount, sectionName);



                            #region Presssure Rate



                            PtcRate pressureRate = qualityBatchReportCreator.GetPhasePressureRate(batchNumericReportModel.NumericDataTable, startTime, endTime);
                            string[,] rowDataRate = new string[2, 2];
                            string warningMaxTempRate = "";
                            string warningMinTempRate = "";
                            //  string warningGeneral = "***";
                            if (Convert.ToDouble(item.PressureRateMax) < Convert.ToDouble(pressureRate.maxRateValue))
                                warningMaxTempRate = warningGeneral;
                            if (Convert.ToDouble(item.PressureRateMin) > Convert.ToDouble(pressureRate.minRateValue))
                                warningMinTempRate = warningGeneral;
                            string LowRangeText = item.PressureRateMax.ToString() == "" ? "Min" : item.PressureRateMax.ToString();
                            string HighRangeText = item.PressureRateMin.ToString() == "" ? "Max" : item.PressureRateMin.ToString();

                            rowDataRate[0, 0] = $" {warningMaxTempRate} Max. Pressure Rate = {pressureRate.maxRateValue} Deg/Min From {pressureRate.maxRateCHName} at {pressureRate.maxRateMins} mins";
                            rowDataRate[0, 1] = $" (Req < {item.PressureRateMax.ToString()} Deg/Min )";
                            rowDataRate[1, 0] = $" {warningMinTempRate} Min. Pressure Rate = {pressureRate.minRateValue} Deg/Min From {pressureRate.minRateCHName} at {pressureRate.minRateMins} mins";
                            rowDataRate[1, 1] = $" (Req > {item.PressureRateMin.ToString()} Deg/Min )";

                            sectionName = "PRESSURE CONTROL RATE";
                            addReportTableRows(tableHeader.WidthF, tableDetail, rowDataRate, colCount, sectionName.ToUpper());
                            #endregion PressureRate

                        }
                        catch
                        {

                        }
                    }

                    //Çalışması için uygun kriter bul
                    if (!isError && Convert.ToBoolean(item.AirTempStyle)) //4 satır AirTC
                    {
                        try
                        {
                            string[,] rowData = new string[3, 2];

                            string[] airTcMaxValue = qualityBatchReportCreator.getMaxAirValue(startTime, endTime, batchNumericReportModel.NumericDataTable);
                            string[] airTcMinValue = qualityBatchReportCreator.getMinAirValue(startTime, endTime, batchNumericReportModel.NumericDataTable);

                            string warningAirMax = "";
                            string warningAirMin = "";
                            string warningGeneral = "***";
                            if (Convert.ToInt16(item.AirTempMax) < Convert.ToDouble(airTcMaxValue[0]))
                                warningAirMax = warningGeneral;
                            if (Convert.ToInt16(item.AirTempMin) > Convert.ToDouble(airTcMinValue[0]))
                                warningAirMin = warningGeneral;


                            rowData[0, 0] = $"{warningAirMax} Max AirTC During Phase {airTcMaxValue[0]} °C ";
                            rowData[0, 1] = $"(Req < {item.AirTempMax} °C )";
                            rowData[1, 0] = $"{warningAirMin} Min AirTC During Phase  {airTcMinValue[0]} °C ";
                            rowData[1, 1] = $"(Req > {item.AirTempMin} °C )";

                            string sectionName = item.AirTempTitle;
                            addReportTableRows(tableHeader.WidthF, tableDetail, rowData, colCount, sectionName);
                        }
                        catch
                        {
                        }
                    }

                    if (!isError && Convert.ToBoolean(item.PartTempStyle)) //3 satır part temp section
                    {
                        try
                        {
                            string[,] rowData = new string[3, 2];
                            bool rangeControl = true;
                            int LowRange = 0;
                            int HighRange = 0;
                            if (item.PartTempLowRange.ToString() == "" && item.PartTempHighRange.ToString() == "")
                            {
                                rangeControl = false;
                            }
                            else
                            {
                                LowRange = item.PartTempLowRange.ToString() == "" ? 0 : Convert.ToInt32(item.PartTempLowRange);
                                HighRange = item.PartTempHighRange.ToString() == "" ? 1000 : Convert.ToInt32(item.PartTempHighRange);
                            }

                            PtcRate airTcMaxValue = qualityBatchReportCreator.GetPhasePartRate(batchNumericReportModel.NumericDataTable, startTime, endTime, LowRange, HighRange, item.PartTempRateCalcInterval, rangeControl);

                            string warningMaxTempRate = "";
                            string warningMinTempRate = "";
                            string warningGeneral = "***";
                            if (Convert.ToDouble(item.PartTempRateMax) < Convert.ToDouble(airTcMaxValue.maxRateValue))
                                warningMaxTempRate = warningGeneral;
                            if (Convert.ToDouble(item.PartTempRateMin) > Convert.ToDouble(airTcMaxValue.minRateValue))
                                warningMinTempRate = warningGeneral;
                            string LowRangeText = item.PartTempLowRange.ToString() == "" ? "Min" : item.PartTempLowRange.ToString();
                            string HighRangeText = item.PartTempHighRange.ToString() == "" ? "Max" : item.PartTempHighRange.ToString();

                            rowData[0, 0] = $" {warningMaxTempRate} Max. Temp Rate between {LowRangeText} - {HighRangeText} Degs = {String.Format("{0:0.00}", airTcMaxValue.maxRateValue)} Deg/Min From {airTcMaxValue.maxRateCHName} at {airTcMaxValue.maxRateMins} mins";
                            rowData[0, 1] = $" (Req < {item.PartTempRateMax.ToString()} Deg/Min )";
                            rowData[1, 0] = $" {warningMinTempRate} Min. Temp Rate between {LowRangeText} - {HighRangeText} Degs = {airTcMaxValue.minRateValue} Deg/Min From {airTcMaxValue.minRateCHName} at {airTcMaxValue.minRateMins} mins";
                            rowData[1, 1] = $" (Req > {item.PartTempRateMin.ToString()} Deg/Min )";

                            string sectionName = item.PartTempTitle;
                            addReportTableRows(tableHeader.WidthF, tableDetail, rowData, colCount, sectionName);
                        }
                        catch
                        {
                        }
                    }

                    endReportTable(tableDetail);
                    addTable(tableHeader, tableDetail, resultHeader,false, isError);
                    startTime = endTime;

                }
                #endregion public."BatchQualityDetails"

                _qualityReport.xrLabelLoadNumber.Text = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                _qualityReport.xrLabelRecipeName.Text = batchNumericReportModel.NumericReportHeaderInfo.RecipeName;
                _qualityReport.xrLabelRunStart.Text = batchNumericReportModel.NumericReportHeaderInfo.StartDate.ToString();
                _qualityReport.xrLabelRunEnd.Text = batchNumericReportModel.NumericReportHeaderInfo.EndDate.ToString();
                _qualityReport.xrLabelReportDate.Text = DateTime.Now.ToString();

                xtraReport = _qualityReport;
            }
            return xtraReport;
        }


        public XtraReport QualityBagReport(int qualityCardId)
        {
            XtraReport xtraReport = null;

            ActiveTagService activeTagService = new ActiveTagService(_connectionString);

            NumericReportService numericReportService = new NumericReportService(_connectionString);

            BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);

            if (batchNumericReportModel.Bags == null)
                return null;

            DataTable integratedCheckResultsDataTable = new DataTable();
            DataConverter dataConverter = new DataConverter();

            DateTime reportStartTime = DateTime.Now;
            DateTime sectionTime = DateTime.Now;
            double diffSecRunTime = 0;
            double diffTotalRunTime = 0;
            int i = 1, startTime = 0, endTime = 0, colCount = 2, colCountPartNames = 7, diffTime = 0;

            level = 0;
            _qualityReport = new QualityReport();
            _qualityReport.xrLabelReportHeaderTitleRoot.Text = "Composite Curing Control System\nTAI\nQuality Report - Individual Part";

            XRTable thPorts = new XRTable(), tdPorts = new XRTable();
            createReportTable(out thPorts, out tdPorts);
            string portHeader = "PART, TOOL AND SENSOR INFORMATION";
            string[] sectionPort = new string[colCountPartNames];
            sectionPort[0] = "#";
            sectionPort[1] = "FIELD NAME";
            sectionPort[2] = "FIELD VALUE";
            sectionPort[3] = "PARTTC";
            sectionPort[4] = "MONITOR";
            sectionPort[5] = "SRC";
            sectionPort[6] = "LEAKAMT/REQ";
            int maxRow = 0;


            IEnumerable<Bag> bags = batchNumericReportModel.Bags;
            Dictionary<int, ActiveTag> activeTagNames = activeTagService.ActiveTagsByTagIdKey();

            foreach (Bag bagItem in bags)
            {

                BagNumericReportModel bagNumericReportModel = numericReportService.NumericReportByBag(_batchId, bagItem.id, 1000000, 1);

                if (bagNumericReportModel.IntegratedCheckReportItems == null)
                    return null;

                integratedCheckResultsDataTable = dataConverter.ConvertToDataTable(bagNumericReportModel.IntegratedCheckReportItems);

                BagSensors bagSensors = new BagSensors();
                bagSensors.PTCs = bagItem.SelectedPorts.Select(x => activeTagNames[x]).Where(x => x.ActiveTagGroupId == ActiveTagGroups.PTC).OrderBy(x => x.id).Select(x => x.TagName).ToList();
                bagSensors.MONs = bagItem.SelectedPorts.Select(x => activeTagNames[x]).Where(x => x.ActiveTagGroupId == ActiveTagGroups.MON).OrderBy(x => x.id).Select(x => x.TagName).ToList();
                bagSensors.VACs = bagItem.SelectedPorts.Select(x => activeTagNames[x]).Where(x => x.ActiveTagGroupId == ActiveTagGroups.VAC).OrderBy(x => x.id).Select(x => x.TagName).ToList();

                maxRow = bagSensors.PTCs.Count();
                if (maxRow < bagSensors.MONs.Count())
                    maxRow = bagSensors.MONs.Count();
                if (maxRow < bagSensors.VACs.Count())
                    maxRow = bagSensors.VACs.Count();
                if (maxRow < 4)
                    maxRow = 4;//Bag,Soir,Tool ve Başlık için gerekli satır sayısı min 4


                string[,] rowPortData = new string[maxRow, colCountPartNames];
                rowPortData[0, 0] = ""; // todo:m Quality bag sayısı gelecek EK
                rowPortData[0, 1] = "Bag";
                rowPortData[0, 2] = bagItem.BagName;

                DataTable distinctRequirementValue = integratedCheckResultsDataTable.DefaultView.ToTable(true, "RequirementValue");

                rowPortData[0, 6] = $"{integratedCheckResultsDataTable.Compute("MAX(Deviation)", "").ToString()} / { distinctRequirementValue.Rows[0][0]} ";

                rowPortData[1, 1] = "Soir";
                rowPortData[1, 2] = bagNumericReportModel.NumericReportHeaderInfo.SoirNames;
                rowPortData[2, 1] = "Part";
                rowPortData[2, 2] = bagNumericReportModel.NumericReportHeaderInfo.PartNames;
                rowPortData[3, 1] = "Tool";
                rowPortData[3, 2] = bagNumericReportModel.NumericReportHeaderInfo.ToolNames;

                DataTable bagPorts = new DataTable();
                bagPorts.Columns.Add("AllPorts");
                bagPorts.Rows.Add(bagSensors.PTCs);
                bagPorts.Rows.Add(bagSensors.MONs);
                bagPorts.Rows.Add(bagSensors.VACs);

                // todo:m Quality kimse görmeden düzelt burayı
                int rowCountPtc = 0;
                foreach (var item in bagSensors.PTCs)
                {
                    rowPortData[rowCountPtc, 3] = item;
                    rowCountPtc++;
                }
                int rowCountMon = 0;
                foreach (var item in bagSensors.MONs)
                {
                    rowPortData[rowCountMon, 4] = item;
                    rowCountMon++;
                }
                int rowCountVac = 0;
                foreach (var item in bagSensors.VACs)
                {
                    rowPortData[rowCountVac, 5] = item;
                    rowCountVac++;
                }

                addReportTableRows(thPorts.WidthF, tdPorts, rowPortData, colCountPartNames, "ReportHeader", sectionPort);
                endReportTable(tdPorts);
                addTable(thPorts, tdPorts, portHeader, true, false);


                _qualityReport.xrLabelLoadNumber.Text = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                _qualityReport.xrLabelRecipeName.Text = batchNumericReportModel.NumericReportHeaderInfo.RecipeName;
                _qualityReport.xrLabelRunStart.Text = batchNumericReportModel.NumericReportHeaderInfo.StartDate.ToString();
                _qualityReport.xrLabelRunEnd.Text = batchNumericReportModel.NumericReportHeaderInfo.EndDate.ToString();
                _qualityReport.xrLabelReportDate.Text = DateTime.Now.ToString();

                xtraReport = _qualityReport;
            }


            return xtraReport;
        }

        #region quality Batch Report

        private void createReportTable(out XRTable tableHeader, out XRTable tableDetail)
        {
            tableHeader = new XRTable();
            tableDetail = new XRTable();
            try
            {
                int useableWidth = _qualityReport.PageWidth - (_qualityReport.Margins.Left + _qualityReport.Margins.Right);

                tableHeader.BeginInit();
                tableHeader.LocationF = new PointF(0F, 30);
                tableHeader.Height = 20;
                tableHeader.WidthF = useableWidth;

                tableHeader.AdjustSize();
                tableHeader.PerformLayout();
                tableHeader.EndInit();


                tableDetail.BeginInit();
                tableDetail.Width = useableWidth;
            }
            catch
            {

            }
        }
        private static void endReportTable(XRTable tableDetail)
        {
            tableDetail.AdjustSize();
            tableDetail.PerformLayout();
            tableDetail.EndInit();
        }
        private static void addReportTableRows(float width, XRTable tableDetail, string[,] rowValues, int colCount, string secTitle)
        {
            try
            {
                XRTableRow tableRow = new XRTableRow();
                XRTableCell TableCellin = new XRTableCell();
                TableCellin.Font = new System.Drawing.Font("Tahoma", 7F, FontStyle.Bold);
                TableCellin.WidthF = width;
                TableCellin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                TableCellin.Text = secTitle;
                tableRow.Cells.Add(TableCellin);
                tableDetail.Rows.Add(tableRow);

                for (int k = 0; k < rowValues.Length / 2; k++)
                {
                    tableRow = new XRTableRow();
                    for (int i = 0; i < colCount; i++)
                    {
                        TableCellin = new XRTableCell();
                        TableCellin.Font = new System.Drawing.Font("Tahoma", 7F);
                        if (i == 0)
                            TableCellin.WidthF = (float)(width * 0.8);
                        else
                            TableCellin.WidthF = (float)(width * 0.2);
                        TableCellin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        TableCellin.Text = rowValues[k, i];
                        tableRow.Cells.Add(TableCellin);
                    }
                    tableDetail.Rows.Add(tableRow);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void addTable(XRTable tableHeader, XRTable tableDetail, string tableHeaderText, bool isPartInfo = false, bool isError = false)
        {
            try
            {
                DetailReportBand detailReportBand = new DevExpress.XtraReports.UI.DetailReportBand();
                detailReportBand.Level = level;
                XRLabel label = new XRLabel();
                label.BackColor = isError ? Color.Red : Color.Silver;
                label.WidthF = (_qualityReport.PageWidth - (_qualityReport.Margins.Left + _qualityReport.Margins.Right));
                label.Text = tableHeaderText;

                GroupHeaderBand groupHeaderBand = new DevExpress.XtraReports.UI.GroupHeaderBand();
                groupHeaderBand.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                    tableHeader,label
                    });
                groupHeaderBand.Height = 20;
                groupHeaderBand.Name = "GroupHeaderName" + level.ToString();//level.ToString();
                groupHeaderBand.RepeatEveryPage = true;

                DetailBand detailBand = new DevExpress.XtraReports.UI.DetailBand();
                detailBand.Height = 20;

                //detailBand.PageBreak = PageBreak.AfterBand;
                detailBand.Controls
                    .AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                    tableDetail
                    });

                detailReportBand.Bands
                    .AddRange(new DevExpress.XtraReports.UI.Band[] {
                    groupHeaderBand,
                    detailBand
                    });

                _qualityReport.Bands.Add(detailReportBand);
                level++;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion quality Batch Report


        #region quality bag report
        private static void addReportTableRows(float width, XRTable tableDetail, string[,] rowValues, int colCount, string secTitle, string[] titleValues)
        {
            try
            {
                XRTableRow tableRow = new XRTableRow();
                XRTableCell TableCellin = new XRTableCell();

                for (int i = 0; i < titleValues.Length; i++)
                {
                    TableCellin = new XRTableCell();
                    TableCellin.Font = new System.Drawing.Font("Tahoma", 7F, FontStyle.Bold);
                    if (secTitle == "")
                    {
                        TableCellin.WidthF = width / titleValues.Length;
                    }
                    else if (secTitle == "ReportHeader")
                    {
                        switch (i)
                        {
                            case 0:
                                TableCellin.WidthF = 35;//Bag no
                                break;
                            case 1:
                                TableCellin.WidthF = 75;//Field Name
                                break;
                            case 2:
                                TableCellin.WidthF = 298;//Field Value
                                break;
                            case 3:
                                TableCellin.WidthF = 75;//PARTTC
                                break;
                            case 4:
                                TableCellin.WidthF = 75;//MONITOR
                                break;
                            case 5:
                                TableCellin.WidthF = 75;//SRC
                                break;
                            case 6:
                                TableCellin.WidthF = 90;//LEAKAMT/REQ
                                break;
                        }
                    }

                    TableCellin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                    TableCellin.Text = titleValues[i];
                    tableRow.Cells.Add(TableCellin);
                }
                tableDetail.Rows.Add(tableRow);

                for (int k = 0; k < rowValues.Length / colCount; k++)
                {
                    tableRow = new XRTableRow();
                    for (int i = 0; i < colCount; i++)
                    {
                        TableCellin = new XRTableCell();
                        TableCellin.Font = new System.Drawing.Font("Tahoma", 7F);
                        if (secTitle != "ReportHeader")
                        {
                            TableCellin.WidthF = (float)(width * (1 / colCount));
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    TableCellin.WidthF = 35;//Bag no
                                    break;
                                case 1:
                                    TableCellin.WidthF = 75;//Field Name
                                    break;
                                case 2:
                                    TableCellin.WidthF = 298;//Field Value
                                    break;
                                case 3:
                                    TableCellin.WidthF = 75;//PARTTC
                                    break;
                                case 4:
                                    TableCellin.WidthF = 75;//MONITOR
                                    break;
                                case 5:
                                    TableCellin.WidthF = 75;//SRC
                                    break;
                                case 6:
                                    TableCellin.WidthF = 90;//LEAKAMT/REQ
                                    break;
                            }
                        }

                        TableCellin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        TableCellin.Text = rowValues[k, i];
                        tableRow.Cells.Add(TableCellin);
                    }
                    tableDetail.Rows.Add(tableRow);
                }
            }
            catch (Exception ex)
            {
                //logger.AddLog("ReportQualityBagClass", "addReportTableRows", "addReportTableRows", ex.ToString(), GlobalUserAuth.UserName);
                throw ex;
            }
        }



        #endregion quality bag report
    }
}
