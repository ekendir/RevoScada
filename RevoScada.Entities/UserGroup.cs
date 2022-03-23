using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"UserGroups\"")]
    public class UserGroup
    {
        [ExplicitKey]
        public short id { get; set; }
        public string GroupName { get; set; }
        public int[] PermissionIds { get; set; }
        public bool IsActive { get; set; }
    }
}