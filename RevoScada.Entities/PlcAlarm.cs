using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"PlcAlarms\"")]
    public class PlcAlarm
    {
        [ExplicitKey]
        public long id { get; set; }
        public int TagConfigurationId { get; set; }
        public string Status { get; set; }
        public DateTime? InDateTime { get; set; }
        public DateTime? OutDateTime { get; set; }
        public DateTime? AcknowledgedDateTime { get; set; }
        public int BatchId { get; set; }
        public bool PlcValue { get; set; }

        [Computed]
        public int Order { get; set; }

        [Computed]
        public string AlarmKey { get; set; }
    }
}