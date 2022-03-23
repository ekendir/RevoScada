using Quartz;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.DataLoggerService.Jobs
{
    public class DataLoggerInfo
    {
        public CurrentProcessInfo CurrentProcessInfo { get; set; }
        // public bool IsDatalogCycleRunning { get; set; }
        public string AssignedJobId { get; set; }
        public bool IsJobCycleStarted { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public IJobDetail JobDetail { get; set; }
        public ITrigger Trigger { get; set; }
    }
}
