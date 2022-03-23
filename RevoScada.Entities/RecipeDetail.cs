using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"RecipeDetails\"")]
    public class RecipeDetail
    {
        [ExplicitKey]
        public long id { get; set; }
        public int RecipeId { get; set; }
        public short SegmentNo { get; set; }
        public short RecipeFieldId { get; set; }
        public string RecipeFieldValue { get; set; }
    }

}
