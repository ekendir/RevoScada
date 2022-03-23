using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
  public class SiemensWriteCommandItem
    {

        /// <summary>
        /// command id must be guid and 00000000000000000000000000000000 formatted
        /// </summary>
        public string CommandId { get; set; }
        public int PlcId { get; set; }
        public int DbNumber { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public byte[] Buffer { get; set; }
        public string Description { get; set; }
    }
}
