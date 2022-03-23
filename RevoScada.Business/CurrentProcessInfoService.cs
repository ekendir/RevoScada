using System.Linq;
using RevoScada.Entities;
using RevoScada.DataAccess.Abstract;
using System;

namespace RevoScada.Business
{
    public class CurrentProcessInfoService
    {
        private string _connectionString { get; set; }

        private readonly IGenericRepository<CurrentProcessInfo> _repository;

        public CurrentProcessInfoService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.CurrentProcessInfoRepository(_connectionString);
        }

        public CurrentProcessInfo Get()
        {
            return _repository.GetAll().First();
        }

        public bool Update(CurrentProcessInfo entity)
        {
            return _repository.Update(entity);
        }

        public bool InsertOrUpdate(CurrentProcessInfo currentProcessInfo)
        {
            return _repository.InsertOrUpdate(currentProcessInfo);
        }

        

    }
}