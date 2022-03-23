using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class RunOperationTagConfigurations
    {
        public object EnterPartsOk { get; set; }
        public object RecipeOk { get; set; }
        public object IntegrityCheckOk { get; set; }
        public object SkipIntegrityCheck { get; set; }
        public object DoorStatus { get; set; }
        public object StartButtonEnable { get; set; }
        public object BatchStartAction { get; set; }
        public object GoToNextSegment { get; set; }
        public object BackToNextSegment { get; set; }
        public object HoldRun { get; set; }
        public object EndRun { get; set; }
        public object SegmentNo { get; set; }
        public object ActualTemperatureCalcSetPoint { get; set; }
        public object VacActual { get; set; }
        public object PressureActual { get; set; }

    }
}