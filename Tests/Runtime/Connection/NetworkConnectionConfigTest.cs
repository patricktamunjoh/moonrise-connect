using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Enums;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Connection
{
    public class NetworkConnectionConfigTest
    {
        [Test]
        public void ShouldConfigureForHost()
        {
            var host = new Mock<NetworkIdentity>();
            var sut = new NetworkConnectionConfig(host.Object);

            Assert.AreEqual(host.Object, sut.Host);
            Assert.AreEqual(Roles.Client, sut.Role);
            Assert.Null(sut.Clients);
        }

        [Test]
        public void ShouldConfigureForClient()
        {
            var clients = new[] { new Mock<NetworkIdentity>().Object };
            var sut = new NetworkConnectionConfig(clients);

            Assert.AreEqual(clients, sut.Clients);
            Assert.AreEqual(Roles.Host, sut.Role);
            Assert.Null(sut.Host);
        }

        [Test]
        public void ShouldProvideDefaultEstablishmentTimeoutForHost()
        {
            var host = new Mock<NetworkIdentity>();
            var sut = new NetworkConnectionConfig(host.Object);

            Assert.Negative(sut.ConnectionEstablishmentTimeoutMs);
        }

        [Test]
        public void ShouldProvideDefaultEstablishmentTimeoutForClient()
        {
            var client = new Mock<NetworkIdentity>();
            var sut = new NetworkConnectionConfig(new[] { client.Object });

            Assert.Negative(sut.ConnectionEstablishmentTimeoutMs);
        }

        [Test]
        public void ShouldProvideEstablishmentTimeoutForHost()
        {
            var host = new Mock<NetworkIdentity>();
            var sut = new NetworkConnectionConfig(host.Object, 12);

            Assert.AreEqual(12, sut.ConnectionEstablishmentTimeoutMs);
        }

        [Test]
        public void ShouldProvideEstablishmentTimeoutForClient()
        {
            var client = new Mock<NetworkIdentity>();
            var sut = new NetworkConnectionConfig(new[] { client.Object }, 12);

            Assert.AreEqual(12, sut.ConnectionEstablishmentTimeoutMs);
        }
    }
}
