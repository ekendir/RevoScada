using RevoScada.DesktopApplication.Models.ModelEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models.ModelTypes
{
    public class CalibrationCertificationParameters : ObservableObject
    {
        private CalibrationSensorType _selectedCalibrationSensorType;
        private List<KeyValuePair<CalibrationSensorType, string>> _sensorsWithName;
        bool _isParametersSaved;
        private CalibrationCertificationCheckSettings _selectedCalibrationCertificationCheckSettings;
        public Dictionary<CalibrationSensorType, short> SensorCounts { get; set; }
        public Dictionary<CalibrationSensorType, string> SensorUnitSymbols { get; set; }
        public List<KeyValuePair<CalibrationSensorType, string>> SensorsWithName { get => _sensorsWithName; set => OnPropertyChanged(ref _sensorsWithName, value); }
        public CalibrationCertificationCheckSettings SelectedCalibrationCertificationCheckSettings { get => _selectedCalibrationCertificationCheckSettings; set => OnPropertyChanged(ref _selectedCalibrationCertificationCheckSettings, value); }
        public Dictionary<CalibrationSensorType, CalibrationCertificationCheckSettings> CalibrationCertificationSettingsForAllSensorTypes { get; set; }
        public CalibrationSensorType SelectedCalibrationSensorType { get => _selectedCalibrationSensorType; set => OnPropertyChanged(ref _selectedCalibrationSensorType, value); }
        public int SelectedCalibrationSensorTypeIndex { get { return (int)_selectedCalibrationSensorType; } }
        public bool IsParametersSaved { get => _isParametersSaved; set => OnPropertyChanged(ref _isParametersSaved, value); }
    }
}
