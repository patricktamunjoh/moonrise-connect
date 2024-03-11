using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Connection
{
    public class NetworkLinkTest
    {
        [Test]
        public void ShouldRetainProperties()
        {
            var identity = new Mock<NetworkIdentity>().Object;
            var sut = new Mock<NetworkLink>(identity).CallingBase();

            Assert.AreEqual(identity, sut.Object.Identity);
        }

        [Test]
        public void ShouldInitializeAsActive()
        {
            var identity = new Mock<NetworkIdentity>().Object;
            var sut = new Mock<NetworkLink>(identity).CallingBase();

            Assert.True(sut.Object.IsActive);
        }

        [Test]
        public void ShouldNotBeActiveAfterClose()
        {
            var identity = new Mock<NetworkIdentity>().Object;
            var sut = new Mock<NetworkLink>(identity).CallingBase();

            sut.Object.Close();

            Assert.False(sut.Object.IsActive);
        }
    }
}
