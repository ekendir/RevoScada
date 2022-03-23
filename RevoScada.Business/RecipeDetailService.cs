using RevoScada.Entities;
using System.Collections.Generic;
using RevoScada.DataAccess.Abstract;
using System.Linq;
using System;

namespace RevoScada.Business
{
   public class RecipeDetailService:GenericService<RecipeDetail>
    {

        public RecipeDetailService(string connectionString) : base(connectionString)
        {
              _repository = new DataAccess.Concrete.Postgresql.RecipeDetailRepository(_connectionString);
        }

        public IEnumerable<RecipeDetail> GetAllByRecipeId(int recipeId)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM public.\"RecipeDetails\" where \"RecipeId\"={recipeId}");
        }

        public int GetSegmentTotal(int recipeId)
        {
            int maxSegmentNo = _repository.GetAllBySqlQuery($"SELECT MAX(\"SegmentNo\") as \"SegmentNo\" FROM public.\"RecipeDetails\" where \"RecipeId\"= {recipeId}").FirstOrDefault().SegmentNo;
            return maxSegmentNo;
        }

        public IEnumerable<RecipeDetail> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public RecipeDetail GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(RecipeDetail entity, bool generateAutoId = true)
        {
            return   _repository.Insert(entity, generateAutoId);
        }

        public bool Delete(RecipeDetail entity)
        {
            return _repository.Delete(entity);
        }

        //public bool DeleteMany(List<RecipeDetail> recipeDetails)
        //{
        //    List<bool> checkResults = new List<bool>();

        //   var temp = GetAllByRecipeId(recipeDetails[0].RecipeId);

          
        //    foreach (RecipeDetail entity in temp)
        //    {
        //        bool result = _repository.Delete(entity);
        //        checkResults.Add(result); 
        //    }

        //    foreach (RecipeDetail entity in temp)
        //    {
        //        bool result = _repository.Insert(entity);
        //        checkResults.Add(result);
        //    }

        //    return checkResults.Count > 0 && checkResults.TrueForAll(x => x == true);
        //}
        public bool Update(RecipeDetail entity)
        {
            return _repository.Update(entity);
        }

        public bool InsertOrUpdateMany(List<RecipeDetail> recipeDetails)
        {
            string  sql =   "INSERT INTO public.\"RecipeDetails\"(id, \"RecipeId\", \"SegmentNo\", \"RecipeFieldId\", \"RecipeFieldValue\") VALUES (@id, @RecipeId, @SegmentNo, @RecipeFieldId, @RecipeFieldValue);";
            bool insertResult = _repository.InsertOrUpdateMany(sql, recipeDetails);
            return insertResult;
        }
     

        public bool InsertOrUpdate(RecipeDetail entity)
        {
            return _repository.InsertOrUpdate(entity);
        }

        
    }
}
