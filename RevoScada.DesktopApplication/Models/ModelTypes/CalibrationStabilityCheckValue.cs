using RevoScada.DesktopApplication.Models.ModelEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models.ModelTypes
{
    public class CalibrationStabilityCheckValue
    {
        public string MapKey { get { return $"{CalibrationSensorType}::[{MappedDataRowIndex}][{MappedDataColumnIndex}]";  } }
        public int  MappedDataRowIndex { get; set; }
        public int  MappedDataColumnIndex { get; set; }
        public int MappedDataErrorColumnIndex { get; set; }
        public int SensorNo { get; set; }
        public decimal SensorValue { get; set; }
        public short OccurenceCount { get; set; }
        public bool IsStable { get; set; }
        public CalibrationSensorType CalibrationSensorType { get; set; }
    }
}
