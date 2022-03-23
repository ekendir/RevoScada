using Dapper;
using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class SkippedIntegratedCheckResultRepository : DapperGenericPostgreRepository<SkippedIntegratedCheckResult>
    {
        public SkippedIntegratedCheckResultRepository(string connectionString) : base(connectionString){}

        public IEnumerable<SkippedIntegratedCheckResult> GetByDate(DateTime startDate, DateTime endDate)
        {
            IEnumerable<SkippedIntegratedCheckResult> entities;

            try
            {

                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM public.\"SkippedIntegratedCheckResults\" WHERE \"SkipDate\" BETWEEN  @startDate AND  @endDate ";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@startDate", startDate, DbType.DateTime);
                    dp.Add("@endDate", endDate, DbType.DateTime);
                    entities = connection.Query<SkippedIntegratedCheckResult>(query, dp);

                }
            }
            catch (Exception exception)
            {

                exception.Data.Add("DetailMessage", "SkippedIntegratedCheckResultsRepository:GetByDate");
                throw exception;
            }
            return entities;
        }

        public IEnumerable<SkippedIntegratedCheckResult> GetByBatchIdAndDate(int batchId, DateTime startDate, DateTime endDate)
        {
            IEnumerable<SkippedIntegratedCheckResult> entities;

            try
            {

                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM public.\"SkippedIntegratedCheckResults\" WHERE  \"BatchId\"=@batchId AND \"SkipDate\" BETWEEN  @startDate AND  @endDate ";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@batchId", batchId, DbType.Int32);
                    dp.Add("@startDate", startDate, DbType.DateTime);
                    dp.Add("@endDate", endDate, DbType.DateTime);
                    entities = connection.Query<SkippedIntegratedCheckResult>(query, dp);
                }
            }
            catch (Exception exception)
            {

                exception.Data.Add("DetailMessage", "SkippedIntegratedCheckResultRepository:GetByBatchIdAndDate");
                throw exception;
            }
            return entities;
        }


    }
}