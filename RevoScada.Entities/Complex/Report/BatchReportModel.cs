using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    public class BatchReportModel
    {
        public ReportHeaderInfo ReportHeaderInfo { get; set; }
        public List<BagSensorAndPartDetail> BagSensorAndPartDetails { get; set; }
        public List<IntegratedCheckReportItem> IntegratedCheckReportItems { get; set; }
        public SkippedIntegratedCheckReportItem SkippedIntegratedCheckReportItem { get; set; }

    }
}
