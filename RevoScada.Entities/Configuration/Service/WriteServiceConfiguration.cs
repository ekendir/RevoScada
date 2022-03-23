using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Configuration.Service
{
    public class WriteServiceConfiguration:BaseConfiguration
    {
        public string PlcType { get; set; }
        public string RedisServer { get; set; }
        public int CycleWaitInMiliseconds { get; set; }
        public AlarmSettings AlarmSettings { get; set; }
    }
}
