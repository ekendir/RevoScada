using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
   
    public class RecipeGroupRepository : DapperGenericPostgreRepository<RecipeGroup>
    {
        public RecipeGroupRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
