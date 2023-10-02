namespace MoonriseGames.CloudsAhoyConnect.Enums {

    /// <summary>
    /// The different transmission modes available for sending data. These roughly translate to the connection oriented or connectionless
    /// properties of the TCP and UDP protocols.
    /// </summary>
    public enum Transmission {

        /// <summary>
        /// Messages will be sent reliably, ensuring that every message is delivered correctly and that messages are processed in their
        /// original order. This can cause overhead as messages must be acknowledged and missing messages have to be resend. To ensure correct game
        /// state replication, this transmission type should be used for all calls that modify the game state.
        /// </summary>
        Reliable,

        /// <summary>
        /// Messages will be sent unreliably, without any guarantees for message delivery or order. This should be used for frequent calls
        /// which do not modify the game state in a persistent way.
        /// <example>Positional updates or simulation ticks.</example>
        /// </summary>
        Unreliable
    }
}
