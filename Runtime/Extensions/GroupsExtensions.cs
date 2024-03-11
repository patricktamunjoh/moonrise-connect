using MoonriseGames.CloudsAhoyConnect.Enums;

namespace MoonriseGames.CloudsAhoyConnect.Extensions
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
