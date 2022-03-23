using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Newtonsoft.Json;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.ProcessController;

namespace RevoScada.DesktopApplication.Converters
{
    public class BoolToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;

            if (booleanValue == true)
            {
                return "Bold";
            }
            else
            {
                return "Medium";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            if (doubleValue > 1)
            {
                return 10;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LookDirToWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringVal = (string)value;
            double lookDirVal = 0;

            if (!String.IsNullOrEmpty(stringVal))
                lookDirVal = System.Convert.ToDouble(stringVal);

            if (lookDirVal >= 60000)
                return 250;
            else if (lookDirVal >= 45000 && lookDirVal <= 60000)
                return 300;
            else if (lookDirVal >= 35000 && lookDirVal <= 45000)
                return 350;
            else if (lookDirVal <= 35000)
                return 400;

            return 350;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OpacityToZIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            if (doubleValue > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReverseOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            if (doubleValue > 0)
                return 0;
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReverseBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;

            if (boolValue == true)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleVal = (double)value;
            if (doubleVal == 1)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScaleToBool : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                double scaleXValue = (double)item;
                if (scaleXValue > 1)
                {
                    return true;
                }
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiVisibilityToOppositeVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                Visibility visibilityValue = (Visibility)item;
                if (visibilityValue == Visibility.Visible)
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class MultiBoolVisibilityToOppositeVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)values[0];
            Visibility visVal = (Visibility)values[1];

            if (boolVal || visVal == Visibility.Visible)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiDoubleVisibilityToOppositeVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double floatVal = (double)values[0];
            Visibility visVal = (Visibility)values[1];

            if (floatVal == 0 && visVal == Visibility.Visible)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)value;

            if (boolVal == true)
                return 1;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringVal = (string)value;

            if (stringVal?.Length > 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToVisiblity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = (double)value;

            if (doubleValue > 0)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToAbsoluteVisiblity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = (double)value;

            if (doubleValue == 1)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToVisiblity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int intVal = (int)value;

                if (intVal > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityToHalf : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibilityVal = (Visibility)value;

            if (visibilityVal == Visibility.Collapsed)
                return 1;
            else
                return 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)value;

            if (boolVal)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AllBoolToEnabled : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object obj in values)
            {
                bool val = (bool)obj;
                if (!val)
                    return false;
            }
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)value;

            try
            {
                if (boolVal)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FloatToCollapsedVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double floatVal = (double)value;

            if (floatVal > 0.1)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ZeroToCollapsedVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double floatVal = (double)value;

            if (floatVal == 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityToOppositeVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visVal = (Visibility)value;

            if (visVal == Visibility.Visible)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToOppositeVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)value;

            if (boolVal)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)value;

            if (boolVal)
                return 0.4;
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intVal = (int)value;

            if (intVal == -1)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsEnabledToOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnabledVal = (bool)value;

            if (isEnabledVal == false)
                return 0.4;
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OpacityToEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double opacityVal = (double)value;

