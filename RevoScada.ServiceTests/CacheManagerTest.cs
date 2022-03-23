using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Cache;

namespace RevoScada.ServiceTests
{
    [TestFixture]
    public class CacheManagerTest
    {
        CacheManager cacheManager;

        [SetUp]
        public void Init()
        {
            cacheManager = new CacheManager(CacheDBType.ReadService, "localhost");
        }

        [Test]
        public void set_key_get_key_string()
        {

            var setResult = cacheManager.Set<string>("testKey1", "testValue1", null);

            var getResult = cacheManager.GetString("testKey1");

            Assert.IsTrue(setResult);

            Assert.IsTrue(getResult == "testValue1");

        }

        [Test]
        public void set_key_get_key_bytearray()
        {

            byte[] array = new byte[2] { 1, 2 };

            var setResult = cacheManager.Set<byte[]>("testKeyByte", array, null);

            var getResult = cacheManager.GetByte("testKeyByte");

            Assert.That(array, Is.EqualTo(getResult));


        }


        [Test]
        public void List_left_push_left_pop()
        {
            string compareValue = "added_left" + DateTime.Now.Millisecond;



            var a = cacheManager.ListLeftPushString("a11", compareValue,1);

            var s=cacheManager.ListLeftPop("a11",1);


            Assert.IsTrue(compareValue == (string)s.ResultValue);

        }

        [Test]
        public void List_right_push_right_pop()
        {
            string compareValue = "added_right" + DateTime.Now.Millisecond;


            var a = cacheManager.ListRightPushString("a11", compareValue,1);

            var s = cacheManager.ListRightPop("a11",1);


            Assert.IsTrue(compareValue == (string)s.ResultValue);

        }


        [Test]
        public void List_left_push_left_pop_many_times()
        {
             
            string compareValue = "added_left" + DateTime.Now.Millisecond;

            for (int i = 0; i < 10000; i++)
            {
               var a = cacheManager.ListLeftPushString("a11", compareValue,1);

               var s = cacheManager.ListLeftPop("a11",1);
            }

           // Assert.IsTrue(compareValue == (string)s.ResultValue);

        }

    }
}
