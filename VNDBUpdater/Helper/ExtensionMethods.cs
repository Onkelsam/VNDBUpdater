using System;
using System.Collections.Generic;

namespace VNDBUpdater.Helper
{
    public static class ExtensionMethods
    {
        public static T[] Take<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
