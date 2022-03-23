using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities.Complex;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class RepositoryTest
    {
       
        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\Test_ReadService.rsconfig");



            // ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString = @"Data Source = C:\RevoScada.Files\SqliteDbFiles\TagLogData.db";
            // ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString = "Data Source=C:\\RevoOPC\\DBCollections\\TagLogData.db;";

        }

        [Test]
        [TestCase(0)]
        [TestCase(60)]
        public void Get_max_offset_tagconfig_query(int testcaseCount)
        {

            DapperGenericRepository<SiemensReadRequestItem> dapperGenericRepository = new DapperGenericRepository<SiemensReadRequestItem>(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var a = dapperGenericRepository.GetAllBySqlQuery("SELECT DBNumber, max(offset) MaxBufferSize  FROM  SiemensTagConfigurations  GROUP BY DBNumber");

            var count = a.ToList().Count();

            Assert.IsTrue(count > testcaseCount);

        }


        [Test]

        public void Get_furnaces()
        {

            FurnaceRepository furnaceRepository = new FurnaceRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString+"w");
            try
            {
 var furnaces = furnaceRepository.GetAll();

            var count = furnaces.ToList().Count();

            Assert.IsTrue(count > 0);
            }
            catch 
            {

                throw;
            }
           

        }

        [Test]
        public void Get_furnaces_by_Id()
        {

            FurnaceRepository furnaceRepository = new FurnaceRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var furnace = furnaceRepository.GetById(1);

            

            

        }







        [TearDown]
        public void Closing()
        {


            // close connection

            ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString = "Data Source = C:\\RevoScada.Files\\SqliteDbFiles\\TagListManagerLocal.db";

            // ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString = @"Data Source = C:\RevoScada.Files\SqliteDbFiles\TagLogData.db";
            // ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString = "Data Source=C:\\RevoOPC\\DBCollections\\TagLogData.db;";

        }


    }
}
