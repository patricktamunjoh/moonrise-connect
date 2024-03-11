using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using MoonriseGames.CloudsAhoyConnect.Logging;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Connection
{
    public class NetworkConnectionTest
    {
        [Test]
        public void ShouldInitializeAsDisconnected()
        {
            var sut = NetworkConnectionFactory.Build();

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
        }

        [Test]
        public void ShouldProvideHostRoleAsDefault()
        {
            var sut = NetworkConnectionFactory.Build();

            Assert.AreEqual(Roles.Host, sut.Role);
        }

        [Test]
        public void ShouldProvideHostRole()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));

            Assert.AreEqual(Roles.Host, sut.Role);
        }

        [Test]
        public void ShouldProvideHostRoleAfterDisconnect()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.Drop();

            Assert.AreEqual(Roles.Host, sut.Role);
        }

        [Test]
        public void ShouldProvideClientRole()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));

            Assert.AreEqual(Roles.Client, sut.Role);
        }

        [Test]
        public void ShouldProvideAllClients()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var clients = new[] { clientIdentity1, clientIdentity2 };
            var config = new NetworkConnectionConfig(clients);

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);

            Assert.True(clients.SequenceEqual(sut.Clients));
        }

        [Test]
        public void ShouldProvideAllActiveClients()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var clients = new[] { clientIdentity };
            var config = new NetworkConnectionConfig(clients);

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);
            sut.ReceiveNewActiveNetworkLink(new Mock<NetworkLink>(clientIdentity).Object);

            Assert.True(clients.SequenceEqual(sut.ActiveClients));
        }

        [Test]
        public void ShouldProvideClientCount()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var config = new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 });

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);

            Assert.AreEqual(2, sut.ClientCount);
        }

        [Test]
        public void ShouldProvideActiveClientCount()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var config = new NetworkConnectionConfig(new[] { clientIdentity });

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);
            sut.ReceiveNewActiveNetworkLink(new Mock<NetworkLink>(clientIdentity).Object);

            Assert.AreEqual(1, sut.ActiveClientCount);
        }

        [Test]
        public void ShouldRegisterWithStrategy()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var registry = new NetworkFunctionRegistry();
            var queue = new NetworkFunctionQueue(registry);

            var sut = new Mock<NetworkConnection>(strategy.Object, queue);

            strategy.VerifySet(x => x.Connection = sut.Object);
        }

        [Test]
        public void ShouldDropConnectionBeforeEstablishment()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity).CallingBase();
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);

            Assert.True(link.Object.IsActive);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));

            Assert.False(link.Object.IsActive);
        }

        [Test]
        public void ShouldBeConnectionWhenEstablishingConnection()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldInstantlyEstablishConnectionForSessionWithoutClients()
        {
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(Array.Empty<NetworkIdentity>()));

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public void ShouldListenForClientsWhenEstablishingConnectionAsHost()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build(strategy.Object);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));

            strategy.Verify(x => x.StartListeningForClientConnections(), Times.Once);
        }

        [Test]
        public void ShouldNotListenForClientsWhenEstablishingConnectionAsClient()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build(strategy.Object);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));

            strategy.Verify(x => x.StartListeningForClientConnections(), Times.Never);
        }

        [Test]
        public void ShouldNotStopListeningForClientsWhenEstablishmentIsCompleted()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity);
            var sut = NetworkConnectionFactory.Build(strategy.Object);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link.Object);

            strategy.Verify(x => x.StopListeningForClientConnections(), Times.Never);
        }

        [Test]
        public void ShouldConnectToHostWhenEstablishingConnectionAsClient()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build(strategy.Object);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));

            strategy.Verify(x => x.EstablishConnectionToHost(hostIdentity), Times.Once);
        }

        [Test]
        public void ShouldNotConnectToHostWhenEstablishingConnectionAsHost()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build(strategy.Object);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));

            strategy.Verify(x => x.EstablishConnectionToHost(It.IsAny<NetworkIdentity>()), Times.Never);
        }

        [Test]
        public async Task ShouldAbortEstablishmentAfterTimeout()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var config = new NetworkConnectionConfig(hostIdentity, 2);
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);
            await Task.Delay(100);

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
        }

        [Test]
        public async Task ShouldCancelEstablishmentTimeoutAfterSuccessfulEstablishment()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var config = new NetworkConnectionConfig(hostIdentity, 100);
            var link = new Mock<NetworkLink>(hostIdentity);
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);
            sut.ReceiveNewActiveNetworkLink(link.Object);
            await Task.Delay(config.ConnectionEstablishmentTimeoutMs);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public async Task ShouldCancelEstablishmentTimeoutAfterDroppingConnection()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var config1 = new NetworkConnectionConfig(hostIdentity, 100);
            var config2 = new NetworkConnectionConfig(hostIdentity);
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config1);

            sut.Drop();
            sut.Establish(config2);

            await Task.Delay(config1.ConnectionEstablishmentTimeoutMs / 2);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldCloseAllClientLinksWhenDroppingConnection()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1).CallingBase();
            var link2 = new Mock<NetworkLink>(clientIdentity2).CallingBase();

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1.Object);
            sut.ReceiveNewActiveNetworkLink(link2.Object);

            sut.Drop();

            Assert.False(link1.Object.IsActive);
            Assert.False(link2.Object.IsActive);
        }

        [Test]
        public void ShouldCloseAllClientLinksWhenDroppingConnectionDuringEstablishment()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity1).CallingBase();

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link.Object);

            sut.Drop();

            Assert.False(link.Object.IsActive);
        }

        [Test]
        public void ShouldCloseHostLinkWhenDroppingConnection()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity).CallingBase();

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);

            sut.Drop();

            Assert.False(link.Object.IsActive);
        }

        [Test]
        public void ShouldClearConnectionConfigWhenDroppingConnection()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var config = new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 });

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(config);
            sut.Drop();

            Assert.IsEmpty(sut.Clients);
        }

        [Test]
        public void ShouldTransitionToDisconnectedAfterDroppingConnection()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.Drop();

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
        }

        [Test]
        public void ShouldStopListeningForClientsWhenDroppingConnection()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build(strategy.Object);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.Drop();

            strategy.Verify(x => x.StopListeningForClientConnections(), Times.Once);
        }

        [Test]
        public void ShouldOnlyPollWhenConnected()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity1);

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link.Object);

            sut.Poll();

            link.Verify(x => x.Receive(), Times.Never);
        }

        [Test]
        public void ShouldPostIncomingNetworkCallsFromHostToQueue()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var queue = NetworkFunctionQueueFactory.BuildMock();
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Reliable);

            var sut = NetworkConnectionFactory.Build(queue.Object);

            link.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Poll();

            queue.Verify(x => x.EnqueueCall(It.Is<NetworkFunctionCall>(y => y.ObjectId == call.ObjectId && call.FunctionId.Equals(y.FunctionId)), Roles.Client, false), Times.Once);
        }

        [Test]
        public void ShouldPostIncomingNetworkCallsFromClientToQueue()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity);
            var queue = NetworkFunctionQueueFactory.BuildMock();
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Reliable);

            var sut = NetworkConnectionFactory.Build(queue.Object);

            link.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Poll();

            queue.Verify(x => x.EnqueueCall(It.Is<NetworkFunctionCall>(y => y.ObjectId == call.ObjectId && call.FunctionId.Equals(y.FunctionId)), Roles.Host, false), Times.Once);
        }

        [Test]
        public void ShouldPollAllClientNetworkLinks()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1);
            var link2 = new Mock<NetworkLink>(clientIdentity2);

            var sut = NetworkConnectionFactory.Build();

            link1.Setup(x => x.Receive()).Returns(null as byte[]);
            link2.Setup(x => x.Receive()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1.Object);
            sut.ReceiveNewActiveNetworkLink(link2.Object);

            sut.Poll();

            link1.Verify(x => x.Receive(), Times.Once);
            link2.Verify(x => x.Receive(), Times.Once);
        }

        [Test]
        public void ShouldPollHostNetworkLink()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var sut = NetworkConnectionFactory.Build();

            link.Setup(x => x.Receive()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Poll();

            link.Verify(x => x.Receive(), Times.Once);
        }

        [Test]
        public void ShouldPollNetworkLinkUntilAllMessagesAreProcessed()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Reliable);

            var sut = NetworkConnectionFactory.Build();

            link.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(call.ToBytes()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Poll();

            link.Verify(x => x.Receive(), Times.Exactly(3));
        }

        [Test]
        public void ShouldForwardIncomingNetworkCallsToClientsAsHost()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1);
            var link2 = new Mock<NetworkLink>(clientIdentity2);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Reliable);

            var sut = NetworkConnectionFactory.Build();

            link1.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(null as byte[]);
            link2.Setup(x => x.Receive()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1.Object);
            sut.ReceiveNewActiveNetworkLink(link2.Object);
            sut.Poll();

            link2.Verify(x => x.Send(It.Is<byte[]>(y => call.ToBytes().SequenceEqual(y)), Transmission.Reliable), Times.Once);
        }

        [Test]
        public void ShouldForwardIncomingNetworkCallsWithSameTransmission()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1);
            var link2 = new Mock<NetworkLink>(clientIdentity2);

            foreach (var transmission in typeof(Transmission).EnumValues<Transmission>())
            {
                var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), transmission);

                var sut = NetworkConnectionFactory.Build();

                link1.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(null as byte[]);
                link2.Setup(x => x.Receive()).Returns(null as byte[]);

                sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
                sut.ReceiveNewActiveNetworkLink(link1.Object);
                sut.ReceiveNewActiveNetworkLink(link2.Object);
                sut.Poll();

                link2.Verify(x => x.Send(It.Is<byte[]>(y => call.ToBytes().SequenceEqual(y)), transmission), Times.Once);
            }
        }

        [Test]
        public void ShouldNotForwardIncomingNetworkCallsToSender()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            link.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Poll();

            link.Verify(x => x.Send(It.IsAny<byte[]>(), It.IsAny<Transmission>()), Times.Never);
        }

        [Test]
        public void ShouldNotForwardIncomingNetworkCallsAsHost()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            link.SetupSequence(x => x.Receive()).Returns(call.ToBytes()).Returns(null as byte[]);

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Poll();

            link.Verify(x => x.Send(It.IsAny<byte[]>(), It.IsAny<Transmission>()), Times.Never);
        }

        [Test]
        public void ShouldSendMessagesToHostAsClient()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Send(call);

            link.Verify(x => x.Send(It.Is<byte[]>(y => call.ToBytes().SequenceEqual(y)), Transmission.Unreliable), Times.Once);
        }

        [Test]
        public void ShouldBroadcastMessagesToClientsAsHost()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1);
            var link2 = new Mock<NetworkLink>(clientIdentity2);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1.Object);
            sut.ReceiveNewActiveNetworkLink(link2.Object);
            sut.Send(call);

            link1.Verify(x => x.Send(It.Is<byte[]>(y => call.ToBytes().SequenceEqual(y)), Transmission.Unreliable), Times.Once);
            link2.Verify(x => x.Send(It.Is<byte[]>(y => call.ToBytes().SequenceEqual(y)), Transmission.Unreliable), Times.Once);
        }

        [Test]
        public void ShouldOnlySendWhenConnected()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity1);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Send(call);

            link.Verify(x => x.Send(It.IsAny<byte[]>(), It.IsAny<Transmission>()), Times.Never);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingHostLinkWhenNotConnecting()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(hostIdentity).CallingBase().Object;
            var link2 = new Mock<NetworkLink>(hostIdentity).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link1);
            sut.ReceiveNewActiveNetworkLink(link2);

            Assert.True(link1.IsActive);
            Assert.False(link2.IsActive);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingHostLinkWithWrongIdentity()
        {
            var hostIdentityCorrect = new Mock<NetworkIdentity>().Object;
            var hostIdentityWrong = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentityWrong).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentityCorrect));
            sut.ReceiveNewActiveNetworkLink(link);

            Assert.False(link.IsActive);
            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingHostLinkWhileConfiguredAsHost()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);

            Assert.False(link.IsActive);
            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldFinalizeEstablishmentAfterReceivingHostLink()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingClientLinkWhenNotConnecting()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.ReceiveNewActiveNetworkLink(link);

            Assert.False(link.IsActive);
            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingClientLinkWithUnknownIdentity()
        {
            var clientIdentityKnown = new Mock<NetworkIdentity>().Object;
            var clientIdentityUnknown = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentityUnknown).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentityKnown }));
            sut.ReceiveNewActiveNetworkLink(link);

            Assert.False(link.IsActive);
            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingClientLinkWhileConfiguredAsClient()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link);

            Assert.False(link.IsActive);
            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldCloseAndIgnoreLinkWhenReceivingClientLinkForAlreadyConnectedClient()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;
            var link2 = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1);
            sut.ReceiveNewActiveNetworkLink(link2);

            Assert.True(link1.IsActive);
            Assert.False(link2.IsActive);
            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldFinalizeEstablishmentAfterReceivingAllClientLinks()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public void ShouldNotFinalizeEstablishmentBeforeReceivingAllClientLinks()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldNotFinalizeEstablishmentIfClientLinksBecomeInactive()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;
            var link2 = new Mock<NetworkLink>(clientIdentity2).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1);

            link1.Close();
            sut.ReceiveNewActiveNetworkLink(link2);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldFinalizeEstablishmentIfInactiveClientLinkIsReplaced()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link1 = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;
            var link2 = new Mock<NetworkLink>(clientIdentity2).CallingBase().Object;
            var link3 = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;

            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1);

            link1.Close();
            sut.ReceiveNewActiveNetworkLink(link2);
            sut.ReceiveNewActiveNetworkLink(link3);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public void ShouldDropConnectionWhenConnectionDisruptedToHost()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(hostIdentity);

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
            Assert.False(link.IsActive);
        }

        [Test]
        public void ShouldNotCancelEstablishmentWhenConnectionDisruptedOtherThanToHost()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var otherIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.HandleConnectionDisrupted(otherIdentity);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldNotDropConnectionWhenConnectionDisruptedOtherThanToHost()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var otherIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(otherIdentity);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
            Assert.True(link.IsActive);
        }

        [Test]
        public void ShouldCloseLinkWhenConnectionDisruptedToClient()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(clientIdentity);

            Assert.False(link.IsActive);
        }

        [Test]
        public void ShouldAbortConnectionEstablishmentWhenConnectionDisruptedToClient()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(clientIdentity1);

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
        }

        [Test]
        public void ShouldIgnoreDisruptedConnectionsToUnknownClients()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity2 }));
            sut.HandleConnectionDisrupted(clientIdentity1);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldNotDropConnectionWhenConnectionDisruptedToClient()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(clientIdentity);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public void ShouldIgnoreFailedEstablishmentWhenNotConnecting()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionEstablishmentFailed(clientIdentity);

            Assert.AreEqual(Connectivity.Connected, sut.Connectivity);
        }

        [Test]
        public void ShouldIgnoreFailedEstablishmentToUnknownClients()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity2 }));
            sut.HandleConnectionEstablishmentFailed(clientIdentity1);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldDropConnectionWhenEstablishmentFailedToClient()
        {
            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity1).CallingBase().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionEstablishmentFailed(clientIdentity2);

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
            Assert.False(link.IsActive);
        }

        [Test]
        public void ShouldDropConnectionWhenEstablishmentFailedToHost()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var sut = NetworkConnectionFactory.Build();

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.HandleConnectionEstablishmentFailed();

            Assert.AreEqual(Connectivity.Disconnected, sut.Connectivity);
        }

        [Test]
        public void ShouldNotifyWhenConnectionEstablishmentIsSuccessful()
        {
            var args = new List<NetworkConnectionEventArgs>();
            var sut = NetworkConnectionFactory.Build();

            sut.OnNetworkConnectionChanged += (_, eventArgs) => args.Add(eventArgs);
            sut.Establish(new NetworkConnectionConfig(Array.Empty<NetworkIdentity>()));

            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionEstablished, args[0].Type);
        }

        [Test]
        public void ShouldNotifyWhenConnectionEstablishmentFailed()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var args = new List<NetworkConnectionEventArgs>();
            var sut = NetworkConnectionFactory.Build();

            sut.OnNetworkConnectionChanged += (_, eventArgs) => args.Add(eventArgs);
            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.HandleConnectionEstablishmentFailed(clientIdentity);

            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionEstablishmentFailed, args[0].Type);
        }

        [Test]
        public void ShouldNotifyWhenConnectionIsLost()
        {
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity).CallingBase().Object;
            var args = new List<NetworkConnectionEventArgs>();
            var sut = NetworkConnectionFactory.Build();

            sut.OnNetworkConnectionChanged += (_, eventArgs) => args.Add(eventArgs);
            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(hostIdentity);

            Assert.AreEqual(2, args.Count);
            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionLost, args[1].Type);
        }

        [Test]
        public void ShouldNotifyWhenConnectionIsDropped()
        {
            var args = new List<NetworkConnectionEventArgs>();
            var sut = NetworkConnectionFactory.Build();

            sut.OnNetworkConnectionChanged += (_, eventArgs) => args.Add(eventArgs);
            sut.Establish(new NetworkConnectionConfig(Array.Empty<NetworkIdentity>()));
            sut.Drop();

            Assert.AreEqual(2, args.Count);
            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionLost, args[1].Type);
        }

        [Test]
        public void ShouldNotifyWhenConnectionToClientIsLost()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;
            var args = new List<NetworkConnectionEventArgs>();
            var sut = NetworkConnectionFactory.Build();

            sut.OnNetworkConnectionChanged += (_, eventArgs) => args.Add(eventArgs);
            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(clientIdentity);

            Assert.AreEqual(2, args.Count);
            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionToClientLost, args[1].Type);
            Assert.AreEqual(clientIdentity, args[1].Target);
        }

        [Test]
        public void ShouldNotifyWhenConnectionToClientIsLostOnlyOnce()
        {
            var clientIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(clientIdentity).CallingBase().Object;
            var args = new List<NetworkConnectionEventArgs>();
            var sut = NetworkConnectionFactory.Build();

            sut.OnNetworkConnectionChanged += (_, eventArgs) => args.Add(eventArgs);
            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity }));
            sut.ReceiveNewActiveNetworkLink(link);
            sut.HandleConnectionDisrupted(clientIdentity);
            sut.HandleConnectionDisrupted(clientIdentity);

            Assert.AreEqual(2, args.Count);
        }

        [Test]
        public void ShouldRecordNetworkEventsToSnapshot()
        {
            var snapshot = new Mock<Snapshot>();
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            sut.Snapshot = snapshot.Object;

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Drop();

            snapshot.Verify(x => x.RecordNetworkEvent(It.Is<NetworkConnectionEventArgs>(args => args.Type == NetworkConnectionEventArgs.Types.ConnectionEstablished)));
            snapshot.Verify(x => x.RecordNetworkEvent(It.Is<NetworkConnectionEventArgs>(args => args.Type == NetworkConnectionEventArgs.Types.ConnectionLost)));
        }

        [Test]
        public void ShouldRecordOutgoingMessagesToSnapshot()
        {
            var snapshot = new Mock<Snapshot>();
            var hostIdentity = new Mock<NetworkIdentity>().Object;
            var link = new Mock<NetworkLink>(hostIdentity);
            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);

            var sut = NetworkConnectionFactory.Build();

            sut.Snapshot = snapshot.Object;

            sut.Establish(new NetworkConnectionConfig(hostIdentity));
            sut.ReceiveNewActiveNetworkLink(link.Object);
            sut.Send(call);

            snapshot.Verify(x => x.RecordOutgoingNetworkCall(hostIdentity, call));
        }

        [Test]
        public void ShouldRecordOutgoingBroadcastsToSnapshot()
        {
            var snapshot = new Mock<Snapshot>();

            var clientIdentity1 = new Mock<NetworkIdentity>().Object;
            var clientIdentity2 = new Mock<NetworkIdentity>().Object;

            var link1 = new Mock<NetworkLink>(clientIdentity1);
            var link2 = new Mock<NetworkLink>(clientIdentity2);

            var call = new NetworkFunctionCall(12, NetworkHashing.Hash("example"), Transmission.Unreliable);
            var sut = NetworkConnectionFactory.Build();

            sut.Snapshot = snapshot.Object;

            sut.Establish(new NetworkConnectionConfig(new[] { clientIdentity1, clientIdentity2 }));
            sut.ReceiveNewActiveNetworkLink(link1.Object);
            sut.ReceiveNewActiveNetworkLink(link2.Object);
            sut.Send(call);

            snapshot.Verify(x => x.RecordOutgoingNetworkCall(clientIdentity1, call));
            snapshot.Verify(x => x.RecordOutgoingNetworkCall(clientIdentity2, call));
        }
    }
}
