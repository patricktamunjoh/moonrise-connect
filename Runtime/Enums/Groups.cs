namespace MoonriseGames.Connect.Enums
{
    /// <summary>
    /// The different groups the network can be divided into. Each session will have exactly one <see cref="Host" /> game instance. All
    /// other instances are part of the <see cref="Clients" /> group.
    /// </summary>
    public enum Groups
    {
        /// <summary>The group only containing the single game instance, hosting the session.</summary>
        Host,

        /// <summary>The group containing all game instances, except for the hosting instance.</summary>
        Clients,

        /// <summary>The group containing all game instances that are part of the network.</summary>
        All
    }
}
