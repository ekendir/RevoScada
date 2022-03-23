// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Cache;
using RevoScada.PlcAccess;
using RevoScada.PlcConnection.Siemens;
using RevoScada.TAI.Business.Configurations;
using RevoScada.TAI.Configurator;
using RevoScada.TAI.Entities.Complex;
using RevoScada.TAI.Entities.Configuration;

namespace RevoScada.TAI.ServiceTest
{
    //--
    [TestFixture]
    public class WriteServiceTest
    {
        CacheManager writeCacheManager;
        CacheManager readCacheManager;
        SiemensPlcAccess siemensPlcAccess;

        [SetUp]
        public void Init()
        {

            WriteConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_WriteService.rsconfig");
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_ReadService.rsconfig");

            SingleWriteConnectionManager.Instance.SiemensPlcConfigs = (List<SiemensPlcConfig>)WriteConfigurations.Instance.PlcConfigs;
            // SingleWriteConnectionManager.Instance.InitializeConnections(5);

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = (List<SiemensPlcConfig>)ReadConfigurations.Instance.PlcConfigs;
            //  SingleReadConnectionManager.Instance.InitializeConnections(5);

            siemensPlcAccess = new SiemensPlcAccess();

            readCacheManager = new CacheManager(CacheDBType.ReadService, ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);
            writeCacheManager = new CacheManager(CacheDBType.WriteService, WriteConfigurations.Instance.WriteServiceConfiguration.RedisServer);

        }

        [Test]
        public void dequeue_set_item_empty()
        {

            var dequeueResult = writeCacheManager.ListRightPop("SetCommandQueue", 10);
            Assert.IsTrue(dequeueResult.CacheResponseState == CacheResponseStates.EmptyQueue);
        }

        [Test]
        public void dequeue_set_item_success()
        {
            var dequeueResult = writeCacheManager.ListRightPop("SetCommandQueue", 10);
            Assert.IsTrue(dequeueResult.CacheResponseState == CacheResponseStates.Success);
        }

        [Test]
        public void dequeue_set_item_emergency_error()
        {
            var dequeueResult = writeCacheManager.ListRightPop("SetCommandQueue", 10);
            Assert.IsTrue(dequeueResult.CacheResponseState == CacheResponseStates.EmergencyError);
        }

        [Test]
        [TestCase("str.32")]
        public void dequeue_set_item_compare_value(string expected)
        {
            var cacheResponse = writeCacheManager.ListRightPop("SetCommandQueue", 10);

            SiemensWriteCommandItem writeCommandItem = JsonConvert.DeserializeObject<SiemensWriteCommandItem>(Convert.ToString(cacheResponse.ResultValue));

            var stringToCompare = S7.GetStringAt(writeCommandItem.Buffer, 0);


            Assert.IsTrue(expected == stringToCompare);

        }

        [Test]
        public void Dequeue_set_item_many()
        {
            CacheResponse cacheResponse;

            do
            {
                cacheResponse = writeCacheManager.ListRightPop("SetCommandQueue", 10);

                SiemensWriteCommandItem writeCommandItem = JsonConvert.DeserializeObject<SiemensWriteCommandItem>(Convert.ToString(cacheResponse.ResultValue));

                var stringToCompare = S7.GetStringAt(writeCommandItem.Buffer, 0);

                Assert.IsTrue(cacheResponse.CacheResponseState == CacheResponseStates.Success);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Success);



        }

