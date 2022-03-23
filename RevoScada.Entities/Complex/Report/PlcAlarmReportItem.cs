using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    public class PlcAlarmReportItem
    {
        public string MessageName { get; set; }
        public string Status { get; set; }
        public DateTime? InDateTime { get; set; }
        public DateTime? OutDateTime { get; set; }
        public DateTime? AcknowledgedDateTime { get; set; }
        public int BatchId { get; set; }
        public bool PlcValue { get; set; }
    }
}