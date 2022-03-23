using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.ProcessController
{
    public sealed class AlarmManager
    {
        private static readonly Lazy<AlarmManager> lazy = new Lazy<AlarmManager>(() => new AlarmManager());
        public static AlarmManager Instance => lazy.Value;
        private   CacheManager _mainCacheManager;
        private ApplicationConfiguration _applicationConfiguration;
        private int _plcDeviceId;
        private AlarmManager()
        {

        }

        public void Initialize(ApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
           _mainCacheManager = new CacheManager(CacheDBType.Main, _applicationConfiguration.RedisServer);
        }

        public void InitializeSelectedDevice(int plcDeviceId)
        {
            _plcDeviceId = plcDeviceId;
        }

        public List<PlcAlarm> PlcAlarms()
        {
            List<string> alarmKeys = _mainCacheManager.GetKeyNames($"alarm_plc{_plcDeviceId}*");
            List<PlcAlarm> plcAlarms = new List<PlcAlarm>();
            try
            {
                foreach (string alarmKey in alarmKeys)
                {
                    string serializedPlcAlarmItem = _mainCacheManager.GetString(alarmKey);
                    PlcAlarm alarmItem = JsonConvert.DeserializeObject<PlcAlarm>(serializedPlcAlarmItem);
                    alarmItem.OutDateTime = ((alarmItem.OutDateTime ?? null) is null) ? null : (alarmItem.OutDateTime.Value.Equals(DateTime.MinValue) ? null : alarmItem.OutDateTime);
                    alarmItem.AcknowledgedDateTime = ((alarmItem.AcknowledgedDateTime ?? null) is null) ? null : (alarmItem.AcknowledgedDateTime.Value.Equals(DateTime.MinValue) ? null : alarmItem.AcknowledgedDateTime);
                    plcAlarms.Add(alarmItem);
                }
            }
            catch (Exception)
            {
            }
            return plcAlarms;
        }

        public bool SetAlarm(string alarmKey)
        {
            bool resultValueSet = false;
            bool beginResult = _mainCacheManager.SetBeginLockInfo($"AlarmSetLockedPLC{_plcDeviceId}", tryAmount: 10, tryCycleIntervalInMiliseconds: 500);

            try
            {
                if (beginResult)
                {
                    string serializedPlcAlarmItem = _mainCacheManager.GetString(alarmKey);
                    PlcAlarm alarmItem = JsonConvert.DeserializeObject<PlcAlarm>(serializedPlcAlarmItem);
                    alarmItem.AcknowledgedDateTime = DateTime.Now;

                    if (alarmItem.Status == PlcAlarmStatusType.I.ToString())
                    {
                        alarmItem.Status = PlcAlarmStatusType.AI.ToString();
                    }

                    if (alarmItem.Status == PlcAlarmStatusType.IO.ToString())
                    {
                        alarmItem.Status = PlcAlarmStatusType.AIO.ToString();
                    }

                    string serializedPlcAlarmItemToSet = JsonConvert.SerializeObject(alarmItem);
                    _mainCacheManager.Set(alarmKey, serializedPlcAlarmItemToSet);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Incoming alarm set error: {alarmKey} Error: {ex.Message}\n", LogType.Information);
            }
            bool endResult = _mainCacheManager.SetEndLockInfo($"AlarmSetLockedPLC{_plcDeviceId}");
            return resultValueSet;
        }
        public bool ResetPlcAlarms()
        {
            bool result;
            try
            {
                List<bool> resultCheckList = new List<bool>();
                List<string> alarmKeys = _mainCacheManager.GetKeyNames($"alarm_plc{_plcDeviceId}*");
                foreach (var item in alarmKeys)
                {
                    bool deleteItemResult = _mainCacheManager.DeleteKey(item);
                    resultCheckList.Add(deleteItemResult);
                }
                result = resultCheckList.TrueForAll(x => x == true);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public int AlarmCount()
        {
            List<string> alarmKeys = _mainCacheManager.GetKeyNames($"alarm_plc{_plcDeviceId}*");
            int alarmCount = alarmKeys?.Count ?? 0;
            return alarmCount;
        }

        private int _incomingAlarmCount;

        public int IncomingAlarmCount()
        {
            List<PlcAlarm> plcAlarms = PlcAlarms().Where(x => x.Status == PlcAlarmStatusType.I.ToString()).ToList();
            IsNewAlarmOccured = (plcAlarms.Count > _incomingAlarmCount) ? true : IsNewAlarmOccured;
            _incomingAlarmCount = plcAlarms.Count;
            return plcAlarms.Count();
        }

        public bool IsNewAlarmOccured { get; set; }
    }

}

    /*
    public class AlarmManager
    {

        private readonly CacheManager _mainCacheManager;

        private Dictionary<string, ReadResult> _cacheReadResults;

        private PlcCommandManager _plcCommandManager;

        public AlarmManager()
        {
            _mainCacheManager = new CacheManager(CacheDBType.Main, _applicationConfigurations.DesktopApplicationConfiguration.RedisServer);

            _cacheReadResults = new Dictionary<string, ReadResult>();

            _plcCommandManager = new PlcCommandManager();

        }

    }
    */

