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
   public class RecipeFieldService
    {


        private string _connectionString { get; set; }

        private readonly  IGenericRepository<RecipeField> _repository;

        public RecipeFieldService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.RecipeFieldRepository(_connectionString);

        }

        public IEnumerable<RecipeField> GetAll()
        {
           
            return _repository.GetAll();
        }

        public IEnumerable<RecipeField> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public RecipeField GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(RecipeField entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(RecipeField entity)
        {
            return _repository.Update(entity);
        }
    }
}
