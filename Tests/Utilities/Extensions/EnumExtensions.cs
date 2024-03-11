using System;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions
{
    internal static class EnumExtensions
    {
        public static T[] EnumValues<T>(this Type enumType) => Enum.GetValues(enumType) as T[];
    }
}
