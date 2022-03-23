using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    public class ReportHeaderInfo
    {
        public int BatchId { get; set; }
        public string LoadNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public int CreatedByUserId { get; set; }

    }
}
