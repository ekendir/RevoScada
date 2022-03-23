using RevoScada.Entities;
using System.Collections.Generic;

namespace RevoScada.Synchronization.Types
{
    public class SyncSingleBatchData
    {
        public List<ActiveTag> ActiveTags { get; set; }
        public List<Bag> Bags { get; set; }
        public Batch Batch { get; set; }
        public CurrentProcessInfo CurrentProcessInfo { get; set; }
  
        public List<IntegratedCheckResult> IntegratedCheckResults { get; set; }
        public List<LotProperty> LotProperties { get; set; }
 
        public List<ProcessEventLog> ProcessEventLogs { get; set; }

        public List<RecipeDetailHistory> RecipeDetailHistories { get; set; }
        public List<RecipeDetail> RecipeDetails { get; set; }

        public List<RecipeGroup> RecipeGroups { get; set; }
        public Recipe Recipe { get; set; }
        public SkippedIntegratedCheckResult SkippedIntegratedCheckResult { get; set; }

        public string PlcAlarmsCachedRelationKey { get; set; }
        public List<PlcAlarm> PlcAlarms { get; set; }

        public int LastLoadNumber { get; set; }
    }
}