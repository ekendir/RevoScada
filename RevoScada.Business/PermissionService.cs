using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
    public class PermissionService
    {
        private string _connectionString { get; set; }

        private readonly IGenericRepository<Permission> _repository;

        public PermissionService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.PermissionRepository(_connectionString);
        }

        public IEnumerable<Permission> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
