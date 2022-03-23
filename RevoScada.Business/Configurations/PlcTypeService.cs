using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RevoScada.DataAccess.Abstract;

namespace RevoScada.Business.Configurations
{

    public class PlcTypeService
    {


        private string _connectionString { get; set; }

        private readonly IGenericRepository<PlcType> _repository;

        public PlcTypeService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.SqLite.PlcTypeRepository(_connectionString);
        }

        public IEnumerable<PlcType> GetAll(Expression<Func<PlcType, bool>> filter = null)
        {
            return _repository.GetAll();
        }

        public IEnumerable<PlcType> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public PlcType GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(PlcType entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(PlcType entity)
        {
            return _repository.Update(entity);
        }
    }
}
 
