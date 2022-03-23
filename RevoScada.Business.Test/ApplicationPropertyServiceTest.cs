
using Newtonsoft.Json;
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
    public class ApplicationPropertyServiceTest
    {
        ApplicationPropertyService _service;

        [SetUp]
        public void Init()
        {
            _service = new ApplicationPropertyService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        [TestCase("PTCCount")]
        public void Get_by_sql_query(string applicationPropertyName)
        {
            var entity = _service.GetByName(applicationPropertyName);

            Assert.IsTrue(Convert.ToInt32(entity.Value) > 0);
        }


        [Test]
        [TestCase("LastLoadNumber", "101")]

        public void Update(string applicationPropertyName, string testValue)
        {
            var entity = _service.GetByName(applicationPropertyName);
            string initialValue = entity.Value;
            entity.Value = testValue;
            _service.Update(entity);

            entity = _service.GetByName(applicationPropertyName);
            Assert.IsTrue(entity.Value == "101");

            //Update back to initial value
            entity.Value = initialValue;
            _service.Update(entity);
            entity = _service.GetByName(applicationPropertyName);
            Assert.IsTrue(entity.Value == initialValue);
            Assert.IsTrue(entity.Value == initialValue);
        }

        [Test]
        public void get_trend_chart_string_array()
        {
            var entity = _service.GetByName("UI_LiveTrend_SelectedPorts");
            List<string> s = JsonConvert.DeserializeObject<List<string>>(entity.Value);
            Assert.IsTrue(Convert.ToInt32(entity.Value) > 0);
        }
        
        [TearDown]
        public void Closing()
        {
        }
    }
}
