using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"RecipeDetailHistories\"")]
    public class RecipeDetailHistory
    {
        [ExplicitKey]
        public long id { get; set; }
        public int RecipeId { get; set; }
        public short SegmentNo { get; set; }
        public short RecipeFieldId { get; set; }
        public string RecipeCellValue { get; set; }
        public int BatchId { get; set; }
    }
}