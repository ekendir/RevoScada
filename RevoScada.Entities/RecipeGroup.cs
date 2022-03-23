using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"RecipeGroups\"")]
    public class RecipeGroup
    {
        [ExplicitKey]
        public short id { set; get; }
        public string GroupName { set; get; }
        public bool IsActive { set; get; }
    }
}