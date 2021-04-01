using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Insolvency.Common
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                       .GetMember(enumValue.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()
                       // using `GetName()` instead `Name` ensures we're getting localised value
                       ?.GetName();
        }
    }

    public static class NullableBooleanExtensions
    {
        public static bool IsTrue(this bool? property)
        {
            return property.HasValue && property.Value;
        }
    }
}