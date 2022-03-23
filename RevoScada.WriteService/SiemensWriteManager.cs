using System;
using Newtonsoft.Json;
using RevoScada.PlcAccess;
using RevoScada.Entities.Complex;
using RevoScada.Cache;
using Revo.Core;
using System.Threading;
namespace RevoScada.WriteService
{
    class SiemensWriteManager : IWriteManager
    {
        private readonly CacheManager _writeCacheManager;

        private readonly CacheManager _mainCacheManager;

        public SiemensWriteManager(string redisServer)
        {
            _writeCacheManager = new CacheManager(CacheDBType.WriteService, redisServer);

            _mainCacheManager = new CacheManager(CacheDBType.Main, redisServer);
        }


        /// <summary>
        /// Retrieves siemens datablocks by siemens read request items
        /// </summary>
        /// <param name="plcDeviceId">Siemens plc device id</param>
        /// <param name="readRequestItems">Read request item from tag configurations</param>
        /// <returns></returns>
        public bool Write(int plcDeviceId)
        {
            CacheResponse cacheResponse = null;

            do
            {
                cacheResponse = _writeCacheManager.ListRightPop($"SetCommandQueuePLC{plcDeviceId}", 10);

                if (cacheResponse != null && cacheResponse.CacheResponseState == CacheResponseStates.Success)
                {
                    SiemensWriteCommandItem writeCommandItem = JsonConvert.DeserializeObject<SiemensWriteCommandItem>(Convert.ToString(cacheResponse.ResultValue));
                    LogManager.Instance.Log($"{writeCommandItem.CommandId} item dequeued!", LogType.Information);
                    SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();
                    WriteResult writeResult = siemensPlcAccess.WriteDB(writeCommandItem, 10);

                    if (writeResult != null && writeResult.IsSucceeded)
                    {
                        UpdateSetControlItem(writeCommandItem.CommandId);
                        LogManager.Instance.Log($"{writeCommandItem.Description}", LogType.Information);
                        continue;
                    }
                    else
                    {
                        var writeCommandItemJson = JsonConvert.SerializeObject(writeCommandItem);
                        int tryAmount = 500;

                        do
                        {
                            if (tryAmount == 0)
                            {
                                LogManager.Instance.Log($"Unmanaged set operation occured! The {writeCommandItem.CommandId} item couldn't be rollbacked!", LogType.Fatal);
                                break;
                            }
                            tryAmount--;

                            CacheResponse cacheResponsePushRight = _writeCacheManager.ListRightPushString($"SetCommandQueuePLC{writeCommandItem.PlcId}", writeCommandItemJson, 10);
                            if (cacheResponsePushRight.CacheResponseState == CacheResponseStates.EmergencyError)
                            {
                                LogManager.Instance.Log($"The {writeCommandItem.CommandId} item couldn't be rollbacked! {cacheResponsePushRight.Message}", LogType.Fatal);
                                Thread.Sleep(5000);
                            }
                            else
                            {
                                LogManager.Instance.Log($"The {writeCommandItem.CommandId} item has been rollbacked! {cacheResponsePushRight.Message}", LogType.Fatal);
                                break;
                            }
                        } while (true);
                    }
                }
                else
                {
                    break;
                }

            } while (true);

            
            /*
                    List<SiemensReadRequestItem> siemensReadRequestItems = (List<SiemensReadRequestItem>)readRequestItems;
                   
                    DateTime currentDateTime = DateTime.Now;
                    ReadServiceState readServiceState = new ReadServiceState
                    {
                        LastCycleRunTime = currentDateTime,
                        PlcId= plcDeviceId
                    };

                    List<LastDBStatus> lastDBStatuses = new List<LastDBStatus>();
                    SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();
                    var readResults = siemensPlcAccess.GetAllDB(plcDeviceId, siemensReadRequestItems, 20);
                    readServiceState.GetAllDBCount = readResults?.Count ?? 0;
                    List<bool> setResults = new List<bool>();
                    try
                    {
                        foreach (ReadResult readResultItem in readResults)
                        {
                            readResultItem.DateTime = DateTime.Now;
                            string dbKey = $"PLC{plcDeviceId}DB{readResultItem.DbNumber}";
                            var readResultSerialized = JsonConvert.SerializeObject(readResultItem);
                            bool setResult = _cacheManager.Set(dbKey, readResultSerialized, null);
                            setResults.Add(setResult);

                            LastDBStatus lastDBStatus = new LastDBStatus
                            {
                                DBKey = dbKey,
                                DBNumber = readResultItem.DbNumber,
                                LastUpdate = currentDateTime,
                                PlcId= plcDeviceId
                            };

                              lastDBStatuses.Add(lastDBStatus);
                        }

                        var readServiceStateJsonString = JsonConvert.SerializeObject(readServiceState);
                        var setReadServiceState = _mainCacheManager.Set<string>($"ReadServiceStatePLC{plcDeviceId}", readServiceStateJsonString, null);
                        foreach (var lastDBStatus in lastDBStatuses)
                        {
                            var jsonString = JsonConvert.SerializeObject(lastDBStatus);
                            var setReadServiceDBStatus = _mainCacheManager.Set<string>($"LastDBStatus_" + lastDBStatus.DBKey, jsonString, TimeSpan.FromDays(5));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                   #if DEBUG
                       bool isSetResultsSucceeded = (setResults.Count > 0) ? !setResults.Contains(false) : false;
                       LogManager.Instance.Log($"PLC {plcDeviceId}: {setResults.Where(x => x == true).Count()}/{ readResults?.Count ?? 0 } success! Cache Success:{isSetResultsSucceeded}", LogType.Information);
                   #endif
            */
            return true;
        }

        public bool UpdateSetControlItem(string commandId)
        {
            SetControlItem setControlItem = new SetControlItem();
            string setControlSerialized = _mainCacheManager.GetString($"SetControl_{commandId}");
            setControlSerialized = "null".Equals(setControlSerialized) ? null : setControlSerialized;

            if (!string.IsNullOrEmpty(setControlSerialized))
            {
                setControlItem = JsonConvert.DeserializeObject<SetControlItem>(setControlSerialized);
                setControlItem.CommandCompletedDate = DateTime.Now;
                setControlItem.IsCompleted = true;
                setControlSerialized = JsonConvert.SerializeObject(setControlItem);
                bool setControlSetResult = _mainCacheManager.Set($"SetControl_{commandId}", setControlSerialized, TimeSpan.FromDays(1));
                return setControlSetResult;
            }
            return false;
        }

    }
}
