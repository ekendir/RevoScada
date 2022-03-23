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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Reports_Window.xaml
    /// </summary>
    public partial class Reports_Window : Window
    {
        #region Fields
        public object MainView { get; set; }
        public AppViewModel _appViewModel { get; set; }
        #endregion

        public Reports_Window(object mainView, AppViewModel appViewModel)
        {
            InitializeComponent();
            DataContext = this;
            MainView = mainView;
            _appViewModel = appViewModel;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _appViewModel.MainWindow.DeselectReportsButton();
        }
    }
}
