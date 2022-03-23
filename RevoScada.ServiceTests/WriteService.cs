// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RevoScada.Cache;

namespace RevoScada.TAI.ServiceTest
{
    [TestFixture]
    public class WriteService
    {
        CacheManager _cacheManager;

        [SetUp]
        public void Init()
        {
            _cacheManager = new CacheManager(CacheDBType.ReadService, "localhost");
        }

        [Test]
        public void dequeue_set_command_queue()
        {
         CacheResponse response=  _cacheManager.ListRightPop("SetCommandQueue",1);

           

        }


    }
}
