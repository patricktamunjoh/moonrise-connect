using MoonriseGames.CloudsAhoyConnect.Connection;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Connection
{
    public class NetworkConnectionEventArgsTest
    {
        [Test]
        public void ShouldCreateArgsForConnectionEstablished()
        {
            var sut = NetworkConnectionEventArgs.ForConnectionEstablished();

            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionEstablished, sut.Type);
            Assert.Null(sut.Target);
        }

        [Test]
        public void ShouldCreateArgsForConnectionEstablishmentFailed()
        {
            var sut = NetworkConnectionEventArgs.ForConnectionEstablishmentFailed();

            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionEstablishmentFailed, sut.Type);
            Assert.Null(sut.Target);
        }

        [Test]
        public void ShouldCreateArgsForConnectionLost()
        {
            var sut = NetworkConnectionEventArgs.ForConnectionLost();

            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionLost, sut.Type);
            Assert.Null(sut.Target);
        }

        [Test]
        public void ShouldCreateArgsForConnectionToClientLost()
        {
            var client = new Mock<NetworkIdentity>();
            var sut = NetworkConnectionEventArgs.ForConnectionToClientLost(client.Object);

            Assert.AreEqual(NetworkConnectionEventArgs.Types.ConnectionToClientLost, sut.Type);
            Assert.AreEqual(client.Object, sut.Target);
        }
    }
}
