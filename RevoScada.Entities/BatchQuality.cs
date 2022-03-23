using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{


    [Table("public.\"BatchQualities\"")]
    public class BatchQuality
    {
        [ExplicitKey]
        public int id { get; set; }
        public string CardName { get; set; }
        public string Description { get; set; }
        public DateTime LastModified { get; set; }
        public short SortOrder { get; set; }

    }
}
 