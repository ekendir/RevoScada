using Newtonsoft.Json;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Helpers
{
    class ProcessEventLogAdapter
    {
        private SyncIssueManager _syncIssueManager;
        public ProcessEventLogAdapter(string localCacheServer)
        {
            _syncIssueManager = new SyncIssueManager(localCacheServer);
        }

        public void CreateProcessEventLogSyncIssue(ProcessEventLog processEventLog, FromToDirection fromToDirection, int plcDeviceId)
        {
            string serializedEntityObject = JsonConvert.SerializeObject(processEventLog);
            SyncIssue syncIssue = new SyncIssue
            {
                SerializedEntityObject = serializedEntityObject,
                EntityObjectType = typeof(ProcessEventLog),
                SyncDBCommand = SyncDBCommand.Insert,
                FromToDirection = fromToDirection,
                PlcDeviceId = plcDeviceId,
                SyncStatus = SyncStatus.NoneProcessChangesPending,
                TransferType = TransferType.NonProcessChanges,
                BatchId= processEventLog.BatchId,
                 
            };
            _syncIssueManager.CreateNewSyncIssue(syncIssue);
        }

        internal void CreateProcessEventLogSyncIssue(object vacLinesEventLog, object fromToDirection, int ıd)
        {
            throw new NotImplementedException();
        }
    }
}
