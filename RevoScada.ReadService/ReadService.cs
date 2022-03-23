using Revo.Core;
using Revo.ServiceUtilities;
using RevoScada.Cache;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.ReadService
{
    public partial class ReadService : ServiceBase
    {
        private static Thread serviceInfiniteLoop;
        private string _startupConfigurationFile;
        private const string _serviceName = "RevoScadaReadService";

        private static  CycleOperationContext  _cycleOperationContext = null;

        public ReadService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs != null && commandLineArgs.Length > 2 && commandLineArgs[2] == "debug")
            {
#if DEBUG
                if (Debugger.IsAttached == false) Debugger.Launch();
#endif
            }

            try
            {
                _startupConfigurationFile = Environment.GetCommandLineArgs()[1];
                ReadConfigurations.Instance.InitializeConfiguration(_startupConfigurationFile);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(_serviceName, $"Service stopped. Check configuration file location or format. {ex.Message}", EventLogEntryType.Error);
                Environment.Exit(0);
            }

            try
            {
                LogManager.Instance.InitializeConfiguration(ReadConfigurations.Instance.ReadServiceConfiguration.LogSettings);
                LogManager.Instance.Log("service running...", LogType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(_serviceName, $"Service stopped. Check configuration file location or format for Logging. {ex.Message}", EventLogEntryType.Error);
                Environment.Exit(0);
            }
            serviceInfiniteLoop = new Thread(DoWork)
            {
                IsBackground = true
            };
            serviceInfiniteLoop.Start();
        }

        private void DoWork()
        {

            _cycleOperationContext = new CycleOperationContext();

            try
            {
                LogManager.Instance.Log($"Trying to connect...This may take several minutes!", LogType.Information);
                _cycleOperationContext.ConnectDevices((PlcType)Enum.Parse(typeof(PlcType), ReadConfigurations.Instance.ReadServiceConfiguration.PlcType));
                LogManager.Instance.Log($"Plc Connection established for all devices!", LogType.Information);
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Service connection error {ex.Message}", LogType.Error);
            }

            try
            {
                _cycleOperationContext.InitializeCycle();
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"InitializeCycle error {ex.Message}", LogType.Error);
            }

            LogManager.Instance.Log($"<<< Read Service Cycle Started! >>>", LogType.Information);
            _cycleOperationContext.RunCycle();
            
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(_serviceName, $"Abort Started! (ReadService)", EventLogEntryType.Information);
            
            if (_cycleOperationContext != null)
            {
                _cycleOperationContext.AbortCycle();
            }

            if (!serviceInfiniteLoop.Join(10000))
                serviceInfiniteLoop.Abort();

            EventLog.WriteEntry(_serviceName, $"Abort Completed! (ReadService)", EventLogEntryType.Information);
        }
    }
}
