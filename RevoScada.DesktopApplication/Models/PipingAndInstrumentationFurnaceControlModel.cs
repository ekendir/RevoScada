using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class PipingAndInstrumentationFurnaceControlModel : ObservableObject
    {

        private float _airTcHigh;
        public float AirTcHigh
        {
            get => _airTcHigh;
            set => OnPropertyChanged(ref _airTcHigh, value);
        }

        private float _airTcLow;
        public float AirTcLow
        {
            get => _airTcLow;
            set => OnPropertyChanged(ref _airTcLow, value);
        }

        private float _pressureAirExhaustProportionalValveValue;
        public float PressureAirExhaustProportionalValveValue
        {
            get => _pressureAirExhaustProportionalValveValue;
            set => OnPropertyChanged(ref _pressureAirExhaustProportionalValveValue, value);
        }

        private float _limitDeviceTemperature1;
        public float LimitDeviceTemperature1
        {
            get => _limitDeviceTemperature1;
            set => OnPropertyChanged(ref _limitDeviceTemperature1, value);
        }

        private int _towerWaterLevelMax;
        public int TowerWaterLevelMax
        {
            get => _towerWaterLevelMax;
            set => OnPropertyChanged(ref _towerWaterLevelMax, value);
        }
        private int _towerWaterLevelMiddle;
        public int TowerWaterLevelMiddle
        {
            get => _towerWaterLevelMiddle;
            set => OnPropertyChanged(ref _towerWaterLevelMiddle, value);
        }
        private int _towerWaterLevelMin;
        public int TowerWaterLevelMin
        {
            get => _towerWaterLevelMin;
            set => OnPropertyChanged(ref _towerWaterLevelMin, value);
        }

        private float _towerWaterTemperature;
        public float TowerWaterTemperature
        {
            get => _towerWaterTemperature;
            set => OnPropertyChanged(ref _towerWaterTemperature, value);
        }



        private int _fanPanelAndRecineWaterCoolingPump;
        public int FanPanelAndRecineWaterCoolingPump
        {
            get => _fanPanelAndRecineWaterCoolingPump;
            set => OnPropertyChanged(ref _fanPanelAndRecineWaterCoolingPump, value);
        }
        private float _kpPanelTemperature;
        public float KpPanelTemperature
        {
            get => _kpPanelTemperature;
            set => OnPropertyChanged(ref _kpPanelTemperature, value);
        }
        private float _mccPanelTemperature;
        public float MccPanelTemperature
        {
            get => _mccPanelTemperature;
            set => OnPropertyChanged(ref _mccPanelTemperature, value);
        }


        private int _mainCoolingMotorControl;
        public int MainCoolingMotorControl
        {
            get => _mainCoolingMotorControl;
            set => OnPropertyChanged(ref _mainCoolingMotorControl, value);
        }

        private int _drainPumpStatus;
        public int DrainPumpStatus
        {
            get => _drainPumpStatus;
            set => OnPropertyChanged(ref _drainPumpStatus, value);
        }
        private int _drainPumpAutoManual;
        public int DrainPumpAutoManual
        {
            get => _drainPumpAutoManual;
            set => OnPropertyChanged(ref _drainPumpAutoManual, value);
        }
        private int _drainPumpOnOff;
        public int DrainPumpOnOff
        {
            get => _drainPumpOnOff;
            set => OnPropertyChanged(ref _drainPumpOnOff, value);
        }

        private int _coolingBypassValveStatus;
        public int CoolingBypassValveStatus
        {
            get => _coolingBypassValveStatus;
            set => OnPropertyChanged(ref _coolingBypassValveStatus, value);
        }

        private int _coolingBypassValveAutoManual;
        public int CoolingBypassValveAutoManual
        {
            get => _coolingBypassValveAutoManual;
            set => OnPropertyChanged(ref _coolingBypassValveAutoManual, value);
        }

        private int _coolingBypassValveOnOff;
        public int CoolingBypassValveOnOff
        {
            get => _coolingBypassValveOnOff;
            set => OnPropertyChanged(ref _coolingBypassValveOnOff, value);
        }

        private int _coolingProportionalValveStatus;
        public int CoolingProportionalValveStatus
        {
            get => _coolingProportionalValveStatus;
            set => OnPropertyChanged(ref _coolingProportionalValveStatus, value);
        }

        private int _coolingTrimAirCoolingValveStatus;
        public int CoolingTrimAirCoolingValveStatus
        {
            get => _coolingTrimAirCoolingValveStatus;
            set => OnPropertyChanged(ref _coolingTrimAirCoolingValveStatus, value);
        }

        private int _coolingTrimAirCoolingValveAutoManual;
        public int CoolingTrimAirCoolingValveAutoManual
        {
            get => _coolingTrimAirCoolingValveAutoManual;
            set => OnPropertyChanged(ref _coolingTrimAirCoolingValveAutoManual, value);
        }

        private int _coolingTrimAirCoolingValveOnOff;
        public int CoolingTrimAirCoolingValveOnOff
        {
            get => _coolingTrimAirCoolingValveOnOff;
            set => OnPropertyChanged(ref _coolingTrimAirCoolingValveOnOff, value);
        }

        private int _coolingTrimWaterCoolingValveStatus;
        public int CoolingTrimWaterCoolingValveStatus
        {
            get => _coolingTrimWaterCoolingValveStatus;
            set => OnPropertyChanged(ref _coolingTrimWaterCoolingValveStatus, value);
        }

        private int _coolingTrimWaterCoolingValveAutoManual;
        public int CoolingTrimWaterCoolingValveAutoManual
        {
            get => _coolingTrimWaterCoolingValveAutoManual;
            set => OnPropertyChanged(ref _coolingTrimWaterCoolingValveAutoManual, value);
        }

        private int _coolingTrimWaterCoolingValveOnOff;
        public int CoolingTrimWaterCoolingValveOnOff
        {
            get => _coolingTrimWaterCoolingValveOnOff;
            set => OnPropertyChanged(ref _coolingTrimWaterCoolingValveOnOff, value);
        }

        private int _coolingLineDrainValve1Status;
        public int CoolingLineDrainValve1Status
        {
            get => _coolingLineDrainValve1Status;
            set => OnPropertyChanged(ref _coolingLineDrainValve1Status, value);
        }
        private int _coolingLineDrainValve1AutoManual;
        public int CoolingLineDrainValve1AutoManual
        {
            get => _coolingLineDrainValve1AutoManual;
            set => OnPropertyChanged(ref _coolingLineDrainValve1AutoManual, value);
        }
        private int _coolingLineDrainValve1OnOff;
        public int CoolingLineDrainValve1OnOff
        {
            get => _coolingLineDrainValve1OnOff;
            set => OnPropertyChanged(ref _coolingLineDrainValve1OnOff, value);
        }

        private int _coolingLineDrainValve2Status;
        public int CoolingLineDrainValve2Status
        {
            get => _coolingLineDrainValve2Status;
            set => OnPropertyChanged(ref _coolingLineDrainValve2Status, value);
        }
        private int _coolingLineDrainValve2AutoManual;
        public int CoolingLineDrainValve2AutoManual
        {
            get => _coolingLineDrainValve2AutoManual;
            set => OnPropertyChanged(ref _coolingLineDrainValve2AutoManual, value);
        }
        private int _coolingLineDrainValve2OnOff;
        public int CoolingLineDrainValve2OnOff
        {
            get => _coolingLineDrainValve2OnOff;
            set => OnPropertyChanged(ref _coolingLineDrainValve2OnOff, value);
        }

        private int _drainTankWaterLevel;
        public int DrainTankWaterLevel
        {
            get => _drainTankWaterLevel;
            set => OnPropertyChanged(ref _drainTankWaterLevel, value);
        }

        private int _vacuumPumpStatus;
        public int VacuumPumpStatus
        {
            get => _vacuumPumpStatus;
            set => OnPropertyChanged(ref _vacuumPumpStatus, value);
        }
        private int _vacuumPumpOnOff;
        public int VacuumPumpOnOff
        {
            get => _vacuumPumpOnOff;
            set => OnPropertyChanged(ref _vacuumPumpOnOff, value);
        }

        private int _vacuumPumpAutoManual;
        public int VacuumPumpAutoManual
        {
            get => _vacuumPumpAutoManual;
            set => OnPropertyChanged(ref _vacuumPumpAutoManual, value);
        }


        private float _vacuumIntelProportionalValveValue;
        public float VacuumIntelProportionalValveValue
        {
            get => _vacuumIntelProportionalValveValue;
            set => OnPropertyChanged(ref _vacuumIntelProportionalValveValue, value);
        }

        private int _vacuumIntelProportionalValveAutoManual;
        public int VacuumIntelProportionalValveAutoManual
        {
            get => _vacuumIntelProportionalValveAutoManual;
            set => OnPropertyChanged(ref _vacuumIntelProportionalValveAutoManual, value);
        }

        private int _vacuumInletProportionalValveStatus;
        public int VacuumInletProportionalValveStatus
        {
            get => _vacuumInletProportionalValveStatus;
            set => OnPropertyChanged(ref _vacuumInletProportionalValveStatus, value);
        }

        private int _vacuumPumpLeftStatus;
        public int VacuumPumpLeftStatus
        {
            get => _vacuumPumpLeftStatus;
            set => OnPropertyChanged(ref _vacuumPumpLeftStatus, value);
        }
        private int _vacuumPumpLeftAutoManual;
        public int VacuumPumpLeftAutoManual
        {
            get => _vacuumPumpLeftAutoManual;
            set => OnPropertyChanged(ref _vacuumPumpLeftAutoManual, value);
        }
        private int _vacuumPumpLeftOnOff;
        public int VacuumPumpLeftOnOff
        {
            get => _vacuumPumpLeftOnOff;
            set => OnPropertyChanged(ref _vacuumPumpLeftOnOff, value);
        }





        #region pressureInletOutletOnOffValve

        private int _pressureAirInletValveStatus;
        public int PressureAirInletValveStatus
        {
            get => _pressureAirInletValveStatus;
            set => OnPropertyChanged(ref _pressureAirInletValveStatus, value);
        }

        private int _pressureAirInletValveAutoManual;
        public int PressureAirInletValveAutoManual
        {
            get => _pressureAirInletValveAutoManual;
            set => OnPropertyChanged(ref _pressureAirInletValveAutoManual, value);
        }

        private int _pressureAirInletValveOnOff;
        public int PressureAirInletValveOnOff
        {
            get => _pressureAirInletValveOnOff;
            set => OnPropertyChanged(ref _pressureAirInletValveOnOff, value);
        }

        private int _pressureAirOutletValveStatus;
        public int PressureAirOutletValveStatus
        {
            get => _pressureAirOutletValveStatus;
            set => OnPropertyChanged(ref _pressureAirOutletValveStatus, value);
        }

        private int _pressureAirOutletValveAutoManual;
        public int PressureAirOutletValveAutoManual
        {
            get => _pressureAirOutletValveAutoManual;
            set => OnPropertyChanged(ref _pressureAirOutletValveAutoManual, value);
        }

        private int _pressureAirOutletValveOnOff;
        public int PressureAirOutletValveOnOff
        {
            get => _pressureAirOutletValveOnOff;
            set => OnPropertyChanged(ref _pressureAirOutletValveOnOff, value);
        }

        #endregion

        #region pressureInletOutletProportionalValve
        private int _pressureAirInletProportionalValveStatus;
        public int PressureAirInletProportionalValveStatus
        {
            get => _pressureAirInletProportionalValveStatus;
            set => OnPropertyChanged(ref _pressureAirInletProportionalValveStatus, value);
        }

        private float _pressureAirInletProportionalValveValue;
        public float PressureAirInletProportionalValveValue
        {
            get => _pressureAirInletProportionalValveValue;
            set => OnPropertyChanged(ref _pressureAirInletProportionalValveValue, value);
        }

        private int _pressureAirInletProportionalValveAutoManual;
        public int PressureAirInletProportionalValveAutoManual
        {
            get => _pressureAirInletProportionalValveAutoManual;
            set => OnPropertyChanged(ref _pressureAirInletProportionalValveAutoManual, value);
        }


        private float _pressureAirOutletProportionalValveStatus;
        public float PressureAirOutletProportionalValveStatus
        {
            get => _pressureAirOutletProportionalValveStatus;
            set => OnPropertyChanged(ref _pressureAirOutletProportionalValveStatus, value);
        }

        private float _pressureAirOutletProportionalValveValue;
        public float PressureAirOutletProportionalValveValue
        {
            get => _pressureAirOutletProportionalValveValue;
            set => OnPropertyChanged(ref _pressureAirOutletProportionalValveValue, value);
        }

        private int _pressureAirOutletProportionalValveAutoManual;
        public int PressureAirOutletProportionalValveAutoManual
        {
            get => _pressureAirOutletProportionalValveAutoManual;
            set => OnPropertyChanged(ref _pressureAirOutletProportionalValveAutoManual, value);
        }
        #endregion

        #region vacuumOutletValve
        private int _vacuumOutletValveStatus;
        public int VacuumOutletValveStatus
        {
            get => _vacuumOutletValveStatus;
            set => OnPropertyChanged(ref _vacuumOutletValveStatus, value);
        }

        private int _vacuumOutletValveAutoManual;
        public int VacuumOutletValveAutoManual
        {
            get => _vacuumOutletValveAutoManual;
            set => OnPropertyChanged(ref _vacuumOutletValveAutoManual, value);
        }

        private int _vacuumOutletValveOnOff;
        public int VacuumOutletValveOnOff
        {
            get => _vacuumOutletValveOnOff;
            set => OnPropertyChanged(ref _vacuumOutletValveOnOff, value);
        }
        #endregion



        private float _vacuumLineMonitor;
        public float VacuumLineMonitor
        {
            get => _vacuumLineMonitor;
            set => OnPropertyChanged(ref _vacuumLineMonitor, value);
        }

        private float _limitDeviceTemperature2;
        public float LimitDeviceTemperature2
        {
            get => _limitDeviceTemperature2;
            set => OnPropertyChanged(ref _limitDeviceTemperature2, value);
        }

        private float _circulationFanSpeed;
        public float CirculationFanSpeed
        {
            get => _circulationFanSpeed;
            set => OnPropertyChanged(ref _circulationFanSpeed, value);
        }
        private int _circulationFanSpeedAutoMan;
        public int CirculationFanSpeedAutoMan
        {
            get => _circulationFanSpeedAutoMan;
            set => OnPropertyChanged(ref _circulationFanSpeedAutoMan, value);
        }
        private int _circulationFanSpeedOnOff;
        public int CirculationFanSpeedOnOff
        {
            get => _circulationFanSpeedOnOff;
            set => OnPropertyChanged(ref _circulationFanSpeedOnOff, value);
        }   
        private int _circulationWaterStatus;
        public int CirculationWaterStatus
        {
            get => _circulationWaterStatus;
            set => OnPropertyChanged(ref _circulationWaterStatus, value);
        }


        private float _vacuumTankActualValue;
        public float VacuumTankActualValue
        {
            get => _vacuumTankActualValue;
            set => OnPropertyChanged(ref _vacuumTankActualValue, value);
        }

        private float _coolingProportionalValveValue;
        public float CoolingProportionalValveValue
        {
            get => _coolingProportionalValveValue;
            set => OnPropertyChanged(ref _coolingProportionalValveValue, value);
        }

        private int _coolingProportionalValveAutoManual;
        public int CoolingProportionalValveAutoManual
        {
            get => _coolingProportionalValveAutoManual;
            set => OnPropertyChanged(ref _coolingProportionalValveAutoManual, value);
        }


        private int _heater1Status;
        public int Heater1Status
        {
            get => _heater1Status;
            set => OnPropertyChanged(ref _heater1Status, value);
        }
        private int _heater1AutoManual;
        public int Heater1AutoManual
        {
            get => _heater1AutoManual;
            set => OnPropertyChanged(ref _heater1AutoManual, value);
        }
        private int _heater1OnOff;
        public int Heater1OnOff
        {
            get => _heater1OnOff;
            set => OnPropertyChanged(ref _heater1OnOff, value);
        }

        private int _heater3Status;
        public int Heater3Status
        {
            get => _heater3Status;
            set => OnPropertyChanged(ref _heater3Status, value);
        }
        private int _heater3AutoManual;
        public int Heater3AutoManual
        {
            get => _heater3AutoManual;
            set => OnPropertyChanged(ref _heater3AutoManual, value);
        }
        private int _heater3OnOff;
        public int Heater3OnOff
        {
            get => _heater3OnOff;
            set => OnPropertyChanged(ref _heater3OnOff, value);
        }

        private int _heater2Status;
        public int Heater2Status
        {
            get => _heater2Status;
            set => OnPropertyChanged(ref _heater2Status, value);
        }

        private float _heater2Value;
        public float Heater2Value
        {
            get => _heater2Value;
            set => OnPropertyChanged(ref _heater2Value, value);
        }

        private int _heater2AutoManual;
        public int Heater2AutoManual
        {
            get => _heater2AutoManual;
            set => OnPropertyChanged(ref _heater2AutoManual, value);
        }

        private int _heater4Status;
        public int Heater4Status
        {
            get => _heater4Status;
            set => OnPropertyChanged(ref _heater4Status, value);
        }

        private float _heater4Value;
        public float Heater4Value
        {
            get => _heater4Value;
            set => OnPropertyChanged(ref _heater4Value, value);
        }

        private int _heater4AutoManual;
        public int Heater4AutoManual
        {
            get => _heater4AutoManual;
            set => OnPropertyChanged(ref _heater4AutoManual, value);
        }




        private int _circulationFanRunStatus;
        public int CirculationFanRunStatus
        {
            get => _circulationFanRunStatus;
            set => OnPropertyChanged(ref _circulationFanRunStatus, value);
        }



        private float _circulationFanTemperature;
        public float CirculationFanTemperature
        {
            get => _circulationFanTemperature;
            set => OnPropertyChanged(ref _circulationFanTemperature, value);
        }

        private float _circulationFanVibration;
        public float CirculationFanVibration
        {
            get => _circulationFanVibration;
            set => OnPropertyChanged(ref _circulationFanVibration, value);
        }

        private float _resinFilterWaterTemperature;
        public float ResinFilterWaterTemperature
        {
            get => _resinFilterWaterTemperature;
            set => OnPropertyChanged(ref _resinFilterWaterTemperature, value);
        }


        private float _highMonPortNumber;
        public float HighMonPortNumber
        {
            get => _highMonPortNumber;
            set => OnPropertyChanged(ref _highMonPortNumber, value);
        }

        private float _highMonPortValue;
        public float HighMonPortValue
        {
            get => _highMonPortValue;
            set => OnPropertyChanged(ref _highMonPortValue, value);
        }

        private float _lowMonPortNumber;
        public float LowMonPortNumber
        {
            get => _lowMonPortNumber;
            set => OnPropertyChanged(ref _lowMonPortNumber, value);
        }

        private float _lowMonPortValue;
        public float LowMonPortValue
        {
            get => _lowMonPortValue;
            set => OnPropertyChanged(ref _lowMonPortValue, value);
        }



        private float _vacSetPoint;
        public float VacSetPoint
        {
            get => _vacSetPoint;
            set => OnPropertyChanged(ref _vacSetPoint, value);
        }

        private float _vacActualValue;
        public float VacActualValue
        {
            get => _vacActualValue;
            set => OnPropertyChanged(ref _vacActualValue, value);
        }



        private float _highPtcPortNumber;
        public float HighPtcPortNumber
        {
            get => _highPtcPortNumber;
            set => OnPropertyChanged(ref _highPtcPortNumber, value);
        }

        private float _highPtcPortValue;
        public float HighPtcPortValue
        {
            get => _highPtcPortValue;
            set => OnPropertyChanged(ref _highPtcPortValue, value);
        }

        private float _lowPtcPortNumber;
        public float LowPtcPortNumber
        {
            get => _lowPtcPortNumber;
            set => OnPropertyChanged(ref _lowPtcPortNumber, value);
        }

        private float _lowPtcPortValue;
        public float LowPtcPortValue
        {
            get => _lowPtcPortValue;
            set => OnPropertyChanged(ref _lowPtcPortValue, value);
        }


        private float _airTcSetPoint;
        public float AirTcSetPoint
        {
            get => _airTcSetPoint;
            set => OnPropertyChanged(ref _airTcSetPoint, value);
        }

        private float _airTcActualValue;
        public float AirTcActualValue
        {
            get => _airTcActualValue;
            set => OnPropertyChanged(ref _airTcActualValue, value);
        }



        private float _pressureActualValue;
        public float PressureActualValue
        {
            get => _pressureActualValue;
            set => OnPropertyChanged(ref _pressureActualValue, value);
        }

        private float _pressureSupportActualValue;
        public float PressureSupportActualValue
        {
            get => _pressureSupportActualValue;
            set => OnPropertyChanged(ref _pressureSupportActualValue, value);
        }
        
        private float _pressureCompressorOrBoosterControlValue;
        public float PressureCompressorOrBoosterControlValue
        {
            get => _pressureCompressorOrBoosterControlValue;
            set => OnPropertyChanged(ref _pressureCompressorOrBoosterControlValue, value);
        }

        private int _compressorValveStatus;
        public int CompressorValveStatus
        {
            get => _compressorValveStatus;
            set => OnPropertyChanged(ref _compressorValveStatus, value);
        }

        private int _boosterValveStatus;
        public int BoosterValveStatus
        {
            get => _boosterValveStatus;
            set => OnPropertyChanged(ref _boosterValveStatus, value);
        }


        private int _pressureSupplySelectionValveAutoManual;
        public int PressureSupplySelectionValveAutoManual
        {
            get => _pressureSupplySelectionValveAutoManual;
            set => OnPropertyChanged(ref _pressureSupplySelectionValveAutoManual, value);
        }

        private int _pressureSupplySelectionValveControl;
        public int PressureSupplySelectionValveControl
        {
            get => _pressureSupplySelectionValveControl;
            set => OnPropertyChanged(ref _pressureSupplySelectionValveControl, value);
        }
        private int _furnaceRightHumanSensor;
        public int FurnaceRightHumanSensor
        {
            get => _furnaceRightHumanSensor;
            set => OnPropertyChanged(ref _furnaceRightHumanSensor, value);
        }
        private int _furnaceLeftHumanSensor;
        public int FurnaceLeftHumanSensor
        {
            get => _furnaceLeftHumanSensor;
            set => OnPropertyChanged(ref _furnaceLeftHumanSensor, value);
        }
        private int _doorOpenSwStatus;
        public int DoorOpenSwStatus
        {
            get => _doorOpenSwStatus;
            set => OnPropertyChanged(ref _doorOpenSwStatus, value);
        }
        private int _doorCloseSwStatus;
        public int DoorCloseSwStatus
        {
            get => _doorCloseSwStatus;
            set => OnPropertyChanged(ref _doorCloseSwStatus, value);
        }
        private int _ringOpenSwStatus;
        public int RingOpenSwStatus
        {
            get => _ringOpenSwStatus;
            set => OnPropertyChanged(ref _ringOpenSwStatus, value);
        }
        private int _ringCloseSwStatus;
        public int RingCloseSwStatus
        {
            get => _ringCloseSwStatus;
            set => OnPropertyChanged(ref _ringCloseSwStatus, value);
        }
        private int _generalPnomaticStatus;
        public int GeneralPnomaticStatus
        {
            get => _generalPnomaticStatus;
            set => OnPropertyChanged(ref _generalPnomaticStatus, value);
        }
        private int _hydrolicConfirmationStatus;
        public int HydrolicConfirmationStatus
        {
            get => _hydrolicConfirmationStatus;
            set => OnPropertyChanged(ref _hydrolicConfirmationStatus, value);
        }

        private int _furnacePressureSwitch1;
        public int FurnacePressureSwitch1
        {
            get => _furnacePressureSwitch1;
            set => OnPropertyChanged(ref _furnacePressureSwitch1, value);
        }

        private int _furnacePressureSwitch2;
        public int FurnacePressureSwitch2
        {
            get => _furnacePressureSwitch2;
            set => OnPropertyChanged(ref _furnacePressureSwitch2, value);
        }

        private string _totalWorkingTime;
        public string TotalWorkingTime
        {
            get => _totalWorkingTime;
            set => OnPropertyChanged(ref _totalWorkingTime, value);
        }

        private int _coolingLinePressureSwitch;
        public int CoolingLinePressureSwitch
        {
            get => _coolingLinePressureSwitch;
            set => OnPropertyChanged(ref _coolingLinePressureSwitch, value);
        }

        private int _kpPanelStatus;
        public int KpPanelStatus
        {
            get => _kpPanelStatus;
            set => OnPropertyChanged(ref _kpPanelStatus, value);
        }

        private int _mccPanelStatus;
        public int MccPanelStatus
        {
            get => _mccPanelStatus;
            set => OnPropertyChanged(ref _mccPanelStatus, value);
        }
    }
}


         
         
 
 
 
 
 
 