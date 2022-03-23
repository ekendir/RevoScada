using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using System.Text.RegularExpressions;
using RevoScada.Entities.Configuration;
using System.Globalization;
using DevExpress.Xpf.WindowsUI;

namespace RevoScada.DesktopApplication.Views.ManualOperationViews
{
    /// <summary>
    /// Interaction logic for ManualOperationType2.xaml
    /// </summary>
    public partial class ManualOperationType4 : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private ManualOperationVM _viewModel;
        public DispatcherTimer Timer;
        private bool _isControlsEditingMode;
        #endregion

        #region 3D Autoclave Section Values
        Model3D device;
        ModelVisual3D device3D;
        private const string MODEL_PATH = "Resources/1903 01 00_Tai_Autoclave.stl";
        private double curVbMaxWidth = 0;
        #endregion

        #region Vacuum Port Section Values
        private readonly Dispatcher dispatcher;
        private BackgroundWorker bgWorker;
        private int totalVacPortNum = 240;
        private int totalPtcPortNum = 120;
        private int totalMonPortNum = 100;
        private int portSectionNum = 8;
        private StackPanel curPortSp;
        private bool allowCamChanged;
        public int AutoclaveVbMaxWidth { get; set; }
        private bool _allowShowingSections;
        public bool AllowShowingSections
        {
            get
            {
                return _allowShowingSections;
            }
            set
            {
                OnPropertyChanged(ref _allowShowingSections, value);
            }
        }
        #endregion

        #region Storyboard Properties
        Storyboard posChangeAnim;
        #endregion

