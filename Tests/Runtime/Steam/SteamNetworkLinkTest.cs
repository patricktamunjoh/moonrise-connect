using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Steam;
using MoonriseGames.Connect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;
using Steamworks;

namespace MoonriseGames.Connect.Tests.Steam
{
    public class SteamNetworkLinkTest
    {
        private byte[] Bytes { get; } = Encoding.ASCII.GetBytes("example");
        private SteamNetworkIdentity Identity { get; } = new(12);

        [Test]
        public void ShouldSendDataOnPeerToPeerConnection()
        {
            var receivedBytes = null as byte[];
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                .Callback<HSteamNetConnection, IntPtr, uint, int>(
                    (_, ptr, length, _) =>
                    {
                        receivedBytes = new byte[length];
                        Marshal.Copy(ptr, receivedBytes, 0, (int)length);
                    }
                )
                .Returns(EResult.k_EResultOK);

            var sut = new SteamNetworkLink(Identity, default);

            sut.Send(Bytes, Transmission.Reliable);

            Assert.True(Bytes.SequenceEqual(receivedBytes));
        }

        [Test]
        public void ShouldSendOnPeerToPeerConnection()
        {
            var connection = new HSteamNetConnection(12);
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                .Returns(EResult.k_EResultOK);

            var sut = new SteamNetworkLink(Identity, connection);

            sut.Send(Bytes, Transmission.Reliable);

            proxy.Verify(x => x.SendMessageOnPeerToPeerConnection(It.Is<HSteamNetConnection>(y => connection.Equals(y)), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()));
        }

        [Test]
        public void ShouldDeallocateMemoryAfterSending()
        {
            var pointer = new IntPtr();
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                .Callback<HSteamNetConnection, IntPtr, uint, int>(
                    (_, ptr, _, _) =>
                    {
                        pointer = ptr;
                    }
                )
                .Returns(EResult.k_EResultOK);

            var sut = new SteamNetworkLink(Identity, default);

            sut.Send(Bytes, Transmission.Reliable);

            proxy.Verify(x => x.ReleaseSteamNetworkMessageData(pointer), Times.Once);
        }

        [Test]
        public void ShouldUseCorrectSendFlagsForReliableTransmission()
        {
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                .Returns(EResult.k_EResultOK);

            var sut = new SteamNetworkLink(Identity, default);

            sut.Send(Bytes, Transmission.Reliable);

            proxy.Verify(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.Is<int>(y => y == 8)));
        }

        [Test]
        public void ShouldUseCorrectSendFlagsForUnreliableTransmission()
        {
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>()))
                .Returns(EResult.k_EResultOK);

            var sut = new SteamNetworkLink(Identity, default);

            sut.Send(Bytes, Transmission.Unreliable);

            proxy.Verify(x => x.SendMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr>(), It.IsAny<uint>(), It.Is<int>(y => y == 0)));
        }

        [Test]
        public void ShouldReceiveBytesFromPeerToPeerConnection()
        {
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.ReceiveMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr[]>()))
                .Callback<HSteamNetConnection, IntPtr[]>(
                    (_, ptr) =>
                    {
                        var message = new SteamNetworkingMessage_t { m_pData = Marshal.AllocHGlobal(Bytes.Length), m_cbSize = Bytes.Length };
                        Marshal.Copy(Bytes, 0, message.m_pData, message.m_cbSize);

                        ptr[0] = Marshal.AllocHGlobal(Marshal.SizeOf(message));
                        Marshal.StructureToPtr(message, ptr[0], false);
                    }
                )
                .Returns(1);

            var sut = new SteamNetworkLink(Identity, default);

            var receivedBytes = sut.Receive();

            Assert.True(Bytes.SequenceEqual(receivedBytes));
        }

        [Test]
        public void ShouldDeallocateMemoryAfterReceiving()
        {
            var pointer = new IntPtr();
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.ReceiveMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr[]>()))
                .Callback<HSteamNetConnection, IntPtr[]>(
                    (_, ptr) =>
                    {
                        var message = new SteamNetworkingMessage_t { m_pData = Marshal.AllocHGlobal(Bytes.Length), m_cbSize = Bytes.Length };
                        Marshal.Copy(Bytes, 0, message.m_pData, message.m_cbSize);

                        pointer = Marshal.AllocHGlobal(Marshal.SizeOf(message));
                        Marshal.StructureToPtr(message, pointer, false);
                        ptr[0] = pointer;
                    }
                )
                .Returns(1);

            var sut = new SteamNetworkLink(Identity, default);

            sut.Receive();

            proxy.Verify(x => x.ReleaseSteamNetworkMessage(pointer), Times.Once);
        }

        [Test]
        public void ShouldReturnNullWhenNoMessageIsReceived()
        {
            var proxy = SteamProxyFactory.BuildMock();

            proxy.Setup(x => x.ReceiveMessageOnPeerToPeerConnection(It.IsAny<HSteamNetConnection>(), It.IsAny<IntPtr[]>())).Returns(0);

            var sut = new SteamNetworkLink(Identity, default);

            var receivedBytes = sut.Receive();

            Assert.Null(receivedBytes);
        }

        [Test]
        public void ShouldClosePeerToPeerConnection()
        {
            var connection = new HSteamNetConnection(12);
            var proxy = SteamProxyFactory.BuildMock();
            var sut = new SteamNetworkLink(Identity, connection);

            sut.Close();

            proxy.Verify(x => x.ClosePeerToPeerConnection(connection), Times.Once);
        }

        [Test]
        public void ShouldClosePeerToPeerConnectionOnlyOnce()
        {
            var connection = new HSteamNetConnection(12);
            var proxy = SteamProxyFactory.BuildMock();
            var sut = new SteamNetworkLink(Identity, connection);

            sut.Close();
            sut.Close();

            proxy.Verify(x => x.ClosePeerToPeerConnection(connection), Times.Once);
        }
    }
}
