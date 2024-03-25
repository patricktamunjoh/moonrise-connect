using System;
using System.Collections.Generic;
using System.Linq;
using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Logging;
using static MoonriseGames.Connect.Enums.Connectivity;

namespace MoonriseGames.Connect.Connection
{
    internal class NetworkConnection
    {
        public virtual Snapshot Snapshot { get; set; }

        private NetworkConnectionStrategy Strategy { get; }
        private NetworkFunctionQueue Queue { get; }
        private NetworkConnectionConfig ConnectionConfig { get; set; }

        private NetworkLink LinkToHost { get; set; }
        private Dictionary<NetworkIdentity, NetworkLink> LinksToClients { get; } = new();

        public virtual Roles Role => ConnectionConfig?.Role ?? Roles.Host;

        public virtual Connectivity Connectivity { get; private set; } = Disconnected;

        public virtual IEnumerable<NetworkIdentity> Clients => ConnectionConfig?.Clients ?? Enumerable.Empty<NetworkIdentity>();
        public virtual IEnumerable<NetworkIdentity> ActiveClients => LinksToClients.Values.Where(x => x.IsActive).Select(x => x.Identity);

        public virtual int ClientCount => ConnectionConfig?.Clients?.Length ?? 0;
        public virtual int ActiveClientCount => ActiveClients.Count();

        private NetworkTimeout Timeout { get; set; }

        public NetworkConnection(NetworkConnectionStrategy strategy, NetworkFunctionQueue queue)
        {
            Strategy = strategy;
            Strategy.Connection = this;

            Queue = queue;
            OnNetworkConnectionChanged += RecordNetworkEventToSnapshot;
        }

        public virtual event EventHandler<NetworkConnectionEventArgs> OnNetworkConnectionChanged;

        private void RecordNetworkEventToSnapshot(object _, NetworkConnectionEventArgs args) => Snapshot?.RecordNetworkEvent(args);

        public virtual void Establish(NetworkConnectionConfig config)
        {
            if (Connectivity != Disconnected)
                Drop();

            ConnectionConfig = config;
            var isSoloSession = config.Clients?.Length == 0;

            TransitionState(Connecting);

            switch (config.Role)
            {
                case Roles.Host when !isSoloSession:
                    Strategy.StartListeningForClientConnections();
                    break;
                case Roles.Client:
                    Strategy.EstablishConnectionToHost(config.Host);
                    break;
            }

            Timeout = new NetworkTimeout(config.ConnectionEstablishmentTimeoutMs, HandleConnectionEstablishmentTimeout);
            Timeout.Start();

            if (isSoloSession)
                HandleConnectionEstablishmentSuccessful();
        }

        public virtual void Drop()
        {
            Timeout?.Cancel();

            LinkToHost?.Close();
            foreach (var client in LinksToClients.Values)
                client.Close();

            Strategy.StopListeningForClientConnections();

            ConnectionConfig = null;
            LinkToHost = null;
            LinksToClients.Clear();

            TransitionState(Disconnected);
        }

        public virtual void Poll()
        {
            if (Connectivity != Connected)
                return;

            if (LinkToHost != null)
                Poll(LinkToHost);
            foreach (var client in LinksToClients.Values)
                Poll(client);
        }

        private void Poll(NetworkLink link)
        {
            if (!link.IsActive)
                return;
            while (link.Receive() is { } bytes)
                ProcessIncomingNetworkCall(new NetworkFunctionCall(bytes), link.Identity);
        }

        private void ProcessIncomingNetworkCall(NetworkFunctionCall call, NetworkIdentity sender)
        {
            Snapshot?.RecordIncomingNetworkCall(sender, call);

            if (Role == Roles.Host)
                Broadcast(call, sender);
            Queue.EnqueueCall(call, Role, false);
        }

        private void Broadcast(NetworkFunctionCall call, NetworkIdentity exception = null)
        {
            var validClients = LinksToClients.Values.Where(x => !x.Identity.Equals(exception));
            var data = call.ToBytes();

            foreach (var client in validClients)
                Send(call, client, data);
        }

        public virtual void Send(NetworkFunctionCall call)
        {
            if (Connectivity != Connected)
                return;

            if (Role == Roles.Host)
                Broadcast(call);
            else
                Send(call, LinkToHost);
        }

        private void Send(NetworkFunctionCall call, NetworkLink link, byte[] data = null)
        {
            if (!link.IsActive)
                return;
            link.Send(data ?? call.ToBytes(), call.Transmission);
            Snapshot?.RecordOutgoingNetworkCall(link.Identity, call);
        }

        public virtual void ReceiveNewActiveNetworkLink(NetworkLink link)
        {
            if (link.Identity.Equals(ConnectionConfig?.Host))
                ReceiveLinkToHost(link);
            else
                ReceiveLinkToClient(link);
        }

