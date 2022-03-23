using RevoScada.Entities.Configuration.Service;
using Serilog;
using System;
using System.IO;

namespace Revo.Core
{
    public sealed class LogManager 
    {
        private ILogger Logger;
        
        private static readonly Lazy<LogManager> lazy = new Lazy<LogManager>(() => new LogManager());
   
        public static LogManager Instance => lazy.Value;

        private LogManager()
        {

        }

        public void InitializeConfiguration(LogSettings logSettings)
        {
            try
            {
                switch (logSettings.LogType.ToLower())
                {
                    case "file":
                        RollingInterval rollingInterval = (RollingInterval)Enum.Parse(typeof(RollingInterval), logSettings.RollingInterval);
                        Logger = new LoggerConfiguration().WriteTo.File(Path.Combine(logSettings.ApplicationLogRoot ,"applicationlog.log"), rollingInterval: rollingInterval).CreateLogger();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public string LogPrefixText { set; get; } = string.Empty;

        public void Log(string text,LogType logType)
        {

            switch (logType)    
            {
                case LogType.Information:
                    Logger.Information(LogPrefixText  +text);
                    break;
                case LogType.Warning:
                    Logger.Warning(LogPrefixText + text);
                    break;
                case LogType.Error:
                    Logger.Error(LogPrefixText  + text);
                    break;
                case LogType.Fatal:
                    Logger.Fatal(LogPrefixText  + text);
                    break;
                case LogType.Debug:
                    Logger.Debug(LogPrefixText  + text);
                    break;
            }
        }
    }

    public enum LogType { 
    Information,Warning,Error,Fatal, Debug
    }
}
