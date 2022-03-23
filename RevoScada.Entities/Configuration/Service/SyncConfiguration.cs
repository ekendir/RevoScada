
using System.Collections.Generic;

namespace RevoScada.Entities.Configuration.Service
{
    public class SyncConfiguration: BaseConfiguration
    {
        /// <summary>
        /// For single master versions
        /// </summary>
        public bool IsSyncActive { get; set; }
        public Dictionary<int, string> PostgreSqlConnectionStrings { get; set; }
        public string PlcType { get; set; }
        public string RedisServer { get; set; }
        public string RemoteRedisServer { get; set; }
        public string ScadaServer { get; set; }
        public Dictionary<int, string> RemoteComputers { get; set; }
        public short TakeOverTimeDifferenceInSeconds { get; set; }
        public int SyncCycleTimeInSeconds { get; set; }
    }
}
 