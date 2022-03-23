using RevoScada.Configurator;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for FurnaceSelector.xaml
    /// </summary>
    public partial class FurnaceSelector : Window
    {

        private FurnaceSelectorVM _viewModel;

          

        public FurnaceSelector()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new FurnaceSelectorVM();
            DataContext = _viewModel;

            var cycleThread = new Thread(ContinuousUpdate)
            {
                IsBackground = true
            };
            cycleThread.Start();
        }

        private void ContinuousUpdate()
        {
            do
            {

                this.Dispatcher.Invoke(new Action(() =>{
                _viewModel.RefreshFurnaces();
                }));

                Thread.Sleep(2000);

            } while (true);
        }

        private void BtnOpenFurnace_Click(object sender, RoutedEventArgs e)
        {
            var selectionButton = sender as Button;
            int selectedPlcDeviceId = Convert.ToInt32(selectionButton.CommandParameter);

           
            PlcDeviceId = selectedPlcDeviceId;

            ApplicationConfigurations.Instance.RedefineSelectedFurnace(PlcDeviceId);
            var currentWindow = GetWindow(selectionButton);

            if (currentWindow == null)
                return;

            currentWindow.Close();
        }

        public int PlcDeviceId { get; set; }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
