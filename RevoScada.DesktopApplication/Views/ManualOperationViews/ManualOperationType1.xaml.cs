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
    /// Interaction logic for Manual_Operation.xaml
    /// </summary>
    public partial class ManualOperationType1 : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private ManualOperationVM _viewModel;
        public DispatcherTimer Timer;
        private bool _isControlsEditingMode;
        #endregion

        #region 3D Autoclave Section Values
        //Model3D device;
        //ModelVisual3D device3D;
        //private const string MODEL_PATH = "Resources/1903 01 00_Tai_Autoclave.stl";
        //private double curVbMaxWidth = 0;
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

        public ManualOperationType1()
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
            if(!_isControlsEditingMode)
            {
                _viewModel.ContinuousUpdate();
            }
        }

        #region Loading Functionality Section
        //private void Worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    //Display3d(MODEL_PATH);
        //}

        //private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = false;
        //    autoclaveGrid.Visibility = Visibility.Visible;
        //}
        #endregion Loading Functionality Section Ends Here

        /// <summary>
        /// Display 3D Model
        /// </summary>
        /// <param name="modelName">Path to the Model file</param>
        /// <returns>3D Model Content</returns>
        //private void Display3d(string modelName)
        //{
        //    this.Dispatcher.Invoke(new Action(() =>
        //    {
        //        device = null;

        //        try
        //        {
        //        //Adding a gesture here
        //        viewPort3d.RotateGesture = new MouseGesture(MouseAction.None);
        //        viewPort3d.ChangeLookAtGesture = new MouseGesture(MouseAction.None);

        //        //Import 3D model file
        //        ModelImporter import = new ModelImporter();

        //        //Load the 3D model file
        //        device = import.Load(modelName);
        //        device3D = new ModelVisual3D();

        //        device3D.Content = device;
        //        //// Add to view port
        //        viewPort3d.Children.Add(device3D);
        //        }
        //        catch (Exception e)
        //        {
        //        // Handle exception in case can not file 3D model
        //        WinUIMessageBox.Show("Exception Error : " + e.StackTrace);
        //        }
        //    }), DispatcherPriority.Background);
        //}

        private void CreateAllPortButtons()
        {
            curPortSp = new StackPanel() { Orientation = Orientation.Horizontal };

            int totalNumOfSections = 0;
            string portName = "PTC";
            Visibility portVisibility = Visibility.Visible;
            Visibility titleVisibility = Visibility.Visible;
            int portIdNum = 0;
            int last_i_ptc = -9;
            int last_i_mon = -9;
            int last_i_vac = -9;
            Thickness buttonSpMargin = new Thickness(0);
            bool IsPtcPortFull = false;
            bool IsMonPortFull = false;
            bool IsVacPortFull = false;
            Random randNum = new Random();
            int randomMaxLength = randNum.Next(1, 3);
            Style portStyle = (Style)FindResource("GenToggleButton_LightBlue");
            SolidColorBrush titleBdColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#D9E8F5");

            for (int i = 1; i <= 300; i += 10)
            {
                StackPanel buttonSp = new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = buttonSpMargin,
                    MaxWidth = 32
                };

                StackPanel reversedButtonSp = new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = buttonSpMargin,
                    MaxWidth = 32
                };

                for (int j = 0; j < 10; j++)
                {
                    reversedButtonSp.Children.Add(new Button());
                }

                for (int b = 0; b < 10; b++)
                {
                    int num = i + b;
                    int currIndex = 10 - b;

                    if (portName == "PTC" && (num > totalPtcPortNum || b > randomMaxLength))
                        portVisibility = Visibility.Hidden;
                    else if (portName == "MON" && (num > totalMonPortNum || b > randomMaxLength))
                        portVisibility = Visibility.Hidden;
                    else if (portName == "VAC" && (num > totalVacPortNum || b > randomMaxLength))
                        portVisibility = Visibility.Hidden;

                    if (num <= totalPtcPortNum || num <= totalMonPortNum || num <= totalVacPortNum)
                        titleVisibility = Visibility.Visible;
                    else
                        titleVisibility = Visibility.Hidden;

                    // Add a title for ports
                    if (currIndex == 10)
                    {
                        Border portTitleBd = new Border()
                        {
                            //Background = titleBdColor,
                            Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#326690"),
                            MinWidth = 32,
                            MinHeight = 30,
                            BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#575757"),
                            BorderThickness = new Thickness(1),
                            Visibility = titleVisibility
                        };
                        TextBlock portTitle = new TextBlock()
                        {
                            Text = portName,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontWeight = FontWeights.SemiBold,
                            Foreground = Brushes.White,
                            FontSize = 11
                        };
                        portTitleBd.Child = portTitle;

                        reversedButtonSp.Children.Add(portTitleBd);
                    }
                    if (b == 0)
                    {
                        Border portTitleBd = new Border()
                        {
                            //Background = titleBdColor,
                            Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#326690"),
                            MinWidth = 32,
                            MinHeight = 30,
                            BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#575757"),
                            BorderThickness = new Thickness(1),
                            Visibility = titleVisibility
                        };
                        TextBlock portTitle = new TextBlock()
                        {
                            Text = portName,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontWeight = FontWeights.SemiBold,
                            Foreground = Brushes.White,
                            FontSize = 11
                        };
                        portTitleBd.Child = portTitle;

                        buttonSp.Children.Add(portTitleBd);
                    }

                    // Add port buttons
                    ToggleButton portBtn = new ToggleButton()
                    {
                        Style = portStyle,
                        MinWidth = 32,
                        FontSize = 12,
                        Content = num,
                        Visibility = portVisibility
                    };

                    ToggleButton portBtnReversed = new ToggleButton()
                    {
                        Style = portStyle,
                        MinWidth = 32,
                        FontSize = 12,
                        Content = num,
                        Visibility = portVisibility
                    };

                    currIndex--;
                    reversedButtonSp.Children.RemoveAt(currIndex);
                    reversedButtonSp.Children.Insert(currIndex, portBtnReversed);

                    buttonSp.Children.Add(portBtn);

                } // Button for loop ends here

                if (curPortSp.Children.Count < 3)
                {
                    if(totalNumOfSections < 8)
                        curPortSp.Children.Add(reversedButtonSp);
                    else
                        curPortSp.Children.Add(buttonSp);
                }
                else
                {
                    VacBtnSp_Coll.Add(curPortSp);

                    curPortSp = new StackPanel() { Orientation = Orientation.Horizontal };
                    totalNumOfSections++;

                    if (totalNumOfSections < 8)
                        curPortSp.Children.Add(reversedButtonSp);
                    else
                        curPortSp.Children.Add(buttonSp);
                }

                if (portIdNum == 0)
                {
                    int remainingRandNum = 9 - randomMaxLength;
                    last_i_ptc = i - remainingRandNum;
                }
                else if (portIdNum == 1)
                {
                    int remainingRandNum = 9 - randomMaxLength;
                    last_i_mon = i - remainingRandNum;
                }
                else if (portIdNum == 2)
                {
                    int remainingRandNum = 9 - randomMaxLength;
                    last_i_vac = i - remainingRandNum;
                }

                portIdNum++;

                if (portIdNum > 2)
                {
                    if (!IsPtcPortFull)
                        portIdNum = 0;
                    else if (!IsMonPortFull)
                        portIdNum = 1;
                    else if (!IsVacPortFull)
                        portIdNum = 2;
                    else
                        break;
                }

                if (portIdNum == 0)
                {
                    i = last_i_ptc;
                    portName = "PTC";
                    portVisibility = Visibility.Visible;
                    buttonSpMargin = new Thickness(0);
                    portStyle = (Style)FindResource("GenToggleButton_LightBlue");
                    titleBdColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#D9E8F5");
                    randomMaxLength = randNum.Next(1, 10);
                }
                else if (portIdNum == 1)
                {
                    i = last_i_mon;
                    portName = "MON";
                    portVisibility = Visibility.Visible;
                    buttonSpMargin = new Thickness(0);
                    portStyle = (Style)FindResource("GenToggleButton_LightYellow");
                    titleBdColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#F1F5D4");
                    randomMaxLength = randNum.Next(1, 10);
                }
                else if (portIdNum == 2)
                {
                    i = last_i_vac;
                    portName = "VAC";
                    portVisibility = Visibility.Visible;
                    buttonSpMargin = new Thickness(0, 0, 10, 0);
                    portStyle = (Style)FindResource("GenToggleButton");
                    titleBdColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#F0F0F0");
                    randomMaxLength = randNum.Next(1, 10);
                }
                else
                    break;
            }
        }

        //private void AutoRotationByDirections(Vector3D lookDir, Vector3D UpDir, Point3D pos)
        //{
        //    allowCamChanged = false;
        //    AllowShowingSections = false;

        //    UpdateSideSections_Delayed();
        //    viewPort3d.Camera.AnimateTo(pos, lookDir, UpDir, 2000);
        //}

        private void UpdateSideSections_Delayed()
        {
            if (posChangeAnim != null)
                posChangeAnim.Begin();
        }

        private void PosChangeAnim_Completed(object sender, EventArgs e)
        {
            AllowShowingSections = true;
            allowCamChanged = true;
            UpdateSideSections();
        }

        #region 3D Control Event Handlers

        #region Camera Direction Buttons
        //private void autoDirToFrontBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var newDir = new Vector3D(-34178.1748, 0, 0);
        //    var upDir = new Vector3D(0, 0, 1);
        //    var newPos = new Point3D(27000, 21.377, 9140.230);

        //    AutoRotationByDirections(newDir, upDir, newPos);
        //}
        //private void autoDirToRightBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var newDir = new Vector3D(0, 34178.175, 0);
        //    var upDir = new Vector3D(0, 0, 1);
        //    var newPos = new Point3D(129.653, -34156.798, 9140.230);

        //    AutoRotationByDirections(newDir, upDir, newPos);
        //}
        //private void autoDirToUpBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var newDir = new Vector3D(0, 0, -45893.755);
        //    var upDir = new Vector3D(0, 1, 0);
        //    var newPos = new Point3D(129.654, 21.377, 42000);

        //    AutoRotationByDirections(newDir, upDir, newPos);
        //}
        //private void autoDirToBackBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var newDir = new Vector3D(32335.196, 0, 0);
        //    var upDir = new Vector3D(0, 0, 1);
        //    var newPos = new Point3D(-27000, 21.377, 9140.230);

        //    AutoRotationByDirections(newDir, upDir, newPos);
        //}
        //private void autoDirToLeftBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var newDir = new Vector3D(0, -32335.196, 0);
        //    var upDir = new Vector3D(0, 0, 1);
        //    var newPos = new Point3D(129.654, 32356.573, 9140.230);

        //    AutoRotationByDirections(newDir, upDir, newPos);
        //}
        //private void autoDirToDownBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var newDir = new Vector3D(0, 0, 34178.175);
        //    var upDir = new Vector3D(0, -1, 0);
        //    var newPos = new Point3D(129.653, 21.377, -25037.945);

        //    AutoRotationByDirections(newDir, upDir, newPos);
        //}
        #endregion

        private void viewPort3d_CameraChanged(object sender, RoutedEventArgs e)
        {
            if (allowCamChanged)
                UpdateSideSections();
        }

        private void UpdateSideSections()
        {
            //lookDirYVal.Text = viewPort3d.Camera.LookDirection.Y.ToString();
            //lookDirXVal.Text = (viewPort3d.Camera.LookDirection.X * -1).ToString(); // Make it positive before assigning.
            //lookDirZVal.Text = (viewPort3d.Camera.LookDirection.Z * -1).ToString();

            //AutoclaveSectionCheck(autoclaveFrontVb, -41000, -20000, -8000, 8000); // Front side check
            //AutoclaveSectionCheck(autoclaveBackVb, 20000, 41000, -8000, 8000); // Back side check
            //AllowShowingSections = true;
        }

        /// <summary>
        /// According to camera positions it decides which section should be visible
        /// </summary>
        /// <param name="autoclave"></param>
        /// <param name="lookDirX_minVal"></param>
        /// <param name="lookDirX_maxVal"></param>
        /// <param name="lookDirY_minVal"></param>
        /// <param name="lookDirY_maxVal"></param>
        //private void AutoclaveSectionCheck(Viewbox autoclave, int lookDirX_minVal, int lookDirX_maxVal, int lookDirY_minVal, int lookDirY_maxVal)
        //{
        //    if (autoclave.Opacity <= 0)
        //    {
        //        if (viewPort3d.Camera.LookDirection.X >= lookDirX_minVal && viewPort3d.Camera.LookDirection.X <= lookDirX_maxVal &&
        //            viewPort3d.Camera.LookDirection.Y >= lookDirY_minVal && viewPort3d.Camera.LookDirection.Y <= lookDirY_maxVal)
        //            autoclaveCamAnim_Show(autoclave);

        //        return;
        //    }

        //    if (curVbMaxWidth != autoclave.MaxWidth)
        //    {
        //        autoclaveCamAnim_TargetUpdated(autoclave);
        //    }

        //    if (viewPort3d.Camera.LookDirection.X >= lookDirX_maxVal || viewPort3d.Camera.LookDirection.X <= lookDirX_minVal ||
        //        viewPort3d.Camera.LookDirection.Y >= lookDirY_maxVal || viewPort3d.Camera.LookDirection.Y <= lookDirY_minVal)
        //        autoclaveCamAnim_Hide(autoclave);
        //}

        /// <summary>
        /// Same behaviour with previous function but Z direction is included to check Top and Bottom sides of 3D model.
        /// </summary>
        /// <param name="autoclave"></param>
        /// <param name="lookDirX_minVal"></param>
        /// <param name="lookDirX_maxVal"></param>
        /// <param name="lookDirY_minVal"></param>
        /// <param name="lookDirY_maxVal"></param>
        /// <param name="lookDirZ_minVal"></param>
        /// <param name="lookDirZ_maxVal"></param>
        //private void AutoclaveSectionCheck_IncZ(Viewbox autoclave, int lookDirX_minVal, int lookDirX_maxVal, int lookDirY_minVal, int lookDirY_maxVal,
        //                                        int lookDirZ_minVal, int lookDirZ_maxVal, int upDirY)
        //{
        //    if (autoclave.Opacity == 0)
        //    {
        //        if (viewPort3d.Camera.LookDirection.X >= lookDirX_minVal && viewPort3d.Camera.LookDirection.X <= lookDirX_maxVal &&
        //            viewPort3d.Camera.LookDirection.Y >= lookDirY_minVal && viewPort3d.Camera.LookDirection.Y <= lookDirY_maxVal &&
        //            viewPort3d.Camera.LookDirection.Z >= lookDirZ_minVal && viewPort3d.Camera.LookDirection.Z <= lookDirZ_maxVal &&
        //            viewPort3d.Camera.UpDirection.Y >= upDirY)
        //            autoclaveCamAnim_Show(autoclave);

        //        return;
        //    }

        //    if (curVbMaxWidth != autoclave.MaxWidth)
        //    {
        //        autoclaveCamAnim_TargetUpdated(autoclave);
        //    }

        //    if (viewPort3d.Camera.LookDirection.X >= lookDirX_maxVal || viewPort3d.Camera.LookDirection.X <= lookDirX_minVal ||
        //        viewPort3d.Camera.LookDirection.Y >= lookDirY_maxVal || viewPort3d.Camera.LookDirection.Y <= lookDirY_minVal ||
        //        viewPort3d.Camera.LookDirection.Z >= lookDirZ_maxVal || viewPort3d.Camera.LookDirection.Z <= lookDirZ_minVal ||
        //        viewPort3d.Camera.UpDirection.Y <= upDirY)
        //        autoclaveCamAnim_Hide(autoclave);
        //}

        private void autoclaveCamAnim_TargetUpdated(FrameworkElement element)
        {
            //var autoclaveCamAnimUpdated = FindResource("autoclaveCamAnim_Updated") as Storyboard;

            //if (autoclaveCamAnimUpdated != null)
            //    autoclaveCamAnimUpdated.Begin(element);

            element.Opacity = 1;
        }

        private void autoclaveCamAnim_Hide(FrameworkElement element)
        {
            //var autoclaveCamAnimHide = FindResource("autoclaveCamAnim_Hide") as Storyboard;

            //if (autoclaveCamAnimHide != null)
            //    autoclaveCamAnimHide.Begin(element);
            element.Opacity = 0;
        }

        private void autoclaveCamAnim_Show(FrameworkElement element)
        {
            //var autoclaveCamAnimShow = FindResource("autoclaveCamAnim_Show") as Storyboard;

            //if (autoclaveCamAnimShow != null)
            //    autoclaveCamAnimShow.Begin(element);

            element.Opacity = 1;
        }
        #endregion

        #region PLC Set Event Handlers
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

                SiemensTagConfiguration siemensTagConfig = (SiemensTagConfiguration) textBox.Tag;
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
               // if (tagName == "VacControlStatusSpRight" && (floatVal < -760 || floatVal > 0))
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

            if(e.Key == Key.Escape)
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
            if (sectionName == "Vacuum" && (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue))
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
        #endregion

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
    }
}