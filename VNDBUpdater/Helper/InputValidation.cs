using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using VNDBUpdater.Data;

namespace VNDBUpdater.Helper
{
    public class InputValidation : ValidationRule
    {
        private static bool _IsValid;

        public static bool IsValid
        {
            get { return _IsValid; }
            set { _IsValid = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var regex = new Regex(@"^([0-9])*$");
            var val = value.ToString().Trim();

            if (string.IsNullOrEmpty(val))
            {
                _IsValid = false;
                return new ValidationResult(false, Constants.InputValidationStringEmpty);
            }

            if (val.Contains(','))
            {
                string[] split = val.Split(',');

                foreach (var input in split)
                {
                    if (!regex.IsMatch(input))
                    {
                        _IsValid = false;
                        return new ValidationResult(false, Constants.InputValidationFalseInput);
                    }
                }

                _IsValid = true;
                return ValidationResult.ValidResult;
            }
            else
            {
                if (regex.IsMatch(val))
                {
                    _IsValid = true;
                    return ValidationResult.ValidResult;
                }
                else
                {
                    _IsValid = false;
                    return new ValidationResult(false, Constants.InputValidationFalseInput);
                }
            }          
        }
    }
}
