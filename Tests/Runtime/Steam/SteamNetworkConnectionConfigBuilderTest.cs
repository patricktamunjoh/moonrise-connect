using MoonriseGames.CloudsAhoyConnect.Steam;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Steam {
    public class SteamNetworkConnectionConfigBuilderTest {

        [Test]
        public void ShouldBuildForClient() {
            var hostIdentity = new SteamNetworkIdentity(12);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsClient(hostIdentity);

            var config = sut.Build();

            Assert.AreEqual(hostIdentity, config.Host);
            Assert.Null(config.Clients);
        }


        [Test]
        public void ShouldBuildForClientWithLatestValues() {
            var hostIdentity1 = new SteamNetworkIdentity(12);
            var hostIdentity2 = new SteamNetworkIdentity(42);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsClient(hostIdentity1);
            sut.AsClient(hostIdentity2);

            var config = sut.Build();

            Assert.AreEqual(hostIdentity2, config.Host);
            Assert.Null(config.Clients);
        }

        [Test]
        public void ShouldBuildForHost() {
            var clientIdentity = new SteamNetworkIdentity(12);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsHost(clientIdentity);

            var config = sut.Build();

            Assert.NotNull(config.Clients);
            Assert.AreEqual(1, config.Clients.Length);
            Assert.AreEqual(clientIdentity, config.Clients[0]);
            Assert.Null(config.Host);
        }


        [Test]
        public void ShouldBuildForHostWithLatestValues() {
            var clientIdentity = new SteamNetworkIdentity(12);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsHost(clientIdentity);
            sut.AsHost();

            var config = sut.Build();

            Assert.NotNull(config.Clients);
            Assert.Zero(config.Clients.Length);
            Assert.Null(config.Host);
        }

        [Test]
        public void ShouldBuildForHostWithNoClients() {
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsHost();

            var config = sut.Build();

            Assert.NotNull(config.Clients);
            Assert.Zero(config.Clients.Length);
            Assert.Null(config.Host);
        }

        [Test]
        public void ShouldBuildForHostWithMultipleClients() {
            var clientIdentity1 = new SteamNetworkIdentity(12);
            var clientIdentity2 = new SteamNetworkIdentity(60);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsHost(clientIdentity1, clientIdentity2);

            var config = sut.Build();

            Assert.NotNull(config.Clients);
            Assert.AreEqual(2, config.Clients.Length);
            Assert.AreEqual(clientIdentity1, config.Clients[0]);
            Assert.AreEqual(clientIdentity2, config.Clients[1]);
            Assert.Null(config.Host);
        }

        [Test]
        public void ShouldBuildForHostAsDefault() {
            var sut = new SteamNetworkConnectionConfig.Builder();
            var config = sut.Build();

            Assert.NotNull(config.Clients);
            Assert.Zero(config.Clients.Length);
            Assert.Null(config.Host);
        }

        [Test]
        public void ShouldOverwriteClientWithHost() {
            var hostIdentity = new SteamNetworkIdentity(42);
            var clientIdentity = new SteamNetworkIdentity(12);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsClient(hostIdentity);
            sut.AsHost(clientIdentity);

            var config = sut.Build();

            Assert.NotNull(config.Clients);
            Assert.AreEqual(1, config.Clients.Length);
            Assert.AreEqual(clientIdentity, config.Clients[0]);
            Assert.Null(config.Host);
        }

        [Test]
        public void ShouldOverwriteHostWithClient() {
            var hostIdentity = new SteamNetworkIdentity(12);
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.AsHost();
            sut.AsClient(hostIdentity);

            var config = sut.Build();

            Assert.AreEqual(hostIdentity, config.Host);
            Assert.Null(config.Clients);
        }

        [Test]
        public void ShouldUseDefaultEstablishmentTimeout() {
            var sut = new SteamNetworkConnectionConfig.Builder();
            var config = sut.Build();

            Assert.Negative(config.ConnectionEstablishmentTimeoutMs);
        }

        [Test]
        public void ShouldBuildWithEstablishmentTimeout() {
            var sut = new SteamNetworkConnectionConfig.Builder();

            sut.WithConnectionEstablishmentTimeout(30);

            var config = sut.Build();

            Assert.AreEqual(30, config.ConnectionEstablishmentTimeoutMs);
        }

        [Test]
        public void ShouldAlwaysBuildNewInstance() {
            var sut = new SteamNetworkConnectionConfig.Builder();
            var config1 = sut.Build();
            var config2 = sut.Build();

            Assert.AreNotSame(config1, config2);
        }
    }
}
