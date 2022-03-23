using System;

namespace RevoScada.DesktopApplication.Models
{
    public class HamburgerMenuLeftModel : ObservableObject
    {
        private float _airTemperatureCalcSetPoint;
        public float AirTemperatureCalcSetPoint
        {
            get => _airTemperatureCalcSetPoint;
            set => OnPropertyChanged(ref _airTemperatureCalcSetPoint, value);
        }

        private float _actualTemperatureCalcSetPoint;
        public float ActualTemperatureCalcSetPoint
        {
            get => _actualTemperatureCalcSetPoint;
            set => OnPropertyChanged(ref _actualTemperatureCalcSetPoint, value);
        }

        private float _airTCLow;
        public float AirTCLow
        {
            get => _airTCLow;
            set => OnPropertyChanged(ref _airTCLow, value);
        }

        private float _airTCMedium;
        public float AirTCMedium
        {
            get => _airTCMedium;
            set => OnPropertyChanged(ref _airTCMedium, value);
        }

        private float _airTCHigh;
        public float AirTCHigh
        {
            get => _airTCHigh;
            set => OnPropertyChanged(ref _airTCHigh, value);
        }

        private float _vacSet;
        public float VacSet
        {
            get => _vacSet;
            set => OnPropertyChanged(ref _vacSet, value);
        }

        private float _vacSetRight;
        public float VacSetRight
        {
            get => _vacSetRight;
            set => OnPropertyChanged(ref _vacSetRight, value);
        }

        private float _vacActual;
        public float VacActual
        {
            get => _vacActual;
            set => OnPropertyChanged(ref _vacActual, value);
        }

        private float _vacActualRight;
        public float VacActualRight
        {
            get => _vacActualRight;
            set => OnPropertyChanged(ref _vacActualRight, value);
        }

        private float _pressureSet;
        public float PressureSet
        {
            get => _pressureSet;
            set => OnPropertyChanged(ref _pressureSet, value);
        }

        private float _pressureActual;
        public float PressureActual
        {
            get => _pressureActual;
            set => OnPropertyChanged(ref _pressureActual, value);
        }

        private bool _ptcAllSensorsWorking;
        public bool PtcAllSensorsWorking
        {
            get => _ptcAllSensorsWorking;
            set => OnPropertyChanged(ref _ptcAllSensorsWorking, value);
        }

        #region Rate

        private float _vacHeaderRate;
        public float VacHeaderRate
        {
            get => _vacHeaderRate;
            set => OnPropertyChanged(ref _vacHeaderRate, value);
        }

        private float _vacHeaderRateRight;
        public float VacHeaderRateRight
        {
            get => _vacHeaderRateRight;
            set => OnPropertyChanged(ref _vacHeaderRateRight, value);
        }

        private float _airTCRate;
        public float AirTCRate
        {
            get => _airTCRate;
            set => OnPropertyChanged(ref _airTCRate, value);
        }

        private float _highPTCRate;
        public float HighPTCRate
        {
            get => _highPTCRate;
            set => OnPropertyChanged(ref _highPTCRate, value);
        }

        private float _lowPTCRate;
        public float LowPTCRate
        {
            get => _lowPTCRate;
            set => OnPropertyChanged(ref _lowPTCRate, value);
        }

        private float _highMonRate;
        public float HighMonRate
        {
            get => _highMonRate;
            set => OnPropertyChanged(ref _highMonRate, value);
        }

        private float _lowMonRate;
        public float LowMonRate
        {
            get => _lowMonRate;
            set => OnPropertyChanged(ref _lowMonRate, value);
        }

        private float _pressureRate;
        public float PressureRate
        {
            get => _pressureRate;
            set => OnPropertyChanged(ref _pressureRate, value);
        }



        #endregion Rate








        private float _ptcSetPoint;
        public float PtcSetPoint
        {
            get => _ptcSetPoint;
            set => OnPropertyChanged(ref _ptcSetPoint, value);
        }

        private float _highPTCPortNumber;
        public float HighPTCPortNumber
        {
            get => _highPTCPortNumber;
            set => OnPropertyChanged(ref _highPTCPortNumber, value);
        }

        private float _highPTCValue;
        public float HighPTCValue
        {
            get => _highPTCValue;
            set => OnPropertyChanged(ref _highPTCValue, value);
        }

        private float _lowPTCPortNumber;
        public float LowPTCPortNumber
        {
            get => _lowPTCPortNumber;
            set => OnPropertyChanged(ref _lowPTCPortNumber, value);
        }

        private float _lowPTCValue;
        public float LowPTCValue
        {
            get => _lowPTCValue;
            set => OnPropertyChanged(ref _lowPTCValue, value);
        }

        private float _highMONPortNumber;
        public float HighMONPortNumber
        {
            get => _highMONPortNumber;
            set => OnPropertyChanged(ref _highMONPortNumber, value);
        }

        private float _highMONValue;
        public float HighMONValue
        {
            get => _highMONValue;
            set => OnPropertyChanged(ref _highMONValue, value);
        }

        private float _lowMONPortNumber;
        public float LowMONPortNumber
        {
            get => _lowMONPortNumber;
            set => OnPropertyChanged(ref _lowMONPortNumber, value);
        }

        private float _lowMONValue;
        public float LowMONValue
        {
            get => _lowMONValue;
            set => OnPropertyChanged(ref _lowMONValue, value);
        }

        private string _runStatus;
        public string RunStatus
        {
            get => _runStatus;
            set => OnPropertyChanged(ref _runStatus, value);
        }

