using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"PredefinedRecipeFields\"")]
    public class PredefinedRecipeField
    {
        [ExplicitKey]
        public short id { get; set; }
        public short RecipeFieldId { get; set; }
        public string RecipeFieldValue { get; set; }
        
    }
}
