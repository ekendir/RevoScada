using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.WindowsUI;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.ViewModels;

namespace RevoScada.DesktopApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "RevoScadaDesktopApplication";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);
            
            base.OnStartup(e);

            string startArgument = e.Args.Length > 0 ? e.Args[0].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(startArgument) && startArgument == "ResetUserSettings=true")
            {
                if (!string.IsNullOrEmpty(DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath))
                {
                    DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath = string.Empty;
                    DesktopApplication.Properties.Settings.Default.Save();
                }
            }
            else if (!string.IsNullOrEmpty(startArgument) && startArgument == "RestartAction")
            {
                Thread.Sleep(3);
            }
            else if (!createdNew)
            {
                Current.Shutdown();
            }


            try
            {
                if(string.IsNullOrEmpty(DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath))
                {
                    ScadaSettings scadaSettings = new ScadaSettings();
                    scadaSettings.ShowDialog();
                    EventLog.WriteEntry("RevoScadaTAIDesktopApplication", $"InitialConfigurationFilePath has been set!", EventLogEntryType.Information);
                    Thread.CurrentThread.Abort();
                }
                else
                {
                    try
                    {

                        if (File.Exists(DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath))
                        {
                            //EventLog.WriteEntry("RevoScadaTAIDesktopApplication", $"InitialConfigurationFilePath: {DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath}", EventLogEntryType.Information);
                            ApplicationConfigurations.Instance.InitializeConfiguration(DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath, false);
                            ProcessManager.Instance.Initialize(ApplicationConfigurations.Instance.Configuration, ApplicationConfigurations.Instance.TagConfigurations);
                            AlarmManager.Instance.Initialize(ApplicationConfigurations.Instance.Configuration);
                            LogManager.Instance.InitializeConfiguration(ApplicationConfigurations.Instance.Configuration.LogSettings);
                            RegisterExceptionHandler();
                        }
                        else
                        {
                            ScadaSettings scadaSettings = new ScadaSettings();
                            scadaSettings.ShowDialog();

                            EventLog.WriteEntry("RevoScadaTAIDesktopApplication", $"InitialConfigurationFilePath has been set!", EventLogEntryType.Information);
                            Thread.CurrentThread.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry("RevoScadaTAIDesktopApplication", $"\n-------------------\nApplication couldnt be started. Detail: {ex.Message}\n\n-----------------", EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("RevoScadaTAIDesktopApplication", $"\n-------------------\nApplication couldnt be started. Detail: {ex.Message}\n\n-----------------", EventLogEntryType.Error);
            }
        }

        private void RegisterExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                HandlerException(e.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (sender, e) =>
            {
                e.Handled = true;
                HandlerException(e.Exception);
            };
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                HandlerException(e.Exception);
            };
        }

        private void HandlerException(Exception exception)
        {
            EventLog.WriteEntry("RevoScadaTAIDesktopApplication", $"\n-------------------\nApplication couldnt be started. Detail: {exception.Message}\n\n-----------------", EventLogEntryType.Error);
            LogManager.Instance.Log("\n<Application Crash Error Log>\n " + exception.ToString() + "\n<Application Crash Error Log\\>", LogType.Fatal);
            WinUIMessageBox.Show("Kritik düzeyde hata oluştu! Lütfen uygulamayı tekrar başlatın!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Error);

            // check plc last time
            // check services 
            // hepsi ok ise devam değilse kapat
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Set deactive specific datablocks
            foreach (var item in ProcessManager.Instance.GetOnDemandKeyNames())
            {
                ProcessManager.Instance.ChangeDemandReadStateOnCache(item, false);
            }
        }
    }
}
