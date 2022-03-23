using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class IntegratedCheckGridModel
    {
        public long id { get; set; }
        public string PortName { get; set; }
        public float ActualValue { get; set; }
        public float StartValue { get; set; }
        public float FinishValue { get; set; }
        public float Deviation { get; set; }
        public short RequirementValue { get; set; }
        public string BagName { get; set; }
        public DateTime CheckResultSaveDate { get; set; }
    }
}
