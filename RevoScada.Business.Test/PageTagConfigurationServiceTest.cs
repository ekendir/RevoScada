
using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Test
{

    [TestFixture]
    public class PageTagConfigurationServiceTest
    {
        PageTagConfigurationService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new  PageTagConfigurationService (TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
      
        public void Get_by_sql_query()
        {
            var entity = _service.GetByName("VacuumLines");

            Assert.IsTrue(Convert.ToInt32(entity.PageTagConfigurations.ToString().Count()) > 10);
        }

 
      
        [TearDown]
        public void Closing()
        {
        }
    }


}
