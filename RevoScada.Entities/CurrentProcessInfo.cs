using System;
using Dapper.Contrib.Extensions;
using RevoScada.Entities.Enums;

namespace RevoScada.Entities
{
    [Table("public.\"CurrentProcessInfos\"")]
    public class CurrentProcessInfo
    {
        [ExplicitKey]
        public int id { get; set; }
        public int BatchId { get; set; }
        public string LoadNumber { get; set; }
        public bool IsBatchLoaded { get; set; }
        public int ActiveRecipeId { get; set; }
        public string ActiveRecipeName { get; set; }
        public bool IsRecipeLoaded { get; set; }
        public int CurrentSegmentNo { get; set; }
        public string CurrentSegmentDescription { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public BatchCurrentState BatchCurrentState { get; set; }
        public bool IsAlarmSaved { get; set; }
        public int PlcDeviceId { get; set; }
        
       
    }
}
