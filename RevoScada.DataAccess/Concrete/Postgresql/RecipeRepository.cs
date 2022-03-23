using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
   
    public class RecipeRepository : DapperGenericPostgreRepository<Recipe>
    {
        public RecipeRepository(string connectionString) : base(connectionString)
        {
            
        }
    }
}
