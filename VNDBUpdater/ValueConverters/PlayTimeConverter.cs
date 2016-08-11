using System;
using System.Globalization;
using System.Windows.Data;

namespace VNDBUpdater.ValueConverters
{
    public class PlayTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var span = (TimeSpan)value;

            if (value == null)
                return new TimeSpan();
            else
                return string.Format("{0} day{1}, {2} hour{3}, {4} minute{5}",
                                      span.Days,
                                      span.Days == 1 ? "" : "s",
                                      span.Hours,
                                      span.Hours == 1 ? "" : "s",
                                      span.Minutes,
                                      span.Minutes == 1 ? "" : "s");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
