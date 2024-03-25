using MoonriseGames.Connect.Enums;

namespace MoonriseGames.Connect.Connection
{
    /// <summary>
    ///     Holds the configuration for how a new network connection should be established. Depending on the configuration, a connection to a
    ///     host instance is established or incoming client connections are awaited.
    /// </summary>
    public class NetworkConnectionConfig
    {
        internal Roles Role { get; }

        internal NetworkIdentity Host { get; }
        internal NetworkIdentity[] Clients { get; }

        internal int ConnectionEstablishmentTimeoutMs { get; private protected set; } = -1;

        internal NetworkConnectionConfig(NetworkIdentity host, int? connectionEstablishmentTimeoutMs = null)
        {
            Role = Roles.Client;
            Host = host;
            if (connectionEstablishmentTimeoutMs.HasValue)
                ConnectionEstablishmentTimeoutMs = connectionEstablishmentTimeoutMs.Value;
        }

        internal NetworkConnectionConfig(NetworkIdentity[] clients, int? connectionEstablishmentTimeoutMs = null)
        {
            Role = Roles.Host;
            Clients = clients;
            if (connectionEstablishmentTimeoutMs.HasValue)
                ConnectionEstablishmentTimeoutMs = connectionEstablishmentTimeoutMs.Value;
        }
    }
}
