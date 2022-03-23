using RevoScada.DesktopApplication.Models.ModelEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class CalibrationFormInput : ObservableObject
    {
        private float _setSourceToLowSensorRangeValue;
        private float _setSourceToHighSensorRangeValue;
        private short _defaultSequenceOfSensorMax;
        private short _defaultSequenceOfSensorMin;
        private short _sequenceOfSensorRangeStartSelection;
        private short _sequenceOfSensorRangeEndSelection;
        private CalibrationSensorType _selectedCalibrationSensorType { get; set; }
        public CalibrationSensorType SelectedCalibrationSensorType { get { return _selectedCalibrationSensorType; } set { _selectedCalibrationSensorType = value; } }
        public float SetSourceToLowSensorRangeValue { get => _setSourceToLowSensorRangeValue; set => OnPropertyChanged(ref _setSourceToLowSensorRangeValue, value); }
        public float SetSourceToHighSensorRangeValue { get => _setSourceToHighSensorRangeValue; set => OnPropertyChanged(ref _setSourceToHighSensorRangeValue, value); }
        public short DefaultSequenceOfSensorMax { get => _defaultSequenceOfSensorMax; set => OnPropertyChanged(ref _defaultSequenceOfSensorMax, value); }
        public short DefaultSequenceOfSensorMin { get => _defaultSequenceOfSensorMin; set => OnPropertyChanged(ref _defaultSequenceOfSensorMin, value); }
        public short SequenceOfSensorRangeStartSelection { get => _sequenceOfSensorRangeStartSelection; set => OnPropertyChanged(ref _sequenceOfSensorRangeStartSelection, value); }
        public short SequenceOfSensorRangeEndSelection { get => _sequenceOfSensorRangeEndSelection; set => OnPropertyChanged(ref _sequenceOfSensorRangeEndSelection, value); }

    }
}
