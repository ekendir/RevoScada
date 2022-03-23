using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;


namespace RevoScada.DesktopApplication.Models
{
    public class OscillationCriteriaModel :ObservableObject
    {
        private bool _isActionAlarm;
        private bool _isActionAutoDisable;
        private bool _isActionOff;
        private OscillationCriteriaNames _oscillationCriteriaNames;
        private int _action;
        private float _toleranceValue;
        private int _sensorFaultCount;
        private int _checkDurationInMs;
        public OscillationCriteriaNames OscillationCriteriaNames
        {
            get => _oscillationCriteriaNames;
            set => OnPropertyChanged(ref _oscillationCriteriaNames, value);
        }
        public int Action
        {
            get => _action;
            set => OnPropertyChanged(ref _action, value);
        }
        public float ToleranceValue
        {
            get => _toleranceValue;
            set => OnPropertyChanged(ref _toleranceValue, value);
        }
        public int SensorFaultCount
        {
            get => _sensorFaultCount;
            set => OnPropertyChanged(ref _sensorFaultCount, value);
        }
        public int CheckDurationInMs
        {
            get => _checkDurationInMs;
            set => OnPropertyChanged(ref _checkDurationInMs, value);
        }
        public bool IsActionOff
        {
            get
            {
                return (_action == 0) ? true : false;
            }
            set
            {
                Action = 0;
                OnPropertyChanged(ref _isActionOff, value);

            }
        }
        public bool IsActionAutoDisable
        {
            get
            {
                return (_action == 2) ? true : false;
            }
            set
            {
                Action = 2;
                OnPropertyChanged(ref _isActionAutoDisable, value);
            }
        }
        public bool IsActionAlarm
        {
            get
            {
                return (_action == 1) ? true : false;
            }
            set
            {
                Action = 1;
                OnPropertyChanged(ref _isActionAlarm, value);
            }
        }
    
    }
}

 