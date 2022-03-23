using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class OscillationCriteria
    {
        public string Type { get; set; }
        public int Order { get; set; }
        public object Action  { get; set; }
        public object ToleranceValue { get; set; }
        public object SensorFaultCount { get; set; }
        public object CheckDurationInMs { get; set; }
    }
}
