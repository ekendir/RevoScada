using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoScada.DesktopApplication.Models.ModelTypes
{
    public class CalibrationCertificationCheckSettings : ObservableObject
    {
        private int _stabilityCount;
        private int _sequenceOfSensorRangeStartSelection;
        private int _sequenceOfSensorRangeEndSelection;
        private List<object> _checkValueList;
        private decimal _tolerance;
        private bool _isParametersSaved;
        private bool _isSaveEnabled;

        private CalibrationCertificationCheckStatus _calibrationCertificationCheckStatus;
        public int DefaultSequenceOfSensorMax { get; set; }
        public int DefaultSequenceOfSensorMin { get; set; }
     
        public List<object> CheckValueList
        {
            get => _checkValueList;
            set
            {
                if (CalibrationCertificationCheckStatus == CalibrationCertificationCheckStatus.Reset)
                {
                    IsSettingsSaved = false;
                    IsSaveEnabled = true;
                    OnPropertyChanged(ref _checkValueList, value);
                }
            }
        }
        public CalibrationCertificationCheckStatus CalibrationCertificationCheckStatus { get => _calibrationCertificationCheckStatus; set { OnPropertyChanged(ref _calibrationCertificationCheckStatus, value); } }

        [JsonIgnore]
        public List<decimal> CheckValueListAsDecimal { get { return CheckValueList.Select(x => Convert.ToDecimal(x)).ToList(); } }
        public decimal Tolerance
        {
            get => _tolerance;
            set
            {
                OnPropertyChanged(ref _tolerance, value);
                IsSettingsSaved = false;
                IsSaveEnabled = true;
            }
        }
        public int StabilityCount
        {
            get => _stabilityCount;
            set
            {
                IsSettingsSaved = false;
                IsSaveEnabled = true; OnPropertyChanged(ref _stabilityCount, value);
            }
        }
        public bool IsSettingsSaved { get => _isParametersSaved; set => OnPropertyChanged(ref _isParametersSaved, value); }
        public bool IsSaveEnabled { get => _isSaveEnabled; set => OnPropertyChanged(ref _isSaveEnabled, value); }
    }

    public enum CalibrationCertificationCheckStatus
    {
        Reset, Running, Paused
    }
}
