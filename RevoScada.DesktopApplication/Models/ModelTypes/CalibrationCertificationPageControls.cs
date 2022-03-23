using System.Windows;

namespace RevoScada.DesktopApplication.Models.ModelTypes
{
    public class CalibrationCertificationPageControls : ObservableObject {
        private bool _isStartButtonEnabled = false;
        private bool _isStopButtonEnabled = false;
        private bool _isResetAllByTypeEnabled = false;
        private bool _isResetAllEnabled = false;
        private bool _isSettingParametersLayoutEnabled = true;
        private bool _isSensorTypeSelectorEnabled = true;
        private Visibility  _sensorRangeSelectorVisibility = Visibility.Visible;
        private bool _isGridResetSelectedVisible = false;
        
       
        public bool IsSensorTypeSelectorEnabled { get => _isSensorTypeSelectorEnabled; set => OnPropertyChanged(ref _isSensorTypeSelectorEnabled, value); }
        public Visibility SensorRangeSelectorVisibility { get => _sensorRangeSelectorVisibility; set => OnPropertyChanged(ref _sensorRangeSelectorVisibility, value); }
        //public bool IsToleranceSetEnabled { get => _isToleranceSetEnabled; set => OnPropertyChanged(ref _isToleranceSetEnabled, value); }
        //public bool IsCheckValuesSetEnabled { get => _isCheckValuesSetEnabled; set => OnPropertyChanged(ref _isCheckValuesSetEnabled, value); }
        //public bool IsCycleIntervalSetEnabled { get => _isCycleIntervalSetEnabled; set => OnPropertyChanged(ref _isCycleIntervalSetEnabled, value); }
        //public bool IsStabilityCountSetEnabled { get => _isStabilityCountSetEnabled; set => OnPropertyChanged(ref _isStabilityCountSetEnabled, value); }

        public bool IsStartButtonEnabled { get => _isStartButtonEnabled; set => OnPropertyChanged(ref _isStartButtonEnabled, value); }
        public bool IsStopButtonEnabled { get => _isStopButtonEnabled; set => OnPropertyChanged(ref _isStopButtonEnabled, value); }
        public bool IsResetAllByTypeEnabled { get => _isResetAllByTypeEnabled; set => OnPropertyChanged(ref _isResetAllByTypeEnabled, value); }
        public bool IsResetAllEnabled { get => _isResetAllEnabled; set => OnPropertyChanged(ref _isResetAllEnabled, value); }
        public bool IsSettingParametersLayoutEnabled { get => _isSettingParametersLayoutEnabled; set => OnPropertyChanged(ref _isSettingParametersLayoutEnabled, value); }

        private bool _settingParametersLayoutBeforeResetLayout = false;
        private Visibility _settingParametersLayoutStatus = Visibility.Visible;

        public bool IsSettingParametersLayoutBeforeResetEnabled { get => _settingParametersLayoutBeforeResetLayout; set => OnPropertyChanged(ref _settingParametersLayoutBeforeResetLayout, value); }
        public Visibility SettingParametersLayoutStatus { get => _settingParametersLayoutStatus; set => OnPropertyChanged(ref _settingParametersLayoutStatus, value); }


        public bool IsGridResetSelectedVisible { get => _isGridResetSelectedVisible; set => OnPropertyChanged(ref _isGridResetSelectedVisible, value); }



    }
}
