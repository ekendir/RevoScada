using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Alarm
{
    public class LastDBStatus
    {
        public string DBKey { get; set; }
        public DateTime LastUpdate { get; set; }
        public int DBNumber { get; set; }
        public int PlcId { get; set; }
    }
}
