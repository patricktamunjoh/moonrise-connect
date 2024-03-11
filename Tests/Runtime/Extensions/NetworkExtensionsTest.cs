using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions
{
    public class NetworkExtensionsTest
    {
        [Test]
        public void ShouldNotThrowIfInstanceIsNull()
        {
            var identity = new Mock<NetworkIdentity>().Object;

            Session.Instance = null;

            Assert.DoesNotThrow(() => identity.IsClientConnected());
        }

        [Test]
        public void ShouldReturnConnectedFalseIfInstanceIsNull()
        {
            var identity = new Mock<NetworkIdentity>().Object;

            Session.Instance = null;

            Assert.False(identity.IsClientConnected());
        }

        [Test]
        public void ShouldCorrectlyDetectIdentityNotConnected()
        {
            var identity = new Mock<NetworkIdentity>().Object;
            var connection = NetworkConnectionFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.ActiveClients).Returns(Enumerable.Empty<NetworkIdentity>());

            Session.Instance = cac;

            Assert.False(identity.IsClientConnected());
        }

        [Test]
        public void ShouldCorrectlyDetectIdentityConnected()
        {
            var identity = new Mock<NetworkIdentity>().Object;
            var connection = NetworkConnectionFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(connection.Object);

            connection.Setup(x => x.ActiveClients).Returns(new[] { identity });

            Session.Instance = cac;

            Assert.True(identity.IsClientConnected());
        }
    }
}
