using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Synchronization.Enums;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.ItemViews;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;
using Revo.Core.Data;
using RevoScada.DesktopApplication.Models.SettingModels;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class VacuumLinesVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private ApplicationPropertyService _applicationPropertyService;
        private ActiveTagService _activeTagService;
        private ProcessEventLogService _processEventLogService;

        PlcCommandManager _plcCommandManager;
        #endregion

        #region Collections
        public IEnumerable<ActiveTag> ActiveTagVacs;
        public List<SiemensTagConfiguration> SiemensTagConfigurationsVacuumValue { get; set; }
        public List<SiemensTagConfiguration> SiemensTagConfigurationsVacuumPortIsAuto { get; set; }
        public List<SiemensTagConfiguration> SiemensTagConfigurationsVacVentOff { get; set; }
        public VacuumLinesTagConfigurations VacuumLinesTagConfigurations { get; set; }
        private ObservableCollection<VacuumPortItem> _vacuumPortItems;
        public ObservableCollection<VacuumPortItem> VacuumPortItems
        {
            get => _vacuumPortItems;
            set => OnPropertyChanged(ref _vacuumPortItems, value);
        }
        #endregion

        #region Fields
        public int TotalVacs; /// total port numbers
        public int VacItemCount;
        public Vacuum_Lines Vacuum_Lines;
        public VacuumLinesSettingsModel VacuumLinesSettings;
        #region SiemensTagConfigs
        private SiemensTagConfiguration systemVacuumPv;
        private SiemensTagConfiguration rightVsystemVacuumPv;
        private SiemensTagConfiguration systemVacuumSP;
        private SiemensTagConfiguration rightSystemVacuumSP;

        private SiemensTagConfiguration systemVacuumRate;
        private SiemensTagConfiguration rightVsystemVacuumRate;
        private SiemensTagConfiguration monitoringLinesHighMonVacuum;
        private SiemensTagConfiguration monitoringLinesHighMonPort;
        private SiemensTagConfiguration monitoringLinesHighMonVacInTime;
        private SiemensTagConfiguration monitoringLinesLowMonVacuum;
        private SiemensTagConfiguration monitoringLinesLowMonPort;
        private SiemensTagConfiguration monitoringLinesLowMonVacInTime;
        private SiemensTagConfiguration vacuumSetControlStatePid;
        private SiemensTagConfiguration rightVacuumSetControlStatePid;

        private SiemensTagConfiguration vacuumPumpControlStateAuto;
        private SiemensTagConfiguration rightVacuumPumpControlStateAuto;

        private SiemensTagConfiguration vacuumPumpControlStateOnOff;
        private SiemensTagConfiguration rightVacuumPumpControlStateOnOff;

        private SiemensTagConfiguration vacuumSetControlStateAuto;
        private SiemensTagConfiguration rightVacuumSetControlStateAuto;

        private SiemensTagConfiguration vacuumSetControlStateSp;
        private SiemensTagConfiguration rightVacuumSetControlStateSp;

        #endregion
        #endregion

        #region Properties
        private Visibility _containerVisibility;
        public Visibility ContainerVisibility
        {
            get => _containerVisibility;
            set => OnPropertyChanged(ref _containerVisibility, value);
        }
        private int _vacuumPumpControlStateAutoValue;
        public int VacuumPumpControlStateAutoValue
        {
            get => _vacuumPumpControlStateAutoValue;
            set => OnPropertyChanged(ref _vacuumPumpControlStateAutoValue, value);
        }

        private int _rightVacuumPumpControlStateAutoValue;
        public int RightVacuumPumpControlStateAutoValue
        {
            get => _rightVacuumPumpControlStateAutoValue;
            set => OnPropertyChanged(ref _rightVacuumPumpControlStateAutoValue, value);
        }

        private int _vacuumPumpControlStateOnOffValue;
        public int VacuumPumpControlStateOnOffValue
        {
            get => _vacuumPumpControlStateOnOffValue;
            set => OnPropertyChanged(ref _vacuumPumpControlStateOnOffValue, value);
        }

        private int _rightVacuumPumpControlStateOnOffValue;
        public int RightVacuumPumpControlStateOnOffValue
        {
            get => _rightVacuumPumpControlStateOnOffValue;
            set => OnPropertyChanged(ref _rightVacuumPumpControlStateOnOffValue, value);
        }

        private int _vacuumSetControlStateAutoValue;
        public int VacuumSetControlStateAutoValue
        {
            get => _vacuumSetControlStateAutoValue;
            set => OnPropertyChanged(ref _vacuumSetControlStateAutoValue, value);
        }

        private int _setControlStateAutoValueSelection;
        public int SetControlStateAutoValueSelection
        {
            get => _setControlStateAutoValueSelection;
            set => OnPropertyChanged(ref _setControlStateAutoValueSelection, value);
        }

        private int _rightVacuumSetControlStateAutoValue;
        public int RightVacuumSetControlStateAutoValue
        {
            get => _rightVacuumSetControlStateAutoValue;
            set => OnPropertyChanged(ref _rightVacuumSetControlStateAutoValue, value);
        }

        private float _vacuumSetControlStateSpValue;
        public float VacuumSetControlStateSpValue
        {
            get => _vacuumSetControlStateSpValue;
            set => OnPropertyChanged(ref _vacuumSetControlStateSpValue, value);
        }

        private float _setControlStateSpValueSelection;
        public float SetControlStateSpValueSelection
        {
            get => _setControlStateSpValueSelection;
            set => OnPropertyChanged(ref _setControlStateSpValueSelection, value);
        }

        private float _rightVacuumSetControlStateSpValue;
        public float RightVacuumSetControlStateSpValue
        {
            get => _rightVacuumSetControlStateSpValue;
            set => OnPropertyChanged(ref _rightVacuumSetControlStateSpValue, value);
        }

        private float _vacuumSetControlStatePidValue;
        public float VacuumSetControlStatePidValue
        {
            get => _vacuumSetControlStatePidValue;
            set => OnPropertyChanged(ref _vacuumSetControlStatePidValue, value);
        }

        private float _rightVacuumSetControlStatePidValue;
        public float RightVacuumSetControlStatePidValue
        {
            get => _rightVacuumSetControlStatePidValue;
            set => OnPropertyChanged(ref _rightVacuumSetControlStatePidValue, value);
        }

        private float _systemVacuumSPValue;
        public float SystemVacuumSPValue
        {
            get => _systemVacuumSPValue;
            set => OnPropertyChanged(ref _systemVacuumSPValue, value);
        }

        private float _rightVystemVacuumSPValue;
        public float RightSystemVacuumSPValue
        {
            get => _rightVystemVacuumSPValue;
            set => OnPropertyChanged(ref _rightVystemVacuumSPValue, value);
        }


        private float _systemVacuumPvValue;
        public float SystemVacuumPvValue
        {
            get => _systemVacuumPvValue;
            set => OnPropertyChanged(ref _systemVacuumPvValue, value);
        }

        private float _rightVystemVacuumPvValue;
        public float RightSystemVacuumPvValue
        {
            get => _rightVystemVacuumPvValue;
            set => OnPropertyChanged(ref _rightVystemVacuumPvValue, value);
        }

        private float _systemVacuumRateValue;
        public float SystemVacuumRateValue
        {
            get => _systemVacuumRateValue;
            set => OnPropertyChanged(ref _systemVacuumRateValue, value);
        }

        private float _rightVystemVacuumRateValue;
        public float RightSystemVacuumRateValue
        {
            get => _rightVystemVacuumRateValue;
            set => OnPropertyChanged(ref _rightVystemVacuumRateValue, value);
        }

        private float _monitoringLinesHighMonVacuumValue;
        public float MonitoringLinesHighMonVacuumValue
        {
            get => _monitoringLinesHighMonVacuumValue;
            set => OnPropertyChanged(ref _monitoringLinesHighMonVacuumValue, value);
        }
        private float _monitoringLinesHighMonPortValue;
        public float MonitoringLinesHighMonPortValue
        {
            get => _monitoringLinesHighMonPortValue;
            set => OnPropertyChanged(ref _monitoringLinesHighMonPortValue, value);
        }
        private float _monitoringLinesHighMonVacInTimeValue;
        public float MonitoringLinesHighMonVacInTimeValue
        {
            get => _monitoringLinesHighMonVacInTimeValue;
            set => OnPropertyChanged(ref _monitoringLinesHighMonVacInTimeValue, value);
        }
        private float _monitoringLinesLowMonVacuumValue;
        public float MonitoringLinesLowMonVacuumValue
        {
            get => _monitoringLinesLowMonVacuumValue;
            set => OnPropertyChanged(ref _monitoringLinesLowMonVacuumValue, value);
        }
        private float _monitoringLinesLowMonPortValue;
        public float MonitoringLinesLowMonPortValue
        {
            get => _monitoringLinesLowMonPortValue;
            set => OnPropertyChanged(ref _monitoringLinesLowMonPortValue, value);
        }
        private float _monitoringLinesLowMonVacInTimeValue;
        public float MonitoringLinesLowMonVacInTimeValue
        {
            get => _monitoringLinesLowMonVacInTimeValue;
            set => OnPropertyChanged(ref _monitoringLinesLowMonVacInTimeValue, value);
        }
        private string _allVacuumLinesVacTagName;
        public string AllVacuumLinesVacTagName
        {
            get => _allVacuumLinesVacTagName;
            set => OnPropertyChanged(ref _allVacuumLinesVacTagName, value);
        }
        private string _allVacuumLinesVentTagName;
        public string AllVacuumLinesVentTagName
        {
            get => _allVacuumLinesVentTagName;
            set => OnPropertyChanged(ref _allVacuumLinesVentTagName, value);
        }
        private string _allVacuumLinesOffTagName;
        public string AllVacuumLinesOffTagName
        {
            get => _allVacuumLinesOffTagName;
            set => OnPropertyChanged(ref _allVacuumLinesOffTagName, value);
        }
        private string _allVacuumLinesManTagName;
        public string AllVacuumLinesManTagName
        {
            get => _allVacuumLinesManTagName;
            set => OnPropertyChanged(ref _allVacuumLinesManTagName, value);
        }
        private string _allVacuumLinesAutoTagName;
        public string AllVacuumLinesAutoTagName
        {
            get => _allVacuumLinesAutoTagName;
            set => OnPropertyChanged(ref _allVacuumLinesAutoTagName, value);
        }

        private string _controlSetSectionWarningText;
        public string ControlSetSectionWarningText
        {
            get => _controlSetSectionWarningText;
            set => OnPropertyChanged(ref _controlSetSectionWarningText, value);
        }
        #region SiemensTagConfigs
        public SiemensTagConfiguration AllVacuumLinesVac { get; set; }
        public SiemensTagConfiguration AllVacuumLinesVent { get; set; }
        public SiemensTagConfiguration AllVacuumLinesOff { get; set; }
        public SiemensTagConfiguration AllVacuumLinesMan { get; set; }
        public SiemensTagConfiguration AllVacuumLinesAuto { get; set; }
        #endregion                                                     

        public Vacuum_Lines Vacuum_Lines_View { get; set; }

        private bool _lowOpacityOnFilter;
        public bool LowOpacityOnFilter
        {
            get => _lowOpacityOnFilter;
            set => OnPropertyChanged(ref _lowOpacityOnFilter, value);
        }
        private Visibility _vacNotFoundVisibility;
        public Visibility VacNotFoundVisibility
        {
            get => _vacNotFoundVisibility;
            set => OnPropertyChanged(ref _vacNotFoundVisibility, value);
        }

        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }
        public Dictionary<string, string> VacuumLinesLanguageSettings { get; set; }

        #region Vacuum Min/Max Value
        private float _vacuumMinValue;
        public float VacuumMinValue
        {
            get => _vacuumMinValue;
            set => OnPropertyChanged(ref _vacuumMinValue, value);
        }

        private float _vacuumMaxValue;
        public float VacuumMaxValue
        {
            get => _vacuumMaxValue;
            set => OnPropertyChanged(ref _vacuumMaxValue, value);
        }

        #endregion Vacuum Min/Max Value

        private VacuumLinesSettingsModel VacuumLinesSettingsSetter
        {
            get
            {
                VacuumLinesSettingsModel vacuumLinesSettings;

                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("VacuumLinesSettings");

                if (!string.IsNullOrEmpty(applicationProperty?.Value))
                {
                    vacuumLinesSettings = JsonConvert.DeserializeObject<VacuumLinesSettingsModel>(applicationProperty.Value);
                }
                else
                {
                    vacuumLinesSettings = new VacuumLinesSettingsModel();
                }

                return vacuumLinesSettings;
            }
            set
            {
                string vacuumLinesSettingsSerialized = JsonConvert.SerializeObject(value);
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                applicationPropertyService.UpdateByName("VacuumLinesSettings", vacuumLinesSettingsSerialized);
            }
        }

        private object _vacuumLinesItemView;
        public object VacuumLinesItemView
        {
            get => _vacuumLinesItemView;
            set => OnPropertyChanged(ref _vacuumLinesItemView, value);
        }

        private float _controlItemHeight;
        public float ControlItemHeight
        {
            get => _controlItemHeight;
            set => OnPropertyChanged(ref _controlItemHeight, value);
        }
        #endregion

        public VacuumLinesVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions, UserGridModel activeUser)
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _applicationPropertyService = new ApplicationPropertyService(_connectionString);
            _activeTagService = new ActiveTagService(_connectionString);
            _processEventLogService = new ProcessEventLogService(_connectionString);
            VacuumLinesSettings = VacuumLinesSettingsSetter;
            ActiveUser = activeUser;
            Permissions = permissions;

            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            VacNotFoundVisibility = Visibility.Collapsed;
            ContainerVisibility = Visibility.Collapsed;
            ControlSetSectionWarningText = "Please, set the 'Control State SP' value to enable this section.\n(Lütfen bu alanı aktif etmek için 'Control State SP' bölümüne değer girin.)";

            TotalVacs = Convert.ToInt32(ProcessManager.Instance.ApplicationProperties["VACCount"].Value);

            //todo:l id ye göre sıralamada yeni id li eklendiği zaman oluşabilecek sıralama hatası.
            ActiveTagVacs = _activeTagService.ActiveTagsByTagNameKey()
                .Where(x => x.Value.ActiveTagGroupId == ActiveTagGroups.VAC)
                .Select(x => x.Value)
                .OrderBy(x => x.id);


            InitializePageTagConfigurations();
            GetVacuumPortItems();

            // todo:h Implement language preference in a parametric way, currently I'm forcing to using English :/
            if (ApplicationLanguageSettings != null)
                VacuumLinesLanguageSettings = ApplicationLanguageSettings.Eng.VacuumLines;

            AllVacuumLinesVacTagName = AllVacuumLinesVac.TagName;
            AllVacuumLinesVentTagName = AllVacuumLinesVent.TagName;
            AllVacuumLinesOffTagName = AllVacuumLinesOff.TagName;
            AllVacuumLinesManTagName = AllVacuumLinesMan.TagName;
            AllVacuumLinesAutoTagName = AllVacuumLinesAuto.TagName;

            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                case 20:
                    VacuumLinesItemView = new VacuumControlSectionType1();
                    ControlItemHeight = 150;
                    break;
                case 2:
                case 4:
                case 5:
                    VacuumLinesItemView = new VacuumControlSectionType2();
                    ControlItemHeight = 190;
                    break;
                case 3:
                    VacuumLinesItemView = new VacuumControlSectionType3();
                    ControlItemHeight = 190;
                    break;
            }


            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;

            VacuumMinValue = float.Parse(ProcessManager.Instance.ApplicationProperties["VacuumMinValue"].Value);
            VacuumMaxValue = float.Parse(ProcessManager.Instance.ApplicationProperties["VacuumMaxValue"].Value);


        }

        #region PLC Commanding

        private void InitializePageTagConfigurations()
        {
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("VacuumLines");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            VacuumLinesTagConfigurations = JsonConvert.DeserializeObject<VacuumLinesTagConfigurations>(jsonSerializedString);
            SetVacuumLinesDatablock(true);

            SiemensTagConfigurationsVacuumValue = new List<SiemensTagConfiguration>();
            SiemensTagConfigurationsVacuumPortIsAuto = new List<SiemensTagConfiguration>();
            SiemensTagConfigurationsVacVentOff = new List<SiemensTagConfiguration>();

            VacItemCount = Convert.ToInt32(ProcessManager.Instance.ApplicationProperties["VACCount"].Value);

            foreach (var vacuumPort in VacuumLinesTagConfigurations.VacuumPorts.Values.Take(VacItemCount))
            {
                SiemensTagConfiguration mon = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[vacuumPort.Value]);
                SiemensTagConfigurationsVacuumValue.Add(mon);
            }

            foreach (var vacuumPort in VacuumLinesTagConfigurations.VacuumPorts.Values.Take(VacItemCount))
            {
                SiemensTagConfiguration mon = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[vacuumPort.IsAuto]);
                SiemensTagConfigurationsVacuumPortIsAuto.Add(mon);
            }

            foreach (var vacuumPort in VacuumLinesTagConfigurations.VacuumPorts.Values.Take(VacItemCount))
            {
                SiemensTagConfiguration mon = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[vacuumPort.VacVentOff]);
                SiemensTagConfigurationsVacVentOff.Add(mon);
            }

            #region Vacuum Pump Control State Section
            vacuumPumpControlStateAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacPumpControlStateIsAuto];
            vacuumPumpControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacPumpControlStateOnOff];

            #endregion

            #region Vacuum Set Control State
            vacuumSetControlStateAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacSetControlStatusIsAuto];
            vacuumSetControlStateSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacSetControlStatusSp];
            vacuumSetControlStatePid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacSetControlStatusPid];
            #endregion

            #region System Vacuum Values

            systemVacuumSP = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.SystemVacuumValuesSP];
            systemVacuumPv = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.SystemVacuumValuesPv];
            systemVacuumRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.SystemVacuumValuesRate];


            #endregion

            #region Monitoring Lines
            monitoringLinesHighMonVacuum = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesHighMonVacuumValue];
            monitoringLinesHighMonPort = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesHighMonPort];
            monitoringLinesHighMonVacInTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesHighMonVacuumValueInTime];
            monitoringLinesLowMonVacuum = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesLowMonVacuumValue];
            monitoringLinesLowMonPort = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesLowMonPort];
            monitoringLinesLowMonVacInTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesLowMonVacuumValueInTime];
            #endregion

            #region All Vacuum Lines
            AllVacuumLinesVac = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionVac];
            AllVacuumLinesVent = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionVent];
            AllVacuumLinesOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionOff];
            AllVacuumLinesMan = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionManuel];
            AllVacuumLinesAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionAuto];
            #endregion


            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {

                case 2:
                case 3:
                case 4:
                case 5:

                    rightSystemVacuumSP = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightSystemVacuumValuesSP];
                    rightVsystemVacuumPv = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightSystemVacuumValuesPv];
                    rightVsystemVacuumRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightSystemVacuumValuesRate];

                    rightVacuumSetControlStateAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightVacSetControlStatusIsAuto];
                    rightVacuumSetControlStateSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightVacSetControlStatusSp];
                    rightVacuumSetControlStatePid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightVacSetControlStatusPid];

                    rightVacuumPumpControlStateAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightVacPumpControlStateIsAuto];
                    rightVacuumPumpControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.RightVacPumpControlStateOnOff];


                    break;

                case 20:
                    //todo: esra 20 ye göre düzenlemeli!
                    break;
            }
        }

        public async Task<bool> SetVacuumLinesDatablock(bool value)
        {
            bool result = false;

            await Task.Run(() =>
            {
                if (VacuumLinesTagConfigurations.DbNumbers != null)
                {
                    for (int i = 0; i < VacuumLinesTagConfigurations.DbNumbers.Count; i++)
                    {
                        result = ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, VacuumLinesTagConfigurations.DbNumbers[i], value);

                    }
                }
            });
            await Task.Delay(3000);

            return result = false;
        }

        private void GetVacuumPortItems()
        {
            VacuumPortItems = new ObservableCollection<VacuumPortItem>();

            for (int i = 0; i < VacItemCount; i++)
            {
                VacuumPortItem vacuumPortItem = new VacuumPortItem();
                vacuumPortItem.PortName = SiemensTagConfigurationsVacuumValue[i].TagName;
                Task getPlcCommand = Task.Factory.StartNew(() =>
                {
                    vacuumPortItem.PortValue = _plcCommandManager.Get<float>(SiemensTagConfigurationsVacuumValue[i], false);
                    vacuumPortItem.ManuelAutoState = (ManuelAutoState)_plcCommandManager.Get<int>(SiemensTagConfigurationsVacuumPortIsAuto[i], false);
                    vacuumPortItem.VacVentOffState = (VacVentOffState)_plcCommandManager.Get<int>(SiemensTagConfigurationsVacVentOff[i], false);
                });
                VacuumPortItems.Add(vacuumPortItem);
                getPlcCommand.Wait();
            }
        }



        public void ContinuousUpdate(bool allowUpdatingSpValue)
        {
            if (!AllowTimerRun)
            {
                return;
            }
            else
            {
                WaitIndicatorControl.IsWaitIndicatorVisible = false;
            }

            bool getAlwaysUpdatedResult = true;

            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            string FormatOfIntegretyItems = _applicationPropertyService.GetByName("FormatOfIntegretyCheckItems").Value;// it has got be "{0:F0}" or "{0:F2}"
            FormatOfIntegretyCheckItems formatOfIntegretyCheckItems = JsonConvert.DeserializeObject<FormatOfIntegretyCheckItems>(FormatOfIntegretyItems);
            DataConverter dataConverter = new DataConverter();

            VacuumPumpControlStateAutoValue = _plcCommandManager.Get<int>(vacuumPumpControlStateAuto, getAlwaysUpdatedResult);
            VacuumPumpControlStateOnOffValue = _plcCommandManager.Get<int>(vacuumPumpControlStateOnOff, getAlwaysUpdatedResult);
            VacuumSetControlStateAutoValue = _plcCommandManager.Get<int>(vacuumSetControlStateAuto, getAlwaysUpdatedResult);

            if (allowUpdatingSpValue)
                VacuumSetControlStateSpValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(vacuumSetControlStateSp, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));

            VacuumSetControlStatePidValue = _plcCommandManager.Get<float>(vacuumSetControlStatePid, getAlwaysUpdatedResult);
            SystemVacuumSPValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(systemVacuumSP, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            SystemVacuumPvValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(systemVacuumPv, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            SystemVacuumRateValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(systemVacuumRate, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));


            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 4 || ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 2 || ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 3)
            {
                RightVacuumPumpControlStateAutoValue = _plcCommandManager.Get<int>(rightVacuumPumpControlStateAuto, getAlwaysUpdatedResult);
                RightVacuumPumpControlStateOnOffValue = _plcCommandManager.Get<int>(rightVacuumPumpControlStateOnOff, getAlwaysUpdatedResult);
                RightVacuumSetControlStateAutoValue = _plcCommandManager.Get<int>(rightVacuumSetControlStateAuto, getAlwaysUpdatedResult);

                if (allowUpdatingSpValue)
                    RightVacuumSetControlStateSpValue = _plcCommandManager.Get<float>(rightVacuumSetControlStateSp, getAlwaysUpdatedResult);

                RightVacuumSetControlStatePidValue = _plcCommandManager.Get<float>(rightVacuumSetControlStatePid, getAlwaysUpdatedResult);

                RightSystemVacuumSPValue = _plcCommandManager.Get<float>(rightSystemVacuumSP, getAlwaysUpdatedResult);
                RightSystemVacuumPvValue = _plcCommandManager.Get<float>(rightVsystemVacuumPv, getAlwaysUpdatedResult);
                RightSystemVacuumRateValue = _plcCommandManager.Get<float>(rightVsystemVacuumRate, getAlwaysUpdatedResult);
            }

            CheckControlStateSelections();

            MonitoringLinesHighMonVacuumValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(monitoringLinesHighMonVacuum, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            MonitoringLinesHighMonPortValue = _plcCommandManager.Get<float>(monitoringLinesHighMonPort, getAlwaysUpdatedResult);
            MonitoringLinesHighMonVacInTimeValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(monitoringLinesHighMonVacInTime, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            MonitoringLinesLowMonVacuumValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(monitoringLinesLowMonVacuum, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            MonitoringLinesLowMonPortValue = _plcCommandManager.Get<float>(monitoringLinesLowMonPort, getAlwaysUpdatedResult);
            MonitoringLinesLowMonVacInTimeValue = float.Parse(NumericManipulation.ConvertFloatToInteger(_plcCommandManager.Get<float>(monitoringLinesLowMonVacInTime, getAlwaysUpdatedResult), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));



            for (int i = 0; i < VacItemCount; i++)
            {
                float vacuumValue = _plcCommandManager.Get<float>(SiemensTagConfigurationsVacuumValue[i], getAlwaysUpdatedResult);
                int vacVentOffValue = _plcCommandManager.Get<int>(SiemensTagConfigurationsVacVentOff[i], getAlwaysUpdatedResult);
                int autoManValue = _plcCommandManager.Get<int>(SiemensTagConfigurationsVacuumPortIsAuto[i], getAlwaysUpdatedResult);

                VacuumPortItems[i].PortValue = vacuumValue;
                VacuumPortItems[i].VacVentOffState = (VacVentOffState)vacVentOffValue;
                VacuumPortItems[i].ManuelAutoState = (ManuelAutoState)autoManValue;
            }
        }

        private void CheckControlStateSelections()
        {
            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                case 20:
                    SetControlStateAutoValueSelection = VacuumSetControlStateAutoValue;
                    SetControlStateSpValueSelection = VacuumSetControlStateSpValue;
                    break;
                case 2:
                case 4:
                case 5:
                    // In VacuumControlSection type 2 page, there are two "Control State" sections. If one of them meets conditions,
                    // then allow ports to be selected.
                    SetControlStateAutoValueSelection = RightVacuumSetControlStateAutoValue;
                    SetControlStateSpValueSelection = RightVacuumSetControlStateSpValue;

                    if (VacuumSetControlStateAutoValue == 0 && VacuumSetControlStateSpValue != 0)
                    {
                        SetControlStateAutoValueSelection = VacuumSetControlStateAutoValue;
                        SetControlStateSpValueSelection = VacuumSetControlStateSpValue;
                    }
                    break;
                case 3:
                    SetControlStateAutoValueSelection = RightVacuumSetControlStateAutoValue;
                    SetControlStateSpValueSelection = RightVacuumSetControlStateSpValue;
                    break;
                default:
                    break;
            }
        }

        public async Task<bool> SetToPlc<T>(T value, SiemensTagConfiguration siemensTagConfig = null, string siemensTagByName = null)
        {
            LastInvokedCommandTime = DateTime.Now;

            string eventText = string.Empty;
            string setString = string.Empty;
            int intVal = Convert.ToInt32(value);

            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration();
            switch (siemensTagByName)
            {
                case "vacuumPumpControlStateAuto":
                    siemensTagConfiguration = vacuumPumpControlStateAuto;
                    setString = (intVal == 1) ? "AUTO" : "MAN";
                    eventText = string.Format("Vacuum Lines pump control state set to: {0}.", setString);
                    break;
                case "vacuumPumpControlStateAutoRight":
                    siemensTagConfiguration = rightVacuumPumpControlStateAuto;
                    setString = (intVal == 1) ? "AUTO" : "MAN";
                    eventText = string.Format("Vacuum Lines pump control state set to: {0}.", setString);
                    break;
                case "vacuumPumpControlStateOnOff":
                    siemensTagConfiguration = vacuumPumpControlStateOnOff;
                    setString = (intVal == 1) ? "ON" : "OFF";
                    eventText = string.Format("Vacuum Lines pump control state set to: {0}.", setString);
                    break;
                case "vacuumPumpControlStateOnOffRight":
                    siemensTagConfiguration = rightVacuumPumpControlStateOnOff;
                    setString = (intVal == 1) ? "ON" : "OFF";
                    eventText = string.Format("Vacuum Lines pump control state set to: {0}.", setString);
                    break;

                case "vacuumSetControlStateAuto":
                    siemensTagConfiguration = vacuumSetControlStateAuto;
                    setString = (intVal == 1) ? "AUTO" : "MAN";
                    eventText = string.Format("Vacuum Lines set control state set to: {0}.", setString);
                    break;

                case "vacuumSetControlStateAutoRight":
                    siemensTagConfiguration = rightVacuumSetControlStateAuto;
                    setString = (intVal == 1) ? "AUTO" : "MAN";
                    eventText = string.Format("Vacuum Lines set control state set to: {0}.", setString);
                    break;
                case "vacuumSetControlStateSp":
                    siemensTagConfiguration = vacuumSetControlStateSp;
                    eventText = string.Format("Vacuum Lines set control state SP value set to: {0}.", value);
                    break;
                case "vacuumSetControlStateSpRight":
                    siemensTagConfiguration = rightVacuumSetControlStateSp;
                    eventText = string.Format("Vacuum Lines set control state SP value set to: {0}.", value);
                    break;
                case "VAC":
                case "VENT":
                case "OFF":
                case "MAN":
                case "AUTO":
                    siemensTagConfiguration = siemensTagConfig;
                    int portNum = Convert.ToInt32(Regex.Replace(siemensTagConfiguration.TagName, @"[^\d]", "")) + 1;
                    eventText = string.Format("Vacuum Lines port no {0} set to: {1}.", portNum, siemensTagByName);
                    break;
                case "All_VAC":
                case "All_VENT":
                case "All_OFF":
                case "All_MANUAL":
                case "All_AUTO":
                    WaitIndicatorControl.IsWaitIndicatorVisible = true;
                    siemensTagConfiguration = siemensTagConfig;
                    string commandName = siemensTagByName.Split('_')[1];
                    eventText = string.Format("Vacuum Lines all ports set to {0} state.", commandName);
                    break;
                default:
                    eventText = siemensTagByName;
                    siemensTagConfiguration = siemensTagConfig;
                    break;
            }

            bool plcResult = false;
            Guid guid = Guid.NewGuid();

            _plcCommandManager.Set(siemensTagConfiguration, value, guid);
            plcResult = await _plcCommandManager.IsUpdatedResultAsync(guid, false);

            if (plcResult)
            {
                ProcessEventLog vacLinesEventLog = new ProcessEventLog();

                // Get currentBatch object from the cache
                if (ProcessManager.Instance.CurrentProcess != null)
                    vacLinesEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

                vacLinesEventLog.CreateDate = DateTime.Now;
                vacLinesEventLog.Type = ProcessEventLogType.Manual.ToString();
                vacLinesEventLog.EventText = eventText;
                vacLinesEventLog.ModifiedByUserId = ActiveUser.id;

                _processEventLogService.Insert(vacLinesEventLog);

                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                processEventLogAdapter.CreateProcessEventLogSyncIssue(vacLinesEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
            }

            return plcResult;
        }
        #endregion

        public void ApplyFiltering(bool val)
        {
            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            // Save filtering value to App.Settings
            VacuumLinesSettings.GeneralFilter = val ? GeneralFilterState.CurrentItems : GeneralFilterState.AllItems;
            VacuumLinesSettingsSetter = VacuumLinesSettings;

            Vacuum_Lines_View.FilterVacuumPorts(val);
        }
    }
}