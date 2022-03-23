using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace RevoScada.SynchronizationService
{
    public partial class SynchronizationService : ServiceBase
    {
        private static Thread serviceInfiniteLoop;
        private string _startupConfigurationFile;
        private const string _serviceName = "RevoScadaSynchronizationService";

        private OperationCycle _operationCycle;

        public SynchronizationService()
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
                SyncServiceConfigurations.Instance.InitializeConfiguration(_startupConfigurationFile);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(_serviceName, $"Service stopped. Check configuration file location or format. {ex.Message}", EventLogEntryType.Error);
                Environment.Exit(0);
            }

            try
            {
                LogManager.Instance.InitializeConfiguration(SyncServiceConfigurations.Instance.SyncConfiguration.LogSettings);
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
            _operationCycle = new OperationCycle();
            
            try
            {
                _operationCycle.InitializeCycle();
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"InitializeCycle error {ex.Message}", LogType.Error);
            }

            LogManager.Instance.Log($"<<< Sync Service Cycle Started! >>>", LogType.Information);
            _operationCycle.RunCycle();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(_serviceName, $"Abort Started! (SyncService)", EventLogEntryType.Information);

            if (_operationCycle != null)
            {
                _operationCycle.AbortCycle();
            }

            if (!serviceInfiniteLoop.Join(10000))
                serviceInfiniteLoop.Abort();

            EventLog.WriteEntry(_serviceName, $"Abort Completed! (SyncService)", EventLogEntryType.Information);
        }

    }
}
