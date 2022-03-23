using RevoScada.Synchronization.Enums;
using System;

namespace RevoScada.Synchronization.Types
{
    public class MissingBulkDataHeader
    {
        public string CachedKey { get; set; }
        public int PlcDeviceId { get; set; }
        public DateTime CreateDate { get; set; }
        public SyncDataTransferState SyncState { get; set; }
        public FromToDirection FromToDirection { get; set; }
        public TransferType TransferType { get; set; }
        public string MissingBulkDataKey { get; set; }
    }
}