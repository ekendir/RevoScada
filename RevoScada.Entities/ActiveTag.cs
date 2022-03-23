using Dapper.Contrib.Extensions;
using RevoScada.Entities.Enums;

namespace RevoScada.Entities
{
    [Table("public.\"ActiveTags\"")]
    public class ActiveTag
    {
        [ExplicitKey]
        public int id { get; set; }
        public string  TagName { get; set; }
        public bool IsLogData { get; set; }
        public ActiveTagGroups ActiveTagGroupId { get; set; }
    }
}
