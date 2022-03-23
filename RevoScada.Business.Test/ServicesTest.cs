 
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class ServicesTest
    {
        private string _connectionString;


        [SetUp]
        public void Init()
        {
            
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }

        [Test]
        public void Get_read_request_items()
        {

            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(_connectionString);

            var readRequestItems = siemensTagConfigurationService.ReadRequestItems(0);

            var count = readRequestItems.ToList().Count();

            Assert.IsTrue(count > 0);

        }


        [Test]
        public void Get_read_request_items_with_configrator_library()
        {

            
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var readRequestItems = siemensTagConfigurationService.ReadRequestItems(0);

            var count = readRequestItems.ToList().Count();

            Assert.IsTrue(count > 0);

         }





        [TearDown]
        public void Closing()
        {


            // close connection

            _connectionString = "Data Source = C:\\RevoScada.TAI.Files\\SqliteDbFiles\\TagListManagerLocal.db";

            // _connectionString = @"Data Source = C:\RevoScada.TAI.Files\SqliteDbFiles\TagLogData.db";
            // _connectionString = "Data Source=C:\\RevoOPC\\DBCollections\\TagLogData.db;";

        }


    }
}




// sample test parameter usage

//[Test]
//[TestCase(0)]
//[TestCase(60)]

//public void AAA(int aaa)
//{

//}