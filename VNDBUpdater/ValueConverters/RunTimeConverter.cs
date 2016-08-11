using System;
using System.Globalization;
using System.Windows.Data;

namespace VNDBUpdater.ValueConverters
{
    public class RunTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new TimeSpan();
            else
                return new DateTime(((TimeSpan)value).Ticks).ToString("HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
