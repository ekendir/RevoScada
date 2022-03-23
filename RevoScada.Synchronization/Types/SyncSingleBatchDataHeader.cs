using RevoScada.Synchronization.Enums;
using System;
using System.Collections.Generic;

namespace RevoScada.Synchronization.Types
{
    public class SyncSingleBatchDataHeader
    {
        public string CachedKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int BatchId { get; set; }
        public int PlcDeviceId { get; set; }
        public SyncDataTransferState SyncState { get; set; }
        public FromToDirection FromToDirection { get; set; }
        public TransferType TransferType { get; set; }
        public string SyncSingleBatchDataKey { get; set; }
       
    }
}