using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"PageTagConfigurations\"")]
    public class PageTagConfiguration
    {
        [ExplicitKey]
        public short id { get; set; }
        public string  PageName { get; set; }
        public object PageTagConfigurations { get; set; }
    }
}