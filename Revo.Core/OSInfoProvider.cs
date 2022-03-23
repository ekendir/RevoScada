using System;
namespace Revo.Core
{
   public class OSInfoProvider
    {
        public int UpTimeInSeconds
        {
            get
            {
                return   Environment.TickCount / 1000;
            }
        }

        public string UpTimeLiteral
        {
            get
            {
                TimeSpan t = TimeSpan.FromSeconds(Environment.TickCount / 1000);
                return   string.Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s:{4:D3}ms",t.Days, t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            }
        }
    }
}