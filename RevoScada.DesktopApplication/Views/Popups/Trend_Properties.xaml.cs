using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views.TrendViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for Trend_Properties.xaml
    /// </summary>
    public partial class Trend_Properties : Window
    {
        #region Fields
        private Trend_View _trendView;
        private Trend_View_Type_20 _trendViewType20;
        #endregion

        #region Collections
        public ObservableCollection<string> AllPortsColl { get; set; }
        public ObservableCollection<string> SelectedPortsColl { get; set; }
        public ObservableCollection<int> ThicknessValues { get; set; }
        private Dictionary<string, TrendSelectedPortUIProperty> _trendSelectedPortUIProperties;
        #endregion

        public Trend_Properties(Trend_View trendView, IEnumerable<string> allPortNames, IEnumerable<string> selectedPortNames,
                                Dictionary<string, TrendSelectedPortUIProperty> trendSelectedPortUIProperties)
        {
            InitializeComponent();

            DataContext = this;
            SelectedPortsColl = new ObservableCollection<string>();
            AllPortsColl = new ObservableCollection<string>();
            ThicknessValues = GetThicknessValues();
            _trendView = trendView;

            AllPortsColl = allPortNames.ToObservableCollection();

            if (selectedPortNames.Count() > 0)
            {
                SelectedPortsColl = selectedPortNames.Where(s => s != "RateCalc" && s != "RateCalc_Mon" && s != "RateCalc_Press").ToObservableCollection();
                AllPortsColl = UpdateAllPortsCollection();
            }

            _trendSelectedPortUIProperties = trendSelectedPortUIProperties;
        }

        public Trend_Properties(Trend_View_Type_20 trendView, IEnumerable<string> allPortNames, IEnumerable<string> selectedPortNames,
                        Dictionary<string, TrendSelectedPortUIProperty> trendSelectedPortUIProperties)
        {
            InitializeComponent();

            DataContext = this;
            SelectedPortsColl = new ObservableCollection<string>();
            AllPortsColl = new ObservableCollection<string>();
            ThicknessValues = GetThicknessValues();
            _trendViewType20 = trendView;

            AllPortsColl = allPortNames.ToObservableCollection();

            if (selectedPortNames.Count() > 0)
            {
                SelectedPortsColl = selectedPortNames.Where(s => s != "RateCalc" && s != "RateCalc_Mon" && s != "RateCalc_Press").ToObservableCollection();
                AllPortsColl = UpdateAllPortsCollection();
            }

            _trendSelectedPortUIProperties = trendSelectedPortUIProperties;
        }

        private ObservableCollection<int> GetThicknessValues()
        {
            var thicknessValues = new ObservableCollection<int>();
            for (int i = 1; i <= 5; i++)
            {
                thicknessValues.Add(i);
            }
            return thicknessValues;
        }

        private ObservableCollection<string> UpdateAllPortsCollection()
        {
            foreach (var selectedItem in SelectedPortsColl)
            {
                if (AllPortsColl.Contains(selectedItem))
                    AllPortsColl.Remove(selectedItem);
            }
            return AllPortsColl;
        }

        private void selectedElementsListView_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if there are any selected port names in properties settings.
            if (_trendSelectedPortUIProperties.Count == 0)
                return;

            foreach (var item in _trendSelectedPortUIProperties.Keys)
            {
                if(!SelectedPortsColl.Contains(item) && AllPortsColl.Contains(item))
                    SelectedPortsColl.Add(item);
            }
        }

        private void moveToSelectedElementsBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedPortName = (string)allPortListView.SelectedItem;
            string[] selectedPortNames = new string[allPortListView.SelectedItems.Count];

            if (string.IsNullOrEmpty(selectedPortName))
                return;

            if (selectedPortNames.Count() > 1)
            {
                allPortListView.SelectedItems.CopyTo(selectedPortNames, 0);

                foreach (var selectedPort in selectedPortNames)
                {
                    SelectedPortsColl.Add(selectedPort);
                    // Add port name to properties settings and save.
                    if(_trendView != null)
                        _trendView.AddSelectedPortToAppSettings(selectedPort);
                    else if(_trendViewType20 != null)
                        _trendViewType20.AddSelectedPortToAppSettings(selectedPort);

                    if (AllPortsColl.Contains(selectedPort))
                        AllPortsColl.Remove(selectedPort);
                }

                if (_trendView != null)
                    _trendView.AddSelectedSeriesToTrendData(selectedPortNames);
                else if (_trendViewType20 != null)
                    _trendViewType20.AddSelectedSeriesToTrendData(selectedPortNames);
            } else
            {
                SelectedPortsColl.Add(selectedPortName);
                // Add port name to properties settings and save.
                if (_trendView != null)
                    _trendView.AddSelectedPortToAppSettings(selectedPortName);
                else if (_trendViewType20 != null)
                    _trendViewType20.AddSelectedPortToAppSettings(selectedPortName);

                if (AllPortsColl.Contains(selectedPortName))
                    AllPortsColl.Remove(selectedPortName);

                if (_trendView != null)
                    _trendView.AddSelectedSeriesToTrendData(selectedPortName);
                else if (_trendViewType20 != null)
                    _trendViewType20.AddSelectedSeriesToTrendData(selectedPortName);
            }
        }

        private void moveToAllElementsBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedPortName = (string)selectedElementsListView.SelectedItem;
            string[] selectedPortNames = new string[selectedElementsListView.SelectedItems.Count];
            bool isSelectedDisplayNamesEmpty = false;

            if (string.IsNullOrEmpty(selectedPortName))
                return;

            if (selectedPortNames.Count() > 1)
            {
                selectedElementsListView.SelectedItems.CopyTo(selectedPortNames, 0);

                foreach (var selectedPort in selectedPortNames)
                {
                    AllPortsColl.Add(selectedPort);
                    if (SelectedPortsColl.Contains(selectedPort))
                    {
                        SelectedPortsColl.Remove(selectedPort);
                        if (!isSelectedDisplayNamesEmpty)
                        {
                            if (_trendView != null)
                                _trendView.RemoveSelectedPortFromAppSettings(selectedPort);
                            else if (_trendViewType20 != null)
                                _trendViewType20.RemoveSelectedPortFromAppSettings(selectedPort);
                        }
                    }
                }

                if (_trendView != null)
                    _trendView.RemoveSelectedSeriesFromTrendData(selectedPortNames);
                else if (_trendViewType20 != null)
                    _trendViewType20.RemoveSelectedSeriesFromTrendData(selectedPortNames);
            }
            else
            {
                AllPortsColl.Add(selectedPortName);
                if (SelectedPortsColl.Contains(selectedPortName))
                {
                    SelectedPortsColl.Remove(selectedPortName);
                    if (!isSelectedDisplayNamesEmpty)
                    {
                        if (_trendView != null)
                            _trendView.RemoveSelectedPortFromAppSettings(selectedPortName);
                        else if (_trendViewType20 != null)
                            _trendViewType20.RemoveSelectedPortFromAppSettings(selectedPortName);
                    }
                }

                if (_trendView != null)
                    _trendView.RemoveSelectedSeriesFromTrendData(selectedPortName);
                else if (_trendViewType20 != null)
                    _trendViewType20.RemoveSelectedSeriesFromTrendData(selectedPortName);
            }
        }

        private void allPortListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedElementsListView.SelectedIndex = -1;
            thicknessComboBox.SelectedIndex = -1;
        }

        private void selectedElementsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            thicknessComboBox.SelectedIndex = -1;
        }

        private void colorPicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            SolidColorBrush selectedColor = new SolidColorBrush(colorPicker.Color);
            string selectedPortName = (string)selectedElementsListView.SelectedItem;
            string[] selectedPortNames = new string[selectedElementsListView.SelectedItems.Count];

            if (string.IsNullOrEmpty(selectedPortName))
                return;

            if(selectedPortNames.Count() > 1)
            {
                selectedElementsListView.SelectedItems.CopyTo(selectedPortNames, 0);
                if (_trendView != null)
                    _trendView.ChangeSelectedSeriesColor(selectedPortNames, selectedColor);
                else if (_trendViewType20 != null)
                    _trendViewType20.ChangeSelectedSeriesColor(selectedPortNames, selectedColor);
            }
            else
            {
                if (_trendView != null)
                    _trendView.ChangeSelectedSeriesColor(selectedPortName, selectedColor);
                else if (_trendViewType20 != null)
                    _trendViewType20.ChangeSelectedSeriesColor(selectedPortName, selectedColor);
            }
        }

        private void thicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (thicknessComboBox.SelectedItem == null)
                return;

            int thicknessValue = (int) thicknessComboBox.SelectedItem;
            string selectedPortName = (string)selectedElementsListView.SelectedItem;
            string[] selectedPortNames = new string[selectedElementsListView.SelectedItems.Count];

            if (string.IsNullOrEmpty(selectedPortName))
                return;

            if (selectedPortNames.Count() > 1)
            {
                selectedElementsListView.SelectedItems.CopyTo(selectedPortNames, 0);
                if (_trendView != null)
                    _trendView.ChangeSelectedSeriesThickness(selectedPortNames, thicknessValue);
                else if (_trendViewType20 != null)
                    _trendViewType20.ChangeSelectedSeriesThickness(selectedPortNames, thicknessValue);
            }
            else
            {
                if (_trendView != null)
                    _trendView.ChangeSelectedSeriesThickness(selectedPortName, thicknessValue);
                else if (_trendViewType20 != null)
                    _trendViewType20.ChangeSelectedSeriesThickness(selectedPortName, thicknessValue);
            }
        }
    }
}