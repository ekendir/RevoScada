using RevoScada.DesktopApplication.Models.ModelEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RevoScada.DesktopApplication.Models
{
    public class CalibrationDataGrid : ObservableObject
    {

        private short _sensor;
        private float _oldCallOffset;
        private float _newCallOffset;
        private float _oldGain;
        private float _newGain;
        private float _sensorValue;
        private float _sensorRawValue;
        private float _actualValue;

        public Guid TableIndex { get; set; }

        public CalibrationSensorType CalibrationSensorType { get; set; }

        public string SensorTypeLiteral
        {
            get
            {
                return CalibrationSensorType.ToString();
            }
        }

        public string CalibrationSensorValue { get; set; }
        public short Sensor { get => _sensor; set => OnPropertyChanged(ref _sensor, value); }
        public float OldCallOffset { get => _oldCallOffset; set => OnPropertyChanged(ref _oldCallOffset, value); }
        public float NewCallOffset { get => _newCallOffset; set => OnPropertyChanged(ref _newCallOffset, value); }
        public float OldGain { get => _oldGain; set => OnPropertyChanged(ref _oldGain, value); }
        public float NewGain { get => _newGain; set => OnPropertyChanged(ref _newGain, value); }
        public float SensorValue { get => _sensorValue; set => OnPropertyChanged(ref _sensorValue, value); }
        public float SensorRawValue { get => _sensorRawValue; set => OnPropertyChanged(ref _sensorRawValue, value); }
        public float ActualValue { get => _actualValue; set => OnPropertyChanged(ref _actualValue, value); }


    }
}
