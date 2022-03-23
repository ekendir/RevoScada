using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"RecipeFields\"")]
    public class RecipeField
    {
        [ExplicitKey]
        public short id { set; get; }
        public string RecipeFieldName { set; get; }
        public short RecipeFieldOrder { set; get; }
        public short UnitId { get; set; }
        public bool IsActive { set; get; }
        public string DisplayColor { get; set; }
        public bool IsMultipleCell { get; set; }
    }
}
