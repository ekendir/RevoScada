using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class PredefinedRecipeFieldRepository : DapperGenericPostgreRepository<PredefinedRecipeField>
    {
        public PredefinedRecipeFieldRepository(string connectionString) : base(connectionString)
        {
        }
    }
}