using System.Linq;
using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Enums;

namespace MoonriseGames.Connect.Extensions
{
    public static class NetworkExtensions
    {
        /// <summary>
        /// Checks whether a client instance is currently connected. This property is only relevant for the game instance holding the
        /// <see cref="Roles.Host" /> role. On client game instances this always returns false.
        /// </summary>
        /// <param name="identity">The identity of the client to check the connection status for.</param>
        /// <returns> True, if called on the host instance and the target client instance is connected, otherwise false.</returns>
        public static bool IsClientConnected(this NetworkIdentity identity) => Session.Instance?.ConnectedClients.Any(x => x.Equals(identity)) == true;
    }
}
