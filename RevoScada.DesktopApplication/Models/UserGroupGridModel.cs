using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class UserGroupGridModel
    {
        public short id { get; set; }
        public string GroupName { get; set; }
        public int[] PermissionIds{ get; set; }
        public string PermissionNames { get; set; }
        public bool IsActive { get; set; }
    }
}