        [Test, Repeat(1)]
        public void enqueue_set_item()
        {
            for (int i = 0; i < 10; i++)
            {



                // random byte
                //  byte r = Convert.ToByte(DateTime.Now.Second);

                byte[] strArry = new byte[18];

                //   string stringToCompare = "str." + r;
                string stringToCompare = "str__" + i.ToString();

                S7.SetStringAt(strArry, 0, 18, stringToCompare);

                Console.WriteLine(stringToCompare);

                SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
                {
                    CommandId = Guid.NewGuid().ToString("N"),
                    DbNumber = 1,
                    Offset = 500,
                    PlcId = 1,
                    Size = 18,
                    Buffer = strArry
                };

                var json = JsonConvert.SerializeObject(siemensWriteCommandItem);

                Thread.Sleep(1020);

                var leftpushResult = writeCacheManager.ListLeftPushString("SetCommandQueue", json, 10);

                Assert.IsTrue(leftpushResult.CacheResponseState == CacheResponseStates.Success);

            }
        }

        [Test]
        public void enqueue_1_set_item_and_dequeue_1()
        {


            // random byte
            byte r = Convert.ToByte(DateTime.Now.Second);

            byte[] strArry = new byte[18];

            string stringToCompare = "str." + r;


            S7.SetStringAt(strArry, 0, 18, stringToCompare);

            Console.WriteLine(stringToCompare);

            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = Guid.NewGuid().ToString("N"),
                DbNumber = 1,
                Offset = 500,
                PlcId = 1,
                Size = 18,
                Buffer = strArry
            };

            var json = JsonConvert.SerializeObject(siemensWriteCommandItem);



            var leftpushResult = writeCacheManager.ListLeftPushString("SetCommandQueue", json, 10);

            Thread.Sleep(2000);

            Assert.IsTrue(leftpushResult.CacheResponseState == CacheResponseStates.Success);

            var readResult = readCacheManager.GetString($"PLC1DB{siemensWriteCommandItem.DbNumber}");

            var readResultDeserialized = JsonConvert.DeserializeObject<ReadResult>(readResult);

            byte[] arrayFromReadCache = new byte[18];

            Array.Copy(readResultDeserialized.Result, 500, arrayFromReadCache, 0, 18);

            var stringFromReadCache = S7.GetStringAt(arrayFromReadCache, 0);

        }

        [Test]
        [Repeat(1000)]
        public void enqueue_1_set_item_and_dequeue_1_many()
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string guid = Guid.NewGuid().ToString("N");

            // random byte
            byte r = Convert.ToByte(DateTime.Now.Second);

            byte[] strArry = new byte[18];

            string stringToCompare = "str." + r;