        #region Collections
        private List<StackPanel> VacBtnSp_Coll;
        private List<StackPanel> PtcBtnSp_Coll;
        private List<StackPanel> MonBtnSp_Coll;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ManualOperationType4()
        {
            InitializeComponent();

            allowShowingSections.DataContext = this;
            dispatcher = Dispatcher.CurrentDispatcher;
            //autoclaveTopVbPorts.DataContext = this;
            //autoclaveBottomVbPorts.DataContext = this;

            AllowShowingSections = true;
            allowCamChanged = true;

            posChangeAnim = FindResource("posChange") as Storyboard;
            posChangeAnim.Completed += PosChangeAnim_Completed;

            // Initial camera positions
            //viewPort3d.Camera.LookDirection = new Vector3D(0, 0, -45893.755);
            //viewPort3d.Camera.UpDirection = new Vector3D(0, 1, 0);
            //viewPort3d.Camera.Position = new Point3D(129.654, 21.377, 42000);

            //viewPort3d.ZoomAroundMouseDownPoint = false;
            //viewPort3d.IsZoomEnabled = false;

            //lookDirYVal.Text = viewPort3d.Camera.LookDirection.Y.ToString();
            //lookDirXVal.Text = viewPort3d.Camera.LookDirection.X.ToString();

            VacBtnSp_Coll = new List<StackPanel>();
            PtcBtnSp_Coll = new List<StackPanel>();
            MonBtnSp_Coll = new List<StackPanel>();

            //CreateAllPortButtons();

            //Display3d(MODEL_PATH);
            //bgWorker = new BackgroundWorker(); //Initializing the worker object
            //bgWorker.DoWork += Worker_DoWork; //Binding Worker_DoWork method
            //bgWorker.RunWorkerCompleted += Worker_RunWorkerCompleted; //Binding worker_RunWorkerCompleted method

            autoclaveGrid.Visibility = Visibility.Visible;

            //bgWorker.RunWorkerAsync(); //Executing the worker
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as ManualOperationVM;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));

            this.BeginAnimation(UIElement.OpacityProperty, animation);

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(2000);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isControlsEditingMode)
            {
                _viewModel.ContinuousUpdate();
            }
        }

        private void UpdateSideSections_Delayed()
        {
            if (posChangeAnim != null)
                posChangeAnim.Begin();
        }
        private void PosChangeAnim_Completed(object sender, EventArgs e)
        {
            AllowShowingSections = true;
            allowCamChanged = true;
            //UpdateSideSections();
        }
        private async void GeneralTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _isControlsEditingMode = true;

            if (e.Key == Key.Tab)
            {
                _isControlsEditingMode = false;
                return;
            }

            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                if (textBox.Tag == null || _viewModel == null)
                    return;

                var txtBoxBgColor = textBox.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                textBox.Background = loadingYellowColor;

                // Get binding name
                BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TagProperty);
                var tagName = bindingExpression.ResolvedSourcePropertyName;

                SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)textBox.Tag;
                string textboxVal = textBox.Text;

                if (string.IsNullOrEmpty(textboxVal))
                    textboxVal = "0";

                //NumberStyles.Any, CultureInfo.CurrentCulture 
                float floatVal = float.Parse(textboxVal, CultureInfo.InvariantCulture.NumberFormat);

                // Vacuum check set value
                if (tagName == "VacControlStatusSp" && (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue))
                {
                    // Set back color to textbox
                    textBox.Background = txtBoxBgColor;
                    _isControlsEditingMode = false;
                    tempBorder.Focus();
                    WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (tagName == "VacControlStatusSpRight" && (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue))
                {
                    // Set back color to textbox
                    textBox.Background = txtBoxBgColor;
                    _isControlsEditingMode = false;
                    tempBorder.Focus();
                    WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Pressure check set value
                if (tagName == "PressureControlStatusSp" && (floatVal < 0 || floatVal > 16))
                {
                    // Set back color to textbox
                    textBox.Background = txtBoxBgColor;
                    _isControlsEditingMode = false;
                    tempBorder.Focus();
                    WinUIMessageBox.Show("Girilen değer 0'dan büyük 16'dan küçük olmalıdır.", "Başarısız",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, floatVal, tagName);

                textBox.Background = txtBoxBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    textBox.Text = floatVal.ToString();

                _isControlsEditingMode = false;
                tempBorder.Focus();
            }

            if (e.Key == Key.Escape)
            {
                _isControlsEditingMode = false;
                tempBorder.Focus();
            }
        }

        private void GeneralTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox textBox = (TextBox)sender;

            // dot and hyphen (-) added.
            Regex regex = new Regex(@"^[,.0-9-]{0,15}$");

            Match match = regex.Match(textBox.Text);

            if (!match.Success)
            {
                textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);
            }

            // If text contains a coma then change it with dot.
            if (textBox.Text.Contains(','))
            {
                textBox.Text = textBox.Text.Replace(',', '.');
            }
        }

        private async void AddOrDecreaseThenSetToPlc(float increaseAmount, string sectionName)
        {
            _isControlsEditingMode = true;
            TextBox textBox = new TextBox();

            switch (sectionName)
            {
                case "Temperature":
                    textBox = ptcSetValueTb;
                    break;
                case "Pressure":
                    textBox = pressureSetValueTb;
                    break;
                case "Vacuum":
                    textBox = vacuumSetValueTb;
                    break;
                case "VacuumRight":
                    textBox = vacuumSetValueTbR;
                    break;

                default:
                    break;
            }

            if (textBox.Tag == null || _viewModel == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            var txtBoxBgColor = textBox.Background;
            SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            textBox.Background = loadingYellowColor;

            // Get binding name
            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)textBox.Tag;

            string textboxVal = textBox.Text;

            if (string.IsNullOrEmpty(textboxVal))
                textboxVal = "0";

            float floatVal = float.Parse(textboxVal, CultureInfo.InvariantCulture.NumberFormat);

            // Increase value
            floatVal += increaseAmount;

            // Vacuum check set value
            if ((sectionName == "Vacuum" || sectionName == "VacuumRight") && (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue))
            {
                // Set back color to textbox
                textBox.Background = txtBoxBgColor;
                _isControlsEditingMode = false;
                WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Pressure check set value
            if (tagName == "PressureControlStatusSp" && (floatVal < 0 || floatVal > 16))
            {
                // Set back color to textbox
                textBox.Background = txtBoxBgColor;
                _isControlsEditingMode = false;
                tempBorder.Focus();
                WinUIMessageBox.Show("Girilen değer 0'dan büyük 16'dan küçük olmalıdır.", "Başarısız",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            textBox.Text = floatVal.ToString();

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, floatVal, tagName);

            textBox.Background = txtBoxBgColor;

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }
        private void GeneralTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Get binding name
            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            switch (tagName)
            {
                case "PtcValue":
                    _viewModel.AllowUpdatingTempValue = false;
                    ptcSetValueTb.Text = "0";
                    break;
                case "PressureControlStatusSp":
                    _viewModel.AllowUpdatingPressureValue = false;
                    pressureSetValueTb.Text = "0";
                    break;
                case "VacControlStatusSp":
                    _viewModel.AllowUpdatingVacuumValue = false;
                    vacuumSetValueTb.Text = "0";
                    break;
                case "VacControlStatusSpRight":
                    _viewModel.AllowUpdatingVacuumValue = false;
                    vacuumSetValueTbR.Text = "0";
                    break;
                default:
                    break;
            }
        }

        private void GeneralTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Get binding name
            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            switch (tagName)
            {
                case "PtcValue":
                    _viewModel.AllowUpdatingTempValue = true;
                    ptcSetValueTb.Text = _viewModel.FurnaceControlFormInput.PtcValue.ToString();
                    break;
                case "PressureControlStatusSp":
                    _viewModel.AllowUpdatingPressureValue = true;
                    pressureSetValueTb.Text = _viewModel.FurnaceControlFormInput.PressureValue.ToString();
                    break;
                case "VacControlStatusSp":
                    _viewModel.AllowUpdatingVacuumValue = true;
                    vacuumSetValueTb.Text = _viewModel.FurnaceControlFormInput.VacControlStatusSp.ToString();
                    break;
                case "VacControlStatusSpRight":
                    _viewModel.AllowUpdatingVacuumValue = true;
                    vacuumSetValueTbR.Text = _viewModel.FurnaceControlFormInput.VacControlStatusSpRight.ToString();
                    break;
                default:
                    break;
            }
        }

        private async void RbAuto_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 1;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "Auto");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbMan_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 0;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "Manual");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbOn_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 1;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "enabled");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbOff_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 0;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "disabled");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        #region Increase - Decrease Buttons
        private void IncreaseOne_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(1, sectionName);
        }

        private void IncreaseZeroDotOne_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(0.1f, sectionName);
        }

        private void IncreaseTen_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(10, sectionName);
        }

        private void DecreaseOne_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(-1, sectionName);
        }

        private void DecreaseZeroDotOne_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(-0.1f, sectionName);
        }

        private void DecreaseTen_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(-10, sectionName);
        }
        #endregion

        private async void RbAzot_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 1;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "Azot");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbAir_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 0;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "Air");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbOnRight_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 1;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "enabled");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbOffRight_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 0;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "disabled");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbManRight_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 0;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "Manual");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbAutoRight_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
            {
                _isControlsEditingMode = false;
                return;
            }

            // Get binding name
            BindingExpression bindingExpression = radioButton.GetBindingExpression(ToggleButton.TagProperty);
            var tagName = bindingExpression.ResolvedSourcePropertyName;

            SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration)radioButton.Tag;
            int setValue = 1;

            bool plcResult = await _viewModel.SetToPlc(siemensTagConfig, setValue, tagName, "Auto");

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private void IncreaseTenRight_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(10, sectionName);
        }

        private void IncreaseOneRight_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(1, sectionName);
        }

        private void DecreaseTenRight_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(-10, sectionName);
        }

        private void DecreaseOneRight_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag == null)
                return;

            string sectionName = (string)button.Tag;
            AddOrDecreaseThenSetToPlc(-1, sectionName);
        }
    }
}
