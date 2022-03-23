using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
    public class BagDetailDto
    {
        public int BagId { get; set; }
        public int BatchId { get; set; }
        public int SelectedPortTagId { get; set; }
        public string SelectedPortName { get; set; }
        public string BagName { get; set; }
    }
}
