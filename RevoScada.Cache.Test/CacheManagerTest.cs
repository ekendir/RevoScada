using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Complex.Alarm;
using RevoScada.Entities.Enums;

namespace RevoScada.Cache.Test
{
    [TestFixture]
    public class CacheManagerTest
    {
        CacheManager _readCacheManager;
        CacheManager _mainCacheManager;

        [SetUp]
        public void Init()
        {
            _readCacheManager = new CacheManager(CacheDBType.ReadService, "localhost");
            _mainCacheManager = new CacheManager(CacheDBType.Main, "localhost");
        }

        [Test]
        public void set_key_get_key_string()
        {

            var setResult = _readCacheManager.Set<string>("testKey1", "testValue1", null);

            var getResult = _readCacheManager.GetString("testKey1");

            Assert.IsTrue(setResult);

            Assert.IsTrue(getResult == "testValue1");

        }

        [Test]
        public void set_key_get_key_stringw()
        {
            var getResult = _mainCacheManager.GetString("CurrentProcessInfoPLC1");
            Assert.IsTrue(getResult == "testValue1");
        }


        [Test]
        public void set_key_get_key_bytearray()
        {
            byte[] array = new byte[2] { 1, 2 };
            var setResult = _readCacheManager.Set<byte[]>("testKeyByte", array, null);
            var getResult = _readCacheManager.GetByte("testKeyByte");
            Assert.That(array, Is.EqualTo(getResult));
        }


        [Test]
        public void set_key_get_key_long_bytearray()
        {
            byte[] array = new byte[65000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(DateTime.Now.Millisecond % 250);
            }

            var setResult = _readCacheManager.Set<byte[]>("testKeyByte", array, null);
            var getResult = _readCacheManager.GetByte("testKeyByte");
            Assert.That(array, Is.EqualTo(getResult));
        }

        [Test]
        public void List_left_push_left_pop()
        {
            string compareValue = "added_left" + DateTime.Now.Millisecond;
            var a = _readCacheManager.ListLeftPushString("a11", compareValue, 1);
            var s = _readCacheManager.ListLeftPop("a11", 1);
            Assert.IsTrue(compareValue == (string)s.ResultValue);
        }

        [Test]
        public void List_right_push_right_pop()
        {
            string compareValue = "added_right" + DateTime.Now.Millisecond;
            var a = _readCacheManager.ListRightPushString("a11", compareValue, 1);
            var s = _readCacheManager.ListRightPop("a11", 1);
            Assert.IsTrue(compareValue == (string)s.ResultValue);
        }

        [Test]
        [Repeat(100)]
        public void List_right_push_right_pop_byte_array()
        {
            byte[] array = new byte[65000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)((i * DateTime.Now.Millisecond) % 255);
            }

            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem();
            siemensWriteCommandItem.Buffer = array;

            var jsonstring = JsonConvert.SerializeObject(siemensWriteCommandItem);


            var a = _readCacheManager.ListRightPushString("a11", jsonstring, 1);

            var s = _readCacheManager.ListRightPop("a11", 1);


            var deserializedObject = JsonConvert.DeserializeObject<SiemensWriteCommandItem>((string)s.ResultValue);

            Assert.That(array, Is.EqualTo(deserializedObject.Buffer));



        }


        [Test]
        public void List_left_push_left_pop_many_times()
        {

            string compareValue = "added_left" + DateTime.Now.Millisecond;

            for (int i = 0; i < 10000; i++)
            {
                var a = _readCacheManager.ListLeftPushString("a11", compareValue, 1);

                var s = _readCacheManager.ListLeftPop("a11", 1);
            }

            // Assert.IsTrue(compareValue == (string)s.ResultValue);

        }

        [Test]
        public void Key_delete()
        {
            _readCacheManager.Set("deleteTest1", "value1");

            string value = _readCacheManager.GetString("deleteTest1");

            _readCacheManager.DeleteKey("deleteTest1");

            string deletedKey = _readCacheManager.GetString("deleteTest1");

            Assert.IsTrue(value == "value1");
            Assert.IsTrue(deletedKey is null);
        }

