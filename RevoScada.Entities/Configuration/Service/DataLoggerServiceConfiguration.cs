using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Configuration.Service
{
    public class DataLoggerServiceConfiguration: BaseConfiguration
    {
        public Dictionary<int,string> PostgreSqlConnectionStrings { get; set; }
        public string PlcType { get; set; }
        public string RedisServer { get; set; }
        public int CycleWaitInMiliseconds { get; set; }
    }
}
