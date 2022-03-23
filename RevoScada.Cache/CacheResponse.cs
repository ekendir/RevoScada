using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Cache
{
    /// <summary>
    /// Cache return type for application
    /// </summary>
    public class CacheResponse
    {
       public CacheResponseStates CacheResponseState { get; set; }

       public object ResultValue { get; set; }

       public Type resultType { get; set; } 

       public string Message { get; set; }

       

    }

    public enum CacheResponseStates { 
    
        EmergencyError, Success, Fail, EmptyQueue
    
    }
}
