using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.AlarmService
{
    interface IAlarmManager
    {
        bool GetAlarms(int plcDeviceId);
    }
}
