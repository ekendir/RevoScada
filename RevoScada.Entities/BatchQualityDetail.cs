using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"BatchQualityDetails\"")]
    public class BatchQualityDetail
    {
        [ExplicitKey]
        public int id { get; set; }
        public int BatchQualityId { get; set; }
        public string PhaseName { get; set; }
        public DateTime LastModified { get; set; }
        public bool PhaseStyle { get; set; }
        public string PhaseTitle { get; set; }
        public string PhaseChange { get; set; }
        public string PhaseCriteria { get; set; }
        public int PhaseCriteriaValue { get; set; }
        public decimal PhaseMinTime { get; set; }
        public decimal PhaseMaxTime { get; set; }
        public bool AirTempStyle { get; set; }
        public string AirTempTitle { get; set; }
        public decimal AirTempMin { get; set; }
        public decimal AirTempMax { get; set; }
        public bool PressureStyle { get; set; }
        public string PressureTitle { get; set; }
        public float PressureRateMin { get; set; }
        public float PressureRateMax { get; set; }
        public float PressurePhaseStartMin { get; set; }
        public float PressurePhaseStartMax { get; set; }
        public float PressurePhaseEndMin { get; set; }
        public float PressurePhaseEndMax { get; set; }
        public bool ProbeStyle { get; set; }
        public string ProbeTitle { get; set; }
        public decimal ProbePhaseStartMin { get; set; }
        public decimal ProbePhaseStartMax { get; set; }
        public decimal ProbePhaseEndMin { get; set; }
        public decimal ProbePhaseEndMax { get; set; }
        public bool PartTempStyle { get; set; }
        public string PartTempTitle { get; set; }
        public float PartTempRateMin { get; set; }
        public float PartTempRateMax { get; set; }
        public decimal PartTempLowRange { get; set; }
        public decimal PartTempHighRange { get; set; }
        public short SortOrder { get; set; }
        public int PartTempRateCalcInterval { get; set; }

    }
}
 