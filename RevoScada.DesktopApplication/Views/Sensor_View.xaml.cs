using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Sensor_View.xaml
    /// </summary>
    public partial class Sensor_View : UserControl
    {
        #region Fields
        private SensorViewVM _viewModel;
        private DispatcherTimer _timer;
        #endregion

        public Sensor_View()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as SensorViewVM;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));

            this.BeginAnimation(UIElement.OpacityProperty, animation);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _viewModel.ContinuousUpdate();
        }

        private void activeItemsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.FilterSensorItems(true);
        }

        private void allItemsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.FilterSensorItems(false);
        }

        private void activeItemsRadioBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            int filterIndex = (int)_viewModel.SensorViewFilterSettings.GeneralFilter;
            if (filterIndex == 1)
                activeItemsRadioBtn.IsChecked = true;
        }

        private void allItemsRadioBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            int filterIndex = (int)_viewModel.SensorViewFilterSettings.GeneralFilter;
            if (filterIndex == 0)
                allItemsRadioBtn.IsChecked = true;
        }

        private void tempBagButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            ToggleButton toggBtn = (ToggleButton)sender;
            int filterIndex = (int)_viewModel.SensorViewFilterSettings.PartPtcFilter;
            if (filterIndex == 0)
            {
                toggBtn.IsChecked = false;
            }
            else
            {
                toggBtn.IsChecked = true;
                toggBtn.Command.Execute(toggBtn.CommandParameter);
            }
        }

        private void vacBagButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            ToggleButton toggBtn = (ToggleButton)sender;
            int filterIndex = (int)_viewModel.SensorViewFilterSettings.PartVacFilter;
            if (filterIndex == 0)
            {
                toggBtn.IsChecked = false;
            }
            else
            {
                toggBtn.IsChecked = true;
                toggBtn.Command.Execute(toggBtn.CommandParameter);
            }
        }
    }
}
