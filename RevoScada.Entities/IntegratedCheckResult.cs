using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{

    [Table("public.\"IntegratedCheckResults\"")]
    public class IntegratedCheckResult
    {

        [ExplicitKey]
        public long id { get; set; }

        public float ActualValue { get; set; }
        public float StartValue { get; set; }
        public float FinishValue { get; set; }
        public float Deviation { get; set; }
        public short RequirementValue { get; set; }
        public int BagId { get; set; }
        public int BatchId { get; set; }

        public int SensorTagId { get; set; }
        public DateTime CheckResultSaveDate { get; set; }

    }
}