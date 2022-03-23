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
   public class PredefinedRecipeFieldService
    {

        private string _connectionString { get; set; }

        private readonly  IGenericRepository<PredefinedRecipeField> _repository;

        public PredefinedRecipeFieldService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.PredefinedRecipeFieldRepository(_connectionString);

        }

        public IEnumerable<PredefinedRecipeField> GetAll()
        {
            
            return _repository.GetAll();
        }

        public IEnumerable<PredefinedRecipeField> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public PredefinedRecipeField GetById(int id)
        {
            return _repository.GetById(id);
        }
    }
}
