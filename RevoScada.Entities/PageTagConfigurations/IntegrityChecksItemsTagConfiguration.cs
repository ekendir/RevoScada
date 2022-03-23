using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class IntegrityChecksItemsTagConfiguration
    {

        public string PortName { get; set; }
        public SiemensTagConfiguration Actual { get; set; }
        public SiemensTagConfiguration Start { get; set; }
        public SiemensTagConfiguration Finish { get; set; }
        public SiemensTagConfiguration Deviation { get; set; }
    }

}
