using Revo.SiemensDrivers.Sharp7;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace RevoScada.PlcConnection.Siemens
{
    /// <summary>
    /// Singleton object for single connection per device
    /// </summary>
    public sealed class SingleReadConnectionManager : SingleConnectionManagerBase
    {

        private static readonly Lazy<SingleReadConnectionManager> lazy = new Lazy<SingleReadConnectionManager>(() => new SingleReadConnectionManager());
        
        
        public static SingleReadConnectionManager Instance => lazy.Value;

        private SingleReadConnectionManager()
        {
            _clients = new ConcurrentDictionary<int, S7Client>();

            _plcDevicesIsConnected = new ConcurrentDictionary<int, bool>();

            SiemensPlcConfigs = new List<SiemensPlcConfig>();

        }
    }
}