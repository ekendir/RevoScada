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
    public class BatchQualityDetailRepositoryTest
    {
        BatchQualityDetailRepository repository;
        

        [SetUp]
        public void Init()
        {
          repository = new BatchQualityDetailRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var entity = repository.GetAll().FirstOrDefault();

            Assert.IsTrue(entity.id == 1);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = repository.GetById(1);

            Assert.IsTrue(entity.id == 1);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {

           BatchQualityDetail entity = new BatchQualityDetail
            {
                BatchQualityId = 1,
                PhaseName = "phase name test",
                LastModified = DateTime.Now,
                PhaseStyle = true,
                PhaseTitle = "phase title test",
                PhaseChange = "phase change",
                PhaseCriteria = "phase criteria test",
                PhaseMinTime = 0,
                PhaseMaxTime = 0,
                AirTempStyle = false,
                AirTempTitle = "air temp title test",
                AirTempMin = 0,
                AirTempMax = 0,
                PressureStyle = false,
                PressureTitle = "PressureTitle test",
                PressureRateMin = 0,
                PressureRateMax = 0,
                PressurePhaseStartMin = 0,
                PressurePhaseStartMax = 0,
                PressurePhaseEndMin = 0,
                PressurePhaseEndMax = 0,
                ProbeStyle = false,
                ProbeTitle = "ProbeTitle test",
                ProbePhaseStartMin = 0,
                ProbePhaseStartMax = 0,
                ProbePhaseEndMin = 0,
                ProbePhaseEndMax = 0,
                PartTempStyle = false,
                PartTempTitle = "PartTempTitle test",
                PartTempRateMin = 0,
                PartTempRateMax = 0,
                PartTempLowRange = 0,
                PartTempHighRange = 0,
                SortOrder = 0,
                PartTempRateCalcInterval = 0
            };

           bool insertResult = repository.Insert(entity);
           
           Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            DateTime modifiedDate = DateTime.Now;

            var entity=repository.GetById(1);

            entity.LastModified = modifiedDate;

            bool updateResult = repository.Update(entity);

            Assert.IsTrue(updateResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Delete()
        {
            BatchQualityDetail entity = new BatchQualityDetail
            {   
                id=0,
                BatchQualityId = 1,
                PhaseName = "phase name test",
                LastModified = DateTime.Now,
                PhaseStyle = true,
                PhaseTitle = "phase title test",
                PhaseChange = "phase change",
                PhaseCriteria = "phase criteria test",
                PhaseMinTime = 0,
                PhaseMaxTime = 0,
                AirTempStyle = false,
                AirTempTitle = "air temp title test",
                AirTempMin = 0,
                AirTempMax = 0,
                PressureStyle = false,
                PressureTitle = "PressureTitle test",
                PressureRateMin = 0,
                PressureRateMax = 0,
                PressurePhaseStartMin = 0,
                PressurePhaseStartMax = 0,
                PressurePhaseEndMin = 0,
                PressurePhaseEndMax = 0,
                ProbeStyle = false,
                ProbeTitle = "ProbeTitle test",
                ProbePhaseStartMin = 0,
                ProbePhaseStartMax = 0,
                ProbePhaseEndMin = 0,
                ProbePhaseEndMax = 0,
                PartTempStyle = false,
                PartTempTitle = "PartTempTitle test",
                PartTempRateMin = 0,
                PartTempRateMax = 0,
                PartTempLowRange = 0,
                PartTempHighRange = 0,
                SortOrder = 0,
                PartTempRateCalcInterval = 0
            };

            bool insertResult = repository.Insert(entity);
            bool deleteResult = repository.Delete(entity);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(deleteResult);

        }

    }
}
