using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Configuration
{
    public class OnDemandDataBlock
    {
        [Key]
        public int DBNumber { get; set;}
        public string DBDescription { get; set;}
        public int Id { get; set;}
        public int PlcId { get; set;}
    }
}
