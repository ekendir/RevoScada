using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class BatchQualityDetailModel : ObservableObject
    {
        public int id { get; set; }
        private int _batchQualityId;
        public int BatchQualityId
        {
            get => _batchQualityId;
            set => OnPropertyChanged(ref _batchQualityId, value);
        }
        private string _phaseName;
        public string PhaseName
        {
            get => _phaseName;
            set => OnPropertyChanged(ref _phaseName, value);
        }
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified;
            set => OnPropertyChanged(ref _lastModified, value);
        }
        private bool _phaseStyle;
        public bool PhaseStyle
        {
            get => _phaseStyle;
            set => OnPropertyChanged(ref _phaseStyle, value);
        }
        private string _phaseTitle;
        public string PhaseTitle
        {
            get => _phaseTitle;
            set => OnPropertyChanged(ref _phaseTitle, value);
        }
        private string _phaseChange;
        public string PhaseChange
        {
            get => _phaseChange;
            set => OnPropertyChanged(ref _phaseChange, value);
        }
        private string _phaseCriteria;
        public string PhaseCriteria
        {
            get => _phaseCriteria;
            set => OnPropertyChanged(ref _phaseCriteria, value);
        }
        private int _phaseCriteriaValue;
        public int PhaseCriteriaValue
        {
            get => _phaseCriteriaValue;
            set => OnPropertyChanged(ref _phaseCriteriaValue, value);
        }
        private decimal _phaseMinTime;
        public decimal PhaseMinTime
        {
            get => _phaseMinTime;
            set => OnPropertyChanged(ref _phaseMinTime, value);
        }
        private decimal _phaseMaxTime;
        public decimal PhaseMaxTime
        {
            get => _phaseMaxTime;
            set => OnPropertyChanged(ref _phaseMaxTime, value);
        }
        private bool _airTempStyle;
        public bool AirTempStyle
        {
            get => _airTempStyle;
            set => OnPropertyChanged(ref _airTempStyle, value);
        }
        private string _airTempTitle;
        public string AirTempTitle
        {
            get => _airTempTitle;
            set => OnPropertyChanged(ref _airTempTitle, value);
        }
        private decimal _airTempMin;
        public decimal AirTempMin
        {
            get => _airTempMin;
            set => OnPropertyChanged(ref _airTempMin, value);
        }
        private decimal _airTempMax;
        public decimal AirTempMax
        {
            get => _airTempMax;
            set => OnPropertyChanged(ref _airTempMax, value);
        }
        private bool _pressureStyle;
        public bool PressureStyle
        {
            get => _pressureStyle;
            set => OnPropertyChanged(ref _pressureStyle, value);
        }
        private string _pressureTitle;
        public string PressureTitle
        {
            get => _pressureTitle;
            set => OnPropertyChanged(ref _pressureTitle, value);
        }
        private float _pressureRateMin;
        public float PressureRateMin
        {
            get => _pressureRateMin;
            set => OnPropertyChanged(ref _pressureRateMin, value);
        }
        private float _pressureRateMax;
        public float PressureRateMax
        {
            get => _pressureRateMax;
            set => OnPropertyChanged(ref _pressureRateMax, value);
        }
        private float _pressurePhaseStartMin;
        public float PressurePhaseStartMin
        {
            get => _pressurePhaseStartMin;
            set => OnPropertyChanged(ref _pressurePhaseStartMin, value);
        }
        private float _pressurePhaseStartMax;
        public float PressurePhaseStartMax
        {
            get => _pressurePhaseStartMax;
            set => OnPropertyChanged(ref _pressurePhaseStartMax, value);
        }
        private float _pressurePhaseEndMin;
        public float PressurePhaseEndMin
        {
            get => _pressurePhaseEndMin;
            set => OnPropertyChanged(ref _pressurePhaseEndMin, value);
        }
        private float _pressurePhaseEndMax;
        public float PressurePhaseEndMax
        {
            get => _pressurePhaseEndMax;
            set => OnPropertyChanged(ref _pressurePhaseEndMax, value);
        }
        private bool _probeStyle;
        public bool ProbeStyle
        {
            get => _probeStyle;
            set => OnPropertyChanged(ref _probeStyle, value);
        }
        private string _probeTitle;
        public string ProbeTitle
        {
            get => _probeTitle;
            set => OnPropertyChanged(ref _probeTitle, value);
        }
        private decimal _probePhaseStartMin;
        public decimal ProbePhaseStartMin
        {
            get => _probePhaseStartMin;
            set => OnPropertyChanged(ref _probePhaseStartMin, value);
        }
        private decimal _probePhaseStartMax;
        public decimal ProbePhaseStartMax
        {
            get => _probePhaseStartMax;
            set => OnPropertyChanged(ref _probePhaseStartMax, value);
        }
        private decimal _probePhaseEndMin;
        public decimal ProbePhaseEndMin
        {
            get => _probePhaseEndMin;
            set => OnPropertyChanged(ref _probePhaseEndMin, value);
        }
        private decimal _probePhaseEndMax;
        public decimal ProbePhaseEndMax
        {
            get => _probePhaseEndMax;
            set => OnPropertyChanged(ref _probePhaseEndMax, value);
        }
        private bool _partTempStyle;
        public bool PartTempStyle
        {
            get => _partTempStyle;
            set => OnPropertyChanged(ref _partTempStyle, value);
        }
        private string _partTempTitle;
        public string PartTempTitle
        {
            get => _partTempTitle;
            set => OnPropertyChanged(ref _partTempTitle, value);
        }
        private float _partTempRateMin;
        public float PartTempRateMin
        {
            get => _partTempRateMin;
            set => OnPropertyChanged(ref _partTempRateMin, value);
        }
        private float _partTempRateMax;
        public float PartTempRateMax
        {
            get => _partTempRateMax;
            set => OnPropertyChanged(ref _partTempRateMax, value);
        }
        private decimal _partTempLowRange;
        public decimal PartTempLowRange
        {
            get => _partTempLowRange;
            set => OnPropertyChanged(ref _partTempLowRange, value);
        }
        private decimal _partTempHighRange;
        public decimal PartTempHighRange
        {
            get => _partTempHighRange;
            set => OnPropertyChanged(ref _partTempHighRange, value);
        }
        private short _sortOrder;
        public short SortOrder
        {
            get => _sortOrder;
            set => OnPropertyChanged(ref _sortOrder, value);
        }
        private int _partTempRateCalcInterval;
        public int PartTempRateCalcInterval
        {
            get => _partTempRateCalcInterval;
            set => OnPropertyChanged(ref _partTempRateCalcInterval, value);
        }
    }
}
