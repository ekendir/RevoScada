using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class ManualOperationTagConfigurations
    {
        public object VacPumpControlStateOnOff { get; set; }
        public object VacPumpControlStateOnOffRight { get; set; }
        public object VacPumpControlStateIsAutoRight { get; set; }
        public object VacControlStatusPidRight { get; set; }
        public object VacControlStatusIsAutoRight { get; set; }
        public object VacControlStatusSpRight { get; set; }
        public object SystemVacuumValuePVRight { get; set; }
        public object SystemVacuumValueRateRight { get; set; }


        public object VacControlStatusPid { get; set; }
        public object VacControlStatusIsAuto { get; set; }
        public object VacControlStatusSp { get; set; }
        public object SystemVacuumValuePV { get; set; }
        public object SystemVacuumValueRate { get; set; }
        public object SystemPressureValuePV { get; set; }
        public object SystemPressureValueRate { get; set; }
        public object MonitoringLinesLowMonPort { get; set; }
        public object MonitoringLinesLowMonVacuumValue { get; set; }
        public object MonitoringLinesLowMonVacuumValueInTime { get; set; }
        public object MonitoringLinesHighMonPort { get; set; }
        public object MonitoringLinesHighMonVacuumValue { get; set; }
        public object MonitoringLinesHighMonVacuumValueInTime { get; set; }
        public object PtcFanControlStateIsAuto { get; set; }
        public object PtcFanControlStateIsEnable { get; set; }
        public object PtcFanControlStatePid { get; set; }
        public object PtcHeatControlStateIsAuto { get; set; }
        public object PtcHeatControlStateIsEnable { get; set; }
        public object PtcHeatControlStatePid { get; set; }
        public object PtcCoolControlStateIsAuto { get; set; }
        public object PtcCoolControlStateIsEnable { get; set; }
        public object PtcValueIsAuto { get; set; }
        public object PtcValue { get; set; }
        public object PtcWatchAirTempRate { get; set; }
        public object PtcWatchAirTempActual { get; set; }

        public object AirTcHigh { get; set; }
        public object AirTcMediumHigh { get; set; }
        public object AirTcLow { get; set; }
        public object AirTcMediumLow{ get; set; }

        public object AirTcHighRate { get; set; }
        public object AirTcMediumHighRate { get; set; }
        public object AirTcLowRate { get; set; }
        public object AirTcMediumLowRate { get; set; }
        public object LowPtcPort { get; set; }
        public object LowPtcActual { get; set; }
        public object LowPtcRate { get; set; }
        public object HighPtcPort { get; set; }
        public object HighPtcActual { get; set; }
        public object VacPumpControlStateIsAuto { get; set; }
        public object HighPtcRate { get; set; }

        public object PurgeControlStateIsAuto{ get; set; }
        public object PurgeControlStateOnOff { get; set; }
        public object PressureValveControlStateIsAuto { get; set; }
        public object PressureValveControlStateOnOff { get; set; }
        public object PressureSetControlStatusIsAuto { get; set; }
        public object PressureControlStatusSp { get; set; }
        public object PressureLineControlSelect { get; set; }
        public object FanTemperature { get; set; }
        public object FanRpm { get; set; }
        public object FanSpeed { get; set; }
        public object FanSpeedSetValue { get; set; }
        public object CirculationFanSpeedFeedback1 { get; set; }
        public object CirculationFanSpeedFeedback2 { get; set; }
        public object FanVibration { get; set; }
        public object PidHeatOut { get; set; }
        public object PidCoolOut { get; set; }
        public object PidCoolOutStatus { get; set; }
        public object VacuumLineValue { get; set; }
        public object PidVacOut { get; set; }
        public object PidPressureOut { get; set; }
        public object PressureBar { get; set; }
        public object PidAtmosphereOut { get; set; }
        public object DoorStatus { get; set; }
       public object EquipmentTankControlStateIsAuto { get; set; }
        public object EquipmentTankControlStateOnOff { get; set; }
        public object EquipmentTankControlStatusPid { get; set; }
        public object TowerWaterTemperature { get; set; }
        public object BatchTotalWorkingTime { get; set; }


    }
}
