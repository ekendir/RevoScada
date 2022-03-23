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
    public static class S7
    {
        #region [Help Functions]

        private static Int64 bias = 621355968000000000; // "decimicros" between 0001-01-01 00:00:00 and 1970-01-01 00:00:00

        private static int BCDtoByte(byte B)
        {
            return ((B >> 4) * 10) + (B & 0x0F);
        }

        private static byte ByteToBCD(int Value)
        {
            return (byte)(((Value / 10) << 4) | (Value % 10));
        }

        private static byte[] CopyFrom(byte[] Buffer, int Pos, int Size)
        {
            byte[] Result = new byte[Size];
            Array.Copy(Buffer, Pos, Result, 0, Size);
            return Result;
        }

        public static int DataSizeByte(int WordLength)
        {
            switch (WordLength)
            {
                case S7Consts.S7WLBit: return 1;  // S7 sends 1 byte per bit
                case S7Consts.S7WLByte: return 1;
                case S7Consts.S7WLChar: return 1;
                case S7Consts.S7WLWord: return 2;
                case S7Consts.S7WLDWord: return 4;
                case S7Consts.S7WLInt: return 2;
                case S7Consts.S7WLDInt: return 4;
                case S7Consts.S7WLReal: return 4;
                case S7Consts.S7WLCounter: return 2;
                case S7Consts.S7WLTimer: return 2;
                default: return 0;
            }
        }

        #region Get/Set the bit at Pos.Bit
        public static bool GetBitAt(byte[] Buffer, int Pos, int Bit)
        {
            byte[] Mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            if (Bit < 0) Bit = 0;
            if (Bit > 7) Bit = 7;
            return (Buffer[Pos] & Mask[Bit]) != 0;
        }
        public static void SetBitAt(ref byte[] Buffer, int Pos, int Bit, bool Value)
        {
            byte[] Mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            if (Bit < 0) Bit = 0;
            if (Bit > 7) Bit = 7;

            if (Value)
                Buffer[Pos] = (byte)(Buffer[Pos] | Mask[Bit]);
            else
                Buffer[Pos] = (byte)(Buffer[Pos] & ~Mask[Bit]);
        }
        #endregion

        #region Get/Set 8 bit signed value (S7 SInt) -128..127
        public static int GetSIntAt(byte[] Buffer, int Pos)
        {
            int Value = Buffer[Pos];
            if (Value < 128)
                return Value;
            else
                return (int)(Value - 256);
        }
        public static void SetSIntAt(byte[] Buffer, int Pos, int Value)
        {
            if (Value < -128) Value = -128;
            if (Value > 127) Value = 127;
            Buffer[Pos] = (byte)Value;
        }
        #endregion

        #region Get/Set 16 bit signed value (S7 int) -32768..32767
        public static short GetIntAt(byte[] Buffer, int Pos)
        {
            return (short)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
        }
        public static void SetIntAt(byte[] Buffer, int Pos, Int16 Value)
        {
            Buffer[Pos] = (byte)(Value >> 8);
            Buffer[Pos + 1] = (byte)(Value & 0x00FF);
        }
        #endregion

        #region Get/Set 32 bit signed value (S7 DInt) -2147483648..2147483647
        public static int GetDIntAt(byte[] Buffer, int Pos)
        {
            int Result;
            Result = Buffer[Pos]; Result <<= 8;
            Result += Buffer[Pos + 1]; Result <<= 8;
            Result += Buffer[Pos + 2]; Result <<= 8;
            Result += Buffer[Pos + 3];
            return Result;
        }
        public static void SetDIntAt(byte[] Buffer, int Pos, int Value)
        {
            Buffer[Pos + 3] = (byte)(Value & 0xFF);
            Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
            Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
            Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
        }
        #endregion

        #region Get/Set 64 bit signed value (S7 LInt) -9223372036854775808..9223372036854775807
        public static Int64 GetLIntAt(byte[] Buffer, int Pos)
        {
            Int64 Result;
            Result = Buffer[Pos]; Result <<= 8;
            Result += Buffer[Pos + 1]; Result <<= 8;
            Result += Buffer[Pos + 2]; Result <<= 8;
            Result += Buffer[Pos + 3]; Result <<= 8;
            Result += Buffer[Pos + 4]; Result <<= 8;
            Result += Buffer[Pos + 5]; Result <<= 8;
            Result += Buffer[Pos + 6]; Result <<= 8;
            Result += Buffer[Pos + 7];
            return Result;
        }
        public static void SetLIntAt(byte[] Buffer, int Pos, Int64 Value)
        {
            Buffer[Pos + 7] = (byte)(Value & 0xFF);
            Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
            Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
            Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
            Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
            Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
            Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
            Buffer[Pos] = (byte)((Value >> 56) & 0xFF);
        }
        #endregion

        #region Get/Set 8 bit unsigned value (S7 USInt) 0..255
        public static byte GetUSIntAt(byte[] Buffer, int Pos)
        {
            return Buffer[Pos];
        }
        public static void SetUSIntAt(byte[] Buffer, int Pos, byte Value)
        {
            Buffer[Pos] = Value;
        }
        #endregion

        #region Get/Set 16 bit unsigned value (S7 UInt) 0..65535
        public static UInt16 GetUIntAt(byte[] Buffer, int Pos)
        {
            return (UInt16)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
        }
        public static void SetUIntAt(byte[] Buffer, int Pos, UInt16 Value)
        {
            Buffer[Pos] = (byte)(Value >> 8);
            Buffer[Pos + 1] = (byte)(Value & 0x00FF);
        }
        #endregion

        #region Get/Set 32 bit unsigned value (S7 UDInt) 0..4294967296
        public static UInt32 GetUDIntAt(byte[] Buffer, int Pos)
        {
            UInt32 Result;
            Result = Buffer[Pos]; Result <<= 8;
            Result |= Buffer[Pos + 1]; Result <<= 8;
            Result |= Buffer[Pos + 2]; Result <<= 8;
            Result |= Buffer[Pos + 3];
            return Result;
        }
        public static void SetUDIntAt(byte[] Buffer, int Pos, UInt32 Value)
        {
            Buffer[Pos + 3] = (byte)(Value & 0xFF);
            Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
            Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
            Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
        }
        #endregion

        #region Get/Set 64 bit unsigned value (S7 ULint) 0..18446744073709551616
        public static UInt64 GetULIntAt(byte[] Buffer, int Pos)
        {
            UInt64 Result;
            Result = Buffer[Pos]; Result <<= 8;
            Result |= Buffer[Pos + 1]; Result <<= 8;
            Result |= Buffer[Pos + 2]; Result <<= 8;
            Result |= Buffer[Pos + 3]; Result <<= 8;
            Result |= Buffer[Pos + 4]; Result <<= 8;
            Result |= Buffer[Pos + 5]; Result <<= 8;
            Result |= Buffer[Pos + 6]; Result <<= 8;
            Result |= Buffer[Pos + 7];
            return Result;
        }
        public static void SetULintAt(byte[] Buffer, int Pos, UInt64 Value)
        {
            Buffer[Pos + 7] = (byte)(Value & 0xFF);
            Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
            Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
            Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
            Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
            Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
            Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
            Buffer[Pos] = (byte)((Value >> 56) & 0xFF);
        }
        #endregion

        #region Get/Set 8 bit word (S7 Byte) 16#00..16#FF
        public static byte GetByteAt(byte[] Buffer, int Pos)
        {
            return Buffer[Pos];
        }
        public static void SetByteAt(byte[] Buffer, int Pos, byte Value)
        {
            Buffer[Pos] = Value;
        }
        #endregion

        #region Get/Set 16 bit word (S7 Word) 16#0000..16#FFFF
        public static UInt16 GetWordAt(byte[] Buffer, int Pos)
        {
            return GetUIntAt(Buffer, Pos);
        }
        public static void SetWordAt(byte[] Buffer, int Pos, UInt16 Value)
        {
            SetUIntAt(Buffer, Pos, Value);
        }
        #endregion

        #region Get/Set 32 bit word (S7 DWord) 16#00000000..16#FFFFFFFF
        public static UInt32 GetDWordAt(byte[] Buffer, int Pos)
        {
            return GetUDIntAt(Buffer, Pos);
        }
        public static void SetDWordAt(byte[] Buffer, int Pos, UInt32 Value)
        {
            SetUDIntAt(Buffer, Pos, Value);
        }
        #endregion

        #region Get/Set 64 bit word (S7 LWord) 16#0000000000000000..16#FFFFFFFFFFFFFFFF
        public static UInt64 GetLWordAt(byte[] Buffer, int Pos)
        {
            return GetULIntAt(Buffer, Pos);
        }
        public static void SetLWordAt(byte[] Buffer, int Pos, UInt64 Value)
        {
            SetULintAt(Buffer, Pos, Value);
        }
        #endregion

        #region Get/Set 32 bit floating point number (S7 Real) (Range of Single)
        public static Single GetRealAt(byte[] Buffer, int Pos)
        {
            UInt32 Value = GetUDIntAt(Buffer, Pos);
            byte[] bytes = BitConverter.GetBytes(Value);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static void SetRealAt(byte[] Buffer, int Pos, Single Value)
        {
            byte[] FloatArray = BitConverter.GetBytes(Value);
            Buffer[Pos] = FloatArray[3];
            Buffer[Pos + 1] = FloatArray[2];
            Buffer[Pos + 2] = FloatArray[1];
            Buffer[Pos + 3] = FloatArray[0];
        }
        #endregion

        #region Get/Set 64 bit floating point number (S7 LReal) (Range of Double)
        public static Double GetLRealAt(byte[] Buffer, int Pos)
        {
            UInt64 Value = GetULIntAt(Buffer, Pos);
            byte[] bytes = BitConverter.GetBytes(Value);
            return BitConverter.ToDouble(bytes, 0);
        }
        public static void SetLRealAt(byte[] Buffer, int Pos, Double Value)
        {
            byte[] FloatArray = BitConverter.GetBytes(Value);
            Buffer[Pos] = FloatArray[7];
            Buffer[Pos + 1] = FloatArray[6];
            Buffer[Pos + 2] = FloatArray[5];
            Buffer[Pos + 3] = FloatArray[4];
            Buffer[Pos + 4] = FloatArray[3];
            Buffer[Pos + 5] = FloatArray[2];
            Buffer[Pos + 6] = FloatArray[1];
            Buffer[Pos + 7] = FloatArray[0];
        }
        #endregion

        #region Get/Set DateTime (S7 DATE_AND_TIME)
        public static DateTime GetDateTimeAt(byte[] Buffer, int Pos)
        {
            int Year, Month, Day, Hour, Min, Sec, MSec;

            Year = BCDtoByte(Buffer[Pos]);
            if (Year < 90)
                Year += 2000;
            else
                Year += 1900;

            Month = BCDtoByte(Buffer[Pos + 1]);
            Day = BCDtoByte(Buffer[Pos + 2]);
            Hour = BCDtoByte(Buffer[Pos + 3]);
            Min = BCDtoByte(Buffer[Pos + 4]);
            Sec = BCDtoByte(Buffer[Pos + 5]);
            MSec = (BCDtoByte(Buffer[Pos + 6]) * 10) + (BCDtoByte(Buffer[Pos + 7]) / 10);
            try
            {
                return new DateTime(Year, Month, Day, Hour, Min, Sec, MSec);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetDateTimeAt(byte[] Buffer, int Pos, DateTime Value)
        {
            int Year = Value.Year;
            int Month = Value.Month;
            int Day = Value.Day;
            int Hour = Value.Hour;
            int Min = Value.Minute;
            int Sec = Value.Second;
            int Dow = (int)Value.DayOfWeek + 1;
            // MSecH = First two digits of miliseconds 
            int MsecH = Value.Millisecond / 10;
            // MSecL = Last digit of miliseconds
            int MsecL = Value.Millisecond % 10;
            if (Year > 1999)
                Year -= 2000;

            Buffer[Pos] = ByteToBCD(Year);
            Buffer[Pos + 1] = ByteToBCD(Month);
            Buffer[Pos + 2] = ByteToBCD(Day);
            Buffer[Pos + 3] = ByteToBCD(Hour);
            Buffer[Pos + 4] = ByteToBCD(Min);
            Buffer[Pos + 5] = ByteToBCD(Sec);
            Buffer[Pos + 6] = ByteToBCD(MsecH);
            Buffer[Pos + 7] = ByteToBCD(MsecL * 10 + Dow);
        }
        #endregion

        #region Get/Set DATE (S7 DATE) 
        public static DateTime GetDateAt(byte[] Buffer, int Pos)
        {
            try
            {
                return new DateTime(1990, 1, 1).AddDays(GetIntAt(Buffer, Pos));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetDateAt(byte[] Buffer, int Pos, DateTime Value)
        {
            SetIntAt(Buffer, Pos, (Int16)(Value - new DateTime(1990, 1, 1)).Days);
        }

        #endregion

        #region Get/Set TOD (S7 TIME_OF_DAY)
        public static DateTime GetTODAt(byte[] Buffer, int Pos)
        {
            try
            {
                return new DateTime(0).AddMilliseconds(S7.GetDIntAt(Buffer, Pos));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetTODAt(byte[] Buffer, int Pos, DateTime Value)
        {
            TimeSpan Time = Value.TimeOfDay;
            SetDIntAt(Buffer, Pos, (Int32)Math.Round(Time.TotalMilliseconds));
        }
        #endregion

        #region Get/Set LTOD (S7 1500 LONG TIME_OF_DAY)
        public static DateTime GetLTODAt(byte[] Buffer, int Pos)
        {
            // .NET Tick = 100 ns, S71500 Tick = 1 ns
            try
            {
                return new DateTime(Math.Abs(GetLIntAt(Buffer, Pos) / 100));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetLTODAt(byte[] Buffer, int Pos, DateTime Value)
        {
            TimeSpan Time = Value.TimeOfDay;
            SetLIntAt(Buffer, Pos, (Int64)Time.Ticks * 100);
        }
        #endregion

        #region GET/SET LDT (S7 1500 Long Date and Time)
        public static DateTime GetLDTAt(byte[] Buffer, int Pos)
        {
            try
            {
                return new DateTime((GetLIntAt(Buffer, Pos) / 100) + bias);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetLDTAt(byte[] Buffer, int Pos, DateTime Value)
        {
            SetLIntAt(Buffer, Pos, (Value.Ticks - bias) * 100);
        }
        #endregion

        #region Get/Set DTL (S71200/1500 Date and Time)
        // Thanks to Johan Cardoen for GetDTLAt
        public static DateTime GetDTLAt(byte[] Buffer, int Pos)
        {
            int Year, Month, Day, Hour, Min, Sec, MSec;

            Year = Buffer[Pos] * 256 + Buffer[Pos + 1];
            Month = Buffer[Pos + 2];
            Day = Buffer[Pos + 3];
            Hour = Buffer[Pos + 5];
            Min = Buffer[Pos + 6];
            Sec = Buffer[Pos + 7];
            MSec = (int)GetUDIntAt(Buffer, Pos + 8) / 1000000;

            try
            {
                return new DateTime(Year, Month, Day, Hour, Min, Sec, MSec);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }
        public static void SetDTLAt(byte[] Buffer, int Pos, DateTime Value)
        {
            short Year = (short)Value.Year;
            byte Month = (byte)Value.Month;
            byte Day = (byte)Value.Day;
            byte Hour = (byte)Value.Hour;
            byte Min = (byte)Value.Minute;
            byte Sec = (byte)Value.Second;
            byte Dow = (byte)(Value.DayOfWeek + 1);

            Int32 NanoSecs = Value.Millisecond * 1000000;

            var bytes_short = BitConverter.GetBytes(Year);

            Buffer[Pos] = bytes_short[1];
            Buffer[Pos + 1] = bytes_short[0];
            Buffer[Pos + 2] = Month;
            Buffer[Pos + 3] = Day;
            Buffer[Pos + 4] = Dow;
            Buffer[Pos + 5] = Hour;
            Buffer[Pos + 6] = Min;
            Buffer[Pos + 7] = Sec;
            SetDIntAt(Buffer, Pos + 8, NanoSecs);
        }

        #endregion

        #region Get/Set String (S7 String)
        // Thanks to Pablo Agirre 
        public static string GetStringAt(byte[] Buffer, int Pos)
        {
            int size = (int)Buffer[Pos + 1];
            return Encoding.UTF8.GetString(Buffer, Pos + 2, size);
        }

        public static void SetStringAt(byte[] Buffer, int Pos, int MaxLen, string Value)
        {
            int size = Value.Length;
            Buffer[Pos] = (byte)MaxLen;
            Buffer[Pos + 1] = (byte)size;
            Encoding.UTF8.GetBytes(Value, 0, size, Buffer, Pos + 2);
        }

        #endregion

        #region Get/Set WString (S7-1500 String)
        public static string GetWStringAt(byte[] Buffer, int Pos)
        {
            //WString size = n characters + 2 Words (first for max length, second for real length)
            //Get the real length in Words
            int size = GetIntAt(Buffer, Pos + 2);
            //Extract string in UTF-16 unicode Big Endian.
            return Encoding.BigEndianUnicode.GetString(Buffer, Pos + 4, size * 2);
        }

        public static void SetWStringAt(byte[] Buffer, int Pos, int MaxCharNb, string Value)
        {
            //Get the length in words from number of characters
            int size = Value.Length;
            //Set the Max length in Words 
            SetIntAt(Buffer, Pos, (short)MaxCharNb);
            //Set the real length in words
            SetIntAt(Buffer, Pos + 2, (short)size);
            //Set the UTF-16 unicode Big endian String (after max length and length)
            Encoding.BigEndianUnicode.GetBytes(Value, 0, size, Buffer, Pos + 4);
        }

        #endregion

        #region Get/Set Array of char (S7 ARRAY OF CHARS)
        public static string GetCharsAt(byte[] Buffer, int Pos, int Size)
        {
            return Encoding.UTF8.GetString(Buffer, Pos, Size);
        }

        public static void SetCharsAt(byte[] Buffer, int Pos, string Value)
        {
            int MaxLen = Buffer.Length - Pos;
            // Truncs the string if there's no room enough        
            if (MaxLen > Value.Length) MaxLen = Value.Length;
            Encoding.UTF8.GetBytes(Value, 0, MaxLen, Buffer, Pos);
        }

        #endregion

        #region Get/Set Array of WChar (S7-1500 ARRAY OF CHARS)

        public static String GetWCharsAt(byte[] Buffer, int Pos, int SizeInCharNb)
        {
            //Extract Unicode UTF-16 Big-Endian character from the buffer. To use with WChar Datatype.
            //Size to read is in byte. Be careful, 1 char = 2 bytes
            return Encoding.BigEndianUnicode.GetString(Buffer, Pos, SizeInCharNb * 2);
        }

        public static void SetWCharsAt(byte[] Buffer, int Pos, string Value)
        {
            //Compute Max length in char number
            int MaxLen = (Buffer.Length - Pos) / 2;
            // Truncs the string if there's no room enough        
            if (MaxLen > Value.Length) MaxLen = Value.Length;
            Encoding.BigEndianUnicode.GetBytes(Value, 0, MaxLen, Buffer, Pos);
        }

        #endregion

        #region Get/Set Counter
        public static int GetCounter(ushort Value)
        {
            return BCDtoByte((byte)Value) * 100 + BCDtoByte((byte)(Value >> 8));
        }

        public static int GetCounterAt(ushort[] Buffer, int Index)
        {
            return GetCounter(Buffer[Index]);
        }

        public static ushort ToCounter(int Value)
        {
            return (ushort)(ByteToBCD(Value / 100) + (ByteToBCD(Value % 100) << 8));
        }

        public static void SetCounterAt(ushort[] Buffer, int Pos, int Value)
        {
            Buffer[Pos] = ToCounter(Value);
        }
        #endregion

        #region Get/Set Timer

        public static S7Timer GetS7TimerAt(byte[] Buffer, int Pos)
        {
            return new S7Timer(new List<byte>(Buffer).GetRange(Pos, 12).ToArray());
        }

        public static void SetS7TimespanAt(byte[] Buffer, int Pos, TimeSpan Value)
        {
            SetDIntAt(Buffer, Pos, (Int32)Value.TotalMilliseconds);
        }

        public static TimeSpan GetS7TimespanAt(byte[] Buffer, int pos)
        {
            if (Buffer.Length < pos + 4)
            {
                return new TimeSpan();
            }

            Int32 a;
            a = Buffer[pos + 0]; a <<= 8;
            a += Buffer[pos + 1]; a <<= 8;
            a += Buffer[pos + 2]; a <<= 8;
            a += Buffer[pos + 3];
            TimeSpan sp = new TimeSpan(0, 0, 0, 0, a);

            return sp;
        }

        public static TimeSpan GetLTimeAt(byte[] Buffer, int pos)
        {
            //LTime size : 64 bits (8 octets)
            //Case if the buffer is too small
            if (Buffer.Length < pos + 8) return new TimeSpan();

            try
            {
                // Extract and Convert number of nanoseconds to tick (1 tick = 100 nanoseconds)
                return TimeSpan.FromTicks(GetLIntAt(Buffer, pos) / 100);
            }
            catch (Exception)
            {
                return new TimeSpan();
            }
        }

        public static void SetLTimeAt(byte[] Buffer, int Pos, TimeSpan Value)
        {
            SetLIntAt(Buffer, Pos, (long)(Value.Ticks * 100));
        }

        #endregion

        #endregion [Help Functions]
    }
}
