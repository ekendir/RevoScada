using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revo.Core.Data
{
    public static class NumericManipulation
    {

        /// <summary>
        /// Truncates decimal value remove remaining part defined in precision. Sample 1.23 with 1 precision is like 1.2. same as 1.29 like 1.2. No rounding applied.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        /// <summary>
        /// Truncates decimal value remove remaining part defined in precision. Sample 1.23 with 1 precision is like 1.2. same as 1.29 like 1.2. No rounding applied.
        /// Float values seem out of expected value regarded as 0.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static decimal TruncateDecimalWithExceptionalValues(float value, int precision)
        {
            value = ConvertExceptionalFloatValueAsZero(value);
            decimal valueAsDecimal = (decimal)value;
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * valueAsDecimal);
            return tmp / step;
        }
        private static float ConvertExceptionalFloatValueAsZero(float value)
        {
            return CheckForOutofRangeFloat(value) ? 0 : value;
        }


        private static bool CheckForOutofRangeFloat(float value)
        {
            return (Single.MinValue.Equals(value) || Single.MaxValue.Equals(value) || Single.NegativeInfinity.Equals(value) || Single.PositiveInfinity.Equals(value) || Single.NaN.Equals(value));
        }


        //todo:l refactor seperation of offsett operation in services and commandmanager
        public static (int IntegralPart, int DecimalPart) ParseDecimalNumber(decimal value, int precision)
        {
            int integralPart = (Convert.ToInt32(Math.Floor(value)));
            int offsetDecimal = Convert.ToInt32((value - Math.Floor(value)) * ((decimal)Math.Pow(10, precision)));
            return ( integralPart, offsetDecimal);
        }

        /// <summary>
        ///  round float to 2 decimal point. If out of range it uses original value
        /// </summary>
        public static float TryRoundFloat(float value,int precision)
        {
        return  CheckForOutofRangeFloat(value)?value: Convert.ToSingle( Math.Round((decimal)value, precision));
        }

        /// <summary>
        /// Gived a float value conver to like integer , for example you give a 10.45 , you have to take 10
        /// this format takes place for formatType , you can look formnatType in FormatOfIntegretyCheckItems'index of ApplicationProperty Tables in postgreSql
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static string ConvertFloatToInteger(float value, string formatType)
        {

            float floatVal = (float)value;
            return String.Format(formatType, floatVal);
        }


    }
}
