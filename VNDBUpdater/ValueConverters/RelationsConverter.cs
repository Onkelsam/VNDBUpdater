using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                var relations = (value as List<Relation>);
                var sb = new StringBuilder();

                foreach (var relation in Constants.RelationsMapper)
                {
                    if (relations.Any(x => x.relation == relation.Key))
                    {
                        sb.Append(relation.Value + ":");

                        foreach (var rel in relations.Where(x => x.relation == relation.Key))
                            sb.Append(Environment.NewLine + "\t\t\t" + rel.title);

                        sb.AppendLine();
                        sb.Append("\t\t");
                    }                    
                }

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
