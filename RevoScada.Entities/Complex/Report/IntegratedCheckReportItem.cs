using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    public class IntegratedCheckReportItem
    {
        public string BagName { get; set; }
        public string SelectedSensorName  { get; set; }
        public float ActualValue { get; set; }
        public float StartValue { get; set; }
        public float FinishValue { get; set; }
        public float Deviation { get; set; }
        public short RequirementValue { get; set; }
    

    }
}
