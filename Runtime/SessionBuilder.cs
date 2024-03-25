using System;
using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Extensions;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Steam;

namespace MoonriseGames.Connect
{
    public partial class Session
    {
        /// <summary>Builder for configuring and creating an instance of the Clouds Ahoy Connect library.</summary>
        public class Builder
        {
            private NetworkConnectionStrategy ConnectionStrategy { get; set; }

            /// <summary>Configures the library for using the Steam peer to peer network.</summary>
            public Builder ForSteam()
            {
                ConnectionStrategy = new SteamNetworkConnectionStrategy();
                return this;
            }

            /// <summary>
            /// Builds a new Clouds Ahoy Connect instance which will be used for sending and receiving network function calls. During the lifetime
            /// of the application only one instance can be created.
            /// </summary>
            /// <returns>The configuration instance with the defined settings.</returns>
            public Session Build()
            {
                if (Instance != null)
                {
                    const string message =
                        @"Moonrise Connect has already been initialized. 
                    Only one instance of Moonrise Connect should be build during the lifetime of each game instance.";

                    throw new InvalidOperationException(message.TrimIndents());
                }

                ConnectionStrategy ??= new SteamNetworkConnectionStrategy();

                var registry = new NetworkFunctionRegistry();
                var queue = new NetworkFunctionQueue(registry);

                var connection = new NetworkConnection(ConnectionStrategy, queue);
                var emitter = new NetworkFunctionEmitter(queue, registry, connection);

                return Instance = new Session(connection, queue, registry, emitter);
            }
        }
    }
}
