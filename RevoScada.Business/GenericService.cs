using RevoScada.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
    public class GenericService<T>
    {
        protected string _connectionString { get; set; }
        protected IGenericRepository<T> _repository;
        public GenericService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<T> GetAllMissingData(dynamic minRequestedId)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"{_repository.DBTableName()}\"  WHERE id >{minRequestedId};");
        }
        
        public dynamic GetMaxId()
        {
            dynamic entity = _repository.GetAllBySqlQuery($"SELECT Max(id) as id FROM public.\"{_repository.DBTableName()}\" ").FirstOrDefault();
            return entity.id;
        }
    }
}
