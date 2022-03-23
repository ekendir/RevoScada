
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Revo.Core;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration.Service;

namespace RevoScada.PlcConnection.Test
{
    [TestFixture]
    public class PlcConnectionTest
    {
        private ReadServiceConfiguration _readServiceConfiguration;

        [SetUp]
        public void Init()
        {

       //    ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations\ReadService.rsconfig");
          //  _readServiceConfiguration = ReadConfigurations.Instance.ReadServiceConfiguration;

        }

        [Test]
         [TestCase("192.168.0.11", 1,3,4,1740)]
        //[TestCase("10.242.144.31", 1,45,4,216)]
        //[TestCase("192.168.0.11", 1,47,4,172)] 
        //[TestCase("10.242.144.31", 1,5,4,288)]
        //  [TestCase("192.168.1.203", 1,45,4,216)]
        //   [TestCase("192.168.1.206", 1,253,26,0)]
        public void Get_real_value(string ip,float expected,int db,int bufferSize,int offset)
        {
            byte[] buffer = new byte[bufferSize];

            var client = new S7Client();
            var connectResult = client.ConnectTo(ip, 0, 0);

            int result = client.DBRead(db, offset, bufferSize, buffer);

            float value = S7.GetRealAt(buffer, 0);

            Assert.IsTrue(expected == value);

        }


        [Test]
        // [TestCase("192.168.1.207", 1,45,4,216)]
        [TestCase("192.168.0.11", 1, 5, 2, 390)]
        //[TestCase("10.242.144.31", 4, 401, 2, 8)]
        //  [TestCase("192.168.1.203", 1,45,4,216)]
        //   [TestCase("192.168.1.206", 1,253,26,0)]
        public void Get_int_value(string ip, int expected, int db, int bufferSize, int offset)
        {
            byte[] buffer = new byte[bufferSize];

            var client = new S7Client();
            var connectResult = client.ConnectTo(ip, 0, 0);

            int result = client.DBRead(db, offset, bufferSize, buffer);

            int value = S7.GetIntAt(buffer, 0);

            Assert.IsTrue(expected == value);

        }



        [Test]
        // [TestCase("192.168.1.204", "BİLGİN ÇELİK - AIR", 19, 20 , 0)]
        // [TestCase("192.168.1.203", "", 6, 102, 220)]
       // [TestCase("192.168.0.11", "", 6, 100, 322)]
        //[TestCase("10.242.144.40", "", 6, 100, 220)]
        [TestCase("10.242.144.40", "", 6, 100, 322)]
            //  [Repeat(29)]
        public void Get_String_value(string ip, string expected, int db, int bufferSize, int offset)
        {
            byte[] buffer = new byte[bufferSize];

            var client = new S7Client();

            var connectResult = client.ConnectTo(ip, 0, 0);

            int result = client.DBRead(db, offset, bufferSize, buffer);

            string value = S7.GetStringAt(buffer, 0);

            int plcStatus = 0;

            int getStatusResult = client.PlcGetStatus(ref plcStatus);


           Assert.IsTrue(0< value.Length);

        }

      
        [Test]
       
       // [TestCase("10.242.144.31", "", 6, 100, 322)]
        [TestCase("192.168.0.11", 1, 402, 4, 76)]

        public void Set_Real_value(string ip, float expected, int db, int bufferSize, int offset)
        {
            byte[] buffer = new byte[bufferSize];
            var client = new S7Client();
            var connectResult = client.ConnectTo(ip, 0, 0);
            S7.SetRealAt(buffer, 0, expected);
            int result = client.DBWrite(db, 0, bufferSize, buffer);
           // string value = S7.SetRealAt(buffer, 0);
        }

        [Test]
        //[TestCase("192.168.1.204", "", 6, 220, 0)]
        //[TestCase("192.168.1.207", "", 6, 220, 0)]
        //[TestCase("10.242.144.40", "", 6, 100, 322)]
        [TestCase("10.242.144.40", "", 6, 100, 220)]
        //[TestCase("192.168.1.203", "AYIRICI FIRIN YENİ", 19, 20, 0)]
        public void Set_String_value(string ip, string expected, int db, int bufferSize, int offset)
        {
            byte[] buffer = new byte[bufferSize];
            var client = new S7Client();
            var connectResult = client.ConnectTo(ip, 0, 0);
            S7.SetStringAt(buffer, 0,bufferSize,expected);
            int result= client.DBWrite(db, 0, bufferSize, buffer);
            string value = S7.GetStringAt(buffer, 0);
        }


        [TestCase("192.168.0.11", "", 6, 1, 3)]
        public void Set_byte_value(string ip, string expected, int db, int bufferSize, int offset)
        {
            byte[] buffer = new byte[bufferSize];
            var client = new S7Client();
            var connectResult = client.ConnectTo(ip, 0, 0);
            S7.SetByteAt(buffer, 0, 0);
            int result = client.DBWrite(db, 0, bufferSize, buffer);
        }


        #region Read Connection

        [Test]
        [TestCase("10.242.144.31")]
        //[TestCase("10.242.144.31")]
        public void Is_PlcConnected_with_driver(string ip)
        {

            
           byte[] buffer = new byte[2];

           var client = new S7Client();
           var connectResult= client.ConnectTo(ip, 0, 0);

           int result = client.DBRead(2, 4, 2, buffer);
           var value = S7.GetIntAt(buffer, 0);

            Assert.IsTrue(client.Connected);
            Assert.IsTrue(result == 0);
              
        }

