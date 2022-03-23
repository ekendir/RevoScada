using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
   public class SensorViewTagConfigurations
    {
       
        public Dictionary<string, SensorViewPorts> PartVacuumDatas { get; set; }
        public Dictionary<string, SensorViewPorts> PartTemperaturePorts { get; set; }
        public List<int> DbNumbers { get; set; }
    }
}
