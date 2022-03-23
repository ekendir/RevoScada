using RevoScada.Entities.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revo.SiemensDrivers;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Entities.Configuration;
using Revo.Core;
using System.Threading;
using RevoScada.PlcConnection.Siemens;

namespace RevoScada.PlcAccess
{
    public class SiemensPlcAccess
    {

        /// <summary>
        /// Retrieves all db blocks as list of read result
        /// </summary>
        /// <param name="readRequestItems">Provide read request items to read db raw</param>
        public List<ReadResult> GetAllDB(int plcDeviceId,List<SiemensReadRequestItem> readRequestItems, int tryAmount)
        {
            List<ReadResult> readResults = new List<ReadResult>();
           
            List<SiemensReadRequestItem> siemensReadRequestItems = new List<SiemensReadRequestItem>(readRequestItems.Where(x=>x.IsDemanded==true));

            List<SiemensReadRequestItem> readRequestItemsTemp;

            List<Exception> exceptions = new List<Exception>(); 

            do
            {
               

                if (tryAmount == 0)
                {
                   SingleReadConnectionManager.Instance.DisconnectAllDevices();
                   SingleReadConnectionManager.Instance.InitializeConnections(10);

                   LogManager.Instance.Log($"GetAllDB: Reconnected to plc!", LogType.Warning);


                    break;

                   
                }
                tryAmount--;

                readRequestItemsTemp = new List<SiemensReadRequestItem>(siemensReadRequestItems);

                S7Client s7Client;

                try
                {
                    s7Client =  SingleReadConnectionManager.Instance.GetClient(plcDeviceId, 2);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    continue;
                }

                foreach (SiemensReadRequestItem readRequestItem in readRequestItemsTemp.OrderByDescending(x=>x.DbNumber))
                {
                    
                    byte[] buffer = new byte[Convert.ToInt32(readRequestItem.ComputedSize)];

                    int s7Result;
         
                    if (s7Client != null)
                    {
                        s7Result = s7Client.DBRead(readRequestItem.DbNumber, 0, Convert.ToInt32(readRequestItem.ComputedSize), buffer);

                    }
                    else
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    if (s7Result == 0)
                    {

                        ReadResult readResult = new ReadResult();
                        readResult.DbNumber = readRequestItem.DbNumber;
                        readResult.DateTime = DateTime.Now;
                        readResult.S7Result = s7Result;
                        readResult.Result = buffer;
                        readResults.Add(readResult);
                        siemensReadRequestItems.Remove(readRequestItem);

                    }
                    else
                    {
                         
                        LogManager.Instance.Log($"GetAllDB: DB:{readRequestItem.DbNumber}  Client Error:{s7Client.ErrorText(s7Result)}", LogType.Warning);

                        Thread.Sleep(500);

                    }
                }

            } while (siemensReadRequestItems.Count > 0);

            if (readResults!=null)
            {
                return readResults;
            }
            else
            {
                foreach (var exception   in exceptions)
                {
                    LogManager.Instance.Log($"GetAllDB: {exception.Message}", LogType.Error);
                }
                return null;
            }
        }

        public ReadResult ReadDB(int plcDeviceId, SiemensTagConfiguration siemensTagConfiguration, int tryAmount)
        {
            ReadResult readResult = new ReadResult();

            List<Exception> exceptions = new List<Exception>();

            StringBuilder stringBuilder = new StringBuilder();

            do
            {
                if (tryAmount == 0)
                {
                    LogManager.Instance.Log($"ReadDB: ReadResults: {stringBuilder.ToString()}", LogType.Error);

                    break;
                }

                tryAmount--;

                S7Client s7Client = null;

                try
                {
                    s7Client = PlcConnection.Siemens.SingleReadConnectionManager.Instance.GetClient(plcDeviceId, 5);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    continue;
                }


                int dataSize = DataSize(siemensTagConfiguration.DataType);
                byte[] buffer = new byte[dataSize];

                int s7Result;

                if (s7Client != null)
                {
                    s7Result = s7Client.DBRead(siemensTagConfiguration.DBNumber, Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)), dataSize, buffer);
                }
                else
                {

                    continue;
                }


                if (s7Result == 0)
                {

                    readResult.DbNumber = siemensTagConfiguration.DBNumber;
                    readResult.S7Result = s7Result;
                    readResult.Result = buffer;
                    break;
                }
                else
                {
                    stringBuilder.Append(" " + s7Result.ToString());
                    Thread.Sleep(500);
                }

            } while (true);

