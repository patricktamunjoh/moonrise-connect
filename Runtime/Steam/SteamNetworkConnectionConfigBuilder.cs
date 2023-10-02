using System;

namespace MoonriseGames.CloudsAhoyConnect.Steam {
    public partial class SteamNetworkConnectionConfig {

        /// <summary>Builder for creating new configurations for connections on the Steam peer to peer network.</summary>
        public class Builder {

            private SteamNetworkIdentity Host { get; set; }
            private SteamNetworkIdentity[] Clients { get; set; } = Array.Empty<SteamNetworkIdentity>();

            private int? ConnectionEstablishmentTimeoutMs { get; set; }

            /// <summary>Define the configuration for a client game instance.</summary>
            /// <param name="host">The identity of the host game instance to connect to.</param>
            public Builder AsClient(SteamNetworkIdentity host) {
                Clients = null;
                Host = host;
                return this;
            }

            /// <summary>
            /// Define the configuration for the host game instance. All clients that should join the session must be provided. A solo session can
            /// be configured by providing no client identities.
            /// </summary>
            /// <param name="clients">The identities of the client game instances expected to join the session.</param>
            public Builder AsHost(params SteamNetworkIdentity[] clients) {
                Host = null;
                Clients = clients;
                return this;
            }

            /// <summary>
            /// Configures a timeout for the connection establishment. If no connection can be established within the given time limit, the
            /// connection establishment is aborted.
            /// </summary>
            /// <param name="timeoutMs">The connection establishment timeout in milliseconds.</param>
            /// <returns></returns>
            public Builder WithConnectionEstablishmentTimeout(int timeoutMs) {
                ConnectionEstablishmentTimeoutMs = timeoutMs;
                return this;
            }

            /// <summary>Builds a new configuration instance.</summary>
            /// <returns>The configuration instance with the defined settings.</returns>
            public SteamNetworkConnectionConfig Build() {
                var config = Host == null ? new SteamNetworkConnectionConfig(Clients) : new SteamNetworkConnectionConfig(Host);

                if (ConnectionEstablishmentTimeoutMs.HasValue)
                    config.ConnectionEstablishmentTimeoutMs = ConnectionEstablishmentTimeoutMs.Value;

                return config;
            }
        }
    }
}