        private string _batchStartTime;
        public string BatchStartTime
        {
            get => _batchStartTime;
            set => OnPropertyChanged(ref _batchStartTime, value);
        }
        private string _batchFinishTime;
        public string BatchFinishTime
        {
            get => _batchFinishTime;
            set => OnPropertyChanged(ref _batchFinishTime, value);
        }

        

        private string _batchName;
        public string BatchName
        {
            get => _batchName;
            set => OnPropertyChanged(ref _batchName, value);
        }

        private string _recipeName;
        public string RecipeName
        {
            get => _recipeName;
            set => OnPropertyChanged(ref _recipeName, value);
        }

        private string _segDescription;
        public string SegDescription
        {
            get => _segDescription;
            set => OnPropertyChanged(ref _segDescription, value);
        }

        private string _segmentNo;
        public string SegmentNo
        {
            get => _segmentNo;
            set => OnPropertyChanged(ref _segmentNo, value);
        }

        private string _segElapsedTime;
        public string SegElapsedTime
        {
            get => _segElapsedTime;
            set => OnPropertyChanged(ref _segElapsedTime, value);
        }

        private string _segWaitingTime;
        public string SegWaitingTime
        {
            get => _segWaitingTime;
            set => OnPropertyChanged(ref _segWaitingTime, value);
        }

        private string _batchElapsedTime;
        public string BatchElapsedTime
        {
            get => _batchElapsedTime;
            set => OnPropertyChanged(ref _batchElapsedTime, value);
        }

        private bool _isSystemAuto;
        public bool IsSystemAuto
        {
            get => _isSystemAuto;
            set => OnPropertyChanged(ref _isSystemAuto, value);
        }

        #region Progress Bar Values

        #region Air Temperature Section
        private double _airTempProgressBarsMax = 260;
        private double _airTempProgressBarsMin = 0;
        private double _calcSetProgressBarValue;
        public double CalcSetProgressBarValue
        {
            get
            {
                // Subtract min from max then divide to received value from PLC. Lastly, multiply with 100.
                double absoluteVal = Math.Abs((AirTemperatureCalcSetPoint / (_airTempProgressBarsMax - _airTempProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _calcSetProgressBarValue, value);
        }

        private double _actualTempProgressBarValue;
        public double ActualTempProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((ActualTemperatureCalcSetPoint / (_airTempProgressBarsMax - _airTempProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _actualTempProgressBarValue, value);
        }
        #endregion

        #region Vacuum Header Section
        private double _vacuumHeaderProgressBarsMax = -750;
        private double _vacuumHeaderProgressBarsMin = 0;
        private double _vacSetProgressBarValue;
        public double VacSetProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((VacSet / (_vacuumHeaderProgressBarsMax - _vacuumHeaderProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _vacSetProgressBarValue, value);
        }

        private double _vacActualProgressBarValue;
        public double VacActualProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((VacActual / (_vacuumHeaderProgressBarsMax - _vacuumHeaderProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _vacActualProgressBarValue, value);
        }



        private double _vacSetProgressBarValueRight;
        public double VacSetProgressBarValueRight
        {
            get
            {
                double absoluteVal = Math.Abs((VacSetRight / (_vacuumHeaderProgressBarsMax - _vacuumHeaderProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _vacSetProgressBarValueRight, value);
        }

        private double _vacActualProgressBarValueRight;
        public double VacActualProgressBarValueRight
        {
            get
            {
                double absoluteVal = Math.Abs((VacActualRight / (_vacuumHeaderProgressBarsMax - _vacuumHeaderProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _vacActualProgressBarValueRight, value);
        }










        #endregion

        #region Part Temperature Section
        private double _partTempProgressBarsMax = 400;
        private double _partTempProgressBarsMin = 0;
        private double _ptcSetPointProgressBarValue;
        public double PtcSetPointProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((PtcSetPoint / (_partTempProgressBarsMax - _partTempProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _ptcSetPointProgressBarValue, value);
        }

        private double _highPtcProgressBarValue;
        public double HighPtcProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((HighPTCValue / (_partTempProgressBarsMax - _partTempProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _highPtcProgressBarValue, value);
        }

        private double _lowPtcProgressBarValue;
        public double LowPtcProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((LowPTCValue / (_partTempProgressBarsMax - _partTempProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _lowPtcProgressBarValue, value);
        }
        #endregion

        #region Part Vacuum Header Section
        private double _partVacuumProgressBarsMax = -750;
        private double _partVacuumProgressBarsMin = 0;
        private double _highMonProgressBarValue;
        public double HighMonProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((HighMONValue / (_partVacuumProgressBarsMax - _partVacuumProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _highMonProgressBarValue, value);
        }

        private double _lowMonProgressBarValue;
        public double LowMonProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((LowMONValue / (_partVacuumProgressBarsMax - _partVacuumProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _lowMonProgressBarValue, value);
        }
        #endregion

        #region Pressure Section
        private double _pressureProgressBarsMax = 16;
        private double _pressureProgressBarsMin = 0;
        private double _pressSetProgressBarValue;
        public double PressSetProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((PressureSet / (_pressureProgressBarsMax - _pressureProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _pressSetProgressBarValue, value);
        }

        private double _pressActualProgressBarValue;
        public double PressActualProgressBarValue
        {
            get
            {
                double absoluteVal = Math.Abs((PressureActual / (_pressureProgressBarsMax - _pressureProgressBarsMin)) * 100);
                return absoluteVal;
            }
            set => OnPropertyChanged(ref _pressActualProgressBarValue, value);
        }
        #endregion

        #endregion
    }
}
