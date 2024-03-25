using System.Text.RegularExpressions;

namespace MoonriseGames.Connect.Extensions
{
    internal static class StringExtensions
    {
        public static string TrimIndents(this string text) => Regex.Replace(text, @"\n\s+", "\n");
    }
}
