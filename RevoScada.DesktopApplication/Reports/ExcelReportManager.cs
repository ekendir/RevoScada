using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Revo.Core.Data;
using RevoScada.Business.Report;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using DevExpress.Spreadsheet;
using System.IO;
using System.Windows;
using DevExpress.Xpf.WindowsUI;
using Revo.Core;
using System.Drawing;
using DevExpress.Spreadsheet.Charts;
using RevoScada.Configurator;
using System.Data;
using System.Linq;
using RevoScada.Business;
using Newtonsoft.Json;

namespace RevoScada.DesktopApplication.Reports
{
    public class ExcelReportManager
    {
        private Workbook _workbook = new Workbook();

        private Worksheet _worksheet;
        private CellRange defaultHeader;
        private readonly string _connectionString;
        private readonly int _batchId;
        public string ExcelFilePassword { get; set; } = "0";
        public string ExcelFileFullPath { get; set; } = "c:\\ExcelReport.xls";
        public object AlarmTagConfigurations { get; set; }
        public ExcelReportManager(string connectionString, int batchId)
        {
            _connectionString = connectionString;
            _batchId = batchId;
        }
        public async Task<bool> CreateAllReport()
        {
            List<bool> resultList = new List<bool>();
            bool result = false;

            result = await CreateBatchSheet();
            resultList.Add(result);

            result = await CreateRecipeSheet();
            resultList.Add(result);

            result = await CreateEventsSheet();
            resultList.Add(result);

            result = await CreateNumericSheet();
            resultList.Add(result);

            if(ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId != 20)
            {
                result = await CreateTrendSheet();
                resultList.Add(result);

                result = await CreateTrendBagSheet();
                resultList.Add(result);
            }

            result = await CreateNumericBagSheet();
            resultList.Add(result);

            var saveResult = WorksheetSave();
            resultList.Add(saveResult);

            return resultList.TrueForAll(x => x == true);
        }



        private async Task<bool> CreateBatchSheet()

        {
            bool result = await Task.Run(() =>
            {

                _worksheet = _workbook.Worksheets[0];

                BatchReportService batchReportService = new BatchReportService(_connectionString);
                BatchReportModel batchReportModel = batchReportService.BatchReport(_batchId);
                System.Data.DataTable IntegratedCheckDataTable = new System.Data.DataTable();
                DataConverter dataConverter = new DataConverter();
                IntegratedCheckDataTable = dataConverter.ConvertToDataTable(batchReportModel.IntegratedCheckReportItems);

                try
                {
                    _workbook.Unit = DevExpress.Office.DocumentUnit.Point;

                    _workbook.BeginUpdate();

                    _worksheet.Name = "BATCH";

                    defaultHeader = _worksheet.Range["A1:E1"];
                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 20;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                    defaultHeader = _worksheet.Range["A2:E2"];
                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 16;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = "BATCH REPORT";

                    //CellRange subTitle = worksheet.Range.Union(worksheet["A3:A6"], worksheet["B3:B6"]);
                    CellRange subTitle = _worksheet["A3:A6"];
                    subTitle.FillColor = Color.LightGray;
                    subTitle.Font.Bold = true;
                    subTitle.AutoFitColumns();
                    subTitle.Font.Name = "Arial Narrow";
                    subTitle.Font.Size = 12;

                    _worksheet.Cells[2, 0].Value = "Batch Name";
                    _worksheet.Cells[3, 0].Value = "Start Date";
                    _worksheet.Cells[4, 0].Value = "End Date";
                    _worksheet.Cells[5, 0].Value = "Recipe Name";

                    _worksheet.Cells[2, 1].Value = batchReportModel.ReportHeaderInfo.LoadNumber;
                    _worksheet.Cells[3, 1].Value = batchReportModel.ReportHeaderInfo.StartDate;
                    _worksheet.Cells[4, 1].Value = batchReportModel.ReportHeaderInfo.EndDate;
                    _worksheet.Cells[5, 1].Value = batchReportModel.ReportHeaderInfo.RecipeName;

                    int rowsCount = 8;
                    foreach (var item in batchReportModel.BagSensorAndPartDetails)
                    {

                        int firstCellIndex = rowsCount + 1;
                        CellRange rangeLotProperties = _worksheet.Range[$"A{firstCellIndex}:D{firstCellIndex}"];

                        _worksheet.Cells[rowsCount, 0].Value = "Bag Name";
                        _worksheet.Cells[rowsCount, 1].Value = "Part Name";
                        _worksheet.Cells[rowsCount, 2].Value = "Tool Name";
                        _worksheet.Cells[rowsCount, 3].Value = "Soir Number";

                        rangeLotProperties.FillColor = Color.LightGray;
                        rangeLotProperties.Font.Bold = true;
                        rangeLotProperties.AutoFitColumns();
                        rangeLotProperties.Font.Name = "Arial Narrow";
                        rangeLotProperties.Font.Size = 12;

                        rowsCount++;

                        _worksheet.Cells[rowsCount, 0].Value = item.BagName;

                        for (int i = 0; i < item.LotProperties.Count; i++)
                        {
                            _worksheet.Cells[rowsCount, 1].Value = item.LotProperties[i].PartName;
                            _worksheet.Cells[rowsCount, 2].Value = item.LotProperties[i].ToolName;
                            _worksheet.Cells[rowsCount, 3].Value = item.LotProperties[i].SoirNumber;
                            rowsCount++;
                        }


                        int subCellIndex = rowsCount + 1;

                        rangeLotProperties = _worksheet.Range[$"B{subCellIndex}:D{subCellIndex}"];
                        rangeLotProperties.FillColor = Color.LightGray;
                        rangeLotProperties.Font.Bold = true;
                        rangeLotProperties.AutoFitColumns();
                        rangeLotProperties.Font.Name = "Arial Narrow";
                        rangeLotProperties.Font.Size = 12;

                        _worksheet.Cells[rowsCount, 1].Value = "T/C";
                        _worksheet.Cells[rowsCount, 2].Value = "MON";
                        _worksheet.Cells[rowsCount, 3].Value = "SRC";



                        rowsCount++;

                        int ptcCount = rowsCount;
                        CellRange rangePtc;
                        for (int i = 0; i < item.BagSensors.PTCs.Count; i++)
                        {
                            _worksheet.Cells[ptcCount, 1].Value = item.BagSensors.PTCs[i];

                            rangePtc = _worksheet.Range[$"B{ptcCount}"];
                            rangePtc.AutoFitRows();
                            rangePtc.Font.Name = "Arial Narrow";
                            rangePtc.Font.Size = 10;
                            ptcCount++;
                        }

                        int monCount = rowsCount;
                        CellRange rangeMon;
                        for (int i = 0; i < item.BagSensors.MONs.Count; i++)
                        {
                            _worksheet.Cells[monCount, 2].Value = item.BagSensors.MONs[i];

                            rangeMon = _worksheet.Range[$"C{monCount}"];
                            rangeMon.AutoFitColumns();
                            rangeMon.Font.Name = "Arial Narrow";
                            rangeMon.Font.Size = 10;
                            monCount++;
                        }

                        int vacCount = rowsCount;
                        CellRange rangeVac;
                        for (int i = 0; i < item.BagSensors.VACs.Count; i++)
                        {
                            _worksheet.Cells[vacCount, 3].Value = item.BagSensors.VACs[i];
                            rangeVac = _worksheet.Range[$"D{vacCount}"];
                            rangeVac.AutoFitColumns();
                            rangeVac.Font.Name = "Arial Narrow";
                            rangeVac.Font.Size = 10;
                            vacCount++;
                        }

                        if (ptcCount >= monCount && ptcCount >= vacCount)
                        {
                            rowsCount = ptcCount;
                        }
                        else if (monCount >= ptcCount && monCount >= vacCount)
                        {
                            rowsCount = monCount;
                        }
                        else if (vacCount >= ptcCount && vacCount >= monCount) { rowsCount = vacCount; }

                    }

                    CellRange integrityCheckTitleAndDetails = _worksheet.Range.Union(_worksheet.Cells[(rowsCount + 3), 1], _worksheet.Cells[rowsCount + 3, 1]);

                    if (IntegratedCheckDataTable == null)
                    {
                        _worksheet.Cells[rowsCount + 2, 0].Value = "Skiped Date";

                        if (batchReportModel.SkippedIntegratedCheckReportItem == null)
                        {
                            _worksheet.Cells[rowsCount + 3, 0].Value = DateTime.Now;
                        }
                        else
                        {
                            _worksheet.Cells[rowsCount + 3, 0].Value = batchReportModel.SkippedIntegratedCheckReportItem.SkipDate;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < IntegratedCheckDataTable.Columns.Count; i++)
                        {
                            _worksheet.Cells[rowsCount + 2, i].Value = IntegratedCheckDataTable.Columns[i].Caption;
                        }
                        for (int i = 0; i < IntegratedCheckDataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < IntegratedCheckDataTable.Columns.Count; j++)
                            {
                                // _worksheet.Cells[(rowsCount + 3 + i), j].Value = IntegratedCheckDataTable.Rows[i][j].ToString().Replace(",", ".");
                                _worksheet.Cells[(rowsCount + 3 + i), j].SetValue(IntegratedCheckDataTable.Rows[i][j]);
                            }
                        }
                    }

                    integrityCheckTitleAndDetails.AutoFitColumns();
                    integrityCheckTitleAndDetails.Font.Name = "Arial Narrow";
                    integrityCheckTitleAndDetails.Font.Size = 10;
                    integrityCheckTitleAndDetails = _worksheet.Range[$"A{rowsCount + 3}:G{rowsCount + 3}"];
                    integrityCheckTitleAndDetails.FillColor = Color.LightGray;
                    integrityCheckTitleAndDetails.AutoFitColumns();
                    integrityCheckTitleAndDetails.Font.Bold = true;
                    integrityCheckTitleAndDetails.Font.Name = "Arial Narrow";
                    integrityCheckTitleAndDetails.Font.Size = 12;



                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                    return false;

                }
                finally
                {
                    _workbook.EndUpdate();
                }
            });


