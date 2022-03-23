using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.Entities.Configuration;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Piping_and_Instrumentation_Diagram_Window.xaml
    /// </summary>
    public partial class Piping_and_Instrumentation_Diagram_Window : Window
    {
        #region Fields
        private DispatcherTimer _timer;
        private PipingAndInstrumentationVM _pipingAndInstrumentationVM;

        private bool _isControlsEditingMode;
        #endregion

        public Piping_and_Instrumentation_Diagram_Window()
        {
            InitializeComponent();
            _pipingAndInstrumentationVM = new PipingAndInstrumentationVM(this);
            DataContext = _pipingAndInstrumentationVM;
        }                                                                           

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isControlsEditingMode)
            {
                _pipingAndInstrumentationVM.ContinuousUpdate();
            }
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void kp1Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            //TextEdit textEdit = (TextEdit)sender;
            //if (textEdit.Tag == null) // type of textEdit.Tag == Int64 120353
            //    return;

            //float kpLastValue = _pipingAndInstrumentationVM.PipingAndInstrumentationFurnaceControlModel.KpPanelTemperature;
            
            //// Create popup window
            //PNI_Full_Screen_Text_Edit fullScreenTextEdit = new PNI_Full_Screen_Text_Edit(true, kpLastValue,
            //                                                                                     _pipingAndInstrumentationVM, (SiemensTagConfiguration)_pipingAndInstrumentationVM.PipingAndInstrumentationTagConfigurations.KpPanelTemperature);
            //fullScreenTextEdit.ShowDialog();
            //kpSecSp.Focus();
        }

     
        private void generalTextNameSender_GotFocus(object sender, RoutedEventArgs e)
        {
            TextEdit txtBox = (TextEdit)sender;

            if (txtBox == null)
                return;

            _pipingAndInstrumentationVM.SenderPropertyName = txtBox.Name;
        }

       private void generalValveOnOff_ShowControl_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Controls.Primitives.ToggleButton toggleButton = (System.Windows.Controls.Primitives.ToggleButton)sender;

            Button button = (Button)sender;

            if (button == null)
                return;

            _pipingAndInstrumentationVM.SenderPropertyName = button.Name;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
    }
}