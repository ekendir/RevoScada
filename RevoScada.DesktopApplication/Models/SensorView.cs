using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class SensorView
    {
        public int Id { get; set; }
        public string PortName { get; set; }
        public string PortValue { get; set; }
        public string BagName { get; set; }
    }
}