            if (opacityVal < 1)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PassThroughConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToArray();
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((value is bool) && (bool)value == false)
                {
                    return false;
                }
            }
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ItemIndexToIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intVal = (int)value;

            if (intVal > -1)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemValueToIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class IsNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;

            if (string.IsNullOrEmpty(val))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsEqualOrGreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;
            double param = (double)parameter;

            if (val >= param)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TagToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;

            string val = (string)value;

            if (val.Equals("True"))
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToPermissionTagValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVal = (bool)value;

            if (boolVal)
                return "PermissionGranted";
            else
                return "PermissionRejected";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AnyVisibilityAvailable : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((value is Visibility) && (Visibility)value == Visibility.Visible)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class StringToAbbreviatedString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            int param = (int)parameter;

            if (string.IsNullOrEmpty(val))
                return null;

            if (val.Length > param)
                return val.Substring(0, param) + "...";
            else
                return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AbbreviatedStringToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            int param = (int)parameter;

            if (string.IsNullOrEmpty(val))
                return false;

            if (val.Length > param)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AlarmConditionBasedFGColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string typeVal = string.Empty;

            try
            {
                typeVal = (string)value;
            }
            catch (Exception)
            {
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#606060");
            }

            if (string.IsNullOrEmpty(typeVal))
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#606060");

            if (typeVal.Equals("I") || typeVal.Equals("IO"))
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#FaFaFa"); // White color

            return (SolidColorBrush)new BrushConverter().ConvertFromString("#606060");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EventConditionBasedBGColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string colorVal = string.Empty;
            string focusedColor = string.Empty;
            bool isFocused = false;
            try
            {
                colorVal = (string)values[0];
                focusedColor = (string)values[1];
                isFocused = (bool)values[2];
            }
            catch (Exception)
            {
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            }

            if (!string.IsNullOrEmpty(focusedColor) && isFocused)
                return (SolidColorBrush)new BrushConverter().ConvertFromString(focusedColor);
            else if (!string.IsNullOrEmpty(colorVal) && !isFocused)
                return (SolidColorBrush)new BrushConverter().ConvertFromString(colorVal);

            if (isFocused)
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#DEDEDE");
            else
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class EventConditionBasedFGColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string typeVal = string.Empty;

            try
            {
                typeVal = (string)value;
            }
            catch (Exception)
            {
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#606060");
            }

            if (string.IsNullOrEmpty(typeVal))
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#606060");

            if (typeVal.Equals("#0083C4")) // blue color
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#FaFaFa"); // White color

            if (typeVal.Equals("#62C400")) // green color
                return (SolidColorBrush)new BrushConverter().ConvertFromString("#FaFaFa"); // White color

            return (SolidColorBrush)new BrushConverter().ConvertFromString("#606060");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollBarVisibilityToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visVal = (Visibility)value;

            if (visVal == Visibility.Visible)
                return new Thickness(0, 0, 20, 0);
            else
                return new Thickness(0, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ZeroToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intVal = (int)value;

            if (intVal == 0)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TwoValueToBool : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0)
                return false;

            // Check whether both of the types meet conditions
            if (values[0].GetType() != typeof(int) && values[1].GetType() != typeof(float))
                return false;

            int valOne = (int)values[0];
            float valTwo = (float)values[1];

            if (valOne == 0 && valTwo != 0)
                return true;
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class StringLengthToFontSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int lengthVal = (int)value;

            switch (lengthVal)
            {
                case int n when n >= 0 && n < 50:
                    return 12;
                case int n when n >= 50 && n < 100:
                    return 11;
                case int n when n >= 100 && n < 150:
                    return 10;
                case int n when n >= 150 && n < 190:
                    return 9;
                case int n when n >= 190 && n < 250:
                    return 8;
                case int n when n >= 250 && n < 300:
                    return 7;
                case int n when n >= 300 && n < 390:
                    return 6;
                case int n when n >= 390:
                    return 5;
                default:
                    return 12;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
    public class FloatToDigitFloat : IValueConverter // TODO:H IntegrtyChecksVM de MON ve PTC gelen value string formatlayıp UI katmanına aktarılımı düzenlenecek
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var FormatOfIntegretyItems = _applicationPropertyService.GetByName("FormatOfIntegretyCheckItems").Value;// it has got be "{0:F0}" or "{0:F2}"
            FormatOfIntegretyCheckItems FormatOfIntegretyCheckItems = JsonConvert.DeserializeObject<FormatOfIntegretyCheckItems>(FormatOfIntegretyItems);

            float floatVal = (float)value;

            return String.Format(FormatOfIntegretyCheckItems.FormatOfIntegretyCheckMonItems, floatVal);   //TypeOfActualValue = "{0:F0}" => 0
                                                                      //or TypeOfActualValue = "{0:F2}" => 0.00
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FloatToDigitFloatPTC : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var FormatOfIntegretyItems = _applicationPropertyService.GetByName("FormatOfIntegretyCheckItems").Value;// it has got be "{0:F0}" or "{0:F2}"
            FormatOfIntegretyCheckItems FormatOfIntegretyCheckItems = JsonConvert.DeserializeObject<FormatOfIntegretyCheckItems>(FormatOfIntegretyItems);

            float floatVal = (float)value;

            return String.Format(FormatOfIntegretyCheckItems.FormatOfIntegretyCheckPtcItems, floatVal);   //TypeOfActualValue = "{0:F0}" => 0
                                                                                                          //or TypeOfActualValue = "{0:F2}" => 0.00
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class IntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringVal = value.ToString();
            return stringVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}