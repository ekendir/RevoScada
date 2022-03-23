using DevExpress.Xpf.Editors;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities.Configuration;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for PNI_Full_Screen_Text_Edit.xaml
    /// </summary>
    public partial class PNI_Full_Screen_Text_Edit : Window, INotifyPropertyChanged
    {
        #region Fields
        private SiemensTagConfiguration _siemensTagConfiguration;
        private PipingAndInstrumentationVM _pipingAndInstrumentationVM;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties
        private float _value;

        public float Value
        {
            get => _value;
            set => OnPropertyChanged(ref _value, value);
        }
        #endregion

        public PNI_Full_Screen_Text_Edit(bool isNumericOnly, float startValue,
                                             PipingAndInstrumentationVM pipingAndInstrumentationVM, SiemensTagConfiguration siemensTagConfiguration)
        {
            InitializeComponent();

            DataContext = this;
            Value = startValue;
            _pipingAndInstrumentationVM = pipingAndInstrumentationVM;
            _siemensTagConfiguration = siemensTagConfiguration;
            SetMaskTypeOfValueTextBox(isNumericOnly);
        }

        /// <summary>
        /// Sets the mask type of the desired textbox according to conditions
        /// </summary>
        /// <param name="isNumericOnly"></param>
        private void SetMaskTypeOfValueTextBox(bool isNumericOnly)
        {
            if (isNumericOnly)
                valueBox.MaskType = MaskType.Numeric;
            else
                valueBox.MaskType = MaskType.Simple;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            float toBeSentValue = (float)Convert.ToDouble(Value);
            await _pipingAndInstrumentationVM.SetToPlc(toBeSentValue,"","");

            this.Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                applyBtn_Click(applyBtn, null);
        }
    }
}