            return result;
        }

        private async Task<bool> CreateRecipeSheet()
        {
            bool result = await Task.Run(() =>
                {
                    try
                    {
                        _worksheet = _workbook.Worksheets.Add();
                        _worksheet.Name = "RECIPE";

                        RecipeReportService recipeReportService = new RecipeReportService(_connectionString);


                        ReportHeaderInfo reportHeaderInfo = recipeReportService.ReportHeaderInfo(_batchId);
                        RevoScada.Business.UserService userService = new Business.UserService(_connectionString);
                        System.Data.DataTable recipeReportTable = recipeReportService.RecipeReport(_batchId);

                        CellRange recipeTitle;

                        for (int i = 0; i < recipeReportTable.Columns.Count; i++)
                        {
                            _worksheet.Cells[8, i].Value = recipeReportTable.Columns[i].Caption.Replace("Title", "");
                            recipeTitle = _worksheet.Range.Union(_worksheet.Cells[8, i], _worksheet.Cells[8, i]);
                            recipeTitle.FillColor = Color.LightGray;
                            recipeTitle.Font.Bold = true;
                            // recipeTitle.AutoFitColumns();
                            recipeTitle.AutoFitRows();
                            recipeTitle.Font.Name = "Arial Narrow";
                            recipeTitle.Font.Size = 12;

                        }

                        for (int i = 0; i < recipeReportTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < recipeReportTable.Columns.Count; j++)
                            {
                                _worksheet.Cells[9 + i, j].Value = recipeReportTable.Rows[i][j].ToString();
                                //_worksheet.Cells[8 + i, j].Value = recipeReportTable.Rows[i][j].ToString().Replace(",", ".");
                            }
                        }

                        defaultHeader = _worksheet.Range["A1:E1"];
                        defaultHeader.Font.Size = 20;
                        defaultHeader.Font.Bold = true;
                        defaultHeader.Font.Name = "Arial Narrow";
                        defaultHeader.Merge();
                        defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                        defaultHeader = _worksheet.Range["A2:E2"];
                        defaultHeader.Font.Name = "Arial Narrow";
                        defaultHeader.Font.Size = 16;
                        defaultHeader.Font.Bold = true;
                        defaultHeader.Merge();
                        defaultHeader.AutoFitColumns();

                        defaultHeader.Value = "RECIPE REPORT";


                        _worksheet.Cells[2, 0].Value = "Batch Name";
                        _worksheet.Cells[3, 0].Value = "Start Date";
                        _worksheet.Cells[4, 0].Value = "End Date";
                        _worksheet.Cells[5, 0].Value = "Recipe Name";
                        _worksheet.Cells[6, 0].Value = "Created By User";

                        _worksheet.Cells[2, 1].Value = reportHeaderInfo.LoadNumber;
                        _worksheet.Cells[3, 1].Value = reportHeaderInfo.StartDate;
                        _worksheet.Cells[4, 1].Value = reportHeaderInfo.EndDate;
                        _worksheet.Cells[5, 1].Value = reportHeaderInfo.RecipeName;
                        _worksheet.Cells[6, 1].Value = userService.GetById(reportHeaderInfo.CreatedByUserId).UserName;

                        //CellRange detailReportHeader = _worksheet.Range.Union(_worksheet.Cells[2,0], _worksheet.Cells[1,4]);

                        CellRange detailReportHeader = _worksheet.Range["A3:A7"];
                        detailReportHeader.FillColor = Color.LightGray;
                        detailReportHeader.Font.Bold = true;
                        detailReportHeader.AutoFitColumns();
                        detailReportHeader.Font.Name = "Arial Narrow";
                        detailReportHeader.Font.Size = 12;
                        detailReportHeader = _worksheet.Range["B3:B7"];
                        detailReportHeader.AutoFitColumns();


                        return true;
                    }

                    catch (Exception ex)
                    {
                        LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                        return false;
                    }
                });

