using Dapper.Contrib.Extensions;
using System;
 

namespace RevoScada.Entities.Configuration
{
    public class PlcDevice
    {
        [Key]
        public int Id { get; set; }
        public byte PlcType { get; set; }
        public int FurnaceId { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
