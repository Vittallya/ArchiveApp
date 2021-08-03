using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Main.Resources
{
    public class ConverterBoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            if (bool.TryParse(value.ToString(), out bool res))
            {
                return res ? Visibility.Visible : Visibility.Collapsed;
            }

            try
            {
                bool val = System.Convert.ToBoolean(value);
                return val;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                return vis == Visibility.Visible;
            }



            return value;
        }
    }
    public class ConverterBoolToVisibilityInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value.ToString(), out bool res))
            {
                return !res ? Visibility.Visible : Visibility.Collapsed;
            }

            try
            {
                bool val = System.Convert.ToBoolean(value);
                return !val;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                return vis != Visibility.Visible;
            }
            return value;
        }
    }
    public class ConverterBoolInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value.ToString(), out bool res))
            {
                return !res;
            }
            try
            {
                bool val = System.Convert.ToBoolean(value);
                return !val;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value.ToString(), out bool res))
            {
                return !res;
            }

            return value;
        }
    }


    public class ConverterVisibilityInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                if (vis == Visibility.Collapsed || vis == Visibility.Hidden)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                if (vis == Visibility.Collapsed || vis == Visibility.Hidden)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            return value;
        }
    }
    public class ConverterNullToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ConverterNullToVisibilityInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !object.Equals(value, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ConverterMinuteToTimeSpan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int res))
            {
                return TimeSpan.FromMinutes(res);
            }
            return TimeSpan.FromMinutes(0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
