using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.ReadService
{
    interface IReadManager
    {
        bool Read(int plcDeviceId, object readRequestItems);
    }
}
