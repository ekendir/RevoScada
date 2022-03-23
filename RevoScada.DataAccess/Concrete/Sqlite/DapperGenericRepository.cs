using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data.SQLite;
using RevoScada.DataAccess.Abstract;
using System.Reflection;

namespace RevoScada.DataAccess.Concrete.SqLite 
{
    // Generic dapper repository for entities
    public class DapperGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        protected string ConnectionString;
        
        public DapperGenericRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private IDbConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
            
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
              TEntity  entity;

            try
            {
                using (IDbConnection connection = GetConnection())
                {

                    connection.Open();
                    entity = connection.Query<TEntity>($"SELECT * FROM { typeof(TEntity).Name}s WHERE {(typeof(TEntity).GetProperties())[0].Name}=@Id", new { Id = id }).FirstOrDefault();

                }

            }
            catch (Exception exception)
            {


                exception.Data.Add("DetailMessage", "Repository:GetById: "+ id);

                throw exception;


            }

            return entity;
        }

        public IEnumerable<TEntity> GetAll()
        {

            IEnumerable<TEntity> entities;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    entities = connection.GetAll<TEntity>().ToList();
                }
            }
            catch (Exception exception)
            {


                exception.Data.Add("DetailMessage", "Repository:GetAll");

                throw exception;

               
            }

            return entities;
        }

        public bool Insert(TEntity entity)
        {
            bool processResult = false;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                  //  SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                    connection.Open();
                    long insertResult = connection.Insert<TEntity>(entity);
                    processResult = (insertResult >= 0) ? true : false;
                }
            }
            catch (Exception exception)
            {


                exception.Data.Add("DetailMessage", "Repository:Insert");

                throw exception;


            }

            return processResult;
        }


        //public bool InsertMany(List<TEntity> entities)
        //{
        //     bool processResult = false;

        //    try
        //    {
        //        using (IDbConnection connection = GetConnection())
        //        {
        //            //  SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
        //            connection.Open();

        //            long insertResult = connection.Insert(entities);

                  
        //            connection.Execute("INSERT INTO [MyObject] (Id, ObjectType, Content, PreviewContent) VALUES(@Id, @ObjectType, @Content, @PreviewContent)", lst);



        //            processResult = (insertResult >= 0) ? true : false;
        //        }
        //    }
        //    catch (Exception exception)
        //    {


        //        exception.Data.Add("DetailMessage", "Repository:Insert");

        //        throw exception;


        //    }

        //    return processResult;





          

        //    }

        public bool Update(TEntity entity)
        {
            bool processResult = false;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    processResult = connection.Update<TEntity>(entity);
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
                    processResult = connection.Delete<TEntity>(entity);
                }
            }
            catch (Exception exception)
            {


                exception.Data.Add("DetailMessage", "Repository:Delete");

                throw exception;


            }

            return processResult;
        }
 
        public bool DeleteMany(IEnumerable<TEntity> entities)
        {
            bool processResult = false;

            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    processResult = connection.Delete(entities);
                }
            }
            catch (Exception exception)
            {


                exception.Data.Add("DetailMessage", "Repository:Delete");

                throw exception;


            }

            return processResult;
        }

        public bool InsertMany(string sql, List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public bool InsertOrUpdateMany(string sql, List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public bool InsertOrUpdate(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public string DBTableName()
        {
            throw new NotImplementedException();
        }

        public bool InsertMany(string sql, List<TEntity> entities, bool generateAutoId = false)
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(long id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(TEntity entity, bool generateAutoId = true)
        {
            throw new NotImplementedException();
        }
    }
}
