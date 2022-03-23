using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Alarm
{
    public class AlarmSaveOrder
    {
        public CurrentProcessInfo CurrentProcessInfo { get; set; }
        public bool IsAlarmSaved { get; set; } = false;
        public string SaveOrderId { get; set; } = string.Empty;
      
    }
}
