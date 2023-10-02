using System;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoonriseGames.CloudsAhoyConnect.Tests {
    public class CloudsAhoyConnectTest {

        [Test]
        public void ShouldRetainProperties() {
            var connection = NetworkConnectionFactory.BuildMock().Object;
            var queue = NetworkFunctionQueueFactory.BuildMock().Object;
            var registry = new Mock<NetworkFunctionRegistry>().Object;
            var emitter = NetworkFunctionEmitterFactory.BuildMock().Object;

            var sut = new CloudsAhoyConnect(connection, queue, registry, emitter);

            Assert.AreEqual(connection, sut.Connection);
            Assert.AreEqual(queue, sut.Queue);
            Assert.AreEqual(registry, sut.Registry);
            Assert.AreEqual(emitter, sut.Emitter);
        }

        [Test]
        public void ShouldProvideRole() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.Connectivity).Returns(Connectivity.Connected);
            connection.Setup(x => x.Role).Returns(Roles.Host);

            Assert.AreEqual(Roles.Host, sut.Role.GetValueOrDefault());
        }

        [Test]
        public void ShouldNotProvideRoleWhileDisconnected() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.Connectivity).Returns(Connectivity.Disconnected);
            connection.Setup(x => x.Role).Returns(Roles.Host);

            Assert.False(sut.Role.HasValue);
        }

        [Test]
        public void ShouldProvideConnectivity() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.Connectivity).Returns(Connectivity.Connecting);

            Assert.AreEqual(Connectivity.Connecting, sut.Connectivity);
        }

        [Test]
        public void ShouldProvideClients() {
            var clients = new[] { new Mock<NetworkIdentity>().Object };
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.Clients).Returns(clients);

            Assert.AreEqual(clients, sut.Clients);
        }

        [Test]
        public void ShouldProvideConnectedClients() {
            var clients = new[] { new Mock<NetworkIdentity>().Object };
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.ActiveClients).Returns(clients);

            Assert.AreEqual(clients, sut.ConnectedClients);
        }

        [Test]
        public void ShouldProvideClientCount() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.ClientCount).Returns(12);

            Assert.AreEqual(12, sut.ClientCount);
        }

        [Test]
        public void ShouldProvideConnectedClientCount() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.ActiveClientCount).Returns(12);

            Assert.AreEqual(12, sut.ConnectedClientsCount);
        }

        [Test]
        public void ShouldForwardNetworkConnectionEvents() {
            var receivedSender = null as object;
            var receivedArgs = null as NetworkConnectionEventArgs;
            var expectedArgs = NetworkConnectionEventArgs.ForConnectionLost();

            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            sut.OnNetworkConnectionChanged += (sender, eventArgs) => {
                receivedSender = sender;
                receivedArgs = eventArgs;
            };

            connection.Raise(x => x.OnNetworkConnectionChanged += null, this, expectedArgs);

            Assert.AreEqual(expectedArgs, receivedArgs);
            Assert.AreEqual(sut, receivedSender);
        }

        [Test]
        public void ShouldForwardConnectionEstablishment() {
            var config = new NetworkConnectionConfig(new Mock<NetworkIdentity>().Object);
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            sut.EstablishConnection(config);

            connection.Verify(x => x.Establish(config));
        }

        [Test]
        public void ShouldForwardConnectionPolling() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            sut.PollConnection();

            connection.Verify(x => x.Poll());
        }

        [Test]
        public void ShouldForwardConnectionDropping() {
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(connection.Object);

            sut.DropConnection();

            connection.Verify(x => x.Drop());
        }

        [Test]
        public void ShouldForwardResetting() {
            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = CloudsAhoyConnectFactory.Build(registry.Object);

            sut.Reset();

            registry.Verify(x => x.ClearRegistrationsAndResetCounter());
        }

        [Test]
        public void ShouldForwardNetworkCallProcessing() {
            var queue = NetworkFunctionQueueFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(queue.Object);

            sut.ProcessQueuedNetworkFunctionCalls();

            queue.Verify(x => x.ProcessQueuedElements());
        }

        [Test]
        public void ShouldForwardRegistrationOfAllGameObjects() {
            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = CloudsAhoyConnectFactory.Build(registry.Object);

            sut.RegisterAllGameObjects();

            registry.Verify(x => x.RegisterAllGameObjects());
        }

        [Test]
        public void ShouldForwardClearingOfAllRegistrations() {
            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = CloudsAhoyConnectFactory.Build(registry.Object);

            sut.ClearAllObjectRegistrations();

            registry.Verify(x => x.ClearRegistrations());
        }

        [Test]
        public void ShouldCorrectlyStartRecordingSnapshot() {
            var registry = new Mock<NetworkFunctionRegistry>();
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(registry.Object, connection.Object);

            registry.SetupProperty(x => x.Snapshot);
            connection.SetupProperty(x => x.Snapshot);

            sut.StartRecordingSnapshot();

            Assert.AreEqual(registry.Object.Snapshot, connection.Object.Snapshot);
            Assert.NotNull(registry.Object.Snapshot);
        }

        [Test]
        public void ShouldCorrectlyRestartRecordingSnapshot() {
            var registry = new Mock<NetworkFunctionRegistry>();
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(registry.Object, connection.Object);

            registry.SetupProperty(x => x.Snapshot);
            connection.SetupProperty(x => x.Snapshot);

            sut.StartRecordingSnapshot();

            var snapshot = registry.Object.Snapshot;

            sut.StartRecordingSnapshot();

            Assert.AreNotEqual(snapshot, registry.Object.Snapshot);
            Assert.AreNotEqual(snapshot, connection.Object.Snapshot);
        }

        [Test]
        public void ShouldCorrectlyStopRecordingSnapshot() {
            var registry = new Mock<NetworkFunctionRegistry>();
            var connection = NetworkConnectionFactory.BuildMock();
            var sut = CloudsAhoyConnectFactory.Build(registry.Object, connection.Object);

            registry.SetupProperty(x => x.Snapshot);
            connection.SetupProperty(x => x.Snapshot);

            sut.StartRecordingSnapshot();

            var snapshot = registry.Object.Snapshot;

            Assert.AreEqual(snapshot, sut.StopRecordingAndCollectSnapshot());
            Assert.Null(registry.Object.Snapshot);
            Assert.Null(connection.Object.Snapshot);
        }

        [Test]
        public void ShouldThrowIfStoppingRecordingBeforeStarting() {
            var sut = CloudsAhoyConnectFactory.Build();

            Assert.Throws<InvalidOperationException>(() => sut.StopRecordingAndCollectSnapshot());
        }
    }
}
