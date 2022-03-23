using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RevoScada.DataAccess.Abstract
{
    public interface IGenericRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> GetAllBySqlQuery(string queryText);

        TEntity GetById(int id);
        TEntity GetById(long id);

        bool Insert(TEntity entity, bool generateAutoId = true);
        bool InsertOrUpdate(TEntity entity);
        
        bool InsertMany(string sql, List<TEntity> entities, bool generateAutoId=false);

        bool InsertOrUpdateMany(string sql, List<TEntity> entities);
        

        bool Update(TEntity entity);

        bool Delete(TEntity entity);

        bool DeleteMany(IEnumerable<TEntity> entities);
        string DBTableName();
    }
}
