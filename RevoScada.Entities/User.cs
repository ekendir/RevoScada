using Dapper.Contrib.Extensions;
using System;

namespace RevoScada.Entities
{
    [Table("public.\"Users\"")]
    public class User
    {
        [ExplicitKey]
        public int id { get; set; }
        public short GroupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public short LogoutTime { get; set; }
    }
}