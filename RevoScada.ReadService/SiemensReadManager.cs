using System;
using System.Linq;
using Newtonsoft.Json;
using RevoScada.PlcAccess;
using System.Collections.Generic;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Complex.Alarm;
using RevoScada.Cache;
using Revo.Core;

namespace RevoScada.ReadService
{
    class SiemensReadManager : IReadManager
    {
        private readonly CacheManager _cacheManager;

        private readonly CacheManager _mainCacheManager;

        public SiemensReadManager(string redisServer)
        {
            _cacheManager = new CacheManager(CacheDBType.ReadService, redisServer);

            _mainCacheManager = new CacheManager(CacheDBType.Main, redisServer);
        }


        /// <summary>
        /// Retrieves siemens datablocks by siemens read request items
        /// </summary>
        /// <param name="plcDeviceId">Siemens plc device id</param>
        /// <param name="readRequestItems">Read request item from tag configurations</param>
        /// <returns></returns>
        public bool Read(int plcDeviceId, object readRequestItems)
        {
            List<SiemensReadRequestItem> siemensReadRequestItems = (List<SiemensReadRequestItem>)readRequestItems;

            DateTime currentDateTime = DateTime.Now;
            ReadServiceState readServiceState = new ReadServiceState
            {
                LastCycleRunTime = currentDateTime,
                PlcId = plcDeviceId
            };

            List<LastDBStatus> lastDBStatuses = new List<LastDBStatus>();
            SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();
            var readResults = siemensPlcAccess.GetAllDB(plcDeviceId, siemensReadRequestItems, 20);
            readServiceState.GetAllDBCount = readResults?.Count ?? 0;
            List<bool> setResults = new List<bool>();

            if (readResults!=null && readResults.Count()>0)
            {
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
                            PlcId = plcDeviceId
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

                return true;
            }

            return false;
        }

    }
}
