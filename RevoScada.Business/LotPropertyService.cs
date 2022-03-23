using System;
using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
   public class LotPropertyService:GenericService<LotProperty>
    {
        
        public LotPropertyService(string connectionString):base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.LotPropertyRepository(_connectionString);
        }
        public IEnumerable<LotProperty> GetByBagIdListProperties(List<int> bagIdList)
        {
            if (bagIdList.Count > 0)
            {
                string bagIds = string.Join(",", bagIdList.Select(x => x.ToString()).ToArray());
                IEnumerable<LotProperty> result = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"LotProperties\" WHERE \"BagId\" IN({bagIds}) ORDER BY id DESC;");
                return result;
            }
            else
            {
                return new List<LotProperty>();
            }
        }
        public IEnumerable<LotProperty> GetByBagId(int bagId)
        {
            IEnumerable<LotProperty> result = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"LotProperties\" WHERE \"BagId\"= {bagId} ORDER BY id;");
            return result;
        }
        public IEnumerable<LotProperty> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }
        public LotProperty GetById(int id)
        {
            return _repository.GetById(id);
        }
        public bool Insert(LotProperty entity)
        {
            return _repository.Insert(entity);
        }
        public bool Update(LotProperty entity)
        {
            return _repository.Update(entity);
        }
        public bool Delete(LotProperty entity)
        {
            return _repository.Delete(entity);
        }
        public bool InsertOrUpdateMany(List<LotProperty> entities)
        {
            string sql= "INSERT INTO public.\"LotProperties\"(id, \"BagId\", \"SoirNumber\", \"PartName\", \"ToolName\") VALUES (@id, @BagId, @SoirNumber, @PartName, @ToolName);";
            
            bool result = _repository.InsertOrUpdateMany(sql,entities);
            return result;
        }

        public bool InsertOrUpdate(LotProperty entity)
        {
            return _repository.InsertOrUpdate(entity);
        }
    }
}
