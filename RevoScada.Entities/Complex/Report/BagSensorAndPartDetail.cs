using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
   public class BagSensorAndPartDetail
    {
        public int BagId { get; set; }
        public string BagName { get; set; }
        public List<LotProperty> LotProperties { get; set; }
        public BagSensors BagSensors { get; set; }



    }


}
