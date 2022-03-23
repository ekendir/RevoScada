using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;

using RevoScada.DataAccess.Abstract;
using System.Reflection;
using Npgsql;
using System.Threading.Tasks;
using System.CodeDom;
using NHibernate.Linq;
using NHibernate.AdoNet.Util;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    // Generic dapper repository for entities
    public class DapperGenericPostgreRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        protected string ConnectionString;

        protected string TableName;

        public DapperGenericPostgreRepository(string connectionString)
        {
            TableName = TableName ?? typeof(TEntity).Name + "s";
            ConnectionString = connectionString;
        }

        protected IDbConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        public string DBTableName()
        {
            return TableName;
        }

        public IEnumerable<TEntity> GetAllBySqlQuery(string queryText)
        {
            IEnumerable<TEntity> entities;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    entities = connection.Query<TEntity>(queryText);
                }
            }
            catch (Exception exception)
            {

                exception.Data.Add("DetailMessage", "Repository:GetAllBySqlQuery: " + queryText);
                throw exception;
            }
            return entities;
        }


        public TEntity GetById(int id)
        {
            TEntity entity;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    entity = connection.Query<TEntity>($"SELECT * FROM public.\"{ TableName }\" WHERE public.\"{ TableName }\".\"id\"=@Id", new { Id = id }).First();
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:GetById: " + id);
                throw exception;
            }
            return entity;
        }
        public TEntity GetById(long id)
        {
            TEntity entity;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    entity = connection.Query<TEntity>($"SELECT * FROM public.\"{ TableName }\" WHERE public.\"{ TableName }\".\"id\"=@Id", new { Id = id }).First();
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:GetById: " + id);
                throw exception;
            }
            return entity;
        }
        public long GetNextId()
        {
            long nextId = 0;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    nextId = Convert.ToInt64(connection.Query($"SELECT MAX(Id) FROM public.\"{ TableName }\"").First().max);
                    nextId++;
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:GetNextId:");
                throw exception;
            }
            return nextId;
        }

        public IEnumerable<TEntity> GetAll()
        {
            IEnumerable<TEntity> entities;
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    entities = connection.Query<TEntity>($"SELECT * FROM public.\"{ TableName }\"");
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:GetAll");
                throw exception;
            }
            return entities;
        }

        public bool Insert(TEntity entity, bool generateAutoId = true)
        {
            bool processResult = false;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    
                    if (generateAutoId)
                    {
                        long nextId = GetNextId();
                        dynamic tempEntity = (TEntity)(entity);
                        tempEntity.id = Convert.ChangeType(nextId, tempEntity.id.GetType());
                        entity = tempEntity;
                    }
                    
                    long insertResult = connection.Insert(entity);
                    processResult = (insertResult >= 0);

                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:Insert");
                throw exception;
            }
            return processResult;
        }

        public bool InsertOrUpdate(TEntity entity)
        {
            bool processResult = false;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    dynamic tempEntity = (TEntity)(entity);

                    TEntity existingEntity;

                    try
                    {
                       existingEntity = GetById(tempEntity.id);
                    }
                    catch (Exception)
                    {
                       existingEntity = null;
                    }
                   

                    long insertResult = 0;

                    if (existingEntity == null)
                    {
                        long nextId = GetNextId();
                        tempEntity.id = Convert.ChangeType(nextId, tempEntity.id.GetType());
                        entity = tempEntity;
                        insertResult = connection.Insert(entity);
                    }
                    else
                    {
                        processResult = Update(entity);
                    }

                    if (insertResult == 0)
                    {
                        processResult = true;
                    }
                    else
                    {

                    }


                    processResult = (insertResult >= 0) || processResult;

                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:InsertOrUpdate");
                throw exception;
            }
            return processResult;

        }


        public bool InsertMany(string sql, List<TEntity> entities,bool generateAutoId=true)
        {
            bool processResult = false;
            try
            {
                if (entities != null)
                {
                    using (IDbConnection connection = GetConnection())
                    {
                        connection.Open();
                        if (generateAutoId)
                        {
                            long nextId = GetNextId();
                            for (int i = 0; i < entities.Count; i++)
                            {
                                dynamic tempEntity = (TEntity)(entities[i]);
                                tempEntity.id = Convert.ChangeType(nextId, tempEntity.id.GetType());
                                entities[i] = tempEntity;
                                nextId++;
                            }
                        }
                        int insertResult = connection.Execute(sql, entities);
                        processResult = (insertResult == entities.Count);
                    }
                }
                else
                {
                    processResult = true;
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:InsertMany");
                throw exception;
            }
            return processResult;
        }
 



        public bool InsertOrUpdateMany(string sql, List<TEntity> entities)
        {
            bool processResult = false;
            try
            {
                if (entities != null)
                {
                    bool deleteResult = DeleteMany(entities);

                    if (deleteResult)
                    {
                        processResult = InsertMany(sql, entities,false);
                    }
                }
                else
                {
                    processResult = true;
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:UpsertMany");
                throw exception;
            }
            return processResult;
        }


        public bool Update(TEntity entity)
        {
            bool processResult = false;
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    processResult = connection.Update(entity);
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:Update");
                throw exception;
            }
            return processResult;
        }

        public bool Delete(TEntity entity)
        {
            bool processResult = false;
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    processResult = connection.Delete(entity);
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:Update");
                throw exception;
            }
            return processResult;
        }

        public bool DeleteMany(IEnumerable<TEntity> entities)
        {
            bool processResult = false;

            try
            {
                if (entities != null)
                {

                    using (IDbConnection connection = GetConnection())
                    {
                        connection.Open();

                            //todo:m check always false result...
                        processResult = connection.Delete(entities);
                        processResult = entities.Count() == 0 && processResult == false || processResult;


                        processResult = true;
                    }

                }
                else
                {
                    processResult = true;
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:Delete");
                throw exception;
            }

            return processResult;
        }

       
    }
}
