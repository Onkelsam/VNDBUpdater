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

        public static T NextOf<T>(this IList<T> list, T item)
        {
            return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
        }

        public static T PreviousOf<T>(this IList<T> list, T item)
        {
            return list[list.IndexOf(item) == 0 ? list.Count - 1 : (list.IndexOf(item) - 1)];
        }
    }
}
