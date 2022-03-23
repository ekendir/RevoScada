using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

using RevoScada.ProcessController;
using RevoScada.Synchronization;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Properties;
using RevoScada.DesktopApplication.ViewModels.CalibrationViewModels;
using RevoScada.DesktopApplication.ViewModels.ManualOperationViewModels;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.ItemViews;
using RevoScada.DesktopApplication.Views.ItemViews.HamburgerMenuViews;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;
using System.Diagnostics;
using System.Threading;
using RevoScada.DesktopApplication.ViewModels.TrendViewModels;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class AppViewModel : ObservableObject
    {
        #region Services
        private readonly string _connectionString;
        private UserGroupService _userGroupService;
        private PermissionService _permissionService;

        private SyncStateManager _syncStateManager;

        #endregion

        #region Properties
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => OnPropertyChanged(ref _currentView, value);
        }

        private string _contentTitle;
        public string ContentTitle
        {
            get => _contentTitle;
            set => OnPropertyChanged(ref _contentTitle, value);
        }

        private object _hamburgerMenuItemView;
        public object HamburgerMenuItemView
        {
            get => _hamburgerMenuItemView;
            set => OnPropertyChanged(ref _hamburgerMenuItemView, value);
        }

        public HamburgerMenuLeftModel HamburgerMenuLeftModel
        {
            get => _hamburgerMenuLeftModel;
            set => OnPropertyChanged(ref _hamburgerMenuLeftModel, value);
        }

        public ValueWrapper<bool> IsHamburgerMenuExpanded { get; set; }

        private string _globalCurrentTime;
        public string GlobalCurrentTime
        {
            get => _globalCurrentTime;
            set => OnPropertyChanged(ref _globalCurrentTime, value);
        }

        private string _furnaceSelectionMenuButtonVisibility;
        public string FurnaceSelectionMenuButtonVisibility
        {
            get => _furnaceSelectionMenuButtonVisibility;
            set => OnPropertyChanged(ref _furnaceSelectionMenuButtonVisibility, value);
        }

        private WaitIndicatorControl _waitIndicatorControl;
        public WaitIndicatorControl WaitIndicatorControl
        {
            get => _waitIndicatorControl;
            set => OnPropertyChanged(ref _waitIndicatorControl, value);
        }

        public ValueWrapper<bool> IsRecipeTableDataChanged { get; set; }

        private bool _isSensorViewOpenedInWindow;
        public bool IsSensorViewOpenedInWindow
        {
            get => _isSensorViewOpenedInWindow;
            set
            {
                _isSensorViewOpenedInWindow = value;

                if (_isSensorViewOpenedInWindow == false && MainWindow != null)
                    MainWindow.ChangeTagValueOfSelectedButton("SensorViewBtn", PageLoadState.UserControl);
            }
        }

        private bool _isTrendOpenedInWindow;
        public bool IsTrendOpenedInWindow
        {
            get => _isTrendOpenedInWindow;
            set
            {
                _isTrendOpenedInWindow = value;

                if (_isTrendOpenedInWindow == false && MainWindow != null)
                    MainWindow.ChangeTagValueOfSelectedButton("TrendBtn", PageLoadState.UserControl);
            }
        }

        private bool _isManOpOpenedInWindow;
        public bool IsManOpOpenedInWindow
        {
            get => _isManOpOpenedInWindow;
            set
            {
                _isManOpOpenedInWindow = value;

                if (_isManOpOpenedInWindow == false && MainWindow != null)
                    MainWindow.ChangeTagValueOfSelectedButton("ManOperationBtn", PageLoadState.UserControl);
            }
        }

        private bool _isCalibrationOpenedInWindow;
        public bool IsCalibrationOpenedInWindow
        {
            get => _isCalibrationOpenedInWindow;
            set
            {
                _isCalibrationOpenedInWindow = value;

                if (_isCalibrationOpenedInWindow == false && MainWindow != null)
                    MainWindow.ChangeTagValueOfSelectedButton("CalibrationBtn", PageLoadState.UserControl);
            }
        }

        private bool _isAlarmOpenedInWindow;
        public bool IsAlarmOpenedInWindow
        {
            get => _isAlarmOpenedInWindow;
            set
            {
                _isAlarmOpenedInWindow = value;

                if (_isAlarmOpenedInWindow == false && MainWindow != null)
                    MainWindow.ChangeTagValueOfSelectedButton("AlarmBtn", PageLoadState.UserControl);
            }
        }

        private ValueWrapper<bool> _incomingAlarmsChecker;
        public ValueWrapper<bool> IncomingAlarmsChecker
        {
            get => _incomingAlarmsChecker;
            set => OnPropertyChanged(ref _incomingAlarmsChecker, value);
        }

        private ValueWrapper<bool> _alarmPageForceLoadOption;
        public ValueWrapper<bool> AlarmPageForceLoadOption
        {
            get => _alarmPageForceLoadOption;
            set => OnPropertyChanged(ref _alarmPageForceLoadOption, value);
        }

        public string WindowStyleProperty
        {
            get
            {
                string windowStyleProperty = "None";
                ApplicationProperty applicationProperty = ProcessManager.Instance.ApplicationProperties["WindowStyleProperty"];
                if (!string.IsNullOrEmpty(applicationProperty?.Value))
                {
                    try
                    {
                        windowStyleProperty = applicationProperty.Value;
                    }
                    catch
                    {
                        windowStyleProperty = "SingleBorderWindow";
                    }
                }
                return windowStyleProperty;
            }
        }
 

        private Visibility _serviceWarningPanelVisibility;
        public Visibility ServiceWarningPanelVisibility
        {
            get => _serviceWarningPanelVisibility;
            set
            {
                OnPropertyChanged(ref _serviceWarningPanelVisibility, value);

                if (_serviceWarningPanelVisibility == Visibility.Visible && AccountWarningPanelVisibility == Visibility.Collapsed)
                    MainWindow.MakeEnableSpecificButtonDisableRest("ReportsBtn");
                else if (_serviceWarningPanelVisibility == Visibility.Collapsed && _masterWarningPanelVisibility == Visibility.Collapsed && ActiveUser != null)
                    MainWindow.SetButtonStatesByAuthorization();
            }
        }

        private Visibility _accountWarningPanelVisibility;
        public Visibility AccountWarningPanelVisibility
        {
            get => _accountWarningPanelVisibility;
            set => OnPropertyChanged(ref _accountWarningPanelVisibility, value);
        }

        private Visibility _masterWarningPanelVisibility = Visibility.Collapsed;
        public Visibility MasterWarningPanelVisibility
        {
            get => _masterWarningPanelVisibility;

            set
            {
                OnPropertyChanged(ref _masterWarningPanelVisibility, value);

                if (_masterWarningPanelVisibility == Visibility.Visible && AccountWarningPanelVisibility == Visibility.Collapsed)
                    MainWindow.MakeEnableSpecificButtonDisableRest("ReportsBtn");
                else if (_masterWarningPanelVisibility == Visibility.Collapsed && _serviceWarningPanelVisibility == Visibility.Collapsed && ActiveUser != null)
                    MainWindow.SetButtonStatesByAuthorization();
            }
        }

        private Visibility _inActivityPanelVisibility = Visibility.Collapsed;
        public Visibility InActivityPanelVisibility
        {
            get => _inActivityPanelVisibility;

            set
            {
                OnPropertyChanged(ref _inActivityPanelVisibility, value);

                if (InActivityPanelVisibility != Visibility.Visible && ServiceWarningPanelVisibility != Visibility.Visible && MasterWarningPanelVisibility != Visibility.Visible)
                {
                    MainWindow.SetButtonStatesByAuthorization();
                    MainWindow.SelectLastSelectedButton();
                }
                else
                    MainWindow.DisableAllButtonsExceptOne(null, true);
            }
        }

        private string _inActivityText;
        public string InActivityText
        {
            get => _inActivityText;
            set => OnPropertyChanged(ref _inActivityText, value);
        }

        private UserGridModel _activeUser;
        public UserGridModel ActiveUser
        {
            get => _activeUser;
            set
            {
                OnPropertyChanged(ref _activeUser, value);

                if (value == null)
                {
                    AccountWarningPanelVisibility = Visibility.Visible;
                    // No user has signed in, that's why disable emergency and appsettings buttons
                    IsEmergencyActionsPageEnabled = false;
                    IsAppSettingsPageEnabled = false;
                }
                else
                {
                    if (value.GroupId > 0)
                    {
                        if (ByPassUser != null)
                            _activeUserGroup = _byPassUserGroup;
                        else
                            _activeUserGroup = _userGroupService.GetById(value.GroupId);

                        if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20 && ByPassUser == null)
                        {
                            IncludeRequiredPermissions();
                        }

                        value.GroupName = _activeUserGroup.GroupName;
                        PermissionsByPage = _permissionService.GetAll().Where(p => _activeUserGroup.PermissionIds.Contains(p.id)).ToObservableCollection();
                        PageTagNames = PermissionsByPage.Where(p => p.PermissionGroup == 1).Select(p => p.PermissionTag).ToList();
                        PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "EmergencyActions" || p.PageName == "ApplicationSettings")
                                                             .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                        IsEmergencyActionsPageEnabled = PermissionValues["EmergencyActionsPage"];
                        IsAppSettingsPageEnabled = PermissionValues["ApplicationSettingsPage"];

                        // Set active auto logout
                        if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
                        {
                            MainWindow.StartAutoLogout();
                        }

                        if (ServiceWarningPanelVisibility != Visibility.Visible && MasterWarningPanelVisibility != Visibility.Visible)
                            MainWindow.SetButtonStatesByAuthorization();
                        else if ((ServiceWarningPanelVisibility == Visibility.Visible || MasterWarningPanelVisibility == Visibility.Visible) && AccountWarningPanelVisibility == Visibility.Collapsed)
                            MainWindow.MakeEnableSpecificButtonDisableRest("ReportsBtn");
                    }

                    AccountWarningPanelVisibility = Visibility.Collapsed;
                }
            }
        }

        private string _disabledControlTooltipText;
        public string DisabledControlTooltipText
        {
            get
            {
                _disabledControlTooltipText = "You have no permission to use this control!\n(Bu kontrolü kullanmak için izniniz bulunmamaktadır!)";
                return _disabledControlTooltipText;
            }
            set
            {
                _disabledControlTooltipText = value;
            }
        }

        private bool _isEmergencyActionsPageEnabled;
        public bool IsEmergencyActionsPageEnabled
        {
            get => _isEmergencyActionsPageEnabled;
            set => OnPropertyChanged(ref _isEmergencyActionsPageEnabled, value);
        }
        private bool _isAppSettingsPageEnabled;
        public bool IsAppSettingsPageEnabled
        {
            get => _isAppSettingsPageEnabled;
            set => OnPropertyChanged(ref _isAppSettingsPageEnabled, value);
        }

        private ValueWrapper<string> _currentBatchNumber;
        public ValueWrapper<string> CurrentBatchNumber
        {
            get => _currentBatchNumber;
            set => OnPropertyChanged(ref _currentBatchNumber, value);
        }

        private ValueWrapper<string> _currentRecipeName;
        public ValueWrapper<string> CurrentRecipeName
        {
            get => _currentRecipeName;
            set => OnPropertyChanged(ref _currentRecipeName, value);
        }
        #endregion

        #region Collections
        public List<string> PageTagNames { get; set; }
        private ObservableCollection<Permission> _permissionsByPage;
        public ObservableCollection<Permission> PermissionsByPage
        {
            get => _permissionsByPage;
            set => OnPropertyChanged(ref _permissionsByPage, value);
        }
        private Dictionary<string, bool> _permissionValues;
        public Dictionary<string, bool> PermissionValues
        {
            get => _permissionValues;
            set => OnPropertyChanged(ref _permissionValues, value);
        }

        public AlarmTagConfigurations AlarmPageTagConfigurations { get; private set; }

        #endregion

        #region Public Commands
        public ICommand Prevent_AltF4_Command { get; set; }
        public ICommand Load_VacuumLines_Command { get; set; }
        public ICommand Load_SensorView_Command { get; set; }
        public ICommand Load_SensorViewWindow_Command { get; set; }
        public ICommand Load_TrendView_Command { get; set; }
        public ICommand Load_TrendWindow_Command { get; set; }
        public ICommand Load_EnterParts_Command { get; set; }
        public ICommand Load_RecipeEditor_Command { get; set; }
        public ICommand Load_IntegrityChecks_Command { get; set; }
        public ICommand Load_RunOperation_Command { get; set; }
        public ICommand Load_ManOperation_Command { get; set; }
        public ICommand Load_ManOperationWindow_Command { get; set; }
        public ICommand Load_Alarm_Command { get; set; }
        public ICommand Load_Recipe_Command { get; set; }
        public ICommand Load_Reports_Command { get; set; }
        public ICommand Load_Calibration_Command { get; set; }
        public ICommand Load_CalibrationWindow_Command { get; set; }
        public ICommand Load_Quality_Command { get; set; }
        public ICommand Load_Oscillation_Command { get; set; }
        public ICommand Load_UserControl_Command { get; set; }
        public ICommand SilenceHornCommand { get; set; }

        #endregion

        #region Fields
        private PlcCommandManager _plcCommandManager;
        private RunOperationTagConfigurations _runOperationTagConfigurations;
        private GlobalTagConfigurations _globalTagConfigurations;
        private Dictionary<HamburgerMenuLeftTagNames, SiemensTagConfiguration> _hamburgerMenuLeftTagConfigurations;
        private HamburgerMenuLeftModel _hamburgerMenuLeftModel;
        private bool _integrityCheckValOk;
        public bool IntegrityCheckRunVal;
        private bool _integrityCheckSkipVal;
        private bool _isIntegrityCheckPageLoaded;
        public MainWindow MainWindow;
        private UserGroup _activeUserGroup;
        private UserGroup _byPassUserGroup;
        public UserGridModel ByPassUser;

        #endregion

        #region Run Operation Checks
        private bool _isEnterPartsChecked;
        public bool IsEnterPartsChecked
        {
            get => _isEnterPartsChecked;
            set => OnPropertyChanged(ref _isEnterPartsChecked, value);
        }
        private bool _isRecipeEditorChecked;
        public bool IsRecipeEditorChecked
        {
            get => _isRecipeEditorChecked;
            set => OnPropertyChanged(ref _isRecipeEditorChecked, value);
        }
        private bool _isIntegrityChecked;
        public bool IsIntegrityChecked
        {
            get => _isIntegrityChecked;
            set => OnPropertyChanged(ref _isIntegrityChecked, value);
        }
        #endregion

        #region Service-plc-UsagePriorityCheck State Checks
        private bool _readServiceChecker;
        public bool ReadServiceChecker
        {
            get => _readServiceChecker;
            set => OnPropertyChanged(ref _readServiceChecker, value);
        }

        private bool _writeServiceChecker;
        public bool WriteServiceChecker
        {
            get => _writeServiceChecker;
            set => OnPropertyChanged(ref _writeServiceChecker, value);
        }

        private bool _alarmServiceChecker;
        public bool AlarmServiceChecker
        {
            get => _alarmServiceChecker;
            set => OnPropertyChanged(ref _alarmServiceChecker, value);
        }

        private bool _dataLogServiceChecker;
        public bool DataLogServiceChecker
        {
            get => _dataLogServiceChecker;
            set => OnPropertyChanged(ref _dataLogServiceChecker, value);
        }

        private bool _cacheServiceChecker;
        public bool CacheServiceChecker
        {
            get => _cacheServiceChecker;
            set => OnPropertyChanged(ref _cacheServiceChecker, value);
        }

        private bool _processManagerServiceChecker;
        public bool ProcessManagerServiceChecker
        {
            get => _processManagerServiceChecker;
            set => OnPropertyChanged(ref _processManagerServiceChecker, value);
        }

        private bool _synchronizationServiceChecker;
        public bool SynchronizationServiceChecker
        {
            get => _synchronizationServiceChecker;
            set => OnPropertyChanged(ref _synchronizationServiceChecker, value);
        }

        private bool _plcStatusChecker;
        public bool PlcStatusChecker
        {
            get => _plcStatusChecker;
            set => OnPropertyChanged(ref _plcStatusChecker, value);
        }

        private bool _isAllServicesRunning;
        public bool IsAllServicesRunning
        {
            get => _isAllServicesRunning;
            set => OnPropertyChanged(ref _isAllServicesRunning, value);
        }

        private bool _isValidMaster;
        public bool IsValidMaster
        {
            get
            {
                _isValidMaster = _syncStateManager.IsValidMaster(ProcessManager.Instance.PlcDeviceId,
                                                                 ApplicationConfigurations.Instance.Configuration.WorkingEnvironment,
                                                                 checkDurationInSeconds: 80,
                                                                 false);
                return _isValidMaster;
            }
            set => OnPropertyChanged(ref _isValidMaster, value);
        }
        #endregion


        #region HamburgerMenu

        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }

        public Dictionary<string, string> MonitoringUnitTitle { get; set; }

        public string MonitoringTitleName { get; set; }

        #endregion HamburgerMenu
        public AppViewModel()
        {
            // Set deactive specific datablocks
            foreach (var item in ProcessManager.Instance.GetOnDemandKeyNames())
            {
                ProcessManager.Instance.ChangeDemandReadStateOnCache(item, false);
            }

            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _userGroupService = new UserGroupService(_connectionString);
            _permissionService = new PermissionService(_connectionString);
            //  FurnaceSwicther furnaceSwicther = new FurnaceSwicther();
            //  bool isDefineFailed = furnaceSwicther.DefineFurnaceSelection();

            IsHamburgerMenuExpanded = new ValueWrapper<bool>(false);
            AlarmPageForceLoadOption = new ValueWrapper<bool>(true);
            if(!string.IsNullOrEmpty(ProcessManager.Instance.ApplicationProperties["AlarmPageForceLoadOption"].Value))
                AlarmPageForceLoadOption.Value = Convert.ToBoolean(ProcessManager.Instance.ApplicationProperties["AlarmPageForceLoadOption"].Value);

            InitializePageTagConfigurations();
            WaitIndicatorControl = new WaitIndicatorControl();
            IncomingAlarmsChecker = new ValueWrapper<bool>(false);
            IsRecipeTableDataChanged = new ValueWrapper<bool>(false);
            CurrentBatchNumber = new ValueWrapper<string>(null);
            CurrentRecipeName = new ValueWrapper<string>(null);

            ServiceWarningPanelVisibility = Visibility.Collapsed;
            AccountWarningPanelVisibility = Visibility.Collapsed;
            PageTagNames = new List<string>();

            // A user and group created for our debug environment
#if DEBUG
            //_byPassUserGroup = new UserGroup()
            //{
            //    id = 1,
            //    GroupName = "Supervisor",
            //    IsActive = true,
            //};

            //// Assign all permissions to user
            //_byPassUserGroup.PermissionIds = _permissionService.GetAll().Select(p => p.id).ToArray();

            //ByPassUser = new UserGridModel()
            //{
            //    id = 1,
            //    CreateDate = DateTime.Now,
            //    FirstName = "byPassFirstName",
            //    LastName = "byPassLastName",
            //    GroupId = 1,
            //    GroupName = "Supervisor",
            //    IsActive = true,
            //    UserName = "Super User",
            //    LogoutTime = 30
            //};
#endif

            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                case 20:
                    HamburgerMenuItemView = new Hamburger_MenuType1();
                    break;
                case 2:
                case 4:
                case 5:
                    HamburgerMenuItemView = new Hamburger_MenuType2();
                    break;
                case 3:
                    HamburgerMenuItemView = new Hamburger_MenuType3();
                    break;
            }
            UserControlBaseVM userControlBaseVM = new UserControlBaseVM();

            if (userControlBaseVM.ApplicationLanguageSettings != null)
                MonitoringUnitTitle = userControlBaseVM.ApplicationLanguageSettings.Eng.GlobalPortName;
            MonitoringTitleName = MonitoringUnitTitle["monitoringTextName"];

           
            ProcessManager.Instance.SynchronizeCurrentProcessInfo(true);

            CurrentBatchNumber.Value = string.IsNullOrEmpty(ProcessManager.Instance.CurrentProcess.LoadNumber) ? null : ProcessManager.Instance.CurrentProcess.LoadNumber;
            CurrentRecipeName.Value = string.IsNullOrEmpty(ProcessManager.Instance.CurrentProcess.ActiveRecipeName) ? null : ProcessManager.Instance.CurrentProcess.ActiveRecipeName;

            _hamburgerMenuLeftModel = new HamburgerMenuLeftModel();
            _hamburgerMenuLeftModel.PtcAllSensorsWorking = true;
            Prevent_AltF4_Command = new RelayCommand(DoNothing);

            Load_VacuumLines_Command = new RelayCommand(LoadVacuumLinesView);
            Load_SensorView_Command = new RelayCommand(LoadSensorView);
            Load_SensorViewWindow_Command = new RelayCommand(LoadSensorViewWindow);
            Load_TrendView_Command = new RelayCommand(LoadTrendView);
            Load_TrendWindow_Command = new RelayCommand(LoadTrendWindow);
            Load_EnterParts_Command = new RelayCommand(LoadEnterPartsView);
            Load_RecipeEditor_Command = new RelayCommand(LoadRecipeEditorView);
            Load_IntegrityChecks_Command = new RelayCommand(LoadIntegrityChecksView);
            Load_RunOperation_Command = new RelayCommand(LoadRunOperationView);
            Load_ManOperation_Command = new RelayCommand(LoadManOperationView);
            Load_ManOperationWindow_Command = new RelayCommand(LoadManOperationWindow);
            Load_Alarm_Command = new RelayCommand(LoadAlarmView);
            Load_Recipe_Command = new RelayCommand(LoadRecipeView);
            Load_Reports_Command = new RelayCommand(LoadReportsView);
            Load_Calibration_Command = new RelayCommand(LoadCalibrationView);
            Load_CalibrationWindow_Command = new RelayCommand(LoadCalibrationWindow);
            Load_Quality_Command = new RelayCommand(LoadQualityView);
            Load_Oscillation_Command = new RelayCommand(LoadOscillationView);
            Load_UserControl_Command = new RelayCommand(LoadUserControlView);
            SilenceHornCommand = new RelayCommand(SilenceHorn);

            IsValidMaster = true;

            _syncStateManager = new SyncStateManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            InActivityText = "Inactivity detected in your session! It will expire soon\nand current user will be signed out unless you make\nyour move!";
        }

        private void InitializePageTagConfigurations()
        {
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("RunOperation");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            _runOperationTagConfigurations = JsonConvert.DeserializeObject<RunOperationTagConfigurations>(jsonSerializedString);

           
            pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            pageTagConfiguration = pageTagConfigurationService.GetByName("Global");
            jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            _globalTagConfigurations = JsonConvert.DeserializeObject<GlobalTagConfigurations>(jsonSerializedString);

            _globalTagConfigurations.LeakageTestInfoCheckStatusRun = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_globalTagConfigurations.LeakageTestInfoCheckStatusRun)];

            _runOperationTagConfigurations.EnterPartsOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.EnterPartsOk)];
            _runOperationTagConfigurations.RecipeOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.RecipeOk)];
            _runOperationTagConfigurations.IntegrityCheckOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.IntegrityCheckOk)];
            _runOperationTagConfigurations.SkipIntegrityCheck = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.SkipIntegrityCheck)];

            pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            pageTagConfiguration = pageTagConfigurationService.GetByName("HamburgerMenuLeft");
            jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            var hamburgerMenuTagConfigurations = JsonConvert.DeserializeObject<HamburgerMenuTagConfigurations>(jsonSerializedString);

            _hamburgerMenuLeftTagConfigurations = new Dictionary<HamburgerMenuLeftTagNames, SiemensTagConfiguration>();

            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.ActualTemperatureCalcSetPoint, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.ActualTemperatureCalcSetPoint]);
            
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.AirTemperatureCalcSetPoint, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.AirTemperatureCalcSetPoint]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.BatchElapsedTime, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.BatchElapsedTime]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.BatchName, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.BatchName]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.BatchStartTime, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.BatchStartTime]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.BatchFinishTime, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.BatchFinishTime]);
            
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.HighMONPortNumber, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.HighMONPortNumber]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.HighMONValue, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.HighMONValue]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.HighPTCPortNumber, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.HighPTCPortNumber]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.HighPTCValue, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.HighPTCValue]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.IsSystemAuto, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.IsSystemAuto]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.LowMONPortNumber, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.LowMONPortNumber]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.LowMONValue, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.LowMONValue]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.LowPTCPortNumber, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.LowPTCPortNumber]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.LowPTCValue, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.LowPTCValue]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.PtcSetPoint, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.PtcSetPoint]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.RecipeName, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.RecipeName]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.RunStatus, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.RunStatus]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.SegDescription, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.SegDescription]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.SegElapsedTime, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.SegElapsedTime]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.SegmentNo, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.SegmentNo]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.SegWaitingTime, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.SegWaitingTime]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.VacActual, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.VacActual]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.VacSet, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.VacSet]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.VacHeaderRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.VacHeaderRate]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.AirTCRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.AirTCRate]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.HighPTCRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.HighPTCRate]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.LowPTCRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.LowPTCRate]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.HighMonRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.HighMonRate]);
            _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.LowMonRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.LowMonRate]);


            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
            {
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.PtcAllSensorsWorking, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.PtcAllSensorsWorking]);
            }

            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 5 || ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 4 || ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 2 || ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 3)
            {
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.VacActualRight, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.VacActualRight]);
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.VacSetRight, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.VacSetRight]);
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.VacHeaderRateRight, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.VacHeaderRateRight]);
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.AirTCLow, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.AirTCLow]);

            }

            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId != 3)
            {
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.PressureActual, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.PressureActual]);
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.PressureSet, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.PressureSet]);
                _hamburgerMenuLeftTagConfigurations.Add(HamburgerMenuLeftTagNames.PressureRate, (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[hamburgerMenuTagConfigurations.PressureRate]);

            }

            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
          




            pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            pageTagConfiguration = pageTagConfigurationService.GetByName("Alarm");
            jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            AlarmPageTagConfigurations = JsonConvert.DeserializeObject<AlarmTagConfigurations>(jsonSerializedString);
            AlarmPageTagConfigurations.SlienceHorn = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(AlarmPageTagConfigurations.SlienceHorn)];


        }

        public void ChangeServiceStatesIndicators()
        {
            var serviceState = ProcessManager.Instance.ServiceStates();
            ReadServiceChecker = serviceState["ReadService"];
            WriteServiceChecker = serviceState["WriteService"];
            DataLogServiceChecker = serviceState["DataLoggerService"];
            AlarmServiceChecker = serviceState["AlarmService"];
            CacheServiceChecker = serviceState["CacheService"];
            ProcessManagerServiceChecker = serviceState["ProcessManagerService"];
            SynchronizationServiceChecker = serviceState["SynchronizationService"];
        }

        public bool CheckIfWarningPanelsClosed()
        {
            if (ServiceWarningPanelVisibility == Visibility.Collapsed && AccountWarningPanelVisibility == Visibility.Collapsed 
                && MasterWarningPanelVisibility == Visibility.Collapsed)
                return true;

            return false;
        }

        /// <summary>
        /// Includes necessary permissions to the group id 1 which is top senior person from customer side.
        /// </summary>
        private void IncludeRequiredPermissions()
        {
            // 42 = User Management Page
            if(_activeUserGroup.id == 1 && !_activeUserGroup.PermissionIds.Contains(42))
            {
                _activeUserGroup.PermissionIds = _activeUserGroup.PermissionIds.Concat(new int[] { 42 }).ToArray();
                _userGroupService.Update(_activeUserGroup);
            }
        }

        public void ContinuousUpdate()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            IsEnterPartsChecked = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.EnterPartsOk, false);
            IsRecipeEditorChecked = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.RecipeOk, false);
            IntegrityCheckRunVal = plcCommandManager.Get<bool>((SiemensTagConfiguration)_globalTagConfigurations.LeakageTestInfoCheckStatusRun, false);
            _integrityCheckValOk = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.IntegrityCheckOk, false);
            _integrityCheckSkipVal = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.SkipIntegrityCheck, false);

            if (_integrityCheckValOk == true || _integrityCheckSkipVal == true)
                IsIntegrityChecked = true;
            else
                IsIntegrityChecked = false;

            if (IsHamburgerMenuExpanded.Value)
            {
                _hamburgerMenuLeftModel.ActualTemperatureCalcSetPoint = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.ActualTemperatureCalcSetPoint], false);
                _hamburgerMenuLeftModel.AirTemperatureCalcSetPoint = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTemperatureCalcSetPoint], false);
                _hamburgerMenuLeftModel.BatchElapsedTime = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.BatchElapsedTime], false);
                _hamburgerMenuLeftModel.BatchName = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.BatchName], false);
                _hamburgerMenuLeftModel.BatchStartTime = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.BatchStartTime], false);
                _hamburgerMenuLeftModel.BatchFinishTime = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.BatchFinishTime], false);
                //_hamburgerMenuLeftModel.BatchTotalWorkingTime = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.BatchTotalWorkingTime], false);



                _hamburgerMenuLeftModel.HighMONPortNumber = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.HighMONPortNumber], false);
                _hamburgerMenuLeftModel.HighMONValue = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.HighMONValue], false);
                _hamburgerMenuLeftModel.HighPTCPortNumber = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.HighPTCPortNumber], false);
                _hamburgerMenuLeftModel.HighPTCValue = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.HighPTCValue], false);
                _hamburgerMenuLeftModel.LowMONPortNumber = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.LowMONPortNumber], false);
                _hamburgerMenuLeftModel.LowMONValue = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.LowMONValue], false);
                _hamburgerMenuLeftModel.LowPTCPortNumber = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.LowPTCPortNumber], false);
                _hamburgerMenuLeftModel.LowPTCValue = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.LowPTCValue], false);
                _hamburgerMenuLeftModel.PtcSetPoint = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PtcSetPoint], false);
                _hamburgerMenuLeftModel.RecipeName = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.RecipeName], false);
                _hamburgerMenuLeftModel.RunStatus = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.RunStatus], false);
                _hamburgerMenuLeftModel.SegDescription = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.SegDescription], false);
                _hamburgerMenuLeftModel.SegElapsedTime = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.SegElapsedTime], false);
                _hamburgerMenuLeftModel.SegmentNo = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.SegmentNo], false);
                _hamburgerMenuLeftModel.SegWaitingTime = plcCommandManager.Get<string>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.SegWaitingTime], false);
                _hamburgerMenuLeftModel.VacActual = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacActual], false);
                _hamburgerMenuLeftModel.VacSet = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacSet], false);

                _hamburgerMenuLeftModel.VacHeaderRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacHeaderRate], false);
                _hamburgerMenuLeftModel.AirTCRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTCRate], false);
                _hamburgerMenuLeftModel.HighPTCRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.HighPTCRate], false);
                _hamburgerMenuLeftModel.LowPTCRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.LowPTCRate], false);
                _hamburgerMenuLeftModel.HighMonRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.HighMonRate], false);
                _hamburgerMenuLeftModel.LowMonRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.LowMonRate], false);
              
                switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
                {
                    case 1:
                        _hamburgerMenuLeftModel.AirTCLow = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTCLow], false);
                        _hamburgerMenuLeftModel.PressureRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureRate], false);
                        _hamburgerMenuLeftModel.PressureActual = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureActual], false);
                        _hamburgerMenuLeftModel.PressureSet = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureSet], false);
                        break;
                    case 20:
                        _hamburgerMenuLeftModel.PtcAllSensorsWorking = plcCommandManager.Get<bool>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PtcAllSensorsWorking], false);
                        _hamburgerMenuLeftModel.PressureRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureRate], false);
                        _hamburgerMenuLeftModel.PressureActual = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureActual], false);
                        _hamburgerMenuLeftModel.PressureSet = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureSet], false);
                        break;
                    case 2:
                    case 4:
                    case 5:
                        _hamburgerMenuLeftModel.AirTCLow = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTCLow], false);
                        _hamburgerMenuLeftModel.VacHeaderRateRight = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacHeaderRateRight], false);
                        _hamburgerMenuLeftModel.VacActualRight = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacActualRight], false);
                        _hamburgerMenuLeftModel.VacSetRight = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacSetRight], false);
                        _hamburgerMenuLeftModel.PressureRate = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureRate], false);
                        _hamburgerMenuLeftModel.PressureActual = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureActual], false);
                        _hamburgerMenuLeftModel.PressureSet = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.PressureSet], false);

                        break;
                    case 3:
                        _hamburgerMenuLeftModel.AirTCLow = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTCLow], false);
                        _hamburgerMenuLeftModel.VacHeaderRateRight = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacHeaderRateRight], false);
                        _hamburgerMenuLeftModel.VacActualRight = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacActualRight], false);
                        _hamburgerMenuLeftModel.VacSetRight = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.VacSetRight], false);

                        //_hamburgerMenuLeftModel.AirTCHigh = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTCHigh], false);
                        //_hamburgerMenuLeftModel.AirTCMedium = plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.AirTCMedium], false);
                        break;
                }

                _hamburgerMenuLeftModel.IsSystemAuto = Convert.ToBoolean(plcCommandManager.Get<float>(_hamburgerMenuLeftTagConfigurations[HamburgerMenuLeftTagNames.IsSystemAuto], false));

                HamburgerMenuLeftModel = _hamburgerMenuLeftModel;
            }

            if (AlarmManager.Instance.IncomingAlarmCount() > 0 && !IncomingAlarmsChecker.Value && (!ContentTitle?.Equals("Alarm") ?? true))
            {
                Thread.Sleep(1000);
                IncomingAlarmsChecker.Value = true;
                if(AlarmPageForceLoadOption.Value && CheckIfWarningPanelsClosed())
                    LoadAlarmWindow();
            } else if (AlarmManager.Instance.IncomingAlarmCount() == 0 || (ContentTitle?.Equals("Alarm") ?? false))
            {
                IncomingAlarmsChecker.Value = false;
            }

            // Check if Integrity Check result is on "running" status, if so then load the page.
            if (!_isIntegrityCheckPageLoaded)
            {
                _isIntegrityCheckPageLoaded = true;
                IntegrityCheckResultCheckerOnStart();
            }
        }

        private async void SilenceHorn()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            Guid guid = Guid.NewGuid();

            await Task.Run(() =>
            {
                plcCommandManager.Set((SiemensTagConfiguration)AlarmPageTagConfigurations.SlienceHorn, true, guid);
            });

            await Task.Delay(2000);

            bool resultValueSet = plcCommandManager.IsUpdatedResult(guid, false, 200);
        }

        private void IntegrityCheckResultCheckerOnStart()
        {
            // Check status on running
            if (IntegrityCheckRunVal)
            {
                LoadIntegrityChecksView(true);
                if (MainWindow != null)
                    MainWindow.SetIntegrityCheckButtonSelected();
            }

            // Check status on pass
            if (_integrityCheckValOk)
            {
                IntegratedCheckResultService integratedCheckResultService = new IntegratedCheckResultService(_connectionString);
                var checkResults = integratedCheckResultService.GetAllByBatchId(ProcessManager.Instance.CurrentProcess.BatchId);

                if (checkResults.Any())
                    return;

                LoadIntegrityChecksView(false, true);
                if (MainWindow != null)
                    MainWindow.SetIntegrityCheckButtonSelected();
            }
        }

        private void DoNothing() { }

        public void UnloadPage(bool isDisableActive)
        {
            CurrentView = null;
            ContentTitle = string.Empty;
            MainWindow.CloseOpenedWindows();

            if (isDisableActive)
                MainWindow.DisableAllButtonsExceptOne(null, true);
            else
            {
                if (ServiceWarningPanelVisibility != Visibility.Visible && MasterWarningPanelVisibility != Visibility.Visible)
                    MainWindow.SetButtonStatesByAuthorization();
            }
        }

        public bool MakeAvailableOnlyUserManagement()
        {
            Load_UserControl_Command.Execute(null);
            MainWindow.DisableAllButtonsExceptOne("UserManagementBtn");
            return true;
        }

        #region Load Views       
        private void LoadVacuumLinesView()
        {
            if (ContentTitle != "Vacuum Lines")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "VacuumLines" || p.PageName == "GenVacuumControl")
                                    .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new VacuumLinesVM(WaitIndicatorControl, PermissionValues, ActiveUser);
                ContentTitle = "Vacuum Lines";
            }
        }
        private void LoadSensorView()
        {
            if (!IsSensorViewOpenedInWindow && ContentTitle != "Sensor View")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "SensorView")
                                   .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new SensorViewVM(WaitIndicatorControl, PermissionValues, ActiveUser);
                ContentTitle = "Sensor View";
            }
        }

        private void LoadSensorViewWindow()
        {
            if (!IsSensorViewOpenedInWindow && ContentTitle != "Sensor View")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                IsSensorViewOpenedInWindow = true;
                Sensor_View_Window sensorViewWindow = new Sensor_View_Window(this);
                sensorViewWindow.MainView = new SensorViewVM(WaitIndicatorControl, PermissionValues, ActiveUser);
                sensorViewWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                sensorViewWindow.Show();
            }
        }

        private void LoadTrendView()
        {
            if (!IsTrendOpenedInWindow && ContentTitle != "Trend")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                CurrentView = new TrendManagementVM(WaitIndicatorControl);
                ContentTitle = "Trend";
            }
        }

        private void LoadTrendWindow()
        {
            if (!IsTrendOpenedInWindow && ContentTitle != "Trend")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                IsTrendOpenedInWindow = true;
                Trend_Window trendWindow = new Trend_Window(this);
                trendWindow.MainView = new TrendManagementVM(WaitIndicatorControl);
                trendWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                trendWindow.Show();
            }
        }

        private void LoadEnterPartsView()
        {
            if (ContentTitle != "Enter Parts")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "EnterParts")
                                   .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new EnterPartsVM(WaitIndicatorControl, PermissionValues, ActiveUser, CurrentBatchNumber, this, IsHamburgerMenuExpanded);
                ContentTitle = "Enter Parts";
            }
        }
        private void LoadRecipeEditorView()
        {
            if (ContentTitle != "Recipe Editor")
            {
                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "RecipeEditor")
                                   .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new RecipeEditorVM(WaitIndicatorControl, IsRecipeTableDataChanged, ActiveUser, PermissionValues, CurrentRecipeName);
                ContentTitle = "Recipe Editor";
            }
        }
        private void LoadIntegrityChecksView()
        {
            if (ContentTitle != "Integrity Checks")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "IntegrityChecks")
                                   .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new IntegrityChecksVM(WaitIndicatorControl, PermissionValues, CurrentView);
                ContentTitle = "Integrity Checks";
            }
        }

        private void LoadIntegrityChecksView(bool startResultsThread = false, bool resetCheckResult = false)
        {
            if (ContentTitle != "Integrity Checks")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "IntegrityChecks")
                                   .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new IntegrityChecksVM(WaitIndicatorControl, PermissionValues, CurrentView, startResultsThread, resetCheckResult);
                ContentTitle = "Integrity Checks";
            }
        }

        private void LoadRunOperationView()
        {
            if (ContentTitle != "Run Operation")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "RunOperation")
                                                     .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new RunOperationVM(WaitIndicatorControl, PermissionValues, ActiveUser, CurrentBatchNumber, CurrentRecipeName, MainWindow);
                ContentTitle = "Run Operation";
            }
        }
        private void LoadManOperationView()
        {
            if (ContentTitle != "Manual Operation")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "ManualOperation" || p.PageName == "GenVacuumControl")
                                                     .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new ManualOperationManagementVM(WaitIndicatorControl, PermissionValues);
                ContentTitle = "Manual Operation";
            }
        }

        private void LoadManOperationWindow()
        {
            if (!IsManOpOpenedInWindow && ContentTitle != "Manual Operation")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                IsManOpOpenedInWindow = true;
                Manual_Operation_Window manOpWindow = new Manual_Operation_Window(this);
                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "ManualOperation" || p.PageName == "GenVacuumControl")
                                                              .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                manOpWindow.MainView = new ManualOperationVM(WaitIndicatorControl, PermissionValues);
                manOpWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                manOpWindow.Show();
            }
        }

        private void LoadAlarmView()
        {
            if (ContentTitle != "Alarm")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                CurrentView = new AlarmVM(WaitIndicatorControl, IncomingAlarmsChecker, AlarmPageForceLoadOption);
                ContentTitle = "Alarm";
            }
        }

        private void LoadAlarmWindow()
        {
            if (!IsAlarmOpenedInWindow && ContentTitle != "Alarm")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                MainWindow.AlarmNewWindowBtnSetOpen();
            }
        }

        private void LoadRecipeView()
        {
            if (ContentTitle != "Recipe")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                PermissionValues = _permissionService.GetAll().Where(p => p.PageName == "ActiveRecipe")
                                                     .ToDictionary(p => p.PermissionTag, p => _activeUserGroup.PermissionIds.Contains(p.id));
                CurrentView = new ActiveRecipeVM(WaitIndicatorControl, PermissionValues, ActiveUser);
                ContentTitle = "Recipe";
            }
        }
        private void LoadReportsView()
        {
            if (CurrentView is RecipeEditorVM)
            {
                RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
            }

            CurrentView = null;
            ContentTitle = string.Empty;
            ReportsVM reportsVM = new ReportsVM(WaitIndicatorControl);
            Reports_Window reportsWindow = new Reports_Window(reportsVM, this);
            reportsWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            reportsWindow.ShowDialog();
        }

        private void LoadCalibrationView()
        {
            if (!IsCalibrationOpenedInWindow && ContentTitle != "Calibration")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
                {
                    case 1:
                        CurrentView = new CalibrationType1VM();
                        break;
                    case 2:
                        CurrentView = new CalibrationType2VM();
                        break;
                    case 3:
                        CurrentView = new CalibrationType3VM();
                        break;
                    case 4:
                    case 5:
                        CurrentView = new CalibrationType4VM();
                        break;
                    case 20:
                        CurrentView = new CalibrationType20VM();
                        break;
                }
                ContentTitle = "Calibration";
            }
        }

        private void LoadCalibrationWindow()
        {
            if (!IsCalibrationOpenedInWindow && ContentTitle != "Calibration")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                IsCalibrationOpenedInWindow = true;
                Calibration_Window calibrationWindow = new Calibration_Window(this);

                switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
                {
                    case 1:
                        calibrationWindow.MainView = new CalibrationType1VM();
                        break;
                    case 2:
                        calibrationWindow.MainView = new CalibrationType2VM();
                        break;
                    case 3:
                        calibrationWindow.MainView = new CalibrationType3VM();
                        break;
                    case 4:
                    case 5:
                        calibrationWindow.MainView = new CalibrationType4VM();
                        break;
                    case 20:
                        calibrationWindow.MainView = new CalibrationType20VM();
                        break;
                }

                calibrationWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                calibrationWindow.Show();
            }
        }

        private void LoadQualityView()
        {
            if (ContentTitle != "Quality")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                CurrentView = new QualityVM();
                ContentTitle = "Quality";
            }
        }

        private void LoadOscillationView()
        {
            if (ContentTitle != "Oscillation")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                CurrentView = new OscillationVM(WaitIndicatorControl);
                ContentTitle = "Oscillation";
            }
        }

        private void LoadUserControlView()
        {
            if (ContentTitle != "User Management")
            {
                if (CurrentView is RecipeEditorVM)
                {
                    RecipeEditorVM recipeEditorVM = (RecipeEditorVM)CurrentView;
                    recipeEditorVM.Recipe_Editor_View.CheckIfAnyUnsavedTableValues();
                }

                CurrentView = new UserManagementVM(ActiveUser);
                ContentTitle = "User Management";
            }
        }
        #endregion
    }
}