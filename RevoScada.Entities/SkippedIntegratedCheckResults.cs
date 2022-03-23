using Dapper.Contrib.Extensions;
using System;
namespace RevoScada.Entities
{
    [Table("public.\"SkippedIntegratedCheckResults\"")]
    public class SkippedIntegratedCheckResult
    {
        [ExplicitKey]
        public int id { get; set; }
        public int BatchId { get; set; }
        public DateTime SkipDate { get; set; }
    }
}