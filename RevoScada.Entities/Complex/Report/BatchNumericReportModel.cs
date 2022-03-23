using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    public class BatchNumericReportModel
    {
        public NumericReportHeaderInfo NumericReportHeaderInfo { get; set; }

        public List<IntegratedCheckReportItem> IntegratedCheckReportItems { get; set; }
        public SkippedIntegratedCheckReportItem SkippedIntegratedCheckReportItem { get; set; }

        public DataTable NumericDataTable { get; set; }

        public  IEnumerable <Bag> Bags { get; set; }

    }
}
