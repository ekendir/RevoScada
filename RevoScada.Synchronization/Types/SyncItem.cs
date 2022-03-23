using RevoScada.Synchronization.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Synchronization.Types
{
    public class SyncItem:ICloneable
    {
        public string MachineId { get; set; }
        public int PlcDeviceId { get; set; }
        public UsagePriority UsagePriority { get; set; }
        public int BatchId { get; set; }
        public DateTime LastAccessDateToRemote { get; set; }
        public DateTime LastAccessDateToPLC { get; set; }
        public SyncStatus SyncItemStatus { get; set; }

        public Object Clone()
        {
            return this;
        }
    }



}
