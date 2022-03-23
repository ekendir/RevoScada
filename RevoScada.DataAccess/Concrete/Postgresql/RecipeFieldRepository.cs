using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
   
    public class RecipeFieldRepository : DapperGenericPostgreRepository<RecipeField>
    {
        public RecipeFieldRepository(string connectionString) : base(connectionString)
        {
        }
    }
}

