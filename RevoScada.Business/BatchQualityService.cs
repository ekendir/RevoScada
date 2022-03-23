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
   public class BatchQualityService: GenericService<BatchQuality>
    {
        public BatchQualityService(string connectionString):base(connectionString)
        { 
            _repository = new DataAccess.Concrete.Postgresql.BatchQualityRepository(_connectionString);
        }

        public IEnumerable<BatchQuality> GetAll()
        {
            return _repository.GetAll();
        }
 
        public IEnumerable<BatchQuality> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public BatchQuality GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(BatchQuality entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(BatchQuality entity)
        {
            return _repository.Update(entity);
        }

        public bool Delete(BatchQuality entity)
        {
            return _repository.Delete(entity);
        }
    }
}
