using MoonriseGames.CloudsAhoyConnect.Enums;

namespace MoonriseGames.CloudsAhoyConnect.Connection {
    public abstract class NetworkLink {

        public NetworkIdentity Identity { get; }
        public bool IsActive { get; private set; }

        protected NetworkLink(NetworkIdentity identity) {
            Identity = identity;
            IsActive = true;
        }

        public virtual void Close() => IsActive = false;

        public abstract void Send(byte[] data, Transmission transmission);

        public abstract byte[] Receive();
    }
}
