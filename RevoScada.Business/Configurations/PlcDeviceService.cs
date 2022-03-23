using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RevoScada.Entities.Complex;
using RevoScada.DataAccess.Abstract;

namespace RevoScada.Business.Configurations
{
    
        public class PlcDeviceService
        {

        private string _connectionString { get; set; }

        private readonly IGenericRepository<PlcDevice> _repository;

        public PlcDeviceService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.SqLite.PlcDeviceRepository(_connectionString);
        }


        public IEnumerable<PlcDevice> GetAll(Expression<Func<PlcDevice, bool>> filter = null)
            {
                return _repository.GetAll();
            }

            public IEnumerable<PlcDevice> GetAllBySqlQuery(string queryText)
            {
                return _repository.GetAllBySqlQuery(queryText);
            }

            public PlcDevice GetById(int id)
            {
                return _repository.GetById(id);
            }

            public bool Insert(PlcDevice entity)
            {
                return _repository.Insert(entity);
            }

            public bool Update(PlcDevice entity)
            {
                return _repository.Update(entity);
            }

        

        }
    }
 
