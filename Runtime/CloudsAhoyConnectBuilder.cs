using System;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Steam;

namespace MoonriseGames.CloudsAhoyConnect {
    public partial class CloudsAhoyConnect {

        /// <summary>Builder for configuring and creating an instance of the Clouds Ahoy Connect library.</summary>
        public class Builder {

            private NetworkConnectionStrategy ConnectionStrategy { get; set; }

            /// <summary>Configures the library for using the Steam peer to peer network.</summary>
            public Builder ForSteam() {
                ConnectionStrategy = new SteamNetworkConnectionStrategy();
                return this;
            }

            /// <summary>
            /// Builds a new Clouds Ahoy Connect instance which will be used for sending and receiving network function calls. During the lifetime
            /// of the application only one instance can be created.
            /// </summary>
            /// <returns>The configuration instance with the defined settings.</returns>
            public CloudsAhoyConnect Build() {
                if (Instance != null) {
                    const string message = @"Clouds Ahoy Connect has already been initialized. 
                    Only one instance of Clouds Ahoy Connect should be build during the lifetime of each game instance.";

                    throw new InvalidOperationException(message.TrimIndents());
                }

                ConnectionStrategy ??= new SteamNetworkConnectionStrategy();

                var registry = new NetworkFunctionRegistry();
                var queue = new NetworkFunctionQueue(registry);

                var connection = new NetworkConnection(ConnectionStrategy, queue);
                var emitter = new NetworkFunctionEmitter(queue, registry, connection);

                return Instance = new CloudsAhoyConnect(connection, queue, registry, emitter);
            }
        }
    }
}
