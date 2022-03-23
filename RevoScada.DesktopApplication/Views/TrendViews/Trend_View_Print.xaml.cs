using DevExpress.Xpf.Charts;
using RevoScada.Business.Report;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.TrendViews
{
    /// <summary>
    /// Interaction logic for Trend_View_Print.xaml
    /// </summary>
    public partial class Trend_View_Print : UserControl
    {
        #region Collections
        public Dictionary<string, List<float>> TrendValuesByTagNames;
        public List<DateTime> TrendDateTimeValues;
        public List<ushort> Mins;
        private Dictionary<string, ObservableCollection<TrendModel>> _seriesDataSource;
        private SeriesCollection _lineSeriesCollection;
        #endregion

        #region Fields
        private TrendReportService _trendReportService;
        private int _selectedBatchId;
        #endregion

        public Trend_View_Print(int selectedBatchId)
        {
            InitializeComponent();
             var connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _trendReportService = new TrendReportService(connectionString);
            _selectedBatchId = selectedBatchId;

            _lineSeriesCollection = new SeriesCollection();
            _seriesDataSource = new Dictionary<string, ObservableCollection<TrendModel>>();

            TrendValuesByTagNames = new Dictionary<string, List<float>>();
            TrendDateTimeValues = new List<DateTime>();
            Mins = new List<ushort>();

            StartGettingData();
        }

        private async void StartGettingData()
        {
            await GetTrendDataAsync();
            await CreateChartData();
        }

        private async Task<int> GetTrendDataAsync()
        {
            DataTable trendDataTable = new DataTable();
            await Task.Run(() =>
            {
                TrendValuesByTagNames.Clear();
                Mins.Clear();
                trendDataTable = _trendReportService.BatchNumericReport(_selectedBatchId);

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
                    if (item != "Time")
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
                            ushort value = (dataRow[0] != DBNull.Value) ? Convert.ToUInt16(dataRow[0]) : default;
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

        public async Task CreateChartData()
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    int chartLength = Mins.Count;
                    var seriesNames = TrendValuesByTagNames.Keys.ToArray();
                    int totalLineSeries = seriesNames.Count();

                    Random rand = new Random();

                    // Create line series
                    for (int i = 0; i < totalLineSeries; i++)
                    {
                        ObservableCollection<TrendModel> lineSeriesData = new ObservableCollection<TrendModel>();
                        string keyName = seriesNames[i];

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
                        lineSeries.ArgumentDataMember = "Minute";
                        lineSeries.ValueDataMember = "YVal";
                        lineSeries.CrosshairLabelPattern = keyName + ": {V:F1}";
                        //lineSeries.MarkerVisible = _isChartMarkerVisible;
                        lineSeries.LineTension = 1;

                        _lineSeriesCollection.Add(lineSeries);
                        trendXyDiagram2d.Series.Add(lineSeries);
                        _seriesDataSource.Add(keyName, lineSeriesData);
                    }
                });
            });
        }
    }
}
