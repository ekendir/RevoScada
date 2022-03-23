using Dapper.Contrib.Extensions;
using System;
 

namespace RevoScada.Entities.Configuration
{
    public class SiemensTagConfiguration: ITagConfiguration
    {
        [ExplicitKey]
        public int Id { get; set; }
        public int PlcId { get; set; }
        public string TagName { get; set; }
        public string Address { get; set; }
        public short DBNumber { get; set; }
        public decimal Offset { get; set; }
        public string DataType { get; set; }
        public short SiemensTagGroupId { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}