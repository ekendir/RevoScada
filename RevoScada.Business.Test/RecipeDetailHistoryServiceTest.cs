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
    public class RecipeDetailHistoryServiceTest
    {
        private RecipeDetailHistoryService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new RecipeDetailHistoryService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }


        [Test]
        public void GetById()
        {
         

            RecipeDetailHistoryService service = new RecipeDetailHistoryService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);

            var recipeDetailList = service.GetById(1);



        }


        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void InsertMany()
        {
            int recipeId = 123;

            int batchId = 356;


            RecipeDetailService recipeDetailService = new RecipeDetailService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);

            var recipeDetailList=  recipeDetailService.GetAllByRecipeId(recipeId).ToList();

            RecipeDetailHistoryService recipeDetailHistoryService = new RecipeDetailHistoryService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);


            var list = (from s in recipeDetailList select new RecipeDetailHistory { 
              RecipeId=s.RecipeId,
              SegmentNo=s.SegmentNo,
              RecipeFieldId=s.RecipeFieldId,
              RecipeCellValue=s.RecipeFieldValue,
              BatchId= batchId
            }).ToList();





       //     recipeDetailHistoryService.InsertMany(list);


        }



        [TearDown]
        public void Closing()
        {
        }
    }


}
