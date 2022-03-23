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
   public class BatchQualityDetailService:GenericService<BatchQualityDetail>
    {
        public BatchQualityDetailService(string connectionString):base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.BatchQualityDetailRepository(_connectionString);
        }
        public IEnumerable<BatchQualityDetail> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public IEnumerable<BatchQualityDetail> GetAllByQualityBatchId(int qualityBatchId)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"BatchQualityDetails\" WHERE  \"BatchQualityId\" ={qualityBatchId};");
        }

        public BatchQualityDetail GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(BatchQualityDetail entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(BatchQualityDetail entity)
        {
            return _repository.Update(entity);
        }

        public bool Delete(BatchQualityDetail entity)
        {
            return _repository.Delete(entity);
        }
    }
}
