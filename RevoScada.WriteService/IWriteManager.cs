﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.WriteService
{
    interface IWriteManager
    {
        bool Write(int plcDeviceId);
    }
}