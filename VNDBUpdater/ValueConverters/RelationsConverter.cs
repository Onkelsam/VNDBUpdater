using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;

namespace VNDBUpdater.ValueConverters
{
    public class RelationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var sb = new StringBuilder();

                foreach (var relation in (List<Relation>)value)
                    sb.Append(relation.title + " (" + Constants.RelationsMapper[relation.relation] + ") ");

                return sb.ToString();
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
