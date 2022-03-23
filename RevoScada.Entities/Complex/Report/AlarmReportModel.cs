using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    public class AlarmReportModel
    {
        public ReportHeaderInfo ReportHeaderInfo { get; set; }
        public List<PlcAlarmReportItem> PlcAlarmReportItems { get; set; }
        public List<ProcessEventLogReportItem> ProcessEventLogReportItems { get; set; }
       
    }
}
