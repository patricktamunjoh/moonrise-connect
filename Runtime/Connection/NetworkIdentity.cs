namespace MoonriseGames.CloudsAhoyConnect.Connection
{
    /// <summary>
    /// Represents a games instance within the network. Each game instance must have a unique identity it can be referenced and identified
    /// by. This is necessary for correctly marshalling messages between instances.
    /// </summary>
    public interface NetworkIdentity
    {
        /// <summary>A human readable representation of the identity. This may not be unique.</summary>
        string DisplayName { get; }
    }
}
