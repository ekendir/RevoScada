using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
   public class ActiveTagService
    {
        private string _connectionString { get; set; }

        private readonly  IGenericRepository<ActiveTag> _repository;

        public ActiveTagService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.ActiveTagRepository(_connectionString);
        }

        public IEnumerable<ActiveTag> GetAll()
        {
            return _repository.GetAll();
        }

        public Dictionary<string, ActiveTag> ActiveTagsByTagNameKey()
        {
            Dictionary<string, ActiveTag> dictionaryResult=_repository.GetAll().ToDictionary(x=>x.TagName,x=>x);

            return dictionaryResult;
        }

        public Dictionary<int, ActiveTag> ActiveTagsByTagIdKey()
        {
            Dictionary<int, ActiveTag> dictionaryResult = _repository.GetAll().ToDictionary(x => x.id, x => x);

            return dictionaryResult;
        }

        public IEnumerable<ActiveTag> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public ActiveTag GetById(int id)
        {
            return _repository.GetById(id);
        }
     
        public bool Update(ActiveTag entity)
        {
            return _repository.Update(entity);
        }

        public bool InsertOrUpdateMany(List<ActiveTag> entities)
        {
            string sql = "INSERT INTO public.\"ActiveTags\" (id, \"IsLogData\", \"TagName\",  \"ActiveTagGroupId\") VALUES ( @id,@IsLogData, @TagName, @ActiveTagGroupId);";
            return _repository.InsertOrUpdateMany(sql,entities);
        }
       
    }
}
