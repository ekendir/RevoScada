using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    public class ProcessEventLogReportItem
    {
       
        public string EventText { get; set; }
        public DateTime InTime { get; set; }
        public string AlarmType { get; set; }
    }
}