using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.WindowsUI;
using RevoScada.Business;
using RevoScada.Business.Report;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.DesktopApplication.ViewModels.TrendViewModels;
using RevoScada.DesktopApplication.Views.ReportTemplates;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using RevoScada.ProcessController;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using WpfAnimatedGif;

namespace RevoScada.DesktopApplication.Views.TrendViews
{
    /// <summary>
    /// Interaction logic for Trend_Window_Print.xaml
    /// </summary>
    public partial class Trend_Window_Print : Window
    {
        #region Services
        private TrendReportService _trendReportService;
        #endregion

        #region Collections
        public Dictionary<string, List<float>> TrendValuesByTagNames;
        public List<DateTime> TrendDateTimeValues;
        public List<float> Mins;
        #endregion

        #region Fields
        private readonly string _connectionString;
        private ReportsVM _reportsVM;
        public List<string> SelectedSeriesNames;
        private TrendPrintVM _viewModel;
        #endregion

        #region Properties
        private ReportExportSettings ReportExportSettings;
        #endregion

        public Trend_Window_Print(ReportsVM reportsVM, ReportExportSettings reportExportSettings, List<string> selectedSeriesNames)
        {
            InitializeComponent();

            _viewModel = DataContext as TrendPrintVM;
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            _trendReportService = new TrendReportService(_connectionString);
            _reportsVM = reportsVM;
            ReportExportSettings = reportExportSettings;

            TrendValuesByTagNames = new Dictionary<string, List<float>>();
            TrendDateTimeValues = new List<DateTime>();
            Mins = new List<float>();
            SelectedSeriesNames = selectedSeriesNames;

            SetAxisYMinMaxValues();
            StartGettingData();
        }

        private void SetAxisYMinMaxValues()
        {
            trendXyDiagram2d_temp.AxisY.WholeRange = new Range();
            trendXyDiagram2d_temp.AxisY.WholeRange.MinValue = _viewModel.TrendChartYAxisParamaters.TemperatureParamaterMin;
            trendXyDiagram2d_temp.AxisY.WholeRange.MaxValue = _viewModel.TrendChartYAxisParamaters.TemperatureParamaterMax;
            trendXyDiagram2d_temp.AxisY.WholeRange.SideMarginsValue = 0;

            trendXyDiagram2d_vacuum.WholeRange = new Range();
            trendXyDiagram2d_vacuum.WholeRange.MinValue = _viewModel.TrendChartYAxisParamaters.VacuumParamaterMin;
            trendXyDiagram2d_vacuum.WholeRange.MaxValue = _viewModel.TrendChartYAxisParamaters.VacuumParamaterMax;
            trendXyDiagram2d_vacuum.WholeRange.SideMarginsValue = 0;

            trendXyDiagram2d_pressure.WholeRange = new Range();
            trendXyDiagram2d_pressure.WholeRange.MinValue = _viewModel.TrendChartYAxisParamaters.PressureParamaterMin;
            trendXyDiagram2d_pressure.WholeRange.MaxValue = _viewModel.TrendChartYAxisParamaters.PressureParamaterMax;
            trendXyDiagram2d_pressure.WholeRange.SideMarginsValue = 0;
        }

        private async void StartGettingData(DataTable readyTrendDataTable = null)
        {
            if(readyTrendDataTable == null)
                 await GetTrendDataAsync();
            else
                await GetTrendDataAsync(readyTrendDataTable);

            await CreateChartDataAsync();

            if (readyTrendDataTable == null)
                await PrintTrendData(this);
        }

