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
    public class CurrentProcessInfoServiceTest
    {
        private CurrentProcessInfoService _service;
    

        [SetUp]
        public void Init()
        {
             _service = new CurrentProcessInfoService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get()
        {
            var entity = _service.Get();
        }

  
    }

}
