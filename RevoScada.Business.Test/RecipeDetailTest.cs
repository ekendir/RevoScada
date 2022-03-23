using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Entities;
using RevoScada.Configurator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using RevoScada.Business;

namespace RevoScada.Business.Test
{
   

    [TestFixture]
    public class RecipeDetailServiceTest
    {
        private RecipeDetailService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new RecipeDetailService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var list = _service.GetAllByRecipeId(31).ToList();
        
        
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = _service.GetById(2);

            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            RecipeDetail entity = new RecipeDetail();
            entity.RecipeId = 1;
            entity.SegmentNo = 1;
            entity.RecipeFieldId = 1;
            entity.RecipeFieldValue = "value1";

            bool insertResult =   _service.Insert(entity);

            Assert.IsTrue(insertResult);
        }

        [Test]
        public void GetActiveSegmentTotal()
        {
          
            var insertResult = _service.GetSegmentTotal(1);

           // Assert.IsTrue(insertResult);
        }




        //[Test]
        //public void Insert_Many()
        //{

        //    List<RecipeDetail> recipeDetails = new List<RecipeDetail>();


        //    for (int i = 0; i < 400; i++)
        //    {
        //        RecipeDetail entity = new RecipeDetail();
        //        entity.RecipeId = 0;
        //        entity.SegmentNo = 1;
        //        entity.RecipeFieldId = 1;
        //        entity.RecipeFieldValue = "value1";
        //        recipeDetails.Add(entity);
        //    }


        //    Stopwatch stopwatch = new Stopwatch();

        //    stopwatch.Start();




        //    Task task = _service.InsertMany(recipeDetails);


        //    // task.Start();

        //    task.Wait();

        //    Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

        //    // Assert.IsTrue(entity.Value == initialValue);

        //}



        [Test]
        public void Update()
        {
            var entity = _service.GetById(1);

            entity.RecipeFieldValue = "value"+ DateTime.Now.Millisecond.ToString();


            bool insertResult = _service.Update(entity);

            Assert.IsTrue(insertResult);
        }

        [TearDown]
        public void Closing()
        {
        }
    }


}
