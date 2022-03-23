using Newtonsoft.Json;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.ProcessController;
using RevoScada.DesktopApplication.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RevoScada.DesktopApplication.Views.Popups;
using System.Threading;
using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;
using System.Collections.Generic;
using RevoScada.Entities;
using RevoScada.Entities.Enums;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Synchronization.Enums;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class PipingAndInstrumentationVM : UserControlBaseVM
    {
        #region Fields
        private PlcCommandManager _plcCommandManager;
        private ProcessEventLogService _processEventLogService;
        #endregion

        #region Properties
        public PipingAndInstrumentationTagConfigurations PipingAndInstrumentationTagConfigurations { get; set; }

        private PipingAndInstrumentationFurnaceControlModel _pipingAndInstrumentationFurnaceControlModel;
        public PipingAndInstrumentationFurnaceControlModel PipingAndInstrumentationFurnaceControlModel
        {
            get => _pipingAndInstrumentationFurnaceControlModel;
            set => OnPropertyChanged(ref _pipingAndInstrumentationFurnaceControlModel, value);
        }


        private string _senderPropertyName;

        public string SenderPropertyName
        {
            get => _senderPropertyName;
            set => OnPropertyChanged(ref _senderPropertyName, value);
        }
        #endregion

        #region Commands
        public ICommand SetAlarmCommand { get; set; }
        public ICommand SetMccPanelShowControlPopUpExample { get; set; }
        public ICommand FurnaceControlCommand { get; set; }
        public ICommand ValveControlCommand { get; set; }
        public ICommand ValveControlOnOffCommand { get; set; }
        public ICommand PressureSupplyControlCommand { get; set; }

        #endregion

        public PipingAndInstrumentationVM(Piping_and_Instrumentation_Diagram_Window pipingAndInstrumentationWindow)
        {
            WaitIndicatorControl = new WaitIndicatorControl();
            PipingAndInstrumentationFurnaceControlModel = new PipingAndInstrumentationFurnaceControlModel();
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            _processEventLogService = new ProcessEventLogService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            SetAlarmCommand = new RelayCommand(SetAlarm);
            FurnaceControlCommand = new RelayCommand(FurnaceControlAutoMan);
            ValveControlCommand = new RelayCommand(ValveControl);
            ValveControlOnOffCommand = new RelayCommand(ValveControlOnOff);
            PressureSupplyControlCommand = new RelayCommand(PressureSupplyControlSection);
            SetMccPanelShowControlPopUpExample = new RelayCommand(MccPanelShowControlPopUp);

            InitializePageTagConfigurations();

            // Timer timer = new Timer(new TimerCallback(CreateRandomNumber), null, 0, 50);


        }



        private void InitializePageTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("PipingAndInstrumentation");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            PipingAndInstrumentationTagConfigurations = JsonConvert.DeserializeObject<PipingAndInstrumentationTagConfigurations>(jsonSerializedString);

            PipingAndInstrumentationTagConfigurations.AirTcActualValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.AirTcActualValue)];
            PipingAndInstrumentationTagConfigurations.CirculationFanRunStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationFanRunStatus)];
            PipingAndInstrumentationTagConfigurations.CirculationFanSpeed = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationFanSpeed)];
            PipingAndInstrumentationTagConfigurations.CirculationFanSpeedAutoMan = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationFanSpeedAutoMan)];
            PipingAndInstrumentationTagConfigurations.CirculationFanSpeedOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationFanSpeedOnOff)];
            PipingAndInstrumentationTagConfigurations.KpPanelTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.KpPanelTemperature)];
            PipingAndInstrumentationTagConfigurations.MccPanelTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.MccPanelTemperature)];
            PipingAndInstrumentationTagConfigurations.CirculationFanTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationFanTemperature)];
            PipingAndInstrumentationTagConfigurations.CirculationFanVibration = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationFanVibration)];
            PipingAndInstrumentationTagConfigurations.CirculationWaterStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CirculationWaterStatus)];
            PipingAndInstrumentationTagConfigurations.CoolingProportionalValveValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingProportionalValveValue)];
            PipingAndInstrumentationTagConfigurations.CoolingProportionalValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingProportionalValveAutoManual)];
            PipingAndInstrumentationTagConfigurations.CoolingLinePressureSwitch = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLinePressureSwitch)];


            #region pressureInletOutletOnOffValve
            PipingAndInstrumentationTagConfigurations.PressureAirInletValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirInletValveStatus)];
            PipingAndInstrumentationTagConfigurations.PressureAirInletValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirInletValveAutoManual)];
            PipingAndInstrumentationTagConfigurations.PressureAirInletValveOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirInletValveOnOff)];

            PipingAndInstrumentationTagConfigurations.PressureAirOutletValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirOutletValveStatus)];
            PipingAndInstrumentationTagConfigurations.PressureAirOutletValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirOutletValveAutoManual)];
            PipingAndInstrumentationTagConfigurations.PressureAirOutletValveOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirOutletValveOnOff)];

            #endregion

            #region pressureInletOutletProportionalValve

            PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveStatus)]; 
            PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveValue)];
            PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveAutoManual)];

            PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveStatus)];
            PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveValue)];
            PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveAutoManual)];
            #endregion

            #region vacuumInletOutletProportionalValve
            PipingAndInstrumentationTagConfigurations.VacuumInletProportionalValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumInletProportionalValveStatus)];
            PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveValue)];
            PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveAutoManual)];

            PipingAndInstrumentationTagConfigurations.VacuumOutletValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumOutletValveStatus)];
            PipingAndInstrumentationTagConfigurations.VacuumOutletValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumOutletValveAutoManual)];
            PipingAndInstrumentationTagConfigurations.VacuumOutletValveOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumOutletValveOnOff)];
            #endregion vacuumInletOutletProportionalValve

            PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1Status = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1Status)];
            PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1AutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1AutoManual)];
            PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1OnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1OnOff)];
            PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2Status = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2Status)];
            PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2AutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2AutoManual)];
            PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2OnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2OnOff)];
            PipingAndInstrumentationTagConfigurations.DrainPumpStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.DrainPumpStatus)];
            PipingAndInstrumentationTagConfigurations.DrainPumpAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.DrainPumpAutoManual)];
            PipingAndInstrumentationTagConfigurations.DrainPumpOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.DrainPumpOnOff)];
            PipingAndInstrumentationTagConfigurations.DrainTankWaterLevel = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.DrainTankWaterLevel)];

            PipingAndInstrumentationTagConfigurations.VacuumPumpStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumPumpStatus)];
            PipingAndInstrumentationTagConfigurations.VacuumPumpAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumPumpAutoManual)];
            PipingAndInstrumentationTagConfigurations.VacuumPumpOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumPumpOnOff)];

            PipingAndInstrumentationTagConfigurations.VacuumPumpLeftStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumPumpLeftStatus)];
            PipingAndInstrumentationTagConfigurations.VacuumPumpLeftAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumPumpLeftAutoManual)];
            PipingAndInstrumentationTagConfigurations.VacuumPumpLeftOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumPumpLeftOnOff)];


            PipingAndInstrumentationTagConfigurations.VacuumLineMonitor = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumLineMonitor)];
            PipingAndInstrumentationTagConfigurations.VacuumTankActualValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacuumTankActualValue)];
            PipingAndInstrumentationTagConfigurations.PressureActualValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureActualValue)];
            PipingAndInstrumentationTagConfigurations.PressureSupportActualValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureSupportActualValue)];
            PipingAndInstrumentationTagConfigurations.LimitDeviceTemperature2 = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.LimitDeviceTemperature2)];
            PipingAndInstrumentationTagConfigurations.LimitDeviceTemperature1 = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.LimitDeviceTemperature1)];
            PipingAndInstrumentationTagConfigurations.ResinFilterWaterTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.ResinFilterWaterTemperature)];
            PipingAndInstrumentationTagConfigurations.HighMonPortNumber = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.HighMonPortNumber)];
            PipingAndInstrumentationTagConfigurations.HighMonValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.HighMonValue)];
            PipingAndInstrumentationTagConfigurations.LowMonPortNumber = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.LowMonPortNumber)];
            PipingAndInstrumentationTagConfigurations.LowMonValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.LowMonValue)];
            PipingAndInstrumentationTagConfigurations.VacSetPoint = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacSetPoint)];
            PipingAndInstrumentationTagConfigurations.VacActualValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.VacActualValue)];

            PipingAndInstrumentationTagConfigurations.HighPtcPortNumber = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.HighPtcPortNumber)];
            PipingAndInstrumentationTagConfigurations.HighPtcPortValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.HighPtcPortValue)];
            PipingAndInstrumentationTagConfigurations.LowPtcPortNumber = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.LowPtcPortNumber)];
            PipingAndInstrumentationTagConfigurations.LowPtcValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.LowPtcValue)];
            PipingAndInstrumentationTagConfigurations.AirTcSetPoint = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.AirTcSetPoint)];
            PipingAndInstrumentationTagConfigurations.TotalWorkingTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.TotalWorkingTime)];




            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 2:
                case 3:
                    #region P&I
                    PipingAndInstrumentationTagConfigurations.AirTCMediumL = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.AirTCMediumL)];
                    #endregion
                    break;
                case 20:

                    //PipingAndInstrumentationTagConfigurations.kpPanelStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.kpPanelStatus)];
                    //PipingAndInstrumentationTagConfigurations.mccPanelStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.kpPanelStatus)];
                    
                    PipingAndInstrumentationTagConfigurations.FurnacePressureSwitch1 = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.FurnacePressureSwitch1)];
                    PipingAndInstrumentationTagConfigurations.FurnacePressureSwitch2 = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.FurnacePressureSwitch2)];


                    PipingAndInstrumentationTagConfigurations.FurnaceRightHumanSensor = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.FurnaceRightHumanSensor)];
                    PipingAndInstrumentationTagConfigurations.FurnaceLeftHumanSensor = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.FurnaceLeftHumanSensor)];
                    
                    PipingAndInstrumentationTagConfigurations.DoorOpenSwStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.DoorOpenSwStatus)];
                    PipingAndInstrumentationTagConfigurations.DoorCloseSwStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.DoorCloseSwStatus)];
                    PipingAndInstrumentationTagConfigurations.RingOpenSwStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.RingOpenSwStatus)];
                    PipingAndInstrumentationTagConfigurations.RingCloseSwStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.RingCloseSwStatus)];
                    PipingAndInstrumentationTagConfigurations.GeneralPnomaticStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.GeneralPnomaticStatus)];
                    PipingAndInstrumentationTagConfigurations.HydrolicConfirmationStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.HydrolicConfirmationStatus)];



                    PipingAndInstrumentationTagConfigurations.CompressorValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CompressorValveStatus)];
                    PipingAndInstrumentationTagConfigurations.PressureSupplySelectionValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureSupplySelectionValveAutoManual)];
                    PipingAndInstrumentationTagConfigurations.pressureSupplySelectionValveControl = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.pressureSupplySelectionValveControl)];

                    PipingAndInstrumentationTagConfigurations.BoosterValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.BoosterValveStatus)];
                    PipingAndInstrumentationTagConfigurations.PressureCompressorOrBoosterControlValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureCompressorOrBoosterControlValue)];



                    PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveStatus)];
                    PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveAutoManual)];
                    PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveOnOff)];

                    PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveStatus)];
                    PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveAutoManual)];
                    PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveOnOff)];

                    PipingAndInstrumentationTagConfigurations.CoolingBypassValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingBypassValveStatus)];
                    PipingAndInstrumentationTagConfigurations.CoolingBypassValveAutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingBypassValveAutoManual)];
                    PipingAndInstrumentationTagConfigurations.CoolingBypassValveOnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingBypassValveOnOff)];

                    PipingAndInstrumentationTagConfigurations.TowerWaterTemperature = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.TowerWaterTemperature)];

                    PipingAndInstrumentationTagConfigurations.Heater2Status = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater2Status)];
                    PipingAndInstrumentationTagConfigurations.Heater2Value = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater2Value)];
                    PipingAndInstrumentationTagConfigurations.Heater2AutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater2AutoManual)];

                    PipingAndInstrumentationTagConfigurations.Heater4Status = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater4Status)];
                    PipingAndInstrumentationTagConfigurations.Heater4Value = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater4Value)];
                    PipingAndInstrumentationTagConfigurations.Heater4AutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater4AutoManual)];

                    PipingAndInstrumentationTagConfigurations.Heater1Status = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater1Status)];
                    PipingAndInstrumentationTagConfigurations.Heater1AutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater1AutoManual)];
                    PipingAndInstrumentationTagConfigurations.Heater1OnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater1OnOff)];

                    PipingAndInstrumentationTagConfigurations.Heater3Status = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater3Status)];
                    PipingAndInstrumentationTagConfigurations.Heater3AutoManual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater3AutoManual)];
                    PipingAndInstrumentationTagConfigurations.Heater3OnOff = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.Heater3OnOff)];

                    PipingAndInstrumentationTagConfigurations.TowerWaterLevelMax = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.TowerWaterLevelMax)];
                    PipingAndInstrumentationTagConfigurations.TowerWaterLevelMiddle = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.TowerWaterLevelMiddle)];
                    PipingAndInstrumentationTagConfigurations.TowerWaterLevelMin = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.TowerWaterLevelMin)];
                    PipingAndInstrumentationTagConfigurations.FanPanelAndRecineWaterCoolingPump = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.FanPanelAndRecineWaterCoolingPump)];
                    PipingAndInstrumentationTagConfigurations.MainCoolingMotorControl = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.MainCoolingMotorControl)];
                    PipingAndInstrumentationTagConfigurations.CoolingProportionalValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.CoolingProportionalValveStatus)];
                     //PipingAndInstrumentationTagConfigurations.PressureAirExhasustProportionalValveStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(PipingAndInstrumentationTagConfigurations.PressureAirExhasustProportionalValveStatus)];  //TODO : DataBlock:5 Ofsett:278,0 SqliteDb İnsert

                    break;
                default:
                    break;
            }
            //ContinuousUpdate();
        }

        public void ContinuousUpdate()
        {
            Guid guid = Guid.NewGuid();  //

            if (!AllowTimerRun)
                return;

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            PipingAndInstrumentationFurnaceControlModel.AirTcActualValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.AirTcActualValue, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingProportionalValveValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingProportionalValveValue, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingProportionalValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingProportionalValveAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.CirculationFanRunStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanRunStatus, false); ;
            PipingAndInstrumentationFurnaceControlModel.CirculationFanSpeed = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanSpeed, false); ;
            PipingAndInstrumentationFurnaceControlModel.CirculationFanSpeedAutoMan = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanSpeedAutoMan, false); ;

            PipingAndInstrumentationFurnaceControlModel.CirculationFanSpeedOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanSpeedOnOff, false); ;
            PipingAndInstrumentationFurnaceControlModel.CirculationWaterStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationWaterStatus, false); ;
            PipingAndInstrumentationFurnaceControlModel.PressureAirInletValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletValveStatus, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirInletValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletValveAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirInletValveOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletValveOnOff, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirOutletValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletValveStatus, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirOutletValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletValveAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirOutletValveOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletValveOnOff, false);

            PipingAndInstrumentationFurnaceControlModel.PressureAirInletProportionalValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveStatus, false); //TODO : DataBlock:5 Ofsett:276,0 SqliteDb İnsert
            PipingAndInstrumentationFurnaceControlModel.PressureAirInletProportionalValveValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveValue, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirInletProportionalValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveAutoManual, false);

            PipingAndInstrumentationFurnaceControlModel.PressureAirOutletProportionalValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveStatus, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirOutletProportionalValveValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveValue, false);
            PipingAndInstrumentationFurnaceControlModel.PressureAirOutletProportionalValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveAutoManual, false);

            PipingAndInstrumentationFurnaceControlModel.VacuumInletProportionalValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumInletProportionalValveStatus, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumIntelProportionalValveValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveValue, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumIntelProportionalValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveAutoManual, false);

            PipingAndInstrumentationFurnaceControlModel.VacuumOutletValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumOutletValveStatus, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumOutletValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumOutletValveAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumOutletValveOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumOutletValveOnOff, false);

            PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve1Status = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1Status, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve1AutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1AutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve1OnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1OnOff, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve2Status = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2Status, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve2AutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2AutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve2OnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2OnOff, false);
            PipingAndInstrumentationFurnaceControlModel.CoolingLinePressureSwitch = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLinePressureSwitch, false);

            PipingAndInstrumentationFurnaceControlModel.DrainPumpStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DrainPumpStatus, false);
            PipingAndInstrumentationFurnaceControlModel.DrainPumpAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DrainPumpAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.DrainPumpOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DrainPumpOnOff, false);

            PipingAndInstrumentationFurnaceControlModel.DrainTankWaterLevel = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DrainTankWaterLevel, false);

            PipingAndInstrumentationFurnaceControlModel.VacuumPumpStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpStatus, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumPumpAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumPumpOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpOnOff, false);

            PipingAndInstrumentationFurnaceControlModel.VacuumPumpLeftStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpLeftStatus, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumPumpLeftAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpLeftAutoManual, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumPumpLeftOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpLeftOnOff, false);


            PipingAndInstrumentationFurnaceControlModel.AirTcSetPoint = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.AirTcSetPoint, false);
            PipingAndInstrumentationFurnaceControlModel.VacSetPoint = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacSetPoint, false);

            PipingAndInstrumentationFurnaceControlModel.VacuumTankActualValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumTankActualValue, false);
            PipingAndInstrumentationFurnaceControlModel.PressureActualValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureActualValue, false);
            PipingAndInstrumentationFurnaceControlModel.PressureSupportActualValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureSupportActualValue, false);

            PipingAndInstrumentationFurnaceControlModel.KpPanelTemperature = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.KpPanelTemperature, false);
            PipingAndInstrumentationFurnaceControlModel.MccPanelTemperature = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.MccPanelTemperature, false);
            PipingAndInstrumentationFurnaceControlModel.CirculationFanVibration = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanVibration, false);
            PipingAndInstrumentationFurnaceControlModel.CirculationFanTemperature = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanTemperature, false);
            PipingAndInstrumentationFurnaceControlModel.VacuumLineMonitor = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumLineMonitor, false);
            PipingAndInstrumentationFurnaceControlModel.LimitDeviceTemperature2 = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.LimitDeviceTemperature2, false);
            PipingAndInstrumentationFurnaceControlModel.LimitDeviceTemperature1 = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.LimitDeviceTemperature1, false);
            PipingAndInstrumentationFurnaceControlModel.ResinFilterWaterTemperature = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.ResinFilterWaterTemperature, false);
            PipingAndInstrumentationFurnaceControlModel.HighMonPortNumber = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.HighMonPortNumber, false);
            PipingAndInstrumentationFurnaceControlModel.HighMonPortValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.HighMonValue, false);
            PipingAndInstrumentationFurnaceControlModel.LowMonPortNumber = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.LowMonPortNumber, false);
            PipingAndInstrumentationFurnaceControlModel.LowMonPortValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.LowMonValue, false);
            PipingAndInstrumentationFurnaceControlModel.VacActualValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacActualValue, false);

            PipingAndInstrumentationFurnaceControlModel.HighPtcPortNumber = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.HighPtcPortNumber, false);
            PipingAndInstrumentationFurnaceControlModel.HighPtcPortValue = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.HighPtcPortValue, false);
            PipingAndInstrumentationFurnaceControlModel.LowPtcPortNumber = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.LowPtcPortNumber, false);
            PipingAndInstrumentationFurnaceControlModel.LowPtcPortValue =  plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.LowPtcValue, false);
            PipingAndInstrumentationFurnaceControlModel.TotalWorkingTime =  plcCommandManager.Get<string>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.TotalWorkingTime, false);



            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 2:
                case 3:
                    break;
                case 20:

                    PipingAndInstrumentationFurnaceControlModel.KpPanelStatus = 1;// plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.kpPanelStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.MccPanelStatus = 0;// plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.mccPanelStatus, false);
                    
                    PipingAndInstrumentationFurnaceControlModel.FurnacePressureSwitch1 = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.FurnacePressureSwitch1, false);
                    PipingAndInstrumentationFurnaceControlModel.FurnacePressureSwitch2 = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.FurnacePressureSwitch2, false);


                    PipingAndInstrumentationFurnaceControlModel.FurnaceRightHumanSensor = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.FurnaceRightHumanSensor, false);
                    PipingAndInstrumentationFurnaceControlModel.FurnaceLeftHumanSensor = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.FurnaceLeftHumanSensor, false);
                                        
                    PipingAndInstrumentationFurnaceControlModel.DoorOpenSwStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DoorOpenSwStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.DoorCloseSwStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DoorCloseSwStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.RingOpenSwStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.RingOpenSwStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.RingCloseSwStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.RingCloseSwStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.GeneralPnomaticStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.GeneralPnomaticStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.HydrolicConfirmationStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.HydrolicConfirmationStatus, false);
                  
                    PipingAndInstrumentationFurnaceControlModel.CompressorValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CompressorValveStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.BoosterValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.BoosterValveStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.PressureCompressorOrBoosterControlValue = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureCompressorOrBoosterControlValue, false);

                    PipingAndInstrumentationFurnaceControlModel.PressureSupplySelectionValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureSupplySelectionValveAutoManual, false);
                    PipingAndInstrumentationFurnaceControlModel.PressureSupplySelectionValveControl = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.pressureSupplySelectionValveControl, false);

                    PipingAndInstrumentationFurnaceControlModel.CoolingTrimAirCoolingValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingTrimAirCoolingValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveAutoManual, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingTrimAirCoolingValveOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveOnOff, false);

                    PipingAndInstrumentationFurnaceControlModel.CoolingTrimWaterCoolingValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingTrimWaterCoolingValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveAutoManual, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingTrimWaterCoolingValveOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveOnOff, false);

                    PipingAndInstrumentationFurnaceControlModel.CoolingBypassValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingBypassValveStatus, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingBypassValveAutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingBypassValveAutoManual, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingBypassValveOnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingBypassValveOnOff, false);

                    PipingAndInstrumentationFurnaceControlModel.TowerWaterTemperature = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.TowerWaterTemperature, false);

                    PipingAndInstrumentationFurnaceControlModel.Heater2Status = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater2Status, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater2Value = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater2Value, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater2AutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater2AutoManual, false);

                    PipingAndInstrumentationFurnaceControlModel.Heater4Status = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater4Status, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater4Value = plcCommandManager.Get<float>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater4Value, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater4AutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater4AutoManual, false);

                    PipingAndInstrumentationFurnaceControlModel.Heater1Status = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater1Status, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater1AutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater1AutoManual, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater1OnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater1OnOff, false);

                    PipingAndInstrumentationFurnaceControlModel.Heater3Status = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater3Status, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater3AutoManual = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater3AutoManual, false);
                    PipingAndInstrumentationFurnaceControlModel.Heater3OnOff = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater3OnOff, false);

                    PipingAndInstrumentationFurnaceControlModel.FanPanelAndRecineWaterCoolingPump = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.FanPanelAndRecineWaterCoolingPump, false);
                    PipingAndInstrumentationFurnaceControlModel.MainCoolingMotorControl = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.MainCoolingMotorControl, false);
                    PipingAndInstrumentationFurnaceControlModel.TowerWaterLevelMax = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.TowerWaterLevelMax, false);
                    PipingAndInstrumentationFurnaceControlModel.TowerWaterLevelMiddle = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.TowerWaterLevelMiddle, false);
                    PipingAndInstrumentationFurnaceControlModel.TowerWaterLevelMin = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.TowerWaterLevelMin, false);
                    PipingAndInstrumentationFurnaceControlModel.CoolingProportionalValveStatus = plcCommandManager.Get<int>((SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingProportionalValveStatus, false);

                    break;
                default:
                    break;
            }
        }


        public async Task<bool> SetToPlc<T>(T value, string propertyName = null, string stateText=null)
        {
            ProcessEventLog pAndIEventLog = new ProcessEventLog();
            LastInvokedCommandTime = DateTime.Now;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            SiemensTagConfiguration siemensTagConfig = new SiemensTagConfiguration();
            string eventText = String.Empty;
            if (stateText == null)
                stateText = string.Empty;

            Guid guid = Guid.NewGuid();
            bool plcResult = false;

            switch (SenderPropertyName)
            {
                case "circulationFanBtn":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanSpeedAutoMan;
                            eventText = string.Format("P&I Circulation Fan state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanSpeedOnOff;
                            eventText = string.Format("P&I Circulation Fan {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CirculationFanSpeed;
                            eventText = string.Format("P&I Circulation Fan Value set to: {0:F2}.", value);
                            break;
                    }
                    break;
                case "ppv800":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveAutoManual;
                            eventText = string.Format("P&I PPV800 state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletProportionalValveValue;
                            eventText = string.Format("P&I PPV800 {0}.", stateText);
                            break;
                    }
                    break;
                case "pev900":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveAutoManual;
                            eventText = string.Format("P&I PEV900 state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletProportionalValveValue;
                            eventText = string.Format("P&I PEV900 {0}.", stateText);
                            break;
                    }
                    break;
                case "ppv400":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingProportionalValveAutoManual;
                            eventText = string.Format("P&I PPV400 state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingProportionalValveValue;
                            eventText = string.Format("P&I PPV400 {0}.", stateText);
                            break;
                    }
                    break;
                case "pkv100":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveAutoManual;
                            eventText = string.Format("P&I PKV100 state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumIntelProportionalValveValue;
                            eventText = string.Format("P&I PKV100 {0}.", stateText);
                            break;
                    }
                    break;
                case "pev800":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletValveAutoManual;
                            eventText = string.Format("P&I PEV800 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirInletValveOnOff;
                            eventText = string.Format("P&I PEV800 {0}.", stateText);
                            break;

                    }
                    break;
                case "pev802":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletValveAutoManual;
                            eventText = string.Format("P&I PEV802 state set to: {0}.", stateText);

                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureAirOutletValveOnOff;
                            eventText = string.Format("P&I PEV802 {0}.", stateText);
                            break;

                    }
                    break;
                case "sev100":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumOutletValveAutoManual;
                            eventText = string.Format("P&I SEV100 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumOutletValveOnOff;
                            eventText = string.Format("P&I SEV100 {0}.", stateText);
                            break;
                    }
                    break;
                case "sev401":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveAutoManual;
                            eventText = string.Format("P&I SEV401 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimAirCoolingValveOnOff;
                            eventText = string.Format("P&I SEV401 {0}.", stateText);
                            break;
                    }
                    break;
                case "sev400":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveAutoManual;
                            eventText = string.Format("P&I SEV400 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingTrimWaterCoolingValveOnOff;
                            eventText = string.Format("P&I SEV400 state set to: {0}.", stateText);
                            break;
                    }
                    break;
                case "pev501_1":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingBypassValveAutoManual;
                            eventText = string.Format("P&I PEV501_1 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingBypassValveOnOff;
                            eventText = string.Format("P&I PEV501_1 {0}.", stateText);
                            break;
                    }
                    break;
                case "pev401":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1AutoManual;
                            eventText = string.Format("P&I PEV401 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve1OnOff;
                            eventText = string.Format("P&I PEV401 {0}.", stateText);
                            break;
                    }
                    break;
                case "pev402":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2AutoManual;
                            eventText = string.Format("P&I PEV402 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.CoolingLineDrainValve2OnOff;
                            eventText = string.Format("P&I PEV402 {0}.", stateText);
                            break;
                    }
                    break;
                case "drain01":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DrainPumpAutoManual;
                            eventText = string.Format("P&I Drain Pump state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.DrainPumpOnOff;
                            eventText = string.Format("P&I Drain Pump {0}.", stateText);
                            break;
                    }
                    break;
                case "htr402":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater2AutoManual;
                            eventText = string.Format("P&I HTR402 state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater2Value;
                            eventText = string.Format("P&I HTR402 {0}.", stateText);
                            break;
                    }
                    break;
                case "htr404":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater4AutoManual;
                            eventText = string.Format("P&I HTR404 state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater4Value;
                            eventText = string.Format("P&I HTR404 {0}.", stateText);
                            break;
                    }
                    break;
                case "htr401":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater1AutoManual;
                            eventText = string.Format("P&I HTR401 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater1OnOff;
                            eventText = string.Format("P&I HTR401 {0}.", stateText);
                            break;
                    }
                    break;
                case "htr403":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater3AutoManual;
                            eventText = string.Format("P&I HTR403 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.Heater3OnOff;
                            eventText = string.Format("P&I HTR403 {0}.", stateText);
                            break;
                    }
                    break;
                case "vp100":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpAutoManual;
                            eventText = string.Format("P&I VP100 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpOnOff;
                            eventText = string.Format("P&I VP100 {0}.", stateText);
                            break;
                    }
                    break;
                case "pev501_3":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpLeftAutoManual;
                            eventText = string.Format("P&I PEV501_3 state set to: {0}.", stateText);
                            break;
                        case "RbOn":
                        case "RbOff":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.VacuumPumpLeftOnOff;
                            eventText = string.Format("P&I PEV501_3 {0}.", stateText);
                            break;
                    }
                    break;

                case "compressorValve":
                case "boosterValve":
                    switch (propertyName)
                    {
                        case "RbManual":
                        case "RbAuto":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureSupplySelectionValveAutoManual;
                            eventText = string.Format("P&I Compressor Or Booster state set to: {0}.", stateText);
                            break;
                        case "RbCompressor":
                        case "RbBooster":
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.pressureSupplySelectionValveControl;
                            eventText = string.Format("P&I Compressor Or Booster state set to: {0}.", stateText);
                            break;
                        default:
                            siemensTagConfig = (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.PressureCompressorOrBoosterControlValue;
                            eventText = string.Format("P&I Compressor Or Booster Value set to: {0:F2}.", value);
                            break;
                    }
                    break;
                default:
                    break;
            }

            _plcCommandManager.Set(siemensTagConfig, value, guid);

            plcResult = await _plcCommandManager.IsUpdatedResultAsync(guid, false);

            if (ProcessManager.Instance.CurrentProcess != null)
                pAndIEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

            pAndIEventLog.CreateDate = DateTime.Now;
            pAndIEventLog.Type = ProcessEventLogType.Manual.ToString();

            pAndIEventLog.EventText = eventText;
            _processEventLogService.Insert(pAndIEventLog);

            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
            processEventLogAdapter.CreateProcessEventLogSyncIssue(pAndIEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);

            WaitIndicatorControl.IsWaitIndicatorVisible = false;
            return plcResult;
        }

        private void FurnaceControlAutoMan()
        {
            float textEditValue = 0;
            int radioButtonAutoMan = 0;
            int radioButtonOnOff = 0;
            switch (SenderPropertyName)
            {
                case "circulationFanBtn":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.CirculationFanSpeed;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CirculationFanSpeedAutoMan;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.CirculationFanSpeedOnOff;

                    break;
                default:
                    break;
            }

            FurnaceControl furnaceControl = new FurnaceControl(this, textEditValue, radioButtonAutoMan, radioButtonOnOff);
            furnaceControl.ShowDialog();
        }

        private void ValveControl()
        {
            float textEditValue = 0;
            int radioButtonAutoMan = 0;
            int radioButtonOnOff = 0;
            switch (SenderPropertyName)
            {
                case "ppv400":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.CoolingProportionalValveValue;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CoolingProportionalValveAutoManual;
                    break;
                case "ppv800":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.PressureAirInletProportionalValveValue;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.PressureAirInletProportionalValveAutoManual;
                    break;
                case "pev900":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.PressureAirOutletProportionalValveValue;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.PressureAirOutletProportionalValveAutoManual;
                    break;
                case "pkv100":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.VacuumIntelProportionalValveValue;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.VacuumIntelProportionalValveAutoManual;
                    break;
                case "htr402":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.Heater2Value;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.Heater2AutoManual;
                    break;
                case "htr404":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.Heater4Value;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.Heater4AutoManual;
                    break;
                default:
                    break;
            }

            ValveControlAutoManual valveControlAutoManual = new ValveControlAutoManual(this, textEditValue, radioButtonAutoMan, radioButtonOnOff);
            valveControlAutoManual.ShowDialog();
        }

        private void ValveControlOnOff()
        {
            int radioButtonAutoMan = 0;
            int radioButtonOnOff = 0;
            switch (SenderPropertyName)
            {
                case "pev800":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.PressureAirInletValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.PressureAirInletValveOnOff;
                    break;

                case "pev802":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.PressureAirOutletValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.PressureAirOutletValveOnOff;
                    break;
                case "sev100":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.VacuumOutletValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.VacuumOutletValveOnOff;
                    break;
                case "sev401":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CoolingTrimAirCoolingValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.CoolingTrimAirCoolingValveOnOff;
                    break;
                case "sev400":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CoolingTrimWaterCoolingValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.CoolingTrimWaterCoolingValveOnOff;
                    break;
                case "pev501_1":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CoolingBypassValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.CoolingBypassValveOnOff;
                    break;
                case "pev401":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve1AutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve1OnOff;
                    break;
                case "pev402":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve2AutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.CoolingLineDrainValve2OnOff;
                    break;
                case "drain01":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.DrainPumpAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.DrainPumpOnOff;
                    break;
                case "htr403":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.Heater3AutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.Heater3OnOff;
                    break;
                case "htr401":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.Heater1AutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.Heater1OnOff;
                    break;
                case "vp100":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.VacuumPumpAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.VacuumPumpOnOff;
                    break;
                case "pev501_3":
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.VacuumPumpLeftAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.VacuumPumpLeftOnOff;
                    break;
                default:
                    break;
            }

            ValveControlOnOff valveControlOnOff = new ValveControlOnOff(this, radioButtonAutoMan, radioButtonOnOff);
            valveControlOnOff.ShowDialog();
        }
        private void PressureSupplyControlSection()
        {
            float textEditValue = 0;
            int radioButtonAutoMan = 0;
            int radioButtonOnOff = 0;
            switch (SenderPropertyName)
            {
                case "compressorValve":
                case "boosterValve":
                    textEditValue = PipingAndInstrumentationFurnaceControlModel.PressureCompressorOrBoosterControlValue;
                    radioButtonAutoMan = PipingAndInstrumentationFurnaceControlModel.PressureSupplySelectionValveAutoManual;
                    radioButtonOnOff = PipingAndInstrumentationFurnaceControlModel.PressureSupplySelectionValveControl;
                    break;
                default:

                    break;
            }

            PressureSupplyControl pressureSupplyControl = new PressureSupplyControl(this, radioButtonAutoMan, radioButtonOnOff, textEditValue);
            pressureSupplyControl.ShowDialog();
        }

        private void MccPanelShowControlPopUp()
        {

            float mccPanelLastValue = PipingAndInstrumentationFurnaceControlModel.MccPanelTemperature;
            PNI_Full_Screen_Text_Edit pniFullScreenTextEdit = new PNI_Full_Screen_Text_Edit(true, mccPanelLastValue, this, (SiemensTagConfiguration)PipingAndInstrumentationTagConfigurations.MccPanelTemperature);
            pniFullScreenTextEdit.ShowDialog();

        }

        private void SetAlarm()
        {
            PNI_Full_Screen_Set_Alarm PNIFullScreenSetAlarm = new PNI_Full_Screen_Set_Alarm();
            PNIFullScreenSetAlarm.ShowDialog();
        }

    }
}