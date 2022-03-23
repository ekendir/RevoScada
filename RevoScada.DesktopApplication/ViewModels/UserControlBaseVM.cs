using System;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Helpers;
using System.Collections.Generic;
using RevoScada.Entities;
using System.Collections.ObjectModel;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.ProcessController;
using Newtonsoft.Json;
using Revo.Core;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class UserControlBaseVM : ObservableObject
    {
        private WaitIndicatorControl _waitIndicatorControl;
        public WaitIndicatorControl WaitIndicatorControl
        {
            get => _waitIndicatorControl;
            set => OnPropertyChanged(ref _waitIndicatorControl, value);
        }

        private string _disabledControlTooltipText;
        public string DisabledControlTooltipText 
        {
            get
            {
                _disabledControlTooltipText = "You have no permission to use this control!\n(Bu kontrolü kullanmak için izniniz yoktur!)";
                return _disabledControlTooltipText;
            }
            set
            {
                _disabledControlTooltipText = value;
            }
        }

        public Dictionary<string, bool> Permissions { get; set; }
        private ApplicationLanguageSettings _applicationLanguageSettings;
        public ApplicationLanguageSettings ApplicationLanguageSettings 
        {
            get
            {
                try
                {
                    string appLangSettings = ProcessManager.Instance.ApplicationProperties["ApplicationLanguageSettings"].Value;
                    _applicationLanguageSettings = JsonConvert.DeserializeObject<ApplicationLanguageSettings>(appLangSettings);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"ApplicationLanguageSettings property may not be found in ApplicationProperties table: {ex.Message}\n\n", LogType.Error);
                    throw ex;
                }

                return _applicationLanguageSettings;
            }
            set
            {
                _applicationLanguageSettings = value;
            }
        }
        public UserGridModel ActiveUser { get; set; }

        protected DateTime LastInvokedCommandTime { get; set; }
        protected bool AllowTimerRun
        {
            get
            {
                if (LastInvokedCommandTime == default)
                {
                    return true;
                }
                else
                {
                    int diffInSeconds = Convert.ToInt32((DateTime.Now - LastInvokedCommandTime).TotalMilliseconds / 1000);
                    return (diffInSeconds > 2);
                }
            }
        }

       
        public int PlcDeviceId { get { return ApplicationConfigurations.Instance.Configuration.PlcDevice.Id; } }

       
    }
}
