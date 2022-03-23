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
   public class BagService:GenericService<Bag>
    {
     // private string _connectionString { get; set; }
     // private readonly  IGenericRepository<Bag> _repository;
     // protected readonly IGenericRepository<Bag> _repository;

        public BagService(string connectionString):base(connectionString)
        {
           _repository = new DataAccess.Concrete.Postgresql.BagRepository(_connectionString);
        }

        public IEnumerable<Bag> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public Bag GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(Bag entity)
        {
            return _repository.Insert(entity);
        }
        public bool InsertOrUpdateMany(List< Bag> entity)
        {
            string sql = "INSERT INTO public.\"Bags\" (id, \"BatchId\", \"BagName\", \"SelectedPorts\") VALUES (@id, @BatchId, @BagName, @SelectedPorts); ";
            return _repository.InsertOrUpdateMany(sql,entity);
        }

        public bool Update(Bag entity)
        {
            return _repository.Update(entity);
        }

        public IEnumerable<Bag> BagsByBatch(int batchId)
        {
          return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"Bags\" WHERE  \"BatchId\" ={batchId} ORDER BY id ASC;");
        }

        public bool Delete(Bag entity)
        {
            return _repository.Delete(entity);
        }

        public bool InsertOrUpdate(Bag entity)
        {
            return _repository.InsertOrUpdate(entity);
        }
    }
}
