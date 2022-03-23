using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Entities.Configuration;

namespace RevoScada.PlcConnection.Siemens
{

    /// <summary>
    /// Singleton object for single connection per device
    /// </summary>
    public sealed class SingleWriteConnectionManager : SingleConnectionManagerBase
    {

        private static readonly Lazy<SingleWriteConnectionManager> lazy = new Lazy<SingleWriteConnectionManager>(() => new SingleWriteConnectionManager());

        public static SingleWriteConnectionManager Instance => lazy.Value;

        private SingleWriteConnectionManager()
        {
            _clients = new ConcurrentDictionary<int, S7Client>();

            _plcDevicesIsConnected = new ConcurrentDictionary<int, bool>();

            SiemensPlcConfigs = new List<SiemensPlcConfig>();

        }
    }
}