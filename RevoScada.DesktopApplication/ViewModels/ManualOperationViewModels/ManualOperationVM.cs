using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Synchronization.Enums;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RevoScada.DesktopApplication.Models.SettingModels;
using Revo.Core.Data;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class ManualOperationVM : UserControlBaseVM
    {

        /// ManualOperationPortControlModel 5-10 snde bir okunacak.
        /// ManualOperationFurnaceControlModel 2-3 snde bir (değişebilir) okunacak Dispatcher timer kullanılacak...

        #region Services
        private readonly string _connectionString;
        private ActiveTagService _activeTagService;
        private ProcessEventLogService _processEventLogService;
        #endregion

        #region Properties
        public ManualOperationTagConfigurations ManualOperationTagConfigurations { get; set; }
        private ManualOperationFurnaceControlModel _furnaceControlFormInput;
        public ManualOperationFurnaceControlModel FurnaceControlFormInput
        {
            get => _furnaceControlFormInput;
            set => OnPropertyChanged(ref _furnaceControlFormInput, value);
        }


        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }

        private bool _enableStatuOfVacuumControlPanel;
        public bool EnableStatuOfVacuumControlPanel
        {
            get => _enableStatuOfVacuumControlPanel;
            set => OnPropertyChanged(ref _enableStatuOfVacuumControlPanel, value);
        }

        private string _manuelOperationsMonItemsFormat;
        public string ManuelOperationsMonItemsFormat
        {
            get
            {
                return String.Format("{0:F0}", _manuelOperationsMonItemsFormat);
            }

            set => OnPropertyChanged(ref _manuelOperationsMonItemsFormat, value);

        }




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

        #endregion

        #region Collections
        private ObservableCollection<PortDetailInfo> _ptcPorts;
        public ObservableCollection<PortDetailInfo> PTCPorts
        {
            get => _ptcPorts;
            set => OnPropertyChanged(ref _ptcPorts, value);
        }
        private ObservableCollection<PortDetailInfo> _vacPorts;
        public ObservableCollection<PortDetailInfo> VACPorts
        {
            get => _vacPorts;
            set => OnPropertyChanged(ref _vacPorts, value);
        }
        private ObservableCollection<PortDetailInfo> _monPorts;
        public ObservableCollection<PortDetailInfo> MONPorts
        {
            get => _monPorts;
            set => OnPropertyChanged(ref _monPorts, value);
        }
        #endregion

        #region Fields
        private PlcCommandManager _plcCommandManager;
        public bool AllowUpdatingTempValue;
        public bool AllowUpdatingPressureValue;
        public bool AllowUpdatingVacuumValue;
        #endregion

        public ManualOperationVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions)
        {
            WaitIndicatorControl = waitIndicatorControl;
            Permissions = permissions;
            WaitIndicatorControl.IsWaitIndicatorVisible = false;

            AllowUpdatingTempValue = true;
            AllowUpdatingPressureValue = true;
            AllowUpdatingVacuumValue = true;

            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            PTCPorts = new ObservableCollection<PortDetailInfo>();
            VACPorts = new ObservableCollection<PortDetailInfo>();
            MONPorts = new ObservableCollection<PortDetailInfo>();

            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _activeTagService = new ActiveTagService(_connectionString);
            _processEventLogService = new ProcessEventLogService(_connectionString);

            FurnaceControlFormInput = new ManualOperationFurnaceControlModel();
            InitializePageTagConfigurations();
            GetActiveTags();
            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
            VacuumMinValue = float.Parse(ProcessManager.Instance.ApplicationProperties["VacuumMinValue"].Value);
            VacuumMaxValue = float.Parse(ProcessManager.Instance.ApplicationProperties["VacuumMaxValue"].Value);
        }

        private void InitializePageTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("ManualOperation");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            ManualOperationTagConfigurations = JsonConvert.DeserializeObject<ManualOperationTagConfigurations>(jsonSerializedString);

            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            string FormatOfIntegretyItems = _applicationPropertyService.GetByName("FormatOfIntegretyCheckItems").Value;// it has got be "{0:F0}" or "{0:F2}"
            FormatOfIntegretyCheckItems formatOfIntegretyCheckItems = JsonConvert.DeserializeObject<FormatOfIntegretyCheckItems>(FormatOfIntegretyItems);

            ManuelOperationsMonItemsFormat = FurnaceControlFormInput.HighPtcRate.ToString();

            ManualOperationTagConfigurations.AirTcLow = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLow)];
            ManualOperationTagConfigurations.AirTcLowRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLowRate)];

            ManualOperationTagConfigurations.PtcFanControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcFanControlStateIsAuto)];
            ManualOperationTagConfigurations.PtcFanControlStateIsEnable = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcFanControlStateIsEnable)];
             ManualOperationTagConfigurations.PtcHeatControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcHeatControlStateIsAuto)];
            ManualOperationTagConfigurations.PtcHeatControlStateIsEnable = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcHeatControlStateIsEnable)];
            ManualOperationTagConfigurations.PtcCoolControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcCoolControlStateIsAuto)];
            ManualOperationTagConfigurations.PtcCoolControlStateIsEnable = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcCoolControlStateIsEnable)];
            
            ManualOperationTagConfigurations.PtcValueIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcValueIsAuto)];
            ManualOperationTagConfigurations.PtcValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcValue)];
            
            ManualOperationTagConfigurations.LowPtcPort = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.LowPtcPort)];
            ManualOperationTagConfigurations.LowPtcActual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.LowPtcActual)];
            ManualOperationTagConfigurations.LowPtcRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.LowPtcRate)];
            
            ManualOperationTagConfigurations.HighPtcPort = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.HighPtcPort)];
            ManualOperationTagConfigurations.HighPtcActual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.HighPtcActual)];
            ManualOperationTagConfigurations.HighPtcRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.HighPtcRate)];
           
            ManualOperationTagConfigurations.VacPumpControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateOnOff)];

            ManualOperationTagConfigurations.VacControlStatusPid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusPid)];
            ManualOperationTagConfigurations.VacControlStatusIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusIsAuto)];
            ManualOperationTagConfigurations.VacControlStatusSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusSp)];
            ManualOperationTagConfigurations.VacPumpControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateIsAuto)];
            ManualOperationTagConfigurations.SystemVacuumValuePV = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValuePV)];
            ManualOperationTagConfigurations.SystemVacuumValueRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValueRate)];

            ManualOperationTagConfigurations.MonitoringLinesLowMonPort = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.MonitoringLinesLowMonPort)];
            ManualOperationTagConfigurations.MonitoringLinesLowMonVacuumValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.MonitoringLinesLowMonVacuumValue)];
            ManualOperationTagConfigurations.MonitoringLinesLowMonVacuumValueInTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.MonitoringLinesLowMonVacuumValueInTime)];
            
            ManualOperationTagConfigurations.MonitoringLinesHighMonPort = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.MonitoringLinesHighMonPort)];
            ManualOperationTagConfigurations.MonitoringLinesHighMonVacuumValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.MonitoringLinesHighMonVacuumValue)];
            ManualOperationTagConfigurations.MonitoringLinesHighMonVacuumValueInTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.MonitoringLinesHighMonVacuumValueInTime)];

             ManualOperationTagConfigurations.FanSpeed = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.FanSpeed)];
            ManualOperationTagConfigurations.FanVibration = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.FanVibration)];
            ManualOperationTagConfigurations.PidCoolOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidCoolOut)];

            ManualOperationTagConfigurations.VacuumLineValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacuumLineValue)];
             ManualOperationTagConfigurations.DoorStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.DoorStatus)];

            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                    ManualOperationTagConfigurations.PidAtmosphereOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidAtmosphereOut)];
                    ManualOperationTagConfigurations.FanTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.FanTemperature)];
                    ManualOperationTagConfigurations.PtcHeatControlStatePid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcHeatControlStatePid)];
                    ManualOperationTagConfigurations.PtcFanControlStatePid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcFanControlStatePid)];

                    ManualOperationTagConfigurations.FanRpm = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.FanRpm)];
                    ManualOperationTagConfigurations.PidHeatOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidHeatOut)];
                    ManualOperationTagConfigurations.PidVacOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidVacOut)];
                    //ManualOperationTagConfigurations.PtcWatchAirTempActual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcWatchAirTempActual)];
                    //ManualOperationTagConfigurations.PtcWatchAirTempRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcWatchAirTempRate)];

                    ManualOperationTagConfigurations.PressureValveControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateOnOff)];
                    ManualOperationTagConfigurations.PressureValveControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateIsAuto)];
                    ManualOperationTagConfigurations.PressureSetControlStatusIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureSetControlStatusIsAuto)];
                    ManualOperationTagConfigurations.PressureControlStatusSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureControlStatusSp)];
                    ManualOperationTagConfigurations.SystemPressureValuePV = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValuePV)];
                    ManualOperationTagConfigurations.SystemPressureValueRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValueRate)];

                    ManualOperationTagConfigurations.PidPressureOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidPressureOut)];
                   // ManualOperationTagConfigurations.PressureBar = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureBar)];

                    break;
                case 2: //AC8-9
                    ManualOperationTagConfigurations.PidAtmosphereOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidAtmosphereOut)];

                    ManualOperationTagConfigurations.AirTcHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHigh)];
                    ManualOperationTagConfigurations.AirTcHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHighRate)];

                    ManualOperationTagConfigurations.AirTcMediumHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHigh)];
                    ManualOperationTagConfigurations.AirTcMediumHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHighRate)];

                    ManualOperationTagConfigurations.AirTcMediumLow = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumLow)];
                    ManualOperationTagConfigurations.AirTcMediumLowRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumLowRate)];


                    ManualOperationTagConfigurations.VacPumpControlStateOnOffRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateOnOffRight)];
                    ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight)];
                    ManualOperationTagConfigurations.VacControlStatusPidRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusPidRight)];
                    ManualOperationTagConfigurations.VacControlStatusIsAutoRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusIsAutoRight)];
                    ManualOperationTagConfigurations.VacControlStatusSpRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusSpRight)];
                    ManualOperationTagConfigurations.SystemVacuumValuePVRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValuePVRight)];
                    ManualOperationTagConfigurations.SystemVacuumValueRateRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValueRateRight)];
                    ManualOperationTagConfigurations.PressureLineControlSelect = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureLineControlSelect)];
                    ManualOperationTagConfigurations.PurgeControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PurgeControlStateIsAuto)];
                    ManualOperationTagConfigurations.PurgeControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PurgeControlStateOnOff)];

                 
                    ManualOperationTagConfigurations.PressureValveControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateOnOff)];
                    ManualOperationTagConfigurations.PressureValveControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateIsAuto)];
                    ManualOperationTagConfigurations.PressureSetControlStatusIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureSetControlStatusIsAuto)];
                    ManualOperationTagConfigurations.PressureControlStatusSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureControlStatusSp)];
                    ManualOperationTagConfigurations.SystemPressureValuePV = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValuePV)];
                    ManualOperationTagConfigurations.SystemPressureValueRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValueRate)];

                    ManualOperationTagConfigurations.PidPressureOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidPressureOut)];
                   // ManualOperationTagConfigurations.PressureBar = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureBar)];

                    ManualOperationTagConfigurations.PidCoolOutStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidCoolOutStatus)];

                    break;

                case 3://Cure

                    ManualOperationTagConfigurations.PtcHeatControlStatePid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PtcHeatControlStatePid)];

                    ManualOperationTagConfigurations.AirTcHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHigh)];
                    ManualOperationTagConfigurations.AirTcHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHighRate)];

                    ManualOperationTagConfigurations.AirTcMediumHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHigh)];
                    ManualOperationTagConfigurations.AirTcMediumHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHighRate)];



                    ManualOperationTagConfigurations.VacControlStatusSpRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusSpRight)];
                  
                    ManualOperationTagConfigurations.VacPumpControlStateOnOffRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateOnOffRight)];
                    ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight)];
                    ManualOperationTagConfigurations.VacControlStatusPidRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusPidRight)];
                    ManualOperationTagConfigurations.VacControlStatusIsAutoRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusIsAutoRight)];
                    ManualOperationTagConfigurations.SystemVacuumValuePVRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValuePVRight)];
                    ManualOperationTagConfigurations.SystemVacuumValueRateRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValueRateRight)];

                    ManualOperationTagConfigurations.CirculationFanSpeedFeedback1 = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.CirculationFanSpeedFeedback1)];
                    ManualOperationTagConfigurations.CirculationFanSpeedFeedback2 = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.CirculationFanSpeedFeedback2)];

                  //  ManualOperationTagConfigurations.AirTcHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHigh)];
                   // ManualOperationTagConfigurations.AirTcMediumHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHigh)];
                   // ManualOperationTagConfigurations.AirTcLow = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLow)];

                   // ManualOperationTagConfigurations.AirTcHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHighRate)];
                   // ManualOperationTagConfigurations.AirTcMediumHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHighRate)];
                   // ManualOperationTagConfigurations.AirTcLowRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLowRate)];


                    break;
                case 4://Ac10-11
                case 5:

                    ManualOperationTagConfigurations.PidAtmosphereOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidAtmosphereOut)];

                    ManualOperationTagConfigurations.AirTcHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHigh)];
                    ManualOperationTagConfigurations.AirTcHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHighRate)];

                    ManualOperationTagConfigurations.AirTcMediumHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHigh)];
                    ManualOperationTagConfigurations.AirTcMediumHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHighRate)];


                    ManualOperationTagConfigurations.VacPumpControlStateOnOffRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateOnOffRight)];
                    ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight)];
                    ManualOperationTagConfigurations.VacControlStatusPidRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusPidRight)];
                    ManualOperationTagConfigurations.VacControlStatusIsAutoRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusIsAutoRight)];
                    ManualOperationTagConfigurations.VacControlStatusSpRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.VacControlStatusSpRight)];
                    ManualOperationTagConfigurations.SystemVacuumValuePVRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValuePVRight)];
                    ManualOperationTagConfigurations.SystemVacuumValueRateRight = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemVacuumValueRateRight)];
                    ManualOperationTagConfigurations.PressureLineControlSelect = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureLineControlSelect)];
                    ManualOperationTagConfigurations.PurgeControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PurgeControlStateIsAuto)];
                    ManualOperationTagConfigurations.PurgeControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PurgeControlStateOnOff)];

                   //ManualOperationTagConfigurations.AirTcHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHigh)];
                   //ManualOperationTagConfigurations.AirTcMediumHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHigh)];
                   //ManualOperationTagConfigurations.AirTcLow = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLow)];

                   //ManualOperationTagConfigurations.AirTcHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHighRate)];
                   //ManualOperationTagConfigurations.AirTcMediumHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHighRate)];
                   //ManualOperationTagConfigurations.AirTcLowRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLowRate)];

                    ManualOperationTagConfigurations.PressureValveControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateOnOff)];
                    ManualOperationTagConfigurations.PressureValveControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateIsAuto)];
                    ManualOperationTagConfigurations.PressureSetControlStatusIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureSetControlStatusIsAuto)];
                    ManualOperationTagConfigurations.PressureControlStatusSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureControlStatusSp)];
                    ManualOperationTagConfigurations.SystemPressureValuePV = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValuePV)];
                    ManualOperationTagConfigurations.SystemPressureValueRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValueRate)];

                    ManualOperationTagConfigurations.PidPressureOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidPressureOut)];
                   // ManualOperationTagConfigurations.PressureBar = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureBar)];

                    ManualOperationTagConfigurations.PidCoolOutStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidCoolOutStatus)];

                    break;

                case 20: //Ces
                    ManualOperationTagConfigurations.PressureLineControlSelect = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureLineControlSelect)];
                    ManualOperationTagConfigurations.AirTcHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHigh)];
                    ManualOperationTagConfigurations.AirTcMediumHigh = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHigh)];
                     ManualOperationTagConfigurations.AirTcHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcHighRate)];
                    ManualOperationTagConfigurations.AirTcMediumHighRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcMediumHighRate)];
                    // ManualOperationTagConfigurations.AirTcLow = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLow)];

                    // ManualOperationTagConfigurations.AirTcLowRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.AirTcLowRate)];
                    ManualOperationTagConfigurations.EquipmentTankControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.EquipmentTankControlStateIsAuto)];
                    ManualOperationTagConfigurations.EquipmentTankControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.EquipmentTankControlStateOnOff)];
                    ManualOperationTagConfigurations.EquipmentTankControlStatusPid = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.EquipmentTankControlStatusPid)];
                    ManualOperationTagConfigurations.FanSpeedSetValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.FanSpeedSetValue)];

                    ManualOperationTagConfigurations.PressureValveControlStateOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateOnOff)];
                    ManualOperationTagConfigurations.PressureValveControlStateIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureValveControlStateIsAuto)];
                    ManualOperationTagConfigurations.PressureSetControlStatusIsAuto = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureSetControlStatusIsAuto)];
                    ManualOperationTagConfigurations.PressureControlStatusSp = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureControlStatusSp)];
                    ManualOperationTagConfigurations.SystemPressureValuePV = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValuePV)];
                    ManualOperationTagConfigurations.SystemPressureValueRate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.SystemPressureValueRate)];

                    ManualOperationTagConfigurations.PidPressureOut = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PidPressureOut)];
                    //ManualOperationTagConfigurations.PressureBar = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.PressureBar)];
                    ManualOperationTagConfigurations.TowerWaterTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.TowerWaterTemperature)];
                    ManualOperationTagConfigurations.BatchTotalWorkingTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(ManualOperationTagConfigurations.BatchTotalWorkingTime)];

                    break;
            }
            
    }

        private void GetActiveTags()
        {
            PTCPorts = ProcessManager.Instance.CurrentProcessPortDetailInfos()
                       .Where(c => c.ActiveTagGroup == ActiveTagGroups.PTC && c.IsSelected == true).OrderBy(a => a.PortNumeric).ToObservableCollection();
            VACPorts = ProcessManager.Instance.CurrentProcessPortDetailInfos()
                       .Where(c => c.ActiveTagGroup == ActiveTagGroups.VAC && c.IsSelected == true).OrderBy(a => a.PortNumeric).ToObservableCollection();
            MONPorts = ProcessManager.Instance.CurrentProcessPortDetailInfos()
                       .Where(c => c.ActiveTagGroup == ActiveTagGroups.MON && c.IsSelected == true).OrderBy(a => a.PortNumeric).ToObservableCollection();
        }

        public void ContinuousUpdate()
        {
            if (!AllowTimerRun)
                return;

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            string FormatOfIntegretyItems = _applicationPropertyService.GetByName("FormatOfIntegretyCheckItems").Value;// it has got be "{0:F0}" or "{0:F2}"
            FormatOfIntegretyCheckItems formatOfIntegretyCheckItems = JsonConvert.DeserializeObject<FormatOfIntegretyCheckItems>(FormatOfIntegretyItems);


            #region Temperature Control Section Update
            FurnaceControlFormInput.PtcFanControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcFanControlStateIsAuto, false);
            FurnaceControlFormInput.PtcFanControlStateIsEnable = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcFanControlStateIsEnable, false);
         
            FurnaceControlFormInput.PtcHeatControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcHeatControlStateIsAuto, false);
            FurnaceControlFormInput.PtcHeatControlStateIsEnable = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcHeatControlStateIsEnable, false);
           
            FurnaceControlFormInput.PtcCoolControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcCoolControlStateIsAuto, false);
            FurnaceControlFormInput.PtcCoolControlStateIsEnable = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcCoolControlStateIsEnable, false);

            FurnaceControlFormInput.PtcValueStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcValueIsAuto, false);

            if (AllowUpdatingTempValue)
                FurnaceControlFormInput.PtcValue = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcValue, false);

            FurnaceControlFormInput.LowPtcPort = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.LowPtcPort, false);
            FurnaceControlFormInput.LowPtcActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.LowPtcActual, false);
            FurnaceControlFormInput.LowPtcRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.LowPtcRate, false);
            FurnaceControlFormInput.HighPtcPort = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.HighPtcPort, false);
            FurnaceControlFormInput.HighPtcActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.HighPtcActual, false);
            FurnaceControlFormInput.HighPtcRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.HighPtcRate, false);
            #endregion


            #region Vacuum Control Section Update
            FurnaceControlFormInput.VacPumpControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateIsAuto, false);
            FurnaceControlFormInput.VacPumpControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateOnOff, false);
            FurnaceControlFormInput.VacControlStatusPid = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusPid, false);
            FurnaceControlFormInput.VacControlStatusIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusIsAuto, false);
            FurnaceControlFormInput.HighVacuumPort = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.MonitoringLinesHighMonPort, false);
            FurnaceControlFormInput.LowVacuumPort = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.MonitoringLinesLowMonPort, false);



            FurnaceControlFormInput.VacSystemVacuumActual = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValuePV, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            FurnaceControlFormInput.VacSystemVacuumRate = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValueRate, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            FurnaceControlFormInput.LowVacuumActual = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.MonitoringLinesLowMonVacuumValue, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            FurnaceControlFormInput.LowVacuumRate = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.MonitoringLinesLowMonVacuumValueInTime, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            FurnaceControlFormInput.HighVacuumActual = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.MonitoringLinesHighMonVacuumValue, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
            FurnaceControlFormInput.HighVacuumRate = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.MonitoringLinesHighMonVacuumValueInTime, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));

            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                    FurnaceControlFormInput.PidAtmosphereOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidAtmosphereOut, false);

                    FurnaceControlFormInput.FanTemperature = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.FanTemperature, false);

                    FurnaceControlFormInput.PtcHeatControlStatePid = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcHeatControlStatePid, false);

                    FurnaceControlFormInput.PtcFanControlStatePid = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcFanControlStatePid, false);

                    FurnaceControlFormInput.FanRpm = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.FanRpm, false);
                    FurnaceControlFormInput.PidVacOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidVacOut, false);
                    FurnaceControlFormInput.PidHeatOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidHeatOut, false);
                    //FurnaceControlFormInput.PtcWatchAirTempActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcWatchAirTempActual, false);
                   // FurnaceControlFormInput.PtcWatchAirTempRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcWatchAirTempRate, false);


                    #region Pressure Control Section Update
                    FurnaceControlFormInput.PressureValveControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateIsAuto, false);
                    FurnaceControlFormInput.PressureValveControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateOnOff, false);
                    FurnaceControlFormInput.PressureValueStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureSetControlStatusIsAuto, false);

                    if (AllowUpdatingPressureValue)
                        FurnaceControlFormInput.PressureValue = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureControlStatusSp, false);

                    FurnaceControlFormInput.PressureSystemActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValuePV, false);
                    FurnaceControlFormInput.PressureSystemRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValueRate, false);
                    #endregion

                    FurnaceControlFormInput.PidPressureOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidPressureOut, false);
                //    FurnaceControlFormInput.PressureBar = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureBar, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSp = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSp, false);

                    break;
                case 2:
                    FurnaceControlFormInput.PidAtmosphereOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidAtmosphereOut, false);

                    FurnaceControlFormInput.VacPumpControlStateOnOffRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateOnOffRight, false);
                    FurnaceControlFormInput.VacPumpControlStateIsAutoRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight, false);
                    FurnaceControlFormInput.VacControlStatusPidRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusPidRight, false);
                    FurnaceControlFormInput.VacControlStatusIsAutoRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusIsAutoRight, false);
                    FurnaceControlFormInput.VacSystemVacuumActualRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValuePVRight, false);
                    FurnaceControlFormInput.VacSystemVacuumRateRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValueRateRight, false);
                    FurnaceControlFormInput.PressureLineControlStateIsAir = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureLineControlSelect, false);

                    FurnaceControlFormInput.PurgeControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PurgeControlStateIsAuto, false);
                    FurnaceControlFormInput.PurgeControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PurgeControlStateOnOff, false);

                    FurnaceControlFormInput.AirTcHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHigh, false);
                    FurnaceControlFormInput.AirTcMediumHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHigh, false);
                    FurnaceControlFormInput.AirTcLow = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLow, false);
                    FurnaceControlFormInput.AirTcMediumLow = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumLow, false);

                    FurnaceControlFormInput.AirTcHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHighRate, false);
                    FurnaceControlFormInput.AirTcMediumHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHighRate, false);
                    FurnaceControlFormInput.AirTcLowRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLowRate, false);
                    FurnaceControlFormInput.AirTcMediumLowRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumLowRate, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSpRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSpRight, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSp = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSp, false);

                    #region Pressure Control Section Update
                    FurnaceControlFormInput.PressureValveControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateIsAuto, false);
                    FurnaceControlFormInput.PressureValveControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateOnOff, false);
                    FurnaceControlFormInput.PressureValueStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureSetControlStatusIsAuto, false);

                    if (AllowUpdatingPressureValue)
                        FurnaceControlFormInput.PressureValue = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureControlStatusSp, false);

                    FurnaceControlFormInput.PressureSystemActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValuePV, false);
                    FurnaceControlFormInput.PressureSystemRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValueRate, false);
                    #endregion

                    FurnaceControlFormInput.PidPressureOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidPressureOut, false);
                   // FurnaceControlFormInput.PressureBar = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureBar, false);


                    FurnaceControlFormInput.PidCoolOutStatus = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidCoolOutStatus, false);

                    break;
                case 3:
                    FurnaceControlFormInput.PtcHeatControlStatePid = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PtcHeatControlStatePid, false);

                    FurnaceControlFormInput.AirTcHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHigh, false);
                    FurnaceControlFormInput.AirTcMediumHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHigh, false);
                    FurnaceControlFormInput.AirTcLow = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLow, false);


                    FurnaceControlFormInput.AirTcHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHighRate, false);
                    FurnaceControlFormInput.AirTcMediumHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHighRate, false);
                    FurnaceControlFormInput.AirTcLowRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLowRate, false);

                    FurnaceControlFormInput.VacPumpControlStateOnOffRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateOnOffRight, false);
                    FurnaceControlFormInput.VacPumpControlStateIsAutoRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight, false);
                    FurnaceControlFormInput.VacControlStatusPidRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusPidRight, false);
                    FurnaceControlFormInput.VacControlStatusIsAutoRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusIsAutoRight, false);
                    FurnaceControlFormInput.VacSystemVacuumActualRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValuePVRight, false);
                    FurnaceControlFormInput.VacSystemVacuumRateRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValueRateRight, false);
                    FurnaceControlFormInput.CirculationFanSpeedFeedback1 = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.CirculationFanSpeedFeedback1, false);
                    FurnaceControlFormInput.CirculationFanSpeedFeedback2 = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.CirculationFanSpeedFeedback2, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSpRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSpRight, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSp = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSp, false);
                    break;
                case 4:
                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSp = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSp, false);
                    FurnaceControlFormInput.PurgeControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PurgeControlStateIsAuto, false);
                    FurnaceControlFormInput.PurgeControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PurgeControlStateOnOff, false);
                    FurnaceControlFormInput.PidAtmosphereOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidAtmosphereOut, false);

                    break;
                case 5:
                    FurnaceControlFormInput.PidAtmosphereOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidAtmosphereOut, false);

                    FurnaceControlFormInput.VacPumpControlStateOnOffRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateOnOffRight, false);
                    FurnaceControlFormInput.VacPumpControlStateIsAutoRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacPumpControlStateIsAutoRight, false);
                    FurnaceControlFormInput.VacControlStatusPidRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusPidRight, false);
                    FurnaceControlFormInput.VacControlStatusIsAutoRight = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusIsAutoRight, false);
                    FurnaceControlFormInput.VacSystemVacuumActualRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValuePVRight, false);
                    FurnaceControlFormInput.VacSystemVacuumRateRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemVacuumValueRateRight, false);
                    FurnaceControlFormInput.PressureLineControlStateIsAir = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureLineControlSelect, false);

                    FurnaceControlFormInput.PurgeControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PurgeControlStateIsAuto, false);
                    FurnaceControlFormInput.PurgeControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PurgeControlStateOnOff, false);

                    FurnaceControlFormInput.AirTcHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHigh, false);
                    FurnaceControlFormInput.AirTcMediumHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHigh, false);
                    FurnaceControlFormInput.AirTcLow = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLow, false);

                    FurnaceControlFormInput.AirTcHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHighRate, false);
                    FurnaceControlFormInput.AirTcMediumHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHighRate, false);
                    FurnaceControlFormInput.AirTcLowRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLowRate, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSpRight = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSpRight, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSp = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSp, false);

                    #region Pressure Control Section Update
                    FurnaceControlFormInput.PressureValveControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateIsAuto, false);
                    FurnaceControlFormInput.PressureValveControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateOnOff, false);
                    FurnaceControlFormInput.PressureValueStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureSetControlStatusIsAuto, false);

                    if (AllowUpdatingPressureValue)
                        FurnaceControlFormInput.PressureValue = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureControlStatusSp, false);

                    FurnaceControlFormInput.PressureSystemActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValuePV, false);
                    FurnaceControlFormInput.PressureSystemRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValueRate, false);
                    #endregion

                    FurnaceControlFormInput.PidPressureOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidPressureOut, false);
                    //  FurnaceControlFormInput.PressureBar = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureBar, false);


                    FurnaceControlFormInput.PidCoolOutStatus = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidCoolOutStatus, false);

                    break;
                case 20:
                    EnableStatuOfVacuumControlPanel = ProcessManager.Instance.IsBatchRunning();

                    FurnaceControlFormInput.PressureLineControlStateIsAir = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureLineControlSelect, false);
                    FurnaceControlFormInput.AirTcHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHigh, false);
                    FurnaceControlFormInput.AirTcMediumHigh = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHigh, false);
                    FurnaceControlFormInput.AirTcLow = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLow, false);
                    FurnaceControlFormInput.AirTcHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcHighRate, false);
                    FurnaceControlFormInput.AirTcMediumHighRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcMediumHighRate, false);
                    FurnaceControlFormInput.AirTcLowRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.AirTcLowRate, false);
                    FurnaceControlFormInput.EquipmentTankControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.EquipmentTankControlStateIsAuto, false);
                    FurnaceControlFormInput.EquipmentTankControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.EquipmentTankControlStateOnOff, false);
                    FurnaceControlFormInput.EquipmentTankControlStatusPid = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.EquipmentTankControlStatusPid, false);
                    FurnaceControlFormInput.FanSpeedSetValue = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.FanSpeedSetValue, false);

                    #region Pressure Control Section Update
                    FurnaceControlFormInput.PressureValveControlStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateIsAuto, false);
                    FurnaceControlFormInput.PressureValveControlStateOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureValveControlStateOnOff, false);
                    FurnaceControlFormInput.PressureValueStateIsAuto = plcCommandManager.Get<int>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureSetControlStatusIsAuto, false);

                    if (AllowUpdatingPressureValue)
                        FurnaceControlFormInput.PressureValue = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureControlStatusSp, false);

                    if (AllowUpdatingVacuumValue)
                        FurnaceControlFormInput.VacControlStatusSp = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacControlStatusSp, false);
                    
                    FurnaceControlFormInput.PressureSystemActual = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValuePV, false);
                    FurnaceControlFormInput.PressureSystemRate = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.SystemPressureValueRate, false);
                    #endregion


                    FurnaceControlFormInput.PidPressureOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidPressureOut, false);
                    //FurnaceControlFormInput.PressureBar = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PressureBar, false);
                    FurnaceControlFormInput.TowerWaterTemperature = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.TowerWaterTemperature, false);
                    FurnaceControlFormInput.BatchTotalWorkingTime = plcCommandManager.Get<string>((SiemensTagConfiguration)ManualOperationTagConfigurations.BatchTotalWorkingTime, false);

                    break;

            }

            #endregion

            #region Furnace Watch Section

            FurnaceControlFormInput.VacuumLineValue = float.Parse(NumericManipulation.ConvertFloatToInteger(plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.VacuumLineValue, false), formatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems));
             FurnaceControlFormInput.FanSpeed = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.FanSpeed, false);
            FurnaceControlFormInput.FanVibration = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.FanVibration, false);
            FurnaceControlFormInput.PidCoolOut = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.PidCoolOut, false);
            FurnaceControlFormInput.DoorStatus = plcCommandManager.Get<float>((SiemensTagConfiguration)ManualOperationTagConfigurations.DoorStatus, false);

            #endregion
        }



        public async Task<bool> SetToPlc<T>(SiemensTagConfiguration siemensTagConfig, T value, string tagConfigName = null, string stateText = null)
        {
            LastInvokedCommandTime = DateTime.Now;

            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            bool plcResult = false;

            Guid guid = Guid.NewGuid();
            _plcCommandManager.Set(siemensTagConfig, value, guid);
            plcResult = await _plcCommandManager.IsUpdatedResultAsync(guid, false);

            // Save process event log to DB if setting value to PLC is succesful.
            if (plcResult && tagConfigName != null)
            {
                ProcessEventLog manOpProcessEventLog = new ProcessEventLog();
                string eventText = string.Empty;
                if (stateText == null)
                    stateText = string.Empty;

                switch (tagConfigName)
                {
                    case "PtcFanControlStateIsEnable":
                        eventText = string.Format("Manual Operation Temperature Fan {0}.", stateText);
                        break;
                    case "PtcFanControlStateIsAuto":
                        eventText = string.Format("Manual Operation Temperature Fan state set to: {0}.", stateText);
                        break;
                    case "PtcHeatControlStateIsEnable":
                        eventText = string.Format("Manual Operation Temperature Heat {0}.", stateText);
                        break;
                    case "PtcHeatControlStateIsAuto":
                        eventText = string.Format("Manual Operation Temperature Heat state set to: {0}.", stateText);
                        break;
                    case "PtcCoolControlStateIsEnable":
                        eventText = string.Format("Manual Operation Temperature Cool {0}.", stateText);
                        break;
                    case "PtcCoolControlStateIsAuto":
                        eventText = string.Format("Manual Operation Temperature Cool state set to: {0}.", stateText);
                        break;
                    case "PtcValue":
                        eventText = string.Format("Manual Operation Temperature Value set to: {0:F2}.", value);
                        break;
                    case "PtcValueIsAuto":
                        eventText = string.Format("Manual Operation Temperature state set to: {0:F2}.", stateText);
                        break;
                    case "PressureValveControlStateOnOff":
                        eventText = string.Format("Manual Operation Pressure Control {0}.", stateText);
                        break;
                    case "PressureValveControlStateIsAuto":
                        eventText = string.Format("Manual Operation Pressure Control state set to: {0}.", stateText);
                        break;
                    case "PurgeControlStateOnOff":
                        eventText = string.Format("Manual Operation Purge Control {0}.", stateText);
                        break;
                    case "PurgeControlStateIsAuto":
                        eventText = string.Format("Manual Operation Purge Control state set to: {0}.", stateText);
                        break;
                    case "PressureSetControlStatusIsAuto":
                        eventText = string.Format("Manual Operation Pressure Control Set Value state set to: {0}.", stateText);
                        break;
                    case "PressureControlStatusSp":
                        eventText = string.Format("Manual Operation Pressure Value set to: {0:F2}.", value);
                        break;
                    case "PressureLineControlSelect":
                        eventText = string.Format("Manual Operation Pressure Line Control set to: {0:F2}.", stateText);
                        break;
                    case "VacPumpControlStateOnOff":
                        eventText = string.Format("Manual Operation Vacuum Pump {0}.", stateText);
                        break;
                    case "VacPumpControlStateIsAuto":
                        eventText = string.Format("Manual Operation Vacuum Pump state set to: {0}.", stateText);
                        break;
                    case "VacControlStatusIsAuto":
                        eventText = string.Format("Manual Operation Vacuum Pump Control state set to: {0}.", stateText);
                        break;
                    case "VacControlStatusSp":
                        eventText = string.Format("Manual Operation Vacuum Pump Value set to: {0:F2}.", value);
                        break;
                    case "VacControlStatusSpRight":
                        eventText = string.Format("Manual Operation Right Vacuum Pump Value set to: {0:F2}.", value);
                        break;
                    case "VacPumpControlStateOnOffRight":
                        eventText = string.Format("Manual Operation Right Vacuum Pump {0}.", stateText);
                        break;
                    case "VacPumpControlStateIsAutoRight":
                        eventText = string.Format("Manual Operation Right Vacuum Pump Control state set to: {0}.", stateText);
                        break;
                    case "VacControlStatusIsAutoRight":
                        eventText = string.Format("Manual Operation Right Vacuum Set state set to: {0}.", stateText);
                        break;

                    case "EquipmentTankControlStateOnOff":
                        eventText = string.Format("Manual Operation Equipment Tank On/Off {0}.", stateText);
                        break;
                    case "EquipmentTankControlStateIsAuto":
                        eventText = string.Format("Manual Operation Equipment Tank Control state set to: {0}.", stateText);
                        break;
                    case "FanSpeedSetValue":
                        eventText = string.Format("Manual Operation Temperature Value set to: {0:F2}.", value);
                        break;
                    default:
                        break;
                }
                
                // Get currentBatch object from the cache
                if (ProcessManager.Instance.CurrentProcess != null)
                    manOpProcessEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

                manOpProcessEventLog.CreateDate = DateTime.Now;
                manOpProcessEventLog.Type = ProcessEventLogType.Manual.ToString();

                manOpProcessEventLog.EventText = eventText;
                _processEventLogService.Insert(manOpProcessEventLog);

                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                processEventLogAdapter.CreateProcessEventLogSyncIssue(manOpProcessEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);


                tagConfigName = String.Empty;
            }

            WaitIndicatorControl.IsWaitIndicatorVisible = false;

            return plcResult;
        }
    }
}
