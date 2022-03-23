using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class CalibrationItem
    {
        public short SensorNo { get; set; }
        public object SensorValue { get; set; }
        public object NewGain { get; set; }
        public object OldGain { get; set; }
        public object NewCallOffset { get; set; }
        public object OldCallOffset { get; set; }
        public object SensorRawValue { get; set; }
    

    }
}
