using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class TrendModel
    {
        public float? YVal { get; set; }
        public double Minute { get; set; }
        public string SeriesName { get; set; }
        public DateTime? Date { get; set; }
    }
}
