using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
    public class ReadResult
    {
        public int DbNumber { get; set; }
        public byte[] Result { get; set; }
        public int S7Result { get; set; }

        public DateTime DateTime { get; set; }
       
    }
}
