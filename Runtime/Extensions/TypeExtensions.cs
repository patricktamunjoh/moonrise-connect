using System;
using System.Collections.Generic;

namespace MoonriseGames.CloudsAhoyConnect.Extensions
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> InheritedTypes(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
