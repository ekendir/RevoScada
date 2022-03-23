using Revo.SiemensDrivers.Sharp7;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RevoScada.PlcConnection.Siemens
{
    public abstract class SingleConnectionManagerBase
    {

        protected internal ConcurrentDictionary<int, S7Client> _clients;

        protected internal ConcurrentDictionary<int, bool> _plcDevicesIsConnected;

        private object lockObject = new object();

        /// <summary>
        /// set plc devices
        /// </summary>
        public List<SiemensPlcConfig> SiemensPlcConfigs;


        /// <summary>
        /// Add or update existing s7client
        /// </summary>
        /// <param name="deviceId">PLC Device Id</param>
        /// <param name="client"></param>
        /// <returns>add S7Client to dictionary</returns>
        protected internal bool AddorUpdate(int deviceId, S7Client client)
        {

            bool addorUpdateResult = false;

            try
            {

                int tryAmount = 5;

                do
                {

                    tryAmount--;

                    if (tryAmount == 0)
                    {
                        break;
                    }

                    try
                    {
                        _clients[deviceId] = client;

                        addorUpdateResult = true;

                        break;
                    }
                    catch
                    {
                        continue;
                    }

                } while (tryAmount > 0);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ReadConnectionPoolManager > AddorUpdate Error: " + ex.Message.ToString());

                addorUpdateResult = false;
            }

            return addorUpdateResult;
        }


        /// <summary>
        /// Retrieves opened s7client
        /// </summary>
        /// <param name="deviceId">Connected Device Id</param>
        /// <param name="tryAmount">Try amount to get opened s7client</param>
        /// <returns>Get S7Client in dictionary</returns>
        public S7Client GetClient(int deviceId, int tryAmount)
        {
            S7Client s7Client = null;

            SiemensPlcConfig siemensPlcConfig;
            
            if (SiemensPlcConfigs.Count > 0)
            {
                siemensPlcConfig = SiemensPlcConfigs.Where(x => x.PlcDeviceId == deviceId).First();
            }
            else
            {
                throw new Exception($"PlcDeviceId {deviceId} not found in initial configuration!");
            }


            bool tryGetClientResult = _clients.TryGetValue(deviceId, out s7Client);

            if (s7Client != null)
            {

                int tryGetClientTryAmount = 4;


                lock (lockObject)
                {
                    int plcStatus = 0;

                    do
                    {
                        tryGetClientTryAmount--;

                        if (tryGetClientTryAmount == 0)
                        {
                            break;
                        }

                        var statusResult = s7Client.PlcGetStatus(ref plcStatus);

                        if (s7Client.Connected)
                        {

                            if (plcStatus == 8)
                            {
                                return s7Client;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {
                            int reconnectResult = -1;
                            
                            do
                            { 
                                
                                if (tryAmount == 0)
                                {
                                    break;
                                }

                                tryAmount--;

                                reconnectResult = s7Client.Connect();

                               

                                Thread.Sleep(100);

                            } while (reconnectResult != 0);

                            if (!s7Client.Connected)
                            {
                                s7Client = Reconnect(deviceId, tryAmount);
                            }
                            else
                            {

                               

                                if (plcStatus == 8)
                                {
                                    return s7Client;
                                }
                                else
                                {
                                    continue;
                                }


                            }
                        }


                        Thread.Sleep(100);

                    } while (!s7Client.Connected && plcStatus==8);


                    if ( plcStatus!=8)
                    {
                         throw new Exception($"Connect Error: GetClient plcStatus:{plcStatus} ");
                    }

                }
                 
            }
            else
            {
                    s7Client = Reconnect(deviceId, tryAmount);
            }

            if (s7Client != null)
            {
                if (s7Client.Connected)
                {
                    _plcDevicesIsConnected[deviceId] = true;
                }
                else
                {
                    _plcDevicesIsConnected[deviceId] = false;

                }
            }

            return s7Client;
        }

        /// <summary>
        ///  Try to get new connections
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="tryAmount"></param>
        ///
        /// Plc Statuses
        /// S7CpuStatusUnknown 0x00 The CPU status is unknown.
        /// S7CpuStatusRun 0x08 The CPU is running.
        /// S7CpuStatusStop 0x04 The CPU is stopped.
        ///
        /// 
        /// <returns></returns>
        protected internal S7Client Reconnect(int deviceId, int tryAmount)
        {

            int newConnectResult = -1;

            S7Client s7ClientNew=null;

            SiemensPlcConfig siemensPlcConfig = SiemensPlcConfigs.Where(x => x.PlcDeviceId == deviceId).First();

            do
            {
                if (tryAmount == 0)
                {
                    break;
                }

                tryAmount--;

                s7ClientNew = new S7Client();

                newConnectResult = s7ClientNew.ConnectTo(siemensPlcConfig.Ip, siemensPlcConfig.Rack, siemensPlcConfig.Slot);

               

                Thread.Sleep(100);

            } while (newConnectResult != 0);

            if (s7ClientNew != null)
            {

                s7ClientNew.Connect();

                if (s7ClientNew.Connected)
                {
                   int plcStatus = 0;

                   int getStatusResult= s7ClientNew.PlcGetStatus(ref plcStatus);

                    //System.Diagnostics.Debug.Write("Reconnect:getStatusResult(0) " + getStatusResult);
                    //System.Diagnostics.Debug.Write("Reconnect:plcStatus (0 8 4) " + plcStatus);

                    if (plcStatus == 8)
                    {
                        bool addorUpdateResult = AddorUpdate(deviceId, s7ClientNew);
                    }
                    else
                    {
                        throw new Exception($"Reconnect Error: GetStatusResult: {getStatusResult} plcStatus:{plcStatus} ");
                    }

                    return s7ClientNew;
                }
                else
                {
                    throw new Exception($"[high] PLC Connection Manager: PLC not connected! IP:{siemensPlcConfig.Ip}  Rack:{siemensPlcConfig.Rack}  Slot:{siemensPlcConfig.Slot}");
                }

            }
            else
            {
                throw new Exception($"[high] PLC not connected! ConnectTo Client is null");
            }

        }


        /// <summary>
        /// Retrieves s7clients by device Id
        /// </summary>
        internal ConcurrentDictionary<int, S7Client> Connections { get { return _clients; } }


        public ConcurrentDictionary<int, bool> PLCDevicesIsConnected { get { return _plcDevicesIsConnected; } }


        /// <summary>
        /// initialize connections for filling s7client dictionary 
        /// </summary>
        /// <param name="createTryAmout"></param>
        public void InitializeConnections(int createTryAmout)
        {
            _clients = new ConcurrentDictionary<int, S7Client>(); 
            
            try
            {
                foreach (var plcDevice in SiemensPlcConfigs)
                {
                    S7Client s7Client = GetClient(plcDevice.PlcDeviceId, createTryAmout);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Checks all devices connections
        /// </summary>
        /// <returns>true for all avaible connection. False if one of them is not avaible</returns>
        public bool IsAllConnectionsAvaible()
        {

            bool checkResult = false;

            try
            {

                checkResult = (_clients.Values.Count() == 0) ? false : !_clients.Any(x => x.Value.Connected == false);
            }
            catch
            {

                checkResult = false;
            }


            return checkResult;
        }


        /// <summary>
        /// Disconnects all devices 
        /// </summary>
        public void DisconnectAllDevices()
        {
            foreach (var connection in Connections)
            {
                if (connection.Value != null)
                {
                    connection.Value.Disconnect();
                }

            }

            _clients = new ConcurrentDictionary<int, S7Client>();
        }


        //public int void GetStatus()
        //{
        //    //plc statusu belirliyor 0-Unknown,4-Stop,8-Run ,hepsinden farklıysa error
        //    Int32 plcConnectionStatus = 0;

        //    S7Connect.client.PlcGetStatus(ref plcConnectionStatus);

        //    if (plcConnectionStatus == 0)
        //          //Unknown
        //        else if (plcConnectionStatus == 4)
        //         //Stop
        //        else if (plcConnectionStatus == 8)
        //           //Run
        //        else
        //            //Error

        //}

    }
}
