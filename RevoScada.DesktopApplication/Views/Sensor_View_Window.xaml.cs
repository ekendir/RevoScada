using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Sensor_View_Window.xaml
    /// </summary>
    public partial class Sensor_View_Window : Window
    {
        public object MainView { get; set; }
        private AppViewModel _appViewModel;

        public Sensor_View_Window(AppViewModel appViewModel)
        {
            InitializeComponent();
            DataContext = this;
            _appViewModel = appViewModel;

            Screen desiredScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (desiredScreen == null)
                desiredScreen = Screen.AllScreens.FirstOrDefault();

            this.Left = desiredScreen.WorkingArea.Left;
            this.Top = desiredScreen.WorkingArea.Top;
            this.Width = desiredScreen.WorkingArea.Width;
            this.Height = desiredScreen.WorkingArea.Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _appViewModel.IsSensorViewOpenedInWindow = false;
        }
    }
}