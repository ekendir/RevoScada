
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
   public class PageTagConfigurationService  
    {
        private string _connectionString { get; set; }

        private readonly  IGenericRepository<PageTagConfiguration> _repository;

        public PageTagConfigurationService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.PageTagConfigurationRepository(_connectionString);
        }

        public PageTagConfiguration GetByName(string pageName)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM public.\"PageTagConfigurations\" WHERE \"PageName\"='{pageName}'").FirstOrDefault();
        }

        public bool Update(PageTagConfiguration pageTagConfiguration)
        {
            return _repository.Update(pageTagConfiguration);
        }
        public bool Insert(PageTagConfiguration pageTagConfiguration)
        {
            return _repository.Insert(pageTagConfiguration, true);
        }

    }
}
