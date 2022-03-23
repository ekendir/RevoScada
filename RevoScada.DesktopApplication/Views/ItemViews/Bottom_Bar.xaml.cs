using DevExpress.Xpf.WindowsUI;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RevoScada.DesktopApplication.Views.ItemViews
{
    /// <summary>
    /// Interaction logic for Bottom_Bar.xaml
    /// </summary>
    public partial class Bottom_Bar : UserControl
    {
        #region Fields
        private Pdf_Viewer _pdfViewer;
        private AppViewModel _viewModel;
        #endregion

        public Bottom_Bar()
        {
            InitializeComponent();
        }
        private void btnAppEmergency_Click(object sender, RoutedEventArgs e)
        {
            EmergencyView emergencyView = new EmergencyView();
            emergencyView.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            emergencyView.ShowDialog();
        }

        private void btnAppSettings_Click(object sender, RoutedEventArgs e)
        {
            ScadaSettings scadaSettings = new ScadaSettings();
            scadaSettings.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            scadaSettings.ShowDialog();
        }

        private  void btnExitApplication_Click(object sender, RoutedEventArgs e)
        {

            var systemShutdown = WinUIMessageBox.Show("Sistemden çıkmak istediğinize emin misiniz?", "",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (systemShutdown == MessageBoxResult.No)
                return;

            bool canShutDown = true;

            if (canShutDown)
            {
                Application.Current.Shutdown();
            }

        }

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            _pdfViewer = new Pdf_Viewer();
            _pdfViewer.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _pdfViewer.ShowDialog();
        }

        private void BtnSwitchFurnaces_Click(object sender, RoutedEventArgs e)
        {

            var switchFurnaceDialogResult = WinUIMessageBox.Show("Are you sure to switch furnaces! (Fırın seçimini değiştirmek istediğinizden emin misiniz?)", "",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (switchFurnaceDialogResult == MessageBoxResult.No)
                return;

            bool canShutDown = true;

            if (canShutDown)
            {
               ProcessStartInfo Info = new ProcessStartInfo();
               Info.FileName = @"C:\RevoScada.TAI.Files\Application\RevoScada.DesktopApplication.exe";
               Process.Start(Info);
               Application.Current.Shutdown();
            }

        }

        private void activeUserBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isUserSigned = false;
            if (_viewModel?.ActiveUser != null)
                isUserSigned = true;

            Login_Window login_Window = new Login_Window(_viewModel, isUserSigned);
            login_Window.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            login_Window.ShowDialog();
        }

        private async Task DelayedLoginWindow()
        {
            await Task.Delay(20);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as AppViewModel;

            if (_viewModel.ByPassUser != null)
                return;

            await DelayedLoginWindow();
            activeUserBtn_Click(activeUserBtn, null);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            VersionNameText.Text = $"V{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}
