using System;

namespace MoonriseGames.Connect.Connection
{
    /// <summary>Container class holding information regarding a change on the network connection.</summary>
    public class NetworkConnectionEventArgs : EventArgs
    {
        /// <summary>The different types of network connection events that may be raised.</summary>
        public enum Types
        {
            /// <summary>
            /// A connection between host and client has been established. On a client instance this indicated a successful connection to the host
            /// instance. On the host this indicates a successful connection to all expected client instances.
            /// </summary>
            ConnectionEstablished,

            /// <summary>
            /// The connection between host and client was lost. On a client instance this indicates that the connection to the host was lost. On
            /// a host instance this indicated that all client connections were lost. This is usually the case if the connection was manually dropped or if
            /// the network connectivity is disrupted.
            /// </summary>
            ConnectionLost,

            /// <summary>
            /// The connection to one client has been lost. This is only relevant on the host and indicated that a single client connection was
            /// lost. This usually indicated a connectivity problem on the client side, if other client connections remain active.
            /// </summary>
            ConnectionToClientLost,

            /// <summary>
            /// No connection between host and client could be established. On a client instance this indicates that the host instance could not
            /// be connected to. On a host instance this indicated that one or more client instances could not be connected to. This might happen if a
            /// client instance fails to connect within the set connection establishment timeout.
            /// </summary>
            ConnectionEstablishmentFailed
        }

        /// <summary>The type of the related connection event. This indicates the reason why the event was raised.</summary>
        public Types Type { get; }

        /// <summary>The identity of the instance the related event refers to.
        /// <value>The target identity or null, if no relevant target is available.</value>
        /// <example>On <see cref="Types.ConnectionToClientLost" /> events the target contains the identity of the disconnected client instance.</example>
        /// </summary>
        public NetworkIdentity Target { get; }

        private NetworkConnectionEventArgs(Types type, NetworkIdentity target = null)
        {
            Type = type;
            Target = target;
        }

        internal static NetworkConnectionEventArgs ForConnectionToClientLost(NetworkIdentity client) => new(Types.ConnectionToClientLost, client);

        internal static NetworkConnectionEventArgs ForConnectionEstablished() => new(Types.ConnectionEstablished);

        internal static NetworkConnectionEventArgs ForConnectionEstablishmentFailed() => new(Types.ConnectionEstablishmentFailed);

        internal static NetworkConnectionEventArgs ForConnectionLost() => new(Types.ConnectionLost);
    }
}
