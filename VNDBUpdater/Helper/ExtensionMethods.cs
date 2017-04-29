using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace VNDBUpdater.Helper
{
    public static class ExtensionMethods
    {
        public static T GetEnumValueFromDescription<T>(this string description)
        {
            var type = typeof(T);

            FieldInfo[] fields = type.GetFields();

            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((DescriptionAttribute)a.Att)
                                .Description == description).SingleOrDefault();

            return field == null
                ? default(T) 
                : (T)field.Field.GetRawConstantValue();
        }

        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();

            var memInfo = type.GetMember(enumVal.ToString());

            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return (attributes.Length > 0) 
                ? (T)attributes[0] 
                : null;
        }

        public static PropertyInfo FollowPropertyPath(this object value, string path)
        {
            Type currentType = value.GetType();
            PropertyInfo currentProperty = null;

            foreach (string propertyName in path.Split('.'))
            {
                PropertyInfo property = currentType.GetProperty(propertyName);
                currentProperty = property;
                currentType = property.PropertyType;
            }

            return currentProperty;
        }

        public static object FollowPropertyPathAndGetValue(this object value, object obj, string path)
        {
            Type currentType = obj.GetType();

            foreach (string propertyName in path.Split('.'))
            {
                PropertyInfo property = currentType.GetProperty(propertyName);
                obj = property.GetValue(obj, null);
                currentType = property.PropertyType;
            }

            return obj;
        }
    }
}