            if (readResult != null)
            {
                return readResult;
            }
            else
            {
                foreach (var exception in exceptions)
                {
                    LogManager.Instance.Log($"ReadDB: {exception.Message}", LogType.Error);
                }
                return null;
            }

        }

        public WriteResult WriteDB(SiemensWriteCommandItem siemensWriteCommandItem, int tryAmount)
        {
            WriteResult writeResult = new WriteResult();
            
            do
            {
                int s7Result = -1;

                if (tryAmount == 0)
                {
                    LogManager.Instance.Log($"ReadDB: Write Error", LogType.Error);
                    return writeResult;
                }

                tryAmount--;

                S7Client s7Client = null;

                try
                {
                    s7Client = PlcConnection.Siemens.SingleWriteConnectionManager.Instance.GetClient(siemensWriteCommandItem.PlcId, 5);
                }
                catch 
                {
                    writeResult.S7Result = s7Result;

                    Thread.Sleep(100);
                    continue;
                }

               

                if (s7Client != null)
                {
                    s7Result = s7Client.DBWrite(siemensWriteCommandItem.DbNumber, siemensWriteCommandItem.Offset, siemensWriteCommandItem.Size, siemensWriteCommandItem.Buffer);
                    writeResult.S7Result = s7Result;
                }
                else
                {
                    writeResult.S7Result = s7Result;
                    Thread.Sleep(100);
                    continue;
                }

                if (s7Result == 0)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                    continue;
                }

            } while (true);

            return writeResult;
        }

        public T GetFromReadResult<T>(SiemensTagConfiguration siemensTagConfiguration, ReadResult readResult)
        {
            object result = null;
            
            try
            {
                switch (siemensTagConfiguration.DataType.ToLower())
                {
                    case "byte":
                        result = S7.GetByteAt(readResult.Result, 0);
                        break;
                    case "int":
                        result = S7.GetIntAt(readResult.Result, 0);
                        break;
                    case "real":
                        result = S7.GetRealAt(readResult.Result, 0);
                        break;
                    case string typeName when typeName.Contains("string"):
                        result = S7.GetStringAt(readResult.Result, 0);
                        break;
                }

            }
            catch (Exception ex)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public SiemensWriteCommandItem GetSiemensWriteCommandItem(SiemensTagConfiguration siemensTagConfiguration, string value)
        {
            int offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
            int offsetDecimal = Convert.ToInt32((siemensTagConfiguration.Offset - Math.Floor(siemensTagConfiguration.Offset)) * 10);
            byte[] setBuffer = null;
            int dataSize = 0;

            try
            {
                switch (siemensTagConfiguration.DataType.ToLower())
                {
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
                return null;
            }

            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                DbNumber = siemensTagConfiguration.DBNumber,
                Offset = offsetIntegral,
                PlcId = siemensTagConfiguration.PlcId,
                Size = dataSize,
                Buffer = setBuffer
            };

            return siemensWriteCommandItem;
        }


        public int DataSize(string dataType)
        {
            int dataSize = 0;
            try
            {
              dataType = dataType.ToLower();
              dataType = dataType == "bool" ? "bit" : dataType;
              dataType = ( dataType == "int" || dataType == "ınt") ? "word" : dataType;
              dataType = ( dataType == "real" || dataType == "dword") ? "dword" : dataType;
              dataType = ( dataType == "udint" || dataType == "udınt") ? "udint" : dataType;
              dataType = ( dataType == "strıng") ? "string" : dataType;

                switch (dataType)
                {
                    case "bit": dataSize = 1; break;
                    case "byte": dataSize = 1; break;
                    case "word": dataSize = 2; break;
                    case "dword": dataSize = 4; break;
                    case "real": dataSize = 4; break;
                    case "udint": dataSize = 4; break;
                }

                if (dataType.Contains("string"))
                {
                    dataSize = Convert.ToByte(System.Text.RegularExpressions.Regex.Match(dataType, @"\d+").ToString())+2;
                    dataType = "string";
                }

               // ComputedSize = ComputedSize + dataSize;

            }
            catch 
            {
                dataSize = 0;
            }

            return dataSize;

        }


    }
}
