using System;
using Revo.Core;
using Newtonsoft.Json;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Cache;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Complex.Alarm;
using RevoScada.Entities.Configuration;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RevoScada.ProcessController
{
    public class PlcCommandManager
    {
        private readonly CacheManager _readCacheManager;
        private readonly CacheManager _writeCacheManager;
        private readonly CacheManager _mainCacheManager;

        private ReadResult _cacheReadResult;

        private readonly Dictionary<string, ReadResult> _cacheReadResults;

        public PlcCommandManager(string cacheServer)
        {
            _readCacheManager = new CacheManager(CacheDBType.ReadService, cacheServer);
            _writeCacheManager = new CacheManager(CacheDBType.WriteService, cacheServer);
            _mainCacheManager = new CacheManager(CacheDBType.Main, cacheServer);

            _cacheReadResults = new Dictionary<string, ReadResult>();
        }

        public bool Set<T>(SiemensTagConfiguration siemensTagConfiguration, T value, Guid setCommandId = default(Guid))
        {
            int offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
            int offsetDecimal = Convert.ToInt32((siemensTagConfiguration.Offset - Math.Floor(siemensTagConfiguration.Offset)) * 10);
            byte[] setBuffer = null;
            int dataSize = 0;

            try
            {
                switch (siemensTagConfiguration.DataType.ToLower())
                {
                    case "bool":
                        var readResult = _readCacheManager.GetString($"PLC{siemensTagConfiguration.PlcId}DB{siemensTagConfiguration.DBNumber}");
                        var readResultDeserialized = JsonConvert.DeserializeObject<ReadResult>(readResult);
                        byte[] dbBuffer = readResultDeserialized.Result;
                        dataSize = 1;
                        //First get whole byte to update any bit
                        byte readByte = S7.GetByteAt(dbBuffer, offsetIntegral);
                        setBuffer = new byte[1] { readByte };
                        S7.SetBitAt(ref setBuffer, 0, offsetDecimal, Convert.ToBoolean(value));
                        break;

                    case "byte":
                        byte castedByte = Convert.ToByte(value);
                        dataSize = 1;
                        setBuffer = new byte[1];
                        S7.SetByteAt(setBuffer, 0, castedByte);
                        break;
                    case "int":
                        /// Int is short equivalent of int clr
                        short castedInt = Convert.ToInt16(value);
                        dataSize = 2;
                        setBuffer = new byte[dataSize];
                        S7.SetIntAt(setBuffer, 0, castedInt);
                        break;
                    case "real":
                        float castedFloat = (float)Convert.ToDouble(value);
                        dataSize = 4;
                        setBuffer = new byte[dataSize];
                        S7.SetRealAt(setBuffer, 0, castedFloat);
                        break;
                    case string typeName when typeName.Contains("string"):
                        string castedString = value.ToString();
                        dataSize = Convert.ToByte(System.Text.RegularExpressions.Regex.Match(siemensTagConfiguration.DataType, @"\d+").ToString()) + 2;
                        setBuffer = new byte[dataSize];
                        S7.SetStringAt(setBuffer, 0, dataSize, castedString);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"{ex.Message}", LogType.Fatal);
                return false;
            }

            string commandId = (setCommandId == default) ? Guid.NewGuid().ToString("N") : setCommandId.ToString("N");
            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = commandId,
                DbNumber = siemensTagConfiguration.DBNumber,
                Offset = offsetIntegral,
                PlcId = siemensTagConfiguration.PlcId,
                Size = dataSize,
                Buffer = setBuffer,
                Description = $"{commandId} " +
                $"PLC:{ siemensTagConfiguration.PlcId } " +
                $"DB:{siemensTagConfiguration.DBNumber} " +
                $"Offset:{offsetIntegral}.{offsetDecimal} " +
                $"Size:{dataSize} " +
                $"Value:{ value} " +
                $"item set to plc!"
            };
            var siemensWriteCommandItemSerialized = JsonConvert.SerializeObject(siemensWriteCommandItem);
            var leftpushResult = _writeCacheManager.ListLeftPushString($"SetCommandQueuePLC{siemensTagConfiguration.PlcId}", siemensWriteCommandItemSerialized, 10);
            AddSetControlItem(commandId, siemensTagConfiguration.PlcId, siemensTagConfiguration.DBNumber);
            return (leftpushResult.CacheResponseState == CacheResponseStates.Success);
        }

        public void SetToBuffer<T>(SiemensTagConfiguration siemensTagConfiguration, byte[] buffer, T value)
        {
            int offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
            int offsetDecimal = Convert.ToInt32((siemensTagConfiguration.Offset - Math.Floor(siemensTagConfiguration.Offset)) * 10);

            try
            {
                switch (siemensTagConfiguration.DataType.ToLower())
                {
                    case "bool":
                        S7.SetBitAt(ref buffer, offsetIntegral, offsetDecimal, Convert.ToBoolean(value));
                        break;
                    case "byte":
                        //todo:l  implement
                        break;
                    case "int":
                        short castedInt = Convert.ToInt16(value);
                        S7.SetIntAt(buffer, offsetIntegral, castedInt);
                        break;
                    case "real":
                        //todo:l  implement SetToBuffer real
                        break;
                    case string typeName when typeName.Contains("string"):
                        //todo:l  implement SetToBuffer string
                        string castedString = value.ToString();
                        int dataSize = Convert.ToByte(System.Text.RegularExpressions.Regex.Match(siemensTagConfiguration.DataType, @"\d+").ToString()) + 2;
                        S7.SetStringAt(buffer, offsetIntegral, dataSize, castedString);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"PlcCommandManager: {ex}", LogType.Error);
            }
        }

        public byte[] GetAllBuffer(int plcId, int dbNumber)
        {
            var readResult = _readCacheManager.GetString($"PLC{plcId}DB{dbNumber}");
            _cacheReadResult = JsonConvert.DeserializeObject<ReadResult>(readResult);
            return _cacheReadResult.Result;
        }


        /// <summary>
        /// empty the buffer in local
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public byte[] ResetBuffer(byte[] buffer)
        {
            for (int i = 0; i < buffer.Count(); i++)
            {
                buffer[i] = 0;
            }
            return buffer;
        }

        public bool SetBufferToPLC(byte[] buffer, int plcId, int dbNumber)
        {
            string commandId = Guid.NewGuid().ToString("N");
            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = commandId,
                DbNumber = dbNumber,
                Offset = 0,
                PlcId = plcId,
                Size = buffer.Length,
                Buffer = buffer,
                Description = $"{commandId} " +
                $"PLC:{ plcId } " +
                $"DB:{dbNumber} " +
                $"Offset:0.0 " +
                $"Size:{buffer.Length} " +
                $"Value:- " +
                $" Bulk buffer inserted!"
            };

            var siemensWriteCommandItemSerialized = JsonConvert.SerializeObject(siemensWriteCommandItem);
            var leftpushResult = _writeCacheManager.ListLeftPushString($"SetCommandQueuePLC{plcId}", siemensWriteCommandItemSerialized, 10);
            return (leftpushResult.CacheResponseState == CacheResponseStates.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="siemensTagConfiguration"></param>
        /// <param name="getAlwaysUpdatedResult">if you call many times in a moment send false for fast result. otherwise it gets byte array from cache for every call</param>
        /// <returns></returns>
        public T Get<T>(SiemensTagConfiguration siemensTagConfiguration, bool getAlwaysUpdatedResult, [CallerMemberName] string propertyName = null)
        {
            object result = null;
            string bufferCacheKey = $"PLC{ siemensTagConfiguration.PlcId}DB{ siemensTagConfiguration.DBNumber}";

            if (getAlwaysUpdatedResult)
            {
                var readResult = _readCacheManager.GetString($"PLC{siemensTagConfiguration.PlcId}DB{siemensTagConfiguration.DBNumber}");
                _cacheReadResult = JsonConvert.DeserializeObject<ReadResult>(readResult);

                if (_cacheReadResults.ContainsKey(bufferCacheKey))
                {
                    _cacheReadResults[bufferCacheKey] = _cacheReadResult;
                }
                else
                {
                    _cacheReadResults.Add(bufferCacheKey, _cacheReadResult);
                }
            }
            else
            {

                if (_cacheReadResults.ContainsKey(bufferCacheKey))
                {
                    _cacheReadResult = _cacheReadResults[bufferCacheKey];
                }
                else
                {
                    byte tryAmount = 10;

                    do
                    {
                        if (tryAmount == 0)
                        {
                            break;
                        }

                        var readResult = _readCacheManager.GetString($"PLC{siemensTagConfiguration.PlcId}DB{siemensTagConfiguration.DBNumber}");

                        if (readResult != null)
                        {
                            _cacheReadResult = JsonConvert.DeserializeObject<ReadResult>(readResult);
                            _cacheReadResults.Add(bufferCacheKey, _cacheReadResult);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(20);
                            tryAmount--;
                        }

                    } while (true);
                }
            }

            int offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
            int offsetDecimal = Convert.ToInt32((siemensTagConfiguration.Offset - Math.Floor(siemensTagConfiguration.Offset)) * 10);

            try
            {
                switch (siemensTagConfiguration.DataType.ToLower())
                {
                    case "bool":
                        result = S7.GetBitAt(_cacheReadResults[bufferCacheKey].Result, offsetIntegral, offsetDecimal);
                        break;

                    case "byte":
                        result = S7.GetByteAt(_cacheReadResults[bufferCacheKey].Result, offsetIntegral);
                        break;
                    case "int":
                        result = S7.GetIntAt(_cacheReadResults[bufferCacheKey].Result, offsetIntegral);
                        break;

                    case "real":
                        result = S7.GetRealAt(_cacheReadResults[bufferCacheKey].Result, offsetIntegral);
                        break;

                    case string typeName when typeName.Contains("string"):
                        result = S7.GetStringAt(_cacheReadResults[bufferCacheKey].Result, offsetIntegral);
                        break;
                }

            }
            catch (Exception ex)
            {

                LogManager.Instance.Log($"PlcCommandManager: {ex} {bufferCacheKey}:{offsetIntegral}:{offsetDecimal}", LogType.Fatal);

                return default(T);

            }
            return (T)Convert.ChangeType(result, typeof(T));
        }




        /// <summary>
        /// Sets alarm commandId to track changes in service
        /// </summary>
        /// <param name="commandId">Write command commandId as string</param>
        /// <returns>true for successful set result</returns>
        private bool AddSetControlItem(string commandId, int plcDeviceId, int dbNumber)
        {
            SetControlItem setControlItem = new SetControlItem
            {
                CommandId = $"SetControl_{commandId}",
                CommandStartDate = DateTime.Now,
                PlcDeviceId = plcDeviceId,
                DbNumber = dbNumber,
                IsCompleted = false,
            };

            string setControlItemSerialized = JsonConvert.SerializeObject(setControlItem);

            bool result = _mainCacheManager.Set($"SetControl_{commandId}", setControlItemSerialized, TimeSpan.FromDays(1));

            return result;
        }

        public bool IsUpdatedResult(Guid commandId, bool checkWithReadResponse, int tryAmount = 50)
        {
            string commandIdFormatted = commandId.ToString("N");
            SetControlItem setControlItem = new SetControlItem();
            LastDBStatus lastDBStatus = new LastDBStatus();
            bool checkResult = false;
            
            do
            {
                if (tryAmount == 0)
                {
                    break;
                }

                tryAmount--;

                string setControlSerialized = _mainCacheManager.GetString($"SetControl_{commandIdFormatted}");
                setControlSerialized = "null".Equals(setControlSerialized) ? null : setControlSerialized;

                if (!string.IsNullOrEmpty(setControlSerialized))
                {
                    setControlItem = JsonConvert.DeserializeObject<SetControlItem>(setControlSerialized);
                    string lastDBStatusSerialized = _mainCacheManager.GetString($"LastDBStatus_PLC{setControlItem.PlcDeviceId}DB{setControlItem.DbNumber}");
                    lastDBStatusSerialized = "null".Equals(lastDBStatusSerialized) ? null : lastDBStatusSerialized;

                 
                    if (!string.IsNullOrEmpty(lastDBStatusSerialized))
                    {
                        lastDBStatus = JsonConvert.DeserializeObject<LastDBStatus>(lastDBStatusSerialized);
                    }
                    else
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                }
                else
                {
                    Thread.Sleep(50);
                    continue;
                }

                List<bool> checklist = new List<bool>();
                bool checkPointIsCompleted = setControlItem.IsCompleted;
                checklist.Add(checkPointIsCompleted);
                bool checkPointDateIsCurrent = checkWithReadResponse ? DateTime.Compare(lastDBStatus.LastUpdate, setControlItem.CommandCompletedDate) >= 0 : true;
                checklist.Add(checkPointIsCompleted);
                checkResult = checklist.TrueForAll(x => x == true);

                if (!checkResult)
                {
                    Thread.Sleep(1000);
                }

            } while (!checkResult);
            return checkResult;
        }


        public async Task<bool> IsUpdatedResultAsync(Guid commandId, bool checkWithReadResponse, int tryAmount = 500)
        {
            bool checkResult = false;

            tryAmount = (tryAmount < 500) ? 500 : tryAmount;

            bool task = await Task.Run(() =>
            {
                string commandIdFormatted = commandId.ToString("N");

                SetControlItem setControlItem = new SetControlItem();
                LastDBStatus lastDBStatus = new LastDBStatus();

                do
                {
                    Thread.Sleep(50);

                    if (tryAmount == 0)
                    {
                        break;
                    }

                    tryAmount--;

                    string setControlSerialized = _mainCacheManager.GetString($"SetControl_{commandIdFormatted}");
                    setControlSerialized = "null".Equals(setControlSerialized) ? null : setControlSerialized;

                    if (!string.IsNullOrEmpty(setControlSerialized))
                    {
                        setControlItem = JsonConvert.DeserializeObject<SetControlItem>(setControlSerialized);

                        string lastDBStatusSerialized = _mainCacheManager.GetString($"LastDBStatus_PLC{setControlItem.PlcDeviceId}DB{setControlItem.DbNumber}");
                        lastDBStatusSerialized = "null".Equals(lastDBStatusSerialized) ? null : lastDBStatusSerialized;

                        if (!string.IsNullOrEmpty(lastDBStatusSerialized))
                        {
                            lastDBStatus = JsonConvert.DeserializeObject<LastDBStatus>(lastDBStatusSerialized);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    List<bool> checklist = new List<bool>();
                    bool checkPointIsCompleted = setControlItem.IsCompleted;
                    checklist.Add(checkPointIsCompleted);


                    bool checkPointDateIsCurrent = checkWithReadResponse ? DateTime.Compare(lastDBStatus.LastUpdate, setControlItem.CommandCompletedDate) == 1 : true;
                    checklist.Add(checkPointDateIsCurrent);
                    checkResult = checklist.TrueForAll(x => x == true);

                } while (!checkResult);

                return checkResult;
            });

            return checkResult;
        }
    }
}
