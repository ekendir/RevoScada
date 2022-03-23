
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;
using System;
using System.Diagnostics;
using System.Threading;

namespace RevoScada.ProcessController.Test
{
    [TestFixture]
    class RevoScadaProcessControllerTest
    {
        [SetUp]
        public void Init()
        {

            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\Application.rsconfig", false);
        }




        [Test]
        public void Is_finish_acknowledged()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ProcessManager.Instance.Initialize(ApplicationConfigurations.Instance.Configuration, ApplicationConfigurations.Instance.TagConfigurations);
            ProcessManager.Instance.InitializeRunOperationTags();
            ProcessManager.Instance.ChangeUserAcknowledgeForFinish(true);
            bool result = ProcessManager.Instance.IsFinishAcknowledged();
            Assert.IsTrue(result == true);
            ProcessManager.Instance.ChangeUserAcknowledgeForFinish(false);
            result = ProcessManager.Instance.IsFinishAcknowledged();
            Assert.IsTrue(result == false);
            int elapsedTimeresult = (int)sw.ElapsedMilliseconds;
            sw.Stop();
        }

        [Test]
        public void Is_operation_command_working()
        {
            ProcessManager.Instance.Initialize(ApplicationConfigurations.Instance.Configuration, ApplicationConfigurations.Instance.TagConfigurations);
            ProcessManager.Instance.IsRunOperationCommandWorking = true;
            Assert.IsTrue(ProcessManager.Instance.IsRunOperationCommandWorking == true);
            Thread.Sleep(10000);
            Assert.IsTrue(ProcessManager.Instance.IsRunOperationCommandWorking == false);
        }
    }
}
