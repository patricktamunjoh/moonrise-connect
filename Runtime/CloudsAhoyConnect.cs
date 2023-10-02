using System;
using System.Collections.Generic;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Logging;

namespace MoonriseGames.CloudsAhoyConnect {
    /// <summary>
    /// Entry point to the Clouds Ahoy Connect library. All functionality can be assessed through the singleton instance of this class or
    /// static extension functions. Use the <see cref="Builder" /> to configure and instantiate a new instance.
    /// </summary>
    public partial class CloudsAhoyConnect {

        internal static CloudsAhoyConnect Instance { get; set; }

        /// <summary>
        /// The role this game instance holds within the network. The role is determined from the configuration provided during connection
        /// establishment. The role is null while disconnected.
        /// </summary>
        public Roles? Role => Connectivity == Connectivity.Disconnected ? null : Connection.Role;

        /// <summary>
        /// The state the connection is currently in. The initial state is <see cref="Connectivity.Disconnected" /> and changes as connections
        /// are established or dropped.
        /// </summary>
        public Connectivity Connectivity => Connection.Connectivity;

        /// <summary>
        /// The client identities configured to be connected for the current session. This property is only relevant for the game instance
        /// holding the <see cref="Roles.Host" /> role.
        /// </summary>
        public IEnumerable<NetworkIdentity> Clients => Connection.Clients;

        /// <summary>
        /// The identities of all client instances currently connected. This property is only relevant for the game instance holding the
        /// <see cref="Roles.Host" /> role. After a successfully established connection, this contains all configured clients. If clients disconnect
        /// during the session, these clients will no longer be provided.
        /// </summary>
        public IEnumerable<NetworkIdentity> ConnectedClients => Connection.ActiveClients;

        /// <summary>
        /// The total number of clients configured to be connected for the current session. This property is only relevant for the game
        /// instance holding the <see cref="Roles.Host" /> role.
        /// </summary>
        public int ClientCount => Connection.ClientCount;

        /// <summary>
        /// The number of client instances currently connected. This property is only relevant for the game instance holding the
        /// <see cref="Roles.Host" /> role.
        /// </summary>
        public int ConnectedClientsCount => Connection.ActiveClientCount;

        internal NetworkConnection Connection { get; }

        internal NetworkFunctionQueue Queue { get; }
        internal NetworkFunctionRegistry Registry { get; }
        internal NetworkFunctionEmitter Emitter { get; }

        internal CloudsAhoyConnect(
            NetworkConnection connection,
            NetworkFunctionQueue queue,
            NetworkFunctionRegistry registry,
            NetworkFunctionEmitter emitter
        ) {
            Connection = connection;
            Connection.OnNetworkConnectionChanged += (_, args) => OnNetworkConnectionChanged?.Invoke(this, args);

            Queue = queue;
            Registry = registry;
            Emitter = emitter;
        }

        /// <summary>
        /// Event raised when the network connection goes through certain state changes. This can be used to handle error states such as
        /// network disruptions and disconnects or to advance the game when connection establishment was successful.
        /// </summary>
        public event EventHandler<NetworkConnectionEventArgs> OnNetworkConnectionChanged;

        /// <summary>
        /// Establishes a network connection with the given configuration. When configured as client instance a proactive connection attempt
        /// to the host will be made. When configured as host instance incoming client connections are awaited until all configured clients are
        /// connected. For successful connection establishment, this function should be called simultaneously on all participating game instances.
        /// </summary>
        /// <param name="config">The configuration determining the connection behaviour.</param>
        public void EstablishConnection(NetworkConnectionConfig config) => Connection.Establish(config);

        /// <summary>
        /// Polls all active connections for incoming messages and pushes received messages on a queue. This should ideally be called every
        /// frame to minimize latency.
        /// </summary>
        public void PollConnection() => Connection.Poll();

        /// <summary>
        /// Closes all active connections, if any. This does not clear registered objects. Use <see cref="Reset" /> before establishing a new
        /// connection.
        /// </summary>
        public void DropConnection() => Connection.Drop();

        /// <summary>
        /// Removes all registered objects as well as resetting the object id counter. This should be used after a session is concluded to
        /// restore the default state before registering objects for a subsequent session.
        /// </summary>
        public void Reset() => Registry.ClearRegistrationsAndResetCounter();

        /// <summary>
        /// Invokes all queued network function calls. This executes all the remote and local network functions that were called since the
        /// last time this function was executed. This should ideally be called every frame to minimize latency. To process incoming network calls as
        /// soon as possible, this should be called shortly after <see cref="PollConnection" /> was invoked.
        /// </summary>
        public void ProcessQueuedNetworkFunctionCalls() => Queue.ProcessQueuedElements();

        /// <summary>
        /// Registers all active and inactive game objects in all currently loaded scenes. Because this requires a complete traversal of the
        /// entire scene graph, this function should not be called frequently. It is best used after loading a scene to register all initial network
        /// objects. When using this function be careful not to register game objects multiple times. Because game objects are registered in
        /// alphabetical order every game object must have a unique name. If this is not possible it is best to define a custom sorting order and to
        /// register each object manually.
        /// </summary>
        public void RegisterAllGameObjects() => Registry.RegisterAllGameObjects();

        /// <summary>
        /// Removes all object registrations but keeps the object id counter intact. This can be used to clear all registrations to
        /// re-register certain objects.
        /// </summary>
        public void ClearAllObjectRegistrations() => Registry.ClearRegistrations();

        /// <summary>
        /// Starts recording a snapshot which contains information about all object registration and network calls. This should only be used
        /// for debugging purposes and detecting issues with diverging game instance states.
        /// </summary>
        public void StartRecordingSnapshot() {
            var snapshot = new Snapshot();

            Registry.Snapshot = snapshot;
            Connection.Snapshot = snapshot;
        }

        /// <summary>Stops the recording of the current snapshot</summary>
        /// <returns>A snapshot of all network calls and object registrations since the last call to <see cref="StartRecordingSnapshot" />.</returns>
        public Snapshot StopRecordingAndCollectSnapshot() {
            if (Connection.Snapshot == null) {
                var message = $@"No snapshot is currently being recorded.
                    Make sure to call {nameof(StartRecordingSnapshot)} before collecting the snapshot results.";

                throw new InvalidOperationException(message.TrimIndents());
            }

            var snapshot = Connection.Snapshot;

            Registry.Snapshot = null;
            Connection.Snapshot = null;

            return snapshot;
        }
    }
}
