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
    public static class S7Consts
    {
        #region [Exported Consts]
        // Error codes
        //------------------------------------------------------------------------------
        //                                     ERRORS                 
        //------------------------------------------------------------------------------
        public const int errTCPSocketCreation = 0x00000001;
        public const int errTCPConnectionTimeout = 0x00000002;
        public const int errTCPConnectionFailed = 0x00000003;
        public const int errTCPReceiveTimeout = 0x00000004;
        public const int errTCPDataReceive = 0x00000005;
        public const int errTCPSendTimeout = 0x00000006;
        public const int errTCPDataSend = 0x00000007;
        public const int errTCPConnectionReset = 0x00000008;
        public const int errTCPNotConnected = 0x00000009;
        public const int errTCPUnreachableHost = 0x00002751;

        public const int errIsoConnect = 0x00010000; // Connection error
        public const int errIsoInvalidPDU = 0x00030000; // Bad format
        public const int errIsoInvalidDataSize = 0x00040000; // Bad Datasize passed to send/recv : buffer is invalid

        public const int errCliNegotiatingPDU = 0x00100000;
        public const int errCliInvalidParams = 0x00200000;
        public const int errCliJobPending = 0x00300000;
        public const int errCliTooManyItems = 0x00400000;
        public const int errCliInvalidWordLen = 0x00500000;
        public const int errCliPartialDataWritten = 0x00600000;
        public const int errCliSizeOverPDU = 0x00700000;
        public const int errCliInvalidPlcAnswer = 0x00800000;
        public const int errCliAddressOutOfRange = 0x00900000;
        public const int errCliInvalidTransportSize = 0x00A00000;
        public const int errCliWriteDataSizeMismatch = 0x00B00000;
        public const int errCliItemNotAvailable = 0x00C00000;
        public const int errCliInvalidValue = 0x00D00000;
        public const int errCliCannotStartPLC = 0x00E00000;
        public const int errCliAlreadyRun = 0x00F00000;
        public const int errCliCannotStopPLC = 0x01000000;
        public const int errCliCannotCopyRamToRom = 0x01100000;
        public const int errCliCannotCompress = 0x01200000;
        public const int errCliAlreadyStop = 0x01300000;
        public const int errCliFunNotAvailable = 0x01400000;
        public const int errCliUploadSequenceFailed = 0x01500000;
        public const int errCliInvalidDataSizeRecvd = 0x01600000;
        public const int errCliInvalidBlockType = 0x01700000;
        public const int errCliInvalidBlockNumber = 0x01800000;
        public const int errCliInvalidBlockSize = 0x01900000;
        public const int errCliNeedPassword = 0x01D00000;
        public const int errCliInvalidPassword = 0x01E00000;
        public const int errCliNoPasswordToSetOrClear = 0x01F00000;
        public const int errCliJobTimeout = 0x02000000;
        public const int errCliPartialDataRead = 0x02100000;
        public const int errCliBufferTooSmall = 0x02200000;
        public const int errCliFunctionRefused = 0x02300000;
        public const int errCliDestroying = 0x02400000;
        public const int errCliInvalidParamNumber = 0x02500000;
        public const int errCliCannotChangeParam = 0x02600000;
        public const int errCliFunctionNotImplemented = 0x02700000;
        //------------------------------------------------------------------------------
        //        PARAMS LIST FOR COMPATIBILITY WITH Snap7.net.cs           
        //------------------------------------------------------------------------------
        public const Int32 p_u16_LocalPort = 1;  // Not applicable here
        public const Int32 p_u16_RemotePort = 2;
        public const Int32 p_i32_PingTimeout = 3;
        public const Int32 p_i32_SendTimeout = 4;
        public const Int32 p_i32_RecvTimeout = 5;
        public const Int32 p_i32_WorkInterval = 6;  // Not applicable here
        public const Int32 p_u16_SrcRef = 7;  // Not applicable here
        public const Int32 p_u16_DstRef = 8;  // Not applicable here
        public const Int32 p_u16_SrcTSap = 9;  // Not applicable here
        public const Int32 p_i32_PDURequest = 10;
        public const Int32 p_i32_MaxClients = 11; // Not applicable here
        public const Int32 p_i32_BSendTimeout = 12; // Not applicable here
        public const Int32 p_i32_BRecvTimeout = 13; // Not applicable here
        public const Int32 p_u32_RecoveryTime = 14; // Not applicable here
        public const Int32 p_u32_KeepAliveTime = 15; // Not applicable here
                                                     // Area ID
        public const byte S7AreaPE = 0x81;
        public const byte S7AreaPA = 0x82;
        public const byte S7AreaMK = 0x83;
        public const byte S7AreaDB = 0x84;
        public const byte S7AreaCT = 0x1C;
        public const byte S7AreaTM = 0x1D;
        // Word Length
        public const int S7WLBit = 0x01;
        public const int S7WLByte = 0x02;
        public const int S7WLChar = 0x03;
        public const int S7WLWord = 0x04;
        public const int S7WLInt = 0x05;
        public const int S7WLDWord = 0x06;
        public const int S7WLDInt = 0x07;
        public const int S7WLReal = 0x08;
        public const int S7WLCounter = 0x1C;
        public const int S7WLTimer = 0x1D;
        // PLC Status
        public const int S7CpuStatusUnknown = 0x00;
        public const int S7CpuStatusRun = 0x08;
        public const int S7CpuStatusStop = 0x04;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct S7Tag
        {
            public Int32 Area;
            public Int32 DBNumber;
            public Int32 Start;
            public Int32 Elements;
            public Int32 WordLen;
        }
        #endregion
    }
}
