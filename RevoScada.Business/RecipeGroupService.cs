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
   public class RecipeGroupService
    {
        

        private string _connectionString { get; set; }

        private readonly IGenericRepository<RecipeGroup> _repository;

        public RecipeGroupService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.RecipeGroupRepository(_connectionString);

        }

        public IEnumerable<RecipeGroup> GetAll()
        {
            
            return _repository.GetAll();
        }

        public IEnumerable<RecipeGroup> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public RecipeGroup GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(RecipeGroup entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertOrUpdateMany(List<RecipeGroup> entities)
        {
            string sql = "INSERT INTO public.\"RecipeGroups\"( id, \"GroupName\") VALUES (@id, @GroupName); ";
            return _repository.InsertOrUpdateMany(sql,entities);
        }

        public bool Update(RecipeGroup entity)
        {
            return _repository.Update(entity);
        }

        public bool Delete(RecipeGroup entity)
        {
            return _repository.Delete(entity);
        }

        public bool InsertOrUpdate(RecipeGroup entity)
        {
          return  _repository.InsertOrUpdate(entity);
        }
    }
}
