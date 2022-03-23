using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Interaction logic for Oscillation.xaml
    /// </summary>
    public partial class Oscillation : UserControl
    {
        #region Fields
        private DispatcherTimer _timer;
        private OscillationVM _viewModel;
        public volatile bool IsControlsEditingMode;
        #endregion

        public Oscillation()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as OscillationVM;
            _viewModel.Oscillation_View = this;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetOscillationDatablock(false);
            _timer.Stop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);
            rbtnAlarm.Focus();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            IsControlsEditingMode = false;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (!IsControlsEditingMode)
            {
                _viewModel.ContinuousUpdate();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
           IsControlsEditingMode = true;
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            IsControlsEditingMode = true;
            
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Tab)
            {
                if (_viewModel.WaitIndicatorControl.IsWaitIndicatorVisible)
                    e.Handled = true;
                else
                    IsControlsEditingMode = false;

                return;
            }

            if (e.Key == Key.Enter)
            {
                TextEdit txtBox = (TextEdit)sender;
                var txtBoxBgColor = txtBox.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                txtBox.Background = loadingYellowColor;
                BindingExpression bindingExpression = txtBox.GetBindingExpression(TextEdit.TextProperty);
                Binding parentBinding = bindingExpression.ParentBinding;
                var oscilllationCriteria = bindingExpression.ResolvedSource as OscillationCriteriaModel;
                bool plcResult = await _viewModel.SetOscillationValueToPLC(oscilllationCriteria, bindingExpression.ResolvedSourcePropertyName);
                txtBox.Background = txtBoxBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!",  MessageBoxButton.OK, MessageBoxImage.Warning);

                IsControlsEditingMode = false;
                FocusableGrid.Focus();
            }

            if (e.Key == Key.Escape)
            {
                IsControlsEditingMode = false;
                FocusableGrid.Focus();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsControlsEditingMode = false;
        }

        private void TextEdit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
              e.DisplayText = e.DisplayText.Replace(",", "");
        }

        private void txtPTCTolerance1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
