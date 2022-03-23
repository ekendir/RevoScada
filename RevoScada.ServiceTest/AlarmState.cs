using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using Revo.ServiceUtilities;
using RevoScada.Cache;
using RevoScada.PlcConnection.Siemens;
using RevoScada.TAI.Business.Configurations;
using RevoScada.TAI.Configurator;
using RevoScada.TAI.Entities.Complex.Alarm;

namespace RevoScada.TAI.ServiceTest
{
    [TestFixture]
    public class AlarmState
    {
        CacheManager cacheManager;

        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_ReadService.rsconfig");

            cacheManager = new CacheManager(CacheDBType.ReadService, ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);

            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;

        }


        [Test]
        public void check_alarm_state()
        {
 
           
            var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("**************************");

                var jsonStringGet = cacheManager.GetString("ReadServiceState"); 
                ReadServiceState readServiceState = JsonConvert.DeserializeObject<ReadServiceState>(jsonStringGet);

                Debug.WriteLine($"{readServiceState.GetAllDBCount} {readServiceState.LastCycleRunTime}");


                Thread.Sleep(1000);
            }





        }


        [Test]
        public void check_internet_connection()
        {

            for (int i = 0; i < 110; i++)
            {
              bool result=  NetworkChecker.IsConnectedToInternet();
              bool result2=false;

                try
                {
                      result2 = NetworkChecker.PingSucceeded("192.168.1.203");
                }
                catch (Exception)
                {

                }
               
                Debug.WriteLine($"IsConnectedToInternet: {result}");
                Debug.WriteLine($"PingSucceeded: {result2}");



                Thread.Sleep(1000);
            }


          




        }




    }
}
