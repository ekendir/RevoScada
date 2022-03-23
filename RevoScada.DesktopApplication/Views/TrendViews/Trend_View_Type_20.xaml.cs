using DevExpress.Xpf.Charts;
using DevExpress.Xpf.WindowsUI;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.DesktopApplication.Views.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Views.TrendViews
{
    /// <summary>
    /// Interaction logic for Trend_View_Type_20.xaml
    /// </summary>
    public partial class Trend_View_Type_20 : UserControl
    {
        #region Fields
        private TrendVM _viewModel;
        public DispatcherTimer Timer;
        private Storyboard _showFirstMonitorSecAnim;
        private Storyboard _hideFirstMonitorSecAnim;
        private Storyboard _showLastMonitorSecAnim;
        private Storyboard _hideLastMonitorSecAnim;
        private CrosshairLabelMode _selectedCrosshairLabelMode;

        #region Chart Event Fields
        private bool _allowDragging;
        private ChartHitInfo _chartHitInfo;
        private float _lastPointVal;
        private ushort _lastPointArgument;
        private string _lastPointSeriesName;
        private bool _allowVerticalZoomingByKeyboard;
        private bool _allowHorizontalZoomingByKeyboard;
        #endregion
        #endregion

        #region Collections
        private List<string> _seriesNames;
        private Dictionary<string, ObservableCollection<TrendModel>> _seriesDataSource;
        private SeriesCollection _lineSeriesCollection;
        #endregion

        public Trend_View_Type_20()
        {
            InitializeComponent();

            _lineSeriesCollection = new SeriesCollection();
            _seriesDataSource = new Dictionary<string, ObservableCollection<TrendModel>>();
            // Add a series to draw a line between two points.
            _seriesDataSource.Add("RateCalc", new ObservableCollection<TrendModel>());
            _seriesDataSource.Add("RateCalc_Mon", new ObservableCollection<TrendModel>());
            _seriesDataSource.Add("RateCalc_Press", new ObservableCollection<TrendModel>());

            _showFirstMonitorSecAnim = Resources["showFirstMonitorSec"] as Storyboard;
            _hideFirstMonitorSecAnim = Resources["hideFirstMonitorSec"] as Storyboard;
            _showLastMonitorSecAnim = Resources["showLastMonitorSec"] as Storyboard;
            _hideLastMonitorSecAnim = Resources["hideLastMonitorSec"] as Storyboard;

            dxTrendChart.ContextMenu = (ContextMenu)Resources["contextMenu"];
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as TrendVM;
            SetAxisYMinMaxValues();
            LoadSavedValuesFromAppSettings();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.HasBatchFound)
                return;

            Timer.Stop();

            if (trendXyDiagram2d_temp.AxisY.VisualRange == null || trendXyDiagram2d_temp.AxisX.VisualRange == null)
                return;

            float yMinVal = (float)Convert.ToDouble(trendXyDiagram2d_temp.AxisY.VisualRange.ActualMinValue);
            float yMaxVal = (float)Convert.ToDouble(trendXyDiagram2d_temp.AxisY.VisualRange.ActualMaxValue);
            float xMinVal = (float)Convert.ToDouble(trendXyDiagram2d_temp.AxisX.VisualRange.ActualMinValue);
            float xMaxVal = (float)Convert.ToDouble(trendXyDiagram2d_temp.AxisX.VisualRange.ActualMaxValue);

            SaveChartXYRangeValuesToAppSettings(yMinVal, yMaxVal, xMinVal, xMaxVal);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));

            this.BeginAnimation(UIElement.OpacityProperty, animation);

            if (_viewModel == null)
                return;

            if (!_viewModel.HasBatchFound)
                return;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _viewModel.ContinuousUpdate();
        }

        /// <summary>
        /// Set constant minimum and maximum values on chart Axis Y direction.
        /// </summary>
        private void SetAxisYMinMaxValues()
        {
            if (_viewModel == null)
                return;

            trendXyDiagram2d_temp.AxisY.WholeRange = new Range();
            trendXyDiagram2d_temp.AxisY.WholeRange.MinValue = _viewModel.TrendChartYAxisParamaters.TemperatureParamaterMin;
            trendXyDiagram2d_temp.AxisY.WholeRange.MaxValue = _viewModel.TrendChartYAxisParamaters.TemperatureParamaterMax;

            trendXyDiagram2d_vacuum.WholeRange = new Range();
            trendXyDiagram2d_vacuum.WholeRange.MinValue = _viewModel.TrendChartYAxisParamaters.VacuumParamaterMin;
            trendXyDiagram2d_vacuum.WholeRange.MaxValue = _viewModel.TrendChartYAxisParamaters.VacuumParamaterMax;
            trendXyDiagram2d_vacuum.WholeRange.SideMarginsValue = 20;
            trendXyDiagram2d_vacuum.WholeRange.SetAuto();

            trendXyDiagram2d_pressure.WholeRange = new Range();
            trendXyDiagram2d_pressure.WholeRange.MinValue = _viewModel.TrendChartYAxisParamaters.PressureParamaterMin;
            trendXyDiagram2d_pressure.WholeRange.MaxValue = _viewModel.TrendChartYAxisParamaters.PressureParamaterMax;
            trendXyDiagram2d_pressure.WholeRange.SideMarginsValue = 10;
        }

        private void Properties_OnClick(object sender, RoutedEventArgs e)
        {
            if (_lineSeriesCollection.Count == 0)
            {
                WinUIMessageBox.Show("Loglanmış port bulunamadı.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            IEnumerable<string> allPortNames = _lineSeriesCollection.Where(l => l.DisplayName != "SELECT ALL").Select(l => l.DisplayName);
            IEnumerable<string> selectedPortNames = Enumerable.Empty<string>();

            if (trendXyDiagram2d_temp.Series.Count > 0)
                selectedPortNames = trendXyDiagram2d_temp.Series.Where(l => l.DisplayName != "SELECT ALL").Select(l => l.DisplayName);

            Trend_Properties trend_Properties = new Trend_Properties(this, allPortNames, selectedPortNames, _viewModel.TrendSelectedPortUIProperties);
            trend_Properties.ShowDialog();
        }

        private void ResetRanges_OnClick(object sender, RoutedEventArgs e)
        {
            trendXyDiagram2d_temp.AxisX.VisualRange.SetAuto();
            trendXyDiagram2d_temp.AxisY.VisualRange.SetAuto();
        }

        public void AddSelectedPortToAppSettings(string displayName)
        {
            if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(displayName))
            {
                return;
            }
            else
            {
                TrendSelectedPortUIProperty trendSelectedPortUIProperty = new TrendSelectedPortUIProperty();
                trendSelectedPortUIProperty.DisplayName = displayName;
                _viewModel.TrendSelectedPortUIProperties.Add(displayName, trendSelectedPortUIProperty);

                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
        }

        public void RemoveSelectedPortFromAppSettings(string displayName)
        {
            if (!_viewModel.TrendSelectedPortUIProperties.Keys.Contains(displayName))
            {
                return;
            }
            else
            {
                _viewModel.TrendSelectedPortUIProperties.Remove(displayName);
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
        }

        private void SaveChartUIValuesToAppSettings(string displayName, SolidColorBrush color, int thicknessVal)
        {
            System.Drawing.Color convertedColor = System.Drawing.Color.Empty;
            if (color != null)
                convertedColor = System.Drawing.Color.FromArgb(color.Color.A,
                                                               color.Color.R,
                                                               color.Color.G,
                                                               color.Color.B);

            if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(displayName))
            {
                if (convertedColor != System.Drawing.Color.Empty)
                    _viewModel.TrendSelectedPortUIProperties[displayName].Color = convertedColor;
                if (thicknessVal > 0)
                    _viewModel.TrendSelectedPortUIProperties[displayName].Thickness = thicknessVal;

                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
            else
            {
                TrendSelectedPortUIProperty trendSelectedPortUIProperty = new TrendSelectedPortUIProperty();
                if (convertedColor != System.Drawing.Color.Empty)
                    trendSelectedPortUIProperty.Color = convertedColor;
                if (thicknessVal > 0)
                    trendSelectedPortUIProperty.Thickness = thicknessVal;

                trendSelectedPortUIProperty.DisplayName = displayName;
                _viewModel.TrendSelectedPortUIProperties.Add(displayName, trendSelectedPortUIProperty);
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
        }

        private void SaveChartXYRangeValuesToAppSettings(float yRangeMinVal, float yRangeMaxVal, float xRangeMinVal, float xRangeMaxVal)
        {
            if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains("RangeValues"))
            {
                _viewModel.TrendSelectedPortUIProperties["RangeValues"].YRangeMinValue = yRangeMinVal;
                _viewModel.TrendSelectedPortUIProperties["RangeValues"].YRangeMaxValue = yRangeMaxVal;

                if (xRangeMaxVal > 1)
                {
                    _viewModel.TrendSelectedPortUIProperties["RangeValues"].XRangeMinValue = xRangeMinVal;
                    _viewModel.TrendSelectedPortUIProperties["RangeValues"].XRangeMaxValue = xRangeMaxVal;
                }

                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
            else
            {
                TrendSelectedPortUIProperty trendSelectedPortUIProperty = new TrendSelectedPortUIProperty();
                trendSelectedPortUIProperty.YRangeMinValue = yRangeMinVal;
                trendSelectedPortUIProperty.YRangeMaxValue = yRangeMaxVal;

                if (xRangeMaxVal > 1)
                {
                    trendSelectedPortUIProperty.XRangeMinValue = xRangeMinVal;
                    trendSelectedPortUIProperty.XRangeMaxValue = xRangeMaxVal;
                }

                trendSelectedPortUIProperty.DisplayName = "RangeValues";
                _viewModel.TrendSelectedPortUIProperties.Add("RangeValues", trendSelectedPortUIProperty);
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
        }

        public void AddSelectedSeriesToTrendData(string displayName)
        {
            foreach (var lineSeries in _lineSeriesCollection)
            {
                if (lineSeries.DisplayName == displayName)
                {
                    lineSeries.CheckedInLegend = false;

                    var displayNameVal = Regex.Replace(lineSeries.DisplayName, @"[^A-Za-z_]+", String.Empty);

                    if (_viewModel.VacPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                    {
                        XYDiagram2D.SetSeriesAxisY((XYSeries)lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[0]);
                        trendXyDiagram2d_temp.Series.Add(lineSeries);
                    }
                    else if (_viewModel.PressPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                    {
                        XYDiagram2D.SetSeriesAxisY((XYSeries)lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[1]);
                        trendXyDiagram2d_temp.Series.Add(lineSeries);
                    }
                    else
                        trendXyDiagram2d_temp.Series.Add(lineSeries);
                }
            }
        }

        public void AddSelectedSeriesToTrendData(string[] displayNames)
        {
            foreach (var lineSeries in _lineSeriesCollection)
            {
                foreach (var displayName in displayNames)
                {
                    if (lineSeries.DisplayName == displayName)
                    {
                        lineSeries.CheckedInLegend = false;

                        var displayNameVal = Regex.Replace(lineSeries.DisplayName, @"[^A-Za-z_]+", String.Empty);

                        if (_viewModel.VacPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                        {
                            XYDiagram2D.SetSeriesAxisY((XYSeries)lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[0]);
                            trendXyDiagram2d_temp.Series.Add(lineSeries);
                        }
                        else if (_viewModel.PressPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                        {
                            XYDiagram2D.SetSeriesAxisY((XYSeries)lineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[1]);
                            trendXyDiagram2d_temp.Series.Add(lineSeries);
                        }
                        else
                            trendXyDiagram2d_temp.Series.Add(lineSeries);
                    }
                }
            }
        }

        public void RemoveSelectedSeriesFromTrendData(string displayName)
        {
            for (int i = trendXyDiagram2d_temp.Series.Count - 1; i >= 0; i--)
            {
                if (trendXyDiagram2d_temp.Series[i].DisplayName == displayName)
                {
                    trendXyDiagram2d_temp.Series.Remove(trendXyDiagram2d_temp.Series[i]);
                }
            }
        }

        public void RemoveSelectedSeriesFromTrendData(string[] displayNames)
        {
            foreach (var displayItem in displayNames)
            {
                for (int i = trendXyDiagram2d_temp.Series.Count - 1; i >= 0; i--)
                {
                    if (trendXyDiagram2d_temp.Series[i].DisplayName == displayItem)
                    {
                        trendXyDiagram2d_temp.Series.Remove(trendXyDiagram2d_temp.Series[i]);
                    }
                }
            }
        }

        public void ChangeSelectedSeriesColor(string displayName, SolidColorBrush newColor)
        {
            for (int i = trendXyDiagram2d_temp.Series.Count - 1; i >= 0; i--)
            {
                if (trendXyDiagram2d_temp.Series[i].DisplayName == displayName)
                {
                    SplineSeries2D lineSeries = (SplineSeries2D)trendXyDiagram2d_temp.Series[i];
                    lineSeries.Brush = newColor;
                    trendXyDiagram2d_temp.Series.RemoveAt(i);
                    trendXyDiagram2d_temp.Series.Insert(i, lineSeries);

                    // Save setting to the App.Settings
                    SaveChartUIValuesToAppSettings(displayName, newColor, 0);
                }
            }
        }

        public void ChangeSelectedSeriesColor(string[] displayNames, SolidColorBrush newColor)
        {
            foreach (var displayItem in displayNames)
            {
                for (int i = trendXyDiagram2d_temp.Series.Count - 1; i >= 0; i--)
                {
                    if (trendXyDiagram2d_temp.Series[i].DisplayName == displayItem)
                    {
                        SplineSeries2D lineSeries = (SplineSeries2D)trendXyDiagram2d_temp.Series[i];
                        lineSeries.Brush = newColor;
                        trendXyDiagram2d_temp.Series.RemoveAt(i);
                        trendXyDiagram2d_temp.Series.Insert(i, lineSeries);

                        // Save setting to the App.Settings
                        SaveChartUIValuesToAppSettings(displayItem, newColor, 0);
                    }
                }
            }
        }

        public void ChangeSelectedSeriesThickness(string displayName, int thicknessValue)
        {
            for (int i = trendXyDiagram2d_temp.Series.Count - 1; i >= 0; i--)
            {
                if (trendXyDiagram2d_temp.Series[i].DisplayName == displayName)
                {
                    SplineSeries2D lineSeries = (SplineSeries2D)trendXyDiagram2d_temp.Series[i];
                    lineSeries.LineStyle = new LineStyle(thicknessValue);
                    trendXyDiagram2d_temp.Series.RemoveAt(i);
                    trendXyDiagram2d_temp.Series.Insert(i, lineSeries);

                    // Save setting to the App.Settings
                    SaveChartUIValuesToAppSettings(displayName, null, thicknessValue);
                }
            }
        }

        public void ChangeSelectedSeriesThickness(string[] displayNames, int thicknessValue)
        {
            foreach (var displayItem in displayNames)
            {
                for (int i = trendXyDiagram2d_temp.Series.Count - 1; i >= 0; i--)
                {
                    if (trendXyDiagram2d_temp.Series[i].DisplayName == displayItem)
                    {
                        SplineSeries2D lineSeries = (SplineSeries2D)trendXyDiagram2d_temp.Series[i];
                        lineSeries.LineStyle = new LineStyle(thicknessValue);
                        trendXyDiagram2d_temp.Series.RemoveAt(i);
                        trendXyDiagram2d_temp.Series.Insert(i, lineSeries);

                        // Save setting to the App.Settings
                        SaveChartUIValuesToAppSettings(displayItem, null, thicknessValue);
                    }
                }
            }
        }

        private void LoadSavedValuesFromAppSettings()
        {
            trendXyDiagram2d_temp.AxisX.VisualRange = new Range();
            trendXyDiagram2d_temp.AxisY.VisualRange = new Range();

            // Check if there is any saved range values for chart.
            if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains("RangeValues"))
            {
                float yMinVal = _viewModel.TrendSelectedPortUIProperties["RangeValues"].YRangeMinValue;
                if (yMinVal == 0)
                    yMinVal = _viewModel.TrendChartYAxisParamaters.TemperatureParamaterMin;

                float yMaxVal = _viewModel.TrendSelectedPortUIProperties["RangeValues"].YRangeMaxValue;
                if (yMaxVal == 0)
                    yMaxVal = _viewModel.TrendChartYAxisParamaters.TemperatureParamaterMax;

                trendXyDiagram2d_temp.AxisY.VisualRange.SetMinMaxValues(yMinVal, yMaxVal);

                float xMinVal = _viewModel.TrendSelectedPortUIProperties["RangeValues"].XRangeMinValue;
                float xMaxVal = _viewModel.TrendSelectedPortUIProperties["RangeValues"].XRangeMaxValue;

                if (xMinVal == 0 && xMaxVal == 0)
                    return;

                trendXyDiagram2d_temp.AxisX.VisualRange.SetMinMaxValues(xMinVal, xMaxVal);
            }
        }

        /// <summary>
        /// For first data markers should be active then on second data markers should be hidden on the chart.
        /// </summary>
        public void HideVisibleMarkers()
        {
            foreach (var lineSeries in _lineSeriesCollection)
            {
                SplineSeries2D SplineSeries2D = (SplineSeries2D)lineSeries;
                SplineSeries2D.MarkerVisible = false;
            }
        }

        public void AddSelectAllSeriesToChart()
        {
            // First, check if already exists
            foreach (var series in _lineSeriesCollection)
            {
                if (series.DisplayName == "SELECT ALL")
                    return;
            }

            SplineSeries2D selectAllSeries = new SplineSeries2D();
            selectAllSeries.DisplayName = "SELECT ALL";
            selectAllSeries.CheckedInLegend = false;
            selectAllSeries.Brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#404040");

            DependencyPropertyDescriptor SelectAllLegendDescriptor = DependencyPropertyDescriptor.FromProperty(Series.CheckedInLegendProperty, typeof(Series));
            SelectAllLegendDescriptor.AddValueChanged(selectAllSeries, SelectAllLegendChanged);

            _lineSeriesCollection.Add(selectAllSeries);
            trendXyDiagram2d_temp.Series.Add(selectAllSeries);
        }

        private void CheckOrUncheckSelectAllSeries()
        {
            bool isAllSeriesChecked = true;
            // Check Select All series if all loaded series checked
            foreach (var series in _lineSeriesCollection)
            {
                if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(series.DisplayName) || _viewModel.LiveTrendPrimaryPorts.Contains(series.DisplayName))
                {
                    if (!series.CheckedInLegend)
                        isAllSeriesChecked = false;
                }
            }

            var selectAllSeries = _lineSeriesCollection.Where(l => l.DisplayName == "SELECT ALL").FirstOrDefault();

            if (selectAllSeries != null)
                selectAllSeries.CheckedInLegend = isAllSeriesChecked;
        }

        public void AddPrimaryPortsToChart()
        {
            this.Dispatcher.Invoke(() =>
            {
                // First, add Select All series
                AddSelectAllSeriesToChart();

                foreach (var item in _viewModel.LiveTrendPrimaryPorts)
                {
                    SplineSeries2D lineSeries = new SplineSeries2D();
                    lineSeries.DisplayName = item;

                    // Check from App.Settings if there is any saved color and thickness value for this series
                    if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(item))
                    {
                        System.Drawing.Color colorVal = _viewModel.TrendSelectedPortUIProperties[item].Color;

                        // Set color value
                        if (colorVal != System.Drawing.Color.Empty)
                        {
                            var color = new Color() { R = colorVal.R, G = colorVal.G, B = colorVal.B, A = colorVal.A };
                            lineSeries.Brush = new SolidColorBrush(color);
                        }
                    }

                    // If this series' legend checked in TrendSelectedPortUIProperties then check it here
                    if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(item))
                    {
                        lineSeries.CheckedInLegend = _viewModel.TrendSelectedPortUIProperties[item].IsLegendChecked;
                    }

                    DependencyPropertyDescriptor CheckedInLegendDescriptor = DependencyPropertyDescriptor.FromProperty(Series.CheckedInLegendProperty, typeof(Series));
                    CheckedInLegendDescriptor.AddValueChanged(lineSeries, CheckedInLegendChanged);

                    _lineSeriesCollection.Add(lineSeries);
                    trendXyDiagram2d_temp.Series.Add(lineSeries);
                }

                CheckOrUncheckSelectAllSeries();
            });

        }

        public void CreateChartData()
        {
            if (_viewModel.ValuesByTagNamesSensorValue.Count() == 0)
                return;

            trendXyDiagram2d_temp.Series.Clear();
            _lineSeriesCollection.Clear();

            int chartLength = _viewModel.AxisXMins.Count;
            int totalLineSeries = _viewModel.ValuesByTagNamesSensorValue.Count;
            List<string> addedSeriesDisplayNames = new List<string>();

            _seriesNames = _viewModel.ValuesByTagNamesSensorValue.Keys.ToList();
            _seriesNames.Add("Mins");
            totalLineSeries++;

            int counter = 0;

            // First, create "Select All" series
            AddSelectAllSeriesToChart();

            // Create line series
            for (int i = 0; i < totalLineSeries; i++)
            {
                ObservableCollection<TrendModel> lineSeriesData = new ObservableCollection<TrendModel>();
                string keyName = string.Empty;
                keyName = _seriesNames[counter];

                for (int j = 0; j < chartLength; j++)
                {
                    TrendModel trendModel = new TrendModel();

                    float? value = null;

                    trendModel.Minute = _viewModel.AxisXMins[j];

                    if (keyName != "Mins")
                    {
                        if (j < _viewModel.ValuesByTagNamesSensorValue[keyName].Count)
                        {
                            value = _viewModel.ValuesByTagNamesSensorValue[keyName][j];
                        }
                    }
                    else
                    {
                        if (j < _viewModel.Mins.Count)
                        {
                            value = (float)_viewModel.Mins[j];
                        }
                    }

                    trendModel.YVal = value;
                    trendModel.SeriesName = keyName;

                    if (j < _viewModel.DateTimeValues.Count)
                        trendModel.Date = _viewModel.DateTimeValues[j];

                    lineSeriesData.Add(trendModel);
                }

                _seriesDataSource.Add(keyName, lineSeriesData);

                SplineSeries2D lineSeries = new SplineSeries2D();
                lineSeries.DisplayName = keyName;

                lineSeries.DataSource = _seriesDataSource[keyName];
                lineSeries.ArgumentDataMember = "Minute";
                lineSeries.ValueDataMember = "YVal";
                lineSeries.CrosshairLabelPattern = keyName + ": {V:F1}";
                lineSeries.MarkerVisible = _viewModel.IsMarkerVisible;
                lineSeries.MarkerSize = 7;
                lineSeries.LineTension = 1;

                DependencyPropertyDescriptor CheckedInLegendDescriptor = DependencyPropertyDescriptor.FromProperty(Series.CheckedInLegendProperty, typeof(Series));
                CheckedInLegendDescriptor.AddValueChanged(lineSeries, CheckedInLegendChanged);

                // Check from App.Settings if there is any saved color and thickness value for this series
                if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(keyName))
                {
                    System.Drawing.Color colorVal = _viewModel.TrendSelectedPortUIProperties[keyName].Color;
                    int thicknessVal = _viewModel.TrendSelectedPortUIProperties[keyName].Thickness;

                    // Set color value
                    if (colorVal != System.Drawing.Color.Empty)
                    {
                        var color = new Color() { R = colorVal.R, G = colorVal.G, B = colorVal.B, A = colorVal.A };
                        lineSeries.Brush = new SolidColorBrush(color);
                    }

                    // Set thickness value
                    lineSeries.LineStyle = new LineStyle(thicknessVal);
                }

                _lineSeriesCollection.Add(lineSeries);

                // If this series selected in TrendSelectedPortUIProperties or in LiveTrendPrimaryPorts then add to the chart.
                if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(keyName) || _viewModel.LiveTrendPrimaryPorts.Contains(keyName))
                {
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

                // If this series' legend checked in TrendSelectedPortUIProperties then check it here
                if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(keyName))
                {
                    lineSeries.CheckedInLegend = _viewModel.TrendSelectedPortUIProperties[keyName].IsLegendChecked;
                }

                counter++;
            }

            CheckOrUncheckSelectAllSeries();

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
            trendXyDiagram2d_temp.Series.Add(rateCalcLineSeries);

            // For MON typed port calculations
            SplineSeries2D rateCalcMonLineSeries = new SplineSeries2D();
            rateCalcMonLineSeries.DisplayName = "RateCalc_Mon";
            rateCalcMonLineSeries.Brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#3A4E7A"); // MainBlueColor
            rateCalcMonLineSeries.LineStyle = new LineStyle(1);
            rateCalcMonLineSeries.LineStyle.DashStyle = DashStyles.Dash;
            rateCalcMonLineSeries.ShowInLegend = false;
            rateCalcMonLineSeries.DataSource = _seriesDataSource["RateCalc_Mon"];
            rateCalcMonLineSeries.ArgumentDataMember = "Minute";
            rateCalcMonLineSeries.ValueDataMember = "YVal";
            trendXyDiagram2d_temp.Series.Add(rateCalcMonLineSeries);
            XYDiagram2D.SetSeriesAxisY(rateCalcMonLineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[0]);

            // For Press typed port calculations
            SplineSeries2D rateCalcPressLineSeries = new SplineSeries2D();
            rateCalcPressLineSeries.DisplayName = "RateCalc_Press";
            rateCalcPressLineSeries.Brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#3A4E7A"); // MainBlueColor
            rateCalcPressLineSeries.LineStyle = new LineStyle(1);
            rateCalcPressLineSeries.LineStyle.DashStyle = DashStyles.Dash;
            rateCalcPressLineSeries.ShowInLegend = false;
            rateCalcPressLineSeries.DataSource = _seriesDataSource["RateCalc_Press"];
            rateCalcPressLineSeries.ArgumentDataMember = "Minute";
            rateCalcPressLineSeries.ValueDataMember = "YVal";
            trendXyDiagram2d_temp.Series.Add(rateCalcPressLineSeries);
            XYDiagram2D.SetSeriesAxisY(rateCalcPressLineSeries, ((XYDiagram2D)dxTrendChart.Diagram).SecondaryAxesY[1]);

            _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = false;

            Timer.Interval = TimeSpan.FromSeconds(30); // 30 seconds
            Timer.Start();
        }

        public void UpdateChartData(Dictionary<string, List<float>> UpdatedValuesByTagNamesSensorValue, List<ushort> updatedMins,
                                    List<DateTime> updatedDateValues)
        {
            if (_seriesDataSource.Count < 1 || UpdatedValuesByTagNamesSensorValue.Count < 1)
            {
                _viewModel.InitializePageData();
                return;
            }

            for (int i = 0; i < updatedMins.Count; i++)
            {
                // Check if minute value is 3 less than LastAxisLimitVal (50). 
                // e.g: if min value is 47-97.. then it's good to go...
                if (_viewModel.LastAxisLimitVal - 3 <= updatedMins[i])
                {
                    ushort startIndex = (ushort)_viewModel.LastAxisLimitVal;
                    startIndex++;

                    _viewModel.LastAxisLimitVal += 50;

                    for (ushort j = startIndex; j <= _viewModel.LastAxisLimitVal; j++)
                    {
                        _viewModel.AxisXMins.Add(j);
                    }

                    foreach (var item in UpdatedValuesByTagNamesSensorValue)
                    {
                        foreach (float value in item.Value)
                        {
                            _viewModel.ValuesByTagNamesSensorValue[item.Key].Add(value);
                        }
                    }

                    var maxMinVal = _viewModel.Mins.Max();

                    foreach (var min in updatedMins)
                    {
                        if (min > maxMinVal)
                            _viewModel.Mins.Add(min);
                    }

                    trendXyDiagram2d_temp.Series.Clear();
                    _lineSeriesCollection.Clear();
                    _seriesDataSource.Clear();
                    // Add a series to draw a line between two points.
                    _seriesDataSource.Add("RateCalc", new ObservableCollection<TrendModel>());
                    _seriesDataSource.Add("RateCalc_Mon", new ObservableCollection<TrendModel>());
                    _seriesDataSource.Add("RateCalc_Press", new ObservableCollection<TrendModel>());
                    _viewModel.Mins.Clear();
                    _viewModel.InitializePageData();
                    return;
                }

                for (int k = 0; k < _seriesNames.Count(); k++)
                {
                    string keyName = _seriesNames[k];
                    int totalLogs = updatedMins.Count;

                    for (int j = 0; j < totalLogs; j++)
                    {
                        TrendModel trendModel = new TrendModel();

                        trendModel.Minute = updatedMins[j];
                        if (j < updatedDateValues.Count)
                            trendModel.Date = updatedDateValues[j];

                        if (keyName == "Mins")
                        {
                            trendModel.YVal = updatedMins[j];
                        }
                        else
                        {
                            if (j < UpdatedValuesByTagNamesSensorValue[keyName].Count)
                            {
                                trendModel.YVal = UpdatedValuesByTagNamesSensorValue[keyName][j];
                            }
                        }

                        // If minute is already there then by accessing its index, change its model. 
                        // If not, add this new model to the seriesDataSource.
                        if (_seriesDataSource[keyName].Where(s => s.Minute == updatedMins[j]).Any())
                        {
                            int index = CollectionUtils.FindIndex(_seriesDataSource[keyName], s => s.Minute == updatedMins[j]);
                            _seriesDataSource[keyName][index] = trendModel;
                        }
                        else
                        {
                            _seriesDataSource[keyName].Add(trendModel);
                        }
                    }
                }
            }
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

                var lastPointSeriesName = Regex.Replace(_lastPointSeriesName, @"[^A-Za-z_]+", String.Empty);

                if (_viewModel.VacPortNames.Where(v => v.StartsWith(lastPointSeriesName)).Any())
                {
                    AddRateCalcValues("RateCalc_Mon", trendModel);
                    MonitorRateDetails("RateCalc_Mon");
                }
                else if (_viewModel.PressPortNames.Where(v => v.StartsWith(lastPointSeriesName)).Any())
                {
                    AddRateCalcValues("RateCalc_Press", trendModel);
                    MonitorRateDetails("RateCalc_Press");
                }
                else
                {
                    AddRateCalcValues("RateCalc", trendModel);
                    MonitorRateDetails("RateCalc");
                }
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
                    _lastPointArgument = Convert.ToUInt16(element.SeriesPoint.Argument);
                    _lastPointSeriesName = element.SeriesPoint.Series.DisplayName;
                }

                var displayNameVal = Regex.Replace(_lastPointSeriesName, @"[^A-Za-z_]+", String.Empty);
                string rateCalVal = string.Empty;

                if (_viewModel.VacPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                    rateCalVal = "RateCalc_Mon";
                else if (_viewModel.PressPortNames.Where(v => v.StartsWith(displayNameVal)).Any())
                    rateCalVal = "RateCalc_Press";
                else
                    rateCalVal = "RateCalc";

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
                _seriesDataSource["RateCalc_Mon"].Clear();
                _seriesDataSource["RateCalc_Press"].Clear();
            }

            _selectedCrosshairLabelMode = dxTrendCrosshairOptions.CrosshairLabelMode;
            dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
            //MonitorRateDetails();
        }

        private void dxTrendChart_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _allowDragging = false;
            _seriesDataSource["RateCalc"].Clear();
            _seriesDataSource["RateCalc_Mon"].Clear();
            _seriesDataSource["RateCalc_Press"].Clear();

            if (firstValuesLayout.Opacity == 1)
                _hideFirstMonitorSecAnim.Begin();

            if (lastValuesLayout.Opacity == 1)
                _hideLastMonitorSecAnim.Begin();

            dxTrendCrosshairOptions.CrosshairLabelMode = _selectedCrosshairLabelMode;
        }
        #endregion

        private void SelectAllLegendChanged(object sender, EventArgs e)
        {
            SplineSeries2D lineSeries = (SplineSeries2D)sender;
            bool isItChecked = lineSeries.CheckedInLegend;

            foreach (var series in _lineSeriesCollection)
            {
                if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(series.DisplayName) || _viewModel.LiveTrendPrimaryPorts.Contains(series.DisplayName))
                    series.CheckedInLegend = isItChecked;
            }
        }

        private void CheckedInLegendChanged(object sender, EventArgs e)
        {
            SplineSeries2D lineSeries = (SplineSeries2D)sender;
            bool isItChecked = lineSeries.CheckedInLegend;

            if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains(lineSeries.DisplayName))
            {
                _viewModel.TrendSelectedPortUIProperties[lineSeries.DisplayName].IsLegendChecked = isItChecked;
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
            else
            {
                TrendSelectedPortUIProperty trendSelectedPortUIProperty = new TrendSelectedPortUIProperty();
                trendSelectedPortUIProperty.DisplayName = lineSeries.DisplayName;
                trendSelectedPortUIProperty.IsLegendChecked = isItChecked;
                _viewModel.TrendSelectedPortUIProperties.Add(lineSeries.DisplayName, trendSelectedPortUIProperty);
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }
        }

        private void showCommonForAllSeries_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            bool isCheckedVal = false;

            // Check from App.Settings if there is any saved value for crosshair type of the chart.
            if (_viewModel.TrendSelectedPortUIProperties.Keys.Contains("CrosshairType"))
            {
                isCheckedVal = _viewModel.TrendSelectedPortUIProperties["CrosshairType"].IsShowCommonForAllSeriesChecked;
                checkBox.IsChecked = isCheckedVal;
            }
            else
            {
                isCheckedVal = true;
                TrendSelectedPortUIProperty trendSelectedPortUIProperty = new TrendSelectedPortUIProperty();
                trendSelectedPortUIProperty.IsShowCommonForAllSeriesChecked = isCheckedVal;
                _viewModel.TrendSelectedPortUIProperties.Add("CrosshairType", trendSelectedPortUIProperty);
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;
            }

            checkBox.IsChecked = isCheckedVal;

            if (isCheckedVal)
                dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowCommonForAllSeries;
            else
                dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
        }

        private void showCommonForAllSeries_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (!checkBox.IsLoaded)
                return;

            bool isCheckedVal = checkBox.IsChecked ?? false;

            if (!_viewModel.TrendSelectedPortUIProperties.Keys.Contains("CrosshairType"))
            {
                TrendSelectedPortUIProperty trendSelectedPortUIProperty = new TrendSelectedPortUIProperty();
                trendSelectedPortUIProperty.IsShowCommonForAllSeriesChecked = isCheckedVal;
                _viewModel.TrendSelectedPortUIProperties.Add("CrosshairType", trendSelectedPortUIProperty);
                _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;

                return;
            }

            _viewModel.TrendSelectedPortUIProperties["CrosshairType"].IsShowCommonForAllSeriesChecked = isCheckedVal;
            _viewModel.TrendSelectedPortUIPropertiesSetter = _viewModel.TrendSelectedPortUIProperties;

            if (isCheckedVal)
                dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowCommonForAllSeries;
            else
                dxTrendCrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
        }

        private void dxTrendChart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double minValueX = (double)trendXyDiagram2d_temp.ActualAxisX.ActualVisualRange.ActualMinValue;
            double maxValueX = (double)trendXyDiagram2d_temp.ActualAxisX.ActualVisualRange.ActualMaxValue;
            double minValueY = (double)trendXyDiagram2d_temp.ActualAxisY.ActualVisualRange.ActualMinValue;
            double maxValueY = (double)trendXyDiagram2d_temp.ActualAxisY.ActualVisualRange.ActualMaxValue;

            if (e.Delta > 0)
            {
                double minValueNewX = Math.Floor(0.5 * minValueX + 1);
                double maxValueNewX = Math.Floor(0.5 * maxValueX + 1);
                double minValueNewY = Math.Floor(0.7 * minValueY + 50);
                double maxValueNewY = Math.Floor(0.7 * maxValueY + 50);

                if (_allowHorizontalZoomingByKeyboard)
                    trendXyDiagram2d_temp.ActualAxisX.ActualVisualRange.SetMinMaxValues(minValueNewX, maxValueNewX);

                if (_allowVerticalZoomingByKeyboard)
                    trendXyDiagram2d_temp.ActualAxisY.ActualVisualRange.SetMinMaxValues(minValueNewY, maxValueNewY);
            }
            if (e.Delta < 0)
            {
                double minValueNewX = Math.Floor(0.8 * minValueX + 1);
                double maxValueNewX = Math.Floor(1.1 * maxValueX + 1);
                double minValueNewY = Math.Floor(1.1 * minValueY + 50);
                double maxValueNewY = Math.Floor(1.1 * maxValueY + 50);

                if (_allowHorizontalZoomingByKeyboard)
                    trendXyDiagram2d_temp.ActualAxisX.ActualVisualRange.SetMinMaxValues(minValueNewX, maxValueNewX);

                if (_allowVerticalZoomingByKeyboard)
                    trendXyDiagram2d_temp.ActualAxisY.ActualVisualRange.SetMinMaxValues(minValueNewY, maxValueNewY);
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
    }
}
