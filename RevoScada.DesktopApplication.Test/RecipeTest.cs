 
using System;
using Newtonsoft.Json;
using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.PageTagConfigurations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RevoScada.Entities;

namespace RevoScada.DesktopApplication.Test
{
    [TestFixture]
    public class RecipeTest
    {


        List<RecipeDetailCellData> RecipeDetailCellDataList { get; set; }

        [SetUp]
        public void Init()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);

            RecipeDetailCellDataList = new List<RecipeDetailCellData>();
        }


        [Test]
        public void Change_cell_value()
        {

            {// bound grid 


                RecipeDetail recipeDetail = new RecipeDetail();
                recipeDetail.id = 16392;
                recipeDetail.RecipeFieldId = 0;
                recipeDetail.SegmentNo = 0;
                recipeDetail.RecipeFieldValue = "valueOld";

                RecipeDetailCellData recipeDetailCellView = new RecipeDetailCellData();
                recipeDetailCellView.RecipeDetail = recipeDetail;
                recipeDetailCellView.CellChangeState = CellChangeStates.Unchanged;


                // butun recipeDetail ler bu şekilde yüklenecek. recipe editor açıldığında. sayfa geçişlerin...
                RecipeDetailCellDataList.Add(recipeDetailCellView);


            }


            {// textbox changed event mock

                RecipeDetail recipeDetailComesFromEventParameter = new RecipeDetail();
                recipeDetailComesFromEventParameter.id = 16392;
                recipeDetailComesFromEventParameter.RecipeFieldId = 0;
                recipeDetailComesFromEventParameter.SegmentNo = 0;
                recipeDetailComesFromEventParameter.RecipeFieldValue = "newValue";


                RecipeDetailCellData recipeDetailCellView = new RecipeDetailCellData();
                recipeDetailCellView.RecipeDetail = recipeDetailComesFromEventParameter;
                recipeDetailCellView.CellChangeState = CellChangeStates.Modified;

                //RecipeDetailCellView recipeDetailCellView = RecipeDetailCellList2.First(x => x.RecipeDetail.id == 16392);
                //recipeDetailCellView.RecipeDetail = recipeDetailComesFromEventParameter;
                //recipeDetailCellView.CellChangeState = CellChangeStates.Modified;

                // RecipeDetailCellList2.First(x => x.RecipeDetail.id == 16392) = recipeDetailCellView;

                int index = RecipeDetailCellDataList.FindIndex(p => p.RecipeDetail.id == 16392);
                RecipeDetailCellDataList[index] = recipeDetailCellView;



            }

        }


        [Test]
        public void Save_changes()
        {
            Change_cell_value();

            RecipeDetailService recipeDetailService = new RecipeDetailService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            foreach (var item in RecipeDetailCellDataList)
            {

                switch (item.CellChangeState)
                {
                    case CellChangeStates.Modified:
                        recipeDetailService.Update(item.RecipeDetail);
                        break;
                    case CellChangeStates.Added:
                        recipeDetailService.Insert(item.RecipeDetail);
                        break;
                    case CellChangeStates.Deleted:
                        recipeDetailService.Delete(item.RecipeDetail);
                        break;
                    default:
                        break;
                }

            }






        }

        public class RecipeDetailCellData
        {
            // public int RecipeDetailId { get; set; }
            public RecipeDetail RecipeDetail { get; set; }
            public CellChangeStates CellChangeState { get; set; }
        }

        public enum CellChangeStates
        {
            Modified, Added, Deleted, Unchanged
        }

    }
}