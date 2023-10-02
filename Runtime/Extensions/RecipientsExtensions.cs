using MoonriseGames.CloudsAhoyConnect.Enums;

namespace MoonriseGames.CloudsAhoyConnect.Extensions {
    internal static class RecipientsExtensions {

        internal static bool Contains(this Recipients recipients, Roles role, bool isSender) => recipients switch {
            Recipients.Host    => role == Roles.Host,
            Recipients.Clients => role == Roles.Client,
            Recipients.Others  => !isSender,
            _                  => true
        };
    }
}
