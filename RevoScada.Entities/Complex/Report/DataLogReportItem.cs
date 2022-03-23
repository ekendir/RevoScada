using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
   public class DataLogReportItem
    {
        public DateTime ReceivedDate { get; set; }
        public float TagValue { get; set; }
    }
}
