using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
   public class RecipeDetailHistoryService:GenericService<RecipeDetailHistory>
    {
        public RecipeDetailHistoryService(string connectionString) : base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.RecipeDetailHistoryRepository(_connectionString);
        }

        public IEnumerable<RecipeDetailHistory> GetByBatch(int recipeId,int batchId)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"RecipeDetailHistories\" WHERE  \"RecipeId\" = {recipeId} AND  \"BatchId\" = {batchId};");
        }

        public int GetSegmentTotal(int batchId)
        {
            int maxSegmentNo = _repository.GetAllBySqlQuery($"SELECT MAX(\"SegmentNo\") as \"SegmentNo\" FROM public.\"RecipeDetailHistories\" where \"BatchId\"= {batchId}").FirstOrDefault().SegmentNo;
            return maxSegmentNo;
        }



        public bool DeleteAll(int recipeId, int batchId)
        {
            IEnumerable<RecipeDetailHistory> list = GetByBatch(recipeId, batchId);
            bool result = _repository.DeleteMany(list);
            return result;
        }

        public RecipeDetailHistory GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(RecipeDetailHistory entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertMany(List<RecipeDetailHistory> recipeHistories)
        {
            string sql = "INSERT INTO public.\"RecipeDetailHistories\"(id,\"RecipeId\", \"SegmentNo\", \"RecipeFieldId\", \"RecipeCellValue\", \"BatchId\") VALUES(@id,@RecipeId, @SegmentNo, @RecipeFieldId, @RecipeCellValue, @BatchId); ";
 
            bool insertResult=_repository.InsertMany(sql, recipeHistories,true);

            return insertResult;
        }


        public bool Update(RecipeDetailHistory entity)
        {
            return _repository.Update(entity);
        }
        public bool InsertOrUpdateMany(List<RecipeDetailHistory> recipeHistories)
        {
            string sql = "INSERT INTO public.\"RecipeDetailHistories\"(id,\"RecipeId\", \"SegmentNo\", \"RecipeFieldId\", \"RecipeCellValue\", \"BatchId\") VALUES(@id,@RecipeId, @SegmentNo, @RecipeFieldId, @RecipeCellValue, @BatchId); ";

            bool insertResult = _repository.InsertOrUpdateMany(sql, recipeHistories);
            return insertResult;
        }

        public long GetMaxId()
        {
            long maxId = _repository.GetAllBySqlQuery($"SELECT Max(id) as id FROM public.\"RecipeDetailHistories\" ").FirstOrDefault().id;
            return maxId;
        }
    }
}
