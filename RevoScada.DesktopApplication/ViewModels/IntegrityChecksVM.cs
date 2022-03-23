using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Reports;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.ReportTemplates;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;
using System.Threading;
using System.Diagnostics;
using Revo.Core.Data;
using RevoScada.DesktopApplication.Models.SettingModels;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class IntegrityChecksVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private IntegratedCheckResultService _integratedCheckResultService;
        private SkippedIntegratedCheckResultService _skippedIntegratedCheckResultService;
        private BagService _bagService;
        private ApplicationPropertyService _applicationPropertyService;
        #endregion

        #region Collections
        private List<IntegrityChecksItemsTableRow> IntegrityCheckItems;
        private List<IntegrityChecksItemsTableRow> IntegrityCheckPtcItems;

        private List<IntegrityChecksItemsTableRow> _checksItemsTableRows;
        private List<IntegrityChecksItemsTableRow> _checksPtcItemsTableRows;
        private ObservableCollection<IntegrityChecksItemsTableRow> _integrityChecksItemsFiltered;
        public ObservableCollection<IntegrityChecksItemsTableRow> IntegrityCheckItemsFiltered
        {
            get => _integrityChecksItemsFiltered;
            set => OnPropertyChanged(ref _integrityChecksItemsFiltered, value);
        }

        private ObservableCollection<IntegrityChecksItemsTableRow> _integrityChecksPtcItemsFiltered;
        public ObservableCollection<IntegrityChecksItemsTableRow> IntegrityCheckPtcItemsFiltered
        {
            get => _integrityChecksPtcItemsFiltered;
            set => OnPropertyChanged(ref _integrityChecksPtcItemsFiltered, value);
        }


        private Dictionary<string, BagDetailDto> BagDetails;
        private List<IntegrityChecksItemsTagConfiguration> IntegrityChecksItemsTagConfigurationsFiltered;
        private List<IntegrityChecksItemsTagConfiguration> IntegrityChecksItemsPtcTagConfigurationsFiltered;
        public List<IntegrityChecksItemsTagConfiguration> IntegrityChecksItemsTagConfigurations;
        public List<IntegrityChecksItemsTagConfiguration> IntegrityChecksItemsPtcTagConfigurations;
        public Dictionary<string, string> IntegrityCheckLanguageSettings { get; set; }
        #endregion

        #region Fields
        private PlcCommandManager _plcCommandManager;
        private IntegrityChecksTagConfigurations integrityChecksTagConfigurations;
        private SiemensTagConfiguration leakageTestFailureCriteriaTest;
        private SiemensTagConfiguration leakageTestFailureCriteriaSetTime;
        private SiemensTagConfiguration leakageTestInfoCheckStatusOk;
        private SiemensTagConfiguration leakageTestInfoCheckStatusRun;
        private SiemensTagConfiguration leakageTestInfoCheckStatusStop;
        private SiemensTagConfiguration leakageTestInfoCheckStatusFault;
        private SiemensTagConfiguration leakageTestInfoCheckElapsedMinute;
        private SiemensTagConfiguration leakageTestInfoCheckElapsedSecond;
        private SiemensTagConfiguration sensorDataMinMon;
        private SiemensTagConfiguration sensorDataMaxMon;
        private SiemensTagConfiguration sensorDataMinMonTitle;
        private SiemensTagConfiguration sensorDataMaxMonTitle;
        private SiemensTagConfiguration operationCommand;
        private SiemensTagConfiguration skipCommand;
        private SiemensTagConfiguration integrityCheckOk;

        private object _mainView;
        public Integrity_Checks Integrity_Checks_View;
        private bool _isSkippedIntegrityCheckResultSaved;
        private bool _isAnyMonItemSelected;
        private bool _startResultsTask;
        private bool _resetCheckResult;
        #endregion

        #region Properties

        private int _integrityCheckMaxTimeValue;
        public int IntegrityCheckMaxTimeValue
        {
            get => _integrityCheckMaxTimeValue;
            set => OnPropertyChanged(ref _integrityCheckMaxTimeValue, value);
        }

        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }

        private string _integrityCheckTimeFormatTitle;
        public string IntegrityCheckTimeFormatTitle
        {
            get => _integrityCheckTimeFormatTitle;
            set => OnPropertyChanged(ref _integrityCheckTimeFormatTitle, value);
        }

        private float _leakageTestFailureCriteriaTestValue;
        public float LeakageTestFailureCriteriaTestValue
        {
            get => _leakageTestFailureCriteriaTestValue;
            set => OnPropertyChanged(ref _leakageTestFailureCriteriaTestValue, value);
        }
        private int _leakageTestFailureCriteriaSetTimeValue;
        public int LeakageTestFailureCriteriaSetTimeValue
        {
            get => _leakageTestFailureCriteriaSetTimeValue;
            set => OnPropertyChanged(ref _leakageTestFailureCriteriaSetTimeValue, value);
        }
        public bool LeakageTestInfoCheckStatusOkValue { get; set; }
        public bool LeakageTestInfoCheckStatusRunValue { get; set; }
        public bool leakageTestInfoCheckStatusStopValue { get; set; }
        public bool LeakageTestInfoCheckStatusFaultValue { get; set; }
        private string _leakageTestInfoCheckStatusText;
        public string LeakageTestInfoCheckStatusText
        {
            get => _leakageTestInfoCheckStatusText;
            set => OnPropertyChanged(ref _leakageTestInfoCheckStatusText, value);
        }
        private int LeakageTestInfoCheckElapsedMinuteValue { get; set; }
        private int LeakageTestInfoCheckElapsedSecondValue { get; set; }
        private TimeSpan LeakageTestInfoCheckElapsedTimeSpan;
        private string _leakageTestInfoCheckElapsedWholeTime;
        public string LeakageTestInfoCheckElapsedWholeTime
        {
            get => _leakageTestInfoCheckElapsedWholeTime;
            set => OnPropertyChanged(ref _leakageTestInfoCheckElapsedWholeTime, value);
        }
        private float _sensorDataMinMonValue;
        public float SensorDataMinMonValue
        {
            get => _sensorDataMinMonValue;
            set => OnPropertyChanged(ref _sensorDataMinMonValue, value);
        }
        private float _sensorDataMaxMonValue;
        public float SensorDataMaxMonValue
        {
            get => _sensorDataMaxMonValue;
            set => OnPropertyChanged(ref _sensorDataMaxMonValue, value);
        }
        private int _sensorDataMinMonTitleValue;
        public int SensorDataMinMonTitleValue
        {
            get => _sensorDataMinMonTitleValue;
            set => OnPropertyChanged(ref _sensorDataMinMonTitleValue, value);
        }
        private int _sensorDataMaxMonTitleValue;
        public int SensorDataMaxMonTitleValue
        {
            get => _sensorDataMaxMonTitleValue;
            set => OnPropertyChanged(ref _sensorDataMaxMonTitleValue, value);
        }
        public int OperationCommandValue { get; set; }
        private bool _skipCommandValue;
        public bool SkipCommandValue
        {
            get => _skipCommandValue;
            set => OnPropertyChanged(ref _skipCommandValue, value);
        }
        public bool IntegrityCheckOkValue { get; set; }
        private string _batchName;
        public string BatchName
        {
            get => _batchName;
            set
            {
                _batchName = value;
                if (string.IsNullOrEmpty(_batchName))
                {
                    _batchName = null;
                }
                OnPropertyChanged(ref _batchName, _batchName);
            }
        }

        private string _fullRecipeName;
        public string FullRecipeName 
        {
            get => _fullRecipeName;
            set => OnPropertyChanged(ref _fullRecipeName, value);
        }

        private string _abbreviatedRecipeName;
        public string AbbreviatedRecipeName
        {
            get => _abbreviatedRecipeName;
            set 
            {
                _abbreviatedRecipeName = value;

                if(_abbreviatedRecipeName?.Length > 30)
                {
                    var fixedVal = _abbreviatedRecipeName.Substring(0, 26) + "...";
                    _abbreviatedRecipeName = fixedVal;
                    FullRecipeName = value;
                } else
                {
                    FullRecipeName = string.Empty;
                }

                OnPropertyChanged(ref _abbreviatedRecipeName, _abbreviatedRecipeName);
            }
        }
        private Visibility _loadingBarVisibility;
        public Visibility LoadingBarVisibility
        {
            get => _loadingBarVisibility;
            set => OnPropertyChanged(ref _loadingBarVisibility, value);
        }
        private bool _isPTCPortsGridVisible;
        public bool IsPTCPortsGridVisible
        {
            get => _isPTCPortsGridVisible;
            set => OnPropertyChanged(ref _isPTCPortsGridVisible, value);
        }
        private bool _isShowPTCPortsBtnVisible;
        public bool IsShowPTCPortsBtnVisible
        {
            get => _isShowPTCPortsBtnVisible;
            set => OnPropertyChanged(ref _isShowPTCPortsBtnVisible, value);
        }
        private bool _isHidePTCPortsBtnVisible;
        public bool IsHidePTCPortsBtnVisible
        {
            get => _isHidePTCPortsBtnVisible;
            set => OnPropertyChanged(ref _isHidePTCPortsBtnVisible, value);
        }

        #region Button Enabled Properties
        private bool _isStopBtnEnabled;
        public bool IsStopBtnEnabled
        {
            get => _isStopBtnEnabled;
            set => OnPropertyChanged(ref _isStopBtnEnabled, value);
        }

        private bool _isResetBtnEnabled;
        public bool IsResetBtnEnabled
        {
            get => _isResetBtnEnabled;
            set => OnPropertyChanged(ref _isResetBtnEnabled, value);
        }

        private bool _isStartBtnEnabled;
        public bool IsStartBtnEnabled
        {
            get => _isStartBtnEnabled;
            set => OnPropertyChanged(ref _isStartBtnEnabled, value);
        }

        private bool _isSkipBtnEnabled;
        public bool IsSkipBtnEnabled
        {
            get => _isSkipBtnEnabled;
            set => OnPropertyChanged(ref _isSkipBtnEnabled, value);
        }

        private bool _isViewReportsBtnEnabled;
        public bool IsViewReportsBtnEnabled
        {
            get => _isViewReportsBtnEnabled;
            set => OnPropertyChanged(ref _isViewReportsBtnEnabled, value);
        }

        private bool _isCriteriaTestValueBtnEnabled;
        public bool IsCriteriaTestValueBtnEnabled
        {
            get => _isCriteriaTestValueBtnEnabled;
            set => OnPropertyChanged(ref _isCriteriaTestValueBtnEnabled, value);
        }

        private bool _isCriteriaManTimeBtnEnabled;
        public bool IsCriteriaManTimeBtnEnabled
        {
            get => _isCriteriaManTimeBtnEnabled;
            set => OnPropertyChanged(ref _isCriteriaManTimeBtnEnabled, value);
        }
        #endregion
        #endregion

        #region Commands
        public ICommand SkipCommand { get; set; }
        public ICommand StartIntegrityCheckCommand { get; set; }
        public ICommand StopIntegrityCheckCommand { get; set; }
        public ICommand ResetIntegrityCheckCommand { get; set; }
        public ICommand ViewReportsCommand { get; set; }
        #endregion

        public IntegrityChecksVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions, object mainview, bool startResultsTask = false, 
                                 bool resetCheckResult = false)
        {
            WaitIndicatorControl = waitIndicatorControl;
            _mainView = mainview;
            _startResultsTask = startResultsTask;
            _resetCheckResult = resetCheckResult;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            IsPTCPortsGridVisible = false;
            IsShowPTCPortsBtnVisible = true;
            IsHidePTCPortsBtnVisible = false;
            FullRecipeName = string.Empty;
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _integratedCheckResultService = new IntegratedCheckResultService(_connectionString);
            _skippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(_connectionString);
            _bagService = new BagService(_connectionString);
            _applicationPropertyService = new ApplicationPropertyService(_connectionString);
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            Permissions = permissions;

            // Commands
            SkipCommand = new RelayCommand(SkipOperationAsync);
            StartIntegrityCheckCommand = new RelayCommand(StartIntegrityCheckAsync);
            StopIntegrityCheckCommand = new RelayCommand(StopIntegrityCheckAsync);
            ResetIntegrityCheckCommand = new RelayCommand(ResetIntegrityCheckAsync);
            ViewReportsCommand = new RelayCommand(ViewReports);

            IntegrityChecksItemsTagConfigurationsFiltered = new List<IntegrityChecksItemsTagConfiguration>();
            IntegrityChecksItemsPtcTagConfigurationsFiltered = new List<IntegrityChecksItemsTagConfiguration>();
            IntegrityCheckItems = new List<IntegrityChecksItemsTableRow>();
            IntegrityCheckPtcItems = new List<IntegrityChecksItemsTableRow>();
            IntegrityCheckItemsFiltered = new ObservableCollection<IntegrityChecksItemsTableRow>();
            IntegrityCheckPtcItemsFiltered = new ObservableCollection<IntegrityChecksItemsTableRow>();
            _checksItemsTableRows = new List<IntegrityChecksItemsTableRow>();
            _checksPtcItemsTableRows = new List<IntegrityChecksItemsTableRow>();

            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
            IntegrityCheckTimeFormatTitle = ProcessManager.Instance.ApplicationProperties["IntegrityCheckTimeFormatTitle"].Value;
            IntegrityCheckMaxTimeValue = Convert.ToInt32(ProcessManager.Instance.ApplicationProperties["IntegrityCheckMaxTimeValue"].Value);

            if (ApplicationLanguageSettings != null)
                IntegrityCheckLanguageSettings = ApplicationLanguageSettings.Eng.GlobalPortName;
        }

        public void ContinuousUpdate()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            string FormatOfIntegretyItems = _applicationPropertyService.GetByName("FormatOfIntegretyCheckItems").Value;// it has got be "{0:F0}" or "{0:F2}"
            FormatOfIntegretyCheckItems formatOfIntegretyCheckItems = JsonConvert.DeserializeObject<FormatOfIntegretyCheckItems>(FormatOfIntegretyItems);

            if (IntegrityCheckItems.Count == 0)
            {
                InitializePageTagConfigurations();
                GetIntegrityCheckTableValuesByBatch();
                LeakageTestFailureCriteriaTestValue = plcCommandManager.Get<float>(leakageTestFailureCriteriaTest, false);
                LeakageTestFailureCriteriaSetTimeValue = plcCommandManager.Get<int>(leakageTestFailureCriteriaSetTime, false);

                WaitIndicatorControl.IsWaitIndicatorVisible = false;
            }

            LeakageTestInfoCheckStatusOkValue = plcCommandManager.Get<bool>(leakageTestInfoCheckStatusOk, false);
            LeakageTestInfoCheckStatusRunValue = plcCommandManager.Get<bool>(leakageTestInfoCheckStatusRun, false);
            leakageTestInfoCheckStatusStopValue = plcCommandManager.Get<bool>(leakageTestInfoCheckStatusStop, false);
            LeakageTestInfoCheckStatusFaultValue = plcCommandManager.Get<bool>(leakageTestInfoCheckStatusFault, false);
            LeakageTestInfoCheckElapsedMinuteValue = plcCommandManager.Get<int>(leakageTestInfoCheckElapsedMinute, false);
            LeakageTestInfoCheckElapsedSecondValue = plcCommandManager.Get<int>(leakageTestInfoCheckElapsedSecond, false);
            LeakageTestInfoCheckElapsedTimeSpan = new TimeSpan(0, LeakageTestInfoCheckElapsedMinuteValue, LeakageTestInfoCheckElapsedSecondValue);
            LeakageTestInfoCheckElapsedWholeTime = LeakageTestInfoCheckElapsedTimeSpan.ToString("mm\\:ss");
            SensorDataMinMonValue = plcCommandManager.Get<float>(sensorDataMinMon, false);
            SensorDataMaxMonValue = plcCommandManager.Get<float>(sensorDataMaxMon, false);
            SensorDataMinMonTitleValue = plcCommandManager.Get<int>(sensorDataMinMonTitle, false);
            SensorDataMaxMonTitleValue = plcCommandManager.Get<int>(sensorDataMaxMonTitle, false);
            OperationCommandValue = plcCommandManager.Get<int>(operationCommand, false);
            SkipCommandValue = plcCommandManager.Get<bool>(skipCommand, false);
            IntegrityCheckOkValue = plcCommandManager.Get<bool>(integrityCheckOk, false);

            #region Datagrid update
            for (int i = 0; i < IntegrityCheckItemsFiltered.Count; i++)
            {
                IntegrityCheckItemsFiltered[i].Actual = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>(IntegrityChecksItemsTagConfigurationsFiltered[i].Actual, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));  //String.Format("{0:0.#}", 0.0);
                IntegrityCheckItemsFiltered[i].Start = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>(IntegrityChecksItemsTagConfigurationsFiltered[i].Start, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
                IntegrityCheckItemsFiltered[i].Finish = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>(IntegrityChecksItemsTagConfigurationsFiltered[i].Finish, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
                IntegrityCheckItemsFiltered[i].Deviation = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>(IntegrityChecksItemsTagConfigurationsFiltered[i].Deviation, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));

            }

            // PTC Update
            for (int i = 0; i < IntegrityCheckPtcItemsFiltered.Count; i++)
            {
                IntegrityCheckPtcItemsFiltered[i].Actual = plcCommandManager.Get<float>(IntegrityChecksItemsPtcTagConfigurationsFiltered[i].Actual, false);
                IntegrityCheckPtcItemsFiltered[i].Start = plcCommandManager.Get<float>(IntegrityChecksItemsPtcTagConfigurationsFiltered[i].Start, false);
                IntegrityCheckPtcItemsFiltered[i].Finish = plcCommandManager.Get<float>(IntegrityChecksItemsPtcTagConfigurationsFiltered[i].Finish, false);
            }
            #endregion

            LeakageTestInfoCheckStatusText = GetTestInfoCheckStatus();

            CheckButtonsAreEnabled();
            CheckCriteriaButtonsAreEnabled();

            if(_startResultsTask)
            {
                _startResultsTask = false;
                IntegrityCheckResultsTaskStarter();
            }

            if(_resetCheckResult)
            {
                _resetCheckResult = false;
                WaitIndicatorControl.IsWaitIndicatorVisible = true;
                ResetIntegrityCheckOnStartAsync();
            }
        }

        public int GetCriteriaTestValueFromPlc()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            return plcCommandManager.Get<int>(leakageTestFailureCriteriaTest, false);
        }

        private void CheckButtonsAreEnabled()
        {
            bool isBatchRunning = ProcessManager.Instance.IsBatchRunning();

            bool isEnabledValue = false;
            bool isStartBtnEnabled = false;
            bool isStopBtnEnabled = false;
            if (isBatchRunning)
                isEnabledValue = false;
            else
                isEnabledValue = true;

            if (string.IsNullOrEmpty(GetTestInfoCheckStatus()) || GetTestInfoCheckStatus().Equals("STOPPED"))
                isStartBtnEnabled = true;
            else
                isStartBtnEnabled = false;

            if(GetTestInfoCheckStatus().Equals("RUNNING"))
                isStopBtnEnabled = true;
            else
                isStopBtnEnabled = false;

            IsStopBtnEnabled = (isEnabledValue && !SkipCommandValue && isStopBtnEnabled);
            IsResetBtnEnabled = (isEnabledValue && !isStartBtnEnabled);
            IsStartBtnEnabled = (isEnabledValue && !SkipCommandValue && isStartBtnEnabled);
            IsSkipBtnEnabled = (isEnabledValue && !SkipCommandValue && string.IsNullOrEmpty(GetTestInfoCheckStatus()));
        }

        private void CheckCriteriaButtonsAreEnabled()
        {
            if (LeakageTestFailureCriteriaTestValue == 0)
                IsCriteriaTestValueBtnEnabled = true;
            else
                IsCriteriaTestValueBtnEnabled = false;

            if (LeakageTestFailureCriteriaSetTimeValue == 0)
                IsCriteriaManTimeBtnEnabled = true;
            else
                IsCriteriaManTimeBtnEnabled = false;
        }

        #region PLC commanding
        private void InitializePageTagConfigurations()
        {
            BatchName = ProcessManager.Instance.CurrentProcess.LoadNumber;
            AbbreviatedRecipeName = ProcessManager.Instance.CurrentProcess.ActiveRecipeName;

            Task getProcessBagDetailsTask = Task.Factory.StartNew(() => { BagDetails = ProcessManager.Instance.ProcessBagDetails; });

            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("IntegrityChecks");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            integrityChecksTagConfigurations = JsonConvert.DeserializeObject<IntegrityChecksTagConfigurations>(jsonSerializedString);

            leakageTestFailureCriteriaTest = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(integrityChecksTagConfigurations.LeakageTestFailureCriteriaTestValue)];
            leakageTestFailureCriteriaSetTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestFailureSetTime];
            leakageTestInfoCheckStatusOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestInfoCheckStatusOk];
            leakageTestInfoCheckStatusRun = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestInfoCheckStatusRun];
            leakageTestInfoCheckStatusStop = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestInfoCheckStatusStop];
            leakageTestInfoCheckStatusFault = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestInfoCheckStatusFault];
            leakageTestInfoCheckElapsedMinute = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestInfoCheckElapsedMinute];
            leakageTestInfoCheckElapsedSecond = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.LeakageTestInfoCheckElapsedSecond];
            sensorDataMinMon = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.SensorDataMinMonValue];
            sensorDataMaxMon = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.SensorDataMaxMonValue];
            sensorDataMinMonTitle = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.SensorDataMinMonTitle];
            sensorDataMaxMonTitle = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.SensorDataMaxMonTitle];
            operationCommand = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.OperationCommand];
            skipCommand = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.SkipCommand];
            integrityCheckOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[integrityChecksTagConfigurations.IntegrityCheckOk];

            IntegrityChecksItemsTagConfigurations = new List<IntegrityChecksItemsTagConfiguration>();
            IntegrityChecksItemsPtcTagConfigurations = new List<IntegrityChecksItemsTagConfiguration>();
            if (ApplicationLanguageSettings != null)
                IntegrityCheckLanguageSettings = ApplicationLanguageSettings.Eng.GlobalPortName;


            //foreach (var item in integrityChecksTagConfigurations.IntegrityChecksItems["MON"])
            foreach (var item in integrityChecksTagConfigurations.IntegrityChecksItems[IntegrityCheckLanguageSettings["monitoringTextName"]])
            {
                IntegrityChecksItemsTagConfiguration integrityChecksItemsTagConfiguration = new IntegrityChecksItemsTagConfiguration();
                integrityChecksItemsTagConfiguration.PortName = item.Key;
                integrityChecksItemsTagConfiguration.Actual = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Actual]);
                integrityChecksItemsTagConfiguration.Start = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Start]);
                integrityChecksItemsTagConfiguration.Finish = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Finish]);
                integrityChecksItemsTagConfiguration.Deviation = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Deviation]);

                IntegrityChecksItemsTagConfigurations.Add(integrityChecksItemsTagConfiguration);
            }

            IntegrityChecksItemsTagConfigurationsFiltered = IntegrityChecksItemsTagConfigurations;

            foreach (var item in integrityChecksTagConfigurations.IntegrityChecksItems["PTC"])
            {
                IntegrityChecksItemsTagConfiguration integrityChecksItemsTagConfiguration = new IntegrityChecksItemsTagConfiguration();
                integrityChecksItemsTagConfiguration.PortName = item.Key;
                integrityChecksItemsTagConfiguration.Actual = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Actual]);
                integrityChecksItemsTagConfiguration.Start = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Start]);
                integrityChecksItemsTagConfiguration.Finish = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[item.Value.Finish]);

                IntegrityChecksItemsPtcTagConfigurations.Add(integrityChecksItemsTagConfiguration);
            }

            IntegrityChecksItemsPtcTagConfigurationsFiltered = IntegrityChecksItemsPtcTagConfigurations;

            getProcessBagDetailsTask.Wait();
        }

        private string GetTestInfoCheckStatus()
        {
            IsViewReportsBtnEnabled = true;

            if(SkipCommandValue)
            {
                IsViewReportsBtnEnabled = true;
                IsSkipBtnEnabled = false;
                return "SKIPPED";
            }
            if (LeakageTestInfoCheckStatusRunValue)
            {
                IsSkipBtnEnabled = false;
                IsViewReportsBtnEnabled = false;
                return "RUNNING";
            }
            if (leakageTestInfoCheckStatusStopValue)
            {
                IsSkipBtnEnabled = false;
                IsViewReportsBtnEnabled = false;
                return "STOPPED";
            }
            if (LeakageTestInfoCheckStatusOkValue)
            {
                IsViewReportsBtnEnabled = true;
                IsSkipBtnEnabled = false;
                return "PASS";
            }
            if (LeakageTestInfoCheckStatusFaultValue)
            {
                IsSkipBtnEnabled = false;
                IsViewReportsBtnEnabled = false;
                return "FAULT";
            }

            return string.Empty;
        }

        public void GetIntegrityCheckTableValuesByBatch()
        {
            // MON
            foreach (var integrityChecksItem in IntegrityChecksItemsTagConfigurations)
            {
                IntegrityChecksItemsTableRow integrityChecksItemsTableRow = new IntegrityChecksItemsTableRow();
                integrityChecksItemsTableRow.ActualName = (BagDetails.ContainsKey(integrityChecksItem.PortName)) ? integrityChecksItem.PortName + " - " + BagDetails[integrityChecksItem.PortName].BagName : integrityChecksItem.PortName;
                integrityChecksItemsTableRow.BagId = (BagDetails.ContainsKey(integrityChecksItem.PortName)) ? BagDetails[integrityChecksItem.PortName].BagId : 0;
                integrityChecksItemsTableRow.BatchId = (BagDetails.ContainsKey(integrityChecksItem.PortName)) ? BagDetails[integrityChecksItem.PortName].BatchId : 0;
                if (integrityChecksItemsTableRow.BagId > 0)
                    integrityChecksItemsTableRow.IsItSelected = true;

                IntegrityCheckItems.Add(integrityChecksItemsTableRow);
            }

            IntegrityCheckItemsFiltered = IntegrityCheckItems.ToObservableCollection();
            _isAnyMonItemSelected = IntegrityCheckItems.Where(m => m.IsItSelected).Any();

            // PTC
            foreach (var integrityChecksItem in IntegrityChecksItemsPtcTagConfigurations)
            {
                IntegrityChecksItemsTableRow integrityChecksItemsPtcTableRow = new IntegrityChecksItemsTableRow();
                integrityChecksItemsPtcTableRow.ActualName = (BagDetails.ContainsKey(integrityChecksItem.PortName)) ? integrityChecksItem.PortName + " - " + BagDetails[integrityChecksItem.PortName].BagName : integrityChecksItem.PortName;
                integrityChecksItemsPtcTableRow.BagId = (BagDetails.ContainsKey(integrityChecksItem.PortName)) ? BagDetails[integrityChecksItem.PortName].BagId : 0;
                integrityChecksItemsPtcTableRow.BatchId = (BagDetails.ContainsKey(integrityChecksItem.PortName)) ? BagDetails[integrityChecksItem.PortName].BatchId : 0;
                if (integrityChecksItemsPtcTableRow.BagId > 0)
                    integrityChecksItemsPtcTableRow.IsItSelected = true;

                IntegrityCheckPtcItems.Add(integrityChecksItemsPtcTableRow);
            }

           // IntegrityCheckPtcItemsFiltered = IntegrityCheckPtcItems.ToObservableCollection();
        }

        private void IntegrityCheckResultsController()
        {
            bool dbSaveVal = false;
            short terminationCounter = 5;
            while(!dbSaveVal)
            {
                Thread.Sleep(1000);

                if(!(_mainView is IntegrityChecksVM))
                    ContinuousUpdate();

                Debug.WriteLine($"################ {IntegrityCheckItemsFiltered[0].Actual} -- {GetTestInfoCheckStatus()} -- {DateTime.Now.Second}");
                Console.WriteLine(DateTime.Now.Second);

                if (GetTestInfoCheckStatus() == "SKIPPED" || GetTestInfoCheckStatus() == "STOPPED" || GetTestInfoCheckStatus() == "FAULT" || 
                    string.IsNullOrEmpty(GetTestInfoCheckStatus()))
                {
                    if(terminationCounter > 0)
                    {
                        terminationCounter--;
                        continue;
                    }

                    ProcessManager.Instance.SaveIntegrityTask = null;
                    break;
                }

                if (GetTestInfoCheckStatus() == "PASS")
                {
                    Thread.Sleep(3000);
                    dbSaveVal = SaveIntegrityCheckResultsToDb(GetTestInfoCheckStatus());
                    ProcessManager.Instance.SaveIntegrityTask = null;
                }
            }
        }

        private bool SaveIntegrityCheckResultsToDb(string checkStatusInfo)
        {
            ContinuousUpdate();

            ActiveTagService _activeTagService = new ActiveTagService(_connectionString);
            _checksItemsTableRows = new List<IntegrityChecksItemsTableRow>();
            _checksPtcItemsTableRows = new List<IntegrityChecksItemsTableRow>();

            var monResultsBySelectedBag = IntegrityCheckItemsFiltered.Where(i => i.BagId > 0).ToObservableCollection();
            var ptcResultsBySelectedBag = IntegrityCheckPtcItemsFiltered.Where(i => i.BagId > 0).ToObservableCollection();

            List<IntegratedCheckResult> integratedCheckResultsList = new List<IntegratedCheckResult>();

            foreach (var integrityCheck in monResultsBySelectedBag)
            {
                IntegratedCheckResult integratedCheckResult = new IntegratedCheckResult();

                integratedCheckResult.ActualValue = integrityCheck.Actual;
                integratedCheckResult.StartValue = integrityCheck.Start;
                integratedCheckResult.FinishValue = integrityCheck.Finish;
                integratedCheckResult.Deviation = integrityCheck.Deviation;
                integratedCheckResult.BagId = integrityCheck.BagId;
                integratedCheckResult.BatchId = integrityCheck.BatchId;
                integratedCheckResult.RequirementValue = (short)LeakageTestFailureCriteriaTestValue;
                integratedCheckResult.CheckResultSaveDate = DateTime.Now;

                try
                {
                    //todo:h Test edilecek
                    integratedCheckResult.SensorTagId = _activeTagService.ActiveTagsByTagNameKey()[integrityCheck.ActualName.Split(' ')[0]].id;
                    // Insert data to database
                    _checksItemsTableRows.Add(integrityCheck);
                }
                catch
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("\n");
                    stringBuilder.AppendLine("<Integrity Check Mon>");
                    stringBuilder.AppendLine($"ActualValue      :{integrityCheck.Actual} ");
                    stringBuilder.AppendLine($"StartValue       :{integrityCheck.Start}");
                    stringBuilder.AppendLine($"FinishValue      :{integrityCheck.Finish}");
                    stringBuilder.AppendLine($"BagId            :{integrityCheck.BagId}");
                    stringBuilder.AppendLine($"BatchId          :{integrityCheck.BatchId}");
                    stringBuilder.AppendLine($"RequirementValue :{LeakageTestFailureCriteriaTestValue}");
                    stringBuilder.AppendLine($"CheckResultSaveDate:{DateTime.Now}");
                    stringBuilder.AppendLine("<Integrity Check Mon\\>");

                    LogManager.Instance.Log($" {stringBuilder}", LogType.Error);
                }

                integratedCheckResultsList.Add(integratedCheckResult);
            }

            foreach (var integrityCheck in ptcResultsBySelectedBag)
            {
                IntegratedCheckResult integratedCheckResult = new IntegratedCheckResult();
                integratedCheckResult.ActualValue = integrityCheck.Actual;
                integratedCheckResult.StartValue = integrityCheck.Start;
                integratedCheckResult.FinishValue = integrityCheck.Finish;
                integratedCheckResult.BagId = integrityCheck.BagId;
                integratedCheckResult.BatchId = integrityCheck.BatchId;
                integratedCheckResult.RequirementValue = (short)LeakageTestFailureCriteriaTestValue;
                integratedCheckResult.CheckResultSaveDate = DateTime.Now;

                integratedCheckResultsList.Add(integratedCheckResult);
                try
                {
                    //todo:h Test edilecek
                    integratedCheckResult.SensorTagId = _activeTagService.ActiveTagsByTagNameKey()[integrityCheck.ActualName.Split(' ')[0]].id;

                    // Insert data to database
                    _checksPtcItemsTableRows.Add(integrityCheck);
                }
                catch
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("\n");
                    stringBuilder.AppendLine("<Integrity Check Ptc>");
                    stringBuilder.AppendLine($"ActualValue      :{integrityCheck.Actual} ");
                    stringBuilder.AppendLine($"StartValue       :{integrityCheck.Start}");
                    stringBuilder.AppendLine($"FinishValue      :{integrityCheck.Finish}");
                    stringBuilder.AppendLine($"BagId            :{integrityCheck.BagId}");
                    stringBuilder.AppendLine($"BatchId          :{integrityCheck.BatchId}");
                    stringBuilder.AppendLine($"RequirementValue :{LeakageTestFailureCriteriaTestValue}");
                    stringBuilder.AppendLine($"CheckResultSaveDate:{DateTime.Now}");
                    stringBuilder.AppendLine("<Integrity Check Ptc\\>");

                    LogManager.Instance.Log($" {stringBuilder}", LogType.Error);
                }
            }

            _integratedCheckResultService.ResetCheckResult(ProcessManager.Instance.CurrentProcess.BatchId);
            return _integratedCheckResultService.InsertCheckResults(integratedCheckResultsList);
        }

        private List<IntegrityChecksItemsTableRow> GetCheckedItemsFromTable()
        {
            return IntegrityCheckItemsFiltered.Where(i => i.BagId > 0).ToList();
        }


        private List<IntegrityChecksItemsTableRow> CheckedPtcItemsFromTable()
        {

            return IntegrityCheckPtcItemsFiltered.Where(x => x.BagId > 0).ToList();
        }

        private void SaveSkippedIntegrityCheckResultsToDb()
        {
            SkippedIntegratedCheckResult skippedIntegratedCheckResult = new SkippedIntegratedCheckResult();
            skippedIntegratedCheckResult.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;
            skippedIntegratedCheckResult.SkipDate = DateTime.Now;

            // Insert data to database
            _skippedIntegratedCheckResultService.Insert(skippedIntegratedCheckResult);
        }

        private async void SkipOperationAsync()
        {
            var skipResult = WinUIMessageBox.Show("Integrity Checks yapmadan geçmek istediğinize emin misiniz? Skip edildikten sonra start edilemez!", "Skip",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (skipResult == MessageBoxResult.No)
                return;

            // Stop timer
            Integrity_Checks_View.IsControlsEditingMode = true;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            bool plcResult = false;

            Guid guid = Guid.NewGuid();
            // Send skip command to PLC
            _plcCommandManager.Set(skipCommand, true, guid);
            plcResult = await _plcCommandManager.IsUpdatedResultAsync(guid,false, 100);

            WaitIndicatorControl.IsWaitIndicatorVisible = false;
            Integrity_Checks_View.IsControlsEditingMode = false;

            if (plcResult == false)
            {
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                                        MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Then save skipped results to SkippedIntegratedCheckResults table
            if (_isSkippedIntegrityCheckResultSaved == false)
            {
                _isSkippedIntegrityCheckResultSaved = true;
                SaveSkippedIntegrityCheckResultsToDb();
            }
        }

        private async Task<bool> SetCommandToPlcAsync(float testValue, int setTimeValue, int commandValue, bool skipResetValue)
        {
            // Stop timer
            Integrity_Checks_View.IsControlsEditingMode = true;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            bool plcTestValueResult = false;
            bool plcSetTimeValueResult = false;
            bool commandValueResult = false;
            bool plcSetSkipResetResult = false;

            Guid testValueGuid = Guid.NewGuid();
            // Set Test value
            _plcCommandManager.Set(leakageTestFailureCriteriaTest, testValue, testValueGuid);
            plcTestValueResult = await _plcCommandManager.IsUpdatedResultAsync(testValueGuid,false);

            Guid setTimeValueGuid = Guid.NewGuid();
            // Set Manual Time value
            _plcCommandManager.Set(leakageTestFailureCriteriaSetTime, setTimeValue, setTimeValueGuid);
            plcSetTimeValueResult = await _plcCommandManager.IsUpdatedResultAsync(setTimeValueGuid,false);

            Guid setSkipResetGuid = Guid.NewGuid();
            // Send skip command to PLC
            _plcCommandManager.Set(skipCommand, skipResetValue, setSkipResetGuid);
            plcSetSkipResetResult = await _plcCommandManager.IsUpdatedResultAsync(setSkipResetGuid, false, 100);


            // Update UI Values
            LeakageTestFailureCriteriaTestValue = testValue;
            LeakageTestFailureCriteriaSetTimeValue = setTimeValue;

            Guid commandValueGuid = Guid.NewGuid();
            // Send command to PLC
            _plcCommandManager.Set(operationCommand, commandValue, commandValueGuid);
            commandValueResult = await _plcCommandManager.IsUpdatedResultAsync(commandValueGuid,false);

            WaitIndicatorControl.IsWaitIndicatorVisible = false;
            Integrity_Checks_View.IsControlsEditingMode = false;

            if (plcTestValueResult && plcSetTimeValueResult && commandValueResult)
                return true;
            else
                return false;
        }

        private async void StartIntegrityCheckAsync()
        {
            if(LeakageTestFailureCriteriaTestValue == 0)
            {
                WinUIMessageBox.Show("Lütfen Test kriter değerine 0'dan fazla bir değer giriniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isAnyMonItemSelected)
            {
                WinUIMessageBox.Show("Lütfen MON portu seçimi yapınız.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int commandStart = (int)IntegrityChecksOperationCommand.Start;
            bool plcResult = await SetCommandToPlcAsync(LeakageTestFailureCriteriaTestValue, LeakageTestFailureCriteriaSetTimeValue, commandStart,false);

            if (plcResult == false)
            {
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Start a task to save results to database when pass result is available
            IntegrityCheckResultsTaskStarter();
        }

        public void IntegrityCheckResultsTaskStarter() 
        {
            if (ProcessManager.Instance.SaveIntegrityTask == null)
            {
                Action myAction = () => IntegrityCheckResultsController();
                var integrityCheckTask = new Task(myAction);
                integrityCheckTask.Start();
                ProcessManager.Instance.SaveIntegrityTask = integrityCheckTask;
            }
        }

        private async void ResetIntegrityCheckAsync()
        {
            var resetResult = WinUIMessageBox.Show("Check'i resetlemek istediğinize emin misiniz?", "Reset",
                                                   MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resetResult == MessageBoxResult.No)
                return;

            int commandReset = (int)IntegrityChecksOperationCommand.Reset;
            bool plcResult = await SetCommandToPlcAsync(0, 0, commandReset,false);



            if (plcResult == false)
            {
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                                     MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Reset preset time buttons
            if (Integrity_Checks_View != null)
                Integrity_Checks_View.SetPresetTimeRadioButtonsToFalse();

            ProcessManager.Instance.SaveIntegrityTask = null;
            _integratedCheckResultService.ResetCheckResult(ProcessManager.Instance.CurrentProcess.BatchId);
        }

        private async void ResetIntegrityCheckOnStartAsync()
        {
            int commandReset = (int)IntegrityChecksOperationCommand.Reset;
            bool plcResult = await SetCommandToPlcAsync(0, 0, commandReset,false);

            ProcessManager.Instance.SaveIntegrityTask = null;
            _integratedCheckResultService.ResetCheckResult(ProcessManager.Instance.CurrentProcess.BatchId);
        }

        private async void StopIntegrityCheckAsync()
        {
            var resetResult = WinUIMessageBox.Show("Check'i durdurmak istediğinize emin misiniz?", "Stop",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resetResult == MessageBoxResult.No)
                return;

            int commandStop = (int)IntegrityChecksOperationCommand.Stop;
            bool plcResult = await SetCommandToPlcAsync(LeakageTestFailureCriteriaTestValue, LeakageTestFailureCriteriaSetTimeValue, commandStop,false);

            if (plcResult == false)
                WinUIMessageBox.Show("PLC access error! Please check services!", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ViewReports()
        {
            ReportCreator reportCreator = new ReportCreator(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            List<IntegrityChecksItemsTableRow> integrityChecksAllItems = new List<IntegrityChecksItemsTableRow>();

           XtraReport xtraReportItem = null;
            if (_checksItemsTableRows.Count == 0)
            { 
                _checksItemsTableRows = GetCheckedItemsFromTable();
            }

            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
            {
                if (_checksPtcItemsTableRows.Count == 0)
                {
                    _checksPtcItemsTableRows = CheckedPtcItemsFromTable();
                }
                integrityChecksAllItems.AddRange(_checksPtcItemsTableRows);
            }

            integrityChecksAllItems.AddRange(_checksItemsTableRows);
           

            xtraReportItem = reportCreator.IntegrityCheckReport(integrityChecksAllItems, BatchName, AbbreviatedRecipeName);

            ReportViewer reportViewer = new ReportViewer(xtraReportItem);
            reportViewer.ShowDialog();
        }

        public async Task<bool> SetFailureCriteriaTestToPlc()
        {
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            bool plcResult = false;

            Guid guid = Guid.NewGuid();
            _plcCommandManager.Set(leakageTestFailureCriteriaTest, LeakageTestFailureCriteriaTestValue, guid);

            plcResult = await _plcCommandManager.IsUpdatedResultAsync(guid,false);
            WaitIndicatorControl.IsWaitIndicatorVisible = false;

            return plcResult;
        }

        public async Task<bool> SetFailureCriteriaSetTimeValueToPlc()
        {
            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            bool plcResult = false;

            Guid guid = Guid.NewGuid();

            _plcCommandManager.Set(leakageTestFailureCriteriaSetTime, LeakageTestFailureCriteriaSetTimeValue, guid);

            plcResult = await _plcCommandManager.IsUpdatedResultAsync(guid,false);

            WaitIndicatorControl.IsWaitIndicatorVisible = false;

            return plcResult;
        }
        #endregion

        public void FilterIntegrityCheckItems(bool showCurrentItems)
        {
            if (showCurrentItems)
            {
                // MON
                IntegrityCheckItemsFiltered = IntegrityCheckItems.Where(i => i.BagId > 0).ToObservableCollection();
                IntegrityChecksItemsTagConfigurationsFiltered = IntegrityChecksItemsTagConfigurations.Where(i => BagDetails.ContainsKey(i.PortName)).ToList();
                // PTC
                IntegrityCheckPtcItemsFiltered = IntegrityCheckPtcItems.Where(i => i.BagId > 0).ToObservableCollection();
                IntegrityChecksItemsPtcTagConfigurationsFiltered = IntegrityChecksItemsPtcTagConfigurations.Where(i => BagDetails.ContainsKey(i.PortName)).ToList();
            }
            else
            {
                // MON
                IntegrityCheckItemsFiltered = IntegrityCheckItems.ToObservableCollection();
                IntegrityChecksItemsTagConfigurationsFiltered = IntegrityChecksItemsTagConfigurations.ToList();
                // PTC
                IntegrityCheckPtcItemsFiltered = IntegrityCheckPtcItems.ToObservableCollection();
                IntegrityChecksItemsPtcTagConfigurationsFiltered = IntegrityChecksItemsPtcTagConfigurations.ToList();
            }
        }
    }
}
