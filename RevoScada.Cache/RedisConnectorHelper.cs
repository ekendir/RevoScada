using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace RevoScada.Cache
{
    public sealed class RedisConnectorHelper
    {
        private  Dictionary<string, ConnectionMultiplexer> _connections { get; set; }
        private RedisConnectorHelper()
        {
            _connections = new Dictionary<string, ConnectionMultiplexer>();
        }
        
        private static readonly Lazy<RedisConnectorHelper> lazy = new Lazy<RedisConnectorHelper>(() => new RedisConnectorHelper());
        
        public static RedisConnectorHelper Instance
        {
            get
            {
                return lazy.Value;
            }
        }
 
        public  void SetServer(string server)
        {
            if (!_connections.ContainsKey(server))
            {
                ConnectionMultiplexer connection =   ConnectionMultiplexer.Connect(server);
                _connections.Add(server, connection);
            }
        }


        public  ConnectionMultiplexer Connection(string server)
        {
            return _connections[server];
        }
    }










    /// <summary>
    /// Makes connection single for application lifecycle
    /// </summary>
    /*
    public class RedisConnectorHelper
    {
        private static List<string> _servers;

        public static string Server{get; set;}
        
        public static List<string> Servers
        {
            get
            {
                return _servers;
            }
            set
            {
                if (!_servers.Contains(value.ToString()))
                {
                    _servers.Add(value.ToString());
                }
            }
        }

        public static ConfigurationOptions ConfigurationOptions { get; set; }

        private static Lazy<ConnectionMultiplexer> lazyConnection;
      
        private static Lazy<Dictionary<string, ConnectionMultiplexer>> lazyConnections;

        static RedisConnectorHelper()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ;
              //  return ConnectionMultiplexer.Connect(ConfigurationOptions);
            });

            lazyConnections = new Lazy<Dictionary<string, ConnectionMultiplexer>>(() =>

            {return 
                
                
                }            
            );

        }

       

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static Dictionary<string, ConnectionMultiplexer> Connections
        {
            get
            {
                return lazyConnection.Value;
            }
        }


    }*/
     
}


/*
 
   public class RedisConnectorHelper
    {
        public static string Server{get; set;}
       // public static List<string> Servers{get; set;}

        public static ConfigurationOptions ConfigurationOptions { get; set; }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        static RedisConnectorHelper()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(Server);
              //  return ConnectionMultiplexer.Connect(ConfigurationOptions);
            });
        }

       

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
 */