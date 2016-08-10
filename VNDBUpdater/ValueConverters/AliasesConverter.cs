using System;
using System.Globalization;
using System.Windows.Data;

namespace VNDBUpdater.ValueConverters
{
    public class AliasesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.ToString().Replace("\n", ", ");
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
