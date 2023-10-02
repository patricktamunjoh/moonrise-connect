namespace MoonriseGames.CloudsAhoyConnect.Enums {

    /// <summary>The different states a connection can be in.</summary>
    public enum Connectivity {

        /// <summary>There is no active network connection between client ans host.</summary>
        Disconnected,

        /// <summary>
        /// The connection between client and host is established and ready to transfer messages. On a client instance this indicated a
        /// successful connection to the host instance. On the host this indicates a successful connection to all expected client instances.
        /// </summary>
        Connected,

        /// <summary>The connection between client and host is currently being established.</summary>
        Connecting
    }
}
