using System.Linq;
using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Extensions;
using MoonriseGames.Connect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Extensions
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
            var cac = SessionFactory.Build(connection.Object);

            connection.Setup(x => x.ActiveClients).Returns(Enumerable.Empty<NetworkIdentity>());

            Session.Instance = cac;

            Assert.False(identity.IsClientConnected());
        }

        [Test]
        public void ShouldCorrectlyDetectIdentityConnected()
        {
            var identity = new Mock<NetworkIdentity>().Object;
            var connection = NetworkConnectionFactory.BuildMock();
            var cac = SessionFactory.Build(connection.Object);

            connection.Setup(x => x.ActiveClients).Returns(new[] { identity });

            Session.Instance = cac;

            Assert.True(identity.IsClientConnected());
        }
    }
}
