using System;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Logging;
using Steamworks;
using static Steamworks.ESteamNetworkingConnectionState;

namespace MoonriseGames.CloudsAhoyConnect.Steam {
    internal class SteamNetworkConnectionStrategy : NetworkConnectionStrategy {

        private HSteamListenSocket? ListenSocket { get; set; }
        private Callback<SteamNetConnectionStatusChangedCallback_t> CallbackConnectionChanged { get; set; }

        public SteamNetworkConnectionStrategy() =>
            CallbackConnectionChanged = SteamProxy.Instance.CreateConnectionStatusChangedCallback(OnConnectionChanged);

        public override void EstablishConnectionToHost(NetworkIdentity host) {
            if (host is not SteamNetworkIdentity identity) {
                var message = @$"The provided host network identity {host?.DisplayName} is no valid Steam identity.
                    Make sure to provide Steam identities when connecting through the Steam network.";

                throw new ArgumentException(message.TrimIndents());
            }

            SteamProxy.Instance.EstablishPeerToPeerConnection(identity);
        }

        public override void StartListeningForClientConnections() {
            if (ListenSocket.HasValue) return;
            ListenSocket = SteamProxy.Instance.CreateAndOpenListenSocket();
        }

        public override void StopListeningForClientConnections() {
            if (ListenSocket.HasValue) SteamProxy.Instance.CloseListenSocket(ListenSocket.Value);
            ListenSocket = null;
        }

        private void OnConnectionChanged(SteamNetConnectionStatusChangedCallback_t result) {
            var oldState = result.m_eOldState;
            var newState = result.m_info.m_eState;

            if (oldState == k_ESteamNetworkingConnectionState_None && newState == k_ESteamNetworkingConnectionState_Connecting) {
                HandleConnectionIncoming(result);
                return;
            }

            if (oldState != k_ESteamNetworkingConnectionState_Connected && newState == k_ESteamNetworkingConnectionState_Connected) {
                HandleConnectionEstablished(result);
                return;
            }

            var isNewStateErrorState = newState is k_ESteamNetworkingConnectionState_ClosedByPeer
                or k_ESteamNetworkingConnectionState_ProblemDetectedLocally;

            if (oldState == k_ESteamNetworkingConnectionState_Connected && isNewStateErrorState) {
                HandleConnectionDisrupted(result);
                return;
            }

            if (result.m_eOldState is k_ESteamNetworkingConnectionState_Connecting && isNewStateErrorState)
                HandleConnectionEstablishmentFailed(result);
        }

        private void HandleConnectionEstablished(SteamNetConnectionStatusChangedCallback_t result) {
            var identity = (SteamNetworkIdentity)result.m_info.m_identityRemote.GetSteamID();
            Connection?.ReceiveNewActiveNetworkLink(new SteamNetworkLink(identity, result.m_hConn));
        }

        private void HandleConnectionIncoming(SteamNetConnectionStatusChangedCallback_t result) {
            if (Connection?.Role != Roles.Host) return;

            var client = (SteamNetworkIdentity)result.m_info.m_identityRemote.GetSteamID();
            var isSuccess = SteamProxy.Instance.AcceptIncomingPeerToPeerConnection(result.m_hConn);

            if (!isSuccess) {
                var log = $"Failed to accept incoming connection from {client.DisplayName}.";
                NetworkLogger.Warn(log);
            }
        }

        private void HandleConnectionDisrupted(SteamNetConnectionStatusChangedCallback_t result) {
            var identity = (SteamNetworkIdentity)result.m_info.m_identityRemote.GetSteamID();
            Connection?.HandleConnectionDisrupted(identity);
        }

        private void HandleConnectionEstablishmentFailed(SteamNetConnectionStatusChangedCallback_t result) {
            var identity = (SteamNetworkIdentity)result.m_info.m_identityRemote.GetSteamID();
            Connection?.HandleConnectionEstablishmentFailed(identity);
        }
    }
}
