using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"DataLogs\"")]
    public class DataLog
    {
        [ExplicitKey]
        public long id { get; set; }
        public int BatchId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public float TagValue { get; set; }
        public int TagConfigurationId { get; set; }
    }
}