using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class PartVacuumMon
    {
        public string ActualName { get; set; }
        public string Actual { get; set; }
        public string Start { get; set; }
        public string Finish { get; set; }
        public string Deviation { get; set; }
    }
}
