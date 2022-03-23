using DevExpress.Xpf.WindowsUI;
using RevoScada.ProcessController;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for EmergencyView.xaml
    /// </summary>
    public partial class EmergencyView : Window
    {
        private DispatcherTimer _timer;
        private EmergencyVM _viewModel;

        public EmergencyView()
        {
            _viewModel = new EmergencyVM();
            DataContext = _viewModel;

            InitializeComponent();
        }

       

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateWriteCommandList();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(2000);
            _timer.Tick += _timer_Tick;
            _timer.Start(); 
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _viewModel.UpdateWriteCommandList();
        }

        private void btnEmptyList_Click(object sender, RoutedEventArgs e)
        {

            var startRunResult = WinUIMessageBox.Show("Proses kuyruğu silinecektir.", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (startRunResult == MessageBoxResult.No)
                return;

            startRunResult = WinUIMessageBox.Show("Kuyrukta bekleyen işlemleriniz silinecektir! İptal etmek için Cancel tuşuna basın! ", "", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (startRunResult == MessageBoxResult.Cancel)
                return;

            ProcessManager.Instance.ResetSiemensWriteCommandItems();
        }

        private void btnStopAllProcess_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
