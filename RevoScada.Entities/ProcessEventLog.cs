using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"ProcessEventLogs\"")]
    public class ProcessEventLog
    {
        [ExplicitKey]
        public long id { get; set; }
        public string EventText { get; set; }
        public DateTime CreateDate { get; set; }
        public int BatchId { get; set; }
        public string Type { get; set; }
        public int ModifiedByUserId { get; set; }
    }
}