            return result;
        }

        private async Task<bool> CreateEventsSheet()
        {
            bool result = await Task.Run(() =>
            {
                try
                {

                    _worksheet = _workbook.Worksheets.Add();
                    _worksheet.Name = "EVENTS";

                    AlarmReportService allAlarmReportService = new AlarmReportService(_connectionString);
                    allAlarmReportService.AlarmTagConfigurations = AlarmTagConfigurations;
                    AlarmReportModel plcAlarmReportModel = allAlarmReportService.PlcAlarmReport(_batchId);

                    System.Data.DataTable plcAlarmResultsDataTable = new System.Data.DataTable();

                    DataConverter dataConverter = new DataConverter();

                    plcAlarmResultsDataTable = dataConverter.ConvertToDataTable(plcAlarmReportModel.PlcAlarmReportItems);

                    if (plcAlarmResultsDataTable != null)
                    {
                        plcAlarmResultsDataTable.Columns.Remove("BatchId");
                        plcAlarmResultsDataTable.Columns.Remove("PlcValue");
                    }


                    CellRange plcAlarmReport;
                    if (plcAlarmResultsDataTable != null)
                    {
                        for (int i = 0; i < plcAlarmResultsDataTable.Columns.Count; i++)
                        {
                            _worksheet.Cells[12, i].Value = plcAlarmResultsDataTable.Columns[i].Caption;

                            plcAlarmReport = _worksheet.Range.Union(_worksheet.Cells[12, i], _worksheet.Cells[12, i]);
                            plcAlarmReport.AutoFitColumns();
                            plcAlarmReport.Font.Name = "Arial Narrow";
                            plcAlarmReport.Font.Size = 10;
                        }
                        for (int i = 0; i < plcAlarmResultsDataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < plcAlarmResultsDataTable.Columns.Count; j++)
                            {
                                //_worksheet.Cells[i + 13, j].Value = plcAlarmResultsDataTable.Rows[i][j].ToString().Replace(",", ".");
                                _worksheet.Cells[i + 13, j].Value = plcAlarmResultsDataTable.Rows[i][j].ToString();
                            }
                        }
                    }

                    plcAlarmReport = _worksheet.Range["A13:E13"];
                    plcAlarmReport.FillColor = Color.LightGray;
                    //plcAlarmReport.AutoFitColumns();
                    plcAlarmReport.Font.Bold = true;
                    plcAlarmReport.Font.Name = "Arial Narrow";
                    plcAlarmReport.Font.Size = 12;
                    #region ReportHeader

                    defaultHeader = _worksheet.Range["A1:E1"];
                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 20;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                    defaultHeader = _worksheet.Range["A2:E2"];
                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 16;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = "EVENT REPORT";


                    CellRange detailReportHeader = _worksheet.Range["A3:A6"];
                    detailReportHeader.FillColor = Color.LightGray;
                    detailReportHeader.Font.Bold = true;
                    detailReportHeader.Font.Name = "Arial Narrow";
                    detailReportHeader.Font.Size = 12;

                    _worksheet.Cells[2, 0].Value = "Batch Name";
                    _worksheet.Cells[3, 0].Value = "Start Date";
                    _worksheet.Cells[4, 0].Value = "End Date";
                    _worksheet.Cells[5, 0].Value = "Recipe Name";

                    _worksheet.Cells[2, 1].Value = plcAlarmReportModel.ReportHeaderInfo.LoadNumber;
                    _worksheet.Cells[3, 1].Value = plcAlarmReportModel.ReportHeaderInfo.StartDate;
                    _worksheet.Cells[4, 1].Value = plcAlarmReportModel.ReportHeaderInfo.EndDate;
                    _worksheet.Cells[5, 1].Value = plcAlarmReportModel.ReportHeaderInfo.RecipeName;

                    #endregion ReportHeader

                    #region ProcessEventLog

                    System.Data.DataTable processEventLogResultsDataTable = new System.Data.DataTable();

                    processEventLogResultsDataTable = dataConverter.ConvertToDataTable(plcAlarmReportModel.ProcessEventLogReportItems);


                    int plcAlarmRowCount = 0;
                    if (plcAlarmResultsDataTable != null)
                    {
                        plcAlarmRowCount = plcAlarmResultsDataTable.Rows.Count;
                    }
                    else
                    {
                        plcAlarmRowCount = 14;
                    }


                    CellRange processEventReport;
                    if (processEventLogResultsDataTable != null)
                    {
                        for (int i = 0; i < processEventLogResultsDataTable.Columns.Count; i++)
                        {
                            _worksheet.Cells[plcAlarmRowCount + 14, i].Value = processEventLogResultsDataTable.Columns[i].Caption;
                            processEventReport = _worksheet.Range.Union(_worksheet.Cells[plcAlarmRowCount + 14, 0], _worksheet.Cells[plcAlarmRowCount + 14, i]);

                            //processEventReport.AutoFitRows();
                            processEventReport.Font.Name = "Arial Narrow";
                            processEventReport.Font.Size = 10;
                            processEventReport.FillColor = Color.LightGray;
                            processEventReport.Font.Bold = true;

                        }
                        for (int i = 0; i < processEventLogResultsDataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < processEventLogResultsDataTable.Columns.Count; j++)
                            {
                                // _worksheet.Cells[i + plcAlarmRowCount + 15, j].Value = processEventLogResultsDataTable.Rows[i][j].ToString().Replace(",", ".");
                                _worksheet.Cells[i + plcAlarmRowCount + 15, j].Value = processEventLogResultsDataTable.Rows[i][j].ToString();
                            }
                        }

                        processEventReport = _worksheet.Range.Union(_worksheet.Cells[processEventLogResultsDataTable.Rows.Count + plcAlarmRowCount, 0], _worksheet.Cells[processEventLogResultsDataTable.Rows.Count + plcAlarmRowCount, 50]);
                        processEventReport.Font.Name = "Arial Narrow";
                        //processEventReport.AutoFitRows();
                        processEventReport.Font.Size = 12;
                    }

                    #endregion ProcessEventLog



                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                    return false;
                }
            });

            return result;
        }

        private async Task<bool> CreateNumericSheet()
        {
            bool result = await Task.Run(() =>
            {
                try
                {

                    _worksheet = _workbook.Worksheets.Add();
                    _worksheet.Name = "BAG_NUM";

                    NumericReportService numericReportService = new NumericReportService(_connectionString);
                    BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);
                    System.Data.DataTable integratedCheckResultsDataTable = new System.Data.DataTable();
                    DataConverter dataConverter = new DataConverter();
                    integratedCheckResultsDataTable = dataConverter.ConvertToDataTable(batchNumericReportModel.IntegratedCheckReportItems);
                    DisabledPortService _disabledPort = new DisabledPortService(_connectionString);
                    Dictionary<DateTime, List<string>> disablePortByBag = _disabledPort.GetByBatchGroupedByReceivedDateWithPortName(_batchId);



                    #region ReportHeader
                    defaultHeader = _worksheet.Range["A1:E1"];

                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 20;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                    defaultHeader = _worksheet.Range["A2:E2"];
                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 16;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = "NUMERIC REPORT";

                    _worksheet.Cells[2, 0].Value = "Batch Name";
                    _worksheet.Cells[3, 0].Value = "Bag Name";
                    _worksheet.Cells[4, 0].Value = "Soir Name";
                    _worksheet.Cells[5, 0].Value = "Part Name";
                    _worksheet.Cells[6, 0].Value = "Tool Name";
                    _worksheet.Cells[7, 0].Value = "Start Date";
                    _worksheet.Cells[8, 0].Value = "End Date";
                    _worksheet.Cells[9, 0].Value = "Recipe Name";

                    _worksheet.Cells[2, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                    _worksheet.Cells[3, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.BagNames;
                    _worksheet.Cells[4, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.SoirNames;
                    _worksheet.Cells[5, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.PartNames;
                    _worksheet.Cells[6, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.ToolNames;
                    _worksheet.Cells[7, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.StartDate;
                    _worksheet.Cells[8, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.EndDate;
                    _worksheet.Cells[9, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.RecipeName;

                    CellRange detailReportHeader = _worksheet.Range["A3:A10"];
                    detailReportHeader.AutoFitColumns();
                    detailReportHeader.FillColor = Color.LightGray;
                    detailReportHeader.Font.Bold = true;
                    detailReportHeader.Font.Name = "Arial Narrow";
                    detailReportHeader.Font.Size = 12;
                    detailReportHeader = _worksheet.Range["B3:B10"];
                    //detailReportHeader = _worksheet.Range["C3:C10"];
                    detailReportHeader.AutoFitColumns();

                    #endregion ReportHeader

                    #region IntegrityCheck
                    int integrityCheckRowCount = 0;

                    if (integratedCheckResultsDataTable == null)
                    {
                        integrityCheckRowCount = 14;
                    }
                    else
                    {
                        integrityCheckRowCount = 14 + integratedCheckResultsDataTable.Rows.Count;
                    }

                    CellRange integrityCheckTitle = _worksheet.Range.Union(_worksheet.Cells[12, 0], _worksheet.Cells[12, 1]);
                    //integrityCheckTitle.AutoFitColumns();
                    integrityCheckTitle.Font.Bold = true;
                    integrityCheckTitle.Font.Name = "Arial Narrow";
                    integrityCheckTitle.Font.Size = 10;

                    if (integratedCheckResultsDataTable == null)
                    {
                        _worksheet.Cells[12, 0].Value = "Skiped Date";

                        if (batchNumericReportModel.SkippedIntegratedCheckReportItem == null)
                        {
                            _worksheet.Cells[13, 0].Value = DateTime.Now;
                        }
                        else
                        {
                            _worksheet.Cells[13, 0].Value = batchNumericReportModel.SkippedIntegratedCheckReportItem.SkipDate;
                        }

                        //integrityCheckRowCount = 2;
                    }
                    else
                    {
                        for (int i = 0; i < integratedCheckResultsDataTable.Columns.Count; i++)
                        {
                            _worksheet.Cells[12, i].Value = integratedCheckResultsDataTable.Columns[i].Caption;
                            integrityCheckTitle = _worksheet.Range.Union(_worksheet.Cells[12, i], _worksheet.Cells[12, integratedCheckResultsDataTable.Columns.Count - 1]);
                            integrityCheckTitle.FillColor = Color.LightGray;
                            // integrityCheckTitle.AutoFitColumns();
                            integrityCheckTitle.Font.Bold = true;
                            integrityCheckTitle.Font.Name = "Arial Narrow";
                            integrityCheckTitle.Font.Size = 10;


                        }
                        for (int i = 0; i < integratedCheckResultsDataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < integratedCheckResultsDataTable.Columns.Count; j++)
                            {
                                //_worksheet.Cells[13 + i, j].Value = integratedCheckResultsDataTable.Rows[i][j].ToString().Replace(",", ".");
                                _worksheet.Cells[13 + i, j].SetValue(integratedCheckResultsDataTable.Rows[i][j]);
                                // _worksheet.Cells[13 + i, j].NumberFormat = "#,#";
                            }
                        }

                        //integrityCheckRowCount = integratedCheckResultsDataTable.Rows.Count + 14;
                    }

                    #endregion IntegrityCheck

                    CellRange numericReport;

                    if (batchNumericReportModel.NumericDataTable == null)
                        return false;

                    batchNumericReportModel.NumericDataTable = ReportHeaderSortOrder(batchNumericReportModel.NumericDataTable);

                    if(ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId !=20)
                    {batchNumericReportModel.NumericDataTable.Columns.Remove("Segment_No"); }
                    


                    for (int i = 0; i < batchNumericReportModel.NumericDataTable.Columns.Count; i++)
                    {

                        _worksheet.Cells[integrityCheckRowCount + 1, i].Value = batchNumericReportModel.NumericDataTable.Columns[i].Caption;

                        numericReport = _worksheet.Range.Union(_worksheet.Cells[integrityCheckRowCount + 1, i], _worksheet.Cells[integrityCheckRowCount + 1, batchNumericReportModel.NumericDataTable.Columns.Count]);
                        numericReport.FillColor = Color.LightGray;
                        numericReport.Font.Bold = true;
                        //numericReport.AutoFitColumns();
                        numericReport.Font.Name = "Arial Narrow";
                        numericReport.Font.Size = 10;
                    }
                    integrityCheckRowCount += 1;
                    for (int i = 0; i < batchNumericReportModel.NumericDataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < batchNumericReportModel.NumericDataTable.Columns.Count; j++)
                        {
                            //_worksheet.Cells[1 + integrityCheckRowCount + i, j].Value = batchNumericReportModel.NumericDataTable.Rows[i][j].ToString().Replace(",", ".");

                            var disableEnablePortResult = disablePortByBag.Where(x => x.Key == Convert.ToDateTime(batchNumericReportModel.NumericDataTable.Rows[i][0]) && x.Value.Contains(batchNumericReportModel.NumericDataTable.Columns[j].ColumnName)).Select(x => x.Key == Convert.ToDateTime(batchNumericReportModel.NumericDataTable.Rows[i][0]) && x.Value.Contains(batchNumericReportModel.NumericDataTable.Columns[j].ColumnName)).FirstOrDefault();

                            if (disableEnablePortResult)
                            {
                                _worksheet.Cells[1 + integrityCheckRowCount + i, j].SetValue($"[D]{String.Format("{0:0.0}", batchNumericReportModel.NumericDataTable.Rows[i][j])}");
                                CellRange disablePortFont = _worksheet.Cells[1 + integrityCheckRowCount + i, j];
                                disablePortFont.Font.Color = Color.LightGray;

                            }
                            else
                            {
                                _worksheet.Cells[1 + integrityCheckRowCount + i, j].SetValue(batchNumericReportModel.NumericDataTable.Rows[i][j]);

                            }


                            if (batchNumericReportModel.NumericDataTable.Columns[j].ColumnName != "Time" && !batchNumericReportModel.NumericDataTable.Columns[j].ColumnName.Contains("Press") && !_worksheet.Cells[1 + integrityCheckRowCount + i, j].ToString().Contains("[D]"))
                            {
                                _worksheet.Cells[1 + integrityCheckRowCount + i, j].NumberFormat = "#0.0";
                            }
                            else if (batchNumericReportModel.NumericDataTable.Columns[j].ColumnName.Contains("Press"))
                            {
                                _worksheet.Cells[1 + integrityCheckRowCount + i, j].NumberFormat = "#0.00";
                            }
                            else if (batchNumericReportModel.NumericDataTable.Columns[j].ColumnName == "Time")
                            {

                                _worksheet.Cells[1 + integrityCheckRowCount + i, j].NumberFormat = "HH:mm:ss";
                            }

                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                    return false;
                }
            });

            return result;
        }

        private async Task<bool> CreateTrendSheet()
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    _worksheet = _workbook.Worksheets.Add();
                    _worksheet.Name = "TREND";

                    NumericReportService numericReportService = new NumericReportService(_connectionString);
                    BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);

                    _workbook.BeginUpdate();

                    _workbook.Worksheets.ActiveWorksheet = _worksheet;
                    _worksheet.Columns[0].WidthInCharacters = 2.0;

                    defaultHeader = _worksheet.Range["A1:G1"];

                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 20;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                    defaultHeader = _worksheet.Range["A2:E2"];
                    defaultHeader.Font.Name = "Arial Narrow";
                    defaultHeader.Font.Size = 16;
                    defaultHeader.Font.Bold = true;
                    defaultHeader.Merge();
                    defaultHeader.Value = "TREND";

                    _worksheet.Cells[2, 0].Value = "Batch Name";
                    _worksheet.Cells[3, 0].Value = "Bag Name";
                    _worksheet.Cells[4, 0].Value = "Soir Name";
                    _worksheet.Cells[5, 0].Value = "Part Name";
                    _worksheet.Cells[6, 0].Value = "Tool Name";
                    _worksheet.Cells[7, 0].Value = "Start Date";
                    _worksheet.Cells[8, 0].Value = "End Date";
                    _worksheet.Cells[9, 0].Value = "Recipe Name";

                    _worksheet.Cells[2, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                    _worksheet.Cells[3, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.BagNames;
                    _worksheet.Cells[4, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.SoirNames;
                    _worksheet.Cells[5, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.PartNames;
                    _worksheet.Cells[6, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.ToolNames;
                    _worksheet.Cells[7, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.StartDate;
                    _worksheet.Cells[8, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.EndDate;
                    _worksheet.Cells[9, 1].Value = batchNumericReportModel.NumericReportHeaderInfo.RecipeName;

                    CellRange detailReportHeader = _worksheet.Range["A3:A10"];
                    detailReportHeader.AutoFitColumns();
                    detailReportHeader.FillColor = Color.LightGray;
                    detailReportHeader.Font.Bold = true;
                    detailReportHeader.Font.Name = "Arial Narrow";
                    detailReportHeader.Font.Size = 12;
                    detailReportHeader = _worksheet.Range["B3:B10"];
                    detailReportHeader.AutoFitColumns();



                    // Create a chart and specify its location.
                    Chart chart = _worksheet.Charts.Add(ChartType.Line);

                    chart.TopLeftCell = _worksheet.Cells["B12"];
                    chart.BottomRightCell = _worksheet.Cells["W50"];

                    Dictionary<string, List<CellValue>> valuesByTagNamesSensorValue = new Dictionary<string, List<CellValue>>();
                    List<CellValue> arguments = new List<CellValue>();
                    List<CellValue> values = new List<CellValue>();
                    string currColumnName = string.Empty;

                    var rowList = batchNumericReportModel.NumericDataTable.Rows;
                    var columnList = batchNumericReportModel.NumericDataTable.Columns;

                    int columnCounter = 0;
                    int rowCounter = 0;

                    List<string> columns = new List<string>();
                    List<string> rows = new List<string>();

                    foreach (var item in columnList)
                    {
                        columns.Add(item.ToString());
                    }

                    foreach (var item in columns)
                    {
                        if (item != "Mins" && item != "Time")
                            valuesByTagNamesSensorValue.Add(item, new List<CellValue>());
                    }

                    foreach (var item in rowList)
                    {
                        DataRow dataRow = (DataRow)item;

                        rowCounter = 0;
                        columnCounter = 0;
                        foreach (var dataItem in dataRow.ItemArray)
                        {
                            string columnName = columns[columnCounter];

                            if (columnName == "Time")
                            {
                                rowCounter++;
                                columnCounter++;
                                continue;
                            }

                            if (columnName == "Mins")
                            {
                                CellValue value = (dataRow[0] != DBNull.Value) ? Convert.ToUInt16(dataRow[0]) : default;
                                arguments.Add(value);
                            }
                            else
                            {
                                CellValue value = (dataRow[rowCounter] != DBNull.Value && !Double.IsNaN(Convert.ToDouble(dataRow[rowCounter]))) ? (float)Convert.ToDouble(dataRow[rowCounter]) : 0;
                                valuesByTagNamesSensorValue[columnName].Add(value);
                            }

                            rowCounter++;
                            columnCounter++;
                        }
                    }

                    for (int j = 0; j < batchNumericReportModel.NumericDataTable.Columns.Count; j++)
                    {
                        currColumnName = batchNumericReportModel.NumericDataTable.Columns[j].Caption;

                        if (currColumnName == "Time" || currColumnName == "Mins")
                            continue;

                        chart.Series.Add(currColumnName,
                                        arguments.ToArray(),
                                        valuesByTagNamesSensorValue[currColumnName].ToArray());

                    }

                    bool anyVacOrMonPort = false;

                    // Use the secondary axis for vacuum ports
                    for (int i = 0; i < chart.Series.Count; i++)
                    {
                        if (chart.Series[i].SeriesName.PlainText.StartsWith("MON") || chart.Series[i].SeriesName.PlainText.StartsWith("VAC"))
                        {
                            if (!anyVacOrMonPort)
                                anyVacOrMonPort = true;

                            chart.Series[i].AxisGroup = AxisGroup.Secondary;
                        }
                    }

                    if (anyVacOrMonPort)
                    {
                        chart.SecondaryAxes[1].Position = AxisPosition.Right;
                        Axis axisSecondaryAxes = chart.SecondaryAxes[1];
                        axisSecondaryAxes.Scaling.AutoMax = false;
                        axisSecondaryAxes.Scaling.Max = 1000;
                        axisSecondaryAxes.Scaling.AutoMin = false;
                        axisSecondaryAxes.Scaling.Min = -1000;
                        axisSecondaryAxes.MajorUnit = 100;
                        axisSecondaryAxes.MinorUnit = 20;
                    }

                    _workbook.EndUpdate();

                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                    return false;
                }
            });

            return result;
        }

        private async Task<bool> CreateTrendBagSheet()
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    NumericReportService numericReportService = new NumericReportService(_connectionString);
                    BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);
                    IEnumerable<Bag> bags = batchNumericReportModel.Bags;


                    // If there are no bags more than 1 then return
                    //if (bags == null || bags.ToList().Count < 2)
                    //    return false;

                    _workbook.BeginUpdate();

                    foreach (var bagItem in bags)
                    {
                        BagNumericReportModel bagNumericReportModel = numericReportService.NumericReportByBag(_batchId, bagItem.id, 1000000, 1);

                        _worksheet = _workbook.Worksheets.Add();
                        _worksheet.Name = $"TREND_{bagItem.BagName}";

                        _workbook.Worksheets.ActiveWorksheet = _worksheet;
                        _worksheet.Columns[0].WidthInCharacters = 2.0;

                        defaultHeader = _worksheet.Range["A1:G1"];

                        defaultHeader.Font.Name = "Arial Narrow";
                        defaultHeader.Font.Size = 20;
                        defaultHeader.Font.Bold = true;
                        defaultHeader.Merge();
                        defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                        defaultHeader = _worksheet.Range["A2:E2"];
                        defaultHeader.Font.Name = "Arial Narrow";
                        defaultHeader.Font.Size = 16;
                        defaultHeader.Font.Bold = true;
                        defaultHeader.Merge();
                        defaultHeader.Value = $"TREND {bagItem.BagName}";


                        _worksheet.Cells[2, 0].Value = "Batch Name";
                        _worksheet.Cells[3, 0].Value = "Bag Name";
                        _worksheet.Cells[4, 0].Value = "Soir Name";
                        _worksheet.Cells[5, 0].Value = "Part Name";
                        _worksheet.Cells[6, 0].Value = "Tool Name";
                        _worksheet.Cells[7, 0].Value = "Start Date";
                        _worksheet.Cells[8, 0].Value = "End Date";
                        _worksheet.Cells[9, 0].Value = "Recipe Name";

                        _worksheet.Cells[2, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                        _worksheet.Cells[3, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.BagNames;
                        _worksheet.Cells[4, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.SoirNames;
                        _worksheet.Cells[5, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.PartNames;
                        _worksheet.Cells[6, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.ToolNames;
                        _worksheet.Cells[7, 1].SetValue(bagNumericReportModel.NumericReportHeaderInfo.StartDate.ToString("dd.MM.yyyy HH:mm:ss"));
                        _worksheet.Cells[8, 1].SetValue(bagNumericReportModel.NumericReportHeaderInfo.EndDate.ToString("dd.MM.yyyy HH:mm:ss"));
                        _worksheet.Cells[9, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.RecipeName;

                        CellRange reportHeader = _worksheet.Range["A3:A10"];
                        reportHeader.AutoFitColumns();
                        reportHeader.FillColor = Color.LightGray;
                        reportHeader.Font.Bold = true;
                        reportHeader.Font.Name = "Arial Narrow";
                        reportHeader.Font.Size = 12;
                        reportHeader = _worksheet.Range["B3:B10"];
                        reportHeader.AutoFitColumns();
                        reportHeader.Font.Size = 12;


                        // Create a chart and specify its location.
                        Chart chart = _worksheet.Charts.Add(ChartType.Line);
                        chart.TopLeftCell = _worksheet.Cells["B14"];
                        chart.BottomRightCell = _worksheet.Cells["W60"];


                        Axis axis = chart.PrimaryAxes[1];
                        axis.Scaling.AutoMax = false;
                        axis.Scaling.Max = 300;
                        axis.Scaling.AutoMin = false;
                        axis.Scaling.Min = 0;
                        axis.MajorUnit = 50;
                        axis.MinorUnit = 20;



                        chart.PrimaryAxes[0].MajorTickMarks = AxisTickMarks.None;
                        chart.PrimaryAxes[0].TickLabelSkip = 60;


                        Dictionary<string, List<CellValue>> valuesByTagNamesSensorValue = new Dictionary<string, List<CellValue>>();
                        List<CellValue> arguments = new List<CellValue>();
                        List<CellValue> values = new List<CellValue>();
                        string currColumnName = string.Empty;

                        var rowList = bagNumericReportModel.NumericDataTable.Rows;
                        var columnList = bagNumericReportModel.NumericDataTable.Columns;

                        int columnCounter = 0;
                        int rowCounter = 0;

                        List<string> columns = new List<string>();
                        List<string> rows = new List<string>();

                        foreach (var item in columnList)
                        {
                            columns.Add(item.ToString());
                        }

                        foreach (var item in columns)
                        {
                            if (item != "Mins" && item != "Time")
                                valuesByTagNamesSensorValue.Add(item, new List<CellValue>());
                        }

                        foreach (var item in rowList)
                        {
                            DataRow dataRow = (DataRow)item;

                            rowCounter = 0;
                            columnCounter = 0;
                            foreach (var dataItem in dataRow.ItemArray)
                            {
                                string columnName = columns[columnCounter];

                                if (columnName == "Time")
                                {
                                    rowCounter++;
                                    columnCounter++;
                                    continue;
                                }

                                if (columnName == "Mins")
                                {
                                    // CellValue value = (dataRow[0] != DBNull.Value) ? Convert.ToUInt16(dataRow[0]) : default;
                                    CellValue value = Convert.ToDateTime(dataRow[1]).ToShortTimeString();
                                    arguments.Add(value);
                                }
                                else
                                {
                                    CellValue value = (dataRow[rowCounter] != DBNull.Value && !Double.IsNaN(Convert.ToDouble(dataRow[rowCounter]))) ? (float)Convert.ToDouble(dataRow[rowCounter]) : 0;
                                    valuesByTagNamesSensorValue[columnName].Add(value);
                                }

                                rowCounter++;
                                columnCounter++;
                            }
                        }

                        for (int j = 0; j < bagNumericReportModel.NumericDataTable.Columns.Count; j++)
                        {
                            currColumnName = bagNumericReportModel.NumericDataTable.Columns[j].Caption;

                            if (currColumnName == "Time" || currColumnName == "Mins")
                                continue;

                            chart.Series.Add(currColumnName,
                                            arguments.ToArray(),
                                            valuesByTagNamesSensorValue[currColumnName].ToArray());
                        }

                        bool anyVacOrMonPort = false;

                        // Use the secondary axis for vacuum ports
                        for (int i = 0; i < chart.Series.Count; i++)
                        {
                            if (chart.Series[i].SeriesName.PlainText.StartsWith("MON") || chart.Series[i].SeriesName.PlainText.StartsWith("VAC"))
                            {
                                if (!anyVacOrMonPort)
                                    anyVacOrMonPort = true;

                                chart.Series[i].AxisGroup = AxisGroup.Secondary;
                            }
                        }

                        if (anyVacOrMonPort)
                        {
                            chart.SecondaryAxes[1].Position = AxisPosition.Right;
                            Axis axisSecondaryAxes = chart.SecondaryAxes[1];
                            axisSecondaryAxes.Scaling.AutoMax = false;
                            axisSecondaryAxes.Scaling.Max = 1000;
                            axisSecondaryAxes.Scaling.AutoMin = false;
                            axisSecondaryAxes.Scaling.Min = -1000;
                            axisSecondaryAxes.MajorUnit = 100;
                            axisSecondaryAxes.MinorUnit = 20;
                        }

                        // Create Numeric bag section

                        if (bagNumericReportModel.NumericDataTable == null)
                            return false;

                        CellRange numeraicValueHeader = _worksheet.Range["A66:D66"];
                        numeraicValueHeader.FillColor = Color.LightGray;
                        numeraicValueHeader.Font.Bold = true;
                        numeraicValueHeader.Font.Name = "Arial Narrow";
                        numeraicValueHeader.Font.Size = 12;
                        numeraicValueHeader = _worksheet.Range["B3:B10"];
                        //numeraicValueHeader.AutoFitColumns();

                        _worksheet.Cells[65, 0].Value = "Name";
                        _worksheet.Cells[65, 1].Value = "Minimum";
                        _worksheet.Cells[65, 2].Value = "Maximum";
                        _worksheet.Cells[65, 3].Value = "Duration";


                        _worksheet.Cells[66, 0].Value = "Pressure PV";
                        _worksheet.Cells[67, 0].Value = "AirTc PV";
                        _worksheet.Cells[68, 0].Value = "HighTc";
                        _worksheet.Cells[69, 0].Value = "LowTc";



                        DateTime minDateTime = Convert.ToDateTime(bagNumericReportModel.NumericDataTable.Compute("min([Time])", string.Empty));
                        DateTime maxDateTime = Convert.ToDateTime(bagNumericReportModel.NumericDataTable.Compute("max([Time])", string.Empty));

                        TimeSpan timeSpan = maxDateTime - minDateTime;

                        _worksheet.Cells[66, 1].SetValue(bagNumericReportModel.NumericDataTable.Compute("min([Pressure_Actual])", string.Empty));
                        _worksheet.Cells[66, 2].SetValue(bagNumericReportModel.NumericDataTable.Compute("max([Pressure_Actual])", string.Empty));
                        _worksheet.Cells[67, 1].SetValue(bagNumericReportModel.NumericDataTable.Compute("min([Air_Tc])", string.Empty));
                        _worksheet.Cells[67, 2].SetValue(bagNumericReportModel.NumericDataTable.Compute("max([Air_Tc])", string.Empty));
                        _worksheet.Cells[68, 1].SetValue(bagNumericReportModel.NumericDataTable.Compute("min([High_Tc])", string.Empty));
                        _worksheet.Cells[68, 2].SetValue(bagNumericReportModel.NumericDataTable.Compute("max([High_Tc])", string.Empty));
                        _worksheet.Cells[69, 1].SetValue(bagNumericReportModel.NumericDataTable.Compute("min([Low_Tc])", string.Empty));
                        _worksheet.Cells[69, 2].SetValue(bagNumericReportModel.NumericDataTable.Compute("max([Low_Tc])", string.Empty));
                        _worksheet.Cells[69, 2].SetValue(bagNumericReportModel.NumericDataTable.Compute("max([Low_Tc])", string.Empty));
                        _worksheet.Cells[66, 3].SetValue($"{ timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}");
                        _worksheet.Cells[67, 3].SetValue($"{ timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}");
                        _worksheet.Cells[68, 3].SetValue($"{ timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}");
                        _worksheet.Cells[69, 3].SetValue($"{ timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}");

                        CellRange foreColor = _worksheet.Range["A67:D67"];
                        foreColor.Font.Color = Color.Blue;

                        foreColor = _worksheet.Range["A68:D68"];
                        foreColor.Font.Color = Color.Green;

                        foreColor = _worksheet.Range["A69:D69"];
                        foreColor.Font.Color = Color.Red;

                        foreColor = _worksheet.Range["A70:D70"];
                        foreColor.Font.Color = Color.Black;

                    }


                    _workbook.EndUpdate();
                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                    return false;
                }
            });

            return result;
        }

        private async Task<bool> CreateNumericBagSheet()
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    NumericReportService numericReportService = new NumericReportService(_connectionString);
                    BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_batchId);
                    DisabledPortService _disabledPort = new DisabledPortService(_connectionString);


                    System.Data.DataTable integratedCheckResultsDataTable = new System.Data.DataTable();
                    DataConverter dataConverter = new DataConverter();
                    IEnumerable<Bag> bags = batchNumericReportModel.Bags;

                    Dictionary<DateTime, List<string>> disablePortByBag = _disabledPort.GetByBatchGroupedByReceivedDateWithPortName(_batchId);


                    foreach (var bagItem in bags)
                    {
                        _worksheet = _workbook.Worksheets.Add();
                        _worksheet.Name = bagItem.BagName;
                        _workbook.BeginUpdate();
                        _workbook.Worksheets.ActiveWorksheet = _worksheet;

                        BagNumericReportModel bagNumericReportModel = numericReportService.NumericReportByBag(_batchId, bagItem.id, 1000000, 1);
                        integratedCheckResultsDataTable = dataConverter.ConvertToDataTable(bagNumericReportModel.IntegratedCheckReportItems);


                        bagNumericReportModel.NumericDataTable = ReportHeaderSortOrder(bagNumericReportModel.NumericDataTable);

                        if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId != 20)
                        { bagNumericReportModel.NumericDataTable.Columns.Remove("Segment_No"); }
                        #region Report Header
                        defaultHeader = _worksheet.Range["A1:E1"];

                        defaultHeader.Font.Name = "Arial Narrow";
                        defaultHeader.Font.Size = 20;
                        defaultHeader.Font.Bold = true;
                        defaultHeader.Merge();
                        defaultHeader.Value = ApplicationConfigurations.Instance.Configuration.ReportHeaderTitleRoot;

                        defaultHeader = _worksheet.Range["A2:E2"];
                        defaultHeader.Font.Name = "Arial Narrow";
                        defaultHeader.Font.Size = 16;
                        defaultHeader.Font.Bold = true;
                        defaultHeader.Merge();
                        defaultHeader.Value = bagItem.BagName;

                        _worksheet.Cells[2, 0].Value = "Batch Name";
                        _worksheet.Cells[3, 0].Value = "Bag Name";
                        _worksheet.Cells[4, 0].Value = "Soir Name";
                        _worksheet.Cells[5, 0].Value = "Part Name";
                        _worksheet.Cells[6, 0].Value = "Tool Name";
                        _worksheet.Cells[7, 0].Value = "Start Date";
                        _worksheet.Cells[8, 0].Value = "End Date";
                        _worksheet.Cells[9, 0].Value = "Recipe Name";

                        _worksheet.Cells[2, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                        _worksheet.Cells[3, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.BagNames;
                        _worksheet.Cells[4, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.SoirNames;
                        _worksheet.Cells[5, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.PartNames;
                        _worksheet.Cells[6, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.ToolNames;
                        _worksheet.Cells[7, 1].SetValue(bagNumericReportModel.NumericReportHeaderInfo.StartDate.ToString("dd.MM.yyyy HH:mm:ss"));
                        _worksheet.Cells[8, 1].SetValue(bagNumericReportModel.NumericReportHeaderInfo.EndDate.ToString("dd.MM.yyyy HH:mm:ss"));
                        _worksheet.Cells[9, 1].Value = bagNumericReportModel.NumericReportHeaderInfo.RecipeName;

                        CellRange reportHeader = _worksheet.Range["A3:A10"];
                        reportHeader.AutoFitColumns();
                        reportHeader.FillColor = Color.LightGray;
                        reportHeader.Font.Bold = true;
                        reportHeader.Font.Name = "Arial Narrow";
                        reportHeader.Font.Size = 12;
                        reportHeader = _worksheet.Range["B3:B10"];
                        reportHeader.Font.Size = 12;
                        reportHeader.AutoFitColumns();

                        CellRange detailReportHeader = _worksheet.Range["C3:D10"];
                        detailReportHeader.AutoFitColumns();
                        detailReportHeader.Font.Name = "Arial Narrow";
                        detailReportHeader.Font.Size = 12;

                        #endregion End of Report Header

                        #region Integrity Check
                        int integrityCheckRowCount = 0;

                        if (integratedCheckResultsDataTable == null)
                        {
                            integrityCheckRowCount = 13;
                        }
                        else
                        {
                            integrityCheckRowCount = 13 + integratedCheckResultsDataTable.Rows.Count;
                        }

                        if (integratedCheckResultsDataTable != null)
                        {
                            CellRange integrityReport;

                            for (int i = 0; i < integratedCheckResultsDataTable.Columns.Count; i++)
                            {
                                StringManipulation stringManipulation = new StringManipulation();
                                _worksheet.Cells[12, i].Value = stringManipulation.SeperatePascalCaseString(integratedCheckResultsDataTable.Columns[i].Caption);

                                integrityReport = _worksheet.Range.Union(_worksheet.Cells[12, i], _worksheet.Cells[12, integratedCheckResultsDataTable.Columns.Count]);
                                integrityReport.FillColor = Color.LightGray;
                                integrityReport.Font.Bold = true;
                                integrityReport.AutoFitColumns();
                                integrityReport.Font.Name = "Arial Narrow";
                                integrityReport.Font.Size = 10;
                            }
                            for (int i = 0; i < integratedCheckResultsDataTable.Rows.Count; i++)
                            {
                                for (int j = 0; j < integratedCheckResultsDataTable.Columns.Count; j++)
                                {
                                    //_worksheet.Cells[13 + i, j].Value = integratedCheckResultsDataTable.Rows[i][j].ToString().Replace(",", ".");
                                    _worksheet.Cells[13 + i, j].SetValue(integratedCheckResultsDataTable.Rows[i][j]);
                                    // _worksheet.Cells[13 + i, j].NumberFormat = "#,#";
                                }
                            }
                        }
                        else
                        {
                            _worksheet.Cells[11, 0].Value = "Skipped Date";
                            CellRange skippedIntegrityCheckHeader = _worksheet.Range["A12:E12"];
                            skippedIntegrityCheckHeader.AutoFitColumns();
                            skippedIntegrityCheckHeader.FillColor = Color.LightGray;
                            skippedIntegrityCheckHeader.Font.Bold = true;
                            skippedIntegrityCheckHeader.Font.Name = "Arial Narrow";
                            skippedIntegrityCheckHeader.Font.Size = 12;

                            CellRange skippedIntegrityCheckValue = _worksheet.Range["A13:C13"];
                            if (bagNumericReportModel.SkippedIntegratedCheckReportItem == null)
                            {
                                _worksheet.Cells[12, 0].Value = DateTime.Now;
                            }
                            else
                            {
                                _worksheet.Cells[12, 0].Value = bagNumericReportModel.SkippedIntegratedCheckReportItem.SkipDate;
                            }
                        }
                        #endregion End of Integrity Check

                        if (bagNumericReportModel.NumericDataTable == null)
                            return false;

                        CellRange numericReport;

                        for (int i = 0; i < bagNumericReportModel.NumericDataTable.Columns.Count; i++)
                        {
                            _worksheet.Cells[integrityCheckRowCount, i].Value = bagNumericReportModel.NumericDataTable.Columns[i].Caption;

                            numericReport = _worksheet.Range.Union(_worksheet.Cells[integrityCheckRowCount, i], _worksheet.Cells[integrityCheckRowCount, bagNumericReportModel.NumericDataTable.Columns.Count]);
                            numericReport.FillColor = Color.LightGray;
                            numericReport.Font.Bold = true;
                            /// numericReport.AutoFitColumns();
                            numericReport.Font.Name = "Arial Narrow";
                            numericReport.Font.Size = 10;
                        }
                        for (int i = 0; i < bagNumericReportModel.NumericDataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < bagNumericReportModel.NumericDataTable.Columns.Count; j++)
                            {
                                //_worksheet.Cells[1 + integrityCheckRowCount + i, j].SetValue(bagNumericReportModel.NumericDataTable.Rows[i][j]);


                                var disableEnablePortResult = disablePortByBag.Where(x => x.Key == Convert.ToDateTime(bagNumericReportModel.NumericDataTable.Rows[i][0]) && x.Value.Contains(bagNumericReportModel.NumericDataTable.Columns[j].ColumnName)).Select(x => x.Key == Convert.ToDateTime(bagNumericReportModel.NumericDataTable.Rows[i][0]) && x.Value.Contains(bagNumericReportModel.NumericDataTable.Columns[j].ColumnName)).FirstOrDefault();

                                if (disableEnablePortResult)
                                {
                                    _worksheet.Cells[1 + integrityCheckRowCount + i, j].SetValue($"[D]{String.Format("{0:0.0}", bagNumericReportModel.NumericDataTable.Rows[i][j])}");
                                }
                                else
                                {
                                    _worksheet.Cells[1 + integrityCheckRowCount + i, j].SetValue(bagNumericReportModel.NumericDataTable.Rows[i][j]);

                                }

                                if (bagNumericReportModel.NumericDataTable.Columns[j].ColumnName != "Time" && !bagNumericReportModel.NumericDataTable.Columns[j].ColumnName.Contains("Press") && !_worksheet.Cells[1 + integrityCheckRowCount + i, j].ToString().Contains("[D]"))
                                {
                                    _worksheet.Cells[1 + integrityCheckRowCount + i, j].NumberFormat = "#0.0";
                                }
                                else if (batchNumericReportModel.NumericDataTable.Columns[j].ColumnName.Contains("Press"))
                                {
                                    _worksheet.Cells[1 + integrityCheckRowCount + i, j].NumberFormat = "#0.00";
                                }
                                else if (bagNumericReportModel.NumericDataTable.Columns[j].ColumnName == "Time")
                                {
                                    _worksheet.Cells[1 + integrityCheckRowCount + i, j].NumberFormat = "HH:mm:ss";
                                }
                            }
                        }
                    }
                    _workbook.EndUpdate();
                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                    return false;
                }
            });

            return result;
        }

        private void MakeSheetsReadOnly()
        {
            for (int i = 0; i < _workbook.Worksheets.Count; i++)
            {
                _workbook.Worksheets[i]["$A:$XFD"].Protection.Locked = true; // Lock the entire document
                _workbook.Worksheets[i].Protect(ExcelFilePassword, WorksheetProtectionPermissions.Default);
            }
        }

        private bool WorksheetSave()
        {
            _worksheet = (Worksheet)_workbook.Worksheets[1];
            MakeSheetsReadOnly();

            // Check if this file is already located at the destination.
            if (File.Exists(ExcelFileFullPath))
            {
                MessageBoxResult messageBoxResult = WinUIMessageBox.Show($"A file named \"{ExcelFileFullPath}\" already exists in this location. Do you want to replace it?\n" +
                                                                         $"(Bu dosyadan zaten var bunu değiştirmek istiyor musunuz?)", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return false;
                }
            }

            try
            {
                _workbook.DocumentProperties.Security = DocumentSecurity.ReadonlyRecommended;
                _workbook.SaveDocument(ExcelFileFullPath, DocumentFormat.OpenXml);


            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Excel Export Error: \n {ex}", LogType.Error);
                throw;
            }
            return true;
        }


        private DataTable ReportHeaderSortOrder(DataTable numericReportTable)
        {
            try
            {
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                var excelReportHeaderSortOrder = applicationPropertyService.GetByName("ExcelReportHeaderSortOrder")?.Value ?? string.Empty;
                Dictionary<string, int> headerSortOrder = new Dictionary<string, int>();
                headerSortOrder = JsonConvert.DeserializeObject<Dictionary<string, int>>(excelReportHeaderSortOrder).ToDictionary(t => t.Key, t => t.Value);

                List<int> group1 = headerSortOrder.Where(t => t.Value == 0).Select(t => t.Value).ToList();
                List<int> index = headerSortOrder.Select(t => t.Value).ToList();

                int fieldOrderIndex = 0;
                foreach (var columnNameOrder in headerSortOrder)
                {
                    switch (columnNameOrder.Value)
                    {
                        case 0:
                        case 2:
                        case 4:
                        case 5:
                            if (numericReportTable.Columns.Contains(columnNameOrder.Key))
                            {
                                numericReportTable.Columns[columnNameOrder.Key].SetOrdinal(fieldOrderIndex);
                                fieldOrderIndex++;
                            }
                            break;
                        case 1:

                            string[] monNames = numericReportTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(n => n.Contains("MON")).OrderBy(x => x).ToArray();
                            foreach (var item in monNames)
                            {
                                numericReportTable.Columns[item].SetOrdinal(fieldOrderIndex);
                                fieldOrderIndex++;
                            }
                            break;
                        case 3:

                            string[] ptcNames = numericReportTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(n => n.Contains("PTC")).OrderBy(x => x).ToArray();
                            foreach (var item in ptcNames)
                            {
                                numericReportTable.Columns[item].SetOrdinal(fieldOrderIndex);
                                fieldOrderIndex++;
                            }
                            break;
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return numericReportTable;
        }
    }
}