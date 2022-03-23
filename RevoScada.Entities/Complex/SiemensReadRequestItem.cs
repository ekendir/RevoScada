using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
    public class SiemensReadRequestItem
    {
     public int Id { get; set; }
     public string DataType { get; set; }
     public int DbNumber { get; set; }
     public float ComputedSize { get; set; }
     public bool IsDemanded { get; set; }

    }

}
