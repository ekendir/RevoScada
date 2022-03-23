using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"ApplicationProperties\"")]
    public class ApplicationProperty
    {
        [ExplicitKey]
        public int id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}


 