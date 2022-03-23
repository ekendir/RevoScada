
using System;
using Newtonsoft.Json;
using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.PageTagConfigurations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RevoScada.Entities;
using RevoScada.Entities.Enums;
using System.Threading.Tasks;
using System.Threading;
using RevoScada.ProcessController;

namespace RevoScada.DesktopApplication.Test
{
    [TestFixture]
    class PlcManagerTest
    {
        SiemensTagConfiguration tagInfo;


           PlcCommandManager plcCommandManager;

        string _cacheServer;
        [SetUp]
        public void Init()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);
 
            plcCommandManager = new PlcCommandManager(_cacheServer);

        }


        [Test]
        [Repeat(1)]
        [TestCase(15584)]
        public void Check_set(int tagId)
        {
            tagInfo = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[tagId];
            
            Guid guid = Guid.NewGuid();
            
            plcCommandManager.Set(tagInfo, 2, guid);

            bool result = false;// plcCommandManager.IsUpdatedResult(guid);

            result = plcCommandManager.IsUpdatedResult(guid,false);

            Assert.IsTrue(result);

           // result = Task.Run(()=>plcCommandManager.IsUpdatedResultAsync(guid)).Result;

        }


        [Test]
        [Repeat(1)]
        [TestCase(15584)]
        public async  void Check_set_async(int tagId)
        {
            tagInfo = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[tagId];

            Guid guid = Guid.NewGuid();

            plcCommandManager.Set(tagInfo, 2, guid);

          
           
            Task<bool> task = plcCommandManager.IsUpdatedResultAsync(guid,false, 100);

            await task;

            task.Wait();
            bool x = task.Result;

         //   tbTaskResult.Text = task.Result.ToString();


            Assert.IsTrue(x);

            // result = Task.Run(()=>plcCommandManager.IsUpdatedResultAsync(guid)).Result;

        }

    }
}