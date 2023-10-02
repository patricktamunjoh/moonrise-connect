using System;
using System.Runtime.InteropServices;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Logging;
using Steamworks;

namespace MoonriseGames.CloudsAhoyConnect.Steam {
    internal class SteamNetworkLink : NetworkLink {

        private HSteamNetConnection Socket { get; }
        private IntPtr[] PointerBuffer { get; } = new IntPtr[1];

        public SteamNetworkLink(SteamNetworkIdentity identity, HSteamNetConnection socket) : base(identity) => Socket = socket;

        //TODO: If message delays are an issue, consider using NoNagle or NoDelay send flags
        //https://partner.steamgames.com/doc/api/ISteamNetworkingSockets
        public override void Send(byte[] data, Transmission transmission) {
            var sendFlags = SendFlags(transmission);

            var pointer = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, pointer, data.Length);

            var result = SteamProxy.Instance.SendMessageOnPeerToPeerConnection(Socket, pointer, (uint)data.Length, sendFlags);
            SteamProxy.Instance.ReleaseSteamNetworkMessageData(pointer);

            if (result != EResult.k_EResultOK) {
                var log = $"Received error {result} when sending data to {Identity.DisplayName}.";
                NetworkLogger.Error(log);
            }
        }

        private int SendFlags(Transmission transmission) => transmission switch {
            Transmission.Unreliable => 0,
            Transmission.Reliable   => 8,
            _                       => throw new ArgumentOutOfRangeException()
        };

        //TODO: Investigate poll groups for efficient message retrieval
        //https://partner.steamgames.com/doc/api/ISteamNetworkingSockets#ReceiveMessagesOnConnection
        public override byte[] Receive() {
            var incomingMessageCount = SteamProxy.Instance.ReceiveMessageOnPeerToPeerConnection(Socket, PointerBuffer);
            if (incomingMessageCount < 1) return null;

            var pointer = PointerBuffer[0];
            var message = Marshal.PtrToStructure<SteamNetworkingMessage_t>(pointer);
            var bytes = new byte[message.m_cbSize];

            Marshal.Copy(message.m_pData, bytes, 0, bytes.Length);
            SteamProxy.Instance.ReleaseSteamNetworkMessage(pointer);

            return bytes;
        }

        public override void Close() {
            if (IsActive) SteamProxy.Instance.ClosePeerToPeerConnection(Socket);
            base.Close();
        }
    }
}
