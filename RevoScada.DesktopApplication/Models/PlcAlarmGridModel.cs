using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class PlcAlarmGridModel
    {
        public long id { get; set; }
        public string TagName { get; set; }
        public string Status { get; set; }
        public DateTime? InDateTime { get; set; }
        public DateTime? OutDateTime { get; set; }
        public DateTime? AcknowledgedDateTime { get; set; }
        public bool PlcValue { get; set; }
    }
}