        [Test]
        public void Key_delete_all()
        {
            List<string> alarmKeys = _mainCacheManager.GetKeyNames("LastDBStatus*");

            foreach (string alarmKey in alarmKeys)
            {
                //alarmKeys.Remove(alarmKeys[alarmKeyIteration]);
                _mainCacheManager.DeleteKey(alarmKey);
            }

        }


        [Test]
        [Repeat(10)]
        public void Begin_update_cache_first_time()
        {
            List<int> list = new List<int>();
            List<int> ordered = new List<int>();

            int j = 0;
            int counter = 25;

            Task task1 = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < counter; i++)
                {
                    bool result = _mainCacheManager.SetBeginLockInfo("AlarmSetLocked1", 20, 200);
                    Thread.Sleep(300);
                    if (result)
                    {
                        j++;
                        list.Add(j);
                    }
                    _mainCacheManager.SetEndLockInfo("AlarmSetLocked1");
                }
            });

            Task task2 = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < counter; i++)
                {
                    Thread.Sleep(200);
                    bool result = _mainCacheManager.SetBeginLockInfo("AlarmSetLocked1", 20, 200);
                    Thread.Sleep(300);
                    if (result)
                    {
                        j++;
                        list.Add(j);
                    }
                    _mainCacheManager.SetEndLockInfo("AlarmSetLocked1");
                }
            });

            task1.Wait();
            task2.Wait();

            ordered = list.OrderBy(x => x).ToList();

            string orderedList = string.Join("-", ordered);
            string originalList = string.Join("-", list);

            Assert.IsTrue(orderedList == originalList);
        }

        [Test]
        public void Begin_update_cache_after_lock()
        {

            // second and more call after locked
            bool beginResult = _mainCacheManager.SetBeginLockInfo("AlarmSetLocked", 2, 500);
            Assert.IsTrue(beginResult == false); // cannot begin operation. Wait for end update

        }

        [Test]
        public void End_update_cache_first_time()
        {

            _mainCacheManager.DeleteKey("AlarmSetLocked");

            // second and more call after locked
            bool beginResult = _mainCacheManager.SetBeginLockInfo("AlarmSetLocked", 2, 500);
            Assert.IsTrue(beginResult == true); // cannot begin operation. Wait for end update

        }

        [Test]
        public void end_update_cache()
        {
            bool endlocksetResult = _mainCacheManager.SetEndLockInfo("AlarmSetLocked");

            Assert.IsTrue(endlocksetResult);


        }

        [Test]
        public void Get_keys_with_pattern()
        {
            //   _mainCacheManager.Set("PLC00_ALARM_0", "true");

            List<string> list = _mainCacheManager.GetKeyNames("*_ALARM_*");

            Assert.IsTrue(list[0].Contains("_ALARM_"));

        }


        [Test]
        [TestCase("192.168.1.50")]
        public void Connect_to_remote_Server(string server)
        {
            CacheManager remoteServer = new CacheManager(CacheDBType.Main, server);
            bool result = remoteServer.Set("A", "adfsda", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result);
        }

       



        //[Test]
        //public void set_with_transaction()
        //{
        //    //  cacheManager.SetStringWithTransaction("counter", "e");
        //    var a2d = cacheManager.SetStringWithTransaction("counter111", "1");
        //    cacheManager.Set("counter", "0");


        //    Task t1 = Task.Factory.StartNew(() =>
        //    {

        //        for (int i = 0; i < 100; i++)
        //        {
        //            //int c = Convert.ToInt32(cacheManager.GetString("counter"));

        //            //c++;

        //            var asd = cacheManager.SetStringWithTransaction("counter", true);
        //            Console.WriteLine(c);
        //            Thread.Sleep(20);
        //        }
        //    });

        //    //Thread.Sleep(100);

        //    Task t2 = Task.Factory.StartNew(() =>
        //    {
        //        for (int i = 0; i < 10; i++)
        //        {
        //            //int c = Convert.ToInt32(cacheManager.GetString("counter"));

        //            //c++;


        //            var asd = cacheManager.SetStringWithTransaction("counter", c.ToString());
        //            Console.WriteLine(c);
        //            Thread.Sleep(40);
        //        }

        //    });
        //  //  t1.com


        //    Task.WaitAll(t1, t2);




        //}
    }
}