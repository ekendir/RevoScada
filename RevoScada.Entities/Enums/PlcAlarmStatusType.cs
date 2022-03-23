using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Enums
{
    public enum PlcAlarmStatusType
    {
       I,IO,AI,AIO
    }
}

// Alarm gelmiş I,
// Alarm gelmiş, Acknowledge edilmemiş, plc de gitmiş IO
// Alarm gelmiş ve Acknowledge edilmiş A-I
// Alarm gelmiş, Acknowledge edilmiş, plc de gitmiş A-IO
