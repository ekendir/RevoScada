using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
 public class SensorViewItemsTagConfiguration
    {
         
            public string PortName { get; set; }
            public SiemensTagConfiguration  PortValue { get; set; }
            public string BagName { get; set; }
            public int BagNameNumber { get; set; }
            public SiemensTagConfiguration EnableDisableCommand { get; set; }
            public SiemensTagConfiguration Rate { get; set; }
        
    }
}
