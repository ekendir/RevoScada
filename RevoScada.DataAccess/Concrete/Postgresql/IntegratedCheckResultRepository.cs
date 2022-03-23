using Dapper;
using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class IntegratedCheckResultRepository : DapperGenericPostgreRepository<IntegratedCheckResult>
    {
        public IntegratedCheckResultRepository(string connectionString) : base(connectionString){}

        public IEnumerable<IntegratedCheckResult> GetByDate(DateTime startDate, DateTime endDate)
        {
            IEnumerable<IntegratedCheckResult> entities;

            try
            {

                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM public.\"IntegratedCheckResults\" WHERE \"CheckResultSaveDate\" BETWEEN  @startDate AND  @endDate ";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@startDate", startDate, DbType.DateTime);
                    dp.Add("@endDate", endDate, DbType.DateTime);
                    entities = connection.Query<IntegratedCheckResult>(query, dp);

                }
            }
            catch (Exception exception)
            {

                exception.Data.Add("DetailMessage", "IntegratedCheckResultRepository:GetByDate");
                throw exception;
            }
            return entities;
        }

        public IEnumerable<IntegratedCheckResult> GetByBatchIdAndDate(int batchId, DateTime startDate, DateTime endDate)
        {
            IEnumerable<IntegratedCheckResult> entities;

            try
            {

                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM public.\"IntegratedCheckResults\" WHERE  \"BatchId\"=@batchId AND \"CheckResultSaveDate\" BETWEEN  @startDate AND  @endDate ";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@batchId", batchId, DbType.Int32);
                    dp.Add("@startDate", startDate, DbType.DateTime);
                    dp.Add("@endDate", endDate, DbType.DateTime);
                    entities = connection.Query<IntegratedCheckResult>(query, dp);
                }
            }
            catch (Exception exception)
            {

                exception.Data.Add("DetailMessage", "IntegratedCheckResultRepository:GetByBatchIdAndDate");
                throw exception;
            }
            return entities;
        }

    }
}