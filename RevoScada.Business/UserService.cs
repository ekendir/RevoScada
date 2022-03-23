using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
    public class UserService
    {
        private string _connectionString { get; set; }
        private readonly IGenericRepository<User> _repository;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.Postgresql.UserRepository(_connectionString);
        }

        public IEnumerable<User> GetAll()
        {
            return _repository.GetAll();
        }

        public User GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<User> GetByGroupId(short groupId)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM \"Users\" WHERE \"GroupId\" = {groupId};");
        }

        public bool Update(User user)
        {
            return _repository.Update(user);
        }

        public bool Insert(User user)
        {
            return _repository.Insert(user);
        }

        public bool Delete(User user)
        {
            return _repository.Delete(user);
        }
    }
}