            S7.SetStringAt(strArry, 0, 18, stringToCompare);

            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = guid,
                DbNumber = 1,
                Offset = 500,
                PlcId = 1,
                Size = 18,
                Buffer = strArry
            };

            var json = JsonConvert.SerializeObject(siemensWriteCommandItem);

            var leftpushResult = writeCacheManager.ListLeftPushString("SetCommandQueue", json, 10);

            string stringFromReadCache;

            int tryAmount = 0;

            do
            {
                tryAmount++;

                var readResult = readCacheManager.GetString($"PLC1DB{siemensWriteCommandItem.DbNumber}");

                var readResultDeserialized = JsonConvert.DeserializeObject<ReadResult>(readResult);

                byte[] arrayFromReadCache = new byte[18];

                Array.Copy(readResultDeserialized.Result, 500, arrayFromReadCache, 0, 18);

                stringFromReadCache = S7.GetStringAt(arrayFromReadCache, 0);

                Thread.Sleep(100);


            } while (stringFromReadCache != stringToCompare);

            Console.WriteLine($"{guid}:{tryAmount.ToString("D3")}:{ (stopwatch.Elapsed.Milliseconds).ToString("D3")}");

            Assert.IsTrue(stringFromReadCache == stringToCompare);





            //}
        }

      
        [Test]
        public void write_db_and_check_set_data()
        {
            // random byte
            byte r = Convert.ToByte(DateTime.Now.Second);
            byte[] strArry = new byte[18];

            string stringToCompareA = "str." + r;
            S7.SetStringAt(strArry, 0, 18, stringToCompareA);

            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = Guid.NewGuid().ToString("N"),
                DbNumber = 1,
                Offset = 500,
                PlcId = 1,
                Size = 18,
                Buffer = strArry
            };

            SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();

            siemensPlcAccess.WriteDB(siemensWriteCommandItem, 1);

            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
            {
                Offset = 500,
                DBNumber = 1,
                DataType = "string[18]"
            };

            var readResult = siemensPlcAccess.ReadDB(1, siemensTagConfiguration, 1);

            var stringToCompareB = S7.GetStringAt(readResult.Result, 0);

            CacheManager cacheManagerRead = new CacheManager(CacheDBType.ReadService, "localhost");

            Thread.Sleep(2000);

            byte[] buffer = cacheManagerRead.GetByte("PLC1DB1");

            byte[] arrayFromReadCache = new byte[18];

            Array.Copy(buffer, 500, arrayFromReadCache, 0, 18);

            Assert.That(stringToCompareA, Is.EqualTo(stringToCompareB));

            Assert.That(arrayFromReadCache, Is.EqualTo(strArry));

        }

        //[Test]
        //public void write_to_plc1()
        //{
        //    var result = siemensPlcAccess.GetAllDB(1, ReadConfigurations.Instance.SiemensReadRequestItems, 4);

        //    ReadResult p = result.Where(x => x.DbNumber == 1).First();

        //    byte[] arrayFromReadCache = (new Span<byte>(p.Result, 500, 18)).ToArray();
        //    var stringFromReadCache = S7.GetStringAt(arrayFromReadCache, 0);


        //}

        [Test]
        public void Reset_recipe()
        {
            int bufferSizeofRecipe = 30020;
            int iterationSize = ((bufferSizeofRecipe - 20) / 10);
            byte[] nullBuffer = new byte[bufferSizeofRecipe];
            
            for (int i = 0; i < iterationSize; i++)
            {
                S7.SetStringAt(nullBuffer, i * 10, 10, string.Empty);
            }

            S7.SetStringAt(nullBuffer, bufferSizeofRecipe - 20, 20, string.Empty);
            SiemensWriteCommandItem siemensWriteCommandItem2 = new SiemensWriteCommandItem
            {
                CommandId = Guid.NewGuid().ToString("N"),
                DbNumber = 1,
                Offset = 0,
                PlcId = 1,
                Size = 30020,
                Buffer = nullBuffer
            };

            var json = JsonConvert.SerializeObject(siemensWriteCommandItem2);
            var leftpushResult = writeCacheManager.ListLeftPushString("SetCommandQueue", json, 10);
            Thread.Sleep(3000);
            var plc1db1JsonData = readCacheManager.GetString("PLC1DB1");
            var deserializedJson = JsonConvert.DeserializeObject<ReadResult>(plc1db1JsonData);
            Assert.That(nullBuffer, Is.EqualTo(deserializedJson.Result));
        }



        [Test]
        [Repeat(100)]
        public void Run_more_commands_on_many_plc()
        {
            int plcId = DateTime.Now.Millisecond % 2 == 0 ? 1 : 2;

            bool value = DateTime.Now.Millisecond % 2 == 0 ;

            value = false;
            byte[] buffer = new byte[1];
            S7.SetBitAt(ref buffer,0,0, value);

            string commandId = Guid.NewGuid().ToString("N");

            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = commandId,
                DbNumber = 6,
                Offset = 6,
                PlcId = plcId,
                Size = 1,
                Buffer = buffer,
                Description = $"{commandId} " +
                $"PLC:{ plcId } " +
                $"DB:{6} " +
                $"Offset:{6}.{0} " +
                $"Size:{1} " +
                $"Value:{ value} " +
                $"item set to plc!"
            };
            var json = JsonConvert.SerializeObject(siemensWriteCommandItem);
           
            var leftpushResult = writeCacheManager.ListLeftPushString("SetCommandQueuePLC"+plcId, json, 10);
        }
    }
}