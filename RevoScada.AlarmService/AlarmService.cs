using System;
using Revo.Core;
using System.Threading;
using System.Diagnostics;
using System.ServiceProcess;
using RevoScada.Configurator;

namespace RevoScada.AlarmService
{
    public partial class AlarmService : ServiceBase
    {
        private static Thread serviceInfiniteLoop;
        private string _startupConfigurationFile;
        private const string _serviceName = "RevoScadaAlarmService";

        private static CycleOperationContext _cycleOperationContext = null;

        public AlarmService()
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

                AlarmServiceConfigurations.Instance.InitializeConfiguration(_startupConfigurationFile);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(_serviceName, $"Service stopped. Check configuration file location or format. {ex.Message}", EventLogEntryType.Error);
                Environment.Exit(0);
            }

            try
            {
                LogManager.Instance.InitializeConfiguration(AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.LogSettings);
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
                _cycleOperationContext.InitializeCycle((PlcType)Enum.Parse(typeof(PlcType), AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.PlcType));
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"InitializeCycle error {ex.Message}", LogType.Error);
            }

            LogManager.Instance.Log($"<<< Alarm Service Cycle Started! >>>", LogType.Information);
            _cycleOperationContext.RunCycle();

        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(_serviceName, $"Abort Started! (Alarm Service)", EventLogEntryType.Information);

            if (_cycleOperationContext != null)
            {
                _cycleOperationContext.AbortCycle();
            }

            if (!serviceInfiniteLoop.Join(10000))
                serviceInfiniteLoop.Abort();

            EventLog.WriteEntry(_serviceName, $"Abort Completed! (Alarm Service)", EventLogEntryType.Information);
        }
    }
}

