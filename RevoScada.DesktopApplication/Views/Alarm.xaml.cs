using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
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

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Alarm.xaml
    /// </summary>
    public partial class Alarm : UserControl
    {
        #region Fields
        private AlarmVM _viewModel;
        private DispatcherTimer _timer;
        private BackgroundWorker _bgWorker;
        #endregion

        private Action _action;
        public Alarm()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as AlarmVM;
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += acknowledgeReset_DoWork;
            _bgWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            _viewModel.IncomingAlarmsChecker.Value = false;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
        private void acknowledgeReset_DoWork(object sender, DoWorkEventArgs e)
        {
            _action.Invoke();
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.IsEditingMode = false;
            alarmListView.IsEnabled = true;
            btnAcknowledgeReset.IsEnabled = true;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_viewModel.IsEditingMode)
            {
                _viewModel.UpdatePlcAlarms();
                _viewModel.UpdateEvents();
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,  TimeSpan.FromSeconds(1));
            this.BeginAnimation(OpacityProperty, animation);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void btnAcknowledgeReset_Click(object sender, RoutedEventArgs e)
        {
            btnAcknowledgeReset.IsEnabled = false;
            alarmListView.IsEnabled = false;
            _viewModel.IsEditingMode = true;
            _action = new Action(() => { _viewModel.AcknowledgeReset(); });

            if (!_bgWorker.IsBusy)
                _bgWorker.RunWorkerAsync();
        }

        private void allFurnaceAlarmsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            CheckBox checkBox = (CheckBox) sender;

            bool isChecked = checkBox.IsChecked ?? false;
            _viewModel.SelectOrDeselectAllFurnaceAlarms(isChecked);
        }

        private void SetAlarmWarnedState_Completed(object sender, EventArgs e)
        {
            // If all items have already been warned, then return
            if (!_viewModel.FurnaceAlarmData.Where(f => !f.HasWarned).Any())
                return;

            foreach (var alarm in _viewModel.FurnaceAlarmData)
            {
                alarm.HasWarned = true;
            }
        }

        private void forcePageLoadCb_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            CheckBox checkBox = (CheckBox)sender;
            _viewModel.UpdateAlarmPageForceLoadOption(checkBox.IsChecked ?? false);
        }
    }
}
