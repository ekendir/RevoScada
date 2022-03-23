using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"Permissions\"")]
    public class Permission
    {
        [ExplicitKey]
        public int id { get; set; }
        public string PageName { get; set; }
        public short PermissionGroup { get; set; }
        public string PermissionTag { get; set; }
        public string Description { get; set; }
    }
}