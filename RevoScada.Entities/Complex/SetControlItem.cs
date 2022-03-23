using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
    public class SetControlItem
    {
        /// <summary>
        /// command id must be guid and 00000000000000000000000000000000 formatted
        /// </summary>
        public string CommandId { get; set; }

        public int PlcDeviceId  { get; set; }
        public int DbNumber { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime CommandStartDate { get; set; }
        public DateTime CommandCompletedDate { get; set; }

    }
}
