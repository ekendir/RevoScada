using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex
{
    public class WriteResult
    {
        public int S7Result { get; set; }

        public bool IsSucceeded
        {
            get{ return (S7Result == 0) ? true : false; }
        }
    }
}
