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
   public class RecipeService:GenericService<Recipe>
    {
        public RecipeService(string connectionString):base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.RecipeRepository(_connectionString);
        }

        public IEnumerable<Recipe> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<Recipe> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }


        public Recipe ActiveSelectedRecipe()
        {
            CurrentProcessInfoService currentProcessInfoService = new CurrentProcessInfoService(_connectionString);
            CurrentProcessInfo currentProcessInfo= currentProcessInfoService.Get();
            Recipe recipe = (currentProcessInfo.ActiveRecipeId == 0) ? null : GetById(currentProcessInfo.ActiveRecipeId);
            return recipe;
        }

        public Recipe GetById(int id)
        {
            return _repository.GetById(id);
        }
 
        public bool Insert(Recipe entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertOrUpdate(Recipe entity)
        {
            return _repository.InsertOrUpdate(entity);
        }

        public bool InsertOrUpdateMany(List<Recipe> recipes)
        {
            string sql = "INSERT INTO public.\"Recipes\"(  id, \"RecipeName\", \"Description\", \"RecipeGroupId\", \"CreateDate\", \"ModifyDate\", \"LastRunDate\", \"IsActive\", \"CreatedByUserId\", \"ModifiedByUserId\", \"DeletedByUserId\") VALUES(@id, @RecipeName, @Description, @RecipeGroupId, @CreateDate, @ModifyDate, @LastRunDate, @IsActive, @CreatedByUserId, @ModifiedByUserId, @DeletedByUserId);";
            
            return _repository.InsertOrUpdateMany(sql,recipes);
        }


        public bool Update(Recipe entity)
        {
            return _repository.Update(entity);
        }

      
    }
}
