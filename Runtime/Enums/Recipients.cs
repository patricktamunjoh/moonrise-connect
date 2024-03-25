namespace MoonriseGames.Connect.Enums
{
    /// <summary>
    /// The different instances or groups of instances messages can be send to. This can be used to determine who should be receiving a message
    /// once it is sent. Recipients may also include the sending instance itself.
    /// <example>
    /// Calling a function set to <see cref="Clients" /> on the hosting instance, will only invoke the function on client instances, not
    /// the host itself.
    /// </example>
    /// </summary>
    public enum Recipients
    {
        /// <summary>Messages will only be delivered to the hosting game instance.</summary>
        Host,

        /// <summary>Messages will be delivered to all connected client game instances, but not the hosting game instance.</summary>
        Clients,

        /// <summary>Messages will be delivered to all game instances except for the instance sending the message.</summary>
        Others,

        /// <summary>Message will be delivered to all connected game instances, as well as the instance sending the message.</summary>
        All
    }
}