        private void ReceiveLinkToHost(NetworkLink link)
        {
            if (Connectivity != Connecting)
            {
                var log =
                    $@"Ignoring outgoing connection to {link.Identity.DisplayName}. 
                    Connection was received outside the connection establishment window.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            if (ConnectionConfig?.Role != Roles.Client)
            {
                var log =
                    $@"Ignoring outgoing connection to {link.Identity.DisplayName}. 
                    Connection was received although the current instance is not configured as client.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            if (ConnectionConfig?.Host.Equals(link.Identity) != true)
            {
                var log =
                    $@"Ignoring outgoing connection to {link.Identity.DisplayName}. 
                    Connection was unexpected because the connection id does not match the configured host identity.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            LinkToHost = link;
            HandleConnectionEstablishmentSuccessful();
        }

        private void ReceiveLinkToClient(NetworkLink link)
        {
            if (Connectivity != Connecting)
            {
                var log =
                    $@"Ignoring incoming connection from {link.Identity.DisplayName}. 
                    Connection was received outside the connection establishment window.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            if (ConnectionConfig?.Role != Roles.Host)
            {
                var log =
                    $@"Ignoring incoming connection from {link.Identity.DisplayName}. 
                    Connection was received although the current instance is not configured as host.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            if (!Clients.Contains(link.Identity))
            {
                var log =
                    $@"Ignoring incoming connection from {link.Identity.DisplayName}. 
                    Connection was unexpected because the client id was not configured.
                    Make sure to include the identity of all required client instances in the connection config.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            if (LinksToClients.TryGetValue(link.Identity, out var currentLink) && currentLink.IsActive)
            {
                var log =
                    $@"Ignoring incoming connection from {link.Identity.DisplayName}. 
                    Client instance with this identity is already connected.
                    Make sure to only establish a connection to the host instance once from each client instance.";

                NetworkLogger.Warn(log);
                link.Close();
                return;
            }

            LinksToClients[link.Identity] = link;

            var areAllClientsConnected = Clients.All(x => LinksToClients.TryGetValue(x, out var clientLink) && clientLink.IsActive);
            if (areAllClientsConnected)
                HandleConnectionEstablishmentSuccessful();
        }

        private void HandleConnectionEstablishmentSuccessful()
        {
            Timeout?.Cancel();

            const string log = "Network connection establishment successful.";
            NetworkLogger.Info(log);

            TransitionState(Connected);
        }

        public virtual void HandleConnectionDisrupted(NetworkIdentity target)
        {
            if (Connectivity == Disconnected)
            {
                var log =
                    $@"Ignoring incoming connection error from {target.DisplayName}. 
                    Connection is disconnected and will not handle network callbacks.";

                NetworkLogger.Warn(log);
                return;
            }

            if (ConnectionConfig?.Role == Roles.Client)
                HandleConnectionDisruptedToHost(target);
            if (ConnectionConfig?.Role == Roles.Host)
                HandleConnectionDisruptedToClient(target);
        }

        private void HandleConnectionDisruptedToHost(NetworkIdentity host)
        {
            if (ConnectionConfig?.Host.Equals(host) != true)
                return;

            var log = $"Network connection to host {LinkToHost?.Identity.DisplayName} lost.";
            NetworkLogger.Warn(log);

            Drop();
        }

        private void HandleConnectionDisruptedToClient(NetworkIdentity client)
        {
            if (!LinksToClients.TryGetValue(client, out var link) || !link.IsActive)
                return;

            var log = $"Network connection to client {link.Identity.DisplayName} lost.";
            NetworkLogger.Warn(log);

            link.Close();

            var eventArgs = NetworkConnectionEventArgs.ForConnectionToClientLost(client);
            OnNetworkConnectionChanged?.Invoke(this, eventArgs);

            if (Connectivity == Connecting)
                HandleConnectionEstablishmentFailed(client);
        }

        private void HandleConnectionEstablishmentTimeout()
        {
            const string log = "Network connection establishment failed after timeout.";
            NetworkLogger.Error(log);

            HandleConnectionEstablishmentFailed();
        }

        public virtual void HandleConnectionEstablishmentFailed(NetworkIdentity client = null)
        {
            if (Connectivity != Connecting)
                return;

            if (client != null && ConnectionConfig?.Clients?.Contains(client) != true)
            {
                var log =
                    $@"Ignoring failed connection establishment with {client.DisplayName}. 
                    Connection was unexpected because the client id was not configured.";

                NetworkLogger.Warn(log);
                return;
            }

            Drop();
        }

        private void TransitionState(Connectivity targetState)
        {
            if (targetState == Connectivity)
                return;

            var eventArgs = targetState switch
            {
                Connected when Connectivity == Connecting => NetworkConnectionEventArgs.ForConnectionEstablished(),
                Disconnected when Connectivity == Connected => NetworkConnectionEventArgs.ForConnectionLost(),
                Disconnected when Connectivity == Connecting => NetworkConnectionEventArgs.ForConnectionEstablishmentFailed(),
                _ => null
            };

            Connectivity = targetState;
            if (eventArgs != null)
                OnNetworkConnectionChanged?.Invoke(this, eventArgs);
        }
    }
}
