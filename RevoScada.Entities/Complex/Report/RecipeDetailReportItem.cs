using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    public class RecipeDetailReportItem
    {
        public string RecipeName { get; set; }
        public string RecipeFieldValue { get; set; }
        public int SegmentNo { get; set; }
    }
}
