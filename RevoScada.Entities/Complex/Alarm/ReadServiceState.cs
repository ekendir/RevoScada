using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Alarm
{
    public class ReadServiceState
    {
        public DateTime LastCycleRunTime { get; set; }
        public int GetAllDBCount { get; set;}
        public int PlcId { get; set; }
    }
}
