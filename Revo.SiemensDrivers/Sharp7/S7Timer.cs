/*=============================================================================|
|  PROJECT Sharp7                                                        1.0.7 |
|==============================================================================|
|  Copyright (C) 2016 Davide Nardella                                          |
|  All rights reserved.                                                        |
|==============================================================================|
|  Sharp7 is free software: you can redistribute it and/or modify              |
|  it under the terms of the Lesser GNU General Public License as published by |
|  the Free Software Foundation, either version 3 of the License, or           |
|  (at your option) any later version.                                         |
|                                                                              |
|  It means that you can distribute your commercial software which includes    |
|  Sharp7 without the requirement to distribute the source code of your        |
|  application and without the requirement that your application be itself     |
|  distributed under LGPL.                                                     |
|                                                                              |
|  Sharp7 is distributed in the hope that it will be useful,                   |
|  but WITHOUT ANY WARRANTY; without even the implied warranty of              |
|  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               |
|  Lesser GNU General Public License for more details.                         |
|                                                                              |
|  You should have received a copy of the GNU General Public License and a     |
|  copy of Lesser GNU General Public License along with Sharp7.                |
|  If not, see  http://www.gnu.org/licenses/                                   |
|==============================================================================|
History:
 * 1.0.0 2016/10/09 First Release
 * 1.0.1 2016/10/22 Added CoreCLR compatibility (CORE_CLR symbol must be 
					defined in Build options).
					Thanks to Dirk-Jan Wassink.
 * 1.0.2 2016/11/13 Fixed a bug in CLR compatibility
 * 1.0.3 2017/01/25 Fixed a bug in S7.GetIntAt(). Thanks to lupal1
					Added S7Timer Read/Write. Thanks to Lukas Palkovic 
 * 1.0.4 2018/06/12 Fixed the last bug in S7.GetIntAt(). Thanks to Jérémy HAURAY
					Get/Set LTime. Thanks to Jérémy HAURAY
					Get/Set 1500 WString. Thanks to Jérémy HAURAY
					Get/Set 1500 Array of WChar. Thanks to Jérémy HAURAY
 * 1.0.5 2018/11/21 Implemented ListBlocks and ListBlocksOfType (by Jos Koenis, TEB Engineering)
 * 1.0.6 2019/05/25 Implemented Force Jobs by Bart Swister
 * 1.0.7 2019/10/05 Bugfix in List in ListBlocksOfType. Thanks to Cosimo Ladiana 
*/
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;
//------------------------------------------------------------------------------
// If you are compiling for UWP verify that WINDOWS_UWP or NETFX_CORE are 
// defined into Project Properties->Build->Conditional compilation symbols
//------------------------------------------------------------------------------
#if WINDOWS_UWP || NETFX_CORE
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#else // <-- Including MONO
using System.Net.Sockets;
#endif


namespace Revo.SiemensDrivers.Sharp7
{
    public class S7Timer
    {
        #region S7Timer
        TimeSpan pt;
        TimeSpan et;
        bool input = false;
        bool q = false;
        public S7Timer(byte[] buff, int position)
        {
            if (position + 12 < buff.Length)
            {
                return;
            }
            else
            {
                SetTimer(new List<byte>(buff).GetRange(position, 16).ToArray());
            }
        }

        public S7Timer(byte[] buff)
        {
            SetTimer(buff);
        }

        private void SetTimer(byte[] buff)
        {
            if (buff.Length != 12)
            {
                this.pt = new TimeSpan(0);
                this.et = new TimeSpan(0);
            }
            else
            {
                Int32 resPT;
                resPT = buff[0]; resPT <<= 8;
                resPT += buff[1]; resPT <<= 8;
                resPT += buff[2]; resPT <<= 8;
                resPT += buff[3];
                this.pt = new TimeSpan(0, 0, 0, 0, resPT);

                Int32 resET;
                resET = buff[4]; resET <<= 8;
                resET += buff[5]; resET <<= 8;
                resET += buff[6]; resET <<= 8;
                resET += buff[7];
                this.et = new TimeSpan(0, 0, 0, 0, resET);

                this.input = (buff[8] & 0x01) == 0x01;
                this.q = (buff[8] & 0x02) == 0x02;
            }
        }
        public TimeSpan PT
        {
            get
            {
                return pt;
            }
        }
        public TimeSpan ET
        {
            get
            {
                return et;
            }
        }
        public bool IN
        {
            get
            {
                return input;
            }
        }
        public bool Q
        {
            get
            {
                return q;
            }
        }
        #endregion
    }
}
