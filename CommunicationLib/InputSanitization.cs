using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    internal static class InputSanitization
    {
        internal static void CheckStringArgument(string argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument) + " is null!");

            if (string.IsNullOrEmpty(argument))
                throw new ArgumentException(nameof(argument) + " is empty!");
        }

        internal static void CheckIDArray(int[] IDs)
        {
            if (IDs == null)
                throw new ArgumentNullException(nameof(IDs) + " is null!");

            if (!IDs.Any())
                throw new ArgumentException(nameof(IDs) + " is empty!");

            if (IDs.Length > 100)
                throw new ArgumentException("Max 100 IDs allowed!");
        }

        internal static void CheckInt(int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number) + " is negative is 0!");
        }

        internal static void CheckRange(int number, int min, int max)
        {
            if (number < min || number > max)
                throw new ArgumentOutOfRangeException(nameof(number) + " is out of range!");
        }
    }
}
