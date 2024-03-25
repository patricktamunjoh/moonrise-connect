using System;

namespace MoonriseGames.Connect.Tests.Utilities.Extensions
{
    internal static class EnumExtensions
    {
        public static T[] EnumValues<T>(this Type enumType) => Enum.GetValues(enumType) as T[];
    }
}