        private async Task<int> GetTrendDataAsync(DataTable readyTrendDataTable = null)
        {
            DataTable trendDataTable = new DataTable();
            await Task.Run(() =>
            {
                TrendValuesByTagNames.Clear();
                Mins.Clear();

                if (readyTrendDataTable == null)
                    trendDataTable = _trendReportService.BatchNumericReport(_reportsVM.SelectedBatchId);
                else
                    trendDataTable = readyTrendDataTable;

                if (trendDataTable == null)
                    return;

                var rowList = trendDataTable.Rows;
                var columnList = trendDataTable.Columns;

                List<string> columns = new List<string>();
                List<string> rows = new List<string>();

                foreach (var item in columnList)
                {
                    columns.Add(item.ToString());
                }

                foreach (var item in columns)
                {
                    if (item != "Time" && item != "Mins")
                        TrendValuesByTagNames.Add(item, new List<float>());
                }

                int columnCounter = 0;
                int rowCounter = 0;

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
                            DateTime value = (dataRow[1] != DBNull.Value) ? Convert.ToDateTime(dataRow[1]) : default;
                            TrendDateTimeValues.Add(value);
                            rowCounter++;
                            columnCounter++;
                            continue;
                        }

                        if (columnName == "Mins")
                        {
                            float value = (dataRow[0] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[0]) : default;
                            Mins.Add(value);
                        }
                        else
                        {
                            float value = (dataRow[rowCounter] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[rowCounter]) : default;
                            TrendValuesByTagNames[columnName].Add(value);
                        }

                        rowCounter++;
                        columnCounter++;
                    }
                }
            });

            return trendDataTable?.Rows.Count ?? 0;
        }

        public async Task CreateChartDataAsync()
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    int chartLength = Mins.Count;
                    var seriesNames = TrendValuesByTagNames.Keys.ToArray();
                    int totalLineSeries = seriesNames.Count();
                    List<string> addedSeriesDisplayNames = new List<string>();

                    trendXyDiagram2d_temp.Series.Clear();

                    // Create line series
                    for (int i = 0; i < totalLineSeries; i++)
                    {
                        ObservableCollection<TrendModel> lineSeriesData = new ObservableCollection<TrendModel>();
                        string keyName = seriesNames[i];

                        if (!SelectedSeriesNames.Contains(keyName) && SelectedSeriesNames.Count() != 0)
                            continue;

                        for (int j = 0; j < chartLength; j++)
                        {
                            TrendModel trendModel = new TrendModel();
                            trendModel.Minute = Mins[j];

                            float value = 0;

                            if (keyName == "Mins")
                            {
                                value = Mins[j];
                            }
                            else
                            {
                                if (j < TrendValuesByTagNames[keyName].Count)
                                {
                                    value = TrendValuesByTagNames[keyName][j];
                                }
                            }

                            if (j < TrendDateTimeValues.Count)
                                trendModel.Date = TrendDateTimeValues[j];

                            trendModel.YVal = value;
                            lineSeriesData.Add(trendModel);
                        }

                        SplineSeries2D lineSeries = new SplineSeries2D();
                        lineSeries.DisplayName = keyName;

                        lineSeries.DataSource = lineSeriesData;
                        lineSeries.ArgumentDataMember = "Date";
                        lineSeries.ValueDataMember = "YVal";
                        lineSeries.LineTension = 1;
                        lineSeries.LineStyle = new LineStyle() { Thickness = 3 };

                        if (!addedSeriesDisplayNames.Contains(keyName))
                        {
                            var displayNameVal = Regex.Replace(lineSeries.DisplayName, @"[^A-Za-z_]+", String.Empty);

                            if (_viewModel.VacPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                            {
                                XYDiagram2D.SetSeriesAxisY(lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[0]);
                                trendXyDiagram2d_temp.Series.Add(lineSeries);
                            }
                            else if (_viewModel.PressPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                            {
                                XYDiagram2D.SetSeriesAxisY(lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[1]);
                                trendXyDiagram2d_temp.Series.Add(lineSeries);
                            }
                            else 
                            trendXyDiagram2d_temp.Series.Add(lineSeries);

                            addedSeriesDisplayNames.Add(keyName);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="bagNames"></param>
        /// <returns>A format which looks like this: Bag 1, Bag 2</returns>
        private string FixBagNames(string bagNames)
        {
            if (string.IsNullOrEmpty(bagNames))
                return string.Empty;

            string[] splittedBagNames = bagNames.Replace(",", "").Replace(" ", "-").Split('-').ToArray();
            string fixedBagNames = string.Empty;
            short counter = 1;
            foreach (var item in splittedBagNames)
            {
                if (item.StartsWith("Bag"))
                {
                    var resultString = Regex.Match(item, @"\d+").Value;
                    fixedBagNames += $"Bag {resultString}";

                    if (splittedBagNames.Length != counter)
                        fixedBagNames += ", ";
                }
                counter++;
            }

            return fixedBagNames;
        }

        /// <summary>
        /// Precaution taken to avoid exceptions and Compute method of table used to calculate both min and max values.
        /// First index of array represents minimum value of the desired numeric value,
        /// second one maximum value of the numeric value.
        /// </summary>
        /// <param name="bagNumericReportModel">Bag based numeric model</param>
        /// <param name="colName">Column name of numeric data</param>
        /// <returns>A float based array</returns>
        private float[] GetMinMaxValueOfNumericData(BagNumericReportModel bagNumericReportModel, string colName)
        {
            string minValSt = bagNumericReportModel.NumericDataTable.Compute($"min([{colName}])", string.Empty)?.ToString() ?? string.Empty;
            string maxValSt = bagNumericReportModel.NumericDataTable.Compute($"max([{colName}])", string.Empty)?.ToString() ?? string.Empty;
            float minVal;
            float maxVal;

            if (!string.IsNullOrEmpty(minValSt))
                minVal = (float)Convert.ToDouble(minValSt);
            else
                minVal = 0;

            if (!string.IsNullOrEmpty(maxValSt))
                maxVal = (float)Convert.ToDouble(maxValSt);
            else
                maxVal = 0;

            return new float[] { minVal, maxVal };
        }

        #region Printing logic starts here
        public async Task<bool> PrintTrendData(FrameworkElement element)
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    string printFileName = $"{ReportExportSettings.ExcelExportFileNameBase }" +
                                           $"_{ DateTime.Now:yyyy-MM-dd HH-mm-ss}" +
                                           $"_{_reportsVM.SelectedBatch.LoadNumber}" + "_Trend";

                    string formattedPrintFileName = System.IO.Path.Combine(ReportExportSettings.ExcelExportFilePath, printFileName) + ".xps";

                    XpsDocument doc = new XpsDocument(formattedPrintFileName, FileAccess.ReadWrite);
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    SerializerWriterCollator previewDocument = writer.CreateVisualsCollator();

                    // Start creating pages
                    NumericReportService numericReportService = new NumericReportService(_connectionString);
                    BatchNumericReportModel batchNumericReportModel = numericReportService.BatchNumericReport(_reportsVM.SelectedBatchId);

                    // Fill header info section
                    _viewModel.BatchName = batchNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                    _viewModel.BagName = FixBagNames(batchNumericReportModel.NumericReportHeaderInfo.BagNames);
                    _viewModel.SoirName = batchNumericReportModel.NumericReportHeaderInfo.SoirNames;
                    _viewModel.PartName = batchNumericReportModel.NumericReportHeaderInfo.PartNames;
                    _viewModel.ToolName = batchNumericReportModel.NumericReportHeaderInfo.ToolNames;
                    _viewModel.StartDate = batchNumericReportModel.NumericReportHeaderInfo.StartDate.ToString("dd.MM.yyyy HH:mm:ss");
                    _viewModel.EndDate = batchNumericReportModel.NumericReportHeaderInfo.EndDate.ToString("dd.MM.yyyy HH:mm:ss");
                    _viewModel.RecipeName = batchNumericReportModel.NumericReportHeaderInfo.RecipeName;

                    dxTrendChart.UpdateLayout();

                    previewDocument.BeginBatchWrite();
                    previewDocument.Write(element);

                    IEnumerable<Bag> bags = batchNumericReportModel.Bags;

                    foreach (var bagItem in bags)
                    {
                        TrendValuesByTagNames = new Dictionary<string, List<float>>();
                        TrendDateTimeValues = new List<DateTime>();
                        Mins = new List<float>();
                        _viewModel.SeriesDetails.Clear();
                        List<string> SelectedSeriesNamesByBags = new List<string>();

                        BagNumericReportModel bagNumericReportModel = numericReportService.NumericReportByBag(_reportsVM.SelectedBatchId, bagItem.id, 1000000, 1);

                        if(bagNumericReportModel.NumericDataTable == null)
                        {
                            WinUIMessageBox.Show("No data found to print. (Yazdırılacak veri bulunamadı.)", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            this.Close();
                            return;
                        }

                        // Fill header info section by bags
                        _viewModel.BatchName = bagNumericReportModel.NumericReportHeaderInfo.LoadNumber;
                        _viewModel.BagName = FixBagNames(bagNumericReportModel.NumericReportHeaderInfo.BagNames);
                        _viewModel.SoirName = bagNumericReportModel.NumericReportHeaderInfo.SoirNames;
                        _viewModel.PartName = bagNumericReportModel.NumericReportHeaderInfo.PartNames;
                        _viewModel.ToolName = bagNumericReportModel.NumericReportHeaderInfo.ToolNames;
                        _viewModel.StartDate = bagNumericReportModel.NumericReportHeaderInfo.StartDate.ToString("dd.MM.yyyy HH:mm:ss");
                        _viewModel.EndDate = bagNumericReportModel.NumericReportHeaderInfo.EndDate.ToString("dd.MM.yyyy HH:mm:ss");
                        _viewModel.RecipeName = bagNumericReportModel.NumericReportHeaderInfo.RecipeName;

                        // Fill series detail grid
                        if (seriesDetailsGrid.Visibility == Visibility.Hidden)
                            seriesDetailsGrid.Visibility = Visibility.Visible;

                        TimeSpan timeSpan = bagNumericReportModel.NumericReportHeaderInfo.EndDate - bagNumericReportModel.NumericReportHeaderInfo.StartDate;
                        string formattedTimeSpan = timeSpan.ToString(@"hh\:mm\:ss");

                        SeriesDetailModel pressActualSeries = new SeriesDetailModel()
                        {
                            Name = "Pressure PV",
                            DisplayName = "Pressure PV (bar)",
                            Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, "Pressure_Actual")[0],
                            Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, "Pressure_Actual")[1],
                            Duration = formattedTimeSpan,
                            Type = "Pressure PV"
                        };

                        SeriesDetailModel airTcSeries = new SeriesDetailModel()
                        {
                            Name = "Air TC PV",
                            DisplayName = "Air TC PV °C",
                            Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, "Air_Tc")[0],
                            Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, "Air_Tc")[1],
                            Duration = formattedTimeSpan,
                            Type = "Air TC PV"
                        };

                        SeriesDetailModel highTcSeries = new SeriesDetailModel()
                        {
                            Name = "High TC",
                            DisplayName = "High TC °C",
                            Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, "High_Tc")[0],
                            Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, "High_Tc")[1],
                            Duration = formattedTimeSpan,
                            Type = "High TC"
                        };

                        SeriesDetailModel lowTcSeries = new SeriesDetailModel()
                        {
                            Name = "Low TC",
                            DisplayName = "Low TC °C",
                            Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, "Low_Tc")[0],
                            Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, "Low_Tc")[1],
                            Duration = formattedTimeSpan,
                            Type = "Low TC"
                        };

                        _viewModel.SeriesDetails.Add(pressActualSeries);
                        _viewModel.SeriesDetails.Add(airTcSeries);
                        _viewModel.SeriesDetails.Add(highTcSeries);
                        _viewModel.SeriesDetails.Add(lowTcSeries);
                        SelectedSeriesNamesByBags.Add("Pressure_Actual");
                        SelectedSeriesNamesByBags.Add("Air_Tc");
                        SelectedSeriesNamesByBags.Add("High_Tc");
                        SelectedSeriesNamesByBags.Add("Low_Tc");

                        DataTable trendDataTable = new DataTable();
                        trendDataTable = bagNumericReportModel.NumericDataTable;

                        var rowList = trendDataTable.Rows;
                        var columnList = trendDataTable.Columns;

                        List<string> columns = new List<string>();
                        List<string> rows = new List<string>();

                        foreach (var item in columnList)
                        {
                            columns.Add(item.ToString());
                        }

                        // Get first values of PTC and MON ports
                        var ptcColumns = columns.Where(c => c.StartsWith("PTC")).OrderBy(c => Regex.Replace(c, @"[^A-Z]+", "")).ToList();
                        var monColumns = columns.Where(c => c.StartsWith("MON")).OrderBy(c => Regex.Replace(c, @"[^A-Z]+", "")).ToList();

                        if(ptcColumns.Count() > 0)
                        {
                            SeriesDetailModel firstPtcSeries = new SeriesDetailModel()
                            {
                                Name = $"{ptcColumns[0]}",
                                DisplayName = $"{ptcColumns[0]} °C",
                                Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, ptcColumns[0])[0],
                                Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, ptcColumns[0])[1],
                                Duration = formattedTimeSpan,
                                Type = "PTC"
                            };
                            _viewModel.SeriesDetails.Add(firstPtcSeries);
                            SelectedSeriesNamesByBags.Add(firstPtcSeries.Name);
                        }

                        if (ptcColumns.Count() > 1)
                        {
                            SeriesDetailModel secondPtcSeries = new SeriesDetailModel()
                            {
                                Name = $"{ptcColumns[1]}",
                                DisplayName = $"{ptcColumns[1]} °C",
                                Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, ptcColumns[1])[0],
                                Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, ptcColumns[1])[1],
                                Duration = formattedTimeSpan,
                                Type = "PTC"
                            };
                            _viewModel.SeriesDetails.Add(secondPtcSeries);
                            SelectedSeriesNamesByBags.Add(secondPtcSeries.Name);
                        }

                        if (monColumns.Count() > 0)
                        {
                            SeriesDetailModel firstMonSeries = new SeriesDetailModel()
                            {
                                Name = $"{monColumns[0]}",
                                DisplayName = $"{monColumns[0]} (mbar)",
                                Minimum = GetMinMaxValueOfNumericData(bagNumericReportModel, monColumns[0])[0],
                                Maximum = GetMinMaxValueOfNumericData(bagNumericReportModel, monColumns[0])[1],
                                Duration = formattedTimeSpan,
                                Type = "MON"
                            };
                            _viewModel.SeriesDetails.Add(firstMonSeries);
                            SelectedSeriesNamesByBags.Add(firstMonSeries.Name);
                        }

                        // todo:h Refactor these two sections ASAP
                        #region Get Trend Data
                            TrendValuesByTagNames.Clear();
                            Mins.Clear();

                            foreach (var item in columns)
                            {
                                if (item != "Time" && item != "Mins")
                                    TrendValuesByTagNames.Add(item, new List<float>());
                            }

                            int columnCounter = 0;
                            int rowCounter = 0;

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
                                        DateTime value = (dataRow[1] != DBNull.Value) ? Convert.ToDateTime(dataRow[1]) : default;
                                        TrendDateTimeValues.Add(value);
                                        rowCounter++;
                                        columnCounter++;
                                        continue;
                                    }

                                    if (columnName == "Mins")
                                    {
                                        float value = (dataRow[0] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[0]) : default;
                                        Mins.Add(value);
                                    }
                                    else
                                    {
                                        float value = (dataRow[rowCounter] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[rowCounter]) : default;
                                        TrendValuesByTagNames[columnName].Add(value);
                                    }

                                    rowCounter++;
                                    columnCounter++;
                                }
                            }
                        #endregion
                        #region Create chart
                        int chartLength = Mins.Count;
                        var seriesNames = TrendValuesByTagNames.Keys.ToArray();
                        int totalLineSeries = seriesNames.Count();
                        List<string> addedSeriesDisplayNames = new List<string>();

                        trendXyDiagram2d_temp.Series.Clear();

                        // Create line series
                        for (int i = 0; i < totalLineSeries; i++)
                        {
                            ObservableCollection<TrendModel> lineSeriesData = new ObservableCollection<TrendModel>();
                            string keyName = seriesNames[i];

                            if (!SelectedSeriesNamesByBags.Contains(keyName) && SelectedSeriesNamesByBags.Count() != 0)
                                continue;

                            for (int j = 0; j < chartLength; j++)
                            {
                                TrendModel trendModel = new TrendModel();
                                trendModel.Minute = Mins[j];

                                float value = 0;

                                if (keyName == "Mins")
                                {
                                    value = Mins[j];
                                }
                                else
                                {
                                    if (j < TrendValuesByTagNames[keyName].Count)
                                    {
                                        value = TrendValuesByTagNames[keyName][j];
                                    }
                                }

                                if (j < TrendDateTimeValues.Count)
                                    trendModel.Date = TrendDateTimeValues[j];

                                trendModel.YVal = value;
                                lineSeriesData.Add(trendModel);
                            }

                            SplineSeries2D lineSeries = new SplineSeries2D();
                            lineSeries.DisplayName = keyName;

                            lineSeries.DataSource = lineSeriesData;
                            lineSeries.ArgumentDataMember = "Date";
                            lineSeries.ValueDataMember = "YVal";
                            lineSeries.LineTension = 1;
                            lineSeries.LineStyle = new LineStyle() { Thickness = 3 };

                            if (!addedSeriesDisplayNames.Contains(keyName))
                            {
                                var displayNameVal = Regex.Replace(lineSeries.DisplayName, @"[^A-Za-z_]+", String.Empty);

                                if (_viewModel.VacPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                                {
                                    XYDiagram2D.SetSeriesAxisY(lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[0]);
                                    trendXyDiagram2d_temp.Series.Add(lineSeries);
                                }
                                else if (_viewModel.PressPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                                {
                                    XYDiagram2D.SetSeriesAxisY(lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[1]);
                                    trendXyDiagram2d_temp.Series.Add(lineSeries);
                                }
                                else
                                    trendXyDiagram2d_temp.Series.Add(lineSeries);

                                addedSeriesDisplayNames.Add(keyName);
                            }
                        }
                        #endregion

                        dxTrendChart.UpdateLayout();
                        element.UpdateLayout();

                        previewDocument.Write(element);
                    }

                    previewDocument.EndBatchWrite();

                    FixedDocumentSequence preview = doc.GetFixedDocumentSequence();

                    Print_Window printWindow = new Print_Window(preview);
                    printWindow.ShowDialog();

                    doc.Close();
                    this.Close();
                });
            });

            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var logoImageFromConfiguration = new BitmapImage();
            logoImageFromConfiguration.BeginInit();
            logoImageFromConfiguration.UriSource = new Uri(ApplicationConfigurations.Instance.Configuration.ReportHeaderLogoImage);
            logoImageFromConfiguration.EndInit();
            ImageBehavior.SetAnimatedSource(ApplicationLogo, logoImageFromConfiguration);
        }

        #endregion
    }
}
