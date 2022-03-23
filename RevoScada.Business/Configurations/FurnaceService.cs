
using RevoScada.DataAccess.Abstract;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RevoScada.Business.Configurations
{

    public class FurnaceService
    {

        private string _connectionString { get; set; }

        private readonly IGenericRepository<Furnace> _repository;

        public FurnaceService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.SqLite.FurnaceRepository(_connectionString);

        }

        public IEnumerable<Furnace> GetAll(Expression<Func<Furnace, bool>> filter = null)
        {
            return _repository.GetAll();
        }

        public IEnumerable<Furnace> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public Furnace GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(Furnace entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(Furnace entity)
        {
            return _repository.Update(entity);
        }
    }
}
 
