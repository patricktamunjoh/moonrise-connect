using MoonriseGames.Connect.Connection;

namespace MoonriseGames.Connect.Steam
{
    /// <summary>
    /// Holds the configuration for how a new network connection over the Steam peer to peer servers should be established. Use the
    /// <see cref="Builder" /> for creating new configurations. The configuration can be set for the game instance to act as host or as client
    /// instance. For a client instance the identity of the host instance must be defined. For a host instance the identities of all expected
    /// client instances must be defined. Connection establishment is only considered successful when all configured client instances are
    /// connected.
    /// </summary>
    public partial class SteamNetworkConnectionConfig : NetworkConnectionConfig
    {
        internal SteamNetworkConnectionConfig(SteamNetworkIdentity host)
            : base(host) { }

        internal SteamNetworkConnectionConfig(SteamNetworkIdentity[] clients)
            : base(clients) { }
    }
}
