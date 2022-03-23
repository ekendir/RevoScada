using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Enums
{
    public enum ActiveTagGroups: short
    {
        DefaultSensor=0,
        PTC=1,
        MON=2,
        VAC=3
    }
}
