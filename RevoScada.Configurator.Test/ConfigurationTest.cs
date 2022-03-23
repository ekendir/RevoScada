using NUnit.Framework;

namespace RevoScada.Configurator.Test
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void Get_read_configurations()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\ReadService.rsconfig");
            var config = ReadConfigurations.Instance.ReadServiceConfiguration;
            Assert.IsTrue(config.SqliteConnectionString.Contains("Data"));
            Assert.IsTrue(config.RedisServer.Contains("host"));
        }

        [Test]
        public void Get_read_configurations_multiple_devices()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations\ReadService.rsconfig");
            var config = ReadConfigurations.Instance.MultipleDeviceSiemensReadRequestItems;
        }

        [Test]
        public void Get_write_configurations()
        {
            WriteConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\WriteServiceTest.rsconfig");
            var config = WriteConfigurations.Instance.WriteServiceConfiguration;
            Assert.IsTrue(config.SqliteConnectionString.Contains("Data"));
            Assert.IsTrue(config.RedisServer.Contains("host"));
        }

        [Test]
        public void Get_application_configurations()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations\Application.rsconfig", false);
            var config = ApplicationConfigurations.Instance.Configuration;
            Assert.IsTrue(config.SqliteConnectionString.Contains("Data"));
            Assert.IsTrue(config.RedisServer.Contains("host"));
        }

        [Test]
        public void Get_application_configurations_pc()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\Application.rsconfig", false);
            var config = ApplicationConfigurations.Instance.Configuration;
            Assert.IsTrue(config.SqliteConnectionString.Contains("Data"));
            Assert.IsTrue(config.RedisServer.Contains("host"));
        }

        [Test]
        public void Get_datalog_configurations()
        {
            DataLoggerServiceConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\DataLoggerService.rsconfig");
            var config = DataLoggerServiceConfigurations.Instance;
            Assert.IsTrue((DataLoggerServiceConfigurations.Instance.DataLogTags?.Count ?? 0) >= 612);
            Assert.IsTrue(config.DataLoggerServiceConfiguration.SqliteConnectionString.Contains("Data"));
            Assert.IsTrue(config.DataLoggerServiceConfiguration.RedisServer.Contains("host"));
        }

        [Test]
        public void Get_alarm_configurations()
        {
            AlarmServiceConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_AlarmService.rsconfig");
            var config = AlarmServiceConfigurations.Instance;
            //  Assert.IsTrue((AlarmServiceConfigurations.Instance.DataLogTags?.Count ?? 0) >= 612);
            Assert.IsTrue(config.AlarmServiceConfiguration.SqliteConnectionString.Contains("Data"));
            Assert.IsTrue(config.AlarmServiceConfiguration.RedisServer.Contains("host"));
        }

        [Test]
        public void Get_sync_configurations()
        {
            SyncServiceConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\\MultipleConfigurations\SyncService.rsconfig");
            var config = SyncServiceConfigurations.Instance;
            
        }
    }
}
