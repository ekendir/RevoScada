using Dapper.Contrib.Extensions;
 
namespace RevoScada.Entities.Configuration
{
    public class FurnaceType
    {
        [Key]
        public short Id { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
    }
}
