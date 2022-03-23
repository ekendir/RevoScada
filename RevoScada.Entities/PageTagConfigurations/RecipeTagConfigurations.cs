using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class RecipeTagConfigurations
    {
        public int PlcDevice { get; set; }
        public int Dbnumber { get; set; }
        public int IncrementAmount { get; set; }
        public int Length { get; set; }
        public int ActiveBatchSegmentNo { get; set; }
        public int BufferSizeofRecipe { get; set; }
    }
}
