using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
   public class BatchDetailReportItem
    {
        public string LoadNumber    { get; set; }
        public DateTime StartDate     { get; set; }
        public DateTime EndDate       { get; set; }
        public int RecipeId      { get; set; }
        public string BagName       { get; set; }
        public int[] SelectedPorts { get; set; }
        public int Bagid { get; set; }

    }
}
