using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
   public class BatchDetailLotPropertiesReportItem
    {
      public int BagId      {get;set;}
      public string  SoirNumber {get;set;}
      public string  PartName   {get;set;}
      public string ToolName { get; set; }
    }
}
