using Dapper.Contrib.Extensions;
using System;
//using System.ComponentModel.DataAnnotations.Schema;

namespace RevoScada.Entities.Configuration
{
    public class SiemensPlcConfig
    {
        [Key]
        public int PlcDeviceId { get; set; }
        public string Ip { get; set; }
        public byte Rack { get; set; }
        public byte Slot { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
