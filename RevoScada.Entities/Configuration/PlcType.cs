using Dapper.Contrib.Extensions;
 

namespace RevoScada.Entities.Configuration
{
    public class PlcType
    {
        [Key]
        public byte Id { get; set; }
        public string TypeName { get; set; }
    }
}
