using NUnit.Framework;
using Revo.Core;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RevoScada.PlcAccess.Test
{
    [TestFixture]
    public class PlcAccessReadTest
    {
        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations\ReadService.rsconfig");
            SingleReadConnectionManager.Instance.SiemensPlcConfigs = (List<SiemensPlcConfig>) ReadConfigurations.Instance.PlcConfigs;
            SingleReadConnectionManager.Instance.InitializeConnections(5);
            LogManager.Instance.InitializeConfiguration(ReadConfigurations.Instance.ReadServiceConfiguration.LogSettings);
        }
        [Test]
        public void GetAllDB()
        {
            Dictionary<int, List<SiemensReadRequestItem>> readRequestItems = ReadConfigurations.Instance.MultipleDeviceSiemensReadRequestItems.ToDictionary(x => x.Key, x => x.Value);
            SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();
            siemensPlcAccess.GetAllDB(1,readRequestItems[1], 1);

        }
    }


    [TestFixture]
    public class PlcAccessWriteTest
    {
        [SetUp]
        public void Init()
        {
            WriteConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\WriteServiceTest.rsconfig");

            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(WriteConfigurations.Instance.WriteServiceConfiguration.SqliteConnectionString);

            var siemensPlcConfigs = siemensPlcConfigService.GetAll().ToList();

            SingleWriteConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;

            SingleWriteConnectionManager.Instance.InitializeConnections(5);

        }
         
        [Test]
        public void write_db()
        {
            SiemensWriteCommandItem siemensWriteCommandItem = new SiemensWriteCommandItem
            {
                CommandId = Guid.NewGuid().ToString("N"),
                DbNumber = 1,
                Offset = 0,
                PlcId = 1,
                Size = 8,
                Buffer = new byte[8] { 49, 49, 49, 49, 49, 49, 49, 49 }
            };


            SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();

            siemensPlcAccess.WriteDB(siemensWriteCommandItem, 1);
             
        }



    }
}
