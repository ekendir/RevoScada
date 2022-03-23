using RevoScada.Synchronization.Enums;
using System;

namespace RevoScada.Synchronization.Types
{
    public class SyncIssue 
    {
        public string CachedKey { get; set; }
        public string MachineId { get; set; }
        public int PlcDeviceId { get; set; }
        public int BatchId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public FromToDirection FromToDirection { get; set; }
        public TransferType TransferType { get; set; } = TransferType.NotDefined;
        public SyncDBCommand SyncDBCommand { get; set; }
        public string SerializedEntityObject { get; set; }
        public Type EntityObjectType { get; set; }
         
    }
}
