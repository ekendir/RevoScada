using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.Postgresql;
using RevoScada.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class ApplicationPropertyRepositoryTest
    {
        ApplicationPropertyRepository repository;

        [SetUp]
        public void Init()
        {
          repository = new ApplicationPropertyRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        [TestCase("PTCCount")]
        public void Get_by_sql_query(string applicationPropertyName)
        {
            var entity = repository.GetAllBySqlQuery($"SELECT * FROM public.\"ApplicationProperties\" WHERE \"Name\"='{applicationPropertyName}'").FirstOrDefault();

            Assert.IsTrue(Convert.ToInt32( entity.Value) > 0);
        }


        [Test]
        [TestCase("PTCCount","101")]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update(string applicationPropertyName, string testValue)
        {
            var entity = repository.GetAllBySqlQuery($"SELECT * FROM public.\"ApplicationProperties\" WHERE \"Name\"='{applicationPropertyName}'").FirstOrDefault();
            string initialValue = entity.Value;
            entity.Value = testValue;
            repository.Update(entity);

            entity = repository.GetAllBySqlQuery($"SELECT * FROM public.\"ApplicationProperties\" WHERE \"Name\"='{applicationPropertyName}'").FirstOrDefault();
            Assert.IsTrue(entity.Value == "101");


            //Update back to initial value
            entity.Value = initialValue;
            repository.Update(entity);
            entity = repository.GetAllBySqlQuery($"SELECT * FROM public.\"ApplicationProperties\" WHERE \"Name\"='{applicationPropertyName}'").FirstOrDefault();

            Assert.IsTrue(entity.Value == initialValue);

        }

 

    }
}
