using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Configuration.Service
{
    public class LogSettings
    {
        public string LogType { get; set; }
        public string ApplicationLogRoot { get; set; }
        public string DbConnectionString { get; set; }
        public string RollingInterval { get; set; }
    }

}
