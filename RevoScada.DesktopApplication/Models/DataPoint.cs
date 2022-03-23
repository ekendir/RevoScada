using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class DataPoint
    {
        public double? YVal { get; set; }
        public string XVal { get; set; }
        public DateTime? Date { get; set; }
    }
}
