using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revo.Core.Data
{
    /// <summary>
    /// General string manipulations
    /// </summary>
   public  class StringManipulation
    {
        /// <summary>
        /// ığüşöçĞÜŞİÖÇ replaced with igusocGUSIOC
        /// </summary>
        /// <param name="rawString">raw string contains turkish chars</param>
        /// <returns>edited string</returns>
        public string ReplaceTurkishAndSpecialChars(string rawString)
        {
            if (rawString == null)
                return rawString;

            string Source = "ığüşöçĞÜŞİÖÇ";
            string Destination = "igusocGUSIOC";
            for (int i = 0; i <= Source.Length - 1; i++)
                rawString = rawString.Replace(Source[i], Destination[i]);
                StringBuilder sb = new StringBuilder();
                foreach (char c in rawString)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c==' ' || c=='-')
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
        }

        public string GetSubString(string editingString,int startIndex,int endIndex)
        {
            return (string.IsNullOrEmpty( editingString ))
                   ? string.Empty:
                   (editingString.Length > endIndex) 
                   ? editingString.Substring(startIndex, endIndex) 
                   : editingString;
        }

        public string ByteArrayToStringHex(byte[] buffer)
        {
            StringBuilder hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public string FormatDateTimeToString(DateTime dateTime)
        { 
            return dateTime.ToString("yyyyMMddHHmmss");
        }
        
        public DateTime StringToDateTime(string stringFormattedDateTime)
        {
            DateTime result =  DateTime.ParseExact(stringFormattedDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return result;
        }
        public DateTime GetCurrenDateTimeWithoutMilisecond(DateTime DateTime)
        {
            DateTime result = DateTime.ParseExact(FormatDateTimeToString(DateTime), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return result;
        }

        /// <summary>
        /// Seperates an adjoining string with uppercase letters and returns the fixed value. E.g. Input: BagName --- Output: Bag Name
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public string SeperatePascalCaseString(string stringValue)
        {
            int index = 0;
            int counter = 0;
            int endIndex = 0;
            string fixedCaption;
            foreach (char l in stringValue)
            {
                if (Char.IsUpper(l))
                    counter++;

                if (counter > 1)
                {
                    endIndex = index;
                    break;
                }
                index++;
            }

            // If there is no uppercase letter in this string value, then return passed value again. You're wasting method's time.
            if (counter == 0)
                return stringValue;

            fixedCaption = stringValue.Substring(0, endIndex);
            fixedCaption += " " + stringValue.Substring(endIndex);

            return fixedCaption;
        }
    }
}
