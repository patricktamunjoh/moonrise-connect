namespace MoonriseGames.CloudsAhoyConnect.Connection
{
    internal abstract class NetworkConnectionStrategy
    {
        public virtual NetworkConnection Connection { get; set; }

        public abstract void EstablishConnectionToHost(NetworkIdentity host);

        public abstract void StartListeningForClientConnections();

        public abstract void StopListeningForClientConnections();
    }
}
