using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"LotProperties\"")]
    public class LotProperty
    {
        [ExplicitKey]
        public int id { get; set; }
        public int BagId { get; set; }
        public string SoirNumber { get; set; }
        public string PartName { get; set; }
        public string ToolName { get; set; }
        public int CreatedByUserId { get; set; }
        public int ModifiedByUserId { get; set; }
    }
}