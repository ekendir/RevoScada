
using System.Collections.Generic;

namespace RevoScada.Entities.Configuration.Service
{
    public class AlarmServiceConfiguration
    {
        public string SqliteConnectionString { get; set; }
        public Dictionary<int, string> PostgreSqlConnectionStrings { get; set; }
        public string PlcType { get; set; }
        public string RedisServer { get; set; }
        public int CycleWaitInMiliseconds { get; set; }
        public LogSettings LogSettings { get; set; }
    }
}
 