using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Revo.Core.Data;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Types;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities.Complex.Alarm;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using Revo.Core;
using RevoScada.Cache;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class FurnaceSelectorVM : ObservableObject
    {
        private SyncStateManager _syncStateManager;
        private WorkingEnvironment _workingEnvironment;
        private ObservableCollection<FurnaceSelectionModel> _furnaceSelectionModels;
        private CacheManager _mainCacheManager;
        private StringManipulation _stringManipulation;
        private OSInfoProvider oSInfoProvider;

        public ObservableCollection<FurnaceSelectionModel> FurnaceSelectionModels
        {
            get => _furnaceSelectionModels;
            set => OnPropertyChanged(ref _furnaceSelectionModels, value);
        }

        public Dictionary<int, SiemensPlcConfig> PlcConfigs { get; set; }

        public FurnaceSelectorVM()
        {
            _furnaceSelectionModels = new ObservableCollection<FurnaceSelectionModel>();
            _syncStateManager = new SyncStateManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            _workingEnvironment = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment;
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(ApplicationConfigurations.Instance.Configuration.SqliteConnectionString);
            PlcConfigs = siemensPlcConfigService.GetActiveFurnaceConfigurations().ToDictionary(x => x.PlcDeviceId, x => x);
            _mainCacheManager = new CacheManager(CacheDBType.Main, ApplicationConfigurations.Instance.Configuration.RedisServer);
            _stringManipulation = new StringManipulation();
            oSInfoProvider = new OSInfoProvider();
            FurnaceSelectionModels = new ObservableCollection<FurnaceSelectionModel>();
        }


        public void RefreshFurnaces()
        {

            Dictionary<string, SyncItem> keyValuePairs = _syncStateManager.GetSyncItemsFromCache();

            foreach (var furnaceItem in ApplicationConfigurations.Instance.Configuration.Furnaces)
            {
                ReadServiceState readServiceState = new ReadServiceState();

                try
                {
                    string readServiceStateSerialized = _mainCacheManager.GetString($"ReadServiceStatePLC{furnaceItem.Value.Id}");

                    if (!string.IsNullOrEmpty( readServiceStateSerialized))
                    {
                        readServiceState = JsonConvert.DeserializeObject<ReadServiceState>(readServiceStateSerialized);
                    }
                    else
                    {
                        readServiceState = new ReadServiceState
                        {
                            GetAllDBCount = 0,
                            LastCycleRunTime = DateTime.Now.AddDays(-100),
                            PlcId = furnaceItem.Value.Id
                        };
                    }
                 
                }
                catch (Exception ex)
                {
                    readServiceState = new ReadServiceState
                    {
                        GetAllDBCount = 0,
                        LastCycleRunTime = DateTime.Now.AddDays(-100),
                        PlcId = furnaceItem.Value.Id
                    };
                }

                bool isValidMaster;

                DateTime plcLastAccessDateFromPC = DateTime.MinValue;
                DateTime plcLastAccessDateFromServer = DateTime.MinValue;

                try
                {
                    isValidMaster = _syncStateManager.IsValidMaster(furnaceItem.Value.Id, _workingEnvironment);
                    plcLastAccessDateFromPC = keyValuePairs[$"SyncItem{WorkingEnvironment.pc}PLC{furnaceItem.Value.Id}"]?.LastAccessDateToPLC ?? DateTime.MinValue ;
                    plcLastAccessDateFromServer = keyValuePairs[$"SyncItem{WorkingEnvironment.server}PLC{furnaceItem.Value.Id}"]?.LastAccessDateToPLC ?? DateTime.MinValue;
                }
                catch (Exception ex)
                {

                    isValidMaster = false;
                }

                FurnaceSelectionModel furnaceSelectionModel = FurnaceSelectionModels.FirstOrDefault(x => x.PlcDeviceId == furnaceItem.Value.Id);

                if (furnaceSelectionModel == null)
                {
                    furnaceSelectionModel = new FurnaceSelectionModel
                    {
                        FurnaceName = furnaceItem.Value.FurnaceName,
                        PlcDeviceId = furnaceItem.Value.Id,
                        LastUpTime = DateTime.Now,
                        PLCLastAccessDateFromPC = plcLastAccessDateFromPC,
                        PLCLastAccessDateFromServer = plcLastAccessDateFromServer,
                        SyncStatus = false,
                        ImagePath = furnaceItem.Value.ImagePath,
                        Description = furnaceItem.Value.Description,
                        PlcIpAddress = PlcConfigs[furnaceItem.Value.Id].Ip,
                        RunEnable = isValidMaster,
                        LastCycleRunTime = readServiceState.LastCycleRunTime,
                        OSUptime = oSInfoProvider.UpTimeLiteral
                    };

                    FurnaceSelectionModels.Add(furnaceSelectionModel);
                }
                else
                {
                    int index = FurnaceSelectionModels.IndexOf(furnaceSelectionModel);
                    furnaceSelectionModel = new FurnaceSelectionModel
                    {
                        FurnaceName = furnaceItem.Value.FurnaceName,
                        PlcDeviceId = furnaceItem.Value.Id,
                        LastUpTime = DateTime.Now,
                        PLCLastAccessDateFromPC = plcLastAccessDateFromPC,
                        PLCLastAccessDateFromServer = plcLastAccessDateFromServer,
                        SyncStatus = false,
                        ImagePath = furnaceSelectionModel.ImagePath,
                        Description = furnaceItem.Value.Description,
                        PlcIpAddress = PlcConfigs[furnaceItem.Value.Id].Ip,
                        RunEnable = isValidMaster,
                        LastCycleRunTime = readServiceState.LastCycleRunTime,
                        OSUptime = oSInfoProvider.UpTimeLiteral
                    };
                    FurnaceSelectionModels[index] = furnaceSelectionModel;
                }
            }
        }
    }
}
