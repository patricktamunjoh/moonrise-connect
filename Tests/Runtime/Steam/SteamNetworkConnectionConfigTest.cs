using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Steam;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Steam
{
    public class SteamNetworkConnectionConfigTest
    {
        [Test]
        public void ShouldConfigureForHost()
        {
            var host = new SteamNetworkIdentity(12);
            var sut = new SteamNetworkConnectionConfig(host);

            Assert.AreEqual(host, sut.Host);
            Assert.AreEqual(Roles.Client, sut.Role);
            Assert.Null(sut.Clients);
        }

        [Test]
        public void ShouldConfigureForClient()
        {
            var clients = new[] { new SteamNetworkIdentity(12) };
            var sut = new NetworkConnectionConfig(clients);

            Assert.AreEqual(clients, sut.Clients);
            Assert.AreEqual(Roles.Host, sut.Role);
            Assert.Null(sut.Host);
        }
    }
}
