using System;
using System.Collections.Generic;
using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Steam;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;
using Steamworks;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Steam
{
    public class SteamNetworkConnectionStrategyTest
    {
        private SteamNetworkIdentity Identity { get; } = new(12);

        private ESteamNetworkingConnectionState[] ConnectionStateValues { get; } = (ESteamNetworkingConnectionState[])Enum.GetValues(typeof(ESteamNetworkingConnectionState));

        private IEnumerable<SteamNetConnectionStatusChangedCallback_t> AllCallbackResults() => ConnectionStateValues.SelectMany(x => ConnectionStateValues, CallbackResult);

        protected SteamNetConnectionStatusChangedCallback_t CallbackResult(ESteamNetworkingConnectionState old, ESteamNetworkingConnectionState current) =>
            new()
            {
                m_eOldState = old,
                m_info = new SteamNetConnectionInfo_t { m_eState = current, m_identityRemote = Identity }
            };

        [Test]
        public void ShouldEstablishPeerToPeerConnection()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            SteamProxy.Instance = proxy.Object;

            sut.EstablishConnectionToHost(Identity);

            proxy.Verify(x => x.EstablishPeerToPeerConnection(Identity), Times.Once);
        }

        [Test]
        public void ShouldThrowIfTargetIdentityIsNotForSteam()
        {
            var sut = new SteamNetworkConnectionStrategy();

            Assert.Throws<ArgumentException>(() => sut.EstablishConnectionToHost(null));
        }

        [Test]
        public void ShouldListenForIncomingConnections()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            SteamProxy.Instance = proxy.Object;

            sut.StartListeningForClientConnections();

            proxy.Verify(x => x.CreateAndOpenListenSocket(), Times.Once);
        }

        [Test]
        public void ShouldListenForIncomingConnectionsOnlyOnce()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            proxy.Setup(x => x.CreateAndOpenListenSocket()).Returns(new HSteamListenSocket());

            SteamProxy.Instance = proxy.Object;

            sut.StartListeningForClientConnections();
            sut.StartListeningForClientConnections();

            proxy.Verify(x => x.CreateAndOpenListenSocket(), Times.Once);
        }

        [Test]
        public void ShouldStopListeningForIncomingConnections()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            proxy.Setup(x => x.CreateAndOpenListenSocket()).Returns(new HSteamListenSocket());

            SteamProxy.Instance = proxy.Object;

            sut.StartListeningForClientConnections();
            sut.StopListeningForClientConnections();

            proxy.Verify(x => x.CloseListenSocket(It.IsAny<HSteamListenSocket>()), Times.Once);
        }

        [Test]
        public void ShouldStopListeningForIncomingConnectionsOnlyOnce()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            proxy.Setup(x => x.CreateAndOpenListenSocket()).Returns(new HSteamListenSocket());

            SteamProxy.Instance = proxy.Object;

            sut.StartListeningForClientConnections();
            sut.StopListeningForClientConnections();
            sut.StopListeningForClientConnections();

            proxy.Verify(x => x.CloseListenSocket(It.IsAny<HSteamListenSocket>()), Times.Once);
        }

        [Test]
        public void ShouldStopListeningForIncomingConnectionsOnlyAfterStartingToListen()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            SteamProxy.Instance = proxy.Object;

            sut.StopListeningForClientConnections();

            proxy.Verify(x => x.CloseListenSocket(It.IsAny<HSteamListenSocket>()), Times.Never);
        }

        [Test]
        public void ShouldListenForIncomingConnectionsAgainAfterStopping()
        {
            var proxy = new Mock<SteamProxy>();
            var sut = new SteamNetworkConnectionStrategy();

            proxy.Setup(x => x.CreateAndOpenListenSocket()).Returns(new HSteamListenSocket());

            SteamProxy.Instance = proxy.Object;

            sut.StartListeningForClientConnections();
            sut.StopListeningForClientConnections();
            sut.StartListeningForClientConnections();

            proxy.Verify(x => x.CreateAndOpenListenSocket(), Times.Exactly(2));
        }

        [Test]
        public void ShouldCorrectlyProcessConnectionEstablishment()
        {
            var callback = null as Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate;
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.CreateConnectionStatusChangedCallback(It.IsAny<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>()))
                .Callback<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>(x =>
                {
                    callback = x;
                });

            var sut = new SteamNetworkConnectionStrategy();
            var connection = NetworkConnectionFactory.BuildMock(sut);

            var expectedInvocations = ConnectionStateValues.Length - 1;

            foreach (var result in AllCallbackResults())
                callback.Invoke(result);

            connection.Verify(x => x.ReceiveNewActiveNetworkLink(It.Is<NetworkLink>(x => Identity.Equals(x.Identity))), Times.Exactly(expectedInvocations));
        }

        [Test]
        public void ShouldAcceptIncomingClientConnections()
        {
            var callback = null as Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate;
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.CreateConnectionStatusChangedCallback(It.IsAny<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>()))
                .Callback<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>(x =>
                {
                    callback = x;
                });

            var sut = new SteamNetworkConnectionStrategy();
            var connection = NetworkConnectionFactory.BuildMock(sut);

            connection.Setup(x => x.Role).Returns(Roles.Host);

            foreach (var result in AllCallbackResults())
                callback.Invoke(result);

            proxy.Verify(x => x.AcceptIncomingPeerToPeerConnection(It.IsAny<HSteamNetConnection>()), Times.Once);
        }

        [Test]
        public void ShouldNotAcceptIncomingClientConnectionsAsClient()
        {
            var callback = null as Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate;
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.CreateConnectionStatusChangedCallback(It.IsAny<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>()))
                .Callback<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>(x =>
                {
                    callback = x;
                });

            var sut = new SteamNetworkConnectionStrategy();
            var connection = NetworkConnectionFactory.BuildMock(sut);

            connection.Setup(x => x.Role).Returns(Roles.Client);

            foreach (var result in AllCallbackResults())
                callback.Invoke(result);

            proxy.Verify(x => x.AcceptIncomingPeerToPeerConnection(It.IsAny<HSteamNetConnection>()), Times.Never);
        }

        [Test]
        public void ShouldCorrectlyProcessConnectionDisruption()
        {
            var callback = null as Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate;
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.CreateConnectionStatusChangedCallback(It.IsAny<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>()))
                .Callback<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>(x =>
                {
                    callback = x;
                });

            var sut = new SteamNetworkConnectionStrategy();
            var connection = NetworkConnectionFactory.BuildMock(sut);

            foreach (var result in AllCallbackResults())
                callback.Invoke(result);

            connection.Verify(x => x.HandleConnectionDisrupted(Identity), Times.Exactly(2));
        }

        [Test]
        public void ShouldCorrectlyProcessFailedConnectionEstablishment()
        {
            var callback = null as Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate;
            var proxy = SteamProxyFactory.BuildMock();

            proxy
                .Setup(x => x.CreateConnectionStatusChangedCallback(It.IsAny<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>()))
                .Callback<Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate>(x =>
                {
                    callback = x;
                });

            var sut = new SteamNetworkConnectionStrategy();
            var connection = NetworkConnectionFactory.BuildMock(sut);

            foreach (var result in AllCallbackResults())
                callback.Invoke(result);

            connection.Verify(x => x.HandleConnectionEstablishmentFailed(Identity), Times.Exactly(2));
        }
    }
}
