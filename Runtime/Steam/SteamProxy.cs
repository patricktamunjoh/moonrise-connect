using System;
using System.Runtime.InteropServices;
using Steamworks;
using ConnectionStatusChangedCallback = Steamworks.Callback<Steamworks.SteamNetConnectionStatusChangedCallback_t>;

namespace MoonriseGames.CloudsAhoyConnect.Steam
{
    internal class SteamProxy
    {
        public static SteamProxy Instance { get; set; } = new();

        public virtual string DisplayName(CSteamID steamId)
        {
            var isLocalIdentity = SteamUser.GetSteamID() == steamId;
            return isLocalIdentity ? SteamFriends.GetPersonaName() : SteamFriends.GetFriendPersonaName(steamId);
        }

        public virtual void EstablishPeerToPeerConnection(SteamNetworkIdentity target)
        {
            var networkingIdentity = (SteamNetworkingIdentity)target;
            SteamNetworkingSockets.ConnectP2P(ref networkingIdentity, 0, 0, null);
        }

        public virtual bool AcceptIncomingPeerToPeerConnection(HSteamNetConnection connection)
        {
            var result = SteamNetworkingSockets.AcceptConnection(connection);
            return result == EResult.k_EResultOK;
        }

        public virtual void ClosePeerToPeerConnection(HSteamNetConnection connection) => SteamNetworkingSockets.CloseConnection(connection, 0, string.Empty, false);

        public virtual HSteamListenSocket CreateAndOpenListenSocket() => SteamNetworkingSockets.CreateListenSocketP2P(0, 0, null);

        public virtual void CloseListenSocket(HSteamListenSocket socket) => SteamNetworkingSockets.CloseListenSocket(socket);

        public virtual int ReceiveMessageOnPeerToPeerConnection(HSteamNetConnection connection, IntPtr[] pointerBuffer) =>
            SteamNetworkingSockets.ReceiveMessagesOnConnection(connection, pointerBuffer, pointerBuffer.Length);

        public virtual EResult SendMessageOnPeerToPeerConnection(HSteamNetConnection connection, IntPtr ptr, uint length, int sendFlags) =>
            SteamNetworkingSockets.SendMessageToConnection(connection, ptr, length, sendFlags, out _);

        public virtual ConnectionStatusChangedCallback CreateConnectionStatusChangedCallback(ConnectionStatusChangedCallback.DispatchDelegate callback) =>
            ConnectionStatusChangedCallback.Create(callback);

        public virtual void ReleaseSteamNetworkMessage(IntPtr pointer) => SteamNetworkingMessage_t.Release(pointer);

        public virtual void ReleaseSteamNetworkMessageData(IntPtr pointer) => Marshal.FreeHGlobal(pointer);
    }
}
