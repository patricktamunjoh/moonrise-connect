using MoonriseGames.Connect.Enums;

namespace MoonriseGames.Connect.Extensions
{
    internal static class GroupsExtensions
    {
        internal static bool Contains(this Groups groups, Roles role) =>
            groups switch
            {
                Groups.Host => role == Roles.Host,
                Groups.Clients => role == Roles.Client,
                _ => true
            };
    }
}
