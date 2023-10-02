using MoonriseGames.CloudsAhoyConnect.Connection;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Connection {
    public class NetworkConnectionStrategyTest {

        [Test]
        public void ShouldRetainConnection() {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var connection = new Mock<NetworkConnection>(strategy.Object, null).Object;
            var sut = new Mock<NetworkConnectionStrategy> { CallBase = true };

            Assert.Null(sut.Object.Connection);

            sut.Object.Connection = connection;

            Assert.AreEqual(connection, sut.Object.Connection);
        }
    }
}
