using Dapper.Contrib.Extensions;
namespace RevoScada.Entities
{
    [Table("public.\"Bags\"")]
    public class Bag
    {
        [ExplicitKey]
        public int id { get; set; }
        public int BatchId { get; set; }
        public int[] SelectedPorts { get; set; }
        public string BagName { get; set; }
    }
}