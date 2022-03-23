using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class PipingAndInstrumentationTagConfigurations
    {
        public object PressureActualValue { get; set; }
        public object PressureSupportActualValue { get; set; }
        public object VacuumLineMonitor { get; set; }
        public object VacuumPumpStatus { get; set; }
        public object VacuumPumpAutoManual { get; set; }
        public object VacuumPumpOnOff { get; set; }

        public object VacuumPumpLeftStatus { get; set; }
        public object VacuumPumpLeftAutoManual { get; set; }
        public object VacuumPumpLeftOnOff { get; set; }
        public object DrainPumpStatus { get; set; }
        public object DrainPumpAutoManual { get; set; }
        public object DrainPumpOnOff { get; set; }
        public object DrainTankWaterLevel { get; set; }
        public object CoolingProportionalValveValue { get; set; }
        public object CoolingProportionalValveAutoManual { get; set; }
        public object CoolingProportionalValveStatus { get; set; }
        public object LimitDeviceTemperature1 { get; set; }
        public object LimitDeviceTemperature2 { get; set; }
        public object KpPanelTemperature { get; set; }
        public object MccPanelTemperature { get; set; }
        public object kpPanelStatus { get; set; }
        public object mccPanelStatus { get; set; }
        public object BatchTotalKwStatus { get; set; }
        public object CirculationFanRunStatus { get; set; }
        public object CirculationFanSpeed { get; set; }
        public object CirculationFanSpeedAutoMan { get; set; }
        public object CirculationFanSpeedOnOff { get; set; }
        public object CirculationFanTemperature { get; set; }
        public object CirculationFanVibration { get; set; }
        public object CirculationFanVibrationStatus { get; set; }
        public object CirculationWaterStatus { get; set; }
        
        public object PressureAirInletProportionalValveStatus { get; set; }
        public object PressureAirInletProportionalValveValue { get; set; }
        public object PressureAirInletProportionalValveAutoManual { get; set; }

        public object PressureAirInletValveStatus { get; set; }
        public object PressureAirInletValveAutoManual { get; set; }
        public object PressureAirInletValveOnOff { get; set; }
        public object PressureAirOutletValveStatus { get; set; }
        public object PressureAirOutletValveOnOff { get; set; }
        public object PressureAirOutletValveAutoManual { get; set; }
        public object PressureAirOutletProportionalValveStatus { get; set; }

        public object PressureAirOutletProportionalValveValue { get; set; }
        public object PressureAirOutletProportionalValveAutoManual { get; set; }
        public object VacuumOutletValveStatus { get; set; }
        public object VacuumOutletValveOnOff { get; set; }
        public object VacuumOutletValveAutoManual { get; set; }
        public object VacuumInletProportionalValveStatus { get; set; }
        public object VacuumIntelProportionalValveValue { get; set; }
        public object VacuumIntelProportionalValveAutoManual { get; set; }
        public object CoolingLineDrainValve1Status { get; set; }
        public object CoolingLineDrainValve1AutoManual { get; set; }
        public object CoolingLineDrainValve1OnOff { get; set; }
        public object CoolingLineDrainValve2Status { get; set; }
        public object CoolingLineDrainValve2AutoManual { get; set; }
        public object CoolingLineDrainValve2OnOff { get; set; }
        public object CoolingLinePressureSwitch { get; set; }
      

        public object HighMonPortNumber { get; set; }
        public object HighMonValue { get; set; }
        public object LowMonPortNumber { get; set; }
        public object LowMonValue { get; set; }
        public object VacSetPoint { get; set; }
        public object VacActualValue { get; set; }
        public object AirTcSetPoint { get; set; }
        public object HighPtcPortNumber { get; set; }
        public object HighPtcPortValue { get; set; }
        public object LowPtcPortNumber { get; set; }
        public object LowPtcValue { get; set; }
        public object TotalWorkingTime { get; set; }
        public object AirTcActualValue { get; set; }



        //Ces

        public object FurnacePressureSwitch1 { get; set; }
        public object FurnacePressureSwitch2 { get; set; }
        public object FurnaceRightHumanSensor { get; set; }
        public object FurnaceLeftHumanSensor { get; set; }
        public object DoorOpenSwStatus { get; set; }
        public object DoorCloseSwStatus { get; set; }
        public object RingOpenSwStatus { get; set; }
        public object RingCloseSwStatus { get; set; }
        public object GeneralPnomaticStatus { get; set; }
        public object HydrolicConfirmationStatus { get; set; }

        public object CompressorValveStatus { get; set; }
        public object PressureSupplySelectionValveAutoManual { get; set; }
        public object pressureSupplySelectionValveControl { get; set; }

        public object BoosterValveStatus { get; set; }
        public object PressureCompressorOrBoosterControlValue { get; set; }

        public object TowerWaterTemperature { get; set; }
        public object CoolingTrimAirCoolingValveStatus { get; set; }
        public object CoolingTrimAirCoolingValveAutoManual { get; set; }
        public object CoolingTrimAirCoolingValveOnOff { get; set; }

        public object CoolingTrimWaterCoolingValveStatus { get; set; }
        public object CoolingTrimWaterCoolingValveAutoManual { get; set; }
        public object CoolingTrimWaterCoolingValveOnOff { get; set; }

        public object CoolingBypassValveStatus { get; set; }
        public object CoolingBypassValveAutoManual { get; set; }
        public object CoolingBypassValveOnOff { get; set; }

        public object Heater2Status { get; set; }
        public object Heater2Value { get; set; }
        public object Heater2AutoManual { get; set; }

        public object Heater4Status { get; set; }
        public object Heater4Value { get; set; }
        public object Heater4AutoManual { get; set; }

        public object Heater1Status { get; set; }
        public object Heater1AutoManual { get; set; }
        public object Heater1OnOff { get; set; }
        public object Heater3Status { get; set; }
        public object Heater3AutoManual { get; set; }
        public object Heater3OnOff { get; set; }

        public object TowerWaterLevelMax { get; set; }
        public object TowerWaterLevelMiddle { get; set; }
        public object TowerWaterLevelMin { get; set; }
        public object FanPanelAndRecineWaterCoolingPump { get; set; }
        public object MainCoolingMotorControl { get; set; }
        public object VacuumTankActualValue { get; set; }

        //  public object PressureAirExhaustProportionalValveValue { get; set; }

        //Tai
        public object AirTCLow { get; set; }
        public object AirTCMediumL { get; set; }
        public object AirTCMediumH { get; set; }
        public object AirTCHigh { get; set; }
        public object VacumLineRightValue { get; set; }
        public object VacumLineLeftValue { get; set; }
        public object VacuumPumpStatus2 { get; set; }
        public object VacuumUnloadingValveStatus2 { get; set; }
        public object VacumLinePressureAirValveStatus { get; set; }
        public object ChillerUnitStatus { get; set; }
        public object PressureNitrogenInletValveStatus { get; set; }
        public object PressureAirExhasustProportionalValveStatus { get; set; }
        public object CoolingTrimAirValveStatus { get; set; }
        public object CoolingLinePressureSwStatus { get; set; }
        public object CirculationFanStatus { get; set; }
        public object CirculationFanCoolingPressureSwStatus { get; set; }
        public object HeaterStatus5 { get; set; }
        public object HeaterValue5 { get; set; }
        public object HeaterStatus6 { get; set; }
        public object AirInletProportionalValveValue { get; set; }
        public object AirInletProportionalValveStatus { get; set; }
        public object AirExhasutProportionalValveValue { get; set; }
        public object AirExhasutProportionalValveStatus { get; set; }
        public object CoolingTrimValveValue { get; set; }
        public object CoolingTrimValveValueStatus { get; set; }
        public object CoolingPump1Status { get; set; }
        public object CoolingPump2Status { get; set; }
        public object CoolingPump3Status { get; set; }
        public object CoolingPump4Status { get; set; }
        public object VacuumLineProportionalValveRight { get; set; }
        public object VacuumLineProportionalValveLeft { get; set; }
        public object VacuumLineProportionalValveRightStatus { get; set; }
        public object VacuumLineProportionalValveLeftStatus { get; set; }
        public object ResinFilterWaterTemperature { get; set; }



    }
}
