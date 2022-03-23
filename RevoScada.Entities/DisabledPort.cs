using System;
using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"DisabledPorts\"")]
    public class DisabledPort
    {
        [ExplicitKey]
        public int id { get; set; }
        public int BatchId { get; set; }
        public int[] TagConfigurationList { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
