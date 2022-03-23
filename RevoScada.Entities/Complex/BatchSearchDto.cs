using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
    public class BatchSearchDto
    {
        
        public int id { get; set; }
        public string LoadNumber { get; set; }

        public string  RecipeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Revision { get; set; }

    }
}
