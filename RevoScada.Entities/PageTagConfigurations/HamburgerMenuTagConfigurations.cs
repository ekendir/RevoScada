using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class HamburgerMenuTagConfigurations
    {



        public int AirTemperatureCalcSetPoint { get; set; }
        public int ActualTemperatureCalcSetPoint { get; set; }
        public int AirTCLow { get; set; }
        public int AirTCMedium { get; set; }
        public int AirTCHigh { get; set; }
        public int VacSet { get; set; }
        public int VacActual { get; set; }
        public int VacSetRight { get; set; }
        public int VacActualRight { get; set; }
        public int VacHeaderRateRight { get; set; }
        public int PtcSetPoint { get; set; }
        public int HighPTCPortNumber { get; set; }
        public int HighPTCValue { get; set; }
        public int LowPTCPortNumber { get; set; }
        public int LowPTCValue { get; set; }
        public int HighMONPortNumber { get; set; }
        public int HighMONValue { get; set; }
        public int LowMONPortNumber { get; set; }
        public int LowMONValue { get; set; }
        public int RunStatus { get; set; }
        public int BatchStartTime { get; set; }
        public int BatchFinishTime { get; set; }
        public int BatchName { get; set; }
        public int RecipeName { get; set; }
        public int SegDescription { get; set; }
        public int SegmentNo { get; set; }
        public int SegElapsedTime { get; set; }
        public int SegWaitingTime { get; set; }
        public int BatchElapsedTime { get; set; }
        public int IsSystemAuto { get; set; }
        public int PressureSet { get; set; }
        public int PressureActual { get; set; }
        public int VacHeaderRate { get; set; }
        public int AirTCRate { get; set; }
        public int HighPTCRate { get; set; }
        public int LowPTCRate { get; set; }
        public int HighMonRate { get; set; }
        public int LowMonRate { get; set; }
        public int PressureRate { get; set; }
        public int PtcAllSensorsWorking { get; set; }
      }
}
