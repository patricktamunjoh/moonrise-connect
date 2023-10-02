using System.Text.RegularExpressions;

namespace MoonriseGames.CloudsAhoyConnect.Extensions {
    internal static class StringExtensions {

        public static string TrimIndents(this string text) => Regex.Replace(text, @"\n\s+", "\n");
    }
}
