namespace MoonriseGames.Connect.Enums
{
    /// <summary>The different roles a game instance can hold within the network.</summary>
    public enum Roles
    {
        /// <summary>
        /// The game instance acts as the host, facilitating communication to and between all clients instances. Every session features
        /// exactly one host.
        /// </summary>
        Host,

        /// <summary>The game instance acts as a client, joining a session created by a remote host instance. A session can have zero or more clients.</summary>
        Client
    }
}
