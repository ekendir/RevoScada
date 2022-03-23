using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class PermissionRepository : DapperGenericPostgreRepository<Permission>
    {
        public PermissionRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
