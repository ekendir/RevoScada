using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class CalibrationTagConfigurations
    {
        public Dictionary<string, CalibrationItem> CalibrationItemsPTC { get; set; }
        public Dictionary<string, CalibrationItem> CalibrationItemsMON { get; set; }
        public Dictionary<string, CalibrationItem> CalibrationItems { get; set; }
        public object SensorType { get; set; }
        public object SetSequenceFirst { get; set; }
        public object SetSequenceLast { get; set; }
        public object SetSourcetoLowSensorValue { get; set; }
        public object SetSourcetoHighSensorValue { get; set; }
        public object CommandSetCalculateAcceptReset { get; set; }

        public short DefaultSequenceOfSensorMinPTC { get; set; }
        public short DefaultSequenceOfSensorMaxPTC { get; set; }
        public short DefaultSequenceOfSensorMinMON { get; set; }
        public short DefaultSequenceOfSensorMaxMON { get; set; }
        public List<int> DbNumbers { get; set; }
    }
}
