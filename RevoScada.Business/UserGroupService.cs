using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
    public class UserGroupService
    {
        private string _connectionString { get; set; }
        private readonly IGenericRepository<UserGroup> _repository;

        public UserGroupService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<UserGroup>(_connectionString);
        }

        public IEnumerable<UserGroup> GetAll()
        {
            return _repository.GetAll();
        }

        public UserGroup GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Update(UserGroup userGroup)
        {
            return _repository.Update(userGroup);
        }

        public bool Insert(UserGroup userGroup)
        {
            return _repository.Insert(userGroup);
        }

        public bool Delete(UserGroup userGroup)
        {
            return _repository.Delete(userGroup);
        }
    }
}
