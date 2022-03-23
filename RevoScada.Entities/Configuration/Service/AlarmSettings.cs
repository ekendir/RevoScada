using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Configuration.Service
{
   public class AlarmSettings
    {
        public int MaxPingFailureToAlarm { get; set; }
        public int MaxPlcConnectionFailureToAlarm { get; set; }

    }
}