        [Test]
        [TestCase("10.242.144.31")]
       // [TestCase("10.242.144.31")]
        public void GetSetGet(string ip)
        {

            int valueToCompare = 0;
            byte[] buffer = new byte[2];

            var client = new S7Client();
            var connectResult = client.ConnectTo(ip, 0, 0);
            int result = client.DBRead(2, 4, 2, buffer);
            
            var value = S7.GetIntAt(buffer, 0);
            valueToCompare = value;

            value++;
            valueToCompare++;

            S7.SetIntAt(buffer, 0,value);
            client.DBWrite(2, 0, 2, buffer);

            result = client.DBRead(2, 4, 2, buffer);
            value = S7.GetIntAt(buffer, 0);
            

            Assert.IsTrue(value == valueToCompare);

        }






        [Test]
        public void Set_siemens_plc_configs()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();
            
            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;

            Assert.IsTrue(SingleReadConnectionManager.Instance.SiemensPlcConfigs.Count > 0);

        }


        //single plc test for connection
        [Test]
        public void Siemens_plc_is_connected()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
          
            var s7Client = SingleReadConnectionManager.Instance.GetClient(1, 1);

            s7Client = SingleReadConnectionManager.Instance.GetClient(1, 1);

            Assert.IsTrue(s7Client.Connected == true);

        }




        [Test]
        public void Set_siemens_plc_is_connected()
        {
             
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;


            Stopwatch stopwatchSingleConnection = new Stopwatch();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            LogManager.Instance.InitializeConfiguration(ReadConfigurations.Instance.ReadServiceConfiguration.LogSettings);

            for (int i = 0; i < 1000; i++)
            {
              
                stopwatchSingleConnection.Start();

                var s7Client = SingleReadConnectionManager.Instance.GetClient(1, 1);

                var endStopwatchSingle = stopwatchSingleConnection.Elapsed.Milliseconds;

                LogManager.Instance.Log($"Connection Millisecond:{i} | { endStopwatchSingle} ms", LogType.Information);

                //connectedStatus = s7Client.Connected;

                //if (!connectedStatus)
                //    Assert.IsTrue(connectedStatus == false);

                Assert.IsTrue(s7Client.Connected == true);
            }

            var endStopwatch = stopwatch.Elapsed.TotalMilliseconds;

            LogManager.Instance.Log($"Connection AllTotalMillisecond: { endStopwatch}", LogType.Information);

           // Assert.IsTrue(connectedStatus == true);
        }

        [Test]
        public void check_plc_closed()
        {
            var client = new S7Client();
            client.ConnectTo("192.168.1.204", 0, 0);

            var isConnected = client.Connected;
            int plcStatus = 0;
            var plcStatusGetResult = client.PlcGetStatus(ref plcStatus);

        }

        [Test]
        public void check_plc_status_running()
        {

            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\MultipleConfigurations\ReadService.rsconfig");
            _readServiceConfiguration = ReadConfigurations.Instance.ReadServiceConfiguration;
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);
               
            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;

            var s7Client = SingleReadConnectionManager.Instance.GetClient(1, 1);

            int plcStatus = 0;
            var plcStatusGetResult = s7Client.PlcGetStatus(ref plcStatus);

            Assert.IsTrue(plcStatus == 8);

        }

        [Test]
        public void check_plc_status_stopped()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;

            var s7Client = SingleReadConnectionManager.Instance.GetClient(1,2);

            int plcStatus = 0;
            var plcStatusGetResult = s7Client.PlcGetStatus(ref plcStatus);

            Assert.IsTrue(plcStatus == 4);

        }
        [Test]

        public void reconnect()
        {
            S7Client s7Client = new S7Client();
            int connectResult = s7Client.ConnectTo("192.168.1.204", 0, 0);

            var isConnected = s7Client.Connected;
            int plcStatus = 0;
            var plcStatusGetResult = s7Client.PlcGetStatus(ref plcStatus);


            // s7Client.Connect();

        }



        // status 4 stopped
        // status 8 run
        [Test]
        public void check_plc_status()
        {
            Debug.WriteLine("check_plc_status starts");

            for (int i = 0; i < 4; i++)
            {
                SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);

                var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

                SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
               

                try
                {
                    var s7Client = SingleReadConnectionManager.Instance.GetClient(1, 1);

                    int plcStatus = 0;
                    var plcStatusGetResult = s7Client.PlcGetStatus(ref plcStatus);

                    Assert.IsTrue(plcStatus == 8);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" " +ex.Message);

                }
              
             

                Thread.Sleep(1000);
            }

        }


        #endregion

        #region Write Connection

        [Test]
        public void Set_siemens_plc_configs_Write()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_readServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleWriteConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;

            Assert.IsTrue(SingleWriteConnectionManager.Instance.SiemensPlcConfigs.Count > 0);

        }

        



        //[Test]
        //public void Is_PlcConnected_with_driver()
        //{


        //    byte[] buffer = new byte[500];
        //    var client = new S7Client();
        //    var connectResult = client.ConnectTo("192.168.1.203", 0, 0);
        //    int result = client.DBRead(30, 0, 500, buffer);

        //    Assert.IsTrue(client.Connected);
        //    Assert.IsTrue(result == 0);


        //}


        #endregion





        [TearDown]
        public void Closing()
        {
        }
    }
}
