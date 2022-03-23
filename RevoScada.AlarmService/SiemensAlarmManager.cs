using System;
using Newtonsoft.Json;
using RevoScada.PlcAccess;
using RevoScada.Entities.Complex;
using RevoScada.Cache;
using Revo.Core;
using System.Threading;
using RevoScada.Business;
using System.Collections.Generic;
using RevoScada.Configurator;
using System.Text;
using System.Linq;
using RevoScada.Entities;
using RevoScada.Entities.Enums;
using RevoScada.Entities.Configuration;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Entities.Complex.Alarm;

namespace RevoScada.AlarmService
{
    class SiemensAlarmManager : IAlarmManager
    {

        private CacheManager _mainCacheManager;
        private CacheManager _readCacheManager;
        public SiemensAlarmManager(string redisServer)
        {

            // todo:m refactor cache manager initialization occurs too many times <--
            _mainCacheManager = new CacheManager(CacheDBType.Main, AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
            _readCacheManager = new CacheManager(CacheDBType.ReadService, AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
        }


        /// <summary>
        /// Retrieves siemens datablocks by siemens read request items
        /// </summary>
        /// <param name="plcDeviceId">Siemens plc device id</param>
        /// <param name="readRequestItems">Read request item from tag configurations</param>
        /// <returns></returns>
        public bool GetAlarms(int plcDeviceId)
        {
            SiemensTagConfiguration batchRunningInfoTag = (SiemensTagConfiguration)AlarmServiceConfigurations.Instance.BatchRunningInfoTags[plcDeviceId];
            SiemensTagConfiguration batchFinishInfoTag = (SiemensTagConfiguration)AlarmServiceConfigurations.Instance.BatchFinishInfoTags[plcDeviceId];

            //Cache key came from batchRunningInfoTag or batchFinishInfoTag are the same.
            string serializedReadResultBuffer = _readCacheManager.GetString($"PLC{batchRunningInfoTag.PlcId}DB{batchRunningInfoTag.DBNumber}");
            ReadResult readResult = JsonConvert.DeserializeObject<ReadResult>(serializedReadResultBuffer);

            int offsetIntegral;
            int offsetDecimal;

            offsetIntegral = (Convert.ToInt32(Math.Floor(batchRunningInfoTag.Offset)));
            offsetDecimal = Convert.ToInt32((batchRunningInfoTag.Offset - Math.Floor(batchRunningInfoTag.Offset)) * 10);
            bool isProcessRunning = S7.GetBitAt(readResult.Result, offsetIntegral, offsetDecimal);

            offsetIntegral = (Convert.ToInt32(Math.Floor(batchFinishInfoTag.Offset)));
            offsetDecimal = Convert.ToInt32((batchFinishInfoTag.Offset - Math.Floor(batchFinishInfoTag.Offset)) * 10);
            bool isProcessFinished = S7.GetBitAt(readResult.Result, offsetIntegral, offsetDecimal);
            int alarmDbNumber = ((SiemensTagConfiguration)AlarmServiceConfigurations.Instance.AlarmTagConfigurations[plcDeviceId].Values.First()).DBNumber;
            short[] alarmDbNumbers = AlarmServiceConfigurations.Instance.AlarmTagConfigurations[plcDeviceId].Values.Select(x => ((SiemensTagConfiguration)x).DBNumber).ToArray();
            
            int alarmPlcId = ((SiemensTagConfiguration)AlarmServiceConfigurations.Instance.AlarmTagConfigurations[plcDeviceId].Values.First()).PlcId;
            CurrentProcessInfo currentProcessInfo = null;

            try
            {
                currentProcessInfo = JsonConvert.DeserializeObject<CurrentProcessInfo>(_mainCacheManager.GetString($"CurrentProcessInfoPLC{plcDeviceId}"));
            }
            catch (Exception ex)
            {
                return false;
            }

            Dictionary<int, ReadResult> readResultList = new Dictionary<int, ReadResult>();
            foreach (var item in alarmDbNumbers)
            {
                string serializedAlarmBufferJson = _readCacheManager.GetString($"PLC{alarmPlcId}DB{item}");
                ReadResult alarmReadResult = JsonConvert.DeserializeObject<ReadResult>(serializedAlarmBufferJson);
                readResultList[item] = alarmReadResult;
            }

            bool beginResult = _mainCacheManager.SetBeginLockInfo($"AlarmSetLockedPLC{plcDeviceId}", 20, 200);

            try
            {
                foreach (var alarmTagConfigurationItem in AlarmServiceConfigurations.Instance.AlarmTagConfigurations[plcDeviceId].Where(x => ((SiemensTagConfiguration)x.Value).PlcId == plcDeviceId))
                {
                    SiemensTagConfiguration siemensAlarmTagConfigurationItem = (SiemensTagConfiguration)alarmTagConfigurationItem.Value;
                    offsetIntegral = (Convert.ToInt32(Math.Floor(siemensAlarmTagConfigurationItem.Offset)));
                    offsetDecimal = Convert.ToInt32((siemensAlarmTagConfigurationItem.Offset - Math.Floor(siemensAlarmTagConfigurationItem.Offset)) * 10);
                    
                    bool alarmState = S7.GetBitAt(readResultList[siemensAlarmTagConfigurationItem.DBNumber].Result, offsetIntegral, offsetDecimal);
                    string keyNameRoot = $"alarm_plc{siemensAlarmTagConfigurationItem.PlcId}tagid{siemensAlarmTagConfigurationItem.Id}";
                    List<string> keynames = _mainCacheManager.GetKeyNames($"{keyNameRoot}*");

                    if (keynames.Count > 0)
                    {

                        Dictionary<string, PlcAlarm> plcAlarmsGrouped = new Dictionary<string, PlcAlarm>();
                        PlcAlarm plcAlarm;

                        foreach (var keyname in keynames)
                        {
                            string serializedPlcAlarm = _mainCacheManager.GetString(keyname);
                            plcAlarm = JsonConvert.DeserializeObject<PlcAlarm>(serializedPlcAlarm);
                            plcAlarmsGrouped.Add(keyname, plcAlarm);
                        }

                        int maxOrder = plcAlarmsGrouped.Max(x => x.Value.Order);
                        plcAlarm = new PlcAlarm();
                        plcAlarm = plcAlarmsGrouped.First(x => x.Value.Order == maxOrder).Value;
                        plcAlarm.PlcValue = alarmState;
                        plcAlarm.TagConfigurationId = siemensAlarmTagConfigurationItem.Id;

                        string key = string.Empty;

                        if (beginResult)
                        {
                            if (alarmState)
                            {

                                switch (Enum.Parse(typeof(PlcAlarmStatusType), plcAlarm.Status))
                                {
                                    case PlcAlarmStatusType.I:
                                        break;

                                    case PlcAlarmStatusType.IO:
                                    case PlcAlarmStatusType.AI:
                                    case PlcAlarmStatusType.AIO:

                                        int newOrder = (plcAlarmsGrouped.Max(x => x.Value.Order) + 1);
                                        key = $"alarm_plc{siemensAlarmTagConfigurationItem.PlcId}tagid{siemensAlarmTagConfigurationItem.Id}_{newOrder}";

                                        plcAlarm = new PlcAlarm
                                        {
                                            InDateTime = DateTime.Now,
                                            Status = "I",
                                            TagConfigurationId = siemensAlarmTagConfigurationItem.Id,
                                            BatchId = currentProcessInfo.BatchId,
                                            Order = newOrder,
                                            AlarmKey = key
                                        };

                                        LogManager.Instance.Log($"Alarm State: {alarmState}\tKey: {key} Status Change: {plcAlarm.Status} --> I", LogType.Information);

                                        break;
                                }
                            }
                            else
                            {
                                key = plcAlarm.AlarmKey;

                                switch (Enum.Parse(typeof(PlcAlarmStatusType), plcAlarm.Status))
                                {
                                    case PlcAlarmStatusType.I:

                                        plcAlarm.Status = PlcAlarmStatusType.IO.ToString();
                                        plcAlarm.OutDateTime = DateTime.Now;

                                        break;
                                    case PlcAlarmStatusType.IO:
                                        // 
                                        break;
                                    case PlcAlarmStatusType.AI:
                                        plcAlarm.Status = PlcAlarmStatusType.AIO.ToString();
                                        plcAlarm.OutDateTime = DateTime.Now;
                                        break;
                                    case PlcAlarmStatusType.AIO:
                                        //
                                        break;
                                }
                            }

                            string serializedUpdatedPlcAlarm = JsonConvert.SerializeObject(plcAlarm);
                            _mainCacheManager.Set(key, serializedUpdatedPlcAlarm);

                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (beginResult)
                        {
                            string keyNameNew = $"alarm_plc{siemensAlarmTagConfigurationItem.PlcId}tagid{siemensAlarmTagConfigurationItem.Id}_{1}";

                            if (alarmState) // set cache for first time alarm state true
                            {
                                PlcAlarm plcAlarm = new PlcAlarm
                                {
                                    InDateTime = DateTime.Now,
                                    Status = "I",
                                    TagConfigurationId = siemensAlarmTagConfigurationItem.Id,
                                    BatchId = currentProcessInfo.BatchId,
                                    Order = 1,
                                    AlarmKey = keyNameNew
                                };

                                string serializedPlcAlarmForSet = JsonConvert.SerializeObject(plcAlarm);

                                _mainCacheManager.Set(keyNameNew, serializedPlcAlarmForSet);

                                LogManager.Instance.Log($"Alarm State: {alarmState}\tKey: {keyNameNew} PlcAlarm={serializedPlcAlarmForSet}", LogType.Information);
                            }

                        }
                        else
                        {
                            continue;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Incoming alarm set error: {ex.Message}", LogType.Information);
            }

            bool endResult = _mainCacheManager.SetEndLockInfo($"AlarmSetLockedPLC{plcDeviceId}");

            if ((isProcessRunning == false && isProcessFinished == true) || (isProcessRunning == false && isProcessFinished == false)) // Stopped
            {
                var serializedSaveAlarmOrder = _mainCacheManager.GetString($"AlarmSaveOrderPLC{plcDeviceId}");

                AlarmSaveOrder alarmSaveOrder = null;

                if (serializedSaveAlarmOrder == null)
                {
                    return true;
                }
                else
                {
                    alarmSaveOrder = JsonConvert.DeserializeObject<AlarmSaveOrder>(serializedSaveAlarmOrder);
                }

                if (!alarmSaveOrder.IsAlarmSaved)
                {
                    List<string> alarmKeys = _mainCacheManager.GetKeyNames("alarm_*");
                    PlcAlarmService plcAlarmService = new PlcAlarmService(AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.PostgreSqlConnectionStrings[plcDeviceId]);

                    int tryAmount = alarmKeys.Count() * 3;
                    int alarmKeyIteration = 0;

                    while (alarmKeys.Count() > 0)
                    {
                        if (tryAmount == 0)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine($"ProcessManager:Run:Last alarm state couldn't be saved properly. Unsaved PlcAlarm for batch id: {currentProcessInfo.BatchId}:");

                            alarmKeys = _mainCacheManager.GetKeyNames("alarm_*");

                            foreach (var unsavedAlarmKey in alarmKeys)
                            {
                                string unsavedSerializedAlarm = _mainCacheManager.GetString(unsavedAlarmKey);
                                stringBuilder.AppendLine($"{unsavedAlarmKey} {unsavedSerializedAlarm} ");
                            }

                            LogManager.Instance.Log(stringBuilder.ToString(), LogType.Error);

                            break;
                        }

                        string serializedPlcAlarm = _mainCacheManager.GetString(alarmKeys[alarmKeyIteration]);

                        PlcAlarm plcAlarm = JsonConvert.DeserializeObject<PlcAlarm>(serializedPlcAlarm);

                        bool insertResult = plcAlarmService.Insert(plcAlarm);

                        if (insertResult)
                        {

                            _mainCacheManager.DeleteKey(alarmKeys[alarmKeyIteration]);

                            alarmKeys.Remove(alarmKeys[alarmKeyIteration]);

                            LogManager.Instance.Log($"Alarm data saved: Tag Id: {plcAlarm.TagConfigurationId}", LogType.Information);

                        }
                        else
                        {
                            Thread.Sleep(20);
                        }

                        tryAmount--;

                    }

                    if (tryAmount > 0)
                    {
                        _mainCacheManager.DeleteKey($"AlarmSaveOrderPLC{plcDeviceId}");
                        LogManager.Instance.Log($"{alarmSaveOrder.SaveOrderId} AlarmSaveOrder completed!", LogType.Information);
                    }
                }
            }

            return true;
        }

    }
}
