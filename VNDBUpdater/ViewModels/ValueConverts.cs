using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VNDBUpdater.Models;

namespace VNDBUpdater.ViewModels
{
    public class TraitsListToStringConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var traits = value as List<Trait>;
                var parentWithTraits = new Dictionary<string, List<Trait>>();
                var sb = new StringBuilder();

                foreach (var trait in traits)
                {
                    var parentTrait = trait.LastParentTrait(trait);

                    if (parentWithTraits.ContainsKey(parentTrait.Name))
                        parentWithTraits[parentTrait.Name].Add(trait);
                    else
                    {
                        parentWithTraits.Add(parentTrait.Name, new List<Trait>());
                        parentWithTraits[parentTrait.Name].Add(trait);
                    }                        
                }

                foreach (var foundTraits in parentWithTraits)
                {
                    sb.Append(foundTraits.Key + ": " + string.Join(", ", foundTraits.Value.Select(x => x.Name)));
                    sb.AppendLine();
                    sb.AppendLine();
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
