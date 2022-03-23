using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.DataProcessing;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;
using System.Diagnostics;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class AlarmVM : UserControlBaseVM
    {
        #region Services
        private ApplicationPropertyService _applicationPropertyService;
        #endregion

        public Dictionary<int, SiemensTagConfiguration> AlarmTagConfigurations { get; private set; }
        public AlarmTagConfigurations AlarmPageTagConfigurations { get; private set; }

        private ObservableCollection<AlarmLogGridModel> _furnaceAlarmData;
        public ObservableCollection<AlarmLogGridModel> FurnaceAlarmData
        {
            get => _furnaceAlarmData;
            set
            {
                OnPropertyChanged(ref _furnaceAlarmData, value);
            }
        }

        private ObservableCollection<ProcessEventLogGridModel> _processEventLogs;
        public ObservableCollection<ProcessEventLogGridModel> ProcessEventLogs
        {
            get => _processEventLogs;
            set => OnPropertyChanged(ref _processEventLogs, value);
        }
        public string ProcessEventGridModelFilter { get; set; }

        #region Commands
        public ICommand FilterSystemEventCommand { get; set; }
        public ICommand SilenceHornCommand { get; set; }

        #endregion

        #region Fields
        private string _connectionString;
        private bool _isSelectAllFurnaceAlarms;
        #endregion

        public Dictionary<string, bool> _setAlarmCheckList;

        public bool IsEditingMode { get; set; }
        public bool IsSelectAllFurnaceAlarms
        {
            get => _isSelectAllFurnaceAlarms;
            set => OnPropertyChanged(ref _isSelectAllFurnaceAlarms, value);
        }
        public ValueWrapper<bool> _incomingAlarmsChecker;
        public ValueWrapper<bool> IncomingAlarmsChecker
        {
            get => _incomingAlarmsChecker;
            set => OnPropertyChanged(ref _incomingAlarmsChecker, value);
        }

        private ValueWrapper<bool> _alarmPageForceLoadOption;
        public ValueWrapper<bool> AlarmPageForceLoadOption
        {
            get => _alarmPageForceLoadOption;
            set => OnPropertyChanged(ref _alarmPageForceLoadOption, value);
        }

        public AlarmVM(WaitIndicatorControl waitIndicatorControl, ValueWrapper<bool> incomingAlarmsChecker, ValueWrapper<bool> alarmPageForceLoadOption)
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _applicationPropertyService = new ApplicationPropertyService(_connectionString);
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = false;
            WaitIndicatorControl.IsWaitIndicatorTextActive = false;
            IncomingAlarmsChecker = incomingAlarmsChecker;
            AlarmPageForceLoadOption = alarmPageForceLoadOption;

            FurnaceAlarmData = new ObservableCollection<AlarmLogGridModel>();
            ProcessEventLogs = new ObservableCollection<ProcessEventLogGridModel>();

            FilterSystemEventCommand = new RelayCommand(FilterSystemEventData);
            SilenceHornCommand = new RelayCommand(SilenceHorn);
            FurnaceAlarmData.CollectionChanged += FurnaceAlarmData_CollectionChanged;

            InitializePageTagConfigurations();

            AlarmTagConfigurations = ApplicationConfigurations.Instance.TagConfigurations.Where(x => ((SiemensTagConfiguration)x.Value).SiemensTagGroupId == 3 && ((SiemensTagConfiguration)x.Value).IsActive == true).ToDictionary(x => x.Key, x => (SiemensTagConfiguration)x.Value);
        }

        /// todo:l Delete these methods after UI related development finished
        #region Testing methods
        //private void LoadMockData()
        //{
        //    ProcessEventLogs.Clear();

        //    for (int i = 1; i <= 10; i++)
        //    {
        //        ProcessEventLogGridModel processEventLog = new ProcessEventLogGridModel()
        //        {
        //            EventText = $"Test {i}",
        //            CreateDate = DateTime.Now,
        //            Type = "Manual"
        //        };

        //        if (i == 3 || i == 4 || i == 6 || i == 7)
        //            processEventLog.Type = "System";

        //        ProcessEventLogs.Add(processEventLog);
        //    }

        //    if(FurnaceAlarmData.Count == 0)
        //    {
        //        for (int i = 1; i <= 10; i++)
        //        {
        //            var alarmLogGridModel = new AlarmLogGridModel()
        //            {
        //                AlarmName = $"Test {i}",
        //                Status = "I",
        //                AcknowledgedDateTime = DateTime.Now,
        //                OutDateTime = DateTime.Now,
        //                InDateTime = DateTime.Now,
        //                Id = i - 1,
        //                AlarmKey = $"Test0{i-1}",
        //            };

        //            if (i == 3 || i == 4)
        //                alarmLogGridModel.Status = "AIO";

        //            if (i == 7 || i == 8 || i == 9 || i == 10)
        //                alarmLogGridModel.Status = "IO";

        //            FurnaceAlarmData.Add(alarmLogGridModel);
        //        }
        //    }
        //}

        //public void AddNewAlarms()
        //{
        //    for (int i = 1; i <= 1; i++)
        //    {
        //        Random random = new Random();
        //        var alarmLogGridModel = new AlarmLogGridModel()
        //        {
        //            AlarmName = $"Test {random.Next(0, 20)}",
        //            Status = "I",
        //            AcknowledgedDateTime = DateTime.Now,
        //            OutDateTime = DateTime.Now,
        //            InDateTime = DateTime.Now,
        //            Id = i - 1,
        //            AlarmKey = $"Test0{i - 1}",
        //        };

        //        FurnaceAlarmData.Add(alarmLogGridModel);
        //    }
        //}

        public void ChangeInputAlarmsRandomly()
        {
            Random random = new Random();
            short randIndex = (short)random.Next(0, FurnaceAlarmData.Count);

            FurnaceAlarmData = FurnaceAlarmData.OrderByDescending(f => f.Status).ToObservableCollection();
            FurnaceAlarmData[randIndex].Status = "IO";
        }
        #endregion

        public void InitializePageTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Alarm");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            AlarmPageTagConfigurations = JsonConvert.DeserializeObject<AlarmTagConfigurations>(jsonSerializedString);
            AlarmPageTagConfigurations.ResetAlarms = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(AlarmPageTagConfigurations.ResetAlarms)];
            AlarmPageTagConfigurations.SlienceHorn = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(AlarmPageTagConfigurations.SlienceHorn)];
        }

        public void AcknowledgeReset()
        {
            SilenceHorn();
            _setAlarmCheckList = new Dictionary<string, bool>();

            List<AlarmLogGridModel> alarmGridModel = FurnaceAlarmData.Where(f => f.IsSelected == true).ToList();
           
            foreach (AlarmLogGridModel item in alarmGridModel)
            {
                bool setResult = AlarmManager.Instance.SetAlarm(item.AlarmKey);
                _setAlarmCheckList.Add(item.AlarmKey, setResult);
            }

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            Guid guid = Guid.NewGuid();
            plcCommandManager.Set((SiemensTagConfiguration)AlarmPageTagConfigurations.ResetAlarms, 1, guid);
            bool result = plcCommandManager.IsUpdatedResult(guid, false, 500);
          

            foreach (AlarmLogGridModel item in alarmGridModel)
            {
                int index = FurnaceAlarmData.IndexOf(FurnaceAlarmData.First(x => x.AlarmKey == item.AlarmKey));

                FurnaceAlarmData[index].IsSelected = _setAlarmCheckList.ContainsKey(item.AlarmKey) ? !_setAlarmCheckList[item.AlarmKey] : false;
            }
        }

        private async void SilenceHorn()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            Guid guid = Guid.NewGuid();
           
            await Task.Run(() =>
            {
                plcCommandManager.Set((SiemensTagConfiguration)AlarmPageTagConfigurations.SlienceHorn, true, guid);
            });

            await Task.Delay(2000);

            bool resultValueSet = plcCommandManager.IsUpdatedResult(guid, false, 200);
        }

        public void SelectOrDeselectAllFurnaceAlarms(bool val)
        {
            if (FurnaceAlarmData.Count == 0)
                return;

            IsSelectAllFurnaceAlarms = val;

            for (int i = 0; i < FurnaceAlarmData.Count; i++)
            {
                FurnaceAlarmData[i].IsSelected = IsSelectAllFurnaceAlarms;
            }
        }

        public void UpdateAlarmPageForceLoadOption(bool val)
        {
            AlarmPageForceLoadOption.Value = val;
            _applicationPropertyService.UpdateByName("AlarmPageForceLoadOption", val.ToString());
        }

        private void FurnaceAlarmData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_furnaceAlarmData.Count == 0)
                return;

            if (!_furnaceAlarmData.Where(f => f.IsSelected).Any())
                IsSelectAllFurnaceAlarms = false;

            if (_furnaceAlarmData.Where(f => f.IsSelected).Count() == _furnaceAlarmData.Count)
                IsSelectAllFurnaceAlarms = true;
        }

        private void FilterSystemEventData(object param)
        {
            ProcessEventGridModelFilter = ((string)param).ToLower();
            UpdateEvents();
        }

        public void UpdatePlcAlarms()
        {
            AlarmLogGridModel alarmLogGridModel = null;

            if (FurnaceAlarmData.Count() == 0)
            {
               var plcAlarmsSorted = AlarmManager.Instance.PlcAlarms().ToList().OrderBy(x => (PlcAlarmStatusType)Enum.Parse(typeof(PlcAlarmStatusType), x.Status)).ThenByDescending(x => x.InDateTime);

                foreach (var plcAlarmItem in plcAlarmsSorted)
                {
                    SiemensTagConfiguration siemensTagConfiguration = AlarmTagConfigurations[plcAlarmItem.TagConfigurationId];
                    alarmLogGridModel = new AlarmLogGridModel()
                    {
                        AlarmName = siemensTagConfiguration.TagName,
                        Status = plcAlarmItem.Status,
                        AcknowledgedDateTime = plcAlarmItem.AcknowledgedDateTime,
                        OutDateTime = plcAlarmItem.OutDateTime,
                        InDateTime = plcAlarmItem.InDateTime,
                        Id = plcAlarmItem.TagConfigurationId,
                        AlarmKey = plcAlarmItem.AlarmKey
                    };
                    FurnaceAlarmData.Add(alarmLogGridModel);
                }
            }
            else
            {
                 var plcAlarmsSorted = AlarmManager.Instance.PlcAlarms().ToList().OrderBy(x => (PlcAlarmStatusType) Enum.Parse(typeof(PlcAlarmStatusType), x.Status)).ThenByDescending(x => x.InDateTime);

                foreach (var plcAlarmItem in plcAlarmsSorted)
                 {
                     if (!IsEditingMode)
                     {
                        SiemensTagConfiguration siemensTagConfiguration = AlarmTagConfigurations[plcAlarmItem.TagConfigurationId];

                        alarmLogGridModel = new AlarmLogGridModel()
                        {
                            AlarmName = siemensTagConfiguration.TagName,
                            Status = plcAlarmItem.Status,
                            AcknowledgedDateTime = plcAlarmItem.AcknowledgedDateTime,
                            OutDateTime = plcAlarmItem.OutDateTime,
                            InDateTime = plcAlarmItem.InDateTime,
                            Id = plcAlarmItem.TagConfigurationId,
                            AlarmKey = plcAlarmItem.AlarmKey
                        };

                        AlarmLogGridModel alarmLogGridModelToUpdate = FurnaceAlarmData.FirstOrDefault(x => x.AlarmKey == plcAlarmItem.AlarmKey);

                        if (alarmLogGridModelToUpdate ==null)
                        {
                            FurnaceAlarmData.Add(alarmLogGridModel);
                            FurnaceAlarmData = FurnaceAlarmData.OrderBy(x => (PlcAlarmStatusType)Enum.Parse(typeof(PlcAlarmStatusType), x.Status)).ThenByDescending(x => x.InDateTime).ToObservableCollection();
                        }
                        else
                        {
                            int index = FurnaceAlarmData.IndexOf(alarmLogGridModelToUpdate);
                            alarmLogGridModel.IsSelected = alarmLogGridModelToUpdate.IsSelected;
                            alarmLogGridModel.HasWarned = alarmLogGridModelToUpdate.HasWarned;
                            FurnaceAlarmData[index] = alarmLogGridModel;
                        }
                     }
                 }
            }
        }

        public void UpdateEvents()
        {
            int batchId = ProcessManager.Instance.CurrentProcess.BatchId;

            if (batchId == 0)
            {
                ProcessEventLogs = new ObservableCollection<ProcessEventLogGridModel>();
            }
            else
            {
                ProcessEventLogs.Clear();
                ProcessEventLogService processEventLogService = new ProcessEventLogService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                IEnumerable<ProcessEventLog> processEventLogs = processEventLogService.GetByBatchId(batchId);

                foreach (var processEvent in processEventLogs)
                {
                    ProcessEventLogGridModel processEventGridModel = new ProcessEventLogGridModel()
                    {
                        BatchId = processEvent.BatchId,
                        CreateDate = processEvent.CreateDate,
                        EventText = processEvent.EventText,
                        id = processEvent.id,
                        ModifiedByUserId = processEvent.ModifiedByUserId,
                        Type = processEvent.Type
                    };

                    ProcessEventLogs.Add(processEventGridModel);
                }
            }

            switch (ProcessEventGridModelFilter)
            {
                case "all":
                    ProcessEventLogs = ProcessEventLogs.ToObservableCollection();
                    break;
                case "system":
                case "manual":
                    ProcessEventLogs = ProcessEventLogs.Where(x => x.Type.ToLower() == ProcessEventGridModelFilter).ToObservableCollection();
                    break;
            }
        }
    }
}
