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
   public class ApplicationPropertyService: GenericService<ApplicationProperty>
    {
        public ApplicationPropertyService(string connectionString):base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.ApplicationPropertyRepository(_connectionString);
        }

        public IEnumerable<ApplicationProperty> GetAll()
        {
            return _repository.GetAll();
        }
 
        public ApplicationProperty GetByName(string applicationPropertyName)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM public.\"ApplicationProperties\" WHERE \"Name\"='{applicationPropertyName}'").FirstOrDefault();
        }

        public bool Update(ApplicationProperty entity)
        {
            return _repository.Update(entity);
        }

        public bool UpdateByName(string applicationPropertyName,string serializedObject)
        {
            ApplicationProperty applicationProperty = GetByName(applicationPropertyName);
            applicationProperty.Value = serializedObject;
            return _repository.Update(applicationProperty);
        }
    }
}
