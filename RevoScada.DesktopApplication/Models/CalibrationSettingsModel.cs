using RevoScada.DesktopApplication.Models.ModelEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class CalibrationSettingsModel
    {
        public CalibrationSensorType? SensorType { get; set; }
        public short LastSelectedSequenceOfSensorMinPTC { get; set; }
        public short LastSelectedSequenceOfSensorMaxPTC { get; set; }
        public short LastSelectedSequenceOfSensorMinMON { get; set; }
        public short LastSelectedSequenceOfSensorMaxMON { get; set; }
    }
}
