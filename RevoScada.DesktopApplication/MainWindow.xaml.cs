using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.WindowsUI;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities.Enums;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Views;
using System.Diagnostics;
using RevoScada.DesktopApplication.Models.ModelEnums;
using System.Linq;
using System.Globalization;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using System.Windows.Threading;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace RevoScada.DesktopApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        #region Services
        private readonly string _connectionString;
        BatchService _batchService;
        ActiveTagService _activeTagService;

        #endregion

        #region Fields
        private ToggleButton[] _menuBarButtons;
        private AppViewModel _viewModel;
        private Thread _continuousMainCycleThread;
        private bool _IsUsagePriorityCheckShown = false;
        private bool _isContinuousMainCycleCompleted = true;
        private object _lockUpdateObject = new object();

        private DispatcherTimer _activityTimer;
        private DispatcherTimer _signOutTimer;
        private DispatcherTimer _showUserLogWindowTimer;
        private Point _inactiveMousePosition = new Point(0, 0);
        private string _lastSelectedBtnName;
        #endregion

        public MainWindow()
        {

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            FurnaceSwicther furnaceSwicther = new FurnaceSwicther();
            bool isDefineFailed = furnaceSwicther.DefineFurnaceSelection();

            InitializeComponent();

            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            _batchService = new BatchService(_connectionString);
            _activeTagService = new ActiveTagService(_connectionString);

            _menuBarButtons = new ToggleButton[] { VacuumBtn, EnterPartsBtn, RecipeEditBtn, IntegrityChecksBtn, SensorViewBtn, RunOperBtn, ManOperationBtn,
                                                   TrendBtn, AlarmBtn, RecipeBtn, ReportsBtn, QualityBtn, CalibrationBtn, OscillationBtn, UserManagementBtn};
        }

        /// <summary>
        /// When program first loaded, change style of loaded page's menu bar button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var logoImageFromConfiguration = new BitmapImage();
            logoImageFromConfiguration.BeginInit();
            logoImageFromConfiguration.UriSource = new Uri(ApplicationConfigurations.Instance.Configuration.ApplicationLogoFullPath);
            logoImageFromConfiguration.EndInit();
            ImageBehavior.SetAnimatedSource(ApplicationLogo, logoImageFromConfiguration);

            _viewModel = DataContext as AppViewModel;
            _viewModel.MainWindow = this;

            if (_viewModel.ByPassUser != null)
                _viewModel.ActiveUser = _viewModel.ByPassUser;

            _viewModel.FurnaceSelectionMenuButtonVisibility = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == Entities.Configuration.Service.WorkingEnvironment.server ? "Visible" : "Collapsed";
            _continuousMainCycleThread = new Thread(ContinuousMainCycle);
            _continuousMainCycleThread.IsBackground = true;
            _continuousMainCycleThread.Start();
        }

        public void StartAutoLogout()
        {
            if (_viewModel.ActiveUser == null)
                return;

            // First get logout time of the active user
           short logoutMin =  _viewModel.ActiveUser.LogoutTime;

            if (logoutMin == 0)
                logoutMin = 1;

            InputManager.Current.PreProcessInput += OnActivity;
            _activityTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(logoutMin), IsEnabled = true };
            _activityTimer.Tick += OnInactivity;

            _signOutTimer = new DispatcherTimer();
            _signOutTimer.Interval = TimeSpan.FromSeconds(10);
            _signOutTimer.Tick += SignOutTimer_Tick;
        }

        private void OnInactivity(object sender, EventArgs e)
        {
            // Remember mouse position
            _inactiveMousePosition = Mouse.GetPosition(MainGrid);

            // If user has signed-in, then sign-out
            StartInActivityWarning();
        }

        private void OnActivity(object sender, PreProcessInputEventArgs e)
        {
            InputEventArgs inputEventArgs = e.StagingItem.Input;

            if (inputEventArgs is MouseEventArgs || inputEventArgs is KeyboardEventArgs)
            {
                if (e.StagingItem.Input is MouseEventArgs)
                {
                    MouseEventArgs mouseEventArgs = (MouseEventArgs)e.StagingItem.Input;

                    // No button is pressed and the position is still the same as the application became inactive
                    if (mouseEventArgs.LeftButton == MouseButtonState.Released &&
                        mouseEventArgs.RightButton == MouseButtonState.Released &&
                        mouseEventArgs.MiddleButton == MouseButtonState.Released &&
                        mouseEventArgs.XButton1 == MouseButtonState.Released &&
                        mouseEventArgs.XButton2 == MouseButtonState.Released &&
                        _inactiveMousePosition == mouseEventArgs.GetPosition(MainGrid))
                        return;
                }

                if (_viewModel.InActivityPanelVisibility == Visibility.Visible)
                {
                    _signOutTimer.Stop();
                    _viewModel.InActivityPanelVisibility = Visibility.Collapsed;
                }

                _activityTimer.Stop();
                _activityTimer.Start();
            }
        }

        private void SignOutTimer_Tick(object sender, EventArgs e)
        {
            _signOutTimer.Stop();
            SignOutUser();
        }

        private void StartInActivityWarning()
        {
            if (!_viewModel.CheckIfWarningPanelsClosed())
                return;

            _viewModel.InActivityPanelVisibility = Visibility.Visible;
            CloseOpenedWindows();
            _signOutTimer.Start();
        }

        private void SignOutUser()
        {
            if (_viewModel.ActiveUser == null)
                return;

            if (_viewModel.ByPassUser != null)
                _viewModel.ByPassUser = null;

            _viewModel.ActiveUser = null;
            _viewModel.InActivityPanelVisibility = Visibility.Collapsed;
            _viewModel.AccountWarningPanelVisibility = Visibility.Visible;
            _viewModel.UnloadPage(true);
            _viewModel.ActiveUser = null;
        }


        private void ContinuousMainCycle()
        {
            while (true)
            {
                if (_isContinuousMainCycleCompleted)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    lock (this._lockUpdateObject)
                    {
                        bool shutdownInvoked = false;
                        DateTime lastCycleRunTime = ProcessManager.Instance.LastReadStatus().LastCycleRunTime;
                        int diffInSeconds = Convert.ToInt32((DateTime.Now - lastCycleRunTime).TotalMilliseconds / 1000);
                        _viewModel.PlcStatusChecker = (diffInSeconds < 30);//!!
                        _viewModel.IsAllServicesRunning = ProcessManager.Instance.IsAllServicesRunning();

                        if (ProcessManager.Instance.IsAllServicesRunning() && _viewModel.PlcStatusChecker && _viewModel.ServiceWarningPanelVisibility == Visibility.Visible)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                _viewModel.ServiceWarningPanelVisibility = Visibility.Collapsed;
                            }));
                        }
                        else if (!(ProcessManager.Instance.IsAllServicesRunning() && _viewModel.PlcStatusChecker) && _viewModel.ServiceWarningPanelVisibility != Visibility.Visible)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                _viewModel.ServiceWarningPanelVisibility = Visibility.Visible;
                            }));
                        }

                        if (_viewModel.IsValidMaster && _viewModel.MasterWarningPanelVisibility != Visibility.Collapsed)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                //network Check alarm = 0
                                _viewModel.MasterWarningPanelVisibility = Visibility.Collapsed;
                            }));

                        }
                        else
                        if (!_viewModel.IsValidMaster && _viewModel.MasterWarningPanelVisibility != Visibility.Visible)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                //network Check alarm = 1
                                _viewModel.MasterWarningPanelVisibility = Visibility.Visible;
                            }));
                            _IsUsagePriorityCheckShown = true;
                        }
                        else if (!_viewModel.IsValidMaster && _IsUsagePriorityCheckShown)
                        {
                            if (ProcessManager.Instance.IsAllServicesRunning() && _viewModel.PlcStatusChecker)
                            {
                                if (ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == Entities.Configuration.Service.WorkingEnvironment.pc)
                                {
                                    if ( _viewModel.PlcStatusChecker == true)
                                    {
                                        byte tryAmount = 15;
                                        
                                        do
                                        {
                                            if (tryAmount == 0)
                                            {
                                                LogManager.Instance.Log("Application restart action invoked!", LogType.Information);

                                                this.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    Process.GetCurrentProcess().Kill();
                                                }));
                                                _continuousMainCycleThread.Abort();
                                                shutdownInvoked = true;
                                                break;
                                            }
                                            tryAmount--;
                                            Thread.Sleep(1000);

                                        } while (!_viewModel.IsValidMaster);
                                    }
                                }
                                else
                                {
                                    LogManager.Instance.Log("Application restart action invoked!", LogType.Information);
                                    ProcessStartInfo Info = new ProcessStartInfo();
                                    Info.Arguments = "RestartAction";
                                    //todo:h application startup location should be parameterized
                                    Info.FileName = ApplicationConfigurations.Instance.Configuration.ApplicationFullPath;
                                    Process.Start(Info);
                                    Application.Current.Shutdown();
                                    shutdownInvoked = true;
                                    _continuousMainCycleThread.Abort();
                                    break;
                                }
                                _viewModel.MasterWarningPanelVisibility = Visibility.Collapsed;
                            }
                        }

                        //todo:l refactor shutdown paramaterized cycle
                        if (!shutdownInvoked)
                        {
                            // Display current time on the bottom bar
                            _viewModel.GlobalCurrentTime = DateTime.Now.ToString("HH:mm:ss");
                            _viewModel.ChangeServiceStatesIndicators();
                            _viewModel.ContinuousUpdate();
                            UpdateProcessControl();
                        }
                    }
                }
            }
        }

        public void SetIntegrityCheckButtonSelected()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (CheckIfButtonSelected("IntegrityChecksBtn"))
                    return;

                SetSelectedButton("IntegrityChecksBtn");
            }));
        }

        private void UpdateProcessControl()
        {
            _isContinuousMainCycleCompleted = false;
            //    LogManager.Instance.Log($"-->{JsonConvert.SerializeObject( ProcessManager.Instance.CurrentProcess)}", LogType.Information);
            try
            {
                if (!ProcessManager.Instance.IsRunOperationCommandWorking)
                {
                    if (!ProcessManager.Instance.IsFinishAcknowledged())
                    {
                        ProcessManager.Instance.CurrentProcess.BatchCurrentState = BatchCurrentState.NotStarted;
                        ProcessManager.Instance.CurrentProcess.LoadNumber = string.Empty;
                        ProcessManager.Instance.CurrentProcess.ActiveRecipeName = string.Empty;
                        ProcessManager.Instance.CurrentProcess.ActiveRecipeId = 0;
                        ProcessManager.Instance.CurrentProcess.IsBatchLoaded = false;
                        ProcessManager.Instance.CurrentProcess.IsRecipeLoaded = false;
                        ProcessManager.Instance.CurrentProcess.BatchId = 0;
                        ProcessManager.Instance.CurrentProcess.CurrentSegmentDescription = string.Empty;
                        ProcessManager.Instance.CurrentProcess.CurrentSegmentNo = 0;
                        ProcessManager.Instance.CurrentProcess.LastUpdateDate = DateTime.Now;
                        ProcessManager.Instance.SynchronizeCurrentProcessInfo(false);
                        _viewModel.CurrentBatchNumber.Value = null;
                        _viewModel.CurrentRecipeName.Value = null;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WinUIMessageBox.Show("Process completed successfully! (Çalışan process başarıyla sonlandırıldı!)", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                        }));

                        ProcessManager.Instance.ChangeUserAcknowledgeForFinish(true);

                        // Close the opened windows such as Trend to refresh the data on it
                        CloseOpenedWindows();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Finish Process error! {ex.Message}", LogType.Fatal);
            }
            finally
            {
                _isContinuousMainCycleCompleted = true;
            }
        }

        /// <summary>
        /// If a menubar button is clicked then change its style to show it as a selected one.
        /// </summary>
        /// <param name="buttonName"></param>
        private void SetSelectedButton(string buttonName, bool doNotChangeOthers = false)
        {
            foreach (var item in _menuBarButtons)
            {
                if ((_viewModel.ServiceWarningPanelVisibility == Visibility.Visible || _viewModel.MasterWarningPanelVisibility == Visibility.Visible) 
                    && item.Name == "ReportsBtn")
                {
                    MakeEnableSpecificButtonDisableRest("ReportsBtn");
                    return;
                }

                if (item.Name == buttonName)
                {
                    item.IsEnabled = false;
                    item.IsChecked = true;
                    item.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#bbbbbb");
                }
                else
                {
                    if ((item.Tag != null && (string)item.Tag == "WindowState") || doNotChangeOthers)
                        continue;

                    string buttonPageTagName = MenuButtonBehavior.GetPageTagName(item);

                    if (_viewModel.PageTagNames.Contains(buttonPageTagName))
                    {
                        item.IsEnabled = true;
                        if (item == TrendBtn || item == CalibrationBtn || item == AlarmBtn || item == SensorViewBtn)
                            item.Tag = "UCState";
                        else
                            item.Tag = null;
                        item.IsChecked = false;
                        item.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#a0a0a0");
                    }
                }
            }
        }

        public bool CloseOpenedWindows()
        {
            // Check if Trend window has opened
            var trendWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.Title == "Trend Window");
            if (trendWindow != null)
                trendWindow.Close();

            // Check if Calibration window has opened
            var calibrationWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.Title == "Calibration Window");
            if (calibrationWindow != null)
                calibrationWindow.Close();

            // Check if Alarm window has opened
            var alarmWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.Title == "Alarm Window");
            if (alarmWindow != null)
                alarmWindow.Close();

            // Check if Sensor View window has opened
            var sensorViewWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.Title == "Sensor View Window");
            if (sensorViewWindow != null)
                sensorViewWindow.Close();

            // Check if Reports window has opened
            var reportsWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.Title == "Reports Window");
            if (reportsWindow != null)
                reportsWindow.Close();

            if(string.IsNullOrEmpty(_viewModel.ContentTitle))
                _lastSelectedBtnName = string.Empty;

            return true;

            // todo:m Add Manual Operation window when it is implemented
        }

        public void MakeEnableSpecificButtonDisableRest(string buttonName)
        {
            if (_viewModel.PageTagNames.Count == 0)
                return;

            foreach (var item in _menuBarButtons)
            {
                string buttonPageTagName = MenuButtonBehavior.GetPageTagName(item);
                if (item.Name == buttonName && _viewModel.PageTagNames.Contains(buttonPageTagName))
                {
                    item.IsEnabled = true;
                    item.IsChecked = false;
                    item.Tag = null;
                }
                else
                {
                    item.IsEnabled = false;
                    item.IsChecked = false;
                    item.Tag = "darkMode";
                    item.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#a0a0a0");
                }
            }
            CloseOpenedWindows();
        }

        /// <summary>
        /// Used for disabling all pages except a specific page and loading that page afterwards.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="noException"></param>
        public void DisableAllButtonsExceptOne(string buttonName, bool noException = false)
        {
            foreach (var item in _menuBarButtons)
            {
                if (item.IsChecked ?? false)
                    _lastSelectedBtnName = item.Name;

                if (item.Name == buttonName && !noException)
                {
                    item.IsEnabled = false;
                    if (item == TrendBtn || item == CalibrationBtn || item == AlarmBtn || item == SensorViewBtn)
                        item.Tag = "UCState";
                    else
                        item.Tag = null;
                    item.IsChecked = true;
                    item.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#bbbbbb");
                }
                else
                {
                    item.IsEnabled = false;
                    item.IsChecked = false;
                    item.Tag = "darkMode";
                    item.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#a0a0a0");
                }
            }
        }

        public void SelectLastSelectedButton()
        {
            if(!string.IsNullOrEmpty(_lastSelectedBtnName))
                SetSelectedButton(_lastSelectedBtnName);
        }

        public void DeselectReportsButton()
        {
            if (_viewModel.ServiceWarningPanelVisibility == Visibility.Visible || _viewModel.MasterWarningPanelVisibility == Visibility.Visible)
            {
                MakeEnableSpecificButtonDisableRest("ReportsBtn");
                return;
            }

            ReportsBtn.IsEnabled = true;
            ReportsBtn.IsChecked = false;
            ReportsBtn.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#a0a0a0");
        }

        private void DeselectButton(ToggleButton toggleButton)
        {
            toggleButton.IsEnabled = true;
            toggleButton.IsChecked = false;
            toggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#a0a0a0");
        }

        public void SetButtonStatesByAuthorization()
        {
            if (_viewModel.PageTagNames.Count == 0)
                return;

            foreach (var item in _menuBarButtons)
            {
                string buttonPageTagName = MenuButtonBehavior.GetPageTagName(item);

                if (_viewModel.PageTagNames.Contains(buttonPageTagName))
                {
                    item.IsEnabled = true;
                    if (item == TrendBtn || item == CalibrationBtn || item == AlarmBtn || item == SensorViewBtn)
                        item.Tag = "UCState";
                    else
                        item.Tag = null;
                }
                else
                {
                    item.IsEnabled = false;
                    item.Tag = "disabled";
                }

                item.IsChecked = false;
                item.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#a0a0a0");
            }
        }

        public void ChangeTagValueOfSelectedButton(string buttonName, PageLoadState pageLoadState)
        {
            string tagVal = string.Empty;

            switch (pageLoadState)
            {
                case PageLoadState.UserControl:
                    tagVal = "UCState";
                    break;
                case PageLoadState.Window:
                    tagVal = "WindowState";
                    break;
                default:
                    break;
            }

            foreach (var item in _menuBarButtons)
            {
                if (item.Name == buttonName)
                {
                    if (item.Tag.Equals("darkMode"))
                        return;

                    item.Tag = tagVal;

                    if (pageLoadState == PageLoadState.UserControl)
                        DeselectButton(item);

                    return;
                }
            }
        }

        private bool CheckIfButtonSelected(string btnName)
        {
            foreach (var item in _menuBarButtons)
            {
                bool isItChecked = item.IsChecked ?? false;

                if (isItChecked && item.Name == btnName)
                    return true;
            }
            return false;
        }

        #region Menu Bar buttons

        private void VacuumBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("VacuumBtn");
        }
        private void EnterPartsBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("EnterPartsBtn");
        }
        private void RecipeEditBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("RecipeEditBtn");
        }
        private void IntegrityChecksBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("IntegrityChecksBtn");
        }
        private void SensorViewBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("SensorViewBtn");
        }

        private void SensorViewNewWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ContentTitle == "Sensor View" || _viewModel.IsSensorViewOpenedInWindow)
                return;

            // If Alarm, Calibration or Trend Windows are already opened, then close them.
            if (_viewModel.IsAlarmOpenedInWindow || _viewModel.IsCalibrationOpenedInWindow || _viewModel.IsTrendOpenedInWindow)
            {
                var alarmWindow = Application.Current.Windows.OfType<Alarm_Window>().SingleOrDefault();
                var calibrationWindow = Application.Current.Windows.OfType<Calibration_Window>().SingleOrDefault();
                var trendWindow = Application.Current.Windows.OfType<Trend_Window>().SingleOrDefault();

                if (alarmWindow != null)
                {
                    alarmWindow.Close();
                    _viewModel.IsAlarmOpenedInWindow = false;
                }

                if (calibrationWindow != null)
                {
                    calibrationWindow.Close();
                    _viewModel.IsCalibrationOpenedInWindow = false;
                }

                if (trendWindow != null)
                {
                    trendWindow.Close();
                    _viewModel.IsTrendOpenedInWindow = false;
                }
            }

            SetSelectedButton("SensorViewBtn", true);
            DeselectButton(SensorViewNewWindowBtn);
            ChangeTagValueOfSelectedButton("SensorViewBtn", PageLoadState.Window);
        }

        private void RunOperBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("RunOperBtn");
        }

        private void ManOperationBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("ManOperationBtn");
        }

        // todo:l Delete all codes which are related with ManOpNewWindow button if it'll not be used again.
        //private void ManOpNewWindowBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_viewModel.ContentTitle == "Manual Operation" || _viewModel.IsManOpOpenedInWindow)
        //        return;

        //    // If Trend Window is already opened, then close it.
        //    if (_viewModel.IsTrendOpenedInWindow)
        //    {
        //        var trendWindow = Application.Current.Windows.OfType<Trend_Window>().SingleOrDefault();

        //        if (trendWindow != null)
        //        {
        //            trendWindow.Close();
        //            _viewModel.IsTrendOpenedInWindow = false;
        //        }
        //    }

        //    SetSelectedButton("ManOperationBtn", true);
        //    DeselectButton(ManOpNewWindowBtn);
        //    ChangeTagValueOfSelectedButton("ManOperationBtn", PageLoadState.Window);
        //}

        private void TrendBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("TrendBtn");
        }

        private void TrendNewWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ContentTitle == "Trend" || _viewModel.IsTrendOpenedInWindow)
                return;

            // If Alarm, Calibration or Sensor View Windows are already opened, then close them.
            if (_viewModel.IsAlarmOpenedInWindow || _viewModel.IsCalibrationOpenedInWindow || _viewModel.IsSensorViewOpenedInWindow)
            {
                var alarmWindow = Application.Current.Windows.OfType<Alarm_Window>().SingleOrDefault();
                var calibrationWindow = Application.Current.Windows.OfType<Calibration_Window>().SingleOrDefault();
                var sensorViewWindow = Application.Current.Windows.OfType<Sensor_View_Window>().SingleOrDefault();

                if (alarmWindow != null)
                {
                    alarmWindow.Close();
                    _viewModel.IsAlarmOpenedInWindow = false;
                }

                if (calibrationWindow != null)
                {
                    calibrationWindow.Close();
                    _viewModel.IsCalibrationOpenedInWindow = false;
                }

                if (sensorViewWindow != null)
                {
                    sensorViewWindow.Close();
                    _viewModel.IsSensorViewOpenedInWindow = false;
                }
            }

            SetSelectedButton("TrendBtn", true);
            DeselectButton(TrendNewWindowBtn);
            ChangeTagValueOfSelectedButton("TrendBtn", PageLoadState.Window);
        }

        private void AlarmBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("AlarmBtn");
        }

        public void AlarmNewWindowBtnSetOpen()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (_viewModel.ContentTitle == "Alarm" || _viewModel.IsAlarmOpenedInWindow)
                    return;

                _viewModel.IsAlarmOpenedInWindow = true;
                Alarm_Window alarmWindow = new Alarm_Window(_viewModel);
                alarmWindow.MainView = new AlarmVM(_viewModel.WaitIndicatorControl, _viewModel.IncomingAlarmsChecker, _viewModel.AlarmPageForceLoadOption);
                alarmWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                alarmWindow.Show();

                // If Trend, Calibration and Sensor View Windows are already opened, then close them.
                if (_viewModel.IsCalibrationOpenedInWindow || _viewModel.IsTrendOpenedInWindow || _viewModel.IsSensorViewOpenedInWindow)
                {
                    var calibrationWindow = Application.Current.Windows.OfType<Calibration_Window>().SingleOrDefault();
                    var trendWindow = Application.Current.Windows.OfType<Trend_Window>().SingleOrDefault();
                    var sensorViewWindow = Application.Current.Windows.OfType<Sensor_View_Window>().SingleOrDefault();

                    if (calibrationWindow != null)
                    {
                        calibrationWindow.Close();
                        _viewModel.IsCalibrationOpenedInWindow = false;
                    }

                    if (trendWindow != null)
                    {
                        trendWindow.Close();
                        _viewModel.IsTrendOpenedInWindow = false;
                    }

                    if (sensorViewWindow != null)
                    {
                        sensorViewWindow.Close();
                        _viewModel.IsSensorViewOpenedInWindow = false;
                    }
                }

                SetSelectedButton("AlarmBtn", true);
                ChangeTagValueOfSelectedButton("AlarmBtn", PageLoadState.Window);
            }));
        }

        private void RecipeBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("RecipeBtn");
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("ReportsBtn");
        }
     
        private void QualityBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("QualityBtn");
        }

        private void CalibrationBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("CalibrationBtn");
        }

        private void CalibrationNewWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ContentTitle == "Calibration" || _viewModel.IsCalibrationOpenedInWindow)
                return;

            // If Trend, Alarm and Sensor View Windows are already opened, then close them.
            if (_viewModel.IsAlarmOpenedInWindow || _viewModel.IsTrendOpenedInWindow || _viewModel.IsSensorViewOpenedInWindow)
            {
                var alarmWindow = Application.Current.Windows.OfType<Alarm_Window>().SingleOrDefault();
                var trendWindow = Application.Current.Windows.OfType<Trend_Window>().SingleOrDefault();
                var sensorViewWindow = Application.Current.Windows.OfType<Sensor_View_Window>().SingleOrDefault();

                if (alarmWindow != null)
                {
                    alarmWindow.Close();
                    _viewModel.IsAlarmOpenedInWindow = false;
                }

                if (trendWindow != null)
                {
                    trendWindow.Close();
                    _viewModel.IsTrendOpenedInWindow = false;
                }

                if (sensorViewWindow != null)
                {
                    sensorViewWindow.Close();
                    _viewModel.IsSensorViewOpenedInWindow = false;
                }
            }

            SetSelectedButton("CalibrationBtn", true);
            DeselectButton(CalibrationNewWindowBtn);
            ChangeTagValueOfSelectedButton("CalibrationBtn", PageLoadState.Window);
        }

        private void OscillationBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("OscillationBtn");
        }

        private void UserManagementBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton("UserManagementBtn");
        }
        #endregion

        #region Hamburger Menu
        private void btnHamMenuHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideLeftMenu", btnHamMenuHide, btnHamMenuShow, hamMenuSp);
            _viewModel.IsHamburgerMenuExpanded.Value = false;
        }

        private void btnHamMenuShow_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowLeftMenu", btnHamMenuHide, btnHamMenuShow, hamMenuSp);
            _viewModel.IsHamburgerMenuExpanded.Value = true;
        }

        /// <summary>
        /// Control of the Hamburger Menu bar section.
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="btnHide"></param>
        /// <param name="btnShow"></param>
        /// <param name="pnl"></param>
        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            Storyboard gridCol1 = Resources["mainContentGridCol1Anim"] as Storyboard;
            Storyboard gridCol0 = Resources["mainContentGridCol0Anim"] as Storyboard;

            Storyboard showBtnVis = Resources["btnHamMenuShow_visible"] as Storyboard;
            Storyboard showBtnHid = Resources["btnHamMenuShow_hide"] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                gridCol1.Begin(contentSec);
                btnHide.Visibility = Visibility.Visible;
                showBtnHid.Begin(btnHamMenuShow);
            }
            else if (Storyboard.Contains("Hide"))
            {
                gridCol0.Begin(contentSec);
                btnHide.Visibility = Visibility.Hidden;
                showBtnVis.Begin(btnHamMenuShow);
            }
        }
        #endregion

        private void ApplicationLogo_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Piping_and_Instrumentation_Diagram_Window pipingWindow = new Piping_and_Instrumentation_Diagram_Window();
            pipingWindow.ShowDialog();
        }

        
    }
}
