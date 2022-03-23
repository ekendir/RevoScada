using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class ReportBatchGridModel
    {
        public int BagId { get; set; }
        public string BagName { get; set; }
        public string PartTc { get; set; }
        public string Monitor { get; set; }
        public string Src { get; set; }
        public string SoirName { get; set; }
        public string PartName { get; set; }
        public string ToolName { get; set; }
    }
}
