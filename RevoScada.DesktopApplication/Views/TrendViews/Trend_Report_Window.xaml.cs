using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Printing;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Trend_Report_Window.xaml
    /// </summary>
    public partial class Trend_Report_Window : Window
    {
        #region Fields
        private Storyboard _showFirstMonitorSecAnim;
        private Storyboard _hideFirstMonitorSecAnim;
        private Storyboard _showLastMonitorSecAnim;
        private Storyboard _hideLastMonitorSecAnim;
        private CrosshairLabelMode _selectedCrosshairLabelMode;
        private ReportsVM _viewModel;
        private bool _isChecked;
        private bool _isChartMarkerVisible;

        #region Chart Event Fields
        private bool _allowDragging;
        private ChartHitInfo _chartHitInfo;
        private float _lastPointVal;
        private double _lastPointArgument;
        private string _lastPointSeriesName;
        private bool _allowVerticalZoomingByKeyboard;
        private bool _allowHorizontalZoomingByKeyboard;
        #endregion
        #endregion

        #region Collections
        private Dictionary<string, ObservableCollection<TrendModel>> _seriesDataSource;
        private SeriesCollection _lineSeriesCollection;
        #endregion

        public Trend_Report_Window(ReportsVM viewModel, bool isChartMarkerVisible = false)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _isChartMarkerVisible = isChartMarkerVisible;
            _lineSeriesCollection = new SeriesCollection();
            _seriesDataSource = new Dictionary<string, ObservableCollection<TrendModel>>();
            // Add a series to draw a line between two points.
            _seriesDataSource.Add("RateCalc", new ObservableCollection<TrendModel>());

            _showFirstMonitorSecAnim = Resources["showFirstMonitorSec"] as Storyboard;
            _hideFirstMonitorSecAnim = Resources["hideFirstMonitorSec"] as Storyboard;
            _showLastMonitorSecAnim = Resources["showLastMonitorSec"] as Storyboard;
            _hideLastMonitorSecAnim = Resources["hideLastMonitorSec"] as Storyboard;

            dxTrendChart.ContextMenu = (ContextMenu)Resources["contextMenu"];
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
           await CreateChartData();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.ReportsView.SelectBatchSection();
        }

        private void AddSelectAllSeriesToChart()
        {
            // First, check if already exists
            foreach (var series in _lineSeriesCollection)
            {
                if (series.DisplayName == "SELECT ALL")
                    return;
            }

            SplineSeries2D selectAllSeries = new SplineSeries2D();
            selectAllSeries.DisplayName = "SELECT ALL";
            selectAllSeries.CheckedInLegend = true;
            selectAllSeries.Brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#404040");

            DependencyPropertyDescriptor SelectAllLegendDescriptor = DependencyPropertyDescriptor.FromProperty(Series.CheckedInLegendProperty, typeof(Series));
            SelectAllLegendDescriptor.AddValueChanged(selectAllSeries, SelectAllLegendChanged);

            _lineSeriesCollection.Add(selectAllSeries);
            trendXyDiagram2d.Series.Add(selectAllSeries);
        }

        public double GetRandomNumber(float minimum, float maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public async Task CreateChartData()
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    trendXyDiagram2d.AxisX.VisualRange = new Range();
                    trendXyDiagram2d.AxisX.VisualRange.SideMarginsValue = 5;
                    trendXyDiagram2d.AxisY.VisualRange = new Range();
                    trendXyDiagram2d.AxisY.VisualRange.SideMarginsValue = 10;

                    // First, create "Select All" series
                    AddSelectAllSeriesToChart();

                    // To draw a line between two points create a dashed line series.
                    SplineSeries2D rateCalcLineSeries = new SplineSeries2D();
                    rateCalcLineSeries.DisplayName = "RateCalc";
                    rateCalcLineSeries.Brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#3A4E7A"); // MainBlueColor
                    rateCalcLineSeries.LineStyle = new LineStyle(1);
                    rateCalcLineSeries.LineStyle.DashStyle = DashStyles.Dash;
                    rateCalcLineSeries.ShowInLegend = false;
                    rateCalcLineSeries.DataSource = _seriesDataSource["RateCalc"];
                    rateCalcLineSeries.ArgumentDataMember = "Minute";
                    rateCalcLineSeries.ValueDataMember = "YVal";

                    trendXyDiagram2d.Series.Add(rateCalcLineSeries);

                    int chartLength = _viewModel.Mins.Count;
                    var seriesNames = _viewModel.TrendValuesByTagNames.Keys.ToArray();
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
                            trendModel.Minute = _viewModel.Mins[j];

                            float value = 0;

                            if (keyName == "Mins")
                            {
                                value = (float)_viewModel.Mins[j];
                            }
                            else
                            {
                                if (j < _viewModel.TrendValuesByTagNames[keyName].Count)
                                {
                                    value = _viewModel.TrendValuesByTagNames[keyName][j];
                                }
                            }

                            if (j < _viewModel.TrendDateTimeValues.Count)
                                trendModel.Date = _viewModel.TrendDateTimeValues[j];

                            trendModel.YVal = value;
                            lineSeriesData.Add(trendModel);
                        }

                        SplineSeries2D lineSeries = new SplineSeries2D();
                        lineSeries.DisplayName = keyName;

                        lineSeries.DataSource = lineSeriesData;
                        lineSeries.ArgumentDataMember = "Minute";
                        lineSeries.ValueDataMember = "YVal";
                        lineSeries.CrosshairLabelPattern = keyName + ": {V:F2}";
                        lineSeries.MarkerVisible = _isChartMarkerVisible;
                        lineSeries.LineTension = 99.8;
                        lineSeries.LineStyle = new LineStyle(3);
                        //lineSeries.LineStyle.Thickness = 2; 

                        if (!_viewModel.SelectedSeriesNames.Contains(keyName))
                            lineSeries.CheckedInLegend = false;

                        DependencyPropertyDescriptor CheckedInLegendDescriptor = DependencyPropertyDescriptor.FromProperty(Series.CheckedInLegendProperty, typeof(Series));
                        CheckedInLegendDescriptor.AddValueChanged(lineSeries, CheckedInLegendChanged);

                        _lineSeriesCollection.Add(lineSeries);
                        trendXyDiagram2d.Series.Add(lineSeries);
                        _seriesDataSource.Add(keyName, lineSeriesData);
                    }
                });
            });

            Thread.Sleep(1000);
            this.Width = 1920;
            this.Height = 1080;
            this.WindowState = WindowState.Maximized;

            //if (!trendXyDiagram2d.Series.Any())
            //    return;

            //DependencyPropertyDescriptor CheckedInLegendDescriptor = DependencyPropertyDescriptor.FromProperty(Series.CheckedInLegendProperty, typeof(Series));
            //CheckedInLegendDescriptor.AddValueChanged(trendXyDiagram2d.Series[0], CheckedInLegendChanged);
            //CheckedInLegendChanged(trendXyDiagram2d.Series[0], null);

        }

        private void CheckedInLegendChanged(object sender, EventArgs e)
        {
            SplineSeries2D lineSeries = (SplineSeries2D)sender;
            bool isItChecked = lineSeries.CheckedInLegend;

            if (_viewModel.SelectedSeriesNames.Contains(lineSeries.DisplayName))
            {
                _viewModel.SelectedSeriesNames.Remove(lineSeries.DisplayName);
            }
            else
            {
                _viewModel.SelectedSeriesNames.Add(lineSeries.DisplayName);
            }
        }

        private void showCommonForAllSeries_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (!checkBox.IsLoaded)
                return;

            bool isCheckedVal = checkBox.IsChecked ?? false;

            if (isCheckedVal)
                dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowCommonForAllSeries;
            else
                dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
        }

        private void ResetRanges_OnClick(object sender, RoutedEventArgs e)
        {
            trendXyDiagram2d.AxisX.VisualRange.SetAuto();
            trendXyDiagram2d.AxisY.VisualRange.SetAuto();
        }

        #region Chart Event Handlers
        private void dxTrendChart_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_allowDragging)
            {
                if (float.IsNaN(_lastPointVal) || _lastPointArgument == 0)
                    return;

                TrendModel trendModel = new TrendModel();
                trendModel.YVal = _lastPointVal;
                trendModel.Minute = _lastPointArgument;
                trendModel.SeriesName = _lastPointSeriesName;

                if (_seriesDataSource.ContainsKey(_lastPointSeriesName))
                    trendModel.Date = _seriesDataSource[_lastPointSeriesName].Where(r => r.Minute == _lastPointArgument).FirstOrDefault()?.Date;
               
                AddRateCalcValues("RateCalc", trendModel);
                MonitorRateDetails("RateCalc");
            }
        }

        private void AddRateCalcValues(string rateCalcName, TrendModel trendModel)
        {
            if (_seriesDataSource[rateCalcName].Count == 0)
            {
                _seriesDataSource[rateCalcName].Add(trendModel);
            }
            else if (_seriesDataSource[rateCalcName].Count == 1)
            {
                if (_seriesDataSource[rateCalcName][0].YVal == trendModel.YVal && _seriesDataSource[rateCalcName][0].Minute == trendModel.Minute)
                    return;

                _seriesDataSource[rateCalcName].Add(trendModel);
            }
            else
            {
                if (_seriesDataSource[rateCalcName][1].YVal == trendModel.YVal && _seriesDataSource[rateCalcName][1].Minute == trendModel.Minute)
                    return;

                _seriesDataSource[rateCalcName][1] = trendModel;
            }
        }

        private string CalculateRateAndSpan(float firstLogVal, double firstMinVal, float lastLogVal, double lastMinVal, DateTime? dateTime)
        {
            // Span calculation
            float firstMin = (float)firstMinVal;
            float lastMin = (float)lastMinVal;

            float spanResult = lastMin - firstMin;

            // Rate calculation
            float rateResult;
            float lognum1 = Math.Max(firstLogVal, lastLogVal);
            float lognum2 = Math.Min(firstLogVal, lastLogVal);

            if (spanResult == 0)
                rateResult = 0;
            else
                rateResult = (lognum1 - lognum2) / spanResult;

            string fixedRateResult = rateResult.ToString("0.00");

            return string.Format("Interval: {0}\rRate: {1}\rTime: {2}", spanResult, fixedRateResult, dateTime?.ToString("HH:mm:ss") ?? string.Empty);
        }

        /// <summary>
        /// Find a series point that is closest to an argument from chart coordinates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dxTrendChart_CustomDrawCrosshair(object sender, CustomDrawCrosshairEventArgs e)
        {
            if (!_allowDragging)
            {
                dxTrendCrosshairOptions.PopupTemplate = null;
                return;
            }

            foreach (CrosshairElementGroup group in e.CrosshairElementGroups)
            {
                // Obtain the first series.
                CrosshairElement element = group.CrosshairElements[0];

                if (!double.IsNaN(element.SeriesPoint.Value) && !string.IsNullOrEmpty(element.SeriesPoint.Argument))
                {
                    _lastPointVal = (float)element.SeriesPoint.Value;
                    _lastPointArgument = Convert.ToDouble(element.SeriesPoint.Argument);
                    _lastPointSeriesName = element.SeriesPoint.Series.DisplayName;
                }

                string rateCalVal = "RateCalc";

                if (_seriesDataSource[rateCalVal].Count == 2)
                {
                    element.LabelElement.Text = CalculateRateAndSpan(_seriesDataSource[rateCalVal][0].YVal ?? 0, _seriesDataSource[rateCalVal][0].Minute,
                                                _seriesDataSource[rateCalVal][1].YVal ?? 0, _seriesDataSource[rateCalVal][1].Minute,
                                                _seriesDataSource[rateCalVal][1].Date ?? DateTime.MinValue);

                    element.LabelElement.MarkerVisible = false;
                    element.LabelElement.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#303030");
                    element.LabelElement.FontWeight = FontWeights.SemiBold;
                    dxTrendCrosshairOptions.PopupTemplate = FindResource("rateCustomPopupTemplate") as DataTemplate;
                }
            }
        }

        private void SelectAllLegendChanged(object sender, EventArgs e)
        {
            SplineSeries2D lineSeries = (SplineSeries2D)sender;
            bool isItChecked = lineSeries.CheckedInLegend;

            foreach (var series in _lineSeriesCollection)
            {
               series.CheckedInLegend = isItChecked;
            }
        }

        private void MonitorRateDetails(string rateCalcName)
        {
            if (_seriesDataSource[rateCalcName].Count == 0)
                return;

            first_seriesTb.Text = _seriesDataSource[rateCalcName][0].SeriesName;
            first_valueTb.Text = string.Format("{0:0.000}", _seriesDataSource[rateCalcName][0].YVal);
            first_minTb.Text = _seriesDataSource[rateCalcName][0].Minute.ToString();

            if (firstValuesLayout.Opacity == 0)
                _showFirstMonitorSecAnim.Begin();

            if (lastValuesLayout.Opacity == 0)
                _showLastMonitorSecAnim.Begin();

            if (_seriesDataSource[rateCalcName].Count == 1)
                return;

            last_seriesTb.Text = _seriesDataSource[rateCalcName][1].SeriesName;
            last_valueTb.Text = string.Format("{0:0.000}", _seriesDataSource[rateCalcName][1].YVal);
            last_minTb.Text = _seriesDataSource[rateCalcName][1].Minute.ToString();
        }

        private void dxTrendChart_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _chartHitInfo = dxTrendChart.CalcHitInfo(e.GetPosition(dxTrendChart));

            if (_chartHitInfo.InDiagram && e.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
                _allowDragging = true;
                _seriesDataSource["RateCalc"].Clear();
            }

            _selectedCrosshairLabelMode = dxTrendCrosshairOptions.CrosshairLabelMode;
            dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
        }

        private void dxTrendChart_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _allowDragging = false;
            _seriesDataSource["RateCalc"].Clear();

            if (firstValuesLayout.Opacity == 1)
                _hideFirstMonitorSecAnim.Begin();

            if (lastValuesLayout.Opacity == 1)
                _hideLastMonitorSecAnim.Begin();

            dxTrendCrosshairOptions.CrosshairLabelMode = _selectedCrosshairLabelMode;
        }

        private void dxTrendChart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double minValueX = (double)trendXyDiagram2d.ActualAxisX.ActualVisualRange.ActualMinValue;
            double maxValueX = (double)trendXyDiagram2d.ActualAxisX.ActualVisualRange.ActualMaxValue;
            double minValueY = (double)trendXyDiagram2d.ActualAxisY.ActualVisualRange.ActualMinValue;
            double maxValueY = (double)trendXyDiagram2d.ActualAxisY.ActualVisualRange.ActualMaxValue;

            if (e.Delta > 0)
            {
                double minValueNewX = Math.Floor(0.5 * minValueX + 1);
                double maxValueNewX = Math.Floor(0.5 * maxValueX + 1);
                double minValueNewY = Math.Floor(0.7 * minValueY + 50);
                double maxValueNewY = Math.Floor(0.7 * maxValueY + 50);

                if (_allowHorizontalZoomingByKeyboard)
                    trendXyDiagram2d.ActualAxisX.ActualVisualRange.SetMinMaxValues(minValueNewX, maxValueNewX);

                if (_allowVerticalZoomingByKeyboard)
                    trendXyDiagram2d.ActualAxisY.ActualVisualRange.SetMinMaxValues(minValueNewY, maxValueNewY);
            }
            if (e.Delta < 0)
            {
                double minValueNewX = Math.Floor(0.8 * minValueX + 1);
                double maxValueNewX = Math.Floor(1.1 * maxValueX + 1);
                double minValueNewY = Math.Floor(1.1 * minValueY + 50);
                double maxValueNewY = Math.Floor(1.1 * maxValueY + 50);

                if (_allowHorizontalZoomingByKeyboard)
                    trendXyDiagram2d.ActualAxisX.ActualVisualRange.SetMinMaxValues(minValueNewX, maxValueNewX);

                if (_allowVerticalZoomingByKeyboard)
                    trendXyDiagram2d.ActualAxisY.ActualVisualRange.SetMinMaxValues(minValueNewY, maxValueNewY);
            }
        }

        private void dxTrendChart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                _allowVerticalZoomingByKeyboard = true;

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                _allowHorizontalZoomingByKeyboard = true;
        }

        private void dxTrendChart_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                _allowVerticalZoomingByKeyboard = false;

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                _allowHorizontalZoomingByKeyboard = false;
        }

        private void dxTrendChart_MouseEnter(object sender, MouseEventArgs e)
        {
            dxTrendChart.Focus();
        }
        #endregion
    }
}