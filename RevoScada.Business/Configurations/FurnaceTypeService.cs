using RevoScada.Entities.Configuration;
//using RevoScada.DataAccess.Concrete.SqLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RevoScada.DataAccess.Abstract;

namespace RevoScada.Business.Configurations
{

    public class FurnaceTypeService 
    {

        private string _connectionString { get; set; }

        private readonly IGenericRepository<FurnaceType> _repository;

        public FurnaceTypeService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.SqLite.FurnaceTypeRepository(_connectionString);
             
        }

        public IEnumerable<FurnaceType> GetAll(Expression<Func<FurnaceType, bool>> filter = null)
        {
            return _repository.GetAll();
        }

        public IEnumerable<FurnaceType> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public FurnaceType GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(FurnaceType entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(FurnaceType entity)
        {
            return _repository.Update(entity);
        }

    }
}