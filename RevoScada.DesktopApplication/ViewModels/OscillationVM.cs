using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class OscillationVM : UserControlBaseVM// ObservableObject
    {
        #region Commands
        public ICommand ActionCommand { get; set; }

        #endregion

        #region Properties
        private ObservableCollection<OscillationCriteriaModel> _oscillationCriterias;
        public ObservableCollection<OscillationCriteriaModel> OscillationCriterias
        {
            get => _oscillationCriterias;
            set => OnPropertyChanged(ref _oscillationCriterias, value);
        }


        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }

        private Visibility _pressureValueVisibility;
        public Visibility PressureValueVisibility
        {
            get => _pressureValueVisibility;
            set => OnPropertyChanged(ref _pressureValueVisibility, value);
        }

        

        #endregion

        #region Fields

        public Oscillation Oscillation_View;
        #endregion

        #region Collections
        private Dictionary<OscillationCriteriaNames, OscillationCriteria> _oscillationTagConfigurations;
        #endregion

        public OscillationVM(WaitIndicatorControl waitIndicatorControl)
        {
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            ActionCommand = new RelayCommand(SetActionToPlc);

            _oscillationTagConfigurations = new Dictionary<OscillationCriteriaNames, OscillationCriteria>();

            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;

            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                case 2:
                case 20:
                    PressureValueVisibility = Visibility.Visible;
                    break;
                case 3:
                    PressureValueVisibility = Visibility.Collapsed;
                    break;
            }

            
        }

        private OscillationTagConfigurations OscillationTagConfigurations;
        private void InitializePageTagConfigurations()
        {
            //UserControlBaseVM userControlBaseVM = new UserControlBaseVM();

            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Oscillation");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            OscillationTagConfigurations = JsonConvert.DeserializeObject<OscillationTagConfigurations>(jsonSerializedString);

            foreach (var item in OscillationTagConfigurations.OscillationCriterias)
            {
                item.Action = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Action)];
                item.ToleranceValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.ToleranceValue)];
                item.SensorFaultCount = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.SensorFaultCount)];
                item.CheckDurationInMs = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.CheckDurationInMs)];
                OscillationCriteriaNames oscillationCriteria = (OscillationCriteriaNames)Enum.Parse(typeof(OscillationCriteriaNames), item.Type + item.Order, true);
                _oscillationTagConfigurations[oscillationCriteria] = item;
            }
            SetOscillationDatablock(true);

        }
        public bool SetOscillationDatablock(bool value)
        {
            bool result = false;
            if (OscillationTagConfigurations.DbNumbers != null)
            {
                for (int i = 0; i < OscillationTagConfigurations.DbNumbers.Count; i++)
                {
                    result = ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, OscillationTagConfigurations.DbNumbers[i], value);
                }
            }
            return result = false;
        }

        public void ContinuousUpdate()
        {

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            if (OscillationCriterias == null)
            {
                InitializePageTagConfigurations();

                OscillationCriterias = new ObservableCollection<OscillationCriteriaModel>();

                foreach (var item in _oscillationTagConfigurations)
                {
                    OscillationCriteriaModel oscillationCriteriaModel = new OscillationCriteriaModel();

                    oscillationCriteriaModel.OscillationCriteriaNames = item.Key;
                    oscillationCriteriaModel.Action = plcCommandManager.Get<int>((SiemensTagConfiguration)item.Value.Action, false);
                    oscillationCriteriaModel.CheckDurationInMs = plcCommandManager.Get<int>((SiemensTagConfiguration)item.Value.CheckDurationInMs, false);
                    oscillationCriteriaModel.SensorFaultCount = plcCommandManager.Get<int>((SiemensTagConfiguration)item.Value.SensorFaultCount, false);
                    oscillationCriteriaModel.ToleranceValue = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.ToleranceValue, false);

                    OscillationCriterias.Add(oscillationCriteriaModel);
                }

                WaitIndicatorControl.IsWaitIndicatorVisible = false;
            }
            else
            {
                if (AllowTimerRun)
                {
                    foreach (var item in _oscillationTagConfigurations)
                    {
                        OscillationCriteriaModel oscillationCriteriaModel = new OscillationCriteriaModel();

                        oscillationCriteriaModel.OscillationCriteriaNames = item.Key;
                        oscillationCriteriaModel.Action = plcCommandManager.Get<int>((SiemensTagConfiguration)item.Value.Action, false);
                        oscillationCriteriaModel.CheckDurationInMs = plcCommandManager.Get<int>((SiemensTagConfiguration)item.Value.CheckDurationInMs, false);
                        oscillationCriteriaModel.SensorFaultCount = plcCommandManager.Get<int>((SiemensTagConfiguration)item.Value.SensorFaultCount, false);
                        oscillationCriteriaModel.ToleranceValue = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.ToleranceValue, false);


                        OscillationCriterias[(int)item.Key] = oscillationCriteriaModel;


                    }
                }
            }
        }

        private async void SetActionToPlc(object param)
        {
            LastInvokedCommandTime = DateTime.Now;

            if (Oscillation_View == null)
                return;

            Oscillation_View.IsControlsEditingMode = true;

            string valueArray = (string)param;

            int commandType = Convert.ToInt32(valueArray.Split('-')[0]);

            OscillationCriteriaNames oscillationCriteria = (OscillationCriteriaNames)Convert.ToInt32(valueArray.Split('-')[1]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            Guid guid = Guid.NewGuid();
            plcCommandManager.Set((SiemensTagConfiguration)_oscillationTagConfigurations[oscillationCriteria].Action, commandType, guid);

            bool result = await plcCommandManager.IsUpdatedResultAsync(guid, false, 1000);


            if (result == false)
                WinUIMessageBox.Show("Güncelleme işlemi başarısız! Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            Oscillation_View.IsControlsEditingMode = false;

        }

        public async Task<bool> SetOscillationValueToPLC(OscillationCriteriaModel oscillationCriteriaModel, string propertyName)
        {
            LastInvokedCommandTime = DateTime.Now;


            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration();
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            Guid guid = Guid.NewGuid();

            switch (propertyName)
            {
                case "ToleranceValue":
                    siemensTagConfiguration = (SiemensTagConfiguration)_oscillationTagConfigurations[oscillationCriteriaModel.OscillationCriteriaNames].ToleranceValue;
                    plcCommandManager.Set(siemensTagConfiguration, oscillationCriteriaModel.ToleranceValue, guid);
                    break;
                case "SensorFaultCount":
                    siemensTagConfiguration = (SiemensTagConfiguration)_oscillationTagConfigurations[oscillationCriteriaModel.OscillationCriteriaNames].SensorFaultCount;
                    plcCommandManager.Set(siemensTagConfiguration, oscillationCriteriaModel.SensorFaultCount, guid);
                    break;
                case "CheckDurationInMs":
                    siemensTagConfiguration = (SiemensTagConfiguration)_oscillationTagConfigurations[oscillationCriteriaModel.OscillationCriteriaNames].CheckDurationInMs;
                    plcCommandManager.Set(siemensTagConfiguration, oscillationCriteriaModel.CheckDurationInMs, guid);
                    break;
            }

            bool result = await plcCommandManager.IsUpdatedResultAsync(guid, false);

            return result;
        }
    }
}
