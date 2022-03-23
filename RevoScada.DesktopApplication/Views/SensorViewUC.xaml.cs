
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using DevExpress.XtraGrid.Views.Grid;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for SensorViewUC.xaml
    /// </summary>
    public partial class SensorViewUC : UserControl
    {
        #region Fields
        private SensorViewVM _viewModel;
        private DispatcherTimer _timer;
        private bool _isEditingMode;
        #endregion

        public SensorViewUC()
        {
            InitializeComponent();
            var rowStylePTC = new Style(typeof(GridRowContent));
            var rowStyleMON = new Style(typeof(GridRowContent));
            rowStylePTC.Setters.Add(new Setter(GridRowContent.BackgroundProperty, new Binding("DataContext.RowColor")));
            rowStyleMON.Setters.Add(new Setter(GridRowContent.BackgroundProperty, new Binding("DataContext.RowColor")));
            TVPTCData.RowStyle = rowStylePTC;
            TVMONData.RowStyle = rowStyleMON;
            _isEditingMode = false;
        }

        #region Events
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as SensorViewVM;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetSensorViewDatablock(false);
            _timer.Stop();
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(2000);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isEditingMode)
            {
                _viewModel.UpdateSensorData();
            }
        }
        private async void EnableDisableButton_Click(object sender, RoutedEventArgs e)
        {

            var sensorViewItemsTableRow = (e.OriginalSource as SimpleButton).Tag as SensorViewItemsTableRow;
            string stateText = (sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4) ? "disable" : "enable";
            string stateTextTurkish = (sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4) ? "kapatmak" : "açmak";
            MessageBoxResult dialogResult = WinUIMessageBox.Show($"Are you sure to {stateText}? (Sensorü {stateTextTurkish} istediğinize emin misiniz?)", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                //todo:l refactor language support.
                await _viewModel.TogglePortUsageButtonEnable(sensorViewItemsTableRow);
                await Task.Delay(1000);
            }
        }
        private void GenGridView_FilterChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.SaveSensorViewFilterSettings();
        }
        #endregion
    }
}