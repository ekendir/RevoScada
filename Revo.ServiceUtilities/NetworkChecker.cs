using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace Revo.ServiceUtilities
{
    public class NetworkChecker
    {

        [DllImport("wininet.dll")]
        private extern static  bool InternetGetConnectedState(out int Description, int ReservedValue);


        /// <summary>
        /// Returns true if network is avaible
        /// </summary>
        /// 

        
        public static  bool IsConnectedToInternet()
        {
            int Desc;
            bool b = InternetGetConnectedState(out Desc, 0);
            return b;
        }

        public static int PingFailure { get; set; }

         

        /// <summary>
        /// Checks whether you can ping that ip
        /// </summary>
        /// <param name="ipAddress">formatted ip address like 11.11.11.11</param>
        /// <returns>Ping result is succeeded as bool</returns>
        public static bool PingSucceeded(string ipAddress,int tryAmount=1)
        {
            bool pingResult = false;

            do
            {
                if (tryAmount==0)
                {
                    break;
                }
                tryAmount--;

                try
                {

                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply pingReply;

                    pingReply = ping.Send(ipAddress);

                    if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        pingResult = true;
                    }
                    else
                    {
                        pingResult = false;
                        Thread.Sleep(500);
                    }
                }
                catch (Exception)
                {
                    pingResult = false;
                    Thread.Sleep(500);
                }

            } while (pingResult==false);

            return pingResult;
        }
    }
}
