using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
   
    public class RecipeDetailHistoryRepository : DapperGenericPostgreRepository<RecipeDetailHistory>
    {
        public RecipeDetailHistoryRepository(string connectionString) : base(connectionString)
        {
            TableName = "RecipeDetailHistories";
        }
    }
}
