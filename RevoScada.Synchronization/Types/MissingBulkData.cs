using RevoScada.Entities;
using System;
using System.Collections.Generic;

namespace RevoScada.Synchronization.Types
{
    public class MissingBulkData
    {
        public DateTime CreateDate { get; set; }

        public List<ActiveTag> ActiveTags { get; set; }
        public List<Bag> Bags { get; set; }
        public List<Batch> Batches { get; set; }
        public CurrentProcessInfo CurrentProcessInfo { get; set; }
        public List<DataLog> DataLogs { get; set; }

        public List<IntegratedCheckResult>   IntegratedCheckResults { get; set; }
        public List<LotProperty> LotProperties { get; set; }

        public List<PlcAlarm> PlcAlarms { get; set; }

        public List<ProcessEventLog> ProcessEventLogs { get; set; }


        public List<RecipeDetailHistory>   RecipeDetailHistories { get; set; }
        public List<RecipeDetail>   RecipeDetails { get; set; }
        
        public List<RecipeGroup> RecipeGroups { get; set; }
        public List<Recipe>  Recipes { get; set; }
        public List<SkippedIntegratedCheckResult> SkippedIntegratedCheckResults { get; set; }
      
        public string DataLogsCachedRelationKey { get; set; }
        
        public string PlcAlarmsCachedRelationKey { get; set; }
       
        public int LastLoadNumber { get; set; }



    }
}