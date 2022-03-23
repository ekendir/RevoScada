using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
   
    public class RecipeDetailRepository : DapperGenericPostgreRepository<RecipeDetail>
    {
        public RecipeDetailRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
