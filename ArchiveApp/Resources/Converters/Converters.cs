using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ArchiveApp.Converters
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
            Visibility vis = Equals(value, null) ? Visibility.Collapsed : Visibility.Visible;
            return vis;
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
            Visibility vis = !Equals(value, null) ? Visibility.Collapsed : Visibility.Visible;
            return vis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    
    public class ConverterNonZeroToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IComparable comp)
            {
                Visibility vis = comp.CompareTo(0) == 1 ? Visibility.Visible : Visibility.Collapsed;
                return vis;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class ConverterZeroToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IComparable comp)
            {
                Visibility vis = comp.CompareTo(0) == 0 ? Visibility.Visible : Visibility.Collapsed;
                return vis;
            }

            return value;
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

    public class ConverterSocial : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Models.SocialStatus soc)
            {
                switch (soc)
                {
                    case Models.SocialStatus.Worker: return "Рабочий";
                    case Models.SocialStatus.Peasant: return "Крестьянин";
                    case Models.SocialStatus.Employee: return "Служащий";
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class ConverterFamily : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Models.FamilyStatus f)
            {
                switch (f)
                {
                    case Models.FamilyStatus.MarriedMale: return "Женат";
                    case Models.FamilyStatus.MarriedFeemale: return "Замужем";
                    case Models.FamilyStatus.Widower: return "Вдовец(-ва)";
                    case Models.FamilyStatus.Divorced: return "Разведен(-а)";
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ConverterGender : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool gender)
            {

                if (value == null)
                {
                    return "Не определено";
                }
                return gender ? "Мужской" : "Женский";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ConverterGenderShort : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool gender)
            {

                if (value == null)
                {
                    return "Неопр.";
                }
                return gender ? "М" : "Ж";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class ConverterParty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Models.PartyStatus p)
            {
                switch (p)
                {
                    case Models.PartyStatus.None: return "Беспартийный";
                    case Models.PartyStatus.Member: return "Член партии";
                    case Models.PartyStatus.Komsomol: return "Комсомол";
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ConverterEducation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Models.EducationKind p)
            {
                switch (p)
                {
                    case Models.EducationKind.None: return "Без образования";
                    case Models.EducationKind.Medium: return "Среднее";
                    case Models.EducationKind.High: return "Высшее";
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
