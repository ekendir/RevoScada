using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class DataLogGridModel
    {
        public DateTime ReceivedDate { get; set; }
        public float TagValue { get; set; }
        public string TagName { get; set; }
    }
